using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHS.Infrastructure;

namespace WHS.DEVICE.WEIGHT.Model
{
    [Serializable]
    public class ComboBoxModel 
    {

        /// <summary>
        /// 文本
        /// </summary>
        public string Text
        {
            get;set;
        }

        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get;set;
        }
    }
}
