using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WHS.DEVICE.ROBOTNEW.Commons
{
    internal static class DrawUtility
    {

        internal static Polygon DrawSingleArrow(Point start, Point end, double arrowAngle = Math.PI / 12, double arrowLength = 20)
        {
            return System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                double angleOri = Math.Atan((end.Y - start.Y) / (end.X - start.X));      // 起始点线段夹角
                double angleDown = angleOri - arrowAngle;   // 箭头扩张角度
                double angleUp = angleOri + arrowAngle;     // 箭头扩张角度
                int xdirectionFlag = -1;    // 方向标识
                int ydirectionFlag = -1;    // 方向标识;
                if (end.X < start.X && end.Y > start.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }
                else if (end.X < start.X && end.Y < start.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }
                else if (end.X < start.X && end.Y == start.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }

                double x3 = end.X + ((xdirectionFlag * arrowLength) * Math.Cos(angleDown));   // 箭头第三个点的坐标
                double y3 = end.Y + ((ydirectionFlag * arrowLength) * Math.Sin(angleDown));
                double x4 = end.X + ((xdirectionFlag * arrowLength) * Math.Cos(angleUp));     // 箭头第四个点的坐标
                double y4 = end.Y + ((ydirectionFlag * arrowLength) * Math.Sin(angleUp));

                Point point3 = new Point(x3, y3);   // 箭头第三个点
                Point point4 = new Point(x4, y4);   // 箭头第四个点
                Point[] points = new Point[] { start, end, point3, point4, end };   // 多边形，起点 --> 终点 --> 第三点 --> 第四点 --> 终点
                Polygon myPolygon = new Polygon
                {
                    Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00)),
                    StrokeThickness = 1,      // 多边形线宽
                    StrokeLineJoin = PenLineJoin.Round,
                    Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00)),   // 填充
                };
                for (int i = 0; i < points.Length; i++)
                {
                    myPolygon.Points.Add(points[i]);
                }
                return myPolygon;
            });
        }


        internal static Polygon DrawDoubleArrow(Point start, Point end, double arrowAngle = Math.PI / 12, double arrowLength = 20)
        {
            return System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                double angleOri = Math.Atan((end.Y - start.Y) / (end.X - start.X));      // 起始点线段夹角
                double angleDown = angleOri - arrowAngle;   // 箭头扩张角度
                double angleUp = angleOri + arrowAngle;     // 箭头扩张角度
                int xdirectionFlag = -1;    // 方向标识
                int ydirectionFlag = -1;    // 方向标识;
                if (end.X < start.X && end.Y > start.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }
                else if (end.X < start.X && end.Y < start.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }
                else if (end.X < start.X && end.Y == start.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }

                double x3 = end.X + ((xdirectionFlag * arrowLength) * Math.Cos(angleDown));   // 箭头第三个点的坐标
                double y3 = end.Y + ((ydirectionFlag * arrowLength) * Math.Sin(angleDown));
                double x4 = end.X + ((xdirectionFlag * arrowLength) * Math.Cos(angleUp));     // 箭头第四个点的坐标
                double y4 = end.Y + ((ydirectionFlag * arrowLength) * Math.Sin(angleUp));

                Point point3 = new Point(x3, y3);   // 箭头第三个点
                Point point4 = new Point(x4, y4);   // 箭头第四个点

                //left
                double left_angleOri = Math.Atan((start.Y - end.Y) / (start.X - end.X));      // 起始点线段夹角
                double left_angleDown = left_angleOri - arrowAngle;   // 箭头扩张角度
                double left_angleUp = left_angleOri + arrowAngle;     // 箭头扩张角度
                int left_xdirectionFlag = -1;    // 方向标识
                int left_ydirectionFlag = -1;    // 方向标识;
                if (start.X < end.X && start.Y > end.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }
                else if (start.X < end.X && start.Y < end.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }
                else if (start.X < end.X && start.Y == end.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }
                double x1 = start.X + ((left_xdirectionFlag * arrowLength) * Math.Cos(angleDown));   // 箭头第三个点的坐标
                double y1 = start.Y + ((left_ydirectionFlag * arrowLength) * Math.Sin(angleDown));
                double x2 = start.X + ((left_xdirectionFlag * arrowLength) * Math.Cos(angleUp));     // 箭头第四个点的坐标
                double y2 = start.Y + ((left_ydirectionFlag * arrowLength) * Math.Sin(angleUp));

                Point point1 = new Point(x1, y1);   // 箭头第三个点
                Point point2 = new Point(x2, y2);   // 箭头第四个点



                Point[] points = new Point[] { start, point1, point2, start, end, point3, point4, end };   // 多边形，起点 --> 终点 --> 第三点 --> 第四点 --> 终点
                Polygon myPolygon = new Polygon
                {
                    Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00)),
                    StrokeThickness = 1,      // 多边形线宽
                    StrokeLineJoin = PenLineJoin.Round,
                    Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00)),   // 填充
                };
                for (int i = 0; i < points.Length; i++)
                {
                    myPolygon.Points.Add(points[i]);
                }
                return myPolygon;
            });
        }

        internal static Polyline DrawPathPolyLine(List<Point> points)
        {
            return System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Polyline polyline = new Polyline
                {
                    Stroke = GetRandomStroke(),
                    StrokeThickness = 3,      // 多边形线宽
                    StrokeLineJoin = PenLineJoin.Round,
                    //StrokeDashArray = new DoubleCollection(points.Count)
                    //Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x1F, 0xC5, 0x40)),   // 填充
                };
                for (int i = 0; i < points.Count; i++)
                {
                    //polyline.StrokeDashArray.Add(1);
                    polyline.Points.Add(points[i]);
                }
                return polyline;
            });
        }


        internal static StreamGeometry DrawSingleArrowGeometry(Point start, Point end, double arrowAngle = Math.PI / 12, double arrowLength = 20)
        {
            return System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                double angleOri = Math.Atan((end.Y - start.Y) / (end.X - start.X));      // 起始点线段夹角
                double angleDown = angleOri - arrowAngle;   // 箭头扩张角度
                double angleUp = angleOri + arrowAngle;     // 箭头扩张角度
                int xdirectionFlag = -1;    // 方向标识
                int ydirectionFlag = -1;    // 方向标识;
                if (end.X < start.X && end.Y > start.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }
                else if (end.X < start.X && end.Y < start.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }
                else if (end.X < start.X && end.Y == start.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }

                double x3 = end.X + ((xdirectionFlag * arrowLength) * Math.Cos(angleDown));   // 箭头第三个点的坐标
                double y3 = end.Y + ((ydirectionFlag * arrowLength) * Math.Sin(angleDown));
                double x4 = end.X + ((xdirectionFlag * arrowLength) * Math.Cos(angleUp));     // 箭头第四个点的坐标
                double y4 = end.Y + ((ydirectionFlag * arrowLength) * Math.Sin(angleUp));

                Point point3 = new Point(x3, y3);   // 箭头第三个点
                Point point4 = new Point(x4, y4);   // 箭头第四个点

                StreamGeometry streamGeometry = new StreamGeometry();
                using (StreamGeometryContext geometryContext = streamGeometry.Open())
                {
                    geometryContext.BeginFigure(start, true, true);
                    PointCollection points = new PointCollection
                                             {
                                                 end, point3, point4, end
                                             };
                    geometryContext.PolyLineTo(points, true, true);
                }
                return streamGeometry;
            });
        }


        internal static StreamGeometry DrawDoubleArrowGeometry(Point start, Point end, double arrowAngle = Math.PI / 12, double arrowLength = 20)
        {
            return System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                //right
                double angleOri = Math.Atan((end.Y - start.Y) / (end.X - start.X));      // 起始点线段夹角
                double angleDown = angleOri - arrowAngle;   // 箭头扩张角度
                double angleUp = angleOri + arrowAngle;     // 箭头扩张角度
                int xdirectionFlag = -1;    // 方向标识
                int ydirectionFlag = -1;    // 方向标识;
                if (end.X < start.X && end.Y > start.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }
                else if (end.X < start.X && end.Y < start.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }
                else if (end.X < start.X && end.Y == start.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }

                double x3 = end.X + ((xdirectionFlag * arrowLength) * Math.Cos(angleDown));   // 箭头第三个点的坐标
                double y3 = end.Y + ((ydirectionFlag * arrowLength) * Math.Sin(angleDown));
                double x4 = end.X + ((xdirectionFlag * arrowLength) * Math.Cos(angleUp));     // 箭头第四个点的坐标
                double y4 = end.Y + ((ydirectionFlag * arrowLength) * Math.Sin(angleUp));

                Point point3 = new Point(x3, y3);   // 箭头第三个点
                Point point4 = new Point(x4, y4);   // 箭头第四个点

                //left
                double left_angleOri = Math.Atan((start.Y - end.Y) / (start.X - end.X));      // 起始点线段夹角
                double left_angleDown = left_angleOri - arrowAngle;   // 箭头扩张角度
                double left_angleUp = left_angleOri + arrowAngle;     // 箭头扩张角度

                int left_xdirectionFlag = -1;    // 方向标识
                int left_ydirectionFlag = -1;    // 方向标识;
                if (start.X < end.X && start.Y > end.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }
                else if (start.X < end.X && start.Y < end.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }
                else if (start.X < end.X && start.Y == end.Y)
                {
                    xdirectionFlag = 1;
                    ydirectionFlag = 1;
                }



                double x1 = start.X + ((left_xdirectionFlag * arrowLength) * Math.Cos(angleDown));   // 箭头第三个点的坐标
                double y1 = start.Y + ((left_ydirectionFlag * arrowLength) * Math.Sin(angleDown));
                double x2 = start.X + ((left_xdirectionFlag * arrowLength) * Math.Cos(angleUp));     // 箭头第四个点的坐标
                double y2 = start.Y + ((left_ydirectionFlag * arrowLength) * Math.Sin(angleUp));

                Point point1 = new Point(x1, y1);   // 箭头第三个点
                Point point2 = new Point(x2, y2);   // 箭头第四个点


                StreamGeometry streamGeometry = new StreamGeometry();
                using (StreamGeometryContext geometryContext = streamGeometry.Open())
                {
                    geometryContext.BeginFigure(start, true, true);
                    PointCollection points = new PointCollection
                                             {
                                               point1, point2,start, end, point3, point4, end
                                             };
                    geometryContext.PolyLineTo(points, true, true);
                }
                return streamGeometry;
            });
        }

        private static Brush GetRandomStroke()
        {
            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            int R = ran.Next(255);
            int G = ran.Next(255);
            int B = ran.Next(255);
            B = (R + G > 400) ? R + G - 400 : B;//0 : 380 - R - G;
            B = (B > 255) ? 255 : B;
            return new SolidColorBrush(Color.FromRgb(Convert.ToByte(R), Convert.ToByte(G), Convert.ToByte(B)));
        }

        internal static FrameworkElement DrawWatermark(int floor, double width, double height)
        {
            System.Windows.Controls.TextBox text = new System.Windows.Controls.TextBox();
            text.Name = "tb_watermark";
            text.Text = $"第{floor}层";
            text.FontSize = 96;
            text.Background = Brushes.Transparent;
            text.Opacity = 0.2;
            text.VerticalAlignment = VerticalAlignment.Center;
            text.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            text.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            text.VerticalContentAlignment = VerticalAlignment.Center;
            text.TextAlignment = TextAlignment.Center;
            text.Width = width;
            text.Height = height;
            text.SetValue(System.Windows.Controls.Canvas.LeftProperty, 0d);
            text.SetValue(System.Windows.Controls.Canvas.TopProperty, 0d);
            return text;
        }
    }
}
