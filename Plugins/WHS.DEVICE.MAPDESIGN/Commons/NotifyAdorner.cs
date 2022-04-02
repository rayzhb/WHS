using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WHS.DEVICE.MAPDESIGN.Commons
{
    /// <summary>
    /// 提示Adorner
    /// </summary>
    public class NotifyAdorner : Adorner
    {

        private VisualCollection _visuals;
        private Canvas _canvas;
        private Image _image;
        private TextBlock _toolTip;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="adornedElement"></param>
        /// <param name="errorMessage"></param>
        public NotifyAdorner(UIElement adornedElement, string errorMessage) : base(adornedElement)
        {
            _visuals = new VisualCollection(this);

            BuildNotifyStyle(errorMessage);

            _canvas = new Canvas();

            _canvas.Children.Add(_image);

            _visuals.Add(_canvas);

        }

        private void BuildNotifyStyle(string errorMessage)
        {
            _image = new Image()
            {
                Width = 20,
                Height = 20,
                Source = new BitmapImage(new Uri("你的图片路径", UriKind.Absolute))
            };

            _toolTip = new TextBlock() { FontSize = 14, Text = errorMessage };

            _image.ToolTip = _toolTip;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _visuals.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        public void ChangeToolTip(string errorMessage)
        {
            _toolTip.Text = errorMessage;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _canvas.Arrange(new Rect(finalSize));

            _image.Margin = new Thickness(finalSize.Width + 2, 0, 0, 0);

            return base.ArrangeOverride(finalSize);
        }
    }
}
