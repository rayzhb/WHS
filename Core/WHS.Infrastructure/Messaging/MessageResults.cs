using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Messaging
{
    public class MessageResults
    {
        /// <summary>
        /// 来源设备
        /// </summary>
        [JsonProperty(PropertyName = "Source", NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }
        [JsonProperty(PropertyName = "Result", NullValueHandling = NullValueHandling.Ignore)]
        public dynamic Result { get; set; }
    }
}
