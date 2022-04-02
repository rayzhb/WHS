using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WHS.DEVICE.MAPDESIGN.ViewModels;
using WHS.Infrastructure;

namespace WHS.DEVICE.MAPDESIGN
{ 
    public class MapDesignPluginDefinition : PluginDefinition
    {
        public override Guid Id
        {
            get
            {
                return new Guid("052E9BCB-73C8-4979-BB60-D18DC0CD27FB");
            }
        }

        public override string Name
        {
            get
            {
                return "地图设计";
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
                return MAPDESIGN.Properties.Resource.design;
            }
        }
        public override Type ViewModel
        {
            get
            {
                return typeof(MapDesignViewModel);
            }
        }

        internal static Infrastructure.Config.JsonConfig JsonConfig { get; set; }

        internal static string s_filename { get; set; }
        public override void Init()
        {
            base.Log.Info("地图设计器后台初始化");
            base.Init();
            GlobalContext.SimpleContainer.PerRequest<MapDesignViewModel>();
           s_filename = FileName;
        }


        public override void Close()
        {
            base.Close();
            GlobalContext.SimpleContainer.UnregisterHandler<MapDesignViewModel>();
            base.Log.Info("地图设计器后台关闭");
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
