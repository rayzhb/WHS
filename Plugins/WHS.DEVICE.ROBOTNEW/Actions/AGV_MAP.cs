using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WHS.DEVICE.ROBOTNEW.Commons;
using WHS.DEVICE.ROBOTNEW.Models;
using WHS.DEVICE.ROBOTNEW.ViewModels;
using WHS.Infrastructure;
using WHS.Infrastructure.Action;
using WHS.Infrastructure.Messaging;
using WHS.Infrastructure.Utils;

namespace WHS.DEVICE.ROBOTNEW.Actions
{
    public class AGV_MAP : ActionBase
    {
        public override string Name
        {
            get
            {
                return "agv_map";
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
            var model = JsonConvert.DeserializeObject<REQ_AGVMap>(messageRequest.Params.ToString());
            if (model != null)
            {
                RobotNewViewModel vm = (RobotNewViewModel)GlobalContext.SimpleContainer.GetInstance(typeof(RobotNewViewModel), null);
                var floormap = vm.agvPlatformModels.Where(w => w.Floor == model.Floor).FirstOrDefault();
                if (floormap != null)
                {
                    var resMap = floormap.ConvertToRESMapModel();
                    resMap.TotalFloor = vm.agvPlatformModels.Count;


                    //var json = System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    //{
                    //    return JsonUtility.ObjectToJson2(resMap, false, true, Newtonsoft.Json.Formatting.None);
                    //});


                    MessageResults mr = new MessageResults();
                    mr.Source = "agv_map";
                    mr.Result = resMap;
                    MessageUtility.MessageResponse(messageRequest, 0, "", mr);
                }
                else
                {
                    MessageUtility.MessageResponse(messageRequest, 4, "查无此楼层信息");
                }


            }
            else
            {
                MessageUtility.MessageResponse(messageRequest, 51, "传输的JSON格式不正确");
            }
        }
    }
}
