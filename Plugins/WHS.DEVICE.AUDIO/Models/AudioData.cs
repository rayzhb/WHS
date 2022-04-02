using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.DEVICE.AUDIO.Models
{
    public class AudioData
    {
        /// <summary>
        /// 0:异步，1：同步
        /// </summary>
        public static int PlayType { get; set; }

        /// <summary>
        /// 0：缓存；1在线
        /// </summary>
        public static int LoadViceType { get; set; }
    }
}
