using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WHS.DEVICE.MAPDESIGN.Commons;
using WHS.DEVICE.MAPDESIGN.Controls;
using WHS.DEVICE.MAPDESIGN.Models;
using WHS.Infrastructure.Utils;

namespace WHS.DEVICE.MAPDESIGN.ViewModels
{
    public class MapDesignViewModel : Screen
    {
        public ObservableCollection<MapModel> mapModels
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

        private Type _CustomDisplayType;
        public Type CustomDisplayType
        {
            get
            {
                return _CustomDisplayType;
            }
            set
            {
                _CustomDisplayType = value;
                NotifyOfPropertyChange(() => CustomDisplayType);
            }
        }


        private MapModel _tabSelectedItem;
        public MapModel tabSelectedItem
        {
            get
            {
                return _tabSelectedItem;
            }
            set
            {
                _tabSelectedItem = value;
                NotifyOfPropertyChange(() => tabSelectedItem);
            }
        }


        private int _Row;
        public int Row
        {
            get
            {
                return _Row;
            }
            set
            {
                _Row = value;
                NotifyOfPropertyChange(() => Row);
            }
        }

        private int _Column;
        public int Column
        {
            get
            {
                return _Column;
            }
            set
            {
                _Column = value;
                NotifyOfPropertyChange(() => Column);
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

        private string _SaveFile;
        public string SaveFile
        {
            get
            {
                return _SaveFile;
            }
            set
            {
                _SaveFile = value;
                NotifyOfPropertyChange(() => SaveFile);
            }
        }


        /// <summary>
        /// 当前画布宽度
        /// </summary>
        public double CanvasWidth
        {
            get; set;
        }
        /// <summary>
        /// 当前画布高度
        /// </summary>
        public double CanvasHeight
        {
            get; set;
        }

        private bool _IsDragable;
        public bool IsDragable
        {
            get
            {
                return _IsDragable;
            }
            set
            {
                _IsDragable = value;
                NotifyOfPropertyChange(() => IsDragable);
            }
        }




        /// <summary>
        /// 当前画布的实际宽度
        /// </summary>
        public double currentCanvasWidth
        {
            get; set;
        }
        /// <summary>
        /// 当前画布的实际高度
        /// </summary>
        public double currentCanvasHeight
        {
            get; set;
        }

        public MapDesignViewModel()
        {
            mapModels = new ObservableCollection<MapModel>();
            CustomDisplayType = typeof(CustomDisplayAttribute);
            Row = 10;
            Column = 10;
            SaveFile = "D:\\RCSMap.map";//Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "RCSMap.map");
            CanvasHeight = 768;
            CanvasWidth = 1024;
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            if (File.Exists(this.SaveFile))
            {
                try
                {
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    var jArray = (JArray)JsonUtility.JsonToObject2<object>(File.ReadAllText(this.SaveFile));
                    foreach (var item in jArray)
                    {
                        mapModels.Add(MapModel.ConvertJToken(item));

                    }
                    foreach (var pvm in mapModels)
                    {
                        var watermark = DrawUtility.DrawWatermark(pvm.Floor, pvm.canvasWidth, pvm.canvasHeight);
                        pvm.canvasBackgroudItemsSource.Add(watermark);
                    }


                    CanvasHeight = mapModels[0].canvasHeight;
                    CanvasWidth = mapModels[0].canvasWidth;
                    stopwatch.Stop();
                    System.Diagnostics.Debug.WriteLine($"加载耗时:{stopwatch.ElapsedMilliseconds}");
                }
                catch (Exception ex)
                {

                }
            }
            return base.OnInitializeAsync(cancellationToken);
        }

        public void AddFloor(object sender, object args)
        {
            if (Row == 0 || Column == 0 || CanvasHeight == 0 || CanvasWidth == 0)
            {
                return;
            }
            if (mapModels.Count == 0)
            {
                MapModel floor = new MapModel(1);
                floor.gridRows = Row;
                floor.gridColumns = Column;
                floor.canvasHeight = CanvasHeight;
                floor.canvasWidth = CanvasWidth;
                DrawGrid(floor.gridRows, floor.gridColumns, floor);
                mapModels.Add(floor);
            }
            else
            {
                int floor = mapModels.Max(s => s.Floor);
                floor++;
                MapModel floor2 = new MapModel(floor);
                floor2.gridRows = Row;
                floor2.gridColumns = Column;
                floor2.canvasHeight = CanvasHeight;
                floor2.canvasWidth = CanvasWidth;
                DrawGrid(floor2.gridRows, floor2.gridColumns, floor2);
                mapModels.Add(floor2);
            }

        }

        private void DrawGrid(int rows, int colums, MapModel pvm)
        {
            var mWidth = pvm.canvasWidth / colums;
            var mHeight = pvm.canvasHeight / rows;
            pvm.cellHeight = pvm.cellWidth = Math.Min(mWidth, mHeight);//画出的格子默认正方形
            pvm.canvasHeight = pvm.cellHeight * rows;
            pvm.canvasWidth = pvm.cellWidth * colums;

            var watermark = DrawUtility.DrawWatermark(pvm.Floor, pvm.canvasWidth, pvm.canvasHeight);
            pvm.canvasBackgroudItemsSource.Add(watermark);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < colums; j++)
                {

                    #region 画背景格
                    //var brush = new SolidColorBrush(Colors.Blue);
                    //brush.SetValue(SolidColorBrush.OpacityProperty, 0.2);
                    //double back_left = j * pvm.cellWidth;
                    //double back_top = i * pvm.cellHeight;
                    //CustomRectangle customRectangle = new CustomRectangle()
                    //{
                    //    Id = "b_" + pvm.Floor.ToString() + "_" + i.ToString() + "_" + j.ToString(),
                    //    Left = back_left,
                    //    Top = back_top,
                    //    Stroke = brush,
                    //    StrokeThickness = 2,
                    //    Width = pvm.cellWidth,
                    //    Height = pvm.cellHeight,
                    //    RowIndex = i,
                    //    ColumnIndex = j,

                    //};
                    //customRectangle.SetValue(Canvas.LeftProperty, back_left);
                    //customRectangle.SetValue(Canvas.TopProperty, back_top);
                    //pvm.canvasBackgroudItemsSource.Add(customRectangle);
                    #endregion


                    #region 画圆
                    double left = j * pvm.cellWidth + (pvm.cellWidth / 2 - pvm.cellWidth / 8);
                    double top = i * pvm.cellHeight + (pvm.cellHeight / 2 - pvm.cellHeight / 8);
                    CustomEllipse customEllipse = new CustomEllipse()
                    {
                        Id = "p_" + pvm.Floor.ToString() + "_" + i.ToString() + "_" + j.ToString(),
                        NewWidth = pvm.cellWidth / 4,
                        NewHeight = pvm.cellHeight / 4,
                        RowIndex = i,
                        ColumnIndex = j,
                        Left = left,
                        Top = top
                    };
                    pvm.canvasItemsSource.Add(customEllipse);
                    #endregion

                    #region 画横线
                    int endj = j + 1;
                    if (endj < colums)
                    {

                        double lineleft = j * pvm.cellWidth + (pvm.cellWidth / 2 + pvm.cellWidth / 8);
                        double linetop = i * pvm.cellHeight + (pvm.cellHeight / 2 - pvm.cellHeight / 8);
                        double linewidth = pvm.cellWidth - pvm.cellWidth / 4;
                        CustomLine customLine = new CustomLine(new Point(0, pvm.cellHeight / 8), new Point(linewidth, pvm.cellHeight / 8))
                        {
                            Id = "h_" + pvm.Floor.ToString() + "_" + i.ToString() + "_" + j.ToString(),
                            StartRowIndex = i, StartColumnIndex = j, EndRowIndex = i, EndColumnIndex = endj,
                            NewWidth = linewidth,
                            NewHeight = pvm.cellHeight / 4,
                            Left = lineleft,
                            Top = linetop
                        };
                        pvm.canvasItemsSource.Add(customLine);//画横线
                    }

                    #endregion

                    #region 画竖线
                    int endi = i + 1;
                    if (endi < rows)
                    {
                        double lineleft2 = j * pvm.cellWidth + (pvm.cellWidth / 2 - pvm.cellWidth / 8);
                        double linetop2 = i * pvm.cellHeight + (pvm.cellHeight / 2 + pvm.cellHeight / 8);
                        double lineheight = pvm.cellHeight - pvm.cellHeight / 4;
                        CustomLine customLine2 = new CustomLine(new Point(pvm.cellWidth / 8, 0), new Point(pvm.cellWidth / 8, lineheight))
                        {
                            Id = "v_" + pvm.Floor.ToString() + "_" + i.ToString() + "_" + j.ToString(),

                            StartRowIndex = i, StartColumnIndex = j, EndRowIndex = endi, EndColumnIndex = j,
                            NewStrokeThickness = 1,
                            NewWidth = pvm.cellWidth / 4,
                            NewHeight = lineheight,
                            NewStroke = Brushes.Black,
                            Left = lineleft2,
                            Top = linetop2
                        };
                        pvm.canvasItemsSource.Add(customLine2);
                    }
                    #endregion
                }
            }
        }

