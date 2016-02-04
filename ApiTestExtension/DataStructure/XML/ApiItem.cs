using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;

namespace ApiTestExtension2.DataStructure.XML
{
    public class ApiItem
    {
        public String name;
        public String url;
        public String function;
        public String menuFolder;
        public String menuText;
        public RequestInXML request;
        public ResponseInXML response;

        public ApiItem(XmlNode apiNode)
        {
            //解析API参数
            this.name = apiNode.SelectSingleNode("name").InnerText;
            this.url = apiNode.SelectSingleNode("apiulr").InnerText;
            this.function = apiNode.SelectSingleNode("function").InnerText;
            this.menuFolder = apiNode.SelectSingleNode("menuFolder").InnerText;
            this.menuText = apiNode.SelectSingleNode("menuText").InnerText;

            this.request = new RequestInXML(apiNode.SelectSingleNode("request"));
            this.response = new ResponseInXML(apiNode.SelectSingleNode("response"));
        }

        public override string ToString()
        {
            String resultStr = "";
            resultStr += "name:" + name + "\n";
            resultStr += "url:" + url + "\n";
            resultStr += "function:" + function + "\n";
            resultStr += "menuFolder:" + menuFolder + "\n";
            resultStr += "menuText:" + menuText + "\n";

            resultStr += "request:\n" + Utils.Utils.insertSpaces(request) + "\n";
            resultStr += "response:\n" + Utils.Utils.insertSpaces(response) + "\n";

            resultStr += "\n-------------------------------";

            return resultStr;
        }
    }
}
