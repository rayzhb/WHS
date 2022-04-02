using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace WHS.DEVICE.ROBOTNEW.Models
{
    [Serializable]
    public class RES_MapModel
    {
        /// <summary>
        /// 总楼层
        /// </summary>
        [JsonProperty]
        public int TotalFloor
        {
            get; set;
        }

        /// <summary>
        /// 楼层
        /// </summary>
        [JsonProperty]
        public int Floor
        {
            get;set;
        }

        /// <summary>
        /// 地图中的点线信息
        /// </summary>
        [JsonProperty("mapinfo")]
        public List<FrameworkElement> MapInfo
        {
            get;set;
        }


        /// <summary>
        /// 画布-宽
        /// </summary>
        [JsonProperty]
        public double canvasWidth
        {
            get; set;
        }
        /// <summary>
        /// 画布-高
        /// </summary>
        [JsonProperty]
        public double canvasHeight
        {
            get; set;
        }



        /// <summary>
        /// 行数
        /// </summary>
        [JsonProperty]
        public int gridRows;
        /// <summary>
        /// 列数
        /// </summary>
        [JsonProperty]
        public int gridColumns;
    }
}
