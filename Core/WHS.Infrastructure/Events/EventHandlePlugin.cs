using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Events
{
    public class EventHandlePlugin
    {
        public HandleType HandleType { get; set; }

        public Guid PluginID { get; set; }
    }

    public enum HandleType
    {
        Add,
        Disable,
        Reload,
        Unload,
        Enable
    }
}
