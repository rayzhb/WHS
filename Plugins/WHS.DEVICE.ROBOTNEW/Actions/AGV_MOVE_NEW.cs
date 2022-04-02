using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHS.DEVICE.ROBOTNEW.Commons;
using WHS.DEVICE.ROBOTNEW.Models;
using WHS.DEVICE.ROBOTNEW.ViewModels;
using WHS.Infrastructure;
using WHS.Infrastructure.Action;
using WHS.Infrastructure.Messaging;

namespace WHS.DEVICE.ROBOTNEW.Actions
{
    public class AGV_MOVE_NEW : ActionBase
    {
        public override string Name
        {
            get
            {
                return "agv_move_new";
            }
        }

        public override void ExecuteAction(MessageRequest messageRequest)
        {
            //判断信道注册
            if (!RobotNewPluginDefinition.sConnectChannels.Contains(messageRequest.ChannelID))
            {
                MessageUtility.MessageResponse(messageRequest, 40, "信道没有注册，请先调用agv_regiter命令注册");
                return;
            }
            try
            {
                var model = JsonConvert.DeserializeObject<AGVCommandModel>(messageRequest.Params.ToString());
                if (model != null)
                {
                    Task.Run(() =>
                    {
                        RobotNewViewModel vm = (RobotNewViewModel)GlobalContext.SimpleContainer.GetInstance(typeof(RobotNewViewModel), null);
                        vm.AGVAction(model);
                    });


                    MessageUtility.MessageResponse(messageRequest, 0, "");
                }
                else
                {
                    MessageUtility.MessageResponse(messageRequest, 51, "传输的JSON格式不正确");
                }
            }
            catch (Exception ex)
            {
                MessageUtility.MessageResponse(messageRequest, 41, ex.Message);
            }
        }
    }
}
