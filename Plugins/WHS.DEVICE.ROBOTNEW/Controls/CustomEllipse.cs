using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace WHS.DEVICE.ROBOTNEW.Controls
{
    [JsonObject(Newtonsoft.Json.MemberSerialization.OptIn)]
    public class CustomEllipse : Shape
    {

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

        private string _Type;

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
                        base.Fill = new SolidColorBrush(Color.FromRgb(0x96,0x11,0xEE));
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


        [JsonProperty("r")]
        public int RowIndex
        {
            get; set;
        }
        [JsonProperty("c")]
        public int ColumnIndex
        {
            get; set;
        }

        [JsonProperty("l")]
        public double Left
        {
            get; set;
        }

        [JsonProperty("t")]
        public double Top
        {
            get; set;
        }

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
                NewStrokeThickness = thickness
            };
            customEllipse.SetCurrentValue(Canvas.LeftProperty, left);
            customEllipse.SetCurrentValue(Canvas.TopProperty, top);
            return customEllipse;
        }

        public Point GetCenterPoint()
        {
            return System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                return new Point(this.Left + (this.Width / 2), this.Top + (this.Height / 2));
            });

        }

    }
}
