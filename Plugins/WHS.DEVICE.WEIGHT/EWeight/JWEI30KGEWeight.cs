using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;

namespace WHS.DEVICE.WEIGHT.EWeight
{
    /// <summary>
    /// 钰恒JWE(I)-30KG
    /// 金准家(KG)
    /// 波特率：9600 检验位： None 数据位：8 停止位：1
    /// </summary>
    [WeightAttribute(Name = "耀华XK3190-A27E", Value = "YAOHUAXK3190A27E")]
    [WeightAttribute(Name = "金准家(KG)", Value = "JINZHUNJIA")]
    [WeightAttribute(Name = "钰恒JWE(I)-30KG", Value = "JWEI30")]
    public sealed class JWEI30KGEWeight : IEWeight
    {
        private SerialPort fSerialPort = new SerialPort();
        private bool fInited = false;
        private int fReadSpan = 20;
        private int fSecReadSpan = 20 / 2;
        private bool fIsTryColsePort = false;
        private bool fIsReceiving = false;

        public JWEI30KGEWeight()
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
                //暂停一小会儿，再读取
                Thread.Sleep(this.fReadSpan);

                try
                {
                    tempStr = this.fSerialPort.ReadExisting();
                }
                catch
                {
                }

                if (tempStr.LastIndexOf("kg", StringComparison.OrdinalIgnoreCase) >= 8)
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
                        catch
                        {
                            tempStr = "";
                        }

                        if (tempStr.LastIndexOf("kg", StringComparison.OrdinalIgnoreCase) >= 8)
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
                var result = mc[mc.Count-1].Groups["value"].Value;
                //var result = FormatWeight(weight);
                double oDouble;
                if (double.TryParse(result, out oDouble) && this.fIsReceiving)
                {
                    temp(this, new WeightEventArgs() { Weight = oDouble });
                }
            }
        }

        private string FormatWeight(string text)
        {
            var last = text.LastIndexOf("kg", StringComparison.OrdinalIgnoreCase);
            //把kg之后的内容截掉
            text = text.Substring(0, last).TrimEnd();
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
