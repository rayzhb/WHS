using Caliburn.Micro;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WHS.Common;
using WHS.Infrastructure;
using WHS.Models;
using WHS.ViewModels.Flyouts;
using ControlzEx.Theming;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WHS.Infrastructure.Events;

namespace WHS.ViewModels
{
    public class MainViewModel : Caliburn.Micro.Screen, IHandle<EventHandlePlugin>
    {
        private Server.WebSocketsServer _webSocketsServer;
        public IObservableCollection<DisplayPluginModel> DisplayPluginModels { get; }
        private readonly IWindowManager _windowManager;
        private readonly SimpleContainer _container;
        private INavigationService _navigationService;
        private DisplayPluginModel _currentModel;
        public List<AccentColorMenuData> AccentColors { get; set; }
        public List<AppThemeMenuData> AppThemes { get; set; }
        public IObservableCollection<FlyoutBaseViewModel> FlyoutViewModels { get; }
        private string _Footer;

        public string Footer
        {
            get
            {
                return _Footer;
            }
            set
            {
                Set(ref _Footer, value);
            }
        }

        private static NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();
        private IEventAggregator _eventAggregator;

        public MainViewModel(IWindowManager windowManager, SimpleContainer container, IEventAggregator eventAggregator)
        {
            GlobalContext.WindowManager = _windowManager = windowManager;
            GlobalContext.SimpleContainer = _container = container;
            GlobalContext.EventAggregator = _eventAggregator = eventAggregator;
            _webSocketsServer = new Server.WebSocketsServer();
            DisplayPluginModels = new BindableCollection<DisplayPluginModel>();
            FlyoutViewModels = new BindableCollection<FlyoutBaseViewModel>();
            // create accent color menu items for the demo
            this.AccentColors = ThemeManager.Current.Themes
                                            .GroupBy(x => x.ColorScheme)
                                            .OrderBy(a => a.Key)
                                            .Select(a => new AccentColorMenuData { Name = a.Key, ColorBrush = a.First().ShowcaseBrush })
                                            .ToList();

            // create metro theme color menu items for the demo
            this.AppThemes = ThemeManager.Current.Themes
                                         .GroupBy(x => x.BaseColorScheme)
                                         .Select(x => x.First())
                                         .Select(a => new AppThemeMenuData() { Name = a.BaseColorScheme, BorderColorBrush = a.Resources["MahApps.Brushes.ThemeForeground"] as Brush, ColorBrush = a.Resources["MahApps.Brushes.ThemeBackground"] as Brush })
                                         .ToList();
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.PropertyChanged += Instance_PropertyChanged1;

            Footer = LocalizationHelp.GetLocalizedString(this.GetType().Assembly, "MainWindow.Footer") + " - " + RuntimeInformation.ProcessArchitecture.ToString();

            GlobalContext.EventAggregator.SubscribeOnPublishedThread(this);
        }

        private void Instance_PropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Culture")
            {
                foreach (var item in DisplayPluginModels)
                {
                    if(item.EnableHotReload)
                    {
                        item.SetBadageChange();
                    }
                    item.Text = item.ChangeLanguage.Invoke();
                }
                foreach (var item in this.FlyoutViewModels)
                {
                    item.ChangeLanguage();
                }
            }
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            base.OnInitializeAsync(cancellationToken);
            this._container.PerRequest<AboutViewModel>();

            this.FlyoutViewModels.Add(new FlyoutLogViewModel());

            this.FlyoutViewModels.Add(new FlyoutPluginManageViewModel());

            this.FlyoutViewModels.Add(new FlyoutWSConnectViewModel());

       
            var AboutPlugin = new DisplayPluginModel()
            {
                Id = new Guid("6F8F3990-8DAE-4875-BE59-386C21846E2C"),
                Text = LocalizationHelp.GetLocalizedString(this.GetType().Assembly, "AboutView.Title"),
                Icon = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.About.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()),
                ViewModel = typeof(AboutViewModel),
                ChangeLanguage = () =>
                {
                    return LocalizationHelp.GetLocalizedString(this.GetType().Assembly, "AboutView.Title");
                }
            };
            DisplayPluginModels.Add(AboutPlugin);


            PluginManager.Initialize();

