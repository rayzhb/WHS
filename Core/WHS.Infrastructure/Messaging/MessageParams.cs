using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Messaging
{
    public class MessageParams
    {
        //public int Sequence { get; set; }
        [JsonProperty(PropertyName = "Param", NullValueHandling = NullValueHandling.Ignore)]
        public object Param { get; set; }
    }
}
