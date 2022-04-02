using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using static DotNetty.Codecs.Http.HttpVersion;
using static DotNetty.Codecs.Http.HttpResponseStatus;
using WHS.Infrastructure;
using WHS.Infrastructure.NlogEx;
using WHS.Infrastructure.Messaging;
using WHS.Infrastructure.Action;
using WHS.Infrastructure.Utils;
using System.Net;

namespace WHS.Server.Handler
{
    public sealed class WebSocketServerHandler : SimpleChannelInboundHandler<object>
    {
        const string WebsocketPath = "/websocket";

        public WebSocketServerHandler()
        {
        }

        WebSocketServerHandshaker handshaker;

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);

            var b = WebSocketsServer.s_onlineClients.TryRemove(context.Channel.Id.ToString(), out _);
            if (b)
            {
                //回调各插件的方法
                foreach (PluginDefinition item in PluginManager.GetPluginDefinitions())
                {
                    item.handleCommunicationClose?.Invoke(context.Channel.Id.ToString());
                }
            }
            System.Diagnostics.Debug.Write($"信道状态{ context.Channel.Active} 删除状态:{b}");
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, object msg)
        {
            if (msg is IFullHttpRequest request)
            {
                this.HandleHttpRequest(ctx, request);
            }
            else if (msg is WebSocketFrame frame)
            {
                this.HandleWebSocketFrame(ctx, frame);
            }
        }
        private string text = string.Empty;

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        void HandleHttpRequest(IChannelHandlerContext ctx, IFullHttpRequest req)
        {
            // Handle a bad request.
            if (!req.Result.IsSuccess)
            {
                SendHttpResponse(ctx, req, new DefaultFullHttpResponse(Http11, BadRequest));
                return;
            }

            // Allow only GET methods.
            if (!Equals(req.Method, HttpMethod.Get))
            {
                SendHttpResponse(ctx, req, new DefaultFullHttpResponse(Http11, Forbidden));
                return;
            }

            // Send the demo page and favicon.ico
            if ("/".Equals(req.Uri))
            {

                string dir = AppContext.BaseDirectory + "Html\\";
                if (System.IO.Directory.Exists(dir))
                {
                    string file = dir + "index.html";
                    if (System.IO.File.Exists(file))
                    {
                        var bytes = System.IO.File.ReadAllBytes(file);
                        IByteBuffer content = Unpooled.WrappedBuffer(bytes);
                        var res = new DefaultFullHttpResponse(Http11, OK, content);

                        res.Headers.Set(HttpHeaderNames.ContentType, "text/html; charset=UTF-8");
                        HttpUtil.SetContentLength(res, content.ReadableBytes);

                        SendHttpResponse(ctx, req, res);
                        return;
                    }
                }
                var notfound = new DefaultFullHttpResponse(Http11, NotFound);
                SendHttpResponse(ctx, req, notfound);
                return;
            }
            if ("/vue_rcs".Equals(req.Uri))
            {

                string dir = AppContext.BaseDirectory + "Html\\";
                if (System.IO.Directory.Exists(dir))
                {
                    string file = dir + "vue_rcs.html";
                    if (System.IO.File.Exists(file))
                    {
                        var bytes = System.IO.File.ReadAllBytes(file);
                        IByteBuffer content = Unpooled.WrappedBuffer(bytes);
                        var res = new DefaultFullHttpResponse(Http11, OK, content);

                        res.Headers.Set(HttpHeaderNames.ContentType, "text/html; charset=UTF-8");
                        HttpUtil.SetContentLength(res, content.ReadableBytes);

                        SendHttpResponse(ctx, req, res);
                        return;
                    }
                }
                var notfound = new DefaultFullHttpResponse(Http11, NotFound);
                SendHttpResponse(ctx, req, notfound);
                return;
            }
            // Send the demo page and favicon.ico
            if ("/Performance".Equals(req.Uri))
            {

                IByteBuffer content = WebSocketServerBenchmarkPage.GetContent(GetWebSocketLocation(req));
                var res = new DefaultFullHttpResponse(Http11, OK, content);

                res.Headers.Set(HttpHeaderNames.ContentType, "text/html; charset=UTF-8");
                HttpUtil.SetContentLength(res, content.ReadableBytes);

                SendHttpResponse(ctx, req, res);
                return;
            }
            if ("/favicon.ico".Equals(req.Uri))
            {
                var res = new DefaultFullHttpResponse(Http11, NotFound);
                SendHttpResponse(ctx, req, res);
                return;
            }
            if (req.Uri.EndsWith(".css") || req.Uri.EndsWith(".js")
                || req.Uri.EndsWith(".eot") || req.Uri.EndsWith(".svg") || req.Uri.EndsWith(".ttf") || req.Uri.EndsWith(".woff")
                || req.Uri.EndsWith(".woff2") || req.Uri.EndsWith(".png") || req.Uri.EndsWith(".jpg") || req.Uri.EndsWith(".map"))
            {
                string location = req.Uri.Replace('/', '\\');
                string dir = AppContext.BaseDirectory + "Html";
                string file = dir + location;

                if (System.IO.File.Exists(dir + location))
                {
                    var bytes = System.IO.File.ReadAllBytes(file);
                    IByteBuffer content = Unpooled.WrappedBuffer(bytes);
                    var res = new DefaultFullHttpResponse(Http11, OK, content);
                    string extension = System.IO.Path.GetExtension(file).ToLower();
                    string contentType = "application/x-www-form-urlencoded";
                    switch (extension)
                    {
                        case ".css":
                            contentType = "text/css";
                            break;
                        case ".js":
                            contentType = "application/javascript";
                            break;
                        case ".eot":
                            contentType = "application/vnd.ms-fontobject";
                            break;
                        case ".svg":
                            contentType = "image/svg+xml";
                            break;
                        case ".ttf":
                            contentType = "application/x-font-ttf";
                            break;
                        case ".woff":
                            contentType = "application/font-woff";
                            break;
                        case ".woff2":
                            contentType = "application/font-woff2";
                            break;
                        case ".png":
                            contentType = "application/x-png";
                            break;
                        case ".jpg":
                            contentType = "image/jpeg";
                            break;
                        default:
                            contentType = "application/octet-stream";
                            break;
                    }

                    res.Headers.Set(HttpHeaderNames.ContentType, contentType);
                    HttpUtil.SetContentLength(res, content.ReadableBytes);

                    SendHttpResponse(ctx, req, res);
                    return;
                }
                var notfound = new DefaultFullHttpResponse(Http11, NotFound);
                SendHttpResponse(ctx, req, notfound);
                return;
            }

            // Handshake
            var wsFactory = new WebSocketServerHandshakerFactory(
                GetWebSocketLocation(req), null, true, 5 * 1024 * 1024);
            this.handshaker = wsFactory.NewHandshaker(req);
            if (this.handshaker == null)
            {
                WebSocketServerHandshakerFactory.SendUnsupportedVersionResponse(ctx.Channel);
            }
            else
            {
                this.handshaker.HandshakeAsync(ctx.Channel, req);
            }
            //只记录WEBSOVKET的连接数
            WebSocketsServer.s_onlineClients.TryAdd(ctx.Channel.Id.ToString(), ctx);

        }

        void HandleWebSocketFrame(IChannelHandlerContext ctx, WebSocketFrame frame)
        {
            // Check for closing frame
            if (frame is CloseWebSocketFrame)
            {
                this.handshaker.CloseAsync(ctx.Channel, (CloseWebSocketFrame)frame.Retain());
                return;
            }

            if (frame is PingWebSocketFrame)
            {
                ctx.WriteAsync(new PongWebSocketFrame((IByteBuffer)frame.Content.Retain()));
                return;
            }

            if (frame is TextWebSocketFrame)
            {
                // Echo the frame
                TextWebSocketFrame textWebSocketFrame = frame as TextWebSocketFrame;
                text = textWebSocketFrame.Text();
                if (frame.IsFinalFragment)
                {
                    try
                    {
                        var request = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageRequest>(text);
                        request.ChannelID = ctx.Channel.Id.ToString();
                        EnvironmentManager.Instance.PostRequestMessage(request);
                    }
                    catch
                    {
                        ctx.Channel.WriteAsync(frame.Retain());
                        LogUtil.Error("无法解析的JSON:" + text);
                    }
                }
                return;
            }

            if (frame is BinaryWebSocketFrame)
            {
                // Echo the frame
                BinaryWebSocketFrame binaryWebSocketFrame = frame as BinaryWebSocketFrame;
                text = binaryWebSocketFrame.Content.ToString(Encoding.ASCII);
                if (frame.IsFinalFragment)
                {
                    try
                    {
                        var request = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageRequest>(text);
                        request.ChannelID = ctx.Channel.Id.ToString();
                        EnvironmentManager.Instance.PostRequestMessage(request);
                    }
                    catch
                    {
                        ctx.Channel.WriteAsync(frame.Retain());

                        LogUtil.Error("无法解析的JSON:" + text);
                    }
                }
                return;
            }
            if (frame is ContinuationWebSocketFrame)
            {
                ContinuationWebSocketFrame continuationWebSocketFrame = frame as ContinuationWebSocketFrame;
                text += continuationWebSocketFrame.Text();
                if (frame.IsFinalFragment)
                {
                    try
                    {

                        var request = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageRequest>(text);
                        request.ChannelID = ctx.Channel.Id.ToString();
                        EnvironmentManager.Instance.PostRequestMessage(request);
                    }
                    catch
                    {
                        ctx.Channel.WriteAsync(frame.Retain());
                        LogUtil.Error("无法解析的JSON:" + text);
                    }
                }
                return;
            }
        }

        static void SendHttpResponse(IChannelHandlerContext ctx, IFullHttpRequest req, IFullHttpResponse res)
        {
            // Generate an error page if response getStatus code is not OK (200).
            if (res.Status.Code != 200)
            {
                IByteBuffer buf = Unpooled.CopiedBuffer(Encoding.UTF8.GetBytes(res.Status.ToString()));
                res.Content.WriteBytes(buf);
                buf.Release();
                HttpUtil.SetContentLength(res, res.Content.ReadableBytes);
            }

            // Send the response and close the connection if necessary.
            Task task = ctx.Channel.WriteAndFlushAsync(res);
            if (!HttpUtil.IsKeepAlive(req) || res.Status.Code != 200)
            {
                task.ContinueWith((t, c) => ((IChannelHandlerContext)c).CloseAsync(),
                    ctx, TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext ctx, Exception e)
        {
            Console.WriteLine($"{nameof(WebSocketServerHandler)} {0}", e);
            ctx.CloseAsync();
        }

        static string GetWebSocketLocation(IFullHttpRequest req)
        {
            bool result = req.Headers.TryGet(HttpHeaderNames.Host, out ICharSequence value);
            string location = value.ToString() + WebsocketPath;

            if (ServerSettings.IsSsl)
            {
                return "wss://" + location;
            }
            else
            {
                return "ws://" + location;
            }
        }
    }

}
