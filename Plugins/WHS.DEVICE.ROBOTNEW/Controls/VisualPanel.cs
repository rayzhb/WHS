using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WHS.DEVICE.ROBOTNEW.Controls
{
    public class VisualPanel : FrameworkElement
    {
        public VisualCollection Children { get; set; }
        public VisualPanel()
        {
            Children = new VisualCollection(this);
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return Children.Count;
            }
        }
        protected override Visual GetVisualChild(int index)
        {
            return Children[index];
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (VisualChildrenCount > 0)
            {
                (Children[0] as FrameworkElement).Arrange(new Rect(0, 0, 100, 25));
            }
            return base.ArrangeOverride(finalSize);
        }
    }
}
