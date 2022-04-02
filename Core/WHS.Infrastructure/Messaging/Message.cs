using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Messaging
{
    [Serializable]
    public abstract class Message
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public string ID { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public abstract RequestType RequestType { get; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual string MessageType { get; protected set; }

        /// <summary>
        /// 执行的动作
        /// </summary>
        public string Action { get; set; }
    }

    /// <summary>
    /// 访问消息
    /// </summary>
    [Serializable]
    public class MessageRequest : Message
    {
        public MessageRequest()
        {
            base.MessageType = MessageTypeDefine.MREQ;
        }

        [Newtonsoft.Json.JsonIgnore]
        public override RequestType RequestType
        {
            get
            {
                return RequestType.Request;
            }
        }
        /// <summary>
        /// 当前消息所属的通信客户端ID
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public string ChannelID { get; set; }

        /// <summary>
        /// 执行动作所需的参数
        /// </summary>
        public object Params { get; set; }
    }
    /// <summary>
    /// 返回消息
    /// </summary>
    [Serializable]
    public class MessageResponse : Message
    {
        public MessageResponse()
        {
            base.MessageType = MessageTypeDefine.MRSP;
        }

        [Newtonsoft.Json.JsonIgnore]
        public override RequestType RequestType
        {
            get
            {
                return RequestType.Response;
            }
        }

        /// <summary>
        /// 将当前消息返回给指定通道的客户端
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public string ChannelID { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string SourceCommand { get; set; }

        /// <summary>
        /// 错误码，0正常，其他的都是失败
        /// </summary>
        [JsonProperty(PropertyName = "errCode", NullValueHandling = NullValueHandling.Ignore)]
        public int? errCode { get; set; }

        /// <summary>
        /// 错误信息描述
        /// </summary>
        [JsonProperty(PropertyName = "errText", NullValueHandling = NullValueHandling.Ignore)]
        public string errText { get; set; }

        /// <summary>
        /// 结果信息
        /// </summary>
        [JsonProperty(PropertyName = "params", NullValueHandling = NullValueHandling.Ignore)]
        public MessageResults Results { get; set; }

    }

    /// <summary>
    /// 设备回调
    /// </summary>
    public class MessageCallback : Message
    {

        public MessageCallback()
        {
            base.MessageType = MessageTypeDefine.MCBK;
        }

        [Newtonsoft.Json.JsonIgnore]
        public override RequestType RequestType
        {
            get
            {
                return RequestType.Callback;
            }
        }
        /// <summary>
        /// 将当前消息返回给指定通道的客户端
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public string ChannelID { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string SourceCommand { get; set; }

        /// <summary>
        /// 错误码，0正常，其他的都是失败
        /// </summary>
        [JsonProperty(PropertyName = "errCode", NullValueHandling = NullValueHandling.Ignore)]
        public int? errCode { get; set; }

        /// <summary>
        /// 错误信息描述
        /// </summary>
        [JsonProperty(PropertyName = "errText", NullValueHandling = NullValueHandling.Ignore)]
        public string errText { get; set; }

        /// <summary>
        /// 结果信息
        /// </summary>
        [JsonProperty(PropertyName = "params", NullValueHandling = NullValueHandling.Ignore)]
        public MessageResults Results { get; set; }

    }

    public enum RequestType
    {
        Request,
        Response,
        Callback,
        PostUI
    }

    public delegate object MessageReceiver(Message message);

    /// <summary>
    /// 代理处理通信关闭
    /// </summary>
    /// <param name="channelid"></param>
    public delegate void HandleCommunicationClose(string channelid);
}
