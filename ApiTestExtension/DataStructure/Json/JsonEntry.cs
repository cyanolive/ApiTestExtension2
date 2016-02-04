using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTestExtension2.DataStructure.Json
{
    public class JsonEntry
    {
        public JsonEntryType type = JsonEntryType.DEFAULT;
        public List<JsonEntry> entryList = new List<JsonEntry>();
        public Dictionary<String, JsonEntry> entryDict = new Dictionary<string, JsonEntry>();
        public long intValue;
        public String stringValue;
        public bool boolValue;

        public static JsonEntry analyzeFromJsonToken(JToken jt)
        {
            JsonEntry result = new JsonEntry();

            //try
            //{
                switch (jt.Type)
                {
                    case JTokenType.Array:
                        if ((jt as JArray).Count > 0)
                        {
                            result.type = JsonEntryType.LIST;
                            foreach (JToken jtItem in (jt as JArray))
                            {
                                result.entryList.Add(analyzeFromJsonToken(jtItem));
                            }
                        }
                        else
                        {
                            result.type = JsonEntryType.LIST_NULL;
                        }
                        break;
                    case JTokenType.Object:
                        if ((jt as JObject).HasValues)
                        {
                            result.type = JsonEntryType.DICT;
                            foreach (JProperty jpItem in (jt as JObject).Children())
                            {
                                String key = jpItem.Name;
                                JToken jtValue = jpItem.Value;
                                result.entryDict.Add(key, analyzeFromJsonToken(jtValue));
                            }
                        }
                        else
                        {
                            result.type = JsonEntryType.DICT_NULL;
                        }
                        break;
                    case JTokenType.Null:
                        result.type = JsonEntryType.NULL;
                        break;
                    case JTokenType.Integer:
                        result.type = JsonEntryType.INT;
                        result.intValue = Convert.ToInt64((jt as JValue).Value);
                        break;
                    case JTokenType.Boolean:
                        result.type = JsonEntryType.BOOL;
                        result.boolValue = Convert.ToBoolean((jt as JValue).Value);
                        break;
                    default:
                        result.type = JsonEntryType.STRING;
                        result.stringValue = (jt as JValue).Value.ToString();
                        break;
                }
            //}
            //catch (Exception e)
            //{

            //}

            return result;
        }
    }

    public enum JsonEntryType
    {
        DEFAULT,
        LIST,
        LIST_NULL,
        DICT,
        DICT_NULL,
        STRING,
        INT,
        NULL,
        BOOL
    }
}
