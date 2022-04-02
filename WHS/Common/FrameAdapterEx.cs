using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WHS.Common
{
    public class FrameAdapterEx : INavigationService
    {
        private readonly Frame frame;

        private readonly bool treatViewAsLoaded;

        /// <summary>
        ///   The <see cref="T:System.Uri" /> source.
        /// </summary>
        public Uri Source
        {
            get
            {
                return frame.Source;
            }
            set
            {
                frame.Source = value;
            }
        }

        /// <summary>
        ///   Indicates whether the navigator can navigate back.
        /// </summary>
        public bool CanGoBack => frame.CanGoBack;

        /// <summary>
        ///   Indicates whether the navigator can navigate forward.
        /// </summary>
        public bool CanGoForward => frame.CanGoForward;

        /// <summary>
        ///   The current <see cref="T:System.Uri" /> source.
        /// </summary>
        public Uri CurrentSource => frame.CurrentSource;

        /// <summary>
        ///   The current content.
        /// </summary>
        public object CurrentContent => frame.Content;

        private event NavigatingCancelEventHandler ExternalNavigatingHandler = delegate
        {
        };

        /// <summary>
        ///   Raised after navigation.
        /// </summary>
        public event NavigatedEventHandler Navigated
        {
            add
            {
                frame.Navigated += value;
            }
            remove
            {
                frame.Navigated -= value;
            }
        }

        /// <summary>
        ///   Raised prior to navigation.
        /// </summary>
        public event NavigatingCancelEventHandler Navigating
        {
            add
            {
                ExternalNavigatingHandler += value;
            }
            remove
            {
                ExternalNavigatingHandler -= value;
            }
        }

        /// <summary>
        ///   Raised when navigation fails.
        /// </summary>
        public event NavigationFailedEventHandler NavigationFailed
        {
            add
            {
                frame.NavigationFailed += value;
            }
            remove
            {
                frame.NavigationFailed -= value;
            }
        }

        /// <summary>
        ///   Raised when navigation is stopped.
        /// </summary>
        public event NavigationStoppedEventHandler NavigationStopped
        {
            add
            {
                frame.NavigationStopped += value;
            }
            remove
            {
                frame.NavigationStopped -= value;
            }
        }

        /// <summary>
        ///   Raised when a fragment navigation occurs.
        /// </summary>
        public event FragmentNavigationEventHandler FragmentNavigation
        {
            add
            {
                frame.FragmentNavigation += value;
            }
            remove
            {
                frame.FragmentNavigation -= value;
            }
        }

        /// <summary>
        ///   Creates an instance of <see cref="T:Caliburn.Micro.FrameAdapter" />
        /// </summary>
        /// <param name="frame"> The frame to represent as a <see cref="T:Caliburn.Micro.INavigationService" /> . </param>
        /// <param name="treatViewAsLoaded"> Tells the frame adapter to assume that the view has already been loaded by the time OnNavigated is called. This is necessary when using the TransitionFrame. </param>
        public FrameAdapterEx(Frame frame, bool treatViewAsLoaded = false)
        {
            this.frame = frame;
            this.treatViewAsLoaded = treatViewAsLoaded;
            this.frame.Navigating += OnNavigating;
            this.frame.Navigated += OnNavigated;
        }

        /// <summary>
        ///   Occurs before navigation
        /// </summary>
        /// <param name="sender"> The event sender. </param>
        /// <param name="e"> The event args. </param>
        protected virtual void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            this.ExternalNavigatingHandler(sender, e);
            if (!e.Cancel)
            {
                FrameworkElement frameworkElement = frame.Content as FrameworkElement;
                if (frameworkElement != null)
                {
                    IGuardClose val = frameworkElement.DataContext as IGuardClose;
                    if (val != null && !e.Uri.IsAbsoluteUri)
                    {
                        bool shouldCancel = false;
                        val.CanCloseAsync(new System.Threading.CancellationToken()).Wait();
                        if (shouldCancel)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    IDeactivate val2 = frameworkElement.DataContext as IDeactivate;
                    if (val2 != null && frame.CurrentSource != e.Uri)
                    {
                        val2.DeactivateAsync(CanCloseOnNavigating(sender, e)).Wait();
                    }
                }
            }
        }

        /// <summary>
        /// Called to check whether or not to close current instance on navigating.
        /// </summary>
        /// <param name="sender"> The event sender from OnNavigating event. </param>
        /// <param name="e"> The event args from OnNavigating event. </param>
        protected virtual bool CanCloseOnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            return false;
        }

        /// <summary>
        ///   Occurs after navigation
        /// </summary>
        /// <param name="sender"> The event sender. </param>
        /// <param name="e"> The event args. </param>
        protected virtual void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (!e.Uri.IsAbsoluteUri && e.Content != null)
            {
                ViewLocator.InitializeComponent(e.Content);
                object obj = ViewModelLocator.LocateForView(e.Content);
                if (obj != null)
                {
                    Page page = e.Content as Page;
                    if (page == null)
                    {
                        throw new ArgumentException("View '" + e.Content.GetType().FullName + "' should inherit from Page or one of its descendents.");
                    }
                    if (treatViewAsLoaded)
                    {
                        page.SetValue(View.IsLoadedProperty, true);
                    }
                    TryInjectParameters(obj, e.ExtraData);
                    ViewModelBinder.Bind(obj, page, null);
                    IActivate val = obj as IActivate;
                    if (val != null)
                    {
                        val.ActivateAsync().Wait();
                    }
                    GC.Collect();
                }
            }
        }

        /// <summary>
        ///   Attempts to inject query string parameters from the view into the view model.
        /// </summary>
        /// <param name="viewModel"> The view model.</param>
        /// <param name="parameter"> The parameter.</param>
        protected virtual void TryInjectParameters(object viewModel, object parameter)
        {
            Type type = viewModel.GetType();
            IDictionary<string, object> dictionary = parameter as IDictionary<string, object>;
            if (dictionary != null)
            {
                foreach (KeyValuePair<string, object> item in dictionary)
                {
                    PropertyInfo propertyCaseInsensitive = type.GetPropertyCaseInsensitive(item.Key);
                    if (!(propertyCaseInsensitive == (PropertyInfo)null))
                    {
                        propertyCaseInsensitive.SetValue(viewModel, MessageBinder.CoerceValue(propertyCaseInsensitive.PropertyType, item.Value, null), null);
                    }
                }
            }
            else
            {
                PropertyInfo propertyCaseInsensitive2 = type.GetPropertyCaseInsensitive("Parameter");
                if (!(propertyCaseInsensitive2 == (PropertyInfo)null))
                {
                    propertyCaseInsensitive2.SetValue(viewModel, MessageBinder.CoerceValue(propertyCaseInsensitive2.PropertyType, parameter, null), null);
                }
            }
        }

        /// <summary>
        ///   Stops the loading process.
        /// </summary>
        public void StopLoading()
        {
            frame.StopLoading();
        }

        /// <summary>
        ///   Navigates back.
        /// </summary>
        public void GoBack()
        {
            frame.GoBack();
        }

        /// <summary>
        ///   Navigates forward.
        /// </summary>
        public void GoForward()
        {
            frame.GoForward();
        }

        /// <inheritdoc />
        public void NavigateToViewModel(Type viewModel, object extraData = null)
        {
            Type arg = ViewLocator.LocateTypeForModelType(viewModel, null, null);
            Uri source = new Uri(ViewLocator.DeterminePackUriFromType(viewModel, arg), UriKind.Relative);
            frame.Navigate(source, extraData);
        }

        /// <inheritdoc />
        public void NavigateToViewModel<TViewModel>(object extraData = null)
        {
            NavigateToViewModel(typeof(TViewModel), extraData);
        }

        /// <summary>
        ///   Removes the most recent entry from the back stack.
        /// </summary>
        /// <returns> The entry that was removed. </returns>
        public JournalEntry RemoveBackEntry()
        {
            return frame.RemoveBackEntry();
        }
    }
}