        public void outsidewrapper_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tabSelectedItem != null && e.RightButton == MouseButtonState.Pressed)
            {
                Border uielement = sender as Border;
                tabSelectedItem.previousPoint = e.GetPosition(uielement);
                tabSelectedItem.isTranslateStart = true;
                e.Handled = false;
            }
        }
        public void outsidewrapper_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            if (e.RightButton == MouseButtonState.Pressed)
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
            //if (tabSelectedItem != null && e.RightButton == MouseButtonState.Released && tabSelectedItem.isTranslateStart)
            //{
            //    tabSelectedItem.isTranslateStart = false;
            //}
        }
        public void outside_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            currentCanvasHeight = args.NewSize.Height;
            currentCanvasWidth = args.NewSize.Width;
            //foreach (MapModel floor in mapModels)
            //{

            //    var sHeight = currentCanvasHeight / floor.canvasHeight;
            //    var sWidth = currentCanvasWidth / floor.canvasWidth;
            //    floor.ScaleValue = Math.Min(sHeight, sWidth);
            //    //int direction = floor.ScaleValue > 1 ? 1 : -1;
            //    //floor.DriftX = 0d;
            //    //floor.DriftY = 0d;
            //    //floor.DriftX = (floor.cellWidth) * sWidth * direction;
            //    //floor.DriftY = (floor.cellHeight) * sHeight * direction;
            //}


        }

        public void outside_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (tabSelectedItem != null)
            {
                if (e.Delta == 0) return;

                double d = e.Delta / Math.Abs(e.Delta);
                if (tabSelectedItem.ScaleValue < 0.2 && d < 0) return;

                if (tabSelectedItem.ScaleValue > 5 && d > 0) return;

                tabSelectedItem.ScaleValue += d * .2;
            }
        }

        public void PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs args)
        {
            args.Handled = true;
            if (args.Key == Key.F4)
            {
                IsOpen = true;
            }
            else if (args.Key == Key.Delete)
            {
                var list = sender as ListBox;
                List<FrameworkElement> removelist = new List<FrameworkElement>();
                foreach (var selectedItem in list.SelectedItems)
                {
                    removelist.Add(selectedItem as FrameworkElement);
                }

                foreach (var selectedItem in removelist)
                {
                    tabSelectedItem.canvasItemsSource.Remove(selectedItem);
                    tabSelectedItem.undoList.Push(new UndoModel("remove", selectedItem));
                    if (selectedItem is CustomEllipse)
                    {
                        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                        stopwatch.Start();
                        var ellipse = selectedItem as CustomEllipse;
                        var row = ellipse.RowIndex;
                        var column = ellipse.ColumnIndex;
                        var rightline = tabSelectedItem.canvasItemsSource.Where(w => w is CustomLine
                             && ((CustomLine)w).StartRowIndex == row
                              && ((CustomLine)w).StartColumnIndex == column
                               && ((CustomLine)w).EndRowIndex == row
                              && ((CustomLine)w).EndColumnIndex > column).FirstOrDefault();

                        var leftline = tabSelectedItem.canvasItemsSource.Where(w => w is CustomLine
                            && ((CustomLine)w).StartRowIndex == row
                             && ((CustomLine)w).StartColumnIndex < column
                              && ((CustomLine)w).EndRowIndex == row
                             && ((CustomLine)w).EndColumnIndex == column).FirstOrDefault();

                        var topline = tabSelectedItem.canvasItemsSource.Where(w => w is CustomLine
                             && ((CustomLine)w).StartRowIndex < row
                              && ((CustomLine)w).StartColumnIndex == column
                               && ((CustomLine)w).EndRowIndex == row
                              && ((CustomLine)w).EndColumnIndex == column).FirstOrDefault();

                        var downline = tabSelectedItem.canvasItemsSource.Where(w => w is CustomLine
                                 && ((CustomLine)w).StartRowIndex == row
                                  && ((CustomLine)w).StartColumnIndex == column
                                   && ((CustomLine)w).EndRowIndex > row
                                  && ((CustomLine)w).EndColumnIndex == column).FirstOrDefault();

                        if (rightline != null)
                        {
                            tabSelectedItem.canvasItemsSource.Remove(rightline);
                            tabSelectedItem.undoList.Push(new UndoModel("remove", rightline));
                        }
                        if (leftline != null)
                        {
                            tabSelectedItem.canvasItemsSource.Remove(leftline);
                            tabSelectedItem.undoList.Push(new UndoModel("remove", leftline));
                        }

                        if (topline != null)
                        {
                            tabSelectedItem.canvasItemsSource.Remove(topline);
                            tabSelectedItem.undoList.Push(new UndoModel("remove", topline));
                        }
                        if (downline != null)
                        {
                            tabSelectedItem.canvasItemsSource.Remove(downline);
                            tabSelectedItem.undoList.Push(new UndoModel("remove", downline));
                        }

                        stopwatch.Stop();
                        System.Diagnostics.Debug.WriteLine($"删除耗时:{stopwatch.ElapsedMilliseconds}");
                        stopwatch.Restart();
                        if (rightline != null && leftline != null)
                        {
                            CustomLine r = rightline as CustomLine;
                            CustomLine l = leftline as CustomLine;
                            var width = (r.Left - l.Left) + r.NewWidth;
                            CustomLine customLine = new CustomLine(new Point(0, l.NewHeight / 2), new Point(width, l.NewHeight / 2))
                            {
                                Id = "h_" + tabSelectedItem.Floor.ToString() + "_" + r.StartRowIndex.ToString() + "_" + r.StartColumnIndex.ToString(),
                                StartRowIndex = l.StartRowIndex, StartColumnIndex = l.StartColumnIndex,
                                EndRowIndex = r.EndRowIndex, EndColumnIndex = r.EndColumnIndex,
                                NewWidth = width,
                                NewHeight = l.NewHeight,
                                Left = l.Left,
                                Top = l.Top
                            };

                            tabSelectedItem.canvasItemsSource.Add(customLine);//画横线
                            tabSelectedItem.undoList.Push(new UndoModel("add", customLine));
                        }
                        if (topline != null && downline != null)
                        {
                            CustomLine t = topline as CustomLine;
                            CustomLine d = downline as CustomLine;
                            var height = (d.Top - t.Top) + d.NewHeight;
                            CustomLine customLine2 = new CustomLine(new Point(t.NewWidth / 2, 0), new Point(t.NewWidth / 2, height))
                            {
                                Id = "v_" + tabSelectedItem.Floor.ToString() + "_" + t.StartRowIndex.ToString() + "_" + t.StartColumnIndex.ToString(),
                                StartRowIndex = t.StartRowIndex, StartColumnIndex = t.StartColumnIndex,
                                EndRowIndex = d.EndRowIndex, EndColumnIndex = d.EndColumnIndex,
                                NewStrokeThickness = 1,
                                NewWidth = t.Width,
                                NewHeight = height,
                                NewStroke = Brushes.Black,
                                Left = t.Left,
                                Top = t.Top
                            };

                            tabSelectedItem.canvasItemsSource.Add(customLine2);//画竖线
                            tabSelectedItem.undoList.Push(new UndoModel("add", customLine2));
                        }

                        stopwatch.Stop();
                        System.Diagnostics.Debug.WriteLine($"插入耗时:{stopwatch.ElapsedMilliseconds}");
                    }
                }
            }

        }

        public void Redo(object sender, object e)
        {
            if (tabSelectedItem != null && tabSelectedItem.redoList.Count > 0)
            {
                var model = tabSelectedItem.redoList.Pop();
                tabSelectedItem.undoList.Push(model);
                if (model.Opreator == "remove")
                {
                    tabSelectedItem.canvasItemsSource.Remove(model.Obj);
                }
                else if (model.Opreator == "add")
                {
                    tabSelectedItem.canvasItemsSource.Add(model.Obj);
                }
            }
        }

        public void Undo(object sender, object e)
        {
            if (tabSelectedItem != null && tabSelectedItem.undoList.Count > 0)
            {
                var model = tabSelectedItem.undoList.Pop();
                tabSelectedItem.redoList.Push(model);
                if (model.Opreator == "remove")
                {
                    tabSelectedItem.canvasItemsSource.Add(model.Obj);
                }
                else if (model.Opreator == "add")
                {
                    tabSelectedItem.canvasItemsSource.Remove(model.Obj);
                }
            }
        }

        //public void Print(object sender, object args)
        //{
        //    var json = JsonUtility.ObjectToJson2(mapModels.ToList(), false, false, Newtonsoft.Json.Formatting.None);
        //    ((MahApps.Metro.Controls.MetroWindow)Application.Current.MainWindow).ShowModalMessageExternal("信息", json);

        //}

        public void Save(object sender, object args)
        {
            var json = JsonUtility.ObjectToJson2(mapModels.ToList(), false, true, Newtonsoft.Json.Formatting.None);
            File.WriteAllText(this.SaveFile, json);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            mapModels.Clear();
            return base.OnDeactivateAsync(close, cancellationToken);
        }


        public void canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsDragable && tabSelectedItem != null && tabSelectedItem.selectedGridItem != null && e.LeftButton == MouseButtonState.Pressed)
            {
                if (tabSelectedItem.selectedGridItem is CustomEllipse)
                {
                    Canvas uielement = sender as Canvas;
                    tabSelectedItem.dragableoint = e.GetPosition(uielement);
                    var ellipse = tabSelectedItem.selectedGridItem as CustomEllipse;
                    ellipse.Tag = new Point(ellipse.Left, ellipse.Top);
                    tabSelectedItem.isDragable = true;
                    e.Handled = true;
                }
            }
        }
        public void canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            if (this.IsDragable && e.LeftButton == MouseButtonState.Pressed)
            {
                if (tabSelectedItem != null && tabSelectedItem.selectedGridItem != null && tabSelectedItem.isDragable)
                {
                    if (tabSelectedItem.selectedGridItem is CustomEllipse)
                    {
                        var ellipse = tabSelectedItem.selectedGridItem as CustomEllipse;

                        Canvas uielement = sender as Canvas;
                        Point currentPoint = e.GetPosition(uielement);
                        Vector v = currentPoint - tabSelectedItem.dragableoint;
                        tabSelectedItem.previousPoint = currentPoint;

                        var originPoint = (Point)ellipse.Tag;

                        ellipse.Left = originPoint.X + v.X;
                        ellipse.Top = originPoint.Y + v.Y;
                        var model = tabSelectedItem.GetEliipseLines(ellipse);

                        if (model.DownLine != null)
                        {
                            var startPoint = new Point(ellipse.Left + (ellipse.Width / 2), ellipse.Top + ellipse.Height);
                            var down_ellipse = tabSelectedItem.GetEllipse(model.DownLine.EndRowIndex, model.DownLine.EndColumnIndex);
                            var endPoint = new Point(down_ellipse.Left + (down_ellipse.Width / 2), down_ellipse.Top);
                            model.DownLine.ChangePoint(startPoint, endPoint);
                        }
                        if (model.UpLine != null)
                        {
                            var startPoint = new Point(ellipse.Left + (ellipse.Width / 2), ellipse.Top);
                            var up_ellipse = tabSelectedItem.GetEllipse(model.UpLine.StartRowIndex, model.UpLine.StartColumnIndex);
                            var endPoint = new Point(up_ellipse.Left + (up_ellipse.Width / 2), up_ellipse.Top + up_ellipse.Height);
                            model.UpLine.ChangePoint(startPoint, endPoint);
                        }
                        if (model.LeftLine != null)
                        {
                            var startPoint = new Point(ellipse.Left, ellipse.Top + (ellipse.Height / 2));
                            var left_ellipse = tabSelectedItem.GetEllipse(model.LeftLine.StartRowIndex, model.LeftLine.StartColumnIndex);
                            var endPoint = new Point(left_ellipse.Left + (left_ellipse.Width), left_ellipse.Top + left_ellipse.Height / 2);
                            model.LeftLine.ChangePoint(startPoint, endPoint);
                        }
                        if (model.RightLine != null)
                        {
                            var startPoint = new Point(ellipse.Left + (ellipse.Width), ellipse.Top + ellipse.Height / 2);
                            var right_ellipse = tabSelectedItem.GetEllipse(model.RightLine.EndRowIndex, model.RightLine.EndColumnIndex);
                            var endPoint = new Point(right_ellipse.Left, right_ellipse.Top + right_ellipse.Height / 2);
                            model.RightLine.ChangePoint(startPoint, endPoint);
                        }
                    }

                }
                e.Handled = true;
            }
        }
        public void canvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsDragable && tabSelectedItem != null && e.LeftButton == MouseButtonState.Released && tabSelectedItem.isDragable)
            {
                //Canvas uielement = sender as Canvas;
                //Point endMovePosition = e.GetPosition(uielement);

                ////为了避免跳跃式的变换，单次有效变化 累加入 totalTranslate中。
                //(tabSelectedItem.selectedGridItem.Tag as EllipseBlockTag).TotalTranslate.X += (endMovePosition.X - (tabSelectedItem.selectedGridItem.Tag as EllipseBlockTag).startMovePosition.X);
                //(tabSelectedItem.selectedGridItem.Tag as EllipseBlockTag).TotalTranslate.Y += (endMovePosition.Y - (tabSelectedItem.selectedGridItem.Tag as EllipseBlockTag).startMovePosition.Y);
                tabSelectedItem.selectedGridItem = null;
                tabSelectedItem.isDragable = false;
            }
        }


        public void Resize(object sender, object args)
        {
            //CanvasWidth
            //CanvasHeight
            foreach (var floor in mapModels)
            {
                floor.selectedGridItem = null;

                var originCellWidth = floor.canvasWidth / floor.gridColumns;
                var originCellHeight = floor.canvasHeight / floor.gridRows;
                originCellHeight = originCellWidth = Math.Min(originCellHeight, originCellWidth);//画出的格子默认正方形

                var mWidth = CanvasWidth / floor.gridColumns;
                var mHeight = CanvasHeight / floor.gridRows;

                floor.cellHeight = floor.cellWidth = Math.Min(mWidth, mHeight);//画出的格子默认正方形

                floor.canvasHeight = floor.cellHeight * floor.gridRows;
                floor.canvasWidth = floor.cellWidth * floor.gridColumns;

                var allEllipses = floor.canvasItemsSource.Where(w => w is CustomEllipse).ToList();
                foreach (var item in allEllipses)
                {
                    CustomEllipse ellipse = item as CustomEllipse;
                    var oldLeft = ellipse.ColumnIndex * originCellWidth + (originCellWidth / 2 - originCellWidth / 8);
                    var oldTop = ellipse.RowIndex * originCellHeight + (originCellHeight / 2 - originCellHeight / 8);

                    var subLeft = oldLeft - ellipse.Left;
                    var subTop = oldTop - ellipse.Top;

                    var ratioW = floor.cellWidth / originCellWidth;
                    var ratioH = floor.cellHeight / originCellHeight;

                    var newLeft = ellipse.ColumnIndex * floor.cellWidth + (floor.cellWidth / 2 - floor.cellWidth / 8) - (subLeft * ratioW);
                    var newTop = ellipse.RowIndex * floor.cellHeight + (floor.cellHeight / 2 - floor.cellHeight / 8) - (subTop * ratioH);

                    //var newLeft = ellipse.ColumnIndex * floor.cellWidth + (floor.cellWidth / 2 - floor.cellWidth / 8) ;
                    //var newTop = ellipse.RowIndex * floor.cellHeight + (floor.cellHeight / 2 - floor.cellHeight / 8);
                    ellipse.Left = newLeft;
                    ellipse.Top = newTop;
                    ellipse.NewHeight = (floor.cellHeight / 4);
                    ellipse.NewWidth = (floor.cellWidth / 4);
                }

                var allLines = floor.canvasItemsSource.Where(w => w is CustomLine).ToList();
                foreach (var item in allLines)
                {
                    CustomLine line = item as CustomLine;
                    var startEllipse = floor.GetEllipse(line.StartRowIndex, line.StartColumnIndex);
                    var endEllipse = floor.GetEllipse(line.EndRowIndex, line.EndColumnIndex);
                    if (line.Id.StartsWith("h"))
                    {
                        var startPoint = new Point(startEllipse.Left + startEllipse.Width, startEllipse.Top + (startEllipse.Height / 2));
                        var endoint = new Point(endEllipse.Left, endEllipse.Top + (endEllipse.Height / 2));
                        line.ChangePoint(startPoint, endoint);
                    }
                    else if (line.Id.StartsWith("v"))
                    {
                        var startPoint = new Point(startEllipse.Left + (startEllipse.Width / 2), startEllipse.Top + (startEllipse.Height));
                        var endoint = new Point(endEllipse.Left + (startEllipse.Width / 2), endEllipse.Top);
                        line.ChangePoint(startPoint, endoint);
                    }
                }

                var textbox = floor.canvasBackgroudItemsSource.Where(w => w.Name== "tb_watermark").FirstOrDefault();
                floor.canvasBackgroudItemsSource.Remove(textbox);
                var watermark = DrawUtility.DrawWatermark(floor.Floor, floor.canvasWidth, floor.canvasHeight);
                floor.canvasBackgroudItemsSource.Add(watermark);

            }
        }

    }
}
