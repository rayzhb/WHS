using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using DijkstraAlgorithm;
using WHS.DEVICE.ROBOTNEW.Controls;
using WHS.DEVICE.ROBOTNEW.Models;
using WHS.DEVICE.ROBOTNEW.ViewModels;
using WHS.DEVICE.ROBOTNEW.Views;

namespace WHS.DEVICE.ROBOTNEW.Commons
{
    public static class MoveUtility
    {
        public static ObservableCollection<MapModel> AgvPlatformModels
        {
            get; set;
        }

        public static ThreadSafeObservableCollection<AGVUCViewModel> AgvViewModelItemsSource
        {
            get; set;
        }

        private static Object obj
        {
            get; set;
        } = new object();

        /// <summary>
        /// 每一层各有哪几条主通道
        /// key:层数
        /// val:每层主通道在第几行
        /// </summary>
        public static Dictionary<int, List<int>> MainRoads;

        /// <summary>
        /// 每一层主通道有多少任务
        /// key:层数
        /// val:{key: 主通道在第几行, val:该主通道现存任务数量}
        /// </summary>
        public static Dictionary<int, Dictionary<int, int>> RoadStatuses;

        /// <summary>
        /// 每一层休息位的坐标
        /// key: 层数,
        /// val: {key:坐标点, val:该休息位是否可用}
        /// </summary>
        public static Dictionary<int, Dictionary<Point?, bool>> RestPositions;

        /// <summary>
        /// 每一层充电位的坐标
        /// key:层数
        /// val: {key:坐标点, val:该休息位是否可用}
        /// </summary>
        public static Dictionary<int, Dictionary<Point?, bool>> ChargePositions;

        /// <summary>
        /// 提升机所在位置
        /// key:坐标
        /// value:层
        /// </summary>
        public static Dictionary<Point?, int> LiftPosition;

        public static ConcurrentBag<AGVUCView> AllAGV;

