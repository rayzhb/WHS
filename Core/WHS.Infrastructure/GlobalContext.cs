using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure
{
    public static class GlobalContext
    {
        public static IWindowManager WindowManager { get; set; }
        public static SimpleContainer SimpleContainer { get; set; }
        public static INavigationService NavigationService { get; set; }
        public static IEventAggregator EventAggregator { get; set; }
        public static Architecture OSArchitecture { get; set; }
    }
    
}
