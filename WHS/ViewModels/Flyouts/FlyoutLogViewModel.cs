using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WHS.Infrastructure;
using WHS.Infrastructure.NlogEx;

namespace WHS.ViewModels.Flyouts
{
    public class FlyoutLogViewModel : FlyoutBaseViewModel
    {
        private string output;
        public string Output
        {
            get { return output; }
            set { Set(ref output, value); }
        }


        internal LimitedMemoryTarget LogTarget { get; set; }


        public FlyoutLogViewModel()
        {
            this.Name = "Nlog";
            this.Header = LocalizationHelp.GetLocalizedString(this.GetType().Assembly,"LogView.Content.Title");
            this.Position = MahApps.Metro.Controls.Position.Right;
           
        }

        public override void Open()
        {
            if(LogTarget!=null)
                this.Output = LogTarget.StringBuilderLogs.ToString();
        }

        public override void ChangeLanguage()
        {
            this.Header = LocalizationHelp.GetLocalizedString(this.GetType().Assembly, "LogView.Content.Title");
        }
    }
}
