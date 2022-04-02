using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WHS.DEVICE.WEIGHT.Actions;
using WHS.DEVICE.WEIGHT.ViewModels;
using WHS.Infrastructure;

namespace WHS.DEVICE.WEIGHT
{
    public class WeightPluginDefinition : PluginDefinition
    {

        public override Guid Id
        {
            get
            {
                return new Guid("E389196E-77CD-4840-AAFA-15CD61901086");
            }
        }

        public override string Name
        {
            get
            {
                return "称重";
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
                return WEIGHT.Properties.Resource.Weight_Libra;
            }
        }
        public override Type ViewModel
        {
            get
            {
                return typeof(WeightViewModel);
            }
        }

        internal static Infrastructure.Config.JsonConfig JsonConfig { get; set; }
        public override void Init()
        {
            base.Log.Info("称重初始化");
            base.Init();
            GlobalContext.SimpleContainer.PerRequest<WeightViewModel>();
            JsonConfig = new Infrastructure.Config.JsonConfig(FileName);


        }


        public override void Close()
        {
            base.Close();
            JsonConfig?.Dispose();
            GlobalContext.SimpleContainer.UnregisterHandler<WeightViewModel>();
            base.Log.Info("称重关闭");
        }

        /// <summary>
        /// 处理客户端关闭
        /// </summary>
        /// <param name="channelid"></param>
        protected override void OnHandleCommunicationClose(string channelid)
        {
            if (WEIGHT_OPEN.eWeight != null)
            {
                WEIGHT_OPEN.eWeight.ReadWeight -= WEIGHT_OPEN.EWeight_ReadWeight;
                WEIGHT_OPEN.eWeight.Close();
                WEIGHT_OPEN.eWeight.Dispose();
                WEIGHT_OPEN.eWeight = null;
            }
        }
    }
}
