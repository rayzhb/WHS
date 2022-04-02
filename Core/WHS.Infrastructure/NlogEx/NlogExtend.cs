using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.NlogEx
{
    public static class NlogExtend
    {
        public static void Process(this ILogger logger, string message)
        {
            var logEventInfo = new LogEventInfo(LogLevel.Trace, logger.Name, message);
            logEventInfo.Properties["custLevel"] = Tuple.Create(9, "Process");
            logger.Log(logEventInfo);
        }
    }
}
