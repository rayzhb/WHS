using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PropertyTools.DataAnnotations;
using WHS.DEVICE.MAPDESIGN.Commons;

namespace WHS.DEVICE.MAPDESIGN.Controls
{
    [JsonObject(Newtonsoft.Json.MemberSerialization.OptIn)]
    public class CustomLine : Shape, INotifyPropertyChangedEx, System.ComponentModel.INotifyPropertyChanged
    {
        private SolidColorBrush _NewStroke;

        [CustomDisplay]
        [Category("线信息|线")]
        [DisplayName("颜色")]
        [Converter(typeof(PropertyTools.Wpf.ColorToBrushConverter))]
        [JsonProperty("stroke")]
        public SolidColorBrush NewStroke
        {
            get
            {
                return _NewStroke;
            }
            set
            {
                _NewStroke = value;
                base.Stroke = _NewStroke;
            }
        }

        [Browsable(false)]
        public List<string> Items
        {
            get; set;
        }

        private string _Arrow;
        [CustomDisplay]
        [DisplayName("箭头")]
        [ItemsSourceProperty("Items")]
        [JsonProperty("arrow")]
        public string Arrow
        {
            get
            {
                return _Arrow;

            }
            set
            {
                _Arrow = value;
                base.InvalidateVisual();
            }

        }

        [CustomDisplay]
        [DisplayName("距离")]
        [JsonProperty("dis")]
        public double Distance
        {
            get; set;
        }


        [CustomDisplay]
        [DisplayName("粗细")]
        [JsonProperty("thickness")]
        public double NewStrokeThickness
        {
            get
            {

                return base.StrokeThickness;
            }
            set
            {
                base.StrokeThickness = value;
            }
        }

        [CustomDisplay]
        [DisplayName("开始-行")]
        [Category("线信息|信息")]
        [Comment]
        [JsonProperty("sr")]
        public int StartRowIndex
        {
            get; set;
        }
        [CustomDisplay]
        [DisplayName("开始-列")]
        [Comment]
        [JsonProperty("sc")]
        public int StartColumnIndex
        {
            get; set;
        }
        [CustomDisplay]
        [DisplayName("结束-行")]
        [Comment]
        [JsonProperty("er")]
        public int EndRowIndex
        {
            get; set;
        }
        [CustomDisplay]
        [DisplayName("结束-列")]
        [Comment]
        [JsonProperty("ec")]
        public int EndColumnIndex
        {
            get; set;
        }

        [CustomDisplay]
        [DisplayName("宽")]
        [Comment]
        [JsonProperty("w")]
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

