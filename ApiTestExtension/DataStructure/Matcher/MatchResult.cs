using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTestExtension2.DataStructure.Matcher
{
    public enum MatchResult
    {
        DEFAULT = -1,
        MATCHED,
        NULL,
        PARTLY_MATCHED,
        TYPE_NOT_MATCH = 100,
    }
}
