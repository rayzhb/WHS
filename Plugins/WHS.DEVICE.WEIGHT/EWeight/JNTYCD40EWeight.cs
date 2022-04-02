using System;
using System.IO.Ports;

namespace WHS.DEVICE.WEIGHT.EWeight
{
    /// <summary>
    /// 金华南天:YCD-40邮资秤
    /// 波特率：2400 检验位： None 数据位：8 停止位：1
    /// </summary>
    [WeightAttribute(Name = "金南天YCD40", Value = "JNTYCD40")]
    public sealed class JNTYCD40EWeight : IEWeight
    {
        private SerialPort fSerialPort = new SerialPort();
        private bool fInited = false;
        private int fReadSpan = 20;        
        private bool fIsTryColsePort = false;
        private bool fIsReceiving = false;
        private System.Threading.Timer fTTimer;

        public JNTYCD40EWeight()
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
            this.fSerialPort.ReceivedBytesThreshold = 1; //有数据来就触发事件
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
                catch
                {

                }
                if (tempStr.Length >= 5)
                {
                    this.RaiseReadWeight(tempStr);
                    this.fIsReceiving = false;
                    return;
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

            this.fTTimer = new System.Threading.Timer(TimerAtion);
            this.fTTimer.Change(0, this.fReadSpan);//启动

            return true;
        }

        private void TimerAtion(object state)
        {
            if (this.fIsTryColsePort)
            {
                return;
            }
            this.fSerialPort.Write("a");//秤的说明书里说要发个a数据
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
            if (string.IsNullOrEmpty(weight) || weight.Length < 5)
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
                    temp(this, new WeightEventArgs() { Weight = oDouble });
                }
            }
        }

        private string FormatWeight(string text)
        {
            return text.Substring(0, 5).Insert(2, ".");
        }
    }
}
