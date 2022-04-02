using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHS.DEVICE.WEIGHT;
using WHS.DEVICE.WEIGHT.EWeight;
using WHS.DEVICE.WEIGHT.Model;
using WHS.Infrastructure;
using WHS.Infrastructure.Action;
using WHS.Infrastructure.Messaging;

namespace WHS.DEVICE.WEIGHT.Actions
{
    public class WEIGHT_OPEN : ActionBase
    {
        public override string Name
        {
            get
            {
                return "weight_open";
            }
        }

        /// <summary>
        /// 全局唯一，独占形式的打开称重
        /// </summary>
        internal static IEWeight eWeight { get; set; }

        /// <summary>
        /// 当前打开称重的通信通道ID
        /// </summary>
        internal static string currentChannelID;

        /// <summary>
        /// 当前打开称重的消息ID
        /// </summary>
        internal static string MessageID;

        internal static double CurrentWeight;

        public override void ExecuteAction(MessageRequest messageRequest)
        {
            Newtonsoft.Json.Linq.JObject j_params = messageRequest.Params as Newtonsoft.Json.Linq.JObject;
            int errCode = 0;
            string errText = string.Empty;

            if (eWeight == null)
            {
                if (WeightPluginDefinition.JsonConfig.User != null)
                {
                    var weightSettingModel = WeightPluginDefinition.JsonConfig.User.ToObject<WeightSettingModel>();
                    if (weightSettingModel != null)
                    {
                        try
                        {
                            CurrentWeight = -1;
                            currentChannelID = messageRequest.ChannelID;
                            eWeight = WeightProviderFacotry.CreateEWeight(weightSettingModel.EWType.Value);
                            eWeight.Init(weightSettingModel.PortName.Value, int.Parse(weightSettingModel.BaudRate.Value));
                            eWeight.Open();
                            eWeight.ReadWeight += EWeight_ReadWeight;
                        }
                        catch (Exception ex)
                        {
                            eWeight = null;
                            errCode = 1;
                            errText = "打开称重失败"+ ex.Message;
                        }
                    }
                    else
                    {
                        errCode = 2;
                        errText = "请配置参数";
                    }
                }
                else
                {
                    errCode = 2;
                    errText = "请配置参数";
                }
            }
            else
            {
                errCode = 3;
                errText = "称重已经打开";
            }

            MessageResponse res = new MessageResponse();
            res.ID = messageRequest.ID;
            res.ChannelID = messageRequest.ChannelID;
            res.Action = messageRequest.Action;
            res.errCode = errCode;
            res.errText = errText;
            EnvironmentManager.Instance.PostResponseMessage(res);
        }

        internal static void EWeight_ReadWeight(object sender, WeightEventArgs e)
        {
            if (CurrentWeight != e.Weight)
            {
                CurrentWeight = e.Weight;
                MessageCallback callback = new MessageCallback();
                callback.ID = MessageID;
                callback.ChannelID = currentChannelID;
                callback.Action = "weight_callback";
                callback.Results = new MessageResults();
                callback.Results.Source = "weight";
                callback.Results.Result = e.Weight;
                callback.errCode = 0;
                callback.errText = String.Empty;
                EnvironmentManager.Instance.PostCallbackMessage(callback);
            }
        }
    }
}
