using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTestExtension2.DataStructure.Utils
{
    public class Utils
    {
        public static String insertSpaces(object orgStr)
        {
            return "    " + orgStr.ToString().Replace("\n", "\n    ").Trim(new char[]{' ', '\n'});
        }
    }
}
