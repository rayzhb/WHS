using Caliburn.Micro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.ViewModels
{
    public abstract class FlyoutBaseViewModel : PropertyChangedBase
    {
        private string header;
        private bool isOpen;
        private Position position;
        private FlyoutTheme theme = FlyoutTheme.Accent;
        private string name;

        public string Name
        {
            get { return this.name; }
            set
            {
                if (value == this.name)
                {
                    return;
                }
                this.name = value;
                this.NotifyOfPropertyChange(() => this.Name);
            }
        }

        public string Header
        {
            get { return this.header; }
            set
            {
                if (value == this.header)
                {
                    return;
                }
                this.header = value;
                this.NotifyOfPropertyChange(() => this.Header);
            }
        }

        public bool IsOpen
        {
            get { return this.isOpen; }
            set
            {
                if (value.Equals(this.isOpen))
                {
                    return;
                }
                this.isOpen = value;
                if(this.isOpen)
                {
                    this.Open();
                }
                else
                {
                    this.Close();
                }
                this.NotifyOfPropertyChange(() => this.IsOpen);
            }
        }

        public Position Position
        {
            get { return this.position; }
            set
            {
                if (value == this.position)
                {
                    return;
                }
                this.position = value;
                this.NotifyOfPropertyChange(() => this.Position);
            }
        }

        public FlyoutTheme Theme
        {
            get { return this.theme; }
            set
            {
                if (value == this.theme)
                {
                    return;
                }
                this.theme = value;
                this.NotifyOfPropertyChange(() => this.Theme);
            }
        }

        public virtual void Open()
        {

        }

        public virtual void Close()
        {

        }

        public abstract void ChangeLanguage();
    }

}
