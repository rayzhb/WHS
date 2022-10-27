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

namespace WHS.IDMVS
{
    public class DevicePluginDefinition : PluginDefinition
    {

        public override Guid Id
        {
            get
            {
                //按照下面生成一个
                return new Guid("49D23070-7574-4CE0-83F2-D2228042FE95");
            }
        }

        public override string Name
        {
            get
            {
                return "海康IDMVS";
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
                return Properties.Resource.scan;
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
