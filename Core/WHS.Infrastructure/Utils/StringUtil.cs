using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Utils
{
    public static class StringUtil
    {
        public static int ToInt(this string source,int defaultvalue=0)
        {
            int.TryParse(source, out defaultvalue);
            return defaultvalue;
        }
    }
}
