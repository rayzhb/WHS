using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHS.Infrastructure;
using WHS.Infrastructure.Messaging;

namespace WHS.DEVICE.ROBOTNEW.Commons
{
    internal static class MessageUtility
    {
        /// <summary>
        /// 响应访问信息
        /// </summary>
        /// <param name="messageRequest">访问的对象</param>
        /// <param name="errCode">错误代码,0正确，大于0代表错误，需要填写errText</param>
        /// <param name="errText">显示的错误信息</param>
        /// <param name="messageResults">无错误的时候，返回给客户端的对象(默认为null)</param>
        internal static void MessageResponse(MessageRequest messageRequest, int errCode, string errText, MessageResults messageResults = null)
        {
            MessageResponse res = new MessageResponse();
            res.ID = messageRequest.ID;
            res.ChannelID = messageRequest.ChannelID;
            res.Results = messageResults;
            res.Action = messageRequest.Action;
            res.errCode = errCode;
            res.errText = errText;
            EnvironmentManager.Instance.PostResponseMessage(res);
        }

        /// <summary>
        /// 回调发送给所有注册过的信道
        /// </summary>
        /// <param name="action">命令</param>
        /// <param name="source">来源</param>
        /// <param name="obj">回调给客户端的信息</param>
        internal static void MessageCallBackAll(string action, string source, object obj)
        {
            Task.Run(() =>
            {
                foreach (var channel in RobotNewPluginDefinition.sConnectChannels)
                {
                    MessageCallBackSingle(channel, action, source, obj);
                }
            });
        }

        /// <summary>
        /// 回调指定信道
        /// </summary>
        /// <param name="channel">信道</param>
        /// <param name="action">命令</param>
        /// <param name="source">来源</param>
        /// <param name="obj">回调给客户端的信息</param>
        internal static void MessageCallBackSingle(string channel, string action, string source, object obj)
        {
            MessageCallback callback = new MessageCallback();
            callback.ID = Guid.NewGuid().ToString();
            callback.ChannelID = channel;
            callback.Action = action;
            callback.Results = new MessageResults();
            callback.Results.Source = source;
            callback.Results.Result = obj;
            callback.errCode = 0;
            callback.errText = String.Empty;
            EnvironmentManager.Instance.PostCallbackMessage(callback);

        }
    }
}
