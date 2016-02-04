using ApiTestExtension2.DataStructure.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTestExtension2.DataStructure.Matcher
{
    public class MatchStructure
    {
        public MatchResult matchResult = MatchResult.DEFAULT;
        public JsonEntryType responseJsonType;
        public HashSet<String> jsonEntryNotFound = new HashSet<string>();
        public HashSet<String> xmlItemNotFound = new HashSet<string>();
        public Dictionary<String, MatchStructure> subListItemMatchResult = new Dictionary<string, MatchStructure>();
        public bool isSubItemNotMatch = true;

        public MatchStructure(MatchStructure org)
        {
            this.matchResult = org.matchResult;
            this.responseJsonType = org.responseJsonType;
            this.jsonEntryNotFound = new HashSet<string>(org.jsonEntryNotFound);
            this.xmlItemNotFound = new HashSet<string>(org.xmlItemNotFound);
            this.subListItemMatchResult = new Dictionary<string, MatchStructure>(org.subListItemMatchResult);
            this.isSubItemNotMatch = org.isSubItemNotMatch;
        }

        public MatchStructure()
        {

        }
    }
}
