using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHS.DEVICE.WEIGHT;
using WHS.DEVICE.WEIGHT.EWeight;
using WHS.DEVICE.WEIGHT.Model;
using WHS.Infrastructure.Utils;
using WHS.Infrastructure;
using System.Threading;

namespace WHS.DEVICE.WEIGHT.ViewModels
{
    public class WeightViewModel : Screen
    {
        private static NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();

        private int _EWTypeSelectedIndex;

        public int EWTypeSelectedIndex
        {
            get
            {
                return _EWTypeSelectedIndex;
            }
            set
            {
                Set(ref _EWTypeSelectedIndex, value);
            }
        }

        private string _ErrorText;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorText
        {
            get
            {
                return _ErrorText;
            }
            set
            {
                Set(ref _ErrorText, value);
            }
        }


        private ComboBoxModel _EWType;

        /// <summary>
        /// 电子秤类型
        /// </summary>
        public ComboBoxModel EWType
        {
            get
            {
                return _EWType;
            }
            set
            {
                Set(ref _EWType, value);
            }
        }

        private int _PortNameSelectedIndex;

        public int PortNameSelectedIndex
        {
            get
            {
                return _PortNameSelectedIndex;
            }
            set
            {
                Set(ref _PortNameSelectedIndex, value);
            }
        }

        private ComboBoxModel _PortName;

        /// <summary>
        /// 端口名称
        /// </summary>
        public ComboBoxModel PortName
        {
            get
            {
                return _PortName;
            }
            set
            {
                Set(ref _PortName, value);
            }
        }

        private int _BaudRateSelectedIndex;

        public int BaudRateSelectedIndex
        {
            get
            {
                return _BaudRateSelectedIndex;
            }
            set
            {
                Set(ref _BaudRateSelectedIndex, value);
            }
        }

        private ComboBoxModel _BaudRate;
        /// <summary>
        /// 位/秒
        /// </summary>
        public ComboBoxModel BaudRate
        {
            get
            {
                return _BaudRate;
            }
            set
            {
                Set(ref _BaudRate, value);
            }
        }

        private int _DataBitsSelectedIndex;

        public int DataBitsSelectedIndex
        {
            get
            {
                return _DataBitsSelectedIndex;
            }
            set
            {
                Set(ref _DataBitsSelectedIndex, value);
            }
        }

        private ComboBoxModel _DataBits;
        /// <summary>
        /// 数据位
        /// </summary>
        public ComboBoxModel DataBits
        {
            get
            {
                return _DataBits;
            }
            set
            {
                Set(ref _DataBits, value);
            }
        }

        private int _ParitySelectedIndex;

        public int ParitySelectedIndex
        {
            get
            {
                return _ParitySelectedIndex;
            }
            set
            {
                Set(ref _ParitySelectedIndex, value);
            }
        }
        private ComboBoxModel _Parity;
        /// <summary>
        /// 奇偶校验
        /// </summary>
        public ComboBoxModel Parity
        {
            get
            {
                return _Parity;
            }
            set
            {
                Set(ref _Parity, value);
            }
        }

        private int _StopBitsSelectedIndex;

        public int StopBitsSelectedIndex
        {
            get
            {
                return _StopBitsSelectedIndex;
            }
            set
            {
                Set(ref _StopBitsSelectedIndex, value);
            }
        }

        private ComboBoxModel _StopBits;
        /// <summary>
        /// 停止位
        /// </summary>
        public ComboBoxModel StopBits
        {
            get
            {
                return _StopBits;
            }
            set
            {
                Set(ref _StopBits, value);
            }
        }

        public WeightSettingModel weightSettingModel { get; set; }


        public IObservableCollection<ComboBoxModel> EWTypeSource { get; }
        public IObservableCollection<ComboBoxModel> PortNameSource { get; }
        public IObservableCollection<ComboBoxModel> BaudRateSource { get; }
        public IObservableCollection<ComboBoxModel> DataBitsSource { get; }
        public IObservableCollection<ComboBoxModel> ParitySource { get; }
        public IObservableCollection<ComboBoxModel> StopBitsSource { get; }

