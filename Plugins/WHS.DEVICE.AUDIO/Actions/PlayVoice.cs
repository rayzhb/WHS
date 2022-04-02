using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHS.DEVICE.AUDIO.Until;
using WHS.Infrastructure;
using WHS.Infrastructure.Action;
using WHS.Infrastructure.Messaging;

namespace WHS.DEVICE.AUDIO.Actions
{
    public class PlayVoice: ActionBase
    {
        public override string Name
        {
            get
            {
                return "Play_Voice";
            }
        }

        public override  async void ExecuteAction(MessageRequest messageRequest)
        {
            await Task.Run(() =>
            {
                try
                {
                    SoundPlayers.PlayMain(messageRequest.Params.ToString());
                    messageResponse(messageRequest,0, "");
                }
                catch (Exception ex)
                {
                    messageResponse(messageRequest, 1, ex.Message);
                }
            });
        }
        private void messageResponse(MessageRequest messageRequest, int errCode, string errText)
        {
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
