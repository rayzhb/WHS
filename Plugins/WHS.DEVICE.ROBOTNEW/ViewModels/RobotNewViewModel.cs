using Caliburn.Micro;
using DijkstraAlgorithm;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using WHS.DEVICE.ROBOTNEW.Commons;
using WHS.DEVICE.ROBOTNEW.Controls;
using WHS.DEVICE.ROBOTNEW.Models;
using WHS.DEVICE.ROBOTNEW.Views;
using WHS.Infrastructure;
using WHS.Infrastructure.Utils;

namespace WHS.DEVICE.ROBOTNEW.ViewModels
{
    public class RobotNewViewModel : Screen
    {
        public ObservableCollection<MapModel> agvPlatformModels
        {
            get; set;
        }

        public ThreadSafeObservableCollection<AGVUCViewModel> agvViewModelItemsSource
        {
            get; set;
        }

        public static object selectAGVItem
        {
            get; set;
        }

        private int _tabSelectedIndex;
        public int tabSelectedIndex
        {
            get
            {
                return _tabSelectedIndex;
            }
            set
            {
                _tabSelectedIndex = value;
                NotifyOfPropertyChange(() => tabSelectedIndex);
            }
        }

        public MapModel tabSelectedItem
        {
            get; set;
        }


        public double currentCanvasWidth
        {
            get; set;
        }

        public double currentCanvasHeight
        {
            get; set;
        }
        public string ReadFile
        {
            get; set;
        }

        private string _PageSize;
        public string PageSize
        {
            get
            {
                return _PageSize;
            }
            set
            {
                _PageSize = value;
                NotifyOfPropertyChange(() => PageSize);
            }
        }

        private ConcurrentBag<AGVUCView> allAGV = new ConcurrentBag<AGVUCView>();

        //路径计算用到
        /// <summary>
        /// 每一层的主通道对应的坐标和所属主通道
        /// key:层数,
        /// val: {key:坐标点, val:主通道在第几行}
        /// </summary>
        //private Dictionary<int, Dictionary<Point, int>> MainRoadPositions;

        /// <summary>
        /// 每一层各有哪几条主通道
        /// key:层数
        /// val:每层主通道在第几行
        /// </summary>
        //private Dictionary<int, List<int>> MainRoads;

        /// <summary>
        /// 每一层主通道有多少任务
        /// key:层数
        /// val:{key: 主通道在第几行, val:该主通道现存任务数量}
        /// </summary>
        //private Dictionary<int, Dictionary<int, int>> RoadStatuses;

        /// <summary>
        /// 每一层休息位的坐标
        /// key: 层数,
        /// val: {key:坐标点, val:该休息位是否可用}
        /// </summary>
        //private Dictionary<int, Dictionary<Point?, bool>> RestPositions;

        /// <summary>
        /// 每一层充电位的坐标
        /// key:层数
        /// val: {key:坐标点, val:该休息位是否可用}
        /// </summary>
        //private Dictionary<int, Dictionary<Point?, bool>> ChargePositions;

        /// <summary>
        /// 提升机所在位置
        /// key:坐标
        /// value:层
        /// </summary>
        //private Dictionary<Point?, int> LiftPosition;

