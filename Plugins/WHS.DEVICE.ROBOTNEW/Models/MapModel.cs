using Caliburn.Micro;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WHS.DEVICE.ROBOTNEW.Commons;
using WHS.DEVICE.ROBOTNEW.Controls;
using WHS.DEVICE.ROBOTNEW.ViewModels;
using WHS.DEVICE.ROBOTNEW.Views;

namespace WHS.DEVICE.ROBOTNEW.Models
{
    public class MapModel : PropertyChangedBase
    {
        private int _Floor;
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

        private ThreadSafeObservableCollection<object> _canvasItemsSource;
        public ThreadSafeObservableCollection<object> canvasItemsSource
        {
            get
            {
                return _canvasItemsSource;
            }
            set
            {
                _canvasItemsSource = value;
                NotifyOfPropertyChange(() => canvasItemsSource);
            }
        }

        private ObservableCollection<FrameworkElement> _canvasBackgroudItemsSource;
        public ObservableCollection<FrameworkElement> canvasBackgroudItemsSource
        {
            get
            {
                return _canvasBackgroudItemsSource;
            }
            set
            {
                _canvasBackgroudItemsSource = value;
                NotifyOfPropertyChange(() => canvasBackgroudItemsSource);
            }
        }

        private ThreadSafeObservableCollection<object> _canvasPathItemsSource;
        public ThreadSafeObservableCollection<object> canvasPathItemsSource
        {
            get
            {
                return _canvasPathItemsSource;
            }
            set
            {
                _canvasPathItemsSource = value;
                NotifyOfPropertyChange(() => canvasPathItemsSource);
            }
        }

        private double _ScaleValue;
        /// <summary>
        /// 缩放
        /// </summary>
        public double ScaleValue
        {
            get
            {
                return _ScaleValue;
            }
            set
            {
                _ScaleValue = value;
                NotifyOfPropertyChange(() => ScaleValue);
            }
        }

        /// <summary>
        /// 画布-宽
        /// </summary>
        public double canvasWidth
        {
            get; set;
        }
        /// <summary>
        /// 画布-高
        /// </summary>
        public double canvasHeight
        {
            get; set;
        }


        /// <summary>
        /// 单元格-宽度
        /// </summary>
        public double cellWidth
        {
            get; set;
        }

        /// <summary>
        /// 单元格-高度
        /// </summary>
        public double cellHeight
        {
            get; set;
        }

        public int gridRows;
        public int gridColumns;

        private double _DriftX;
        public double DriftX
        {
            get
            {
                return _DriftX;
            }
            set
            {
                _DriftX = value;
                NotifyOfPropertyChange(() => DriftX);
            }
        }
        private double _DriftY;
        public double DriftY
        {
            get
            {
                return _DriftY;
            }
            set
            {
                _DriftY = value;
                NotifyOfPropertyChange(() => DriftY);
            }
        }

        public Point previousPoint
        {
            get; set;
        }
        public bool isTranslateStart
        {
            get; set;
        }

        public MapModel(int floor)
        {
            Floor = floor;
            ScaleValue = 1d;
            DriftX = 0;
            DriftY = 0;
            canvasItemsSource = new ThreadSafeObservableCollection<object>();
            canvasBackgroudItemsSource = new ObservableCollection<FrameworkElement>();
            canvasPathItemsSource = new ThreadSafeObservableCollection<object>();

        }

        public static MapModel ConvertJToken(JToken token)
        {
            int floor = token["Floor"].Value<int>();
            MapModel map = new MapModel(floor);
            map.gridRows = token["gridRows"].Value<int>();
            map.gridColumns = token["gridColumns"].Value<int>();
            map.canvasHeight = token["canvasHeight"].Value<double>();
            map.canvasWidth = token["canvasWidth"].Value<double>();
            var subjArray = (JArray)token["mapinfo"];
            map.canvasBackgroudItemsSource.Add(DrawUtility.DrawWatermark(floor, map.canvasWidth, map.canvasHeight));
            foreach (var subitem in subjArray)
            {
                var drawtype = subitem["dtype"].Value<string>();
                if (drawtype == "CustomLine")
                {
                    map.canvasBackgroudItemsSource.Add(CustomLine.ConvertJToken(subitem));
                }
                else if (drawtype == "CustomEllipse")
                {
                    map.canvasItemsSource.Add(CustomEllipse.ConvertJToken(subitem));
                }
            }
            return map;
        }

