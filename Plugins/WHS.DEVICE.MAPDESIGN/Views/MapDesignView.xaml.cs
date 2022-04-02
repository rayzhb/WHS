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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using WHS.DEVICE.MAPDESIGN.Models;
using WHS.DEVICE.MAPDESIGN.ViewModels;

namespace WHS.DEVICE.MAPDESIGN.Views
{
    /// <summary>
    /// MapDesignView.xaml 的交互逻辑
    /// </summary>
    public partial class MapDesignView : Page
    {
        public MapDesignView()
        {
            InitializeComponent();
        }

        private async void MetroTabControl_TabItemClosingEvent(object sender, BaseMetroTabControl.TabItemClosingEventArgs e)
        {
            e.Cancel = true;
            var header = e.ClosingTabItem.Header;
            MessageDialogResult result = await ((MahApps.Metro.Controls.MetroWindow)Application.Current.MainWindow).ShowMessageAsync("删除操作", $"确定删除 '{header}' 这个页面吗？",MessageDialogStyle.AffirmativeAndNegative);
            if (result != MessageDialogResult.Canceled)
            {
                var vm = this.DataContext as MapDesignViewModel;
                var map = e.ClosingTabItem.DataContext as MapModel;
                vm.mapModels.Remove(map);
                if (vm.mapModels.Count > 0)
                {
                    vm.tabSelectedIndex = 0;
                }
            }
        }
    }
}
