using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WHS.DEVICE.ROBOTNEW.Commons;
using WHS.DEVICE.ROBOTNEW.Models;
using WHS.Infrastructure;
using WHS.Infrastructure.Action;
using WHS.Infrastructure.Messaging;

namespace WHS.DEVICE.ROBOTNEW.Actions
{
    public class AGV_REGISTER : ActionBase
    {
        public override string Name
        {
            get
            {
                return "agv_register";
            }
        }

        public override void ExecuteAction(MessageRequest messageRequest)
        {
            var model = JsonConvert.DeserializeObject<REQ_AGVRegister>(messageRequest.Params.ToString());
            if (model.UserName == "admin" && model.Password == "123456")
            {
                //判断以及存储WEBSOCKET信道ID，用于后期发送数据，或者返回数据
                if (!RobotNewPluginDefinition.sConnectChannels.Contains(messageRequest.ChannelID))
                {
                    RobotNewPluginDefinition.sConnectChannels.Add(messageRequest.ChannelID);
                }
                MessageUtility.MessageResponse(messageRequest, 0, "注册成功!");
            }
            else
            {
                MessageUtility.MessageResponse(messageRequest, 42, "注册失败,请检查用户名和密码");
            }
        }

    }
}
