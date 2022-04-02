using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHS.DEVICE.MAPDESIGN.Controls;

namespace WHS.DEVICE.MAPDESIGN.Models
{
    public class EliipseLineModel
    {
        public CustomLine UpLine
        {
            get; set;
        }

        public CustomLine LeftLine
        {
            get; set;
        }

        public CustomLine RightLine
        {
            get; set;
        }

        public CustomLine DownLine
        {
            get; set;
        }

    }
}