        [CustomDisplay]
        [DisplayName("高")]
        [Comment]
        [JsonProperty("h")]
        public double NewHeight
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Height = value;
            }
        }


        [CustomDisplay]
        [Category("线信息|信息")]
        [DisplayName("线-Id")]
        [Comment]
        [JsonProperty("id")]
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


        private double _Left;

        //[CustomDisplay]
        //[Category("圆信息|信息")]
        //[DisplayName("画布-Left")]
        //[Comment]
        [JsonProperty("l")]
        public double Left
        {
            get
            {
                return _Left;
            }
            set
            {
                _Left = value;
                NotifyOfPropertyChange(() => Left);
            }
        }


        private double _Top;

        //[CustomDisplay]
        //[Category("圆信息|信息")]
        //[DisplayName("画布-Top")]
        [Comment]
        [JsonProperty("t")]
        public double Top
        {
            get
            {
                return _Top;
            }
            set
            {
                _Top = value;
                NotifyOfPropertyChange(() => Top);
            }
        }

        [JsonProperty("dtype")]
        public string DrawType
        {
            get; set;
        }

        private Point _Start;
        [JsonProperty("sp")]
        public Point Start
        {
            get
            {
                return _Start;
            }
            set
            {
                _Start = value;
                NotifyOfPropertyChange(() => Start);
            }
        }

        private Point _End;
        [JsonProperty("ep")]
        public Point End
        {
            get
            {
                return _End;
            }
            set
            {
                _End = value;
                NotifyOfPropertyChange(() => End);
            }
        }

        public CustomLine(Point start, Point end)
        {
            _Start = start;
            _End = end;
            DrawType = "CustomLine";
            base.SnapsToDevicePixels = true;
            this.Items = new List<string> { "Default", "Double", "Right", "Left" };
            Arrow = "Default";
            NewStrokeThickness = 1;
            Distance = 1d;
            NewStroke = Brushes.Black;
            base.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new LineGeometry(Start, End);
            }

        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.PushGuidelineSet(new GuidelineSet());
            switch (this.Arrow)
            {
                case "Default":
                    base.OnRender(drawingContext);
                    break;
                case "Right":
                    var rightPath = DrawUtility.DrawSingleArrowGeometry(Start, End, Math.PI / 12, NewStrokeThickness + 10);
                    drawingContext.DrawGeometry(NewStroke, new Pen(NewStroke, NewStrokeThickness), rightPath);
                    break;
                case "Left":
                    var leftPath = DrawUtility.DrawSingleArrowGeometry(End, Start, Math.PI / 12, NewStrokeThickness + 10);
                    drawingContext.DrawGeometry(NewStroke, new Pen(NewStroke, NewStrokeThickness), leftPath);
                    break;
                case "Double":
                    var doublePath = DrawUtility.DrawDoubleArrowGeometry(End, Start, Math.PI / 12, NewStrokeThickness + 10);
                    drawingContext.DrawGeometry(NewStroke, new Pen(NewStroke, NewStrokeThickness), doublePath);
                    break;
                default:
                    break;
            }

        }

        public static CustomLine ConvertJToken(JToken token)
        {
            string id = token["id"].Value<string>();
            double newWidth = token["w"].Value<double>();
            double newHeight = token["h"].Value<double>();
            double distance = 1d;
            if (token["dis"] != null)
            {
                distance = token["dis"].Value<double>();
            }
            double left = token["l"].Value<double>();
            double top = token["t"].Value<double>();
            int startRowIndex = token["sr"].Value<int>();
            int startColumnIndex = token["sc"].Value<int>();
            int endRowIndex = token["er"].Value<int>();
            int endColumnIndex = token["ec"].Value<int>();
            string strstroke = token["stroke"].Value<string>();
            var converter = new System.Windows.Media.BrushConverter();
            string arrow = token["arrow"].Value<string>();
            SolidColorBrush brush = (SolidColorBrush)converter.ConvertFromString(strstroke);
            Point spoint, epoint;
            if (token["sp"] != null)
            {
                var value = token["sp"].Value<string>();
                spoint = Point.Parse(value);
            }
            else
            {
                if (id.StartsWith('h'))
                {
                    spoint = new Point(0, newHeight / 2);
                }
                else
                {
                    spoint = new Point(newWidth / 2, 0);
                }
            }
            if (token["ep"] != null)
            {
                var value = token["ep"].Value<string>();
                epoint = Point.Parse(value);
            }
            else
            {
                if (id.StartsWith('h'))
                {
                    epoint = new Point(newWidth, newHeight / 2);
                }
                else
                {
                    epoint = new Point(newWidth / 2, newHeight);
                }
            }

            double thickness = token["thickness"].Value<double>();
            CustomLine customLine = new CustomLine(spoint, epoint)
            {
                Id = id,
                StartRowIndex = startRowIndex, StartColumnIndex = startColumnIndex,
                EndRowIndex = endRowIndex, EndColumnIndex = endColumnIndex,
                NewStrokeThickness = thickness,
                NewWidth = newWidth,
                NewHeight = newHeight,
                NewStroke = brush,
                Left = left,
                Top = top,
                Arrow = arrow,
                Distance = distance
            };
            return customLine;
        }

        public void ChangePoint(Point Start, Point End)
        {
            var tWidth = End.X - Start.X;
            var tHeight = End.Y - Start.Y;

            Left = Math.Min(Start.X, End.X);
            Top = Math.Min(Start.Y, End.Y);

            var aWidth = Math.Abs(tWidth);
            var aHeight = Math.Abs(tHeight);

            Width = aWidth;
            Height = aHeight;
            double subx = 0d;
            double suby = 0d;

            if (tWidth > 0 && tHeight > 0)
            {
                subx = MiniWidth();
                suby = MiniHeight();
                _Start = new Point(subx, suby);
                _End = new Point(aWidth + subx, aHeight + suby);
            }
            else if (tWidth < 0 && tHeight > 0)
            {
                subx = MiniWidth();
                suby = MiniHeight();
                _Start = new Point(aWidth + subx, suby);
                _End = new Point(subx, aHeight + suby);
            }
            else if (tWidth > 0 && tHeight < 0)
            {
                subx = MiniWidth();
                suby = MiniHeight();
                _Start = new Point(subx, aHeight + suby);
                _End = new Point(aWidth + subx, suby);
            }
            else if (tWidth < 0 && tHeight < 0)
            {
                subx = MiniWidth();
                suby = MiniHeight();
                _Start = new Point(subx, suby);
                _End = new Point(aWidth + subx, aHeight + suby);
            }
            else if (Width == 0)
            {
                Left = Left - 4;
                Width = 8;
                _Start = new Point(Width / 2, 0);
                _End = new Point(Width / 2, Height);
            }
            else if (Height == 0)
            {
                Top = Top - 4;
                Height = 8;
                _Start = new Point(0, Height / 2);
                _End = new Point(Width, Height / 2);
            }
            base.InvalidateVisual();
        }

        private double MiniWidth()
        {
            if (Width < 4)
            {
                Left = Left - 2;
                Width = Width + 4;
                return  2d;
            }
            return 0d;
        }
        private double MiniHeight()
        {
            if (Height < 4)
            {
                Top = Top - 2;
                Height = Height + 4;
                return 2d;
            }
            return 0d;
        }


        public virtual bool IsNotifying
        {
            get;
            set;
        } = true;
        public virtual event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public virtual void Refresh()
        {
            NotifyOfPropertyChange(string.Empty);
        }
        public virtual void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
        {
            if (!IsNotifying || this.PropertyChanged == null)
            {
                return;
            }

            if (PlatformProvider.Current.PropertyChangeNotificationsOnUIThread)
            {
                OnUIThread(delegate
                {
                    OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));
                });
            }
            else
            {
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        public void NotifyOfPropertyChange<TProperty>(System.Linq.Expressions.Expression<Func<TProperty>> property)
        {
            NotifyOfPropertyChange(property.GetMemberInfo().Name);
        }
        protected void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }
        protected virtual void OnUIThread(System.Action action)
        {
            action.OnUIThread();
        }
        public virtual bool Set<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return false;
            }

            oldValue = newValue;
            NotifyOfPropertyChange(propertyName ?? string.Empty);
            return true;
        }

    }
}
