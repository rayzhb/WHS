using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure
{
    public class ServerSettings
    {
        public static bool IsSsl
        {
            get
            {
                string ssl = ConfigurationManager.AppSettings["ssl"];
                return !string.IsNullOrEmpty(ssl) && bool.Parse(ssl);
            }
        }

        public static int Port => int.Parse(ConfigurationManager.AppSettings["port"]);

        public static bool UseLibuv
        {
            get
            {
                string libuv = ConfigurationManager.AppSettings["libuv"];
                return !string.IsNullOrEmpty(libuv) && bool.Parse(libuv);
            }
        }

        public static string ApiUrl
        {
            get
            {
                string apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
                return string.IsNullOrEmpty(apiUrl) ? "" : apiUrl;
            }
        }
    }
}
