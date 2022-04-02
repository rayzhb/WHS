using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WHS.DEVICE.WEIGHT.EWeight
{
    public interface IEWeight : IDisposable
    {
        void Init(string portName, int baudRate);

        bool Open();

        bool Close();

        event EventHandler<WeightEventArgs> ReadWeight;
    }
}
