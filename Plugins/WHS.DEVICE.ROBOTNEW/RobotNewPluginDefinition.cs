using Caliburn.Micro;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WHS.DEVICE.ROBOTNEW.ViewModels;
using WHS.Infrastructure;

namespace WHS.DEVICE.ROBOTNEW
{
    public class RobotNewPluginDefinition : PluginDefinition
    {
        internal static ConcurrentBag<string> sConnectChannels;

        public override Guid Id
        {
            get
            {
                return new Guid("886E5603-E8AB-41C3-892B-7CE33041C932");
            }
        }

        public override string Name
        {
            get
            {
                return "机器人(新)";
            }
        }

        public override string VersionString
        {
            get
            {
                return "1.0.0.1";
            }
        }

        public override string Manufacturer
        {
            get
            {
                return "rayzhb";
            }
        }

        public override System.Drawing.Bitmap Icon
        {
            get
            {
                return ROBOTNEW.Properties.Resource.robot;
            }
        }
        public override Type ViewModel
        {
            get
            {
                return typeof(RobotNewViewModel);
            }
        }

        internal static Infrastructure.Config.JsonConfig JsonConfig
        {
            get; set;
        }

        internal static string s_filename
        {
            get; set;
        }
        public override void Init()
        {
            base.Log.Info("机器人后台初始化");
            base.Init();
            sConnectChannels = new ConcurrentBag<string>();
            GlobalContext.SimpleContainer.Singleton<RobotNewViewModel>();
            var vm = GlobalContext.SimpleContainer.GetInstance<RobotNewViewModel>();
            s_filename = FileName;
        }


        public override void Close()
        {
            base.Close();
            //GlobalContext.SimpleContainer.UnregisterHandler<RobotViewModel>();
            base.Log.Info("机器人后台关闭");
        }

        /// <summary>
        /// 处理客户端关闭
        /// </summary>
        /// <param name="channelid"></param>
        protected override void OnHandleCommunicationClose(string channelid)
        {
            sConnectChannels.TryTake(out channelid);
        }
    }
}