        public static string Move(AGVUCViewModel agv, Point point, bool isChangeFloor = false)
        {
            agv.Status = "20";
            agv.Destination = point;
            int x = Convert.ToInt32(point.X);
            int y = Convert.ToInt32(point.Y);
            var pvm = AgvPlatformModels.Where(w => w.Floor == agv.Floor).FirstOrDefault();
            var msg = CalPath(agv, new Point(x, y), isChangeFloor);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                agv.Status = "10";
                return msg;
            }
            if (agv.Status == "30")
            {
                agv.RestWaitingTime = null;
                //停用定时器
                //agv.Timer.Change(Timeout.Infinite, 1000);
                //如果小车从休息位出发，则释放该休息位
                var restPoint = RestPositions[agv.Floor].Keys
                    .Where(t => t.Value.X == agv.ColumnIndex
                    && t.Value.Y == agv.RowIndex).FirstOrDefault();
                if (restPoint != null)
                {
                    RestPositions[agv.Floor][restPoint] = true;
                    agv.Resting = false;
                }
                //如果小车从充电位出库，则释放该充电位
                var chargePoint = ChargePositions[agv.Floor].Keys
                    .Where(t => t.Value.X == agv.ColumnIndex
                    && t.Value.Y == agv.RowIndex).FirstOrDefault();
                if (chargePoint != null)
                {
                    ChargePositions[agv.Floor][chargePoint] = true;
                    agv.Charging = false;
                }
                List<Point> cPoints = new List<Point>();
                foreach (var item in agv.Path)
                {
                    var ellipse = pvm.GetEllipse(Convert.ToInt32(item.Value.Y), Convert.ToInt32(item.Value.X));
                    cPoints.Add(ellipse.GetCenterPoint());
                }
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var polyLine = DrawUtility.DrawPathPolyLine(cPoints);
                    polyLine.Tag = agv.Code;//AGV编码
                    pvm.canvasPathItemsSource.Add(polyLine);
                });
                agv.Move(AgvPlatformModels, AllAGV);
            }
            return string.Empty;
        }

        /// <summary>
        /// 计算小车移动路径
        /// </summary>
        /// <param name="agv">agv对象</param>
        /// <param name="destination">终点</param>
        /// <param name="height">小车高度，背货时填/param>
        private static string CalPath(AGVUCViewModel agv, Point destination, bool isChangeFloor, int height = 0)
        {
            Point source = new Point(agv.ColumnIndex, agv.RowIndex);
            if (source.X == destination.X && source.Y == destination.Y)
            {
                agv.Status = "10";
                return "起点和终点不能相同";
            }
            //当前层其他小车是否占据了终点
            foreach (var item in AgvViewModelItemsSource.Where(t => t.Code != agv.Code && t.Floor == agv.Floor))
            {
                while (item.Status == "20")
                {
                }
                //判断有当前层的车停靠在终点
                if (item.Status == "10" && item.ColumnIndex == destination.X && item.RowIndex == destination.Y)
                {
                    agv.Status = "10";
                    return $"{item.Code}已经停靠在({destination.X},{destination.Y}),无法到达";
                }
                if (item.Status == "30" && item.Path.Last() != null && item.Path.Last().Value.X == destination.X && item.Path.Last().Value.Y == destination.Y)
                {
                    agv.Status = "10";
                    return $"{item.Code}终点为({destination.X},{destination.Y}),无法到达";
                }
            }
            int floor = agv.Floor;
            //获取当前层地图
            var map = AgvPlatformModels.Where(t => t.Floor == floor).FirstOrDefault();
            if (map != null)
            {
                List<Point3D?> points = new List<Point3D?>();
                WeightedDiGraph<Point3D?, double> graph = new WeightedDiGraph<Point3D?, double>();
                foreach (var item in map.canvasBackgroudItemsSource)
                {
                    if (item.GetType().Name == "CustomLine")
                    {
                        CustomLine line = item as CustomLine;
                        Point3D? startPoint = new Point3D(line.StartColumnIndex, line.StartRowIndex, 1000);
                        Point3D? endPoint = new Point3D(line.EndColumnIndex, line.EndRowIndex, 1000);
                        var position = LiftPosition.FirstOrDefault();

                        //非换层任务且起点不在提升机
                        if (!isChangeFloor && !(source.X == position.Key.Value.X && source.Y == position.Key.Value.Y))
                        {
                            if (startPoint.Value.X == position.Key.Value.X && startPoint.Value.Y == position.Key.Value.Y)
                            {
                                continue;
                            }
                            if (endPoint.Value.X == position.Key.Value.X && endPoint.Value.Y == position.Key.Value.Y)
                            {
                                continue;
                            }
                        }

                        //提升机不在当前层，不添加提升机的点和线
                        if (position.Value != floor)
                        {
                            if (startPoint.Value.X == position.Key.Value.X && startPoint.Value.Y == position.Key.Value.Y)
                            {
                                continue;
                            }
                            if (endPoint.Value.X == position.Key.Value.X && endPoint.Value.Y == position.Key.Value.Y)
                            {
                                continue;
                            }
                        }
                        if (!points.Contains(startPoint))
                        {
                            points.Add(startPoint);
                            graph.AddVertex(startPoint);
                        }
                        if (!points.Contains(endPoint))
                        {
                            points.Add(endPoint);
                            graph.AddVertex(endPoint);
                        }
                        //考虑后车先至的情况，如何避免后车先把货放好而前车并没有在计算时认为该点有货（每次都重算所有终点同层车的路径？）
                        //小车高度大于0（背货状态），任意数据库里有货的点则都不添加边，各个小车当前的放货终点（可能还没加到数据库中）也不添加边
                        if (height > 0)
                        {
                        }
                        //小车高度大于任意一点的Z坐标,则不添加边
                        if (height > startPoint.Value.Z || height > startPoint.Value.Z)
                        {
                            continue;
                        }
                        var sp = points.Where(t => t.Value.X == startPoint.Value.X && t.Value.Y == startPoint.Value.Y && t.Value.Z == startPoint.Value.Z).FirstOrDefault();
                        var ep = points.Where(t => t.Value.X == endPoint.Value.X && t.Value.Y == endPoint.Value.Y && t.Value.Z == endPoint.Value.Z).FirstOrDefault();
                        if (line.Arrow == "Default" || line.Arrow == "Double")
                        {
                            graph.AddEdge(sp, ep, line.Distance);
                            graph.AddEdge(ep, sp, line.Distance);
                        }
                        else if (line.Arrow == "left")
                        {
                            graph.AddEdge(ep, sp, line.Distance);
                        }
                        else if (line.Arrow == "right")
                        {
                            graph.AddEdge(sp, ep, line.Distance);
                        }
                    }
                }
                var algorithm = new DijikstraShortestPath<Point3D?, double>(new DijikstraShortestPathOperators());
                var start = points.Where(t => t.Value.X == source.X && t.Value.Y == source.Y).FirstOrDefault();
                var end = points.Where(t => t.Value.X == destination.X && t.Value.Y == destination.Y).FirstOrDefault();
                if (start == null)
                {
                    agv.Status = "10";
                    return $"起点坐标错误";
                }
                if (end == null)
                {
                    agv.Status = "10";
                    return $"终点坐标错误";
                }
                var pathResult = algorithm.FindShortestPath(graph, start, end);

                List<MoveRoute> moveRoutes = new List<MoveRoute>();
                foreach (var item in pathResult._dic)
                {
                    //求出每条路线的总长度和经过哪条主通道
                    double length = item.Value;
                    List<Point3D?> waitPositions = new List<Point3D?>();
                    for (int i = 0; i < item.Key.Count; i++)
                    {
                        if (i > 1)
                        {
                            if (item.Key[i].Value.X != item.Key[i - 2].Value.X && item.Key[i].Value.Y != item.Key[i - 2].Value.Y)
                            {
                                length += AGVUCViewModel.TurnTime / 1000;
                            }
                            //添加主通道前一点和主通道第一点
                            if (MainRoads[floor].Contains(Convert.ToInt32(item.Key[i].Value.Y))
                                && !MainRoads[floor].Contains(Convert.ToInt32(item.Key[i - 1].Value.Y)))
                            {
                                waitPositions.Add(points.Where(t => t.Value.X == item.Key[i - 1].Value.X && t.Value.Y == item.Key[i - 1].Value.Y).FirstOrDefault());
                                //waitPositions.Add(points.Where(t => t.Value.X == item.Key[i].Value.X && t.Value.Y == item.Key[i].Value.Y).FirstOrDefault());
                            }
                            //添加主通道最后一个点
                            else if (i + 1 < item.Key.Count
                                && MainRoads[floor].Contains(Convert.ToInt32(item.Key[i].Value.Y))
                                && !MainRoads[floor].Contains(Convert.ToInt32(item.Key[i + 1].Value.Y)))
                            {
                                waitPositions.Add(points.Where(t => t.Value.X == item.Key[i].Value.X && t.Value.Y == item.Key[i].Value.Y).FirstOrDefault());
                            }
                        }
                        //只考虑车动画不考虑实际车，每个点都判断下,相向会撞车
                        //waitPositions.Add(points.Where(t => t.Value.X == item.Key[i].Value.X && t.Value.Y == item.Key[i].Value.Y).FirstOrDefault());
                    }

                    //每条主通道各自经过几个点
                    Dictionary<int, int> roadDic = new Dictionary<int, int>();
                    foreach (var mainRoad in MainRoads[floor])
                    {
                        roadDic.Add(mainRoad, item.Key.Where(t => t.Value.Y == mainRoad).Count());
                    }
                    //所有路径完全不经过主通道
                    if (roadDic.Values.All(t => t == 0))
                    {
                        moveRoutes.Add(new MoveRoute(item.Key, -1, length, waitPositions));
                    }
                    else
                    {
                        int maxValue = roadDic.Values.Max();
                        moveRoutes.Add(new MoveRoute(item.Key, roadDic.Where(t => t.Value == maxValue).FirstOrDefault().Key, length, waitPositions));
                    }
                }
                if (moveRoutes.Count == 0)
                {
                    agv.Status = "10";
                    return $"无法从({start.Value.X},{start.Value.Y})到达({end.Value.X},{end.Value.Y})";
                }
                else
                {
                    lock (obj)
                    {
                        //选择路径经过的主通道，-1表示不经过主通道任意一点，只在单侧巷道内移动
                        int curMainRoad = -1;
                        //分配路线
                        var minLength = moveRoutes.Min(t => t.Length);
                        //List<Position> path = new List<Position>();
                        var noMainRoad = moveRoutes.Where(t => t.Length == minLength && t.MainRoad == -1).FirstOrDefault();
                        //经过主通道
                        bool isAllocated = false;
                        List<Point3D?> Path = new List<Point3D?>();
                        if (noMainRoad == null)
                        {
                            //循环当前层的所有主通道
                            foreach (var item in RoadStatuses[floor])
                            {
                                //主通道空闲且该路径经过该主通道
                                if (item.Value == 0 &&
                                    moveRoutes.Where(t => t.Length == minLength).Select(t => t.MainRoad).Contains(item.Key))
                                {
                                    curMainRoad = item.Key;
                                    RoadStatuses[floor][curMainRoad] = 1;
                                    var moveRoute = moveRoutes.Where(t => t.Length == minLength && t.MainRoad == item.Key).FirstOrDefault();
                                    Path = moveRoute.Points;
                                    agv.Path = moveRoute.Points;
                                    agv.WaitPoints = moveRoute.WaitPoints;
                                    isAllocated = true;
                                    break;
                                }
                            }
                            if (!isAllocated)
                            {
                                //如果路线只经过一条主通道
                                var allMainRoads = moveRoutes.Where(t => t.Length == minLength).Select(t => t.MainRoad).Distinct().ToList();
                                if (allMainRoads.Count == 1)
                                {
                                    curMainRoad = allMainRoads[0];
                                    var road = RoadStatuses[floor].Where(t => t.Key == curMainRoad).FirstOrDefault();
                                    RoadStatuses[floor][curMainRoad] += 1;
                                    var moveRoute = moveRoutes.Where(t => t.Length == minLength && t.MainRoad == allMainRoads[0]).FirstOrDefault();
                                    Path = moveRoute.Points;
                                    agv.Path = moveRoute.Points;
                                    agv.WaitPoints = moveRoute.WaitPoints;
                                }
                                else
                                {
                                    var minValue = RoadStatuses[floor].Min(t => t.Value);
                                    var road = RoadStatuses[floor].Where(t => t.Value == minValue).FirstOrDefault();
                                    curMainRoad = road.Key;
                                    RoadStatuses[floor][curMainRoad] += 1;
                                    var moveRoute = moveRoutes.Where(t => t.Length == minLength && t.MainRoad == curMainRoad).FirstOrDefault();
                                    Path = moveRoute.Points;
                                    agv.Path = moveRoute.Points;
                                    agv.WaitPoints = moveRoute.WaitPoints;
                                }
                            }
                        }
                        //不经过主通道
                        else
                        {
                            var moveRoute = moveRoutes.Where(t => t.Length == minLength).FirstOrDefault();
                            Path = moveRoute.Points;
                            agv.Path = moveRoute.Points;
                            agv.WaitPoints = moveRoute.WaitPoints;
                        }
                        agv.MainRoad = curMainRoad;
                        agv.MoveList = GetMoveList(agv.Path);
                        agv.Status = "30";
                        return string.Empty;
                    }
                }
            }
            else
            {
                agv.Status = "10";
                return $"当前地图不存在第{floor}层";
            }
        }

        private static ConcurrentQueue<AGVCommand> GetMoveList(List<Point3D?> movePositions)
        {
            ConcurrentQueue<AGVCommand> moves = new ConcurrentQueue<AGVCommand>();
            for (int i = 0; i < movePositions.Count - 1; i++)
            {
                if (movePositions[i].Value.X == movePositions[i + 1].Value.X)
                {
                    var p = movePositions[i + 1].Value.Y - movePositions[i].Value.Y;
                    if (p >= 1)
                    {
                        moves.Enqueue(AGVCommand.down);
                    }
                    else if (p <= -1)
                    {
                        moves.Enqueue(AGVCommand.up);
                    }
                }
                else if (movePositions[i].Value.Y == movePositions[i + 1].Value.Y)
                {
                    var p = movePositions[i + 1].Value.X - movePositions[i].Value.X;
                    if (p >= 1)
                    {
                        moves.Enqueue(AGVCommand.right);
                    }
                    else if (p <= -1)
                    {
                        moves.Enqueue(AGVCommand.left);
                    }
                }
            }
            return moves;
        }

        private class DijikstraShortestPathOperators : IShortestPathOperators<double>
        {
            public double DefaultValue
            {
                get
                {
                    return 0;
                }
            }

            public double MaxValue
            {
                get
                {
                    return double.MaxValue;
                }
            }

            public double Sum(double a, double b)
            {
                return checked(a + b);
            }
        }
    }
}