using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WHS.Infrastructure;
using WHS.Infrastructure.Events;
using WHS.Models;

namespace WHS.ViewModels.Flyouts
{
    public class FlyoutPluginManageViewModel : FlyoutBaseViewModel
    {

        private IObservableCollection<LoadedPluginModel> _loadedPluginModels;
        public IObservableCollection<LoadedPluginModel> LoadedPluginModels
        {
            get { return _loadedPluginModels; }
            set
            {
                Set(ref _loadedPluginModels, value, "LoadedPluginModels");
            }
        }

        public FlyoutPluginManageViewModel()
        {
            this.Name = "PluginManage";
            this.Header = LocalizationHelp.GetLocalizedString(this.GetType().Assembly, "PluginManage.Content.Title");
            this.Position = MahApps.Metro.Controls.Position.Left;
            LoadedPluginModels = new BindableCollection<LoadedPluginModel>();
        }

        public override void Open()
        {
            base.Open();
            RefreshGrid();
        }

        public void RefreshGrid()
        {
            LoadedPluginModels.Clear();
            var MainViewModel = GlobalContext.SimpleContainer.GetInstance<MainViewModel>();
            foreach (PluginDefinition item in PluginManager.GetPluginDefinitions())
            {
                LoadedPluginModel model = new LoadedPluginModel();
                model.Id = item.Id;
                model.Name = item.Name;
                model.VersionString = item.VersionString;
                model.Manufacturer = item.Manufacturer;
                model.EnableHotReload = item.EnableHotReload;
                if (MainViewModel.DisplayPluginModels.Where(p => p.Id == item.Id).FirstOrDefault() == null)
                {
                    model.IsDisplay = false;
                }
                else
                {
                    model.IsDisplay = true;
                }
                LoadedPluginModels.Add(model);

            }
        }

        public void DeepRefreshGrid()
        {
            List<Task> list_tasks = new List<Task>();
            foreach (var item in LoadedPluginModels)
            {
                list_tasks.Add(GlobalContext.EventAggregator.PublishAsync(new EventHandlePlugin()
                {
                    HandleType = HandleType.Unload,
                    PluginID = item.Id
                }, (action) =>
                {
                    return action();
                }));
            }
            Task.WaitAll(list_tasks.ToArray());
            LoadedPluginModels.Clear();
            PluginManager.DeepReload();
            RefreshGrid();
        }


        public void BtnPlugin(LoadedPluginModel data)
        {
            if (data.IsDisplay)
            {
                Disable(data);
            }
            else
            {
                Enable(data);
            }
        }


        public void Unload(LoadedPluginModel data)
        {
            //发布主界面DELETE的异步事件
            GlobalContext.EventAggregator.PublishAsync(new EventHandlePlugin()
            {
                HandleType = HandleType.Unload,
                PluginID = data.Id
            }, (action) =>
            {
                LoadedPluginModels.Remove(data);

                return action();
            });



        }
        private void Disable(LoadedPluginModel data)
        {
            //发布主界面DELETE的异步事件
            GlobalContext.EventAggregator.PublishAsync(new EventHandlePlugin()
            {
                HandleType = HandleType.Disable,
                PluginID = data.Id
            }, (action) =>
            {

                var loadmodel = LoadedPluginModels.Where(p => p.Id == data.Id).FirstOrDefault();
                if (loadmodel != null)
                {
                    loadmodel.IsDisplay = false;
                }
                LoadedPluginModels.Refresh();
                return action();
            });
        }

        private void Enable(LoadedPluginModel data)
        {
            //发布主界面ADD的异步事件
            GlobalContext.EventAggregator.PublishAsync(new EventHandlePlugin()
            {
                HandleType = HandleType.Enable,
                PluginID = data.Id
            }, (action) =>
            {
                var loadmodel = LoadedPluginModels.Where(p => p.Id == data.Id).FirstOrDefault();
                if (loadmodel != null)
                {
                    loadmodel.IsDisplay = true;
                }
                LoadedPluginModels.Refresh();
                return action();
            });

        }

        public override void ChangeLanguage()
        {
            this.Header = LocalizationHelp.GetLocalizedString(this.GetType().Assembly, "PluginManage.Content.Title");
            LoadedPluginModels.Refresh();
        }
    }
}
