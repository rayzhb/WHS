using Caliburn.Micro;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WHS.DEVICE.AUDIO.Models;
using WHS.DEVICE.AUDIO.ViewModels;
using WHS.Infrastructure;

namespace WHS.DEVICE.AUDIO
{
    public class AudioPluginDefinition : PluginDefinition
    {

        public override Guid Id
        {
            get
            {
                return new Guid("46647C64-656A-433D-8D30-090C46975A10");
            }
        }

        public override string Name
        {
            get
            {
                return "声音";
            }
        }

        public override string VersionString
        {
            get
            {
                return "1.0.0.2";
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
                return AUDIO.Properties.Resource.RFID;
            }
        }
        public override Type ViewModel
        {
            get
            {
                return typeof(AUDIOViewModel);
            }
        }

        internal static Infrastructure.Config.JsonConfig JsonConfig { get; set; }

        public override async void Init()
        {
            base.Log.Info("声音初始化");
            base.Init();
            GlobalContext.SimpleContainer.PerRequest<AUDIOViewModel>();
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            JsonConfig = new Infrastructure.Config.JsonConfig(FileName);

            AudioData.LoadViceType = 0;
            AudioData.PlayType = 0;
        }


        public override void Close()
        {
            base.Close();
            JsonConfig?.Dispose();
            GlobalContext.SimpleContainer.UnregisterHandler<AUDIOViewModel>();
            base.Log.Info("声音关闭");
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
