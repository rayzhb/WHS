using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using WHS.IDMVS.Model;
using WHS.IDMVS.SDK;
using WHS.Infrastructure;

namespace WHS.IDMVS.ViewModels
{
    public class DeviceViewModel : Screen
    {

        private MVIDCodeReader.MVID_CAMERA_INFO_LIST stDevList = new MVIDCodeReader.MVID_CAMERA_INFO_LIST();

        private int _CameraSelectedIndex;

        public int CameraSelectedIndex
        {
            get
            {
                return _CameraSelectedIndex;
            }
            set
            {
                Set(ref _CameraSelectedIndex, value);
            }
        }

        private ComboBoxModel _Camera;

        /// <summary>
        /// 相机
        /// </summary>
        public ComboBoxModel Camera
        {
            get
            {
                return _Camera;
            }
            set
            {
                Set(ref _Camera, value);
            }
        }

        public IObservableCollection<ComboBoxModel> CameraSource
        {
            get;
        }
        public DeviceViewModel()
        {
            CameraSource=new BindableCollection<ComboBoxModel>();
            DeviceListAcq();
        }

        private void DeviceListAcq()
        {
            Int32 nRet = MVIDCodeReader.MVID_CR_OK;
            System.GC.Collect();

            // ch:枚举相机 | en:Enumerate cameras
            nRet = MVIDCodeReader.MVID_CR_CAM_EnumDevices_NET(ref stDevList);
            if (MVIDCodeReader.MVID_CR_OK != nRet)
            {
                string csMessage;
                csMessage = "enum device failed 0x" + String.Format("{0:X}", nRet);
                GlobalContext.WindowManager.ShowDialogAsync(csMessage);
                return;
            }

            MVIDCodeReader.MVID_CAMERA_INFO stDevInfo;                            // ch:通用设备信息 | en:General device information
            for (int i = 0; i < stDevList.nDeviceNum; i++)
            {
                stDevInfo = (MVIDCodeReader.MVID_CAMERA_INFO)Marshal.PtrToStructure(stDevList.pstCamInfo[i], typeof(MVIDCodeReader.MVID_CAMERA_INFO));

                if (MVIDCodeReader.MVID_GIGE_CAM == stDevInfo.nCamType)
                {
                    if (stDevInfo.chUserDefinedName != "")
                    {
                        CameraSource.Add(new ComboBoxModel()
                        {
                            Text="[" + i + "] " + "GigE: " + stDevInfo.chUserDefinedName + " (" + stDevInfo.chSerialNumber + ")",
                            Value  =i.ToString(),
                        });
                    }
                    else
                    {
                        CameraSource.Add(new ComboBoxModel()
                        {
                            Text="[" + i + "] " + "GigE: " + stDevInfo.chManufacturerName + " " + stDevInfo.chModelName + " (" + stDevInfo.chSerialNumber + ")",
                            Value  =i.ToString(),
                        });
                    }
                }
                else if (MVIDCodeReader.MVID_USB_CAM == stDevInfo.nCamType)
                {
                    if (stDevInfo.chUserDefinedName != "")
                    {
                        CameraSource.Add(new ComboBoxModel()
                        {
                            Text="[" + i + "] " + "USB: " + stDevInfo.chUserDefinedName + " (" + stDevInfo.chSerialNumber + ")",
                            Value  =i.ToString(),
                        });
                    }
                    else
                    {
                        CameraSource.Add(new ComboBoxModel()
                        {
                            Text="[" + i + "] " + "USB: " + stDevInfo.chManufacturerName + " " + stDevInfo.chModelName + " (" + stDevInfo.chSerialNumber + ")",
                            Value  =i.ToString(),
                        });
                    }
                }
            }
            // ch:选择第一项 | en:Select the first item
            if (stDevList.nDeviceNum != 0)
            {
                CameraSelectedIndex = 0;
            }
        }

    }
}
