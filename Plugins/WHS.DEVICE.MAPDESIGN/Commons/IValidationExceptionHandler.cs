using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.DEVICE.MAPDESIGN.Commons
{
    /// <summary>
    /// 验证异常处理接口，由VM来继承实现
    /// </summary>
    public interface IValidationExceptionHandler
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        bool IsValid
        {
            get;
            set;
        }
    }
}
