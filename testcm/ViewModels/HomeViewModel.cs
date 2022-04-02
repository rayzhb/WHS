using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;

namespace testcm.ViewModels
{
    public class HomeViewModel : Screen
    {
        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyOfPropertyChange("Title");
            }
        }

        public HomeViewModel()
        {
            Title = "Welcome to Caliburn.Micro";
        }
    }
}
