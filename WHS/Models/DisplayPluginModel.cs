using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WHS.Infrastructure;

namespace WHS.Models
{
    public class DisplayPluginModel : ViewModel
    {
        public Guid Id { get; set; }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                RaisePropertyChangedEvent("Text");
            }
        }

        public ImageSource Icon { get; set; }

        public Type ViewModel { get; set; }

        public string Badge
        {
            get
            {
                if (EnableHotReload)
                {
                    return LocalizationHelp.GetLocalizedString(this.GetType().Assembly, "MainWindow.Badge");
                }
                else
                {
                    return "";
                }
            }
        }


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
                RaisePropertyChangedEvent("Badge");
            }
        }

        private bool _IsChecked;
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                _IsChecked = value;
                RaisePropertyChangedEvent("IsChecked");
            }
        }

        internal Func<string> ChangeLanguage { get; set; }

        internal void SetBadageChange()
        {
            RaisePropertyChangedEvent("Badge");
        }
    }
}
