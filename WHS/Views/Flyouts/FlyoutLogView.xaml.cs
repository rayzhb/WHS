using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using WHS.Infrastructure.NlogEx;
using WHS.ViewModels.Flyouts;

namespace WHS.Views.Flyouts
{
    /// <summary>
    /// FlyoutLogView.xaml 的交互逻辑
    /// </summary>
    public partial class FlyoutLogView : UserControl
    {
        private LimitedMemoryTarget _target;
        public FlyoutLogView()
        {
            InitializeComponent();
            _target = NLog.LogManager.Configuration.FindTargetByName<LimitedMemoryTarget>("limitMemory");
            _target.FireLog += Target_FireLog;
        }

        private void Target_FireLog(StringBuilder stringBuilder,string newMessage)
        {
            Dispatcher.BeginInvoke(new ThreadSafeRightText(() =>
            {
                var flyoutLogViewModel = this.DataContext as FlyoutLogViewModel;
                if (flyoutLogViewModel.LogTarget == null)
                {
                    flyoutLogViewModel.LogTarget = _target;
                }
                if (flyoutLogViewModel.IsOpen)
                {
                    this.Paragraph.Text = stringBuilder.ToString();
                    this.RichTextBox.ScrollToEnd();
                    transitioning.Content = new TextBlock { Text = "新:"+newMessage, SnapsToDevicePixels = true,TextWrapping= TextWrapping.WrapWithOverflow };

                }
            }));
        }

        public delegate void ThreadSafeRightText();
    }
}
