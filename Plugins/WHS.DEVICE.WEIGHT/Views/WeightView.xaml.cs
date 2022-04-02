using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using WHS.DEVICE.WEIGHT;

namespace WHS.DEVICE.WEIGHT.Views
{
    /// <summary>
    /// WeightView.xaml 的交互逻辑
    /// </summary>
    public partial class WeightView : Page
    {
        public WeightView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in dg.BindingExpressions)
            {
                item.UpdateSource();
            }
            bool Isvalidation = dg.ValidateWithoutUpdate();
            if (!Isvalidation)
            {
                return;
            }
            var viewmodel = this.DataContext as ViewModels.WeightViewModel;
            var weightSettingModel = viewmodel.weightSettingModel;
            weightSettingModel.EWType = viewmodel.EWType;
            weightSettingModel.PortName = viewmodel.PortName;
            weightSettingModel.BaudRate = viewmodel.BaudRate;
            weightSettingModel.DataBits = viewmodel.DataBits;
            weightSettingModel.Parity = viewmodel.Parity;
            weightSettingModel.StopBits = viewmodel.StopBits;

            try
            {
                WeightPluginDefinition.JsonConfig.SaveUserConfig(weightSettingModel);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
