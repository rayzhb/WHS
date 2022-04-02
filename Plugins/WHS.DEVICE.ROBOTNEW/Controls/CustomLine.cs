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
using WHS.DEVICE.ROBOTNEW.Commons;

namespace WHS.DEVICE.ROBOTNEW.Controls
{
    [JsonObject(Newtonsoft.Json.MemberSerialization.OptIn)]
    public class CustomLine : Shape
    {
        private SolidColorBrush _NewStroke;

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

        public List<string> Items
        {
            get; set;
        }

        [JsonProperty("dis")]
        public double Distance
        {
            get; set;
        }

        private string _Arrow;
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

        [JsonProperty("sr")]
        public int StartRowIndex
        {
            get; set;
        }

        [JsonProperty("sc")]
        public int StartColumnIndex
        {
            get; set;
        }

        [JsonProperty("er")]
        public int EndRowIndex
        {
            get; set;
        }

        [JsonProperty("ec")]
        public int EndColumnIndex
        {
            get; set;
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
            }
        }


        public CustomLine(Point Start, Point End)
        {
            _Start = Start;
            _End = End;
            DrawType = "CustomLine";
            base.SnapsToDevicePixels = true;
            this.Items = new List<string> { "Default", "Double", "Right", "Left" };
            Arrow = "Default";
            NewStrokeThickness = 1;
            Distance = 1d;
            NewStroke = Brushes.Black;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new LineGeometry(_Start, _End);
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
                    var rightPath = DrawUtility.DrawSingleArrowGeometry(_Start, _End, Math.PI / 12, NewStrokeThickness + 10);
                    drawingContext.DrawGeometry(NewStroke, new Pen(NewStroke, NewStrokeThickness), rightPath);
                    break;
                case "Left":
                    var leftPath = DrawUtility.DrawSingleArrowGeometry(_End, _Start, Math.PI / 12, NewStrokeThickness + 10);
                    drawingContext.DrawGeometry(NewStroke, new Pen(NewStroke, NewStrokeThickness), leftPath);
                    break;
                case "Double":
                    var doublePath = DrawUtility.DrawDoubleArrowGeometry(_End, _Start, Math.PI / 12, NewStrokeThickness + 10);
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
            customLine.SetCurrentValue(Canvas.LeftProperty, left);
            customLine.SetCurrentValue(Canvas.TopProperty, top);
            return customLine;
        }
    }
}
