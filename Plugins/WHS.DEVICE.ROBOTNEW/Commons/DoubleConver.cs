using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WHS.DEVICE.ROBOTNEW.Commons
{
    public class DoubleConver : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0d;
            double d = 0d;
            double.TryParse(value.ToString(), out d);
            return d;
        }
        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0d;
            double d = 0d;
            double.TryParse(value.ToString(), out d);
            return d;
        }
    }
}
