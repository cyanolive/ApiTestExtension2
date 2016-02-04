using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace ApiTestExtension2.DataStructure.XML
{
    public class RequestInXML
    {
        public RequestType type;
        public HashSet<RequestParam> requestParams = new HashSet<RequestParam>();

        public RequestInXML(XmlNode xnRequestNode)
        {
            switch (xnRequestNode.Attributes["type"].Value)
            {
                case "GET":
                    this.type = RequestType.GET;
                    break;
                case "POST":
                    this.type = RequestType.POST;
                    break;
                case "PUT":
                    this.type = RequestType.PUT;
                    break;
                case "DELETE":
                    this.type = RequestType.DELETE;
                    break;
                default:
                    this.type = RequestType.DEFAULT;
                    break;
            }

            foreach (XmlNode xnRequestParam in xnRequestNode.SelectNodes("param"))
            {
                requestParams.Add(new RequestParam(xnRequestParam));
            }
        }

        public override string ToString()
        {
            String resultStr = "";

            resultStr += "request type:" + type + "\n";
            resultStr += "request params:\n";
            if (requestParams.Count > 0) {                
                foreach (RequestParam requestParam in requestParams)
                {
                    resultStr += Utils.Utils.insertSpaces(requestParam) + "\n";
                }
            }
            return resultStr;
        }
    }

    public enum RequestType
    {
        DEFAULT = -1,
        GET,
        POST,
        PUT,
        DELETE
    }
}
