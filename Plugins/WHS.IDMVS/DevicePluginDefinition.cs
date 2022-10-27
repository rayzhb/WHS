using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WHS.IDMVS.SDK;
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
        private IntPtr _deviceHandle = IntPtr.Zero;

        public override void Init()
        {
            base.Init();
            GlobalContext.SimpleContainer.Singleton<ViewModels.DeviceViewModel>();
            JsonConfig = new JsonConfig(FileName);
            var dir = Directory.GetParent(FileName);
            var ProcessArchitecture = GlobalContext.OSArchitecture;
            string dictory = System.IO.Path.GetDirectoryName(dir + @"\Libs\" + ProcessArchitecture.ToString().ToLower()+@"\");
            List<string> depends= new List<string>();
            depends.Add(dictory+"\\CPythonExtend.dll");
            depends.Add(dictory+"\\python27.dll");
            depends.Add(dictory+"\\RulerFilter.dll");
            _deviceHandle =WindowsLoadLibrary.Instance.LoadLibrary(dictory+"\\MVIDCodeReader.dll", depends);

        }


        public override void Close()
        {
            base.Close();
            JsonConfig?.Dispose();
            GlobalContext.SimpleContainer.UnregisterHandler<ViewModels.DeviceViewModel>();
            if (_deviceHandle!= IntPtr.Zero)
            {
                WindowsLoadLibrary.Instance.UnLoadLibrary(_deviceHandle);
            }
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
