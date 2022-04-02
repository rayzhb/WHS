using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Events
{
    public class EventMessage
    {
        public EventDefinition EventDefinition { get; set; }

        public object EventData { get; set; }
    }
}
