using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHS.Infrastructure;
using WHS.Infrastructure.Action;
using WHS.Infrastructure.Messaging;

namespace WHS.DEVICE.WEIGHT.Actions
{
    public class WEIGHT_CLOSE : ActionBase
    {
        public override string Name
        {
            get
            {
                return "weight_close";
            }
        }

        public override void ExecuteAction(MessageRequest messageRequest)
        {
            Newtonsoft.Json.Linq.JObject j_params = messageRequest.Params as Newtonsoft.Json.Linq.JObject;
            int errCode = 0;
            string errText = string.Empty;
            try
            {
                if (WEIGHT_OPEN.currentChannelID == messageRequest.ChannelID)
                {
                    if (WEIGHT_OPEN.eWeight != null)
                    {
                        WEIGHT_OPEN.eWeight.ReadWeight -= WEIGHT_OPEN.EWeight_ReadWeight;
                        WEIGHT_OPEN.eWeight.Close();
                        WEIGHT_OPEN.eWeight.Dispose();
                        WEIGHT_OPEN.eWeight = null;
                    }
                    else
                    {
                        errCode = 2;
                        errText = "已经关闭";
                    }
                }
                else
                {
                    errCode = 3;
                    errText = $"当前设备已经被{WEIGHT_OPEN.currentChannelID}占用，无法关闭";
                }

            }
            catch (Exception ex)
            {
                errCode = 1;
                errText = "关闭称重失败";
            }

            MessageResponse res = new MessageResponse();
            res.ID = messageRequest.ID;
            res.ChannelID = messageRequest.ChannelID;
            res.Action = messageRequest.Action;
            res.errCode = errCode;
            res.errText = errText;
            EnvironmentManager.Instance.PostResponseMessage(res);
        }
    }
}
