using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using WHS.DEVICE.ROBOTNEW.Commons;
using WHS.DEVICE.ROBOTNEW.Controls;
using WHS.DEVICE.ROBOTNEW.Models;
using WHS.DEVICE.ROBOTNEW.Views;

namespace WHS.DEVICE.ROBOTNEW.ViewModels
{
    [JsonObject(Newtonsoft.Json.MemberSerialization.OptIn)]
    public class AGVUCViewModel : PropertyChangedBase
    {
        private int _RowIndex;
        [JsonProperty("row")]
        public int RowIndex
        {
            get
            {
                return _RowIndex;
            }
            set
            {
                _RowIndex = value;
                NotifyOfPropertyChange(() => RowIndex);
            }
        }

        private int _ColumnIndex;
        [JsonProperty("column")]
        public int ColumnIndex
        {
            get
            {
                return _ColumnIndex;
            }
            set
            {
                _ColumnIndex = value;
                NotifyOfPropertyChange(() => ColumnIndex);
            }
        }

        private int _Floor;
        [JsonProperty("floor")]
        public int Floor
        {
            get
            {
                return _Floor;
            }
            set
            {
                _Floor = value;
                NotifyOfPropertyChange(() => Floor);
            }
        }

        [JsonProperty("left")]
        public double Left
        {
            get; set;
        }

        [JsonProperty("top")]
        public double Top
        {
            get; set;
        }

        private double _Width;
        [JsonProperty("width")]
        public double Width
        {
            get
            {
                return _Width;
            }
            set
            {
                _Width = value;
                NotifyOfPropertyChange(() => Width);
            }
        }


        private double _Height;
        [JsonProperty("height")]
        public double Height
        {
            get
            {
                return _Height;
            }
            set
            {
                _Height = value;
                NotifyOfPropertyChange(() => Height);
            }
        }


        private bool _IsOpen;
        public bool IsOpen
        {
            get
            {
                return _IsOpen;
            }
            set
            {
                _IsOpen = value;
                NotifyOfPropertyChange(() => IsOpen);
            }
        }