        public CustomEllipse GetEllipse(int Row, int Column)
        {
            return System.Windows.Application.Current.Dispatcher.Invoke(() =>
             {
                 var result = this.canvasItemsSource.Where(w => w is CustomEllipse &&
                     ((CustomEllipse)w).RowIndex == Row && ((CustomEllipse)w).ColumnIndex == Column).FirstOrDefault();
                 if (result == null)
                     return null;
                 else
                     return result as CustomEllipse;
             });
        }

        public CustomLine GetLine(int startRow, int startColumn, int endRow, int endColumn)
        {
            return
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var result = this.canvasBackgroudItemsSource.Where(w => w is CustomLine &&
                 ((CustomLine)w).StartRowIndex == startRow && ((CustomLine)w).StartColumnIndex == startColumn
                 && ((CustomLine)w).EndRowIndex == endRow
                 && ((CustomLine)w).EndColumnIndex == endColumn).FirstOrDefault();
                    if (result == null)
                        return null;
                    else
                        return result as CustomLine;
                });
            ;
        }

        public EliipseLineModel GetEliipseLines(int Row, int Column)
        {
            return
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var result = this.canvasBackgroudItemsSource.Where(w => w is CustomLine &&
                    ((((CustomLine)w).StartRowIndex == Row && ((CustomLine)w).StartColumnIndex == Column) ||
                    (((CustomLine)w).EndRowIndex == Row && ((CustomLine)w).EndColumnIndex == Column)))
               .ToList();

                    EliipseLineModel eliipseLineModel = new EliipseLineModel();
                    foreach (var item in result)
                    {
                        var line = item as CustomLine;
                        if (line.StartRowIndex == Row && line.EndRowIndex == Row &&
                            line.StartColumnIndex < Column && line.EndColumnIndex == Column)
                        {
                            eliipseLineModel.LeftLine = line;
                        }
                        else if (line.StartRowIndex == Row && line.EndRowIndex == Row &&
                         line.StartColumnIndex == Column && line.EndColumnIndex > Column)
                        {
                            eliipseLineModel.RightLine = line;
                        }
                        else if (line.StartRowIndex == Row && line.EndRowIndex > Row &&
         line.StartColumnIndex == Column && line.EndColumnIndex == Column)
                        {
                            eliipseLineModel.DownLine = line;
                        }
                        else if (line.StartRowIndex < Row && line.EndRowIndex == Row &&
        line.StartColumnIndex == Column && line.EndColumnIndex == Column)
                        {
                            eliipseLineModel.UpLine = line;
                        }
                    }
                    return eliipseLineModel;
                });
        }


        public RES_MapModel ConvertToRESMapModel()
        {
            RES_MapModel floorMapModel = new RES_MapModel();
            floorMapModel.Floor = this.Floor;
            floorMapModel.canvasHeight = this.canvasHeight;
            floorMapModel.canvasWidth = this.canvasWidth;
            floorMapModel.gridColumns = this.gridColumns;
            floorMapModel.gridRows = this.gridRows;
            floorMapModel.MapInfo = new List<FrameworkElement>();
            var lines = this.canvasBackgroudItemsSource.Where(w => w is CustomLine).Select(w => (CustomLine)w).ToList();
            floorMapModel.MapInfo.AddRange(lines);

            var ellipses = this.canvasItemsSource.Where(w => w is CustomEllipse).Select(w => (CustomEllipse)w).ToList();
            floorMapModel.MapInfo.AddRange(ellipses);
            return floorMapModel;
        }


    }
}
