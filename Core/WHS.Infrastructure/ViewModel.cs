using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WHS.Infrastructure
{
    public class ViewModel : PropertyChangedBase
    {
        protected void RaisePropertyChangedEvent(string propertyName)
        {
            NotifyOfPropertyChange(propertyName);
        }

        public bool IsInDesignMode
        {
            get
            {
                return (bool)DesignerProperties.IsInDesignModeProperty
                            .GetMetadata(typeof(DependencyObject)).DefaultValue;
            }
        }
    }
}
