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
    public class CustomEllipse : Shape, INotifyPropertyChangedEx, System.ComponentModel.INotifyPropertyChanged
    {

        [CustomDisplay]
        [Category("圆信息|粗细")]
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

        [Browsable(false)]
        public List<string> Items
        {
            get; set;
        }

        [JsonProperty("fill")]
        public Brush NewFill
        {
            get
            {
                return base.Fill;
            }
            set
            {

                base.Fill = value;
            }
        }

        private string _Type;
        [CustomDisplay]
        [Category("圆信息|配置")]
        [DisplayName("类型")]
        [ItemsSourceProperty("Items")]
        [JsonProperty("type")]
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
                switch (_Type)
                {
                    case "Default":
                        base.Fill = new SolidColorBrush(Color.FromRgb(241, 187, 196));
                        break;
                    case "Parking":
                        base.Fill = new SolidColorBrush(Color.FromRgb(91, 155, 213));
                        break;
                    case "Lift":
                        base.Fill = new SolidColorBrush(Color.FromRgb(0x96, 0x11, 0xEE));
                        break;
                    case "MainRoad":
                        base.Fill = new SolidColorBrush(Color.FromRgb(236, 230, 18));
                        break;
                    case "Connect":
                        base.Fill = new SolidColorBrush(Color.FromRgb(0x33, 0xAE, 0xCC));
                        break;
                    case "Charge":
                        base.Fill = new SolidColorBrush(Color.FromRgb(0x00, 0xFF, 0x66));
                        break;
                    default:
                        break;
                }
            }
        }

        [CustomDisplay]
        [DisplayName("行")]
        [Category("圆信息|信息")]
        [Comment]
        [JsonProperty("r")]
        public int RowIndex
        {
            get; set;
        }
        [CustomDisplay]
        [DisplayName("列")]
        [Category("圆信息|信息")]
        [Comment]
        [JsonProperty("c")]
        public int ColumnIndex
        {
            get; set;
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

        [CustomDisplay]
        [Category("圆信息|信息")]
        [DisplayName("圆-Id")]
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

        [JsonProperty("dtype")]
        public string DrawType
        {
            get; set;
        }
        public CustomEllipse()
        {
            DrawType = "CustomEllipse";
            this.Items = new List<string> { "Default", "Parking", "Lift", "MainRoad", "Connect", "Charge" };
            Type = "Default";
            base.SnapsToDevicePixels = true;
            NewStrokeThickness = 1;
            Stroke = Brushes.Transparent;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new EllipseGeometry(new Rect(0, 0, Width, Height));
            }

        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.PushGuidelineSet(new GuidelineSet());
            base.OnRender(drawingContext);
        }

        public static CustomEllipse ConvertJToken(JToken token)
        {
            string id = token["id"].Value<string>();
            double newWidth = token["w"].Value<double>();
            double newHeight = token["h"].Value<double>();
            double left = token["l"].Value<double>();
            double top = token["t"].Value<double>();
            int rowIndex = token["r"].Value<int>();
            int columnIndex = token["c"].Value<int>();
            string type = token["type"].Value<string>();

            double thickness = token["thickness"].Value<double>();
            //string strstroke = token["stroke"].Value<string>();
            //var converter = new System.Windows.Media.BrushConverter();
            //SolidColorBrush brush = (SolidColorBrush)converter.ConvertFromString(strstroke);

            CustomEllipse customEllipse = new CustomEllipse()
            {
                Id = id,
                NewWidth = newWidth,
                NewHeight = newHeight,
                RowIndex = rowIndex,
                ColumnIndex = columnIndex,
                Left = left,
                Top = top,
                Type = type,
                //NewStroke = brush,
                NewStrokeThickness = thickness
            };

            return customEllipse;
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
