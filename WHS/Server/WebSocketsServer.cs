using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WHS.Common;
using WHS.Infrastructure;
using WHS.Infrastructure.Messaging;
using WHS.Infrastructure.NlogEx;
using WHS.Server.Handler;

namespace WHS.Server
{
    public class WebSocketsServer
    {
        private IEventLoopGroup _bossGroup;
        private IEventLoopGroup _workGroup;
        private IChannel _bootstrapChannel;


        private object _registerResponseMessager;
        private object _registerCallbackMessager;
        private object _registerLoginMessager;
        private object _registerPingMessager;

        internal static System.Collections.Concurrent.ConcurrentDictionary<string, IChannelHandlerContext> s_onlineClients;

        private System.Threading.Timer _timer;
        private System.Threading.AutoResetEvent _autoEvent;

        public WebSocketsServer()
        {
            _autoEvent = new System.Threading.AutoResetEvent(true);
            _timer = new System.Threading.Timer(new System.Threading.TimerCallback(TimerProc), _autoEvent, 3000, 10000);
            s_onlineClients = new System.Collections.Concurrent.ConcurrentDictionary<string, IChannelHandlerContext>();
            _registerResponseMessager = EnvironmentManager.Instance.RegisterReceiver(new MessageReceiver(ResponseMessage), new MessagResponseFilter());
            _registerCallbackMessager = EnvironmentManager.Instance.RegisterReceiver(new MessageReceiver(CallbackMessage), new MessagCallbackFilter());
            _registerPingMessager = EnvironmentManager.Instance.RegisterReceiver(new MessageReceiver(ExecutePingMessage), new MessagRequesteCmdListFilter(new string[] { "ping" }));

        }

        private void TimerProc(object state)
        {
            System.Threading.AutoResetEvent autoEvent = (System.Threading.AutoResetEvent)state;
            autoEvent.WaitOne();
            autoEvent.Reset();
            try
            {

                var list = s_onlineClients.ToList();
                foreach (var item in list)
                {
                    item.Value.WriteAsync(new PingWebSocketFrame());
                }
            }
            catch
            {

            }
            finally
            {

            }

            autoEvent.Set();
        }


        public async Task RunServerAsync()
        {
            bool useLibuv = ServerSettings.UseLibuv;
            Console.WriteLine("Transport type : " + (useLibuv ? "Libuv" : "Socket"));

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            }

            if (useLibuv)
            {
                var dispatcher = new DispatcherEventLoopGroup();
                _bossGroup = dispatcher;
                _workGroup = new WorkerEventLoopGroup(dispatcher);
            }
            else
            {
                _bossGroup = new MultithreadEventLoopGroup(1);
                _workGroup = new MultithreadEventLoopGroup();
            }

            X509Certificate2 tlsCertificate = null;
            //if (ServerSettings.IsSsl)
            //{
            //    tlsCertificate = new X509Certificate2(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dotnetty.com.pfx"), "password");
            //}
            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap.Group(_bossGroup, _workGroup);

                if (useLibuv)
                {
                    bootstrap.Channel<TcpServerChannel>();
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                        || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        bootstrap
                            .Option(ChannelOption.SoReuseport, true)
                            .ChildOption(ChannelOption.SoReuseaddr, true);
                    }
                }
                else
                {
                    bootstrap.Channel<TcpServerSocketChannel>();
                }

                bootstrap
                    .Option(ChannelOption.SoBacklog, 8192)
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        if (tlsCertificate != null)
                        {
                            pipeline.AddLast(TlsHandler.Server(tlsCertificate));
                        }
                        pipeline.AddLast(new HttpServerCodec());
                        pipeline.AddLast(new HttpObjectAggregator(65536));
                        pipeline.AddLast(new WebSocketServerHandler());
                    }));

                int port = ServerSettings.Port;
                _bootstrapChannel = await bootstrap.BindAsync(IPAddress.Any, port);
                LogUtil.Info("打开你的浏览器输入: "
                    + $"{(ServerSettings.IsSsl ? "https" : "http")}"
                    + $"://127.0.0.1:{port}/");
                LogUtil.Info("通信监听 "
                    + $"{(ServerSettings.IsSsl ? "wss" : "ws")}"
                    + $"://127.0.0.1:{port}/websocket");
            }
            catch
            {
                _workGroup.ShutdownGracefullyAsync().Wait();
                _bossGroup.ShutdownGracefullyAsync().Wait();
            }
        }

        private object ResponseMessage(Message message)
        {
            var response = message as MessageResponse;
            if (response != null)
            {

                string json = System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(response);
                });
                //string json = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                //LogUtil.Info("ResponseMessage=>" + json);

                if (s_onlineClients.ContainsKey(response.ChannelID))
                {
                    var text = new TextWebSocketFrame(json);
                    s_onlineClients[response.ChannelID].Channel.WriteAndFlushAsync(text.Retain());
                }
                else
                {
                    LogUtil.Warn("ResponseMessage=>信道不存在");
                }
            }
            else
            {
                LogUtil.Warn("the message is not MessageResponse");
            }
            return null;
        }

        private object CallbackMessage(Message message)
        {
            var response = message as MessageCallback;
            if (response != null)
            {
                string json = System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(response);
                });
                //LogUtil.Info("CallbackMessage=>" + json);

                if (response.ChannelID == null)
                {
                    LogUtil.Warn("CallbackMessage=>返回信道值为null");
                }
                else
                {
                    if (s_onlineClients.ContainsKey(response.ChannelID))
                    {
                        var text = new TextWebSocketFrame(json);
                        s_onlineClients[response.ChannelID].Channel.WriteAndFlushAsync(text.Retain());
                    }
                    else
                    {
                        LogUtil.Warn("CallbackMessage=>信道不存在");
                    }
                }
            }
            else
            {
                LogUtil.Warn("the message is not MessageCallback");
            }
            return null;
        }

        private object ExecutePingMessage(Message message)
        {
            MessageRequest requestmessage = message as MessageRequest;
            MessageResponse res = new MessageResponse();
            res.ID = requestmessage.ID;
            res.ChannelID = requestmessage.ChannelID;
            res.Action = requestmessage.Action;
            res.errCode = 0;
            res.errText = "";
            EnvironmentManager.Instance.PostResponseMessage(res);
            return null;
        }

        public async Task StopServerAsync()
        {
            _autoEvent.Dispose();
            if (_timer != null)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                await _timer.DisposeAsync();
            }
            EnvironmentManager.Instance.UnRegisterReceiver(_registerResponseMessager);
            EnvironmentManager.Instance.UnRegisterReceiver(_registerCallbackMessager);
            EnvironmentManager.Instance.UnRegisterReceiver(_registerLoginMessager);
            EnvironmentManager.Instance.UnRegisterReceiver(_registerPingMessager);
            //await _bootstrapChannel.CloseAsync();
            //await _workGroup.ShutdownGracefullyAsync();
            //await _bossGroup.ShutdownGracefullyAsync();
        }
    }
}
