using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ApiTestExtension2.DataStructure.Matcher;
using ApiTestExtension2.DataStructure.Json;
using ApiTestExtension2.DataStructure.Utils;

namespace ApiTestExtension2.DataStructure.XML
{
    public class ResponseParam
    {
        public String name;
        public String remark;
        public ResponseParamType type;
        public Dictionary<String, ResponseParam> subItems = new Dictionary<string, ResponseParam>();
        public MatchStructure matchStruct = new MatchStructure();

        public ResponseParam(ResponseParam param)
        {
            this.name = param.name;
            this.remark = param.remark;
            this.type = param.type;
            this.subItems = new Dictionary<string, ResponseParam>(param.subItems);
        }

        public ResponseParam(XmlNode xnResponseParam)
        {
            this.name = xnResponseParam.SelectSingleNode("name").InnerText;
            this.remark = xnResponseParam.SelectSingleNode("remark").InnerText;

            switch (xnResponseParam.Attributes["type"].Value)
            {
                case "dict":
                    foreach (XmlNode subNode in xnResponseParam.SelectNodes("item"))
                    {
                        ResponseParam subItem = new ResponseParam(subNode);
                        subItems.Add(subItem.name, subItem);
                    }
                    if (subItems.Count > 0)
                    {
                        this.type = ResponseParamType.DICT;
                    }
                    else
                    {
                        this.type = ResponseParamType.DICT_NULL;
                    }
                    break;
                case "list":
                    foreach (XmlNode subNode in xnResponseParam.SelectNodes("item"))
                    {
                        ResponseParam subItem = new ResponseParam(subNode);
                        subItems.Add(subItem.name, subItem);
                    }
                    if (subItems.Count > 0)
                    {
                        this.type = ResponseParamType.LIST;
                    }
                    else
                    {
                        this.type = ResponseParamType.LIST_STRING;
                    }
                    break;
                case "string":
                    this.type = ResponseParamType.STRING;
                    break;
                case "int":
                    this.type = ResponseParamType.INT;
                    break;
                case "bool":
                    this.type = ResponseParamType.BOOL;
                    break;
                default:
                    this.type = ResponseParamType.DEFAULT;
                    break;
            }
        }

        private void mergeListMatchResult(MatchStructure orgMatchStructure, bool isInList)
        {
            if (isInList)
            {
                if (this.matchStruct.matchResult < orgMatchStructure.matchResult)
                {
                    this.matchStruct = orgMatchStructure;
                }
                if (orgMatchStructure.matchResult == MatchResult.MATCHED &&
                    this.matchStruct.matchResult != MatchResult.TYPE_NOT_MATCH)
                {
                    this.matchStruct = orgMatchStructure;
                }
            }
        }

        public void matchWithJsonEntry(JsonEntry jsonEntry, bool isInList)
        {
            MatchStructure orgMatchStructure = this.matchStruct;
            this.matchStruct.responseJsonType = jsonEntry.type;
            switch (jsonEntry.type)
            {
                case JsonEntryType.BOOL:
                    if (this.type == ResponseParamType.BOOL)
                    {
                        this.matchStruct.matchResult = MatchResult.MATCHED;
                        mergeListMatchResult(orgMatchStructure, isInList);
                    }
                    break;
                case JsonEntryType.DICT:
                    if (this.type == ResponseParamType.DICT)
                    {
                        matchDict(jsonEntry);
                        mergeListMatchResult(orgMatchStructure, isInList);
                    }
                    break;
                case JsonEntryType.DICT_NULL:
                    if (this.type == ResponseParamType.DICT_NULL)
                    {
                        this.matchStruct.matchResult = MatchResult.MATCHED;
                        mergeListMatchResult(orgMatchStructure, isInList);
                    }
                    if (this.type == ResponseParamType.DICT)
                    {
                        this.matchStruct.matchResult = MatchResult.NULL;
                        mergeListMatchResult(orgMatchStructure, isInList);
                    }
                    break;
                case JsonEntryType.INT:
                    if (this.type == ResponseParamType.INT)
                    {
                        this.matchStruct.matchResult = MatchResult.MATCHED;
                        mergeListMatchResult(orgMatchStructure, isInList);
                    }
                    break;
                case JsonEntryType.LIST:
                    if (this.type == ResponseParamType.LIST || this.type == ResponseParamType.LIST_STRING)
                    {
                        matchList(jsonEntry);
                        mergeListMatchResult(orgMatchStructure, isInList);
                    }
                    break;
                case JsonEntryType.LIST_NULL:
                    if (this.type == ResponseParamType.LIST)
                    {
                        this.matchStruct.matchResult = MatchResult.MATCHED;
                        mergeListMatchResult(orgMatchStructure, isInList);
                    }
                    break;
                case JsonEntryType.NULL:
                    this.matchStruct.matchResult = MatchResult.MATCHED;
                        mergeListMatchResult(orgMatchStructure, isInList);
                    break;
                case JsonEntryType.STRING:
                    if (this.type == ResponseParamType.STRING)
                    {
                        this.matchStruct.matchResult = MatchResult.MATCHED;
                        mergeListMatchResult(orgMatchStructure, isInList);
                    }
                    break;
                default:
                    break;
            }
            if (this.matchStruct.matchResult == MatchResult.DEFAULT)
            {
                this.matchStruct.matchResult = MatchResult.TYPE_NOT_MATCH;
                mergeListMatchResult(orgMatchStructure, isInList);
                this.matchStruct.isSubItemNotMatch = false;
            }
        }
        
        
        public void matchWithJsonEntry(JsonEntry jsonEntry)
        {
            matchWithJsonEntry(jsonEntry, false);
        }

