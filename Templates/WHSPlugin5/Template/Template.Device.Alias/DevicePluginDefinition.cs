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

namespace Template.Device.Alias
{
    public class DevicePluginDefinition : PluginDefinition
    {

        public override Guid Id
        {
            get
            {
                //按照下面生成一个GUID
                //return new Guid("xxxxxxxxxxxxxxxxxxx");
            }
        }

        public override string Name
        {
            get
            {
                return "DisplayName";
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
                return typeof(ViewModels.DeviceViewModel);
            }
        }

        internal static JsonConfig JsonConfig
        {
            get; set;
        }

        public override void Init()
        {
            base.Init();
            GlobalContext.SimpleContainer.PerRequest<ViewModels.DeviceViewModel>();
            JsonConfig = new JsonConfig(FileName);


        }


        public override void Close()
        {
            base.Close();
            JsonConfig?.Dispose();
            GlobalContext.SimpleContainer.UnregisterHandler<ViewModels.DeviceViewModel>();
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