        public WeightViewModel()
        {
            EWTypeSource = new BindableCollection<ComboBoxModel>();
            PortNameSource = new BindableCollection<ComboBoxModel>();
            BaudRateSource = new BindableCollection<ComboBoxModel>();
            DataBitsSource = new BindableCollection<ComboBoxModel>();
            ParitySource = new BindableCollection<ComboBoxModel>();
            StopBitsSource = new BindableCollection<ComboBoxModel>();
            //电子秤数据
            var ewTypes = WeightProviderFacotry.LoadWeightType();
            if (ewTypes != null)
            {
                foreach (KeyValuePair<string, string> pair in ewTypes)
                {
                    EWTypeSource.Add(new ComboBoxModel() { Text = pair.Key, Value = pair.Value });
                }
            }
            int[] int_BaudRate = new int[] { 110, 300, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };

            foreach (var item in int_BaudRate)
            {
                BaudRateSource.Add(new ComboBoxModel() { Text = item.ToString(), Value = item.ToString() });
            }
            int[] int_DataBits = new int[] { 5, 6, 7, 8 };
            foreach (var item in int_DataBits)
            {
                DataBitsSource.Add(new ComboBoxModel() { Text = item.ToString(), Value = item.ToString() });
            }

            ParitySource.Add(new ComboBoxModel() { Text = "无", Value = System.IO.Ports.Parity.None.ToString() });
            ParitySource.Add(new ComboBoxModel() { Text = "奇校验", Value = System.IO.Ports.Parity.Odd.ToString() });
            ParitySource.Add(new ComboBoxModel() { Text = "偶校验", Value = System.IO.Ports.Parity.Even.ToString() });
            ParitySource.Add(new ComboBoxModel() { Text = "标志", Value = System.IO.Ports.Parity.Mark.ToString() });
            ParitySource.Add(new ComboBoxModel() { Text = "空格", Value = System.IO.Ports.Parity.Space.ToString() });

            StopBitsSource.Add(new ComboBoxModel() { Text = "1", Value = System.IO.Ports.StopBits.One.ToString() });
            StopBitsSource.Add(new ComboBoxModel() { Text = "1.5", Value = System.IO.Ports.StopBits.OnePointFive.ToString() });
            StopBitsSource.Add(new ComboBoxModel() { Text = "2", Value = System.IO.Ports.StopBits.Two.ToString() });
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var task= base.OnActivateAsync(cancellationToken);
            ErrorText = string.Empty;


            //端口名称
            string[] portNames = SerialPort.GetPortNames();
            if (0 == portNames.Length)
            {
                ErrorText = LocalizationHelp.GetLocalizedString(this.GetType().Assembly, "WeightView.Bind.ErrorText");
                logger.Warn("请验证COM端口是否被禁用或未连接设备", "COM端口获取失败");
            }
            else
            {
                foreach (string portName in portNames)
                {
                    PortNameSource.Add(new ComboBoxModel() { Text = portName, Value = portName });
                }
            }

            weightSettingModel = WeightPluginDefinition.JsonConfig.User.ToObject<WeightSettingModel>();

            if (weightSettingModel != null)
            {
                EWType = weightSettingModel.EWType;
                EWTypeSelectedIndex = EWTypeSource.FindIndex<ComboBoxModel>(p => CompareComboBoxModel(EWType, p));
                if (portNames.Length > 0)
                {
                    PortName = weightSettingModel.PortName;
                    PortNameSelectedIndex = PortNameSource.FindIndex<ComboBoxModel>(p => CompareComboBoxModel(PortName, p));
                }
                BaudRate = weightSettingModel.BaudRate;
                BaudRateSelectedIndex = BaudRateSource.FindIndex<ComboBoxModel>(p => CompareComboBoxModel(BaudRate, p));
                DataBits = weightSettingModel.DataBits;
                DataBitsSelectedIndex = DataBitsSource.FindIndex<ComboBoxModel>(p => CompareComboBoxModel(DataBits, p));
                Parity = weightSettingModel.Parity;
                ParitySelectedIndex = ParitySource.FindIndex<ComboBoxModel>(p => CompareComboBoxModel(Parity, p));
                StopBits = weightSettingModel.StopBits;
                StopBitsSelectedIndex = StopBitsSource.FindIndex<ComboBoxModel>(p => CompareComboBoxModel(StopBits, p));

            }
            return task;
        }
        public bool CompareComboBoxModel(ComboBoxModel item, ComboBoxModel source)
        {
            if (item == null || source == null)
                return false;
            if (item.Text == source.Text && item.Value == source.Value)
                return true;
            return false;
        }

    }
}
