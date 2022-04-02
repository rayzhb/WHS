using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WHS.DEVICE.MAPDESIGN.Models
{
    public class UndoModel
    {
        public string Opreator
        {
            get; set;
        }

        public FrameworkElement Obj
        {
            get; set;
        }

        public UndoModel(string op, FrameworkElement obj)
        {
            Opreator = op;
            Obj = obj;

        }
    }
}
