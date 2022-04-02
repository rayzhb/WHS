using System;
using System.IO.Ports;

namespace WHS.DEVICE.WEIGHT.EWeight
{
    /// <summary>
    /// FWE衡之宝:FH1 TC/W
    /// 波特率：9600 检验位： None 数据位：8 停止位：1
    /// </summary>
    [WeightAttribute(Name = "FWE衡之宝：FH1", Value = "FWEFHITC-W")]
    public sealed class FWEEWeight : IEWeight
    {
        private SerialPort fSerialPort = new SerialPort();
        private bool fInited = false;
        private int fReadSpan = 50;
        private bool fIsTryColsePort = false;
        private bool fIsReceiving = false;
        private System.Threading.Timer fTTimer;

        public FWEEWeight()
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
            this.fSerialPort.DataBits = 8;
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
                try
                {
                    tempStr = this.fSerialPort.ReadExisting();
                }
                catch { }

                this.RaiseReadWeight(tempStr);
                this.fIsReceiving = false;
                return;
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

            this.fTTimer = new System.Threading.Timer(TimerAtion);
            this.fTTimer.Change(0, this.fReadSpan);//启动

            //初次发送 49410A （称的要求）
            var bytes = new byte[3] { 0x49, 0x41, 0x0A };
            this.fSerialPort.Write(bytes, 0, bytes.Length);

            return true;
        }

        private void TimerAtion(object state)
        {
            if (this.fIsTryColsePort)
            {
                return;
            }
            this.fSerialPort.Write("\r\n");  //回车
        }

        public bool Close()
        {
            this.fIsTryColsePort = true;
            if (this.fTTimer != null)
            {
                this.fTTimer.Dispose();
            }
            this.fSerialPort.DataReceived -= new SerialDataReceivedEventHandler(fSerialPort_DataReceived);

            if (this.fSerialPort != null && this.fSerialPort.IsOpen)
            {
                try
                {
                    this.fSerialPort.DiscardInBuffer();
                    this.fSerialPort.DiscardOutBuffer();

                    //while (this.fIsReceiving)
                    //{
                    //    Application.DoEvents();
                    //}
                    this.fSerialPort.Close();
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
                if (double.TryParse(result, out oDouble) && this.fIsReceiving)
                {
                    if (oDouble > 0)
                    {
                        temp(this, new WeightEventArgs() { Weight = oDouble });
                    }
                }
            }
        }

        private string FormatWeight(string text)
        {
            var last = this.GetLastOfDegit(text);
            if (last == -1)
            {
                return text.Trim();
            }

            text = text.Substring(0, last + 1).TrimEnd();
            last = this.GetLastOfNoDegitNoPoint(text);
            if (last > -1)
            {
                return text.Substring(last + 1);
            }
            else
            {
                return text.Trim();
            }
        }

        //倒数查找第一个是数字的位置
        //未找到=-1
        private int GetLastOfDegit(string text)
        {
            var count = text.Length;
            for (var index = count - 1; index >= 0; index--)
            {
                if (!Char.IsDigit(text, index))
                {
                    continue;
                }

                return index;
            }
            return -1;
        }

        //倒数查找第一个非数字/非.字符位置
        //未找到=-1
        private int GetLastOfNoDegitNoPoint(string text)
        {
            var count = text.Length;
            for (var index = count - 1; index >= 0; index--)
            {
                if (Char.IsDigit(text, index) ||
                    text[index] == '.')
                {
                    continue;
                }

                return index;
            }
            return -1;
        }
    }
}