        /// <summary>
        /// 机器人编码
        /// </summary>
        private string _Code;
        [JsonProperty("code")]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
                NotifyOfPropertyChange(() => Code);
            }
        }

        private BitmapSource _ImageSource;
        public BitmapSource ImageSource
        {
            get
            {
                return _ImageSource;
            }
            set
            {
                _ImageSource = value;
                NotifyOfPropertyChange(() => ImageSource);
            }
        }


        /// <summary>
        /// 电量
        /// </summary>
        private string _Power;
        [JsonProperty("power")]
        public string Power
        {
            get
            {
                return _Power;
            }
            set
            {
                _Power = value;
                NotifyOfPropertyChange(() => Power);
            }
        }

        /// <summary>
        /// 电量（计算用）
        /// </summary>
        public int PowerI
        {
            get; set;
        }

        /// <summary>
        /// 充电阈值（满电1000）
        /// </summary>
        public const int ChargeThreshold = 800;

        /// <summary>
        /// 转弯等待时间(ms)
        /// </summary>
        public const int TurnTime = 3000;

        /// <summary>
        /// 充电中
        /// </summary>
        [JsonProperty("isCharging")]
        public bool Charging
        {
            get; set;
        }

        /// <summary>
        /// 休息中
        /// </summary>
        [JsonProperty("isResting")]
        public bool Resting
        {
            get; set;
        }

        /// <summary>
        /// 当前机器的姿态
        /// </summary>
        public int CurrentAttitude
        {
            get; set;
        }

        public Point? Destination
        {
            get; set;
        }

        /// <summary>
        /// 移动总路线
        /// </summary>
        public List<Point3D?> Path
        {
            get; set;
        } = new List<Point3D?>();

        /// <summary>
        /// 转弯点
        /// </summary>
        public List<Point3D?> WaitPoints
        {
            get; set;
        } = new List<Point3D?>();

        /// <summary>
        /// 当前移动中路径
        /// </summary>
        public List<Point3D?> CurPath
        {
            get; set;
        } = new List<Point3D?>();

        /// <summary>
        /// 每一层所有的休息位
        /// </summary>
        //public Dictionary<int, Dictionary<Point?, bool>> RestPositions;
        /// <summary>
        /// 每一层所有的充电位
        /// </summary>
        //public Dictionary<int, Dictionary<Point?, bool>> ChargePositions;
        /// <summary>
        /// 等待任务时间
        /// </summary>
        public DateTime? RestWaitingTime
        {
            get; set;
        }
        /// <summary>
        /// 等待移动时间
        /// </summary>
        public DateTime? TaskWaitingTime
        {
            get; set;
        }

        public Timer Timer
        {
            get; set;
        }

        public ConcurrentQueue<AGVCommand> MoveList
        {
            get; set;
        } = new ConcurrentQueue<AGVCommand>();

        private ObservableCollection<MapModel> AgvPlatformModels
        {
            get; set;
        }

        private ConcurrentBag<AGVUCView> AllAGV
        {
            get; set;
        }

        //public Dictionary<int, Dictionary<int, int>> RoadStatuses
        //{
        //    get; set;
        //}

        public int MainRoad
        {
            get; set;
        }
        /// <summary>
        /// 10 空闲
        /// 20 路径计算中
        /// 30 移动中
        /// </summary>
        public string Status
        {
            get; set;
        }

        public AGVUCViewModel()
        {
            ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.AGV.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        public delegate void LocationEventHandler(object? sender, AGVUCViewModel viewModel);

        public event LocationEventHandler LocationEvent;

        private void OnLocationEvent(object? sender, AGVUCViewModel viewModel)
        {
            if (LocationEvent != null)
                LocationEvent(sender, viewModel);
        }

        public void locationClick(object sender, object viewmodel)
        {
            OnLocationEvent(sender, this);
        }

        public void Move(ObservableCollection<MapModel> agvPlatformModels, ConcurrentBag<AGVUCView> allAGV)
        {
            AgvPlatformModels = agvPlatformModels;
            AllAGV = allAGV;
            Task.Run(() =>
            {
                lock (MoveUtility.RoadStatuses)
                {
                    CalMoving();
                    AGVCommand command;
                    if (MoveList.TryDequeue(out command))
                    {
                        ControlCommand(command);
                    }
                }
            });
        }

        private void ControlCommand(AGVCommand command)
        {
            AGVUCView view = this.GetAGVUCView(Code);
            double left = Left;
            double top = Top;
            var pvm = AgvPlatformModels.Where(w => w.Floor == Floor).FirstOrDefault();
            var ellipse = pvm.GetEllipse(RowIndex, ColumnIndex);
            if (ellipse == null)
                return;
            var c_Point = ellipse.GetCenterPoint();
            var ellipseLines = pvm.GetEliipseLines(RowIndex, ColumnIndex);
            switch (command)
            {
                case AGVCommand.left:
                    if (ellipseLines.LeftLine == null)
                        return;
                    var leftellipse = pvm.GetEllipse(RowIndex, ellipseLines.LeftLine.StartColumnIndex);
                    if (leftellipse == null)
                        return;
                    var left_c_Point = leftellipse.GetCenterPoint();
                    var left_Arrow = DrawUtility.DrawSingleArrow(c_Point, left_c_Point, Math.PI / 12, 13);
                    Move(view, new Point(left, top),
                        new Point(left + (left_c_Point.X - c_Point.X),
                            top + (left_c_Point.Y - c_Point.Y)),
                        left_Arrow, pvm, Code, AGVCommand.left, ellipseLines.LeftLine);
                    ColumnIndex = ellipseLines.LeftLine.StartColumnIndex;
                    Left = left + (left_c_Point.X - c_Point.X);
                    Top = top + (left_c_Point.Y - c_Point.Y);
                    break;
                case AGVCommand.right:
                    if (ellipseLines.RightLine == null)
                        return;
                    var rightellipse = pvm.GetEllipse(RowIndex, ellipseLines.RightLine.EndColumnIndex);
                    if (rightellipse == null)
                        return;
                    var right_c_Point = rightellipse.GetCenterPoint();
                    var right_Arrow = DrawUtility.DrawSingleArrow(c_Point, right_c_Point, Math.PI / 12, 13);

                    Move(view, new Point(left, top),
                        new Point(left + (right_c_Point.X - c_Point.X),
                            top + (right_c_Point.Y - c_Point.Y)),
                        right_Arrow, pvm, Code, AGVCommand.right, ellipseLines.RightLine);
                    ColumnIndex = ellipseLines.RightLine.EndColumnIndex;

                    Left = left + (right_c_Point.X - c_Point.X);
                    Top = top + (right_c_Point.Y - c_Point.Y);
                    break;
                case AGVCommand.up:
                    if (ellipseLines.UpLine == null)
                        return;
                    var uptellipse = pvm.GetEllipse(ellipseLines.UpLine.StartRowIndex, ColumnIndex);
                    if (uptellipse == null)
                        return;
                    var up_c_Point = uptellipse.GetCenterPoint();
                    var up_Arrow = DrawUtility.DrawSingleArrow(c_Point, up_c_Point, Math.PI / 12, 13);

                    Move(view, new Point(left, top),
                        new Point(left + (up_c_Point.X - c_Point.X),
                            top + (up_c_Point.Y - c_Point.Y)),
                        up_Arrow, pvm, Code, AGVCommand.up, ellipseLines.UpLine);
                    RowIndex = ellipseLines.UpLine.StartRowIndex;
                    Left = left + (up_c_Point.X - c_Point.X);
                    Top = top + (up_c_Point.Y - c_Point.Y);
                    break;
                case AGVCommand.down:
                    if (ellipseLines.DownLine == null)
                        return;
                    var downtellipse = pvm.GetEllipse(ellipseLines.DownLine.EndRowIndex, ColumnIndex);
                    if (downtellipse == null)
                        return;
                    var down_c_Point = downtellipse.GetCenterPoint();
                    var down_Arrow = DrawUtility.DrawSingleArrow(c_Point, down_c_Point, Math.PI / 12, 13);

                    Move(view, new Point(left, top),
                        new Point(left + (down_c_Point.X - c_Point.X),
                            top + (down_c_Point.Y - c_Point.Y)),
                        down_Arrow, pvm, Code, AGVCommand.down, ellipseLines.DownLine);
                    RowIndex = ellipseLines.DownLine.EndRowIndex;
                    Left = left + (down_c_Point.X - c_Point.X);
                    Top = top + (down_c_Point.Y - c_Point.Y);
                    break;
                default:
                    break;
            }
            //移动任务完成
            if (MoveList.Count == 0)
            {
                ClearPath(pvm);
                //如果小车最终不在休息位或充电位上，启用定时器
                //var restPoint = RestPositions[Floor].Keys
                //    .Where(t => t.Value.X == Path.Last().Value.X
                //    && t.Value.Y == Path.Last().Value.Y).FirstOrDefault();
                //var chargePoint = ChargePositions[Floor].Keys
                //    .Where(t => t.Value.X == Path.Last().Value.X
                //    && t.Value.Y == Path.Last().Value.Y).FirstOrDefault();
                //if (chargePoint == null && restPoint == null)
                //{
                //Timer.Change(0, 1000);
                RestWaitingTime = DateTime.Now;
                //}
                Destination = null;
                Path.Clear();
                WaitPoints.Clear();
                Status = "10";
                if (MainRoad != -1)
                {
                    lock (MoveUtility.RoadStatuses)
                    {
                        MoveUtility.RoadStatuses[Floor][MainRoad] -= 1;
                    }
                }
                MainRoad = -1;
            }
        }

        public void Move(UIElement uIElement, Point start, Point end,
            Polygon myPolygon, MapModel pvm, string agvCode, AGVCommand oldCommand, CustomLine line,
            bool auto = true)
        {
            var path = Path.Select(t => pvm.GetEllipse(Convert.ToInt32(t.Value.Y), Convert.ToInt32(t.Value.X))).Select(t => new{ t.Left, t.Top }).ToList();
            MessageUtility.MessageCallBackAll("agv_move", "ControlCommand", new Cb_AGVMove()
            {
                Code = this.Code,
                Command = oldCommand,
                Floor = Floor,
                StartPoint = start,
                Endoint = end,
                Distance = line.Distance,
                Power = Power,
                Path = path
            }); ;
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                myPolygon.Tag = agvCode;//AGV编码
                pvm.canvasPathItemsSource.Add(myPolygon);
                double length = line.Distance;
                CustomStoryboard storyboard = new CustomStoryboard(myPolygon);   //创建Storyboard对象
                DoubleAnimation XAnimation = new DoubleAnimation(
                  start.X,
                  end.X,
                  new Duration(TimeSpan.FromSeconds(1 * length))
                );
                Storyboard.SetTarget(XAnimation, uIElement);
                Storyboard.SetTargetProperty(XAnimation, new PropertyPath("(Canvas.Left)"));

                DoubleAnimation YAnimation = new DoubleAnimation(
                        start.Y,
                        end.Y,
                        new Duration(TimeSpan.FromSeconds(1 * length))
                );
                Storyboard.SetTarget(YAnimation, uIElement);
                Storyboard.SetTargetProperty(YAnimation, new PropertyPath("(Canvas.Top)"));

                storyboard.Children.Add(XAnimation);
                storyboard.Children.Add(YAnimation);

                storyboard.CustomCompleted += (sender, data, e) =>
                {
                    storyboard.Dispose();

                    var p = data as Polygon;
                    if (p != null)
                    {
                        List<object> removelist = new List<object>();
                        foreach (var item in pvm.canvasPathItemsSource)
                        {
                            if (item is Polygon)
                            {
                                var p2 = item as Polygon;
                                if (p2.Tag == p.Tag)
                                {
                                    removelist.Add(item);
                                }
                            }
                        }
                        foreach (var remove in removelist)
                        {
                            pvm.canvasPathItemsSource.Remove(remove);
                        }
                        System.Diagnostics.Debug.WriteLine("CustomCompleted");
                    }
                    PowerI -= 2;
                    Power = (PowerI * 1.0 / (10 * 1.0)).ToString();
                    if (auto)
                    {
                        Task.Run(() =>
                        {
                            if (MoveList.Count > 0)
                            {
                                CalMoving();
                            }
                            AGVCommand command;
                            if (MoveList.TryDequeue(out command))
                            {
                                if (command != oldCommand)
                                {
                                    Thread.Sleep(TurnTime);
                                }
                                ControlCommand(command);
                            }
                        });
                    }
                    else
                    {
                        Status = "10";
                        RestWaitingTime = DateTime.Now;
                    }
                };
                storyboard.Begin();
            });
        }

        private void ClearPath(MapModel pvm)
        {
            //擦除总路线
            List<object> removelist = new List<object>();
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var item in pvm.canvasPathItemsSource)
                {
                    if (item is Polyline)
                    {
                        var p2 = item as Polyline;
                        if (p2.Tag.ToString() == Code)
                        {
                            removelist.Add(item);
                        }
                    }
                }
                foreach (var remove in removelist)
                {
                    pvm.canvasPathItemsSource.Remove(remove);
                }
            });
        }

        private void CalMoving()
        {
            //小车在起点
            if (Path.First().Value.X == ColumnIndex && Path.First().Value.Y == RowIndex)
            {
                CurPath.Clear();
                while (OtherAgvMoving(true))
                {
                    Thread.Sleep(100);
                }
            }
            //小车在各个转弯点时，判断接下来一段路是否有别的车占据
            else if (WaitPoints.Exists(t => t.Value.X == ColumnIndex && t.Value.Y == RowIndex))
            {
                CurPath.Clear();
                while (OtherAgvMoving())
                {
                    Thread.Sleep(100);
                }
            }
        }

        private bool OtherAgvMoving(bool atStart = false)
        {
            //在起点
            if (atStart)
            {
                //判断起点到第一个转弯点
                if (WaitPoints != null && WaitPoints.Count > 0)
                {
                    List<Point3D?> waitToMovePoints = new List<Point3D?>();
                    foreach (var item in Path)
                    {
                        Point3D? point = new Point3D(item.Value.X, item.Value.Y, item.Value.Z);
                        waitToMovePoints.Add(point);
                        //最后一个点即第一个转弯点
                        if (WaitPoints[0].Value.X == item.Value.X && WaitPoints[0].Value.Y == item.Value.Y)
                        {
                            break;
                        }
                    }
                    bool otherAgvMoving = JudgeMoving(waitToMovePoints);
                    if (!otherAgvMoving)
                    {
                        CurPath = waitToMovePoints;
                    }
                    return otherAgvMoving;
                }
                //判断起点到终点
                else
                {
                    bool otherAgvMoving = JudgeMoving(Path);
                    if (!otherAgvMoving)
                    {
                        foreach (var item in Path)
                        {
                            Point3D? point = new Point3D(item.Value.X, item.Value.Y, item.Value.Z);
                            CurPath.Add(point);
                        }
                    }
                    return otherAgvMoving;
                }
            }
            //小车在中间点
            else
            {
                Point3D? currentPoint = WaitPoints.Where(t => t.Value.X == ColumnIndex && t.Value.Y == RowIndex).FirstOrDefault();
                int index = WaitPoints.IndexOf(currentPoint);
                //最后一段
                if (index == WaitPoints.Count - 1)
                {
                    List<Point3D?> waitToMovePoints1 = new List<Point3D?>();
                    bool startCal = false;
                    foreach (var item in Path)
                    {
                        if (item.Value.X == currentPoint.Value.X && item.Value.Y == currentPoint.Value.Y)
                        {
                            startCal = true;
                        }
                        if (startCal)
                        {
                            Point3D? point = new Point3D(item.Value.X, item.Value.Y, item.Value.Z);
                            waitToMovePoints1.Add(point);
                        }
                    }
                    bool otherAgvMoving = JudgeMoving(waitToMovePoints1);
                    if (!otherAgvMoving)
                    {
                        CurPath = waitToMovePoints1;
                    }
                    return otherAgvMoving;
                }
                //中间某一段
                else
                {
                    Point3D? nextPoint = new Point3D(WaitPoints[index + 1].Value.X, WaitPoints[index + 1].Value.Y, WaitPoints[index + 1].Value.Z);
                    List<Point3D?> waitToMovePoints1 = new List<Point3D?>();
                    bool startCal = false;
                    foreach (var item in Path)
                    {
                        if (item.Value.X == currentPoint.Value.X && item.Value.Y == currentPoint.Value.Y)
                        {
                            startCal = true;
                        }
                        if (startCal)
                        {
                            Point3D? point = new Point3D(item.Value.X, item.Value.Y, item.Value.Z);
                            waitToMovePoints1.Add(point);
                        }
                        if (item.Value.X == nextPoint.Value.X && item.Value.Y == nextPoint.Value.Y)
                        {
                            break;
                        }
                    }
                    bool otherAgvMoving = JudgeMoving(waitToMovePoints1);
                    if (!otherAgvMoving)
                    {
                        CurPath = waitToMovePoints1;
                    }
                    return otherAgvMoving;
                }
            }
        }

        private bool JudgeMoving(List<Point3D?> waitToMovePath)
        {
            var agvs = GetAGVUCViewModel(Floor);
            foreach (var agv in agvs)
            {
                //有小车当前位置在路径中
                if (waitToMovePath.Exists(t => t.Value.X == agv.ColumnIndex && t.Value.Y == agv.RowIndex))
                {
                    return true;
                }
                //有小车移动中路径存在点在当前路径中
                if (agv.CurPath != null && agv.CurPath.Count > 0)
                {
                    bool startCal = false;
                    foreach (var point in agv.CurPath)
                    {
                        if (point.Value.X == agv.ColumnIndex && point.Value.Y == agv.RowIndex)
                        {
                            startCal = true;
                        }
                        if (startCal)
                        {
                            if (waitToMovePath.Exists(t => t.Value.X == point.Value.X && t.Value.Y == point.Value.Y))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }


        private AGVUCView GetAGVUCView(string code)
        {
            return System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                AGVUCView view = null;
                foreach (var item in AllAGV)
                {
                    if (((AGVUCViewModel)item.DataContext).Code == code)
                    {
                        view = item;
                        break;
                    }
                }
                return view;
            });
        }
        private List<AGVUCViewModel> GetAGVUCViewModel(int floor)
        {
            return System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                List<AGVUCViewModel> vmList = new List<AGVUCViewModel>();
                foreach (var item in AllAGV)
                {
                    if (((AGVUCViewModel)item.DataContext).Code != Code && ((AGVUCViewModel)item.DataContext).Floor == floor)
                    {
                        vmList.Add((AGVUCViewModel)item.DataContext);
                    }
                }
                return vmList;
            });
        }

        /// <summary>
        /// 定时任务相关
        /// 充电时定时增加电量
        /// 调度去休息位
        /// 调度去充电位
        /// </summary>
        /// <param name="obj"></param>
        public void CheckMoveToRest(Object obj)
        {
            lock (Timer)
            {
                Timer.Change(Timeout.Infinite, 1000);
                if (Charging)
                {
                    if (RestWaitingTime != null)
                    {
                        if (PowerI < 1000)
                        {
                            if (PowerI + 1 > 1000)
                            {
                                PowerI = 1000;
                                Power = (PowerI * 1.0 / (10 * 1.0)).ToString();
                            }
                            else
                            {
                                PowerI += 1;
                                Power = (PowerI * 1.0 / (10 * 1.0)).ToString();
                            }
                            var p = AgvPlatformModels.Where(t => t.Floor == Floor).FirstOrDefault().GetEllipse(RowIndex, ColumnIndex);
                            MessageUtility.MessageCallBackAll("agv_move", "ControlCommand", new Cb_AGVMove()
                            {
                                Code = this.Code,
                                Command = AGVCommand.charge,
                                Floor = Floor,
                                StartPoint = new Point(p.Left, p.Top),
                                Endoint = new Point(p.Left, p.Top),
                                Distance = 0,
                                Power = Power,
                                Path = null
                            });
                        }
                        else
                        {
                            Charging = false;
                        }
                    }
                }
                else
                {
                    if (PowerI > ChargeThreshold)
                    {
                        lock (MoveUtility.RestPositions)
                        {
                            if (RestWaitingTime != null)
                            {
                                //超过10s没有任务且不在充电不在休息，调度去休息位
                                if (!Resting && (DateTime.Now - RestWaitingTime).Value.TotalSeconds > 10)
                                {
                                    //判断当前层是否有可用休息位
                                    var restPosition = MoveUtility.RestPositions[Floor].Where(t => t.Value).FirstOrDefault();
                                    if (restPosition.Key != null)
                                    {
                                        //先把该休息位占用了
                                        MoveUtility.RestPositions[Floor][restPosition.Key] = false;
                                        string msg = MoveUtility.Move(this, new Point(Convert.ToInt32(restPosition.Key.Value.X), Convert.ToInt32(restPosition.Key.Value.Y)));
                                        if (!string.IsNullOrWhiteSpace(msg))
                                        {
                                            MoveUtility.RestPositions[Floor][restPosition.Key] = true;
                                        }
                                        else
                                        {
                                            Resting = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //自动调度去充电
                        lock (MoveUtility.ChargePositions)
                        {
                            if (RestWaitingTime != null && (DateTime.Now - RestWaitingTime).Value.TotalSeconds > 3)
                            {
                                //判断当前层是否有可用充电位
                                var chargePosition = MoveUtility.ChargePositions[Floor].Where(t => t.Value).FirstOrDefault();
                                if (chargePosition.Key != null)
                                {
                                    //先把该充电位占用了
                                    MoveUtility.ChargePositions[Floor][chargePosition.Key] = false;
                                    string msg = MoveUtility.Move(this, new Point(Convert.ToInt32(chargePosition.Key.Value.X), Convert.ToInt32(chargePosition.Key.Value.Y)));
                                    if (!string.IsNullOrWhiteSpace(msg))
                                    {
                                        MoveUtility.ChargePositions[Floor][chargePosition.Key] = true;
                                    }
                                    else
                                    {
                                        Charging = true;
                                    }
                                }
                            }
                        }
                    }
                }
                Timer.Change(1000, 1000);
            }
        }
    }
}
