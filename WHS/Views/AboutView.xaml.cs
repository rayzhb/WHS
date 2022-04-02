using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace WHS.Views
{
    /// <summary>
    /// AboutView.xaml 的交互逻辑
    /// </summary>
    public partial class AboutView : Page
    {
        public AboutView()
        {
            InitializeComponent();
        }


        private void LaunchEmail(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "mailto:bbaxx1314@qq.com",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void LaunchWHS(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "http://localhost:18080/",
                UseShellExecute = true
            };
            Process.Start(psi);
        }


        private void LaunchAGV(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "http://localhost:18080/vue_rcs",
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}
