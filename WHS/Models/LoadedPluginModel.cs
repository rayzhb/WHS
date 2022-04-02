using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WHS.Infrastructure;

namespace WHS.Models
{
    public class LoadedPluginModel : ViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string VersionString { get; set; }
        public string Manufacturer { get; set; }

        private bool _EnableHotReload;
        public bool EnableHotReload
        {
            get
            {
                return _EnableHotReload;
            }
            set
            {
                _EnableHotReload = value;
                RaisePropertyChangedEvent("EnableHotReload");
                RaisePropertyChangedEvent("UnloadBtnVisibility");
            }
        }

        private bool _IsDisplay;
        public bool IsDisplay
        {
            get
            {
                return _IsDisplay;
            }
            set
            {
                _IsDisplay = value;
                RaisePropertyChangedEvent("IsDisplay");
                RaisePropertyChangedEvent("BtnContent");
            }
        }
        public Visibility UnloadBtnVisibility
        {
            get
            {
                if (EnableHotReload)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
        }


        public string BtnContent
        {
            get
            {
                if (IsDisplay)
                {
                    return LocalizationHelp.GetLocalizedString(this.GetType().Assembly, "FlyoutPluginManageView.Datagrid.Enable");
                }
                else
                {
                    return LocalizationHelp.GetLocalizedString(this.GetType().Assembly, "FlyoutPluginManageView.Datagrid.Disable");
                }
            }
        }

    }
}
