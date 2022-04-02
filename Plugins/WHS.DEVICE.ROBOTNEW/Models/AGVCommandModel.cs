using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.DEVICE.ROBOTNEW.Models
{
    public class AGVCommandModel
    {
        public AGVCommand command { get; set; }

        public int? row { get; set; }

        public int? column { get; set; }

        public int? floor { get; set; }

        public string code { get; set; }
    }
}
