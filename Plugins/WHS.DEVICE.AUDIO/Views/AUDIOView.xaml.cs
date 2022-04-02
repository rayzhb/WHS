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
using WHS.DEVICE.AUDIO.Models;

namespace WHS.DEVICE.AUDIO.Views
{
    /// <summary>
    /// RFIDView.xaml 的交互逻辑
    /// </summary>
    public partial class AUDIOView : Page
    {
        public AUDIOView()
        {
            InitializeComponent();
        }

        private void RadioButton_Checked1(object sender, RoutedEventArgs e)
        {
            AudioData.PlayType = 0;
            
        }
        private void RadioButton_Checked2(object sender, RoutedEventArgs e)
        {
            AudioData.PlayType = 1;
        }
        private void RadioButton_Checked3(object sender, RoutedEventArgs e)
        {
            AudioData.LoadViceType = 0;
        }
        private void RadioButton_Checked4(object sender, RoutedEventArgs e)
        {
            AudioData.LoadViceType = 1;
        }
    }
}
