using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WHS.DEVICE.ROBOTNEW.Commons
{
    public static class VisualUtility
    {
        public static T SearchVisualTree<T>(DependencyObject tarElem) where T : DependencyObject
        {
            if (tarElem != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(tarElem);
                if (count == 0)
                    return null;
                for (int i = 0; i < count; ++i)
                {
                    var child = VisualTreeHelper.GetChild(tarElem, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }
                    else
                    {
                        var res = SearchVisualTree<T>(child);
                        if (res != null)
                        {
                            return res;
                        }
                    }
                }
                return null;
            }
            return null;
        }
    }
}
