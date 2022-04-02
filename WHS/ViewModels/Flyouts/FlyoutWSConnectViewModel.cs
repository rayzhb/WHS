using Caliburn.Micro;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WHS.Infrastructure;
using WHS.Models;
using WHS.Server;

namespace WHS.ViewModels.Flyouts
{
    public class FlyoutWSConnectViewModel : FlyoutBaseViewModel
    {

        private IObservableCollection<WSConnectModel> _WSConnects;
        public IObservableCollection<WSConnectModel> WSConnects
        {
            get { return _WSConnects; }
            set { Set(ref _WSConnects, value, "WSConnects"); }
        }
        public FlyoutWSConnectViewModel()
        {
            WSConnects = new BindableCollection<WSConnectModel>();
            this.Name = "WSConnect";
            this.Header = LocalizationHelp.GetLocalizedString(this.GetType().Assembly, "WSConnectView.Content.Title");
            this.Position = MahApps.Metro.Controls.Position.Right;

        }

        public void RefreshGrid()
        {
            WSConnects.Clear();

            foreach (var v in WebSocketsServer.s_onlineClients.Values.ToList())
            {
                WSConnectModel wSConnectModel = new WSConnectModel();
                wSConnectModel.Id = v.Channel.Id.ToString();
                wSConnectModel.Address = v.Channel.RemoteAddress.ToString();
                WSConnects.Add(wSConnectModel);
            }
        }

        public void RemoveConnection(WSConnectModel data)
        {
            if (WebSocketsServer.s_onlineClients.ContainsKey(data.Id))
            {
                foreach (PluginDefinition item in PluginManager.GetPluginDefinitions())
                {
                    item.handleCommunicationClose?.Invoke(data.Id);
                }
                WebSocketsServer.s_onlineClients[data.Id].CloseAsync();
                IChannelHandlerContext context;
                WebSocketsServer.s_onlineClients.TryRemove(data.Id, out context);
            }
            RefreshGrid();
        }

        public override void ChangeLanguage()
        {
            this.Header = LocalizationHelp.GetLocalizedString(this.GetType().Assembly, "WSConnectView.Content.Title");

        }
    }
}