        private void matchList(JsonEntry jsonEntry)
        {
            if (jsonEntry.entryList[0].type == JsonEntryType.STRING ||
                jsonEntry.entryList[0].type == JsonEntryType.INT)
            {
                if (this.type == ResponseParamType.LIST_STRING)
                {
                    this.matchStruct.matchResult = MatchResult.MATCHED;
                }
            }
            else if (jsonEntry.entryList[0].type == JsonEntryType.DICT)
            {
                if (this.type == ResponseParamType.LIST)
                {
                    foreach (JsonEntry jsonListSubEntry in jsonEntry.entryList)
                    {
                        //foreach (String key in jsonListSubEntry.entryDict.Keys)
                        //{
                        //    this.subItems[key].matchDict(jsonListSubEntry.entryDict[key], true);
                        //}
                        this.matchDict(jsonListSubEntry, true);
                    }

                }
            }

        }

        private void matchDict(JsonEntry jsonEntry, bool isInList)
        {
            HashSet<String> subItemsKeys = new HashSet<string>();
            foreach (String key in this.subItems.Keys)
            {
                subItemsKeys.Add(key);
            }

            //遍历每一个json中的数据格式，和XML对比，找到差异和是否有json中多余的部分，
            //匹配成功则在XML对象中移除相关Key，剩余的便是XML有但是json没有的部分了
            foreach (String key in jsonEntry.entryDict.Keys)
            {
                if (this.subItems.ContainsKey(key))
                {
                    this.subItems[key].matchWithJsonEntry(jsonEntry.entryDict[key], isInList);
                }
                else
                {
                    this.matchStruct.jsonEntryNotFound.Add(key);
                }
                subItemsKeys.Remove(key);
            }

            foreach (String key in subItemsKeys)
            {
                this.matchStruct.xmlItemNotFound.Add(key);
            }

            //json有没有被覆盖的数据（通常有问题，需要更新XML）
            if (this.matchStruct.jsonEntryNotFound.Count > 0)
            {
                this.matchStruct.matchResult = MatchResult.TYPE_NOT_MATCH;
            }

            //XML有没有被覆盖到的数据（常见的，定义的规则中是有字段不是必须返回的）
            else if (this.matchStruct.xmlItemNotFound.Count > 0)
            {
                this.matchStruct.matchResult = MatchResult.PARTLY_MATCHED;
            }

            //针对每个匹配的item再做下对比
            foreach (String key in this.subItems.Keys)
            {
                if (this.matchStruct.matchResult < this.subItems[key].matchStruct.matchResult)
                {
                    this.matchStruct.matchResult = this.subItems[key].matchStruct.matchResult;
                }
            }

            subItemsKeys = null;
        }

        private void matchDict(JsonEntry jsonEntry)
        {
            matchDict(jsonEntry, false);
        }

        public String outputMatchResult()
        {
            String str = "";
            switch (this.matchStruct.matchResult)
            {
                case MatchResult.MATCHED:
                    str += "";
                    break;
                case MatchResult.NULL:
                    str += this.name + ", " + "is dict but json returns null.";
                    break;
                case MatchResult.PARTLY_MATCHED:
                    str += this.name + ", " + this.name + "json has items missing";
                    foreach (String key in this.matchStruct.xmlItemNotFound)
                    {
                        str += key + ",";
                    }
                    str += "\n";
                    break;
                case MatchResult.TYPE_NOT_MATCH:
                    if (this.matchStruct.jsonEntryNotFound.Count > 0)
                    {
                        str += this.name + ", xml missing items:";
                        foreach (String key in this.matchStruct.jsonEntryNotFound)
                        {
                            str += key + ",";
                        }
                        str += "\n";
                    }
                    else if (this.matchStruct.isSubItemNotMatch)
                    {
                        str += this.name + ", " + "sub items not match\n";
                    }
                    else
                    {
                        str += this.name + ", " + "type not match, in xml is:" + this.type
                            + ",but in json is:" + this.matchStruct.responseJsonType + "\n";
                    }
                    break;
                default:
                    break;
            }

            foreach (String key in this.subItems.Keys)
            {
                if (this.subItems[key].matchStruct.matchResult != MatchResult.MATCHED)
                {
                    str += Utils.Utils.insertSpaces(this.subItems[key].outputMatchResult()) + "\n";
                }
            }
            return str;
        }

        public override String ToString()
        {
            String resultStr = "";

            resultStr += "name:" + name + "//" + "type:" + type + "//" + "remark:" + remark + "//";
            if (type == ResponseParamType.DICT)
            {
                resultStr += "SubDict:\n";
                foreach (ResponseParam responseParam in subItems.Values)
                {
                    resultStr += Utils.Utils.insertSpaces(responseParam) + "\n";
                }
            }
            if (type == ResponseParamType.LIST)
            {
                resultStr += "SubList:\n";
                foreach (ResponseParam responseParam in subItems.Values)
                {
                    resultStr += Utils.Utils.insertSpaces(responseParam) + "\n";
                }
            }
            resultStr += "\n";

            return resultStr;
        }
    }

    public enum ResponseParamType
    {
        DEFAULT = -1,
        DICT,
        DICT_NULL,
        LIST,
        LIST_STRING,
        STRING,
        INT,
        BOOL
    }
}
