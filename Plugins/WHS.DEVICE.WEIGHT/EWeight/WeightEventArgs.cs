using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WHS.DEVICE.WEIGHT.EWeight
{
    public class WeightEventArgs : EventArgs
    {
        public WeightEventArgs()
        {
        }

        public WeightEventArgs(double wight)
            : this()
        {
            this.Weight = Weight;
        }

        public double Weight { get; set; }
    }
}
