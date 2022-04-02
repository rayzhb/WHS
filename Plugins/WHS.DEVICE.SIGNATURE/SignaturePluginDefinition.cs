using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WHS.DEVICE.SIGNATURE.ViewModels;
using WHS.Infrastructure;

namespace WHS.DEVICE.SIGNATURE
{ 
    public class SignaturePluginDefinition : PluginDefinition
    {
        public override Guid Id
        {
            get
            {
                return new Guid("5133639C-8539-45BC-99BA-C0B55EAF2A15");
            }
        }

        public override string Name
        {
            get
            {
                return "签名";
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
                return SIGNATURE.Properties.Resource.signature;
            }
        }
        public override Type ViewModel
        {
            get
            {
                return typeof(SignatureViewModel);
            }
        }

        internal static Infrastructure.Config.JsonConfig JsonConfig { get; set; }

        internal static string s_filename { get; set; }
        public override void Init()
        {
            base.Log.Info("签名初始化");
            base.Init();
            GlobalContext.SimpleContainer.PerRequest<SignatureViewModel>();
           s_filename = FileName;
        }


        public override void Close()
        {
            base.Close();
            GlobalContext.SimpleContainer.UnregisterHandler<SignatureViewModel>();
            base.Log.Info("称重关闭");
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
