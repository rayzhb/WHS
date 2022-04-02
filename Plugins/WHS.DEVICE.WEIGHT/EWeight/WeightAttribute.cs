using System;

namespace WHS.DEVICE.WEIGHT.EWeight
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class WeightAttribute : Attribute
    {
        /// <summary>
        /// 简单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Value { get; set; }
    }
}
