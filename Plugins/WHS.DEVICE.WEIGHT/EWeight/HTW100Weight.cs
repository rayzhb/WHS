using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WHS.DEVICE.WEIGHT.SerialPortLib;

namespace WHS.DEVICE.WEIGHT.EWeight
{


    [WeightAttribute(Name = "坤宏HTW-100(HTW-G)", Value = "HTW100")]
    public sealed class HTW100Weight : IEWeight
    {
        private SerialPortInput fSerialPort = new SerialPortInput();
        private bool fInited = false;
        private int fReadSpan = 20;
        private int fSecReadSpan = 20 / 2;
        private bool fIsTryClosePort = false;
        private bool fIsReceiving = false;

        public HTW100Weight()
        {
        }

        #region IEWeighbrige Members

        public void Init(string portName, int baudRate)
        {
            if (string.IsNullOrEmpty(portName))
            {
                throw new ArgumentNullException("portName");
            }
            if (this.fSerialPort.IsConnected)
            {
                throw new InvalidOperationException("端口正在使用中，请先停用。");
            }
            this.fSerialPort.SetPort(portName, baudRate, System.IO.Ports.StopBits.One, System.IO.Ports.Parity.None, DataBits.Eight);

            this.fInited = true;
        }

        public bool Open()
        {
            if (!this.fInited)
            {
                throw new InvalidOperationException("请先调用初始化方法。");
            }

            if (!this.fSerialPort.IsConnected)
            {
                this.fSerialPort.Connect();
            }
            this.fSerialPort.MessageReceived -= FSerialPort_MessageReceived;
            this.fSerialPort.MessageReceived += FSerialPort_MessageReceived;

            this.fIsTryClosePort = false;

            return true;
        }

        private void FSerialPort_MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            if (this.fIsTryClosePort)
            {
                return;
            }

            this.fIsReceiving = true;

            if (this.fSerialPort.IsConnected )
            {
                string strdata = Encoding.ASCII.GetString(args.Data);

                if (strdata.LastIndexOf("kg", StringComparison.OrdinalIgnoreCase) >= 8)
                {
                    this.RaiseReadWeight(strdata);
                    this.fIsReceiving = false;
                    return;
                }
                else
                {
                    if (this.fIsTryClosePort)
                    {
                        this.fIsReceiving = false;
                        return;
                    }

                }

                System.Diagnostics.Debug.WriteLine(strdata);
            }

            this.fIsReceiving = false;
        }

        private void RaiseReadWeight(string weight)
        {
            //Console.WriteLine(weight);

            if (string.IsNullOrEmpty(weight))// || weight.Length < 9)
            {
                return;
            }

            var temp = this.ReadWeight;
            if (temp != null)
            {
                Regex regexObj = new Regex(@"(?<value>[0-9]*\.?[0-9]+\b)\s*kg", RegexOptions.Multiline);
                var mc = regexObj.Matches(weight);
                var result = mc[mc.Count - 1].Groups["value"].Value;
                //var result = FormatWeight(weight);
                if (double.TryParse(result, out double oDouble) && this.fIsReceiving)
                {
                    temp(this, new WeightEventArgs() { Weight = oDouble });
                }
            }
        }

        public bool Close()
        {
            this.fIsTryClosePort = true;
            this.fSerialPort.MessageReceived -= FSerialPort_MessageReceived;

            if (this.fSerialPort != null && this.fSerialPort.IsConnected)
            {
                try
                {
                    //while (this.fIsReceiving)
                    //{
                    //    Application.DoEvents();
                    //}
                    this.fSerialPort.Disconnect();
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public event EventHandler<WeightEventArgs> ReadWeight;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.fIsTryClosePort = true;
            this.fSerialPort.MessageReceived -= FSerialPort_MessageReceived;

            if (this.fSerialPort != null)
            {
                //while (this.fIsReceiving)
                //{
                //    Application.DoEvents();
                //}
                this.fSerialPort.Disconnect();
            }
        }

        #endregion
    }
}
