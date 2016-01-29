using System;
using System.Collections.Generic;
using System.Text;
using Fiddler;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

[assembly: Fiddler.RequiredVersion("2.4.9.3")]
namespace ApiTestExtension
{
    public class FiddlerExtension : IAutoTamper3, IDisposable
    {
        private MenuItem _mnuApiAutoTest;
        private MenuItem _miApiAutoTestEnabled;
        private MenuItem _miOpenRulesDialog;
        private MenuItem _miLogEnabled;

        private bool bAutotestEnabled = false;
        private bool bLogEnabled = false;

        #region Settings
        public static class Settings
        {
            private const string prefix = "ApiTestExtension.";
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
        
        private void EnableLog_CheckedChanged(object sender, EventArgs e)
        {
            MenuItem oSender = (sender as MenuItem);
            oSender.Checked = !oSender.Checked;

            bLogEnabled = _miLogEnabled.Checked;
            Settings.verboseLogging = bLogEnabled;
        }

        void initMenus()
        {
            _mnuApiAutoTest = new MenuItem();
            _miApiAutoTestEnabled = new MenuItem();
            _miOpenRulesDialog = new MenuItem();
            _miLogEnabled = new MenuItem();

            _mnuApiAutoTest.Text = "&APIAutoTestPlatform";
            _mnuApiAutoTest.MenuItems.Add(0, _miApiAutoTestEnabled);
            _mnuApiAutoTest.MenuItems.Add(1, _miLogEnabled);
            _mnuApiAutoTest.MenuItems.Add(2, _miOpenRulesDialog);

            _miApiAutoTestEnabled.Text = "Enabled";
            _miLogEnabled.Text = "Log Enabled";
            _miOpenRulesDialog.Text = "Config Autotest Rules...";
        }
        #endregion
        
        #region Interface Methods
        public void AutoTamperRequestAfter(Session oSession)
        {

        }

        public void AutoTamperRequestBefore(Session oSession)
        {
            if (bAutotestEnabled)
            {
                
            }

        }

        public void AutoTamperResponseAfter(Session oSession)
        {

        }

        public void AutoTamperResponseBefore(Session oSession)
        {

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