        private int _x;
        public int x
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
                NotifyOfPropertyChange(() => x);
            }
        }
        private int _y;
        public int y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                NotifyOfPropertyChange(() => y);
            }
        }

        private string _LiftMsg;
        public string LiftMsg
        {
            get
            {
                return _LiftMsg;
            }
            set
            {
                _LiftMsg = value;
                NotifyOfPropertyChange(() => LiftMsg);
            }
        }

        public int targetFloor
        {
            get; set;
        }

        public RobotNewViewModel()
        {
            agvPlatformModels = new ObservableCollection<MapModel>();
            agvViewModelItemsSource = new ThreadSafeObservableCollection<AGVUCViewModel>();
            ReadFile = "D:\\RCSMap.map";//Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "RCSMap.map");

            //MainRoadPositions = new Dictionary<int, Dictionary<Point, int>>();
            MoveUtility.MainRoads = new Dictionary<int, List<int>>();
            MoveUtility.RoadStatuses = new Dictionary<int, Dictionary<int, int>>();
            MoveUtility.RestPositions = new Dictionary<int, Dictionary<Point?, bool>>();
            MoveUtility.ChargePositions = new Dictionary<int, Dictionary<Point?, bool>>();
            MoveUtility.LiftPosition = new Dictionary<Point?, int>();
            LoadMap();
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {

            return base.OnInitializeAsync(cancellationToken);
        }

        private void LoadMap()
        {
            if (File.Exists(this.ReadFile))
            {
                try
                {
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    var jArray = (JArray)JsonUtility.JsonToObject2<object>(File.ReadAllText(this.ReadFile));
                    int agvindex = 1;
                    foreach (var item in jArray)
                    {
                        var map = MapModel.ConvertJToken(item);
                        agvPlatformModels.Add(map);
                        //所有主通道的坐标
                        Dictionary<Point, int> mainRoadPositions = new Dictionary<Point, int>();
                        //所有休息位的坐标,是否可用
                        Dictionary<Point?, bool> restPositions = new Dictionary<Point?, bool>();
                        //所有充电位的坐标,是否可用
                        Dictionary<Point?, bool> chargePositions = new Dictionary<Point?, bool>();
                        var ellipses = map.canvasItemsSource.Where(t => t.GetType().Name == "CustomEllipse");
                        foreach (var point in ellipses)
                        {
                            CustomEllipse ellipse = point as CustomEllipse;
                            if (ellipse.Type == "MainRoad")
                            {
                                mainRoadPositions.Add(new Point(ellipse.ColumnIndex, ellipse.RowIndex), ellipse.RowIndex);
                            }
                            else if (ellipse.Type == "Parking")
                            {
                                restPositions.Add(new Point(ellipse.ColumnIndex, ellipse.RowIndex), true);
                            }
                            else if (ellipse.Type == "Charge")
                            {
                                chargePositions.Add(new Point(ellipse.ColumnIndex, ellipse.RowIndex), true);
                            }
                            else if (ellipse.Type == "Lift")
                            {
                                if (MoveUtility.LiftPosition.Count == 0)
                                {
                                    MoveUtility.LiftPosition.Add(new Point(ellipse.ColumnIndex, ellipse.RowIndex), 1);
                                    LiftMsg = $"当前提升机在1层";
                                }
                            }
                        }
                        //MainRoadPositions.Add(map.Floor, mainRoadPositions);
                        MoveUtility.MainRoads.Add(map.Floor, mainRoadPositions.Values.Distinct().ToList());
                        Dictionary<int, int> roadStatus = new Dictionary<int, int>();
                        foreach (var road in mainRoadPositions.Values.Distinct())
                        {
                            roadStatus.Add(road, 0);
                        }
                        MoveUtility.RoadStatuses.Add(map.Floor, roadStatus);
                        MoveUtility.RestPositions.Add(map.Floor, restPositions);
                        MoveUtility.ChargePositions.Add(map.Floor, chargePositions);

                        DrawAGV(0, agvindex, map.Floor, agvindex.ToString("AGV000"), map);
                        agvindex++;
                        DrawAGV(0, agvindex, map.Floor, agvindex.ToString("AGV000"), map);
                        agvindex++;
                    }
                    InitMoveUtility();
                    stopwatch.Stop();
                    System.Diagnostics.Debug.WriteLine($"加载耗时:{stopwatch.ElapsedMilliseconds}");

                }
                catch (Exception ex)
                {

                }

            }

        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            return base.OnActivateAsync(cancellationToken);
        }

        private void DrawAGV(int rowindex, int columindex, int floor, string code, MapModel pvm)
        {
            var result = pvm.canvasItemsSource.Where(w => w is CustomEllipse &&
                ((CustomEllipse)w).RowIndex == rowindex && ((CustomEllipse)w).ColumnIndex == columindex).FirstOrDefault();
            if (result == null)
                return;
            var ellipse = result as CustomEllipse;


            double left = ellipse.Left - (ellipse.Width);
            double top = ellipse.Top - (ellipse.Height);
            ImageBrush img = new ImageBrush();
            img.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.AGV.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            img.Stretch = Stretch.Uniform;

            var AGV = new AGVUCView();
            var viewmodel = new AGVUCViewModel()
            {
                Width = ellipse.Width * 3,
                Height = ellipse.Height * 3,
                ColumnIndex = columindex, RowIndex = rowindex, Floor = floor, Code = code,
                Left = left, Top = top, Status = "10", MainRoad = -1,
                RestWaitingTime = DateTime.Now, PowerI = 1000, Power = "100"
            };
            viewmodel.Timer = new Timer(viewmodel.CheckMoveToRest, null, 0, 1000);
            AGV.DataContext = viewmodel;
            viewmodel.LocationEvent += Item_LocationEvent;
            viewmodel.CurrentAttitude = 0;
            agvViewModelItemsSource.Add(viewmodel);

            ContextMenu contextMenu = new ContextMenu();
            MenuItem m_left = new MenuItem();
            m_left.Header = "向左"; m_left.Click += M_Click; m_left.Tag = AGVCommand.left;
            MenuItem m_right = new MenuItem();
            m_right.Header = "向右"; m_right.Click += M_Click; m_right.Tag = AGVCommand.right;
            MenuItem m_up = new MenuItem();
            m_up.Header = "向上"; m_up.Click += M_Click; m_up.Tag = AGVCommand.up;
            MenuItem m_down = new MenuItem();
            m_down.Header = "向下"; m_down.Click += M_Click; m_down.Tag = AGVCommand.down;
            MenuItem m_rotate = new MenuItem();
            m_rotate.Header = "旋转"; m_rotate.Click += M_rotate_Click;
            MenuItem m_move = new MenuItem();
            m_move.Header = "移动"; m_move.Click += M_Click; m_move.Tag = AGVCommand.move;
            MenuItem m_charge = new MenuItem();
            m_charge.Header = "充电"; m_charge.Click += M_Click; m_charge.Tag = AGVCommand.charge;
            MenuItem m_change_floor = new MenuItem();
            m_change_floor.Header = "换层"; m_change_floor.Click += M_Click; m_change_floor.Tag = AGVCommand.change_floor;
            contextMenu.Items.Add(m_left);
            contextMenu.Items.Add(m_right);
            contextMenu.Items.Add(m_up);
            contextMenu.Items.Add(m_down);
            contextMenu.Items.Add(m_rotate);
            contextMenu.Items.Add(m_move);
            contextMenu.Items.Add(m_charge);
            contextMenu.Items.Add(m_change_floor);
            AGV.ContextMenu = contextMenu;
            AGV.SetCurrentValue(Canvas.LeftProperty, left);
            AGV.SetCurrentValue(Canvas.TopProperty, top);

            allAGV.Add(AGV);
            pvm.canvasItemsSource.Add(AGV);
        }

        private void M_Click(object sender, RoutedEventArgs e)
        {

            MenuItem mi = sender as MenuItem;
            var vm = mi.DataContext as AGVUCViewModel;
            AGVAction(new AGVCommandModel() { code = vm.Code, command = (AGVCommand)mi.Tag });

        }


        private AGVUCView GetAGVUCView(string code)
        {
            return System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                AGVUCView view = null;
                foreach (var item in allAGV)
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
        private T ThreadSafe<T>(Func<T> func)
        {
            return System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                return func();
            });
        }

        internal void AGVAction(AGVCommandModel model)
        {
            AGVUCView view = GetAGVUCView(model.code);
            if (view != null)
            {

                var vm = ThreadSafe<AGVUCViewModel>(() =>
                      {
                          return view.DataContext as AGVUCViewModel;
                      });
                double left = vm.Left;
                double top = vm.Top;
                var pvm = agvPlatformModels.Where(w => w.Floor == vm.Floor).FirstOrDefault();
                var ellipse = pvm.GetEllipse(vm.RowIndex, vm.ColumnIndex);
                if (ellipse == null)
                    return;
                var c_Point = ellipse.GetCenterPoint();
                var ellipseLines = pvm.GetEliipseLines(vm.RowIndex, vm.ColumnIndex);
                if (vm.Status != "10")
                {
                    MessageBox.Show("小车状态不正确");
                    return;
                }
                switch (model.command)
                {
                    case AGVCommand.left:
                        if (vm.PowerI <= AGVUCViewModel.ChargeThreshold)
                        {
                            MessageBox.Show("小车电量不足,请先充电");
                            return;
                        }

                        if (ellipseLines.LeftLine == null)
                            return;
                        var leftellipse = pvm.GetEllipse(vm.RowIndex, ellipseLines.LeftLine.StartColumnIndex);
                        if (leftellipse == null)
                            return;
                        vm.Status = "30";
                        vm.RestWaitingTime = null;
                        var left_c_Point = leftellipse.GetCenterPoint();
                        var left_Arrow = DrawUtility.DrawSingleArrow(c_Point, left_c_Point, Math.PI / 12, 13);
                        vm.Move(view, new Point(left, top),
                            new Point(left + (left_c_Point.X - c_Point.X),
                                top + (left_c_Point.Y - c_Point.Y)),
                            left_Arrow, pvm, vm.Code, AGVCommand.left, ellipseLines.LeftLine, false);
                        vm.ColumnIndex = ellipseLines.LeftLine.StartColumnIndex;
                        vm.Left = left + (left_c_Point.X - c_Point.X);
                        vm.Top = top + (left_c_Point.Y - c_Point.Y);
                        break;
                    case AGVCommand.right:
                        if (vm.PowerI <= AGVUCViewModel.ChargeThreshold)
                        {
                            MessageBox.Show("小车电量不足,请先充电");
                            return;
                        }
                        if (ellipseLines.RightLine == null)
                            return;
                        var rightellipse = pvm.GetEllipse(vm.RowIndex, ellipseLines.RightLine.EndColumnIndex);
                        if (rightellipse == null)
                            return;
                        vm.Status = "30";
                        vm.RestWaitingTime = null;
                        var right_c_Point = rightellipse.GetCenterPoint();
                        var right_Arrow = DrawUtility.DrawSingleArrow(c_Point, right_c_Point, Math.PI / 12, 13);

                        vm.Move(view, new Point(left, top),
                            new Point(left + (right_c_Point.X - c_Point.X),
                                top + (right_c_Point.Y - c_Point.Y)),

                            right_Arrow, pvm, vm.Code, AGVCommand.right, ellipseLines.RightLine, false);
                        vm.ColumnIndex = ellipseLines.RightLine.EndColumnIndex;

                        vm.Left = left + (right_c_Point.X - c_Point.X);
                        vm.Top = top + (right_c_Point.Y - c_Point.Y);
                        break;
                    case AGVCommand.up:
                        if (vm.PowerI <= AGVUCViewModel.ChargeThreshold)
                        {
                            MessageBox.Show("小车电量不足,请先充电");
                            return;
                        }
                        if (ellipseLines.UpLine == null)
                            return;
                        var uptellipse = pvm.GetEllipse(ellipseLines.UpLine.StartRowIndex, vm.ColumnIndex);
                        if (uptellipse == null)
                            return;
                        vm.Status = "30";
                        vm.RestWaitingTime = null;
                        var up_c_Point = uptellipse.GetCenterPoint();
                        var up_Arrow = DrawUtility.DrawSingleArrow(c_Point, up_c_Point, Math.PI / 12, 13);

                        vm.Move(view, new Point(left, top),
                            new Point(left + (up_c_Point.X - c_Point.X),
                                top + (up_c_Point.Y - c_Point.Y)),
                            up_Arrow, pvm, vm.Code, AGVCommand.up, ellipseLines.UpLine, false);
                        vm.RowIndex = ellipseLines.UpLine.StartRowIndex;
                        vm.Left = left + (up_c_Point.X - c_Point.X);
                        vm.Top = top + (up_c_Point.Y - c_Point.Y);
                        break;
                    case AGVCommand.down:
                        if (vm.PowerI <= AGVUCViewModel.ChargeThreshold)
                        {
                            MessageBox.Show("小车电量不足,请先充电");
                            return;
                        }
                        if (ellipseLines.DownLine == null)
                            return;
                        var downtellipse = pvm.GetEllipse(ellipseLines.DownLine.EndRowIndex, vm.ColumnIndex);
                        if (downtellipse == null)
                            return;
                        vm.Status = "30";
                        vm.RestWaitingTime = null;
                        var down_c_Point = downtellipse.GetCenterPoint();
                        var down_Arrow = DrawUtility.DrawSingleArrow(c_Point, down_c_Point, Math.PI / 12, 13);

                        vm.Move(view, new Point(left, top),
                            new Point(left + (down_c_Point.X - c_Point.X),
                                top + (down_c_Point.Y - c_Point.Y)),
                            down_Arrow, pvm, vm.Code, AGVCommand.down, ellipseLines.DownLine, false);

                        vm.RowIndex = ellipseLines.DownLine.EndRowIndex;
                        vm.Left = left + (down_c_Point.X - c_Point.X);
                        vm.Top = top + (down_c_Point.Y - c_Point.Y);
                        break;
                    case AGVCommand.rotate:
                        double angle = (double)view.RenderTransform.GetValue(RotateTransform.AngleProperty);

                        view.RenderTransformOrigin = new Point(0.5d, 0.5d);
                        view.RenderTransform = new RotateTransform(angle + 90);
                        var vm1 = view.DataContext as AGVUCViewModel;
                        vm1.CurrentAttitude++;
                        DoubleAnimation dbAscending = new DoubleAnimation(angle, vm1.CurrentAttitude * 90, new Duration(TimeSpan.FromSeconds(2)));
                        Storyboard storyboard = new Storyboard();
                        Storyboard.SetTarget(dbAscending, view);
                        Storyboard.SetTargetProperty(dbAscending, new PropertyPath("RenderTransform.Angle"));
                        storyboard.Children.Add(dbAscending);
                        storyboard.Begin();
                        if (vm1.CurrentAttitude > 3)
                        {
                            vm1.CurrentAttitude = 0;
                        }

                        break;
                    case AGVCommand.move:
                        if (model.row != null && model.column != null && model.floor != null)
                        {
                            x = model.column.Value;
                            y = model.row.Value;
                        }
                        if (vm.PowerI <= AGVUCViewModel.ChargeThreshold)
                        {
                            MessageBox.Show("小车电量不足,请先充电");
                            return;
                        }
                        var chargePoints = MoveUtility.ChargePositions[vm.Floor].Keys.Where(t => t.Value.X == x && t.Value.Y == y);
                        if (chargePoints.Count() > 0)
                        {
                            MessageBox.Show("当前坐标点为充电桩，无法直接移动");
                            return;
                        }
                        var restPoints = MoveUtility.RestPositions[vm.Floor].Keys.Where(t => t.Value.X == x && t.Value.Y == y);
                        if (restPoints.Count() > 0)
                        {
                            MessageBox.Show("当前坐标点为休息位，无法直接移动");
                            return;
                        }
                        var msg = MoveUtility.Move(vm, new Point(x, y));
                        if (!string.IsNullOrWhiteSpace(msg))
                        {
                            MessageBox.Show(msg);
                            return;
                        }
                        break;
                    case AGVCommand.charge:
                        var chargePoint = MoveUtility.ChargePositions[vm.Floor].Where(t => t.Value).FirstOrDefault();
                        if (chargePoint.Key == null)
                        {
                            MessageBox.Show("当前层没有可用充电桩");
                            return;
                        }
                        MoveUtility.ChargePositions[vm.Floor][chargePoint.Key] = false;
                        var chargeMsg = MoveUtility.Move(vm, new Point(Convert.ToInt32(chargePoint.Key.Value.X), Convert.ToInt32(chargePoint.Key.Value.Y)));
                        if (!string.IsNullOrWhiteSpace(chargeMsg))
                        {
                            MoveUtility.ChargePositions[vm.Floor][chargePoint.Key] = true;
                            MessageBox.Show(chargeMsg);
                            return;
                        }
                        else
                        {
                            vm.Charging = true;
                        }
                        break;
                    case AGVCommand.change_floor:
                        if (vm.PowerI <= AGVUCViewModel.ChargeThreshold)
                        {
                            MessageBox.Show("小车电量不足,请先充电");
                            return;
                        }
                        var liftPosition = MoveUtility.LiftPosition.FirstOrDefault();
                        if (liftPosition.Value != vm.Floor)
                        {
                            MessageBox.Show($"当前提升机不在{vm.Floor}层");
                            return;
                        }
                        //判断除自己以外是否有其他车在提升机内
                        foreach (var item in allAGV.Where(t => ((AGVUCViewModel)t.DataContext).Floor == vm.Floor && ((AGVUCViewModel)t.DataContext).Code != vm.Code))
                        {
                            var agv = item.DataContext as AGVUCViewModel;
                            if (agv.Status == "20" && agv.Destination != null && agv.Destination.Value.X == liftPosition.Key.Value.X && agv.Destination.Value.Y == liftPosition.Key.Value.Y)
                            {
                                MessageBox.Show($"{agv.Code}终点为提升机，路径计算中");
                                return;
                            }
                            if (agv.RowIndex == liftPosition.Key.Value.Y && agv.ColumnIndex == liftPosition.Key.Value.X)
                            {
                                MessageBox.Show($"{agv.Code}占用了提升机");
                                return;
                            }
                        }
                        var liftMsg = MoveUtility.Move(vm, new Point(Convert.ToInt32(liftPosition.Key.Value.X), Convert.ToInt32(liftPosition.Key.Value.Y)), true);
                        if (!string.IsNullOrWhiteSpace(liftMsg))
                        {
                            MessageBox.Show(liftMsg);
                            return;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void Move(UIElement uIElement, Point start, Point end, Polygon myPolygon, MapModel pvm, string agvCode)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                myPolygon.Tag = agvCode;//AGV编码
                pvm.canvasPathItemsSource.Add(myPolygon);
                CustomStoryboard storyboard = new CustomStoryboard(myPolygon);   //创建Storyboard对象
                DoubleAnimation XAnimation = new DoubleAnimation(
                  start.X,
                  end.X,
                  new Duration(TimeSpan.FromSeconds(2))
                );
                Storyboard.SetTarget(XAnimation, uIElement);
                Storyboard.SetTargetProperty(XAnimation, new PropertyPath("(Canvas.Left)"));

                DoubleAnimation YAnimation = new DoubleAnimation(
           start.Y,
           end.Y,
           new Duration(TimeSpan.FromSeconds(2))
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
                  };
                storyboard.Begin();
            });
        }

        private void M_rotate_Click(object sender, RoutedEventArgs e)
        {
            if (selectAGVItem == null)
                return;
            MenuItem mi = sender as MenuItem;
            AGVUCView view = selectAGVItem as AGVUCView;
            if (view != null)
            {

                double angle = (double)view.RenderTransform.GetValue(RotateTransform.AngleProperty);

                view.RenderTransformOrigin = new Point(0.5d, 0.5d);
                view.RenderTransform = new RotateTransform(angle + 90);
                var vm = view.DataContext as AGVUCViewModel;
                vm.CurrentAttitude++;
                DoubleAnimation dbAscending = new DoubleAnimation(angle, vm.CurrentAttitude * 90, new Duration(TimeSpan.FromSeconds(2)));
                Storyboard storyboard = new Storyboard();
                Storyboard.SetTarget(dbAscending, view);
                Storyboard.SetTargetProperty(dbAscending, new PropertyPath("RenderTransform.Angle"));
                storyboard.Children.Add(dbAscending);
                storyboard.Begin();
                if (vm.CurrentAttitude > 3)
                {
                    vm.CurrentAttitude = 0;
                }

            }
        }


        private void Item_LocationEvent(object sender, AGVUCViewModel viewModel)
        {
            tabSelectedIndex = viewModel.Floor - 1;
            var map = agvPlatformModels[tabSelectedIndex];

            var view = GetAGVView(viewModel);
            var left = (double)view.GetValue(Canvas.LeftProperty);
            var top = (double)view.GetValue(Canvas.TopProperty);

            if (map.canvasWidth * map.ScaleValue > currentCanvasWidth)
            {
                map.DriftX = ((currentCanvasWidth / 2) - left) * map.ScaleValue;
            }
            else
            {
                map.DriftX = (currentCanvasWidth - (map.canvasWidth * map.ScaleValue)) / 2;
            }

            if (map.canvasHeight * map.ScaleValue > currentCanvasHeight)
            {

                map.DriftY = ((currentCanvasHeight / 2) - top) * map.ScaleValue;
            }
            else
            {
                map.DriftY = (currentCanvasHeight - (map.canvasHeight * map.ScaleValue)) / 2;
            }


            System.Diagnostics.Debug.WriteLine($"移动:DriftX { map.DriftX } DriftY { map.DriftY }");
        }


        private AGVUCView GetAGVView(AGVUCViewModel model)
        {
            foreach (var item in allAGV)
            {
                var vm = item.DataContext as AGVUCViewModel;
                if (vm.Code == model.Code)
                    return item;
            }
            return null;
        }

        public void headerItemClick(object sender, AGVUCViewModel viewmodel)
        {
            viewmodel.IsOpen = true;
        }

        public void outsidewrapper_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tabSelectedItem != null && e.LeftButton == MouseButtonState.Pressed)
            {
                Border uielement = sender as Border;
                tabSelectedItem.previousPoint = e.GetPosition(uielement);
                tabSelectedItem.isTranslateStart = true;
                e.Handled = false;
            }
        }
        public void outsidewrapper_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (tabSelectedItem != null && tabSelectedItem.isTranslateStart)
                {
                    Border uielement = sender as Border;
                    Point currentPoint = e.GetPosition(uielement);
                    Vector v = currentPoint - tabSelectedItem.previousPoint;
                    tabSelectedItem.previousPoint = currentPoint;
                    tabSelectedItem.DriftX = tabSelectedItem.DriftX + (v.X);
                    tabSelectedItem.DriftY = tabSelectedItem.DriftY + (v.Y);

                }
                e.Handled = false;
            }
        }
        public void outside_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (tabSelectedItem != null && e.LeftButton == MouseButtonState.Released && tabSelectedItem.isTranslateStart)
            {
                tabSelectedItem.isTranslateStart = false;
            }
        }

        public void outside_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (tabSelectedItem != null )
            {
                if (e.Delta == 0) return;

                double d = e.Delta / Math.Abs(e.Delta);
                if (tabSelectedItem.ScaleValue < 0.2 && d < 0) return;

                if (tabSelectedItem.ScaleValue > 5 && d > 0) return;

                tabSelectedItem.ScaleValue += d * .2;
            }
        }
        public void Reset(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var item in allAGV)
                {
                    var agv = item.DataContext as AGVUCViewModel;
                    lock (agv.Timer)
                    {
                        agv.Timer.Dispose();
                    }
                    agv.Charging = false;
                    agv.Resting = false;
                    agv.Destination = null;
                    agv.WaitPoints.Clear();
                    agv.CurPath.Clear();
                    agv.Path.Clear();
                    agv.MoveList.Clear();
                }
                allAGV.Clear();
                //MainRoadPositions.Clear();
                MoveUtility.MainRoads.Clear();
                MoveUtility.RoadStatuses.Clear();
                MoveUtility.RestPositions.Clear();
                MoveUtility.ChargePositions.Clear();
                MoveUtility.LiftPosition.Clear();
                agvViewModelItemsSource.Clear();
                agvPlatformModels.Clear();
                this.LoadMap();
            });
        }

        private void InitMoveUtility()
        {
            MoveUtility.AgvPlatformModels = agvPlatformModels;
            MoveUtility.AgvViewModelItemsSource = agvViewModelItemsSource;
            //MoveUtility.MainRoads = MainRoads;
            //MoveUtility.RoadStatuses = RoadStatuses;
            //MoveUtility.RestPositions = RestPositions;
            //MoveUtility.ChargePositions = ChargePositions;
            //MoveUtility.LiftPosition = LiftPosition;
            MoveUtility.AllAGV = allAGV;
        }

        public void LiftMove()
        {
            if (!MoveUtility.MainRoads.Keys.Contains(targetFloor))
            {
                MessageBox.Show($"楼层{targetFloor}不存在");
                return;
            }
            //判断提升机里有没有车
            var liftPosition = MoveUtility.LiftPosition.FirstOrDefault();
            if (targetFloor == liftPosition.Value)
            {
                MessageBox.Show($"无法移动到同一楼层");
                return;
            }
            AGVUCView view = null;
            foreach (var item in allAGV.Where(t => ((AGVUCViewModel)t.DataContext).Floor == liftPosition.Value))
            {
                var agv = item.DataContext as AGVUCViewModel;
                if (agv.Status == "20" && agv.Destination != null
                    && agv.Destination.Value.X == liftPosition.Key.Value.X
                    && agv.Destination.Value.Y == liftPosition.Key.Value.Y)
                {
                    MessageBox.Show($"{agv.Code}终点为提升机且计算路径中，不允许换层");
                    return;
                }
                else if (agv.Status == "30")
                {
                    //如果小车经过提升机且还没有开过提升机，不允许换层
                    //如果小车终点在提升机，且小车没到提升机，不允许换层
                    var curPoint = agv.Path.Where(t => t.Value.X == agv.ColumnIndex && t.Value.Y == agv.RowIndex).FirstOrDefault();
                    var curIndex = agv.Path.IndexOf(curPoint);
                    var liftPoint = agv.Path.Where(t => t.Value.X == liftPosition.Key.Value.X && t.Value.Y == liftPosition.Key.Value.Y).FirstOrDefault();
                    var liftIndex = agv.Path.IndexOf(liftPoint);
                    if (curIndex <= liftIndex)
                    {
                        MessageBox.Show($"{agv.Code}移动经过提升机且未到达，不允许换层");
                        return;
                    }
                    //else
                    //{
                    //    //如果小车终点在提升机，且小车没到提升机，不允许换层
                    //    var destination = agv.Path.Last();
                    //    if (destination != null && destination.Value.X == liftPosition.Key.Value.X
                    //        && destination.Value.Y == liftPosition.Key.Value.Y
                    //        && (agv.RowIndex != liftPosition.Key.Value.Y
                    //        || agv.ColumnIndex == liftPosition.Key.Value.X))
                    //    {
                    //        MessageBox.Show($"{agv.Code}尚未到达提升机不允许换层");
                    //        return;
                    //    }
                    //}
                }
                //小车已经在提升机,可以换层
                else if (agv.RowIndex == liftPosition.Key.Value.Y && agv.ColumnIndex == liftPosition.Key.Value.X)
                {
                    view = item;
                    break;
                }
            }
            if (view == null)
            {
                Point? point = new Point(liftPosition.Key.Value.X, liftPosition.Key.Value.Y);
                MoveUtility.LiftPosition.Clear();
                MoveUtility.LiftPosition.Add(point, targetFloor);
                LiftMsg = $"当前提升机在{targetFloor}层";
            }
            else
            {
                Point? point = new Point(liftPosition.Key.Value.X, liftPosition.Key.Value.Y);
                var p = agvPlatformModels.Where(t => t.Floor == liftPosition.Value).FirstOrDefault().GetEllipse(Convert.ToInt32(point.Value.Y), Convert.ToInt32(point.Value.X));
                MessageUtility.MessageCallBackAll("agv_move", "ControlCommand", new Cb_AGVMove()
                {
                    Code = ((AGVUCViewModel)view.DataContext).Code,
                    Command = AGVCommand.change_floor,
                    Floor = targetFloor,
                    StartPoint = new Point(p.Left, p.Top),
                    Endoint = new Point(p.Left, p.Top),
                    Distance = 0,
                    Power = ((AGVUCViewModel)view.DataContext).Power,
                    Path = null
                });
                var map1 = agvPlatformModels.Where(t => t.Floor == liftPosition.Value).FirstOrDefault();
                map1.canvasItemsSource.Remove(view);
                ((AGVUCViewModel)view.DataContext).Floor = targetFloor;
                ((AGVUCViewModel)view.DataContext).RestWaitingTime = DateTime.Now;
                var map2 = agvPlatformModels.Where(t => t.Floor == targetFloor).FirstOrDefault();
                map2.canvasItemsSource.Add(view);
                MoveUtility.LiftPosition.Clear();
                MoveUtility.LiftPosition.Add(point, targetFloor);
                LiftMsg = $"当前提升机在{targetFloor}层";
            }
        }
    }
    class MoveRoute
    {
        public List<Point3D?> Points
        {
            get; set;
        }
        public int MainRoad
        {
            get; set;
        }
        public double Length
        {
            get; set;
        }
        public List<Point3D?> WaitPoints
        {
            get; set;
        }
        public MoveRoute(List<Point3D?> points, int mainRoad, double length, List<Point3D?> waitPoints)
        {
            Points = points;
            MainRoad = mainRoad;
            Length = length;
            WaitPoints = waitPoints;
        }
    }
}