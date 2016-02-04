using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ApiTestExtension2.DataStructure.XML;

namespace ApiTestExtension2.Dialogs
{
    public partial class Coverage : Form, IDisposable
    {
        public Coverage()
        {
            InitializeComponent();

            foreach (String product in FiddlerExtension2.products.Keys)
            {
                this.cbProduct.Items.Add(product);
            }
            if (this.cbProduct.Items.Count > 0)
            {
                this.cbProduct.SelectedIndex = 0;
            }
        }

        private void initApiTree(String product)
        {
            tvApi.Nodes.Clear();
            foreach (ApiItem item in FiddlerExtension2.products[product].apiItems)
            {
                bool hasCreated = false;
                String menuFolder = item.menuFolder;
                String url = item.url;
                String menuText = item.menuText + "(" + url + ")";
                TreeNode tnNewFolder;
                TreeNode tnApi = new TreeNode();

                foreach (TreeNode tnFolder in tvApi.Nodes)
                {
                    if (tnFolder.Text.Equals(menuFolder))
                    {
                        tnApi = new TreeNode(menuText);
                        tnFolder.Nodes.Add(tnApi);
                        hasCreated = true;
                        break;
                    }
                }

                if (!hasCreated)
                {
                    tnNewFolder = new TreeNode(menuFolder);
                    tnApi = new TreeNode(menuText);
                    tvApi.Nodes.Add(tnNewFolder);
                    tnNewFolder.Nodes.Add(tnApi);                    
                }

                switch (item.response.rootParam.matchStruct.matchResult)
                {
                    case DataStructure.Matcher.MatchResult.MATCHED:
                        tnApi.BackColor = Color.LightGreen;
                        break;
                    case DataStructure.Matcher.MatchResult.NULL:
                    case DataStructure.Matcher.MatchResult.PARTLY_MATCHED:
                        tnApi.BackColor = Color.LightBlue;
                        break;
                    case DataStructure.Matcher.MatchResult.TYPE_NOT_MATCH:
                        tnApi.BackColor = Color.Red;
                        break;
                    default:
                        tnApi.BackColor = Color.White;
                        break;
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            initApiTree(this.cbProduct.Text);
        }
    }
}
