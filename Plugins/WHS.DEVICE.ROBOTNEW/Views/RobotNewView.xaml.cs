using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WHS.DEVICE.ROBOTNEW.ViewModels;

namespace WHS.DEVICE.ROBOTNEW.Views
{
    /// <summary>
    /// RobotView.xaml 的交互逻辑
    /// </summary>
    public partial class RobotNewView : Page
    {

        public RobotNewView()
        {
            InitializeComponent();
        }

        private void CanvasPanel_MouseMove(object sender, MouseEventArgs e)
        {
            var c = sender as Canvas;
            var posizion = e.GetPosition(c);
            this.lb_InfoMessage.Content = $"当前位置:X={posizion.X} Y={posizion.Y}";
        }

        private void inside_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RobotNewViewModel vm = this.DataContext as RobotNewViewModel;
            vm.currentCanvasWidth = e.NewSize.Width;
            vm.currentCanvasHeight = e.NewSize.Height;
            foreach (var map in vm.agvPlatformModels)
            {
                if (vm.currentCanvasWidth > map.canvasWidth)
                {
                    map.DriftX = (vm.currentCanvasWidth - map.canvasWidth) / 2;
                }
                if (vm.currentCanvasHeight > map.canvasHeight)
                {
                    map.DriftY = (vm.currentCanvasHeight - map.canvasHeight) / 2;
                }
            }
        }
    }
}
