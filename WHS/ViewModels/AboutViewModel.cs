using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WHS.Infrastructure;
using WHS.Infrastructure.Events;

namespace WHS.ViewModels
{
    public class AboutViewModel : Caliburn.Micro.Screen
    {
        private readonly IWindowManager _windowManager;
        private readonly SimpleContainer _container;
        private IEventAggregator _eventAggregator;

        private string _HardwareID;

        public string HardwareID
        {
            get
            {
                return _HardwareID;
            }
            set
            {
                Set(ref _HardwareID, value);
            }
        }

        public AboutViewModel(IWindowManager windowManager, SimpleContainer container, IEventAggregator eventAggregator)
        {
            _windowManager = windowManager;
            _container = container;
            _eventAggregator = eventAggregator;
            _HardwareID = WHS.Infrastructure.HardwareID.Value();
        }


        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            return base.OnInitializeAsync(cancellationToken);
        }

        public void Copy()
        {
            Clipboard.SetText(HardwareID);
            EventMessageDialog eventMessageDialog = new EventMessageDialog();
            eventMessageDialog.Title = "信息";
            eventMessageDialog.Message = LocalizationHelp.GetLocalizedString(this.GetType().Assembly, "AboutView.CopyMessage");
            eventMessageDialog.MessageDialogStyle = MessageDialogStyle.Affirmative;
            GlobalContext.EventAggregator.PublishAsync(new EventMessage()
            {
                EventDefinition = EventDefinition.弹出框,
                EventData = eventMessageDialog
            },
                (action) => { return action(); });
        }
    }

}
