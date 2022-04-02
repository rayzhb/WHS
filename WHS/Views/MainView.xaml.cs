using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WHS.Infrastructure;
using System.Windows.Forms;
using WHS.Infrastructure.Events;

namespace WHS.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : MetroWindow, IHandle<EventMessage>
    {
        private NotifyIcon notifyIcon;

        public MainView()
        {
            InitializeComponent();
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.BalloonTipText = "系统运行中... ...";
            this.notifyIcon.ShowBalloonTip(2000);
            this.notifyIcon.Text = "仓储硬件服务";
            this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this.notifyIcon.Visible = true;
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("显示", null, new EventHandler(showMenuItem_Click));
            notifyIcon.ContextMenuStrip.Items.Add("隐藏", null, new EventHandler(hideMenuItem_Click));
            notifyIcon.ContextMenuStrip.Items.Add("退出", null, new EventHandler(quitMenuItem_Click));
            //notifyIcon的MouseDown事件。
            //改为双击
            notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);
            //不让在任务栏显示。
            this.ShowInTaskbar = false;
            GlobalContext.EventAggregator.SubscribeOnPublishedThread(this);
        }


        private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //如果点击了鼠标左键。
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    GlobalContext.EventAggregator.PublishAsync(new EventMessage() { EventDefinition = EventDefinition.最小化托盘双击 }, (action) => { return action(); });
                }
                catch
                { }
                //如果隐藏就显示，否则就隐藏。
                if (this.Visibility == System.Windows.Visibility.Visible)
                {
                    Visibility = Visibility.Hidden;
                    this.Hide();
                }
                else
                {
                    this.Visibility = System.Windows.Visibility.Visible;
                    Show();
                    this.Activate();
                }
            }
        }

        private void quitMenuItem_Click(object sender, EventArgs e)
        {
            if (System.Windows.MessageBox.Show("你确定要退出吗？", "温馨提醒。", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                try
                {
                    GlobalContext.EventAggregator.PublishAsync(new EventMessage() { EventDefinition = EventDefinition.主界面退出 }, (action) => { return action(); });
                }
                catch
                { }
                this.Close();
            }
        }

        private void hideMenuItem_Click(object sender, EventArgs e)
        {
            //如果此窗口是显示模式就隐藏。
            if (this.Visibility == System.Windows.Visibility.Visible)
            {
                Visibility = Visibility.Hidden;
                this.Hide();
            }
        }

        private void showMenuItem_Click(object sender, EventArgs e)
        {
            //如果此窗口是隐藏模式就显示。
            if (this.Visibility == System.Windows.Visibility.Hidden)
            {
                this.Visibility = System.Windows.Visibility.Visible;
                Show();
                this.Activate();
            }
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private async void ButtonClose_Click(object sender, RoutedEventArgs e)
        {

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "关闭",
                NegativeButtonText = "取消",
                FirstAuxiliaryButtonText = "最小化",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            MessageDialogResult result = await this.ShowMessageAsync("温馨提醒!", "你确定要退出吗?",
                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);


            if (result == MessageDialogResult.Affirmative)
            {
                this.Close();
            }
            else if (result == MessageDialogResult.FirstAuxiliary)
            {
                this.Hide();
            }
        }


        protected override void OnClosed(EventArgs e)
        {
            this.notifyIcon.Dispose();
            base.OnClosed(e);
        }

        public Task HandleAsync(EventMessage e, CancellationToken cancellationToken)
        {
            if (e.EventDefinition == EventDefinition.RFID开始盘点)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Hide();
                    notifyIcon.Visible = true;
                }
            }
            else if (e.EventDefinition == EventDefinition.弹出框)
            {
                if (e.EventData != null)
                {
                    EventMessageDialog eventMessageDialog = e.EventData as EventMessageDialog;
                    var result= DialogManager.ShowModalMessageExternal(this, eventMessageDialog.Title,
                       eventMessageDialog.Message
                       , eventMessageDialog.MessageDialogStyle);
                }
            }
            return Task.FromResult(0);
        }
    }
}
