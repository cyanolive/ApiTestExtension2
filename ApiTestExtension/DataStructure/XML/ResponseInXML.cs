using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ApiTestExtension2.DataStructure.Utils;

namespace ApiTestExtension2.DataStructure.XML
{
    public class ResponseInXML
    {
        public ResponseType type;
        public ResponseParam rootParam;

        public ResponseInXML(XmlNode xnResponseNode)
        {
            switch (xnResponseNode.Attributes["type"].Value)
            {
                case "json":
                    this.type = ResponseType.JSON;
                    rootParam = new ResponseParam(xnResponseNode.SelectSingleNode("item"));
                    break;
                case "raw":
                    this.type = ResponseType.RAW;
                    break;
                default:
                    this.type = ResponseType.DEFAULT;
                    break;
            }
        }

        //public String outputJsonMatchResult()
        //{
        //    switch (this.rootParam.matcher.matchResult)
        //    {
        //        case MatchResult.MATCHED:
        //            return "match result: MATCHED" + Utils.Utils.insertSpaces(this.rootParam.outputMatchResult());
        //        case MatchResult.NULL:
        //            return "match result: Json Null" + Utils.Utils.insertSpaces(this.rootParam.outputMatchResult());
        //        case MatchResult.PARTLY_MATCHED:
        //            return "match result: Partly Matched\n" + Utils.Utils.insertSpaces(this.rootParam.outputMatchResult());
        //        case MatchResult.TYPE_NOT_MATCH:
        //            return "match result: Type Not Match\n" + Utils.Utils.insertSpaces(this.rootParam.outputMatchResult());
        //        default:
        //            return "";
        //    }
        //}

        public override string ToString()
        {
            String resultStr = "";

            resultStr += "ResponseType:" + type + "\n";
            resultStr += Utils.Utils.insertSpaces(rootParam) + "\n";

            return resultStr;
        }
    }

    public enum ResponseType
    {
        DEFAULT = -1,
        JSON,
        RAW
    }
}
