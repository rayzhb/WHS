using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WHS.Infrastructure;
using WHS.Infrastructure.NlogEx;
using WHS.ViewModels;

namespace WHS
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer container;

        public Bootstrapper()
        {
            Initialize();
            GlobalContext.OSArchitecture = RuntimeInformation.ProcessArchitecture;
            WHS.Infrastructure.HardwareID.Value();
        }

        protected override void Configure()
        {
            container = new SimpleContainer();

            container.Instance(container);

            container
        .Singleton<IWindowManager, WindowManager>()
        .Singleton<IEventAggregator, EventAggregator>();

            //设置VIEW与 VIEWMODEL的映射关系，使用默认映射
            var typeMappingConfiguration = new TypeMappingConfiguration();

            ViewLocator.ConfigureTypeMappings(typeMappingConfiguration);

            container.Singleton<MainViewModel>();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewFor<MainViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            MessageBox.Show(e.Exception.Message + "" + e.Exception.InnerException?.Message, "An error as occurred", MessageBoxButton.OK);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
            Environment.Exit(0);
        }
    }
}