            return _webSocketsServer.RunServerAsync();
        }



        private System.Windows.Visibility _LoadingVisibility = System.Windows.Visibility.Hidden;
        public System.Windows.Visibility LoadingVisibility
        {
            get
            {
                return _LoadingVisibility;
            }
            set
            {
                Set(ref _LoadingVisibility, value);
            }
        }

        public void RegisterFrame(Frame frame)
        {
            GlobalContext.NavigationService = _navigationService = new Common.FrameAdapterEx(frame);
            _navigationService.NavigationFailed += _navigationService_NavigationFailed;
            _navigationService.Navigated += _navigationService_Navigated;
            _navigationService.NavigationStopped += _navigationService_NavigationStopped;
            //注册INavigationService
            _container.Instance(_navigationService);

            //默认第一个插件选中
            if (DisplayPluginModels.Count > 0)
            {
                var item = DisplayPluginModels[0];
                _currentModel = item;
                item.IsChecked = true;
                _navigationService.NavigateToViewModel(item.ViewModel);
            }
        }

        private void _navigationService_NavigationStopped(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            LoadingVisibility = System.Windows.Visibility.Hidden;
        }

        private void _navigationService_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            LoadingVisibility = System.Windows.Visibility.Hidden;
        }

        private void _navigationService_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                LoadingVisibility = System.Windows.Visibility.Hidden;
            });
        }


        public void ShowPage(DisplayPluginModel model)
        {
            LoadingVisibility = System.Windows.Visibility.Visible;
            if (_currentModel != null)
                _currentModel.IsChecked = false;
            model.IsChecked = true;
            _currentModel = model;
            Task.Run(() =>
            {
                Thread.Sleep(100);
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    _navigationService.NavigateToViewModel(model.ViewModel);
                });
            });

        }

        public void ToggleFlyout(string name)
        {
            var flyout = this.FlyoutViewModels.Where(p => p.Name == name).FirstOrDefault();
            if (flyout != null)
                flyout.IsOpen = !flyout.IsOpen;
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            var task = base.OnDeactivateAsync(close, cancellationToken);

            foreach (PluginDefinition pluginManagerModel in PluginManager.GetPluginDefinitions())
            {
                try
                {
                    pluginManagerModel.Close();
                }
                catch (Exception)
                {
                }
            }
            return _webSocketsServer.StopServerAsync();
        }

        /// <summary>
        /// 处理界面差价C D R异步消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task HandleAsync(EventHandlePlugin message, CancellationToken cancellationToken)
        {
            var plugin = PluginManager.GetPluginDefinition(message.PluginID);
            if (plugin == null)
                return Task.FromResult(0);
            switch (message.HandleType)
            {
                case HandleType.Add:
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        plugin.Init();

                        DisplayPluginModels.Insert(0, new DisplayPluginModel()
                        {
                            Id = plugin.Id,
                            Text = plugin.GetLocalization(),
                            EnableHotReload = plugin.EnableHotReload,
                            Icon = Imaging.CreateBitmapSourceFromHBitmap(plugin.Icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()),
                            ViewModel = plugin.ViewModel,
                            ChangeLanguage = () =>
                            {
                                return plugin.GetLocalization();
                            }
                        }); ;
                    });
                    break;
                case HandleType.Reload:
                    Task.Run(() =>
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        var dpm_reload = DisplayPluginModels.Where(w => w.Id == plugin.Id).FirstOrDefault();
                        if (dpm_reload != null)
                        {
                            DisplayPluginModels.Remove(dpm_reload);
                        }
                        _currentModel = DisplayPluginModels[0];
                        DisplayPluginModels[0].IsChecked = true;

                        _navigationService.NavigateToViewModel(_currentModel.ViewModel);



                    })).ContinueWith((t) =>
                    {
                        t.Wait();
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            plugin.Init();
                            DisplayPluginModels.Insert(0, new DisplayPluginModel()
                            {
                                Id = plugin.Id,
                                Text = plugin.GetLocalization(),
                                EnableHotReload = plugin.EnableHotReload,
                                Icon = Imaging.CreateBitmapSourceFromHBitmap(plugin.Icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()),
                                ViewModel = plugin.ViewModel,
                                ChangeLanguage = () =>
                                {
                                    return plugin.GetLocalization();
                                }
                            });
                            foreach (var item in DisplayPluginModels)
                            {
                                item.IsChecked = false;
                            }
                            _currentModel = DisplayPluginModels[0];
                            DisplayPluginModels[0].IsChecked = true;

                            _navigationService.NavigateToViewModel(_currentModel.ViewModel);
                        });
                    });
                    break;
                case HandleType.Unload:
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        plugin.Close();
                        var dpm = DisplayPluginModels.Where(w => w.Id == plugin.Id).FirstOrDefault();
                        if (dpm != null)
                        {
                            DisplayPluginModels.Remove(dpm);
                        }
                        foreach (var item in DisplayPluginModels)
                        {
                            item.IsChecked = false;
                        }
                        _currentModel = DisplayPluginModels[0];
                        DisplayPluginModels[0].IsChecked = true;
                        PluginManager.UnloadPluginDefinition(plugin);
                        _navigationService.NavigateToViewModel(_currentModel.ViewModel);
                    });
                    break;
                case HandleType.Disable:
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        plugin.Close();
                        var dpm = DisplayPluginModels.Where(w => w.Id == plugin.Id).FirstOrDefault();
                        if (dpm != null)
                        {
                            DisplayPluginModels.Remove(dpm);
                        }
                        foreach (var item in DisplayPluginModels)
                        {
                            item.IsChecked = false;
                        }
                        _currentModel = DisplayPluginModels[0];
                        DisplayPluginModels[0].IsChecked = true;
                        _navigationService.NavigateToViewModel(_currentModel.ViewModel);
                    });
                    break;
                case HandleType.Enable:
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        plugin.Init();
                        DisplayPluginModels.Insert(0, new DisplayPluginModel()
                        {
                            Id = plugin.Id,
                            Text = plugin.GetLocalization(),
                            EnableHotReload = plugin.EnableHotReload,
                            Icon = Imaging.CreateBitmapSourceFromHBitmap(plugin.Icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()),
                            ViewModel = plugin.ViewModel,
                            ChangeLanguage = () =>
                            {
                                return plugin.GetLocalization();
                            }
                        });
                    });
                        break;
                default:
                    break;
            }

            return Task.FromResult(0);
        }
    }
}
