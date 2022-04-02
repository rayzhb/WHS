using System;
namespace WHS.DEVICE.WEIGHT.EWeight
{
    public class EWClassFactory
    {
        public static IEWeight GetEWType()
        {
            //IUserConfig iuc = GlobalContext.AppContainer.Resolve<IUserConfig>();
            //string EWType = iuc.GetValue("EWType");
            //if (string.IsNullOrEmpty(EWType))
            //{
            //    EWType = ConfigurationUtil.GetAppSettingString("EWType", "YaoHua");
            //    if (!string.IsNullOrEmpty(EWType) && !iuc.GetAllValues().ContainsKey("EWType"))
            //    {
            //        bool ewtype = iuc.AddUpdate("EWType", EWType);
            //    }
            //}

            //return WeightbrigeProviderFacotry.CreateEWeighbrige(EWType);
            throw new NotImplementedException();
        }
    }
}
