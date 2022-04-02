using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WHS.DEVICE.MAPDESIGN.Controls
{
    public class CustomRectangle : Shape
    {
        public int RowIndex
        {
            get; set;
        }
        public int ColumnIndex
        {
            get; set;
        }


        public double Left
        {
            get; set;
        }

        public double Top
        {
            get; set;
        }

        public string Id
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        public double NewWidth
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
            }
        }

        public double NewHeight
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Width = value;
            }
        }

        public Double Radius
        {
            get
            {
                return (Double)this.GetValue(RadiusProperty);
            }
            set
            {
                this.SetValue(RadiusProperty, value);
            }
        }
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
         "Radius", typeof(Double), typeof(CustomRectangle), new PropertyMetadata(0.0));

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new RectangleGeometry(new Rect(0, 0, base.Width, base.Height), Radius, Radius);
            }

        }

        public CustomRectangle()
        {
            base.SnapsToDevicePixels = true;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.PushGuidelineSet(new GuidelineSet());
            base.OnRender(drawingContext);
        }
    }
}
