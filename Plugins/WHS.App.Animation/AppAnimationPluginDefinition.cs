using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WHS.Infrastructure;
using WHS.Infrastructure.Config;

namespace WHS.App.Animation
{
    public class AppAnimationPluginDefinition : PluginDefinition
    {
        internal static string s_filename;

        public override Guid Id
        {
            get
            {
                return new Guid("DBDA3AA6-FA94-4300-B22C-8D537AB89BCA");
            }
        }

        public override string Name
        {
            get
            {
                return "WHS.App.Animation";
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
                return Properties.Resource.plugin;
            }
        }
        public override Type ViewModel
        {
            get
            {
                return typeof(ViewModels.MainViewModel);
            }
        }

        internal static JsonConfig JsonConfig
        {
            get; set;
        }

        public override void Init()
        {
            base.Init();
            GlobalContext.SimpleContainer.PerRequest<ViewModels.MainViewModel>();
            JsonConfig = new JsonConfig(FileName);
            s_filename=FileName;
        }


        public override void Close()
        {
            base.Close();
            JsonConfig?.Dispose();
            GlobalContext.SimpleContainer.UnregisterHandler<ViewModels.MainViewModel>();
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
