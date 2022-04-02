using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure
{
    public class LocalizationHelp
    {
        public static string GetLocalizedString(System.Reflection.Assembly assembly, string key, string resourceFileName = "Strings", bool addSpaceAfter = false)
        {
            var localizedString = String.Empty;

            var assemblyName = assembly.GetName().Name;
            var fullKey = assemblyName + ":" + resourceFileName + ":" + key;
            var locExtension = new WPFLocalizeExtension.Extensions.LocExtension(fullKey);
            locExtension.ResolveLocalizedValue(out localizedString);

            if (addSpaceAfter)
            {
                localizedString += " ";
            }

            return localizedString;
        }

    }
}
