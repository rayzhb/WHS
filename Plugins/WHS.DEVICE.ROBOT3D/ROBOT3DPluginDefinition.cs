using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WHS.DEVICE.ROBOT3D.ViewModels;
using WHS.Infrastructure;

namespace WHS.DEVICE.ROBOT3D
{
    public class ROBOT3DPluginDefinition : PluginDefinition
    {

        public override Guid Id
        {
            get
            {
                return new Guid("F7E205C5-3AF8-42A3-9CB6-1F41B5C2C750");
            }
        }

        public override string Name
        {
            get
            {
                return "3D机器人";
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
                return Properties.Resource.robot;
            }
        }
        public override Type ViewModel
        {
            get
            {
                return typeof(ROBOT3DViewModel);
            }
        }

        internal static Infrastructure.Config.JsonConfig JsonConfig
        {
            get; set;
        }

        public static string PluginDir;

        public override async void Init()
        {
            
            base.Log.Info("3D机器人初始化");
            base.Init();
            GlobalContext.SimpleContainer.PerRequest<ROBOT3DViewModel>();
            JsonConfig = new Infrastructure.Config.JsonConfig(FileName);

            PluginDir = Directory.GetParent(FileName).FullName;
        }


        public override void Close()
        {
            base.Close();
            JsonConfig?.Dispose();
            GlobalContext.SimpleContainer.UnregisterHandler<ROBOT3DViewModel>();
            base.Log.Info("3D机器人关闭");
        }

        /// <summary>
        /// 处理客户端关闭
        /// </summary>
        /// <param name="channelid"></param>
        protected override void OnHandleCommunicationClose(string channelid)
        {

        }

    }
}
