using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.DEVICE.SIGNATURE.ViewModels
{
    public class DialogViewModel: Screen
    {
        public string Title { get; }

        public string Message { get; }

        public DialogViewModel(string title, string message)
        {
            Title = title;
            Message = message;
        }

    }
}
