using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using Newtonsoft.Json;

namespace WHS.DEVICE.ROBOTNEW.Models
{
    [Serializable]
    public class Cb_AGVMove
    {
        [JsonProperty("code")]
        public string Code
        {
            get; set;
        }

        [JsonProperty("command")]
        public AGVCommand Command
        {
            get; set;
        }

        [JsonProperty("floor")]
        public int Floor
        {
            get; set;
        }

        [JsonProperty("startPoint")]
        public Point StartPoint
        {
            get; set;
        }

        [JsonProperty("endPoint")]
        public Point Endoint
        {
            get; set;
        }

        [JsonProperty("distance")]
        public double Distance
        {
            get; set;
        }

        [JsonProperty("power")]
        public string Power
        {
            get;set;
        }

        [JsonProperty("linePath")]
        public object Path
        {
            get;set;
        }
    }
}
