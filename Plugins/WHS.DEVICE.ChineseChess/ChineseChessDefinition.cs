using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WHS.DEVICE.ChineseChess.ViewModels;
using WHS.Infrastructure;

namespace WHS.DEVICE.ChineseChess
{ 
    public class ChineseChessDefinition : PluginDefinition
    {
        public override Guid Id
        {
            get
            {
                return new Guid("D207F870-C68B-4BCD-A4DD-4D635B8AD3B4");
            }
        }

        public override string Name
        {
            get
            {
                return "象棋";
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
                return ChineseChess.Properties.Resource.ChineseChess;
            }
        }
        public override Type ViewModel
        {
            get
            {
                return typeof(ChineseChessViewModel);
            }
        }

        internal static Infrastructure.Config.JsonConfig JsonConfig { get; set; }

        internal static string s_filename { get; set; }
        public override void Init()
        {
            base.Log.Info("象棋后台初始化");
            base.Init();
            GlobalContext.SimpleContainer.PerRequest<ChineseChessViewModel>();
           s_filename = FileName;
        }


        public override void Close()
        {
            base.Close();
            GlobalContext.SimpleContainer.UnregisterHandler<ChineseChessViewModel>();
            base.Log.Info("象棋后台关闭");
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
