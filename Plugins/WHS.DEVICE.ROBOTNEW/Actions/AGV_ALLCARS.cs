using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHS.DEVICE.ROBOTNEW.Commons;
using WHS.DEVICE.ROBOTNEW.ViewModels;
using WHS.Infrastructure;
using WHS.Infrastructure.Action;
using WHS.Infrastructure.Messaging;

namespace WHS.DEVICE.ROBOTNEW.Actions
{
    public class AGV_ALLCARS : ActionBase
    {
        public override string Name
        {
            get
            {
                return "agv_allcars";
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
            RobotNewViewModel vm = (RobotNewViewModel)GlobalContext.SimpleContainer.GetInstance(typeof(RobotNewViewModel), null);

            var allcars = vm.agvViewModelItemsSource.ToList();

            MessageResults mr = new MessageResults();
            mr.Source = "agv_allcars";
            mr.Result = allcars;
            MessageUtility.MessageResponse(messageRequest, 0, "", mr);
        }
    }
}
