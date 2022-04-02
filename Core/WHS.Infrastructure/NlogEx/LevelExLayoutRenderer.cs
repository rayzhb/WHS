using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.NlogEx
{
    /// <summary>
    /// config文件：(用levelx取代原来的level即可)
    /// 如： ${levelx:uppercase=true} 
    /// </summary>
    [LayoutRenderer("levelx")]
    public class LevelExLayoutRenderer : LevelLayoutRenderer
    {

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (logEvent.Level == LogLevel.Trace && logEvent.Properties.ContainsKey("custLevel"))
            {
                var custLevel = logEvent.Properties["custLevel"] as Tuple<int, string>;
                if (custLevel == null)
                {
                    throw new InvalidCastException("Invalid Cast Tuple<int, string>");
                }

                switch (this.Format)
                {
                    case LevelFormat.Name:
                        builder.Append(custLevel.Item2);
                        break;
                    case LevelFormat.FirstCharacter:
                        builder.Append(custLevel.Item2[0]);
                        break;
                    case LevelFormat.Ordinal:
                        builder.Append(custLevel.Item1);
                        break;
                }
            }
            else
            {
                base.Append(builder, logEvent);
            }
        }
    }
}
