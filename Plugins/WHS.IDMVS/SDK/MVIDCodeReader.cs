using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WHS.IDMVS.SDK
{
    /// <summary>
    /// MVIDCodeReader
    /// </summary>
    public class MVIDCodeReader
    {
        /// <summary>
        /// ch:图像回调 | en:Image data callback
        /// </summary>
        /// <param name="pstOutput">ch:图像回调参数指针 | en:Callback function pointer</param>
        /// <param name="pUser">ch:用户自定义变量 | en:User defined variable</param>
        public delegate void cbOutputdelegate(IntPtr pstOutput, IntPtr pUser);

        /// <summary>
        /// ch:图像回调 | en:Image buffer callback
        /// </summary>
        /// <param name="pstOutput">ch:图像回调参数结构体 | en:Image parameters structure</param>
        /// <param name="pUser">ch:用户自定义变量 | en:User defined variable</param>
        public delegate void cbImageBufferdelegate(ref MVID_IMAGE_INFO pstOutput, IntPtr pUser);

        /// <summary>
        /// ch:异常消息回调 | en:Exception message callBack
        /// </summary>
        /// <param name="nMsgType">ch:异常回调参数 | en:Exception message</param>
        /// <param name="pUser">ch:用户自定义变量 | en:User defined variable</param>
        public delegate void cbExceptiondelegate(uint nMsgType, IntPtr pUser);

        /// <summary>
        /// ch:预处理回调 | en:Pretreatment image data callback
        /// </summary>
        /// <param name="pstPreOutput">ch:预处理回调参数输出指针 | en:Output image parameters structure</param>
        /// <param name="pstProcInput">ch:预处理回调参数输入指针 | en:Intput image parameters structure</param>
        /// <param name="pUser">ch:用户自定义变量 | en:User defined variable</param>
        public delegate void cbPreOutputdelegate(IntPtr pstPreOutput, IntPtr pstProcInput, IntPtr pUser);

        /// <summary>
        /// ch:事件回调 | en:Event callBack
        /// </summary>
        /// <param name="pEventInfo">ch:事件回调参数指针 | en:Exception callBack function structure</param>
        /// <param name="pUser">ch:用户自定义变量 | en:User defined variable</param>
        public delegate void cbEventdelegate(ref MVID_EVENT_OUT_INFO pEventInfo, IntPtr pUser);

        /// <summary>ch:相机属性 | en:Camera Information</summary>
        public struct MVID_CAMERA_INFO
        {
            /// <summary>ch:相机类型, G口或U口 | en:Camera type: USB, GigE</summary>
            public uint nCamType;

            /// <summary>ch:制造商名字 | en:Manufacturer Name</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string chManufacturerName;

            /// <summary>ch:型号名字 | en:Model Name</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string chModelName;

            /// <summary>ch:设备版本号 | en:Device version No.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string chDeviceVersion;

            /// <summary>ch:制造商特定的信息 | en:Manufacturer specific information</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string chManufacturerSpecificInfo;

            /// <summary>ch:序列号 | en:Device serial No.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string chSerialNumber;

            /// <summary>ch:用户自定义名字 | en:Custom name</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string chUserDefinedName;

            /// <summary>ch:MAC 地址 高位| en:High MAC address</summary>
            public uint nMacAddrHigh;

            /// <summary>ch:MAC 地址 低位| en:Low MAC address</summary>
            public uint nMacAddrLow;

            /// <summary>ch:保留| en:Reserved</summary> 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nCommomReaserved;

            /// <summary>ch:当前IP | en:Current IP address</summary> 
            public uint nCurrentIp;

            /// <summary>ch:网口IP地址 | en:GIGE IP Address</summary>
            public uint nNetExport;

            /// <summary>ch:gige保留 | en:GigE Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public uint[] nGigEReserved;

            /// <summary>ch:控制输入端点 | en:Control input endpoint</summary>
            public byte CrtlInEndPoint;

            /// <summary>ch:控制输出端点 | en:Control output endpoint</summary>
            public byte CrtlOutEndPoint;

            /// <summary>ch:流端点 | en:Stream endpoint</summary>
            public byte StreamEndPoint;

            /// <summary>ch:事件端点 | en:Event endpoint</summary>
            public byte EventEndPoint;

            /// <summary>ch:供应商ID号 | en:Vendor ID</summary> 
            public ushort idVendor;

            /// <summary>ch:产品ID号 | en:Product ID</summary>
            public ushort idProduct;

            /// <summary>ch:设备序列号 | en:Device ID</summary>
            public uint nDeviceNumber;

            /// <summary>ch:USB保留 | en:Usb Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
            public uint[] nUsbReserved;

            /// <summary>ch:选择设备 |en:Choose device</summary>
            public bool bSelectDevice;

            /// <summary>ch:保留| en:Reserved</summary> 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 63)]
            public uint[] nReserved;
        }

        /// <summary>ch:设备信息列表 | en:Device Information List</summary>
        public struct MVID_CAMERA_INFO_LIST
        {
            /// <summary>ch:在线设备数量 | en:The number of online devices</summary>
            public uint nDeviceNum;

            /// <summary>ch:支持最多256个设备 | en:Device information, up to 256 devices can be supported</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public IntPtr[] pstCamInfo;
        }

        /// <summary>ch:条码类型 | en:Code Type</summary>
        public enum MVID_CODE_TYPE
        {
            /// <summary>ch:无可识别条码 | en:No recognizable bar code</summary>
            MVID_CODE_NONE = 0,
            /// <summary>ch:DM码 | en:DM code</summary>
            MVID_CODE_TDCR_DM = 1,
            /// <summary>ch:QR码 | en:QR code</summary>
            MVID_CODE_TDCR_QR = 2,
            /// <summary>ch:EAN8码 | en:EAN8 code</summary>
            MVID_CODE_BCR_EAN8 = 8,
            /// <summary>ch:UPCE码 | en:UPCE code</summary>
            MVID_CODE_BCR_UPCE = 9,
            /// <summary>ch:UPCA码 | en:UPCA code</summary>
            MVID_CODE_BCR_UPCA = 12,
            /// <summary>ch:EAN13码 | en:EAN13 code</summary>
            MVID_CODE_BCR_EAN13 = 13,
            /// <summary>ch:ISBN13码 | en:ISBN13 code</summary>
            MVID_CODE_BCR_ISBN13 = 14,
            /// <summary>ch:库德巴码 | en:Codabar code</summary>
            MVID_CODE_BCR_CODABAR = 20,
            /// <summary>ch:交叉25码 | en:ITF25 code</summary>
            MVID_CODE_BCR_ITF25 = 25,
            /// <summary>ch:Code 39 | en:Code 39</summary>
            MVID_CODE_BCR_CODE39 = 39,
            /// <summary>ch:Code 93 | en:Code 93</summary>
            MVID_CODE_BCR_CODE93 = 93,
            /// <summary>ch:Code 128 | en:Code 128</summary>
            MVID_CODE_BCR_CODE128 = 128
        }

        /// <summary>ch:图像格式 | en:Image Type</summary>
        public enum MVID_IMAGE_TYPE
        {
            /// <summary>ch:未定义 | en:Undefined format</summary>
            MVID_IMAGE_Undefined,
            /// <summary>ch:Mono8 | en:MONO8 format</summary>
            MVID_IMAGE_MONO8,
            /// <summary>ch:JPEG | en:JPEG format</summary>
            MVID_IMAGE_JPEG,
            /// <summary>ch:Bmp | en:BMP format</summary>
            MVID_IMAGE_BMP,
            /// <summary>ch:RGB24 | en:RGB format</summary>
            MVID_IMAGE_RGB24,
            /// <summary>ch:BGR24 | en:BGR format</summary>
            MVID_IMAGE_BGR24,
            /// <summary>ch:Mono10 | en:Mono10 format</summary>
            MVID_IMAGE_MONO10,
            /// <summary>ch:Mono10_Packed | en:Mono10_Packed format</summary>
            MVID_IMAGE_MONO10_Packed,
            /// <summary>ch:Mono12 | en:Mono12 format</summary>
            MVID_IMAGE_MONO12,
            /// <summary>ch:Mono12_Packed | en:Mono12_Packed format</summary>
            MVID_IMAGE_MONO12_Packed,
            /// <summary>ch:Mono16 | en:Mono16 format</summary>
            MVID_IMAGE_MONO16,
            /// <summary>ch:BGR8 | en:BGR8 format</summary>
            MVID_IMAGE_BayerGR8,
            /// <summary>ch:BRG8 | en:BRG8 format</summary>
            MVID_IMAGE_BayerRG8,
            /// <summary>ch:BGB8 | en:BGB8 format</summary>
            MVID_IMAGE_BayerGB8,
            /// <summary>ch:BBG8 | en:BBG8 format</summary>
            MVID_IMAGE_BayerBG8,
            /// <summary>ch:BGR10 | en:BGR10 format</summary>
            MVID_IMAGE_BayerGR10,
            /// <summary>ch:BRG10 | en:BRG10 format</summary>
            MVID_IMAGE_BayerRG10,
            /// <summary>ch:BGB10 | en:BGB10 format</summary>
            MVID_IMAGE_BayerGB10,
            /// <summary>ch:BBG10 | en:BBG10 format</summary>
            MVID_IMAGE_BayerBG10,
            /// <summary>ch:BGR12 | en:BGR12 format</summary>
            MVID_IMAGE_BayerGR12,
            /// <summary>ch:BRG12 | en:BRG12 format</summary>
            MVID_IMAGE_BayerRG12,
            /// <summary>ch:BGB12 | en:BGB12 format</summary>
            MVID_IMAGE_BayerGB12,
            /// <summary>ch:BBG12 | en:BBG12 format</summary>
            MVID_IMAGE_BayerBG12,
            /// <summary>ch:BGR10_Packed | en:BGR10_Packed format</summary>
            MVID_IMAGE_BayerGR10_Packed,
            /// <summary>ch:BRG10_Packed | en:BRG10_Packed format</summary>
            MVID_IMAGE_BayerRG10_Packed,
            /// <summary>ch:BGB10_Packed | en:BGB10_Packed format</summary>
            MVID_IMAGE_BayerGB10_Packed,
            /// <summary>ch:BBG10_Packed | en:BBG10_Packed format</summary>
            MVID_IMAGE_BayerBG10_Packed,
            /// <summary>ch:BGR12_Packed | en:BGR12_Packed format</summary>
            MVID_IMAGE_BayerGR12_Packed,
            /// <summary>ch:BRG12_Packed | en:BRG12_Packed format</summary>
            MVID_IMAGE_BayerRG12_Packed,
            /// <summary>ch:BGB12_Packed | en:BGB12_Packed format</summary>
            MVID_IMAGE_BayerGB12_Packed,
            /// <summary>ch:BBG12_Packed | en:BBG12_Packed format</summary>
            MVID_IMAGE_BayerBG12_Packed,
            /// <summary>ch:YUV422_Packed | en:YUV422_Packed format</summary>
            MVID_IMAGE_YUV422_Packed,
            /// <summary>ch:YUV422_YUYV_Packed | en:YUV422_YUYV_Packed format</summary>
            MVID_IMAGE_YUV422_YUYV_Packed,
            /// <summary>ch:RGB8_Packed | en:RGB8_Packed format</summary>
            MVID_IMAGE_RGB8_Packed,
            /// <summary>ch:BGR8_Packed | en:BGR8_Packed format</summary>
            MVID_IMAGE_BGR8_Packed,
            /// <summary>ch:RGBA8_Packed | en:RGBA8_Packed format</summary>
            MVID_IMAGE_RGBA8_Packed,
            /// <summary>ch:BGRA8_Packed | en:BGRA8_Packed format</summary>
            MVID_IMAGE_BGRA8_Packed
        }

        /// <summary>ch:过滤条码标识 | en:Filter bar code flag</summary>
        public enum MVID_CODE_FLAG
        {
            /// <summary>ch:正常条码 | en:Normal bar code</summary>
            MVID_CODE_CORRECT,
            /// <summary>ch:过滤条码 | en:Filter bar code</summary>
            MVID_CODE_FILTERED
        }

        /// <summary>ch:图像输出类型 | en:Image out put mode</summary>
        public enum MVID_IMAGE_OUTPUT_MODE
        {
            /// <summary>ch:常规输出 | en:normal output</summary>
            MVID_OUTPUT_NORMAL,
            /// <summary>ch:原图输出 | en:raw output</summary>
            MVID_OUTPUT_RAW
        }

        /// <summary>ch:输出图像的信息 | en:Output Frame Information</summary>
        public struct MVID_IMAGE_INFO
        {
            /// <summary>ch:原始图像缓存, 由SDK内部分配 | en:Original image buffer</summary>
            public IntPtr pImageBuf;

            /// <summary>ch:原始图像长度 | en:Original image size</summary>
            public uint nImageLen;

            /// <summary>ch:图像格式 | en:Image Type</summary>
            public MVID_IMAGE_TYPE enImageType;

            /// <summary>ch:图像宽 | en:Image Width</summary>
            public ushort nWidth;

            /// <summary>ch:图像高 | en:Image Height</summary>
            public ushort nHeight;

            /// <summary>ch:帧号 | en:Frame No.</summary>
            public uint nFrameNum;

            /// <summary>ch:时间戳高32位 | en:Timestamp high 32 bits</summary>
            public uint nDevTimeStampHigh;

            /// <summary>ch:时间戳低32位 | en:Timestamp low 32 bits</summary>
            public uint nDevTimeStampLow;

            /// <summary>ch:保留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public uint[] nReserved;
        }

        /// <summary>ch:条码坐标 | en:Code coordinate</summary>
        public struct MVID_POINT_I
        {
            /// <summary>ch:X坐标 | en:X-coordinate</summary>
            public int nX;

            /// <summary>ch:Y坐标 | en:Y-coordinate</summary>
            public int nY;
        }

        /// <summary>ch:条码信息 | en:Code information</summary>
        public struct MVID_CODE_INFO
        {
            /// <summary>ch:字符 | en:Character, maximum size: 4096</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4096)]
            public string strCode;

            /// <summary>ch:字符长度 | en:Character size</summary>
            public int nLen;

            /// <summary>ch:条码类型 | en:Bar code type</summary>
            public MVID_CODE_TYPE enBarType;

            /// <summary>ch:条码位置 | en:Bar code location</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public MVID_POINT_I[] stCornerPt;

            /// <summary>ch:条码角度（0~3600） | en:Bar code angle, range: [0, 3600°]</summary>
            public int nAngle;

            /// <summary>ch:过滤码标识(0为正常码, 1为过滤码) | en:Filter identifier: 0- normal code, 1-filter code</summary>
            public int nFilterFlag;

            /// <summary>ch:保留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
            public uint[] nReserved;
        }

        /// <summary>ch:条码信息 | en:Code information</summary>
        public struct MVID_CODE_INFOEx
        {
            /// <summary>ch:字符 | en:Character, maximum size: 4096</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
            public byte[] strCode;

            /// <summary>ch:字符长度 | en:Character size</summary>
            public int nLen;

            /// <summary>ch:条码类型 | en:Bar code type</summary>
            public MVID_CODE_TYPE enBarType;

            /// <summary>ch:条码位置 | en:Bar code location</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public MVID_POINT_I[] stCornerPt;

            /// <summary>ch:条码角度（0~3600） | en:Bar code angle, range: [0, 3600°]</summary>
            public int nAngle;

            /// <summary>ch:过滤码标识(0为正常码, 1为过滤码) | en:Filter identifier: 0- normal code, 1-filter code</summary>
            public int nFilterFlag;

            /// <summary>ch:保留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
            public uint[] nReserved;
        }

        /// <summary>ch:条码信息列表 | en:Code information list</summary>
        public struct MVID_CODE_INFO_LIST
        {
            /// <summary>ch:条码数量 | en:The number of bar codes</summary>
            public int nCodeNum;

            /// <summary>ch:条码信息 | en:Bar code information, maximum size: 256</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public MVID_CODE_INFO[] stCodeInfo;

            /// <summary>ch:保留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public uint[] nReserved;
        }

        /// <summary>ch:条码信息列表 | en:Code information list</summary>
        public struct MVID_CODE_INFO_LISTEx
        {
            /// <summary>ch:条码数量 | en:The number of bar codes</summary>
            public int nCodeNum;

            /// <summary>ch:条码信息 | en:Bar code information, maximum size: 256</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public MVID_CODE_INFOEx[] stCodeInfo;

            /// <summary>ch:保留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public uint[] nReserved;
        }

        /// <summary>ch:输出信息 | en:Output information</summary>
        public struct MVID_CAM_OUTPUT_INFO
        {
            /// <summary>ch:输出图像的信息 | en:Image information</summary>
            public MVID_IMAGE_INFO stImage;

            /// <summary>ch:条码信息列表 | en:Bar code information</summary>
            public MVID_CODE_INFO_LIST stCodeList;

            /// <summary>ch:抠图缓存, 由SDK内部分配 | en:Image matting buffer</summary>
            public IntPtr pImageWaybill;

            /// <summary>ch:图像大小 | en:Image size</summary>
            public uint nImageWaybillLen;

            /// <summary>ch:抠图图像格式 | en:Image format</summary>
            public MVID_IMAGE_TYPE enWaybillImageType;

            /// <summary>ch:保留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
            public uint[] nReserved;
        }

        /// <summary>ch:输出信息 | en:Output information</summary>
        public struct MVID_CAM_OUTPUT_INFOEx
        {
            /// <summary>ch:输出图像的信息 | en:Image information</summary>
            public MVID_IMAGE_INFO stImage;

            /// <summary>ch:条码信息列表 | en:Bar code information</summary>
            public MVID_CODE_INFO_LISTEx stCodeList;

            /// <summary>ch:抠图缓存, 由SDK内部分配 | en:Image matting buffer</summary>
            public IntPtr pImageWaybill;

            /// <summary>ch:图像大小 | en:Image size</summary>
            public uint nImageWaybillLen;

            /// <summary>ch:抠图图像格式 | en:Image format</summary>
            public MVID_IMAGE_TYPE enWaybillImageType;

            /// <summary>ch:保留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
            public uint[] nReserved;
        }

        /// <summary>ch:运行参数 | en:Process parameter</summary>
        public struct MVID_PROC_PARAM
        {
            /// <summary>ch:原始图像缓存, 由用户传入 | en:Original image buffer</summary>
            public IntPtr pImageBuf;

            /// <summary>ch:原始图像长度 | en:Original image size</summary>
            public uint nImageLen;

            /// <summary>ch:输入图像的格式 | en:Image type</summary>
            public MVID_IMAGE_TYPE enImageType;

            /// <summary>ch:图像宽 | en:Image width</summary>
            public ushort nWidth;

            /// <summary>ch:图像高 | en:Image height</summary>
            public ushort nHeight;

            /// <summary>ch:条码信息 | en:Bar code information</summary>
            public MVID_CODE_INFO_LIST stCodeList;

            /// <summary>ch:抠图缓存, 由SDK内部分配 | en:Matting buffer, which is allocated by SDK</summary>
            public IntPtr pImageWaybill;

            /// <summary>ch:图像大小 | en:Image size</summary>
            public int nImageWaybillLen;

            /// <summary>ch:抠图图像格式 | en:The format of the matted image</summary>
            public MVID_IMAGE_TYPE enWaybillImageType;

            /// <summary>ch:保留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
            public uint[] nReserved;
        }

        /// <summary>ch:运行参数 | en:Process parameter</summary>
        public struct MVID_PROC_PARAMEx
        {
            /// <summary>ch:原始图像缓存, 由用户传入 | en:Original image buffer</summary>
            public IntPtr pImageBuf;

            /// <summary>ch:原始图像长度 | en:Original image size</summary>
            public uint nImageLen;

            /// <summary>ch:输入图像的格式 | en:Image type</summary>
            public MVID_IMAGE_TYPE enImageType;

            /// <summary>ch:图像宽 | en:Image width</summary>
            public ushort nWidth;

            /// <summary>ch:图像高 | en:Image height</summary>
            public ushort nHeight;

            /// <summary>ch:条码信息 | en:Bar code information</summary>
            public MVID_CODE_INFO_LISTEx stCodeList;

            /// <summary>ch:抠图缓存, 由SDK内部分配 | en:Matting buffer, which is allocated by SDK</summary>
            public IntPtr pImageWaybill;

            /// <summary>ch:图像大小 | en:Image size</summary>
            public int nImageWaybillLen;

            /// <summary>ch:抠图图像格式 | en:The format of the matted image</summary>
            public MVID_IMAGE_TYPE enWaybillImageType;

            /// <summary>ch:保留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
            public uint[] nReserved;
        }

        /// <summary>ch:整型参数 | en:Integer value</summary>
        public struct MVID_CR_CAM_INTVALUE
        {
            /// <summary>ch:当前值 | en:Current Value</summary>
            public uint nCurValue;

            /// <summary>ch:最大值 | en:The maximum value</summary>
            public uint nMax;

            /// <summary>ch:最小值 | en:The minimum value</summary>
            public uint nMin;

            /// <summary>ch:增量值 | en:Increment</summary>
            public uint nInc;

            /// <summary>ch:保留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;
        }

        /// <summary>ch:整型参数 | en:Integer value</summary>
        public struct MVID_CAM_INTVALUE_EX
        {
            /// <summary>ch:当前值 | en:Current Value</summary>
            public long nCurValue;

            /// <summary>ch:最大值 | en:The maximum value</summary>
            public long nMax;

            /// <summary>ch:最小值 | en:The minimum value</summary>
            public long nMin;

            /// <summary>ch:增量值 | en:Increment</summary>
            public long nInc;

            /// <summary>ch:保留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public uint[] nReserved;
        }

        /// <summary>ch:枚举型参数 | en:Enumeration value</summary>
        public struct MVID_CAM_ENUMVALUE
        {
            /// <summary>ch:当前值 | en:Current Value</summary>
            public uint nCurValue;

            /// <summary>ch:数据的有效数据个数 | The number of valid data</summary>
            public uint nSupportedNum;

            /// <summary>ch:支持的枚举类型, 每个数组表示一种类型, 最大大小为：64 | en:Supported enumeration types, each array indicates one type, , maximum size: 64</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public uint[] nSupportValue;

            /// <summary>ch:保留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;
        }

        /// <summary>ch:浮点型参数 | en:Float value</summary>
        public struct MVID_CAM_FLOATVALUE
        {
            /// <summary>ch:当前值 | en:Current Value</summary>
            public float fCurValue;

            /// <summary>ch:最大值 | en:The maximum value</summary>
            public float fMax;

            /// <summary>ch:最小值 | en:The minimum value</summary>
            public float fMin;

            /// <summary>ch:保留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;
        }

        /// <summary>ch:字符型参数 | en:String value</summary>
        public struct MVID_CAM_STRINGVALUE
        {
            /// <summary>ch:当前值 | en:Current Value</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string chCurValue;

            /// <summary>ch:最大长度 | en:The maximum size</summary>
            public long nMaxLength;

            /// <summary>ch:保留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] nReserved;
        }

        /// <summary>ch:Event事件回调信息 | en:Event callback infomation</summary>
        public struct MVID_EVENT_OUT_INFO
        {
            /// <summary>ch:Event名称 | en:Event name, , maximum size: 128</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string EventName;

            /// <summary>ch:Event号 | en:Event ID</summary>
            public ushort nEventID;

            /// <summary>ch:流通道序号 | en:Stream channel ID</summary>
            public ushort nStreamChannel;

            /// <summary>ch:帧号高位 | en:High frame ID</summary>
            public uint nBlockIdHigh;

            /// <summary>ch:帧号低位 | en:Low frame ID</summary>
            public uint nBlockIdLow;

            /// <summary>ch:时间戳高位 | en:High timestamp</summary>
            public uint nTimestampHigh;

            /// <summary>ch:时间戳低位 | en:Low timestamp</summary>
            public uint nTimestampLow;

            /// <summary>ch:Event数据 | en:Event data</summary>
            public IntPtr pEventData;

            /// <summary>ch:Event数据长度 | en:Event data size</summary>
            public uint nEventDataSize;

            /// <summary>ch:预留 | en:Reserved</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public uint[] nReserved;
        }

        /// <summary>ch:成功, 无错误 | en:Successed, no error</summary>
        public const int MVID_CR_OK = 0;

        /// <summary>ch:错误或无效的句柄 | en:Error or invalid handle</summary>
        public const int MVID_CR_E_HANDLE = int.MinValue;

        /// <summary>ch:不支持的功能 | en:The function is not supported</summary>
        public const int MVID_CR_E_SUPPORT = -2147483647;

        /// <summary>ch:缓存已满 | en:Buffer is full</summary>
        public const int MVID_CR_E_BUFOVER = -2147483646;

        /// <summary>ch:函数调用顺序错误 | en:Incorrect calling sequence</summary>
        public const int MVID_CR_E_CALLORDER = -2147483645;

        /// <summary>ch:错误的参数 | en:Incorrect parameter</summary>
        public const int MVID_CR_E_PARAMETER = -2147483644;

        /// <summary>ch:资源申请失败 | en:Applying resource failed</summary>
        public const int MVID_CR_E_RESOURCE = -2147483643;

        /// <summary>ch:无数据 | en:No data</summary>
        public const int MVID_CR_E_NODATA = -2147483642;

        /// <summary>ch:前置条件有误, 或运行环境已发生变化 | en:Precondition error, or running environment changed</summary>
        public const int MVID_CR_E_PRECONDITION = -2147483641;

        /// <summary> ch:凭证错误, 可能是未插加密狗, 或加密狗过期 | en:Credential error, possibly because the dongle was not installed or expired</summary>
        public const int MVID_CR_E_ENCRYPT = -2147483640;

        /// <summary>ch:过滤规则相关的错误 | en:Filter rule error</summary>
        public const int MVID_CR_E_RULE = -2147483638;

        /// <summary>ch:动态导入DLL失败 | en:Dynamically importing the DLL file failed</summary>
        public const int MVID_CR_E_LOAD_LIBRARY = -2147483637;

        /// <summary>ch:jpg编码相关错误 | en:Jpg encoding error</summary>
        public const int MVID_CR_E_JPGENC = -2147483630;

        /// <summary>ch:输入的图像数据有损或图像格式,宽高错误 | en:Abnormal image. Incomplete image caused by packet loss or incorrect image format, width, or height</summary>
        public const int MVID_CR_E_IMAGE = -2147483629;

        /// <summary>ch:格式转换错误 | en:Format conversion error</summary>
        public const int MVID_CR_E_CONVERT = -2147483628;

        /// <summary>ch:未知的错误 | en:Unknown error</summary>
        public const int MVID_CR_E_UNKNOW = -2147483393;

        /// <summary>ch:通用错误 | en:General error</summary>
        public const int MVID_CR_E_GC_GENERIC = -2147483392;

        /// <summary>ch:参数非法 | en:Invalid parameter</summary>
        public const int MVID_CR_E_GC_ARGUMENT = -2147483391;

        /// <summary>ch:值超出范围 | en:The value is out of range</summary>
        public const int MVID_CR_E_GC_RANGE = -2147483390;

        /// <summary>ch:属性错误 | en:Attribute error</summary>
        public const int MVID_CR_E_GC_PROPERTY = -2147483389;

        /// <summary>ch:运行环境有问题 | en:Running environment error</summary>
        public const int MVID_CR_E_GC_RUNTIME = -2147483388;

        /// <summary>ch:逻辑错误 | en:Incorrect logic</summary>
        public const int MVID_CR_E_GC_LOGICAL = -2147483387;

        /// <summary>ch:节点访问条件有误 | en:Node accessing condition error</summary>
        public const int MVID_CR_E_GC_ACCESS = -2147483386;

        /// <summary>ch:超时 | en:Timeout</summary>
        public const int MVID_CR_E_GC_TIMEOUT = -2147483385;

        /// <summary>ch:转换异常 | en:Transformation exception</summary>
        public const int MVID_CR_E_GC_DYNAMICCAST = -2147483384;

        /// <summary>ch:GenICam未知错误 | en:GenICam unknown error</summary>
        public const int MVID_CR_E_GC_UNKNOW = -2147483137;

        /// <summary>ch:命令不被设备支持 | en:The command is not supported by device</summary>
        public const int MVID_CR_E_NOT_IMPLEMENTED = -2147483136;

        /// <summary>ch:访问的目标地址不存在 | en:Target address does not exist</summary>
        public const int MVID_CR_E_INVALID_ADDRESS = -2147483135;

        /// <summary>ch:目标地址不可写 | en:The target address is not writable</summary>
        public const int MVID_CR_E_WRITE_PROTECT = -2147483134;

        /// <summary>ch:设备无访问权限 | en:No access permission</summary>
        public const int MVID_CR_E_ACCESS_DENIED = -2147483133;

        /// <summary>ch:设备忙, 或网络断开 | en:Device is busy, or network is disconnected</summary>
        public const int MVID_CR_E_BUSY = -2147483132;

        /// <summary>ch:网络包数据错误 | en:Network packet error</summary>
        public const int MVID_CR_E_PACKET = -2147483131;

        /// <summary>ch:网络相关错误 | en:Network error</summary>
        public const int MVID_CR_E_NETER = -2147483130;

        /// <summary>ch:设备IP冲突 | en:IP address conflicted</summary>
        public const int MVID_CR_E_IP_CONFLICT = -2147483103;

        /// <summary>ch:读usb出错 | en:USB read error</summary>
        public const int MVID_CR_E_USB_READ = -2147482880;

        /// <summary>ch:写usb出错 | en:USB write error</summary>
        public const int MVID_CR_E_USB_WRITE = -2147482879;

        /// <summary>ch:设备异常 | en:Device exception</summary>
        public const int MVID_CR_E_USB_DEVICE = -2147482878;

        /// <summary>ch:GenICam相关错误 | en:GenICam error</summary>
        public const int MVID_CR_E_USB_GENICAM = -2147482877;

        /// <summary>ch:带宽不足  该错误码新增 | en:Insufficient bandwidth, this error code is newly added</summary>
        public const int MVID_CR_E_USB_BANDWIDTH = -2147482876;

        /// <summary>ch:驱动不匹配或者未装驱动 | en:Driver is mismatched, or is not installed.</summary>
        public const int MVID_CR_E_USB_DRIVER = -2147482875;

        /// <summary>ch:USB未知的错误 | en:USB unknown error</summary>
        public const int MVID_CR_E_USB_UNKNOW = -2147482625;

        /// <summary>ch:相机相关的错误 | en:Camera error</summary>
        public const int MVID_CR_E_CAMERA = -2147475200;

        /// <summary>ch:一维码相关错误 | en:1D barcode error</summary>
        public const int MVID_CR_E_BCR = -2147474944;

        /// <summary>ch:二维码相关错误 | en:2D barcode error</summary>
        public const int MVID_CR_E_TDCR = -2147474688;

        /// <summary>ch:抠图相关错误 | en:Matting error</summary>
        public const int MVID_CR_E_WAYBILL = -2147474432;

        /// <summary>ch:脚本规则相关错误 | en:Script rule error</summary>
        public const int MVID_CR_E_SCRIPT = -2147474176;

        /// <summary>ch:GigE设备 | en:GigE Device</summary>
        public const int MVID_GIGE_CAM = 1;

        /// <summary>ch:USB3.0 设备 | en:USB3.0 Device</summary>
        public const int MVID_USB_CAM = 4;

        /// <summary>ch:设备断开连接 | en:The device is disconnected</summary>
        public const int MVID_EXCEPTION_DEV_DISCONNECT = 32769;

        /// <summary>ch:加密狗掉线 | en:The softdog is disconnected</summary>
        public const int MVID_EXCEPTION_SOFTDOG_DISCONNECT = 32770;

        /// <summary>ch:最大支持的设备个数 | en:The maximum number of supported devices</summary>
        public const int MVID_MAX_CAM_NUM = 256;

        /// <summary>ch:一维码 | en:One-dimensional code</summary>
        public const int MVID_BCR = 1;

        /// <summary>ch:二维码 | en:Two-dimensional code</summary>
        public const int MVID_TDCR = 2;

        /// <summary>ch:面单抠图 | en:Image Matting</summary>
        public const int MVID_WAYBILL = 4;

        /// <summary>ch:最大条码长度 | en:Maximum barcode length</summary>
        public const int MVID_MAX_CODECHARATERLEN = 4096;

        /// <summary>ch:最大条码数量 | en:Maximum number of barcodes</summary>
        public const int MVID_MAX_CODENUM = 256;

        /// <summary>ch:每个已经实现单元的名称数量 | en:The number of names of each implemented unit</summary>
        public const int MVID_MAX_XML_SYMBOLIC_NUM = 64;

        /// <summary>ch:相机Event事件名称最大长度 | en:Max length of event name</summary>
        public const int MVID_MAX_EVENT_NAME_SIZE = 128;

        /// <summary>ch:算法支持最小宽度 | en:Algorithm supports minimum width</summary>
        public const int MVID_ALGORITHM_MIN_WIDTH = 128;

        /// <summary>ch:算法支持最小高度 | en:Algorithm supports minimum height</summary>
        public const int MVID_ALGORITHM_MIN_HEIGHT = 128;

        /// <summary>ch:算法能力集, 含Code39[1], Code128[2], CodeBar[4], EAN[8], ITF25[16], CODE93[32], 默认值63, 范围[0,63] | en:Algorithm capability set.contain:Code39[1], Code128[2], CodeBar[4], EAN[8], ITF25[16], CODE93[32],Range: [0, 63], default: 63</summary>
        public const string KEY_BCR_ABILITY = "BCR_Ability";

        /// <summary>ch:图像ROI X方向偏移, 默认值0, 范围[0,65535] | en:Image ROI X direction offset,Range: [0,65535], default: 0</summary>
        public const string KEY_BCR_ROI_X = "BCR_PositionXROI";

        /// <summary>ch:图像ROI Y方向偏移, 默认值0, 范围[0,65535] | en:Image ROI Y direction offset,Range: [0,65535], default: 0</summary>
        public const string KEY_BCR_ROI_Y = "BCR_PositionYROI";

        /// <summary>ch:图像ROI宽度, 默认值65535, 范围[100,65535] | en:Image ROI width. Range: [100,65535], default: 65535</summary>
        public const string KEY_BCR_ROI_WIDTH = "BCR_WidthROI";

        /// <summary>ch:图像ROI高度, 默认值65535, 范围[40,65535] | en:Image ROI height. Range: [40,65535], default: 65535</summary>
        public const string KEY_BCR_ROI_HEIGHT = "BCR_HeightROI";

        /// <summary>ch:算法最大宽度, 默认3840, 范围[0,65535] | en:Maximum width of the algorithm. Range: [0,65535], default: 3840</summary>
        public const string KEY_BCR_MAX_WIDTH = "BCR_MaxWidth";

        /// <summary>ch:算法最大高度, 默认2748, 范围[0,65535] | en:Maximum height of the algorithm. Range: [0,65535], default: 2748</summary>
        public const string KEY_BCR_MAX_HEIGHT = "BCR_MaxHeight";

        /// <summary>ch:条形码区域定位模块条码区域个数, 默认值4, 范围[1,200] | en:The number of barcodes to read, Range: [1, 200], default: 4</summary>
        public const string KEY_BCR_LOCBARNUM = "BCR_LocBarNum";

        /// <summary>ch:条形码区域定位模块窗口大小, 默认值4, 范围[4,65] | en:Barcode height, Range: [4, 65], default: 4</summary>
        public const string KEY_BCR_LOCWINSIZE = "BCR_LocWinSize";

        /// <summary>ch:算法库Process最大运行时间, 超过限定时间强行return, 默认值500, 范围[0,5000] | en:Algorithm library's maximum running time. The current image processing will end when exceeding the specified running time., Range: [0, 5000], default: 500</summary>
        public const string KEY_BCR_WAITINGTIME = "BCR_WaitingTime";

        /// <summary>ch:条码静区宽度, 默认值30, 范围[0,200] | en:Width of the barcode quiet zone Range: [0, 200], default: 30</summary>
        public const string KEY_BCR_SEGQUIETW = "BCR_SegQuietW";

        /// <summary>ch:去伪过滤尺寸下限（宽不足此数的条码删掉）, 默认值30, 范围[0,4000] | en:Lower limit of barcode width, below which the bar code will be filtered out.Range: [0, 4000], default: 30</summary>
        public const string KEY_BCR_DFKSIZELOWERLIMIT = "BCR_DfkSizeLowerLimit";

        /// <summary>ch:去伪过滤尺寸上限（宽超过此数的条码删掉）, 默认值2400, 范围[0,4000] | en:Upper limit of barcode width, above which the barcode will be filtered out.Range: [0, 4000], default: 2400</summary>
        public const string KEY_BCR_DFKSIZEUPPERLIMIT = "BCR_DfkSizeUpperLimit";

        /// <summary>ch:保存未译出图片的灵敏度, 默认值1, 范围[1,3] | en:Sensitivity of saving untranslated images, Range: [1, 3], default: 1</summary>
        public const string KEY_BCR_SAVEIMAGELEVEL = "BCR_SaveImageLevel";

        /// <summary>ch:算法库运行模式（动态模式/预留模式）, 默认值1, 范围[0,2147483646] | en:Algorithm library operating mode in dynamic mode or reservation mode. Range: [0, 2147483646], default: 1</summary>
        public const string KEY_BCR_APPMODE = "BCR_AppMode";

        /// <summary>ch:动态模式下 透视畸变开关, 默认值0, 范围[0,2147483646] | en:Perspective distortion switch in expert mode. Range: [0, 2147483646], default: 0</summary>
        public const string KEY_BCR_DISTORTION = "BCR_Distortion";

        /// <summary>ch:动态模式下 印刷质量开关, 默认值0, 范围[0,2147483646] | en:Printing quality switch in dynamic mode, Range: [0, 2147483646], default: 0</summary>
        public const string KEY_BCR_WHITEGAP = "BCR_WhiteGap";

        /// <summary>ch:动态模式下 镜面反光开关, 默认值0, 范围[0,2147483646] | en:Specular reflective switch in dynamic mode. Range: [0, 2147483646], default: 0</summary>
        public const string KEY_BCR_SPOT = "BCR_Spot";

        /// <summary>ch:图像采样尺度, 默认值1, 范围[1,8] | en:Image sampling scale. Range: [1, 8], default: 1</summary>
        public const string KEY_BCR_SAMPLELEVEL = "BCR_SampleLevel";

        /// <summary>ch:图像形态学预处理, 默认值0, 范围[0, 2] | en:Image morphology preprocessing. Range: [0, 2], default: 0</summary>
        public const string KEY_BCR_IAMGEMORPH = "BCR_ImageMorph";

        /// <summary>ch:降错识开关, 默认值1, 范围[0,1] | en:Lower error identification switch. Range: [0, 1], default: 1</summary>
        public const string KEY_BCR_DELERRFLAG = "BCR_DelErrFlag";

        /// <summary>ch:算法能力集, 含QR[1], DM[2], 默认值3, 范围[0,3] | en:Algorithm capability set.contain:QR[1], DM[2], Range: [0, 3], default: 3</summary>
        public const string KEY_TDCR_ABILITY = "TDCR_Ability";

        /// <summary>ch:图像ROI X方向偏移, 默认值0, 范围[0,65535] | en:Image ROI X direction offset,Range: [0,65535], default: 0</summary>
        public const string KEY_TDCR_ROI_X = "TDCR_PositionXROI";

        /// <summary>ch:图像ROI Y方向偏移, 默认值0, 范围[0,65535] | en:Image ROI Y direction offset,Range: [0,65535], default: 0</summary>
        public const string KEY_TDCR_ROI_Y = "TDCR_PositionYROI";

        /// <summary>ch:图像ROI宽度, 默认值65535, 范围[128,65535] | en:Image ROI width. Range: [128,65535], default: 65535</summary>
        public const string KEY_TDCR_ROI_WIDTH = "TDCR_WidthROI";

        /// <summary>ch:图像ROI高度, 默认值65535, 范围[128,65535] | en:Image ROI height. Range: [128,65535], default: 65535</summary>
        public const string KEY_TDCR_ROI_HEIGHT = "TDCR_HeightROI";

        /// <summary>ch:算法最大宽度, 默认3840, 范围[0,65535] | en:Maximum width of the algorithm. Range: [0,65535], default: 3840</summary>
        public const string KEY_TDCR_MAX_WIDTH = "TDCR_MaxWidth";

        /// <summary>ch:算法最大高度, 默认2748, 范围[0,65535] | en:Maximum height of the algorithm. Range: [0,65535], default: 2748</summary>
        public const string KEY_TDCR_MAX_HEIGHT = "TDCR_MaxHeight";

        /// <summary>ch:检测模块输出ROI个数, 默认值5, 范围[1,1000] | en:The number of ROIs outputted by detection module. Range: [1, 1000], default: 5</summary>
        public const string KEY_TDCR_LOCCODENUM = "TDCR_LocCodeNum";

        /// <summary>ch:blob筛选时, 最小宽高, 默认值40, 范围[20,1000] | en:The minimum width and height when filtering blobs. Range: [20, 1000], default: 40</summary>
        public const string KEY_TDCR_MINBARSIZE = "TDCR_MinBarSize";

        /// <summary>ch:blob筛选时, 最大宽高, 默认值300, 范围[20,1000] | en:The maximum width and height when filtering blobs. Range: [20, 1000], default: 300</summary>
        public const string KEY_TDCR_MAXBARSIZE = "TDCR_MaxBarSize";

        /// <summary>ch:镜像模式是否打开, 默认值0, 范围[0,2] | en:Whether to enable mirror mode. Range: [0, 2], default: 0</summary>
        public const string KEY_TDCR_MIRRORMODE = "TDCR_MirrorMode";

        /// <summary>ch:图像降采样倍数, 默认值1, 范围[1,8] | en:Image downsampling ratio. Range: [1, 8], default: 1</summary>
        public const string KEY_TDCR_SAMPLELEVEL = "TDCR_SampleLevel";

        /// <summary>ch:白底黒码标识, 默认值0, 范围[0,2] | en:Identifier of the black bar code on white background. Range: [0, 2], default: 0</summary>
        public const string KEY_TDCR_CODECOLOR = "TDCR_CodeColor";

        /// <summary>ch:连续与离散码标志, 0-连续码 1-离散码, 默认值0, 范围[0,2] | en:Code flag: "0"-continuous code, "1"-discrete code, "2"-self-adaptive Range: [0, 2], default: 0</summary>
        public const string KEY_TDCR_DISCRETEFLAG = "TDCR_DiscreteFlag";

        /// <summary>ch:QR畸变配置参数, 默认值0, 范围[0,1] | en:QR distortion configuration parameter. Range: [0, 1], default: 0</summary>
        public const string KEY_TDCR_DISTORTIONFLAG = "TDCR_DistortionFlag";

        /// <summary>ch:高级参数, 默认值0, 范围[0,2147483640] | en:Advanced parameters. Range: [0, 2147483640], default: 0</summary>
        public const string KEY_TDCR_ADVANCEPARAM = "TDCR_AdvanceParam";

        /// <summary>ch:高级参数2, 默认值0, 范围[0,2147483640] | en:Advanced parameters 2. Range: [0, 2147483640], default: 0</summary>
        public const string KEY_TDCR_ADVANCEPARAM2 = "TDCR_AdvanceParam2";

        /// <summary>ch:超时退出时间, 默认值1000, 范围[0,5000] | en:Timeout exit time. Range: [0, 5000], default: 1000</summary>
        public const string KEY_TDCR_WAITINGTIME = "TDCR_WaitingTime";

        /// <summary>ch:debug信息是否打开, 默认值0, 范围[0,1] | en:Whether to enable debug information. Range: [0, 1], default: 0</summary>
        public const string KEY_TDCR_DEBUGFLAG = "TDCR_DebugFlag";

        /// <summary>ch:dm正方形长方形码类型, 0 正方形 1 长方形 2 兼容模式, 默认值0, 范围[0,2] | en:Code types: 0-sqaure, 1-rectangle, 2-compatible mode. Range: [0, 2], default: 0</summary>
        public const string KEY_TDCR_RECTANGLE = "TDCR_Rectangle";

        /// <summary>ch:算法库运行模式（普通模式/专业模式/极速模式）, 默认值0, 范围[0,2] | en:Algorithm library operation mode</summary>
        public const string KEY_TDCR_APPMODE = "TDCR_AppMode";

        /// <summary>ch:算法能力集, 含面单提取[1], 图像增强[2], 码提取[4], 默认7, 范围[1,7] | en:Algorithm capability set. contain:Waybill [1], image enhancement [2], code extraction [4]. Range: [1, 7], default: 7</summary>
        public const string KEY_WAYBILL_ABILITY = "WAYBILL_Ability";

        /// <summary>ch:算法最大宽度, 默认3840, 范围[0,65535] | en:Maximum width of the algorithm. Range: [0,65535], default: 3840</summary>
        public const string KEY_WAYBILL_MAX_WIDTH = "WAYBILL_Max_Width";

        /// <summary>ch:算法最大高度, 默认2748, 范围[0,65535] | en:Maximum height of the algorithm. Range: [0,65535], default: 2748</summary>
        public const string KEY_WAYBILL_MAX_HEIGHT = "WAYBILL_Max_Height";

        /// <summary>ch:面单抠图输出的图片格式, 默认Jpg, 范围[1,3],1为Mono8, 2为Jpg, 3为BMP | en:Waybill Image format of the image output, 1-Mono8,2-Jpg,3-BMP,Range: [1,3], default: jpg</summary>
        public const string KEY_WAYBILL_OUTPUTIMAGETYPE = "WAYBILL_OutputImageType";

        /// <summary>ch:jpg编码质量, 默认80, 范围[1,100] | en:Jpp encoding quality. Range: [1,100], default: 80</summary>
        public const string KEY_WAYBILL_JPGQUALITY = "WAYBILL_JpgQuality";

        /// <summary>ch:waybill最小宽, 宽是长边, 高是短边, 默认100, 范围[15,2592] | en:Minimum width of waybill. Range: [15, 2592], default: 100</summary>
        public const string KEY_WAYBILL_MINWIDTH = "WAYBILL_MinWidth";

        /// <summary>ch:waybill最小高, 默认100, 范围[10,2048] | en:Minimum height of waybill. Range: [10, 2048], default: 100</summary>
        public const string KEY_WAYBILL_MINHEIGHT = "WAYBILL_MinHeight";

        /// <summary>ch:waybill最大宽, 宽是长边, 高是短边, 默认3072, 范围[15,3072] | en:Maximum width of waybill. Range: [15, 3072], default: 3072</summary>
        public const string KEY_WAYBILL_MAXWIDTH = "WAYBILL_MaxWidth";

        /// <summary>ch:waybill最大高, 默认2048, 范围[10,2048] | en:Maximum height of waybill. Range: [10, 2048], default: 2048</summary>
        public const string KEY_WAYBILL_MAXHEIGHT = "WAYBILL_MaxHeight";

        /// <summary>ch:膨胀次数, 默认0, 范围[0,10] | en:Expansion times. Range: [0, 10], default: 0</summary>
        public const string KEY_WAYBILL_MORPHTIMES = "WAYBILL_MorphTimes";

        /// <summary>ch:面单上条码和字符灰度最小值, 默认0, 范围[0,255] | en:Minimum gray value of the bar code and character gray on the waybill. Range: [0, 255], default: 0</summary>
        public const string KEY_WAYBILL_GRAYLOW = "WAYBILL_GrayLow";

        /// <summary>ch:面单上灰度中间值, 用于区分条码和背景, 默认70, 范围[0,255] | en:Median gray value of waybill which is used to distinguish barcode from background. Range: [0, 255], default: 70</summary>
        public const string KEY_WAYBILL_GRAYMID = "WAYBILL_GrayMid";

        /// <summary>ch:面单上背景灰度最大值, 默认130, 范围[0,255] | en:Maximum gray value of waybill background. Range: [0, 255], default: 130</summary>
        public const string KEY_WAYBILL_GRAYHIGH = "WAYBILL_GrayHigh";

        /// <summary>ch:自适应二值化, 默认1, 范围[0,1] | en:Adaptive binarization. Range: [0, 1], default: 1</summary>
        public const string KEY_WAYBILL_BINARYADAPTIVE = "WAYBILL_BinaryAdaptive";

        /// <summary>ch:面单抠图行方向扩边, 默认0, 范围[0,2000] | en:Expand the edge in row direction when matting waybill. Range: [0, 2000], default: 0</summary>
        public const string KEY_WAYBILL_BOUNDARYROW = "WAYBILL_BoundaryRow";

        /// <summary>ch:面单抠图列方向扩边, 默认0, 范围[0,2000] | en:Expand the edge in column direction when matting waybill. Range: [0, 2000], default: 0</summary>
        public const string KEY_WAYBILL_BOUNDARYCOL = "WAYBILL_BoundaryCol";

        /// <summary>ch:最大面单和条码高度比例, 默认20, 范围[1,100] | en:Maximum height ratio of waybill to barcode. Range: [1, 100], default: 20</summary>
        public const string KEY_WAYBILL_MAXBILLBARHEIGTHRATIO = "WAYBILL_MaxBillBarHightRatio";

        /// <summary>ch:最大面单和条码宽度比例, 默认5, 范围[1,100] | en:Maximum width ratio of waybill to barcode. Range: [1, 100], default: 5</summary>
        public const string KEY_WAYBILL_MAXBILLBARWIDTHRATIO = "WAYBILL_MaxBillBarWidthRatio";

        /// <summary>ch:最小面单和条码高度比例, 默认5, 范围[1,100] | en:Minimum height ratio of waybill to barcode. Range: [1, 100], default: 5</summary>
        public const string KEY_WAYBILL_MINBILLBARHEIGTHRATIO = "WAYBILL_MinBillBarHightRatio";

        /// <summary>ch:最小面单和条码宽度比例, 默认2, 范围[1,100] | en:Minimum width ratio of waybill to barcode. Range: [1, 100], default: 2</summary>
        public const string KEY_WAYBILL_MINBILLBARWIDTHRATIO = "WAYBILL_MinBillBarWidthRatio";

        /// <summary>ch:增强方法, 默认2, 范围[1,4] | en:Enhancement method. Range: [1, 4], default: 2</summary>
        public const string KEY_WAYBILL_ENHANCEMETHOD = "WAYBILL_EnhanceMethod";

        /// <summary>ch:增强拉伸低阈值比例, 默认1, 范围[0,100] | en:Enhance the low threshold ratio of stretching. Range: [0, 100], default: 1</summary>
        public const string KEY_WAYBILL_ENHANCECLIPRATIOLOW = "WAYBILL_ClipRatioLow";

        /// <summary>ch:增强拉伸高阈值比例, 默认99, 范围[0,100] | en:Enhance the high threshold ratio of stretching. Range: [0, 100], default: 99</summary>
        public const string KEY_WAYBILL_ENHANCECLIPRATIOHIGH = "WAYBILL_ClipRatioHigh";

        /// <summary>ch:对比度系数, 默认100, 范围[1,10000] | en:Contrast ratio. Range: [1, 10000], default: 100</summary>
        public const string KEY_WAYBILL_ENHANCECONTRASTFACTOR = "WAYBILL_ContrastFactor";

        /// <summary>ch:锐化系数, 默认0, 范围[0,10000] | en:Sharpness. Range: [0,10000], default: 0</summary>
        public const string KEY_WAYBILL_ENHANCESHARPENFACTOR = "WAYBILL_SharpenFactor";

        /// <summary>ch:锐化滤波核大小, 默认3, 范围[3,15] | en:Size of sharpening filter core. Range: [3, 15], default: 3</summary>
        public const string KEY_WAYBILL_SHARPENKERNELSIZE = "WAYBILL_KernelSize";

        /// <summary>ch:码单抠图行方向扩边, 默认0, 范围[0,2000] | en:Expand the edge in row direction when matting weight memo. Range: [0, 2000], default: 0</summary>
        public const string KEY_WAYBILL_CODEBOUNDARYROW = "WAYBILL_CodeBoundaryRow";

        /// <summary>ch:码单抠图列方向扩边, 默认0, 范围[0,2000] | en:Expand the edge in column direction when matting weight memo. Range: [0, 2000], default: 0</summary>
        public const string KEY_WAYBILL_CODEBOUNDARYCOL = "WAYBILL_CodeBoundaryCol";

        private IntPtr handle;

        /// <summary>
        /// ch:构造函数 | en:Constructor
        /// </summary>
        public MVIDCodeReader()
        {
            handle = IntPtr.Zero;
        }

        /// <summary>
        /// ch:析构函数 | en:Destructor
        /// </summary>
        ~MVIDCodeReader()
        {
            MVID_CR_DestroyHandle_NET();
        }

        /// <summary>
        /// ch:获取SDK版本号 | en:Get SDK Version
        /// </summary>
        /// <param name="pstrVersion">ch:SDK版本号 | en:SDK Version</param>
        /// <returns>ch:返回SDK, 一维码, 二维码版本号 | en:SDK Version</returns>
        public static int MVID_CR_GetVersion_NET(IntPtr pstrVersion)
        {
            return MVID_CR_GetVersion(pstrVersion);
        }

        /// <summary>
        /// ch:枚举设备 | en:Enumerate Device
        /// </summary>
        /// <param name="pstCamList">ch:设备列表 | en:Device List</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public static int MVID_CR_CAM_EnumDevices_NET(ref MVID_CAMERA_INFO_LIST pstCamList)
        {
            return MVID_CR_CAM_EnumDevices(ref pstCamList);
        }

        /// <summary>
        /// ch:根据配置文件枚举指定设备 | en:Enumerate Specified Series Device
        /// </summary>
        /// <param name="pstCamList">ch:设备列表 | en:Device List</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public static int MVID_CR_CAM_EnumDevicesByCfg_NET(ref MVID_CAMERA_INFO_LIST pstCamList)
        {
            return MVID_CR_CAM_EnumDevicesByCfg(ref pstCamList);
        }

        /// <summary>
        /// ch:创建句柄 | en:Create Handle
        /// </summary>
        /// <param name="nCodeAbility">ch:读码能力集 | en:Code Ability</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CreateHandle_NET(uint nCodeAbility)
        {
            if (IntPtr.Zero != handle)
            {
                MVID_CR_DestroyHandle(handle);
                handle = IntPtr.Zero;
            }
            return MVID_CR_CreateHandle(ref handle, nCodeAbility);
        }

        /// <summary>
        /// ch:销毁句柄 | en:Destroy Handle
        /// </summary>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_DestroyHandle_NET()
        {
            int result = MVID_CR_DestroyHandle(handle);
            handle = IntPtr.Zero;
            return result;
        }

        /// <summary>
        /// ch:设置算法整型参数 | en:Set Algorithm Integer Value
        /// </summary>
        /// <param name="strParamKeyName">ch:属性键值 | en:Param KeyName</param>
        /// <param name="nValue">ch:参数值 | en:Value</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_Algorithm_SetIntValue_NET(string strParamKeyName, int nValue)
        {
            return MVID_CR_Algorithm_SetIntValue(handle, strParamKeyName, nValue);
        }

        /// <summary>
        /// ch:获取算法整型参数 | en:Get Algorithm Integer Value
        /// </summary>
        /// <param name="strParamKeyName">ch:属性键值 | en:Param KeyName</param>
        /// <param name="pnValue">ch:参数值 | en:Value</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_Algorithm_GetIntValue_NET(string strParamKeyName, ref int pnValue)
        {
            return MVID_CR_Algorithm_GetIntValue(handle, strParamKeyName, ref pnValue);
        }

        /// <summary>
        /// ch:设置算法浮点型参数 | en:Set Algorithm Float Value
        /// </summary>
        /// <param name="strParamKeyName">ch:属性键值 | en:Param KeyName</param>
        /// <param name="fValue">ch:参数值 | en:Value</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_Algorithm_SetFloatValue_NET(string strParamKeyName, float fValue)
        {
            return MVID_CR_Algorithm_SetFloatValue(handle, strParamKeyName, fValue);
        }

        /// <summary>
        /// ch:获取算法浮点型参数 | en:Get Algorithm Float Value
        /// </summary>
        /// <param name="strParamKeyName">ch:属性键值 | en:Param KeyName</param>
        /// <param name="pfValue">ch:参数值 | en:Value</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_Algorithm_GetFloatValue_NET(string strParamKeyName, ref float pfValue)
        {
            return MVID_CR_Algorithm_GetFloatValue(handle, strParamKeyName, ref pfValue);
        }

        /// <summary>
        /// ch:设置算法字符串型参数 | en:Set Algorithm String Value
        /// </summary>
        /// <param name="strParamKeyName">ch:属性键值 | en:Param KeyName</param>
        /// <param name="strValue">ch:参数值 | en:Value</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_Algorithm_SetStringValue_NET(string strParamKeyName, string strValue)
        {
            return MVID_CR_Algorithm_SetStringValue(handle, strParamKeyName, strValue);
        }

        /// <summary>
        /// ch:获取算法字符串型参数 | en:Get Algorithm String Value
        /// </summary>
        /// <param name="strParamKeyName">ch:属性键值 | en:Param KeyName</param>
        /// <param name="strValue">ch:参数值 | en:Value</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_Algorithm_GetStringValue_NET(string strParamKeyName, ref byte strValue)
        {
            return MVID_CR_Algorithm_GetStringValue(handle, strParamKeyName, ref strValue);
        }

        /// <summary>
        /// ch:读码流程 | en:Process
        /// </summary>
        /// <param name="pstParam">ch:图片信息 | en:Param of input Image</param>
        /// <param name="nCodeAbility">ch:读码能力集 | en:Code Ability</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_Process_NET(IntPtr pstParam, uint nCodeAbility)
        {
            return MVID_CR_Process(handle, pstParam, nCodeAbility);
        }

        /// <summary>
        /// ch:绑定设备 | en:Bind Device
        /// </summary>
        /// <param name="pstCamInfo">ch:设备信息 | en:Camera Information</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_BindDevice_NET(IntPtr pstCamInfo)
        {
            return MVID_CR_CAM_BindDevice(handle, pstCamInfo);
        }

        /// <summary>
        /// ch:通过IP绑定设备 | en:Bind Device By IP
        /// </summary>
        /// <param name="chCurrentIp">ch:相机IP | en:Camera IP</param>
        /// <param name="chNetExport">ch:当前PC IP | en:Current PC IP</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_BindDeviceByIP_NET(string chCurrentIp, string chNetExport)
        {
            return MVID_CR_CAM_BindDeviceByIP(handle, chCurrentIp, chNetExport);
        }

        /// <summary>
        /// ch:通过序列号绑定设备 | en:Bind Device By SerialNumber
        /// </summary>
        /// <param name="chSerialNumber">ch:相机序列号 | en:Camera Serial Number</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_BindDeviceBySerialNumber_NET(string chSerialNumber)
        {
            return MVID_CR_CAM_BindDeviceBySerialNumber(handle, chSerialNumber);
        }

        /// <summary>
        /// ch:注册图像数据回调, 包含解码信息 | en:Register image data callback, include barcode info
        /// </summary>
        /// <param name="cbOutput">ch:回调函数指针 | en:Callback function pointer</param>
        /// <param name="pUser">ch:用户自定义变量 | en:User defined variable</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_RegisterImageCallBack_NET(cbOutputdelegate cbOutput, IntPtr pUser)
        {
            return MVID_CR_CAM_RegisterImageCallBack(handle, cbOutput, pUser);
        }

        /// <summary>
        /// ch:注册图像数据回调, 不包含解码信息 | en:Register image data callback, without barcode info
        /// </summary>
        /// <param name="cbOutput">ch:回调函数指针 | en:Callback function pointer</param>
        /// <param name="pUser">ch:用户自定义变量 | en:User defined variable</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_RegisterImageBufferCallBack_NET(cbImageBufferdelegate cbOutput, IntPtr pUser)
        {
            return MVID_CR_CAM_RegisterImageBufferCallBack(handle, cbOutput, pUser);
        }

        /// <summary>
        /// ch:注册全部事件回调, 在打开设备之后调用,只支持GIGE | en:Register event callback, which is called after the device is opened
        /// </summary>
        /// <param name="cbEvent">ch:用户注册事件回调函数 | en:Exception CallBack Function Pointer</param>
        /// <param name="pUser">ch:用户自定义变量 | en:User defined variable</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_RegisterAllEventCallBack_NET(cbEventdelegate cbEvent, IntPtr pUser)
        {
            return MVID_CR_CAM_RegisterAllEventCallBack(handle, cbEvent, pUser);
        }

        /// <summary>
        /// ch:开始取流 | en:Start Grabbing
        /// </summary>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_StartGrabbing_NET()
        {
            return MVID_CR_CAM_StartGrabbing(handle);
        }

        /// <summary>
        /// ch:停止取流 | en:Stop Grabbing
        /// </summary>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_StopGrabbing_NET()
        {
            return MVID_CR_CAM_StopGrabbing(handle);
        }

        /// <summary>
        /// ch:采用超时机制获取一帧图片, SDK内部等待直到有数据时返回, 包含解码信息 | en:Timeout mechanism is used to get image, and the SDK waits inside until the data is returned, include barcode info
        /// </summary>
        /// <param name="pFrameInfo">ch:图像信息 | en:Image information pointer</param>
        /// <param name="nMsec">ch:等待超时时间 | en:Waiting timeout</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_GetOneFrameTimeout_NET(IntPtr pFrameInfo, uint nMsec)
        {
            return MVID_CR_CAM_GetOneFrameTimeout(handle, pFrameInfo, nMsec);
        }

        /// <summary>
        /// ch:采用超时机制获取一帧图片, SDK内部等待直到有数据时返回, 不包含解码信息 | en:Timeout mechanism is used to get image, and the SDK waits inside until the data is returned, without barcode info
        /// </summary>
        /// <param name="pFrameInfo">ch:图像信息 | en:Image information structure</param>
        /// <param name="nMsec">ch:等待超时时间 | en:Waiting timeout</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_GetImageBuffer_NET(ref MVID_IMAGE_INFO pFrameInfo, uint nMsec)
        {
            return MVID_CR_CAM_GetImageBuffer(handle, ref pFrameInfo, nMsec);
        }

        /// <summary>
        /// ch:设置相机Int型属性值 | en:Set Camera Int value
        /// </summary>
        /// <param name="strKey">ch:属性键值, 如获取宽度信息则为"Width" | en:Key value, for example, using "Width" to set width</param>
        /// <param name="nValue">ch:想要设置的相机的属性值 | en:Feature value to set</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_SetIntValue_NET(string strKey, long nValue)
        {
            return MVID_CR_CAM_SetIntValue(handle, strKey, nValue);
        }

        /// <summary>
        /// ch:获取相机Int属性值 | en:Get Camera Int value
        /// </summary>
        /// <param name="strKey">ch:属性键值, 如获取宽度信息则为"Width" | en:Key value, for example, using "Width" to set width</param>
        /// <param name="pIntValue">ch:返回给调用者有关相机属性结构体 | en:Structure pointer of camera features</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_GetIntValue_NET(string strKey, ref MVID_CAM_INTVALUE_EX pIntValue)
        {
            return MVID_CR_CAM_GetIntValue(handle, strKey, ref pIntValue);
        }

        /// <summary>
        /// ch:设置相机Enum型属性值 | en:Set Camera Enum value
        /// </summary>
        /// <param name="strKey">ch:属性键值, 如获取像素格式信息则为"PixelFormat" | en:Key value, for example, using "PixelFormat" to set pixel format</param>
        /// <param name="nValue">ch:想要设置的相机的属性值 | en:Feature value to set</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_SetEnumValue_NET(string strKey, uint nValue)
        {
            return MVID_CR_CAM_SetEnumValue(handle, strKey, nValue);
        }

        /// <summary>
        /// ch:通过字符串设置相机Enum型属性值 | en:Set Camera Enum value by string
        /// </summary>
        /// <param name="strKey">ch:属性键值, 如获取像素格式信息则为"PixelFormat" | en:Key value, for example, using "PixelFormat" to set pixel format</param>
        /// <param name="sValue">ch:想要设置的相机的属性字符串 | en:Feature value to set</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_SetEnumValueByString_NET(string strKey, string sValue)
        {
            return MVID_CR_CAM_SetEnumValueByString(handle, strKey, sValue);
        }

        /// <summary>
        /// ch:获取相机Enum属性值 | en:Get Camera Enum value
        /// </summary>
        /// <param name="strKey">ch:属性键值, 如获取像素格式信息则为"PixelFormat" | en:Key value, for example, using "PixelFormat" to get pixel format</param>
        /// <param name="pEnumValue">ch:返回给调用者有关相机属性结构体 | en:Structure pointer of camera features</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_GetEnumValue_NET(string strKey, ref MVID_CAM_ENUMVALUE pEnumValue)
        {
            return MVID_CR_CAM_GetEnumValue(handle, strKey, ref pEnumValue);
        }

        /// <summary>
        /// ch:设置相机Float型属性值 | en:Set Camera Float value
        /// </summary>
        /// <param name="strKey">ch:属性键值 | en:Key value</param>
        /// <param name="fValue">ch:想要设置的相机的属性值 | en:Feature value to set</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_SetFloatValue_NET(string strKey, float fValue)
        {
            return MVID_CR_CAM_SetFloatValue(handle, strKey, fValue);
        }

        /// <summary>
        /// ch:获取相机Float属性值 | en:Get Camera Float value
        /// </summary>
        /// <param name="strKey">ch:属性键值 | en:Key value</param>
        /// <param name="pFloatValue">ch:返回给调用者有关相机属性结构体 | en:Structure pointer of camera features</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_GetFloatValue_NET(string strKey, ref MVID_CAM_FLOATVALUE pFloatValue)
        {
            return MVID_CR_CAM_GetFloatValue(handle, strKey, ref pFloatValue);
        }

        /// <summary>
        /// ch:设置相机String型属性值 | en:Set Camera String value
        /// </summary>
        /// <param name="strKey">ch:属性键值 | en:Key value</param>
        /// <param name="sValue">ch:想要设置的相机的属性值 | en:Feature value to set</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_SetStringValue_NET(string strKey, string sValue)
        {
            return MVID_CR_CAM_SetStringValue(handle, strKey, sValue);
        }

        /// <summary>
        /// ch:获取相机String属性值 | en:Get Camera String value
        /// </summary>
        /// <param name="strKey">ch:属性键值 | en:Key value</param>
        /// <param name="pStringValue">ch:返回给调用者有关相机属性结构体 | en:Structure pointer of camera features</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_GetStringValue_NET(string strKey, ref MVID_CAM_STRINGVALUE pStringValue)
        {
            return MVID_CR_CAM_GetStringValue(handle, strKey, ref pStringValue);
        }

        /// <summary>
        /// ch:设置相机Boolean型属性值 | en:Set Camera Boolean value
        /// </summary>
        /// <param name="strKey">ch:属性键值 | en:Key value</param>
        /// <param name="bValue">ch:想要设置的相机的属性值 | en:Feature value to set</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_SetBoolValue_NET(string strKey, bool bValue)
        {
            return MVID_CR_CAM_SetBoolValue(handle, strKey, bValue);
        }

        /// <summary>
        /// ch:获取相机Boolean属性值 | en:Get Camera Boolean value
        /// </summary>
        /// <param name="strKey">ch:属性键值 | en:Key value</param>
        /// <param name="pBoolValue">ch:返回给调用者有关相机属性值 | en:Structure pointer of camera features</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_GetBoolValue_NET(string strKey, ref bool pBoolValue)
        {
            return MVID_CR_CAM_GetBoolValue(handle, strKey, ref pBoolValue);
        }

        /// <summary>
        /// ch:设置相机Command型属性值 | en:Set Camera Command value
        /// </summary>
        /// <param name="strKey">ch:属性键值 | en:Key value</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_SetCommandValue_NET(string strKey)
        {
            return MVID_CR_CAM_SetCommandValue(handle, strKey);
        }

        /// <summary>
        /// ch:设置SDK内部图像缓存节点个数, 大于等于1, 在抓图前调用 | en:Set the number of the internal image cache nodes in SDK, Greater than or equal to 1, to be called before the capture
        /// </summary>
        /// <param name="nNum">ch:缓存节点个数 | en:Image Node Number</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_SetImageNodeNum_NET(uint nNum)
        {
            return MVID_CR_CAM_SetImageNodeNum(handle, nNum);
        }

        /// <summary>
        /// ch:设置图像输出模式(MVID_OUTPUT_NORMAL - 非MONO8均转换为MVID_IMAGE_BGR24 | MVID_OUTPUT_RAW - 以原始图像格式输出) | en:Set Image OutPutMode
        /// </summary>
        /// <param name="enImageOutPutMode">ch:图像输出模式 | en:Image OutPut Mode</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_SetImageOutPutMode_NET(MVID_IMAGE_OUTPUT_MODE enImageOutPutMode)
        {
            return MVID_CR_CAM_SetImageOutPutMode(handle, enImageOutPutMode);
        }

        /// <summary>
        /// ch:注册预处理图像数据回调 | en:Register pretreatment image data callback
        /// </summary>
        /// <param name="cbPreOutput">ch:回调函数指针, 预处理输入参数内存由SDK内部分配 | en:Callback function pointer, Preprocessing input parameter memory is allocated internally by the SDK</param>
        /// <param name="pUser">ch:用户自定义变量 | en:User defined variable</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_CAM_RegisterPreImageCallBack_NET(cbPreOutputdelegate cbPreOutput, IntPtr pUser)
        {
            return MVID_CR_CAM_RegisterPreImageCallBack(handle, cbPreOutput, pUser);
        }

        /// <summary>
        /// ch:注册异常消息回调, 在打开设备之后调用 | en:Register Exception Message CallBack, call after open device
        /// </summary>
        /// <param name="cbException">ch:异常回调函数指针 | en:Exception Message CallBack Function Pointer</param>
        /// <param name="pUser">ch:用户自定义变量 | en:User defined variable</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_RegisterExceptionCallBack_NET(cbExceptiondelegate cbException, IntPtr pUser)
        {
            return MVID_CR_RegisterExceptionCallBack(handle, cbException, pUser);
        }

        /// <summary>
        /// ch:保存图片, 支持Bmp和Jpeg.编码质量在50-99之间 | en:Save image, support Bmp and Jpeg. Encoding quality, [50-99]
        /// </summary>
        /// <param name="pstInputImage">ch:输入图片参数结构体,支持Mono8/BGR24格式图像 | en:Input image parameters structure</param>
        /// <param name="enImageType">ch:目标转换类型, 默认为Jpeg | en:Convery image type, default Jpeg</param>
        /// <param name="pstOutputImage">ch:输出图片参数结构体 | en:OutPut image parameters structure</param>
        /// <param name="nJpgQuality">ch:JPG压缩质量, 默认为80, 若目标转换类型为BMP则该参数无效 | en:Jpg quality, default quality 80, no use for Bmp image</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_SaveImage_NET(ref MVID_IMAGE_INFO pstInputImage, MVID_IMAGE_TYPE enImageType, ref MVID_IMAGE_INFO pstOutputImage, uint nJpgQuality)
        {
            return MVID_CR_SaveImage(handle, ref pstInputImage, enImageType, ref pstOutputImage, nJpgQuality);
        }

        /// <summary>
        /// ch:过滤规则导入 | en:Load Rule File
        /// </summary>
        /// <param name="pFileName">ch:过滤规则文件路径, 设置NULL为取消过滤 | en:FileName of Rule</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_RuleLoad_NET(string pFileName)
        {
            return MVID_CR_RuleLoad(handle, pFileName);
        }

        /// <summary>
        /// ch:脚本文件导入 | en:Load Script File
        /// </summary>
        /// <param name="pFilePath">ch:脚本文件路径, 设置NULL为取消过滤 | en:FilePath of Script</param>
        /// <param name="pFuncName">ch:过滤函数名称 | en:Filter Function Name</param>
        /// <returns>ch:成功, 返回MVID_CR_OK; 错误, 返回错误码 | en:Success, return MVID_CR_OK. Failure, return error code</returns>
        public int MVID_CR_ScriptLoad_NET(string pFilePath, string pFuncName)
        {
            return MVID_CR_ScriptLoad(handle, pFilePath, pFuncName);
        }

        /// <summary>
        /// Byte array to struct
        /// </summary>
        /// <param name="bytes">Byte array</param>
        /// <param name="type">Struct type</param>
        /// <returns>Struct object</returns>
        public static object ByteToStruct(byte[] bytes, Type type)
        {
            int num = Marshal.SizeOf(type);
            if (num > bytes.Length)
            {
                return null;
            }
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.Copy(bytes, 0, intPtr, num);
            object result = Marshal.PtrToStructure(intPtr, type);
            Marshal.FreeHGlobal(intPtr);
            return result;
        }

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_GetVersion(IntPtr pstrVersion);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CreateHandle(ref IntPtr handle, uint nCodeAbility);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_DestroyHandle(IntPtr handle);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_Algorithm_SetIntValue(IntPtr handle, string strParamKeyName, int nValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_Algorithm_GetIntValue(IntPtr handle, string strParamKeyName, ref int pnValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_Algorithm_SetFloatValue(IntPtr handle, string strParamKeyName, float fValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_Algorithm_GetFloatValue(IntPtr handle, string strParamKeyName, ref float pfValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_Algorithm_SetStringValue(IntPtr handle, string strParamKeyName, string strValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_Algorithm_GetStringValue(IntPtr handle, string strParamKeyName, ref byte strValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_Process(IntPtr handle, IntPtr pstParam, uint nCodeAbility);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_EnumDevices(ref MVID_CAMERA_INFO_LIST pstCamList);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_EnumDevicesByCfg(ref MVID_CAMERA_INFO_LIST pstCamList);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_BindDevice(IntPtr handle, IntPtr pstCamInfo);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_BindDeviceByIP(IntPtr handle, string chCurrentIp, string chNetExport);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_BindDeviceBySerialNumber(IntPtr handle, string chSerialNumber);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_RegisterImageCallBack(IntPtr handle, cbOutputdelegate cbOutput, IntPtr pUser);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_RegisterImageBufferCallBack(IntPtr handle, cbImageBufferdelegate cbOutput, IntPtr pUser);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_RegisterAllEventCallBack(IntPtr handle, cbEventdelegate cbEvent, IntPtr pUser);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_StartGrabbing(IntPtr handle);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_StopGrabbing(IntPtr handle);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_GetOneFrameTimeout(IntPtr handle, IntPtr pFrameInfo, uint nMsec);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_GetImageBuffer(IntPtr handle, ref MVID_IMAGE_INFO pFrameInfo, uint nMsec);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_SetIntValue(IntPtr handle, string strKey, long nValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_GetIntValue(IntPtr handle, string strKey, ref MVID_CAM_INTVALUE_EX pIntValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_SetEnumValue(IntPtr handle, string strKey, uint nValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_SetEnumValueByString(IntPtr handle, string strKey, string sValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_GetEnumValue(IntPtr handle, string strKey, ref MVID_CAM_ENUMVALUE pEnumValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_SetFloatValue(IntPtr handle, string strKey, float fValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_GetFloatValue(IntPtr handle, string strKey, ref MVID_CAM_FLOATVALUE pFloatValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_SetStringValue(IntPtr handle, string strKey, string sValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_GetStringValue(IntPtr handle, string strKey, ref MVID_CAM_STRINGVALUE pStringValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_SetBoolValue(IntPtr handle, string strKey, bool bValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_GetBoolValue(IntPtr handle, string strKey, ref bool pBoolValue);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_SetCommandValue(IntPtr handle, string strKey);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_SetImageNodeNum(IntPtr handle, uint nNum);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_SetImageOutPutMode(IntPtr handle, MVID_IMAGE_OUTPUT_MODE enImageOutPutMode);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_CAM_RegisterPreImageCallBack(IntPtr handle, cbPreOutputdelegate cbPreOutput, IntPtr pUser);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_RegisterExceptionCallBack(IntPtr handle, cbExceptiondelegate cbException, IntPtr pUser);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_SaveImage(IntPtr handle, ref MVID_IMAGE_INFO pstInputImage, MVID_IMAGE_TYPE enImageType, ref MVID_IMAGE_INFO pstOutputImage, uint nJpgQuality);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_RuleLoad(IntPtr handle, string pFileName);

        [DllImport("MVIDCodeReader.dll")]
        private static extern int MVID_CR_ScriptLoad(IntPtr handle, string pFilePath, string pFuncName);
    }

}
