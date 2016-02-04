using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ApiTestExtension2.DataStructure.XML
{
    public class ProductInXML
    {
        public String productName;
        public HashSet<ApiItem> apiItems = new HashSet<ApiItem>();

        public ProductInXML(FileInfo fi) {            
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(fi.FullName);
            XmlNode root = xmldoc.SelectSingleNode("root");

            this.productName = root.Attributes["product"].Value;
            foreach (XmlNode xnApiNode in root.SelectNodes("api"))
            {
                ApiItem item = new ApiItem(xnApiNode);
                apiItems.Add(item);
            }
        }

        public override string ToString()
        {
            String resultStr = "";

            resultStr += "productName:" + productName + "\n";
            foreach(ApiItem apiItem in apiItems){
                resultStr += apiItem.ToString() + "\n";
            }

            return resultStr;
        }
    }
}
