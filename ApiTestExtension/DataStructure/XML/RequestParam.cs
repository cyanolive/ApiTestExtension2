using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ApiTestExtension2.DataStructure.XML
{
    public class RequestParam
    {
        RequestParamType type;
        bool compulsory;
        String paramName;
        String paramInstruction;

        public RequestParam(XmlNode xnRequestParam)
        {
            switch (xnRequestParam.Attributes["type"].Value)
            {
                case "string":
                    this.type = RequestParamType.STRING;
                    break;
                case "int":
                    this.type = RequestParamType.INT;
                    break;
                case "bool":
                    this.type = RequestParamType.BOOL;
                    break;
                default:
                    this.type = RequestParamType.DEFAULT;
                    break;
            }

            this.compulsory = xnRequestParam.Attributes["compulsory"].Value == "true" ? true : false;
            this.paramName = xnRequestParam.SelectSingleNode("paramname").InnerText;
            this.paramInstruction = xnRequestParam.SelectSingleNode("paramInstruction").InnerText;
        }

        public override string ToString()
        {
            String resultStr = "";

            resultStr += "RequestParamType:" + type + "//" + "compulsory:" + compulsory + "//"
                + "paramName:" + paramName + "//" + "paramInstruction:" + paramInstruction + "\n";

            return resultStr;
        }
    }

    public enum RequestParamType
    {
        DEFAULT = -1,
        STRING,
        INT,
        BOOL
    }
}
