using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace testcm.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public IScreen ActiveItem { get; set; }
        public ShellViewModel()
        {
            ActiveItem = new HomeViewModel();
        }



    }
}
