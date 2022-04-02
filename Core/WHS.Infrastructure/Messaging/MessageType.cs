using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Messaging
{
    public class MessageTypeDefine
    {
        /// <summary>
        /// 无
        /// </summary>
        public static readonly string NONE = "none";
        /// <summary>
        /// 访问设备
        /// </summary>
        public static readonly string MREQ = "1xx1";
        /// <summary>
        /// 设备响应
        /// </summary>
        public static readonly string MRSP = "1xx1";
        /// <summary>
        /// 设备回调
        /// </summary>
        public static readonly string MCBK = "1xx1";
        /// <summary>
        /// 设备事件
        /// </summary>
        public static readonly string MEVT = "0xx1";
    }

}
