using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHS.Infrastructure;

namespace WHS.DEVICE.WEIGHT.Model
{
    [Serializable]
    public class WeightSettingModel
    {

        /// <summary>
        /// 电子秤类型
        /// </summary>
        public ComboBoxModel EWType
        {
            get; set;
        }

        /// <summary>
        /// 端口名称
        /// </summary>
        public ComboBoxModel PortName
        {
            get; set;
        }

        /// <summary>
        /// 位/秒
        /// </summary>
        public ComboBoxModel BaudRate
        {
            get; set;
        }

        /// <summary>
        /// 数据位
        /// </summary>
        public ComboBoxModel DataBits
        {
            get; set;
        }
        /// <summary>
        /// 奇偶校验
        /// </summary>
        public ComboBoxModel Parity
        {
            get; set;
        }

        /// <summary>
        /// 停止位
        /// </summary>
        public ComboBoxModel StopBits
        {
            get; set;
        }


        public WeightSettingModel()
        {

        }

    }
}
