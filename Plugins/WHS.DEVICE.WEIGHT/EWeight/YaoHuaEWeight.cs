using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace WHS.DEVICE.WEIGHT.EWeight
{
    [WeightAttribute(Name = "耀华", Value = "YAOHUA")]
    public sealed class YaoHuaEWeight : IEWeight
    {
        private SerialPort fSerialPort = new SerialPort();
        private bool fInited = false;
        private int fReadSpan = 20;
        private int fSecReadSpan = 20 / 2;
        private bool fIsTryColsePort = false;
        private bool fIsReceiving = false;

        public YaoHuaEWeight()
        {
        }

        #region IEWeighbrige Members

        public void Init(string portName, int baudRate)
        {
            if (string.IsNullOrEmpty(portName))
            {
                throw new ArgumentNullException("portName");
            }
            if (this.fSerialPort.IsOpen)
            {
                throw new InvalidOperationException("端口正在使用中，请先停用。");
            }

            this.fSerialPort.PortName = portName;
            this.fSerialPort.BaudRate = baudRate;
            this.fSerialPort.Parity = System.IO.Ports.Parity.None;
            this.fSerialPort.StopBits = System.IO.Ports.StopBits.One;
            this.fSerialPort.ReceivedBytesThreshold = 20;
            this.fSerialPort.WriteTimeout = 1000;
            this.fSerialPort.ReadTimeout = 1000;

            this.fInited = true;
        }

        void fSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this.fIsTryColsePort)
            {
                return;
            }

            this.fIsReceiving = true;

            var tempStr = "";
            if (this.fSerialPort.IsOpen && this.fSerialPort.BytesToRead > 0)
            {
                //暂停一小会儿，再读取
                Thread.Sleep(this.fReadSpan);

                try
                {
                    tempStr = this.fSerialPort.ReadExisting();
                }
                catch { }

                if (tempStr.LastIndexOf('=') >= 8)
                {
                    this.RaiseReadWeight(tempStr);
                    this.fIsReceiving = false;
                    return;
                }
                else
                {
                    if (this.fIsTryColsePort)
                    {
                        this.fIsReceiving = false;
                        return;
                    }

                    if (this.fSerialPort.IsOpen && this.fSerialPort.BytesToRead > 0)
                    {
                        //再读一次
                        Thread.Sleep(fSecReadSpan);
                        try
                        {
                            tempStr += this.fSerialPort.ReadExisting();
                        }
                        catch { }

                        if (tempStr.LastIndexOf('=') >= 8)
                        {
                            this.RaiseReadWeight(tempStr);
                            this.fIsReceiving = false;
                            return;
                        }
                    }
                }
            }

            this.fIsReceiving = false;
        }

        public bool Open()
        {
            if (!this.fInited)
            {
                throw new InvalidOperationException("请先调用初始化方法。");
            }

            if (!this.fSerialPort.IsOpen)
            {
                this.fSerialPort.Open();
            }

            this.fSerialPort.DataReceived -= new SerialDataReceivedEventHandler(fSerialPort_DataReceived);
            this.fSerialPort.DataReceived += new SerialDataReceivedEventHandler(fSerialPort_DataReceived);

            this.fIsTryColsePort = false;

            return true;
        }

        public bool Close()
        {
            this.fIsTryColsePort = true;
            this.fSerialPort.DataReceived -= new SerialDataReceivedEventHandler(fSerialPort_DataReceived);

            try
            {
                if (this.fSerialPort != null && this.fSerialPort.IsOpen)
                {
                    this.fSerialPort.DiscardInBuffer();
                    this.fSerialPort.DiscardOutBuffer();

                    //while (this.fIsReceiving)
                    //{
                    //    Application.DoEvents();
                    //}

                    this.fSerialPort.Close();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public event EventHandler<WeightEventArgs> ReadWeight;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.fIsTryColsePort = true;
            this.fSerialPort.DataReceived -= new SerialDataReceivedEventHandler(fSerialPort_DataReceived);

            if (this.fSerialPort != null)
            {
                //while (this.fIsReceiving)
                //{
                //    Application.DoEvents();
                //}

                this.fSerialPort.Dispose();
            }
        }

        #endregion

        private void RaiseReadWeight(string weight)
        {
            if (string.IsNullOrEmpty(weight))
            {
                return;
            }

            var temp = this.ReadWeight;
            if (temp != null)
            {
                var result = FormatWeight(weight);
                double oDouble;
                if (double.TryParse(result, out oDouble))
                {
                    temp(this, new WeightEventArgs() { Weight = oDouble });
                }
            }
        }

        private string FormatWeight(string text)
        {
            var last = text.LastIndexOf('=');
            //把=之后的内容截掉
            text = text.Substring(0, last).Trim();
            last = text.LastIndexOf('=');
            if (last > -1)
            {
                text = text.Substring(last + 1);
            }

            var chars = text.ToCharArray();
            var reChars = chars.Reverse();
            return new String(reChars.ToArray());
        }
    }
}
