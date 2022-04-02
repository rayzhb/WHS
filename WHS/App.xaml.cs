using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WHS.Infrastructure.NlogEx;

namespace WHS
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //string path = AppDomain.CurrentDomain.BaseDirectory + "AutoUpdate.exe";
            ////同时启动自动更新程序
            //if (File.Exists(path))
            //{
            //    ProcessStartInfo processStartInfo = new ProcessStartInfo()
            //    {
            //        FileName = "AutoUpdate.exe",
            //        Arguments = "WHS 0"
            //    };
            //    Process proc = Process.Start(processStartInfo);
            //    if (proc != null)
            //    {
            //        proc.WaitForExit();
            //    }
            //}

            NLog.Config.ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("levelx", typeof(LevelExLayoutRenderer));//注册自定义的布局生成器，这样config文件中的levelx才能生效

            NLog.Targets.Target.Register<LimitedMemoryTarget>("LimitedMemory");

            LogUtil.Info("程序开始");
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.IncludeInvariantCulture = false;
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
    }
}
