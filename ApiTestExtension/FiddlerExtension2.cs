using System;
using System.Collections.Generic;
using System.Text;
using Fiddler;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.IO;
using ApiTestExtension2.DataStructure.XML;
using ApiTestExtension2.DataStructure.Json;
using ApiTestExtension2.DataStructure.Utils;
using ApiTestExtension2.DataStructure.Matcher;
using ApiTestExtension2.Dialogs;

[assembly: Fiddler.RequiredVersion("2.4.9.3")]
namespace ApiTestExtension2
{
    public class FiddlerExtension2 : IAutoTamper3, IDisposable
    {
        private MenuItem _mnuApiAutoTest;
        private MenuItem _miApiAutoTestEnabled;
        private MenuItem _miOpenRulesDialog;
        private MenuItem _miLogEnabled;
        private MenuItem _miLoadXML;
        private MenuItem _miCoverageTest;
        private FolderBrowserDialog fbdLoadXML = new FolderBrowserDialog();

        private bool bAutotestEnabled = false;
        private bool bLogEnabled = false;

        public static Dictionary<String, ProductInXML> products = new Dictionary<String, ProductInXML>();

        #region Settings
        public static class Settings
        {
            private const string prefix = "ApiTestExtension2.";
            public static bool verboseLogging
            {
                get
                {
                    return Fiddler.FiddlerApplication.Prefs.GetBoolPref(prefix + "verboseLogging", false);
                }
                set
                {
                    Fiddler.FiddlerApplication.Prefs.SetBoolPref(prefix + "verboseLogging", value);
                }
            }

            public static String xmlFilePath
            {
                get
                {
                    return Fiddler.FiddlerApplication.Prefs.GetStringPref(prefix + "xmlFilePath", "");
                }
                set
                {
                    Fiddler.FiddlerApplication.Prefs.SetStringPref(prefix + "xmlFilePath", value);
                }
            }
        }
        #endregion

        #region InitMenu
        private void EnableMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            MenuItem oSender = (sender as MenuItem);
            oSender.Checked = !oSender.Checked;

            bAutotestEnabled = _miApiAutoTestEnabled.Checked;
            if (bAutotestEnabled)
            {
                _mnuApiAutoTest.Text = "&APIAutoTest-ON";
            }
            else
            {
                _mnuApiAutoTest.Text = "&APIAutoTest-OFF";
            }
        }

        private void EnableLog_CheckedChanged(object sender, EventArgs e)
        {
            MenuItem oSender = (sender as MenuItem);
            oSender.Checked = !oSender.Checked;

            bLogEnabled = _miLogEnabled.Checked;
            Settings.verboseLogging = bLogEnabled;
        }

        private void LoadXML_Click(object sender, EventArgs e)
        {
            if (fbdLoadXML.ShowDialog() == DialogResult.OK)
            {
                String xmlFolderPath = fbdLoadXML.SelectedPath;
                DirectoryInfo xmlFolder = new DirectoryInfo(xmlFolderPath);
                try
                {
                    foreach (FileInfo fi in xmlFolder.GetFiles())
                    {
                        if (fi.FullName.EndsWith(".xml"))
                        {
                            ProductInXML product = new ProductInXML(fi);
                            products[product.productName] = product;
                            log("\n*****************************************************\n" + product.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, ex.GetType().ToString());
                }
            }
        }

        private void CoverageTest_Click(object sender, EventArgs e)
        {
            Coverage coverageDlg = new Coverage();
            coverageDlg.ShowDialog(FiddlerApplication.UI);
        }

        void initMenus()
        {
            this.fbdLoadXML.Description = "请选择要加载的XML文件夹路径";
            this.fbdLoadXML.RootFolder = System.Environment.SpecialFolder.MyDocuments;

            _mnuApiAutoTest = new MenuItem();
            _miApiAutoTestEnabled = new MenuItem();
            _miOpenRulesDialog = new MenuItem();
            _miLogEnabled = new MenuItem();
            _miLoadXML = new MenuItem();
            _miCoverageTest = new MenuItem();

            _mnuApiAutoTest.Text = "&APIAutoTestPlatform";
            _mnuApiAutoTest.MenuItems.Add(0, _miApiAutoTestEnabled);
            _mnuApiAutoTest.MenuItems.Add(1, _miLoadXML);
            _mnuApiAutoTest.MenuItems.Add(2, _miLogEnabled);
            _mnuApiAutoTest.MenuItems.Add(3, _miOpenRulesDialog);
            _mnuApiAutoTest.MenuItems.Add(4, _miCoverageTest);

            _miApiAutoTestEnabled.Text = "Enabled";
            _miLogEnabled.Text = "Log Enabled";
            _miOpenRulesDialog.Text = "Config Autotest Rules...";

            _miLoadXML.Text = "加载API-XML文件";
            _miCoverageTest.Text = "查看API覆盖率";
            _miLoadXML.Click += new EventHandler(LoadXML_Click);
            _miLogEnabled.Click += new EventHandler(EnableLog_CheckedChanged);
            _miApiAutoTestEnabled.Click += new EventHandler(EnableMenuItem_CheckedChanged);
            _miCoverageTest.Click += new EventHandler(CoverageTest_Click);
        }

        public static List<FileInfo> getXMLFiles(String xmlFolderPath)
        {
            List<FileInfo> xmlFiles = new List<FileInfo>();
            DirectoryInfo xmlFolder = new DirectoryInfo(xmlFolderPath);
            foreach (FileInfo fi in xmlFolder.GetFiles())
            {
                if (fi.FullName.EndsWith(".xml"))
                {
                    xmlFiles.Add(fi);
                }
            }
            return xmlFiles;
        }
        #endregion

        public FiddlerExtension2()
        {
            initMenus();
        }

        public void log(string message)
        {
            if (Settings.verboseLogging)
            {
                FiddlerApplication.Log.LogString("ApiTestExtension: " + message + "\n");
            }
        }

        #region Interface Methods
        public void AutoTamperRequestAfter(Session oSession)
        {

        }

        public void AutoTamperRequestBefore(Session oSession)
        {

        }

        public void AutoTamperResponseAfter(Session oSession)
        {

        }

        public void AutoTamperResponseBefore(Session oSession)
        {
            if (bAutotestEnabled && oSession.responseCode == 200)
            {
                foreach (ProductInXML product in products.Values)
                {
                    foreach (ApiItem item in product.apiItems)
                    {
                        if (oSession.uriContains(item.url) && oSession.RequestMethod.Equals(item.request.type.ToString()))
                        {
                            try
                            {
                                String orgResponseBody = Encoding.UTF8.GetString(oSession.responseBodyBytes);
                                item.response.rootParam.matchWithJsonEntry(JsonEntry.analyzeFromJsonToken(JToken.Parse(orgResponseBody)));
                                if (item.response.rootParam.matchStruct.matchResult == MatchResult.TYPE_NOT_MATCH)
                                {
                                    oSession["ui-backcolor"] = "red";
                                }
                                log("\n****" + item.url + "****\n" + Utils.insertSpaces(item.response.rootParam.outputMatchResult()));
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                }
            }

        }

        public void OnBeforeReturningError(Session oSession)
        {

        }

        public void OnPeekAtRequestHeaders(Session oSession)
        {

        }

        public void OnPeekAtResponseHeaders(Session oSession)
        {

        }

        public void Dispose()
        {

        }

        public void OnBeforeUnload()
        {

        }

        public void OnLoad()
        {
            FiddlerApplication.UI.mnuMain.MenuItems.Add(_mnuApiAutoTest);
        }
        #endregion
    }
}

