using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WHS.DEVICE.MAPDESIGN.Commons;
using WHS.DEVICE.MAPDESIGN.Controls;
using WHS.DEVICE.MAPDESIGN.ViewModels;
using WHS.DEVICE.MAPDESIGN.Views;

namespace WHS.DEVICE.MAPDESIGN.Models
{
    [Serializable]
    public class MapModel : PropertyChangedBase
    {
        internal Stack<UndoModel> undoList
        {
            get; set;
        }
        internal Stack<UndoModel> redoList
        {
            get; set;
        }

        private int _Floor;
        /// <summary>
        /// 楼层
        /// </summary>
        [JsonProperty]
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

        private ObservableCollection<FrameworkElement> _canvasItemsSource;
        /// <summary>
        /// 元素集合
        /// </summary>
        [JsonProperty("mapinfo")]
        public ObservableCollection<FrameworkElement> canvasItemsSource
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
        /// <summary>
        /// 背景元素集合
        /// </summary>
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

        private FrameworkElement _selectedGridItem;
        /// <summary>
        /// 选中的元素
        /// </summary>
        public FrameworkElement selectedGridItem
        {
            get
            {
                return _selectedGridItem;
            }
            set
            {
                _selectedGridItem = value;
                NotifyOfPropertyChange(() => selectedGridItem);
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
        [JsonProperty]
        public double canvasWidth
        {
            get; set;
        }
        /// <summary>
        /// 画布-高
        /// </summary>
        [JsonProperty]
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

        /// <summary>
        /// 行数
        /// </summary>
        [JsonProperty]
        public int gridRows;
        /// <summary>
        /// 列数
        /// </summary>
        [JsonProperty]
        public int gridColumns;

        private double _DriftX;
        /// <summary>
        /// 水平平移
        /// </summary>
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
        /// <summary>
        /// 垂直平移
        /// </summary>
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

        /// <summary>
        /// 鼠标点-用于平移画布
        /// </summary>
        public Point previousPoint
        {
            get; set;
        }

        public bool isTranslateStart
        {
            get; set;
        }

        public Point dragableoint
        {
            get; set;
        }

        public bool isDragable
        {
            get; set;
        }

        public MapModel(int floor)
        {
            Floor = floor;
            ScaleValue = 1d;
            DriftX = 0;
            DriftY = 0;
            canvasItemsSource = new ObservableCollection<FrameworkElement>();
            canvasBackgroudItemsSource = new ObservableCollection<FrameworkElement>();
            undoList = new Stack<UndoModel>(1000);
            redoList = new Stack<UndoModel>(1000);
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
            foreach (var subitem in subjArray)
            {
                var drawtype = subitem["dtype"].Value<string>();
                if (drawtype == "CustomLine")
                {
                    map.canvasItemsSource.Add(CustomLine.ConvertJToken(subitem));
                }
                else if (drawtype == "CustomEllipse")
                {
                    map.canvasItemsSource.Add(CustomEllipse.ConvertJToken(subitem));
                }
            }
            return map;
        }

        public EliipseLineModel GetEliipseLines(int Row, int Column)
        {
            var result = this.canvasItemsSource.Where(w => w is CustomLine &&
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
        }

        public EliipseLineModel GetEliipseLines(CustomEllipse ellipse)
        {
            return GetEliipseLines(ellipse.RowIndex, ellipse.ColumnIndex);
        }

        public CustomEllipse GetEllipse(int Row, int Column)
        {
            var result = this.canvasItemsSource.Where(w => w is CustomEllipse &&
                    ((CustomEllipse)w).RowIndex == Row && ((CustomEllipse)w).ColumnIndex == Column).FirstOrDefault();
            if (result == null)
                return null;
            else
                return result as CustomEllipse;
        }
    }
}
