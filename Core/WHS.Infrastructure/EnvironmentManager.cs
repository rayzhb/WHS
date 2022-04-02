using WHS.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using WHS.Infrastructure.NlogEx;

namespace WHS.Infrastructure
{
    public class EnvironmentManager
    {
        private static EnvironmentManager _instance;
        private static object _lock = new object();
        private List<RegistreredReceiver> _messageFilters;

        private EnvironmentManager()
        {
            _messageFilters = new List<RegistreredReceiver>();
        }

        public static EnvironmentManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new EnvironmentManager();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 注册接收器
        /// </summary>
        /// <param name="messageReceiver">接收委托</param>
        /// <param name="messageFilter">消息过滤</param>
        /// <returns></returns>
        public object RegisterReceiver(MessageReceiver messageReceiver, MessageFilter messageFilter)
        {
            List<RegistreredReceiver> messageFilters = this._messageFilters;
            object result;
            lock (messageFilters)
            {
                RegistreredReceiver registreredReceiver = new RegistreredReceiver(messageReceiver, messageFilter);
                this._messageFilters.Add(registreredReceiver);
                result = registreredReceiver;
            }
            return result;
        }

        /// <summary>
        /// 注销接收器
        /// </summary>
        /// <param name="registeredReceiver"></param>
        public void UnRegisterReceiver(object registeredReceiver)
        {
            List<RegistreredReceiver> messageFilters = this._messageFilters;
            lock (messageFilters)
            {
                RegistreredReceiver registreredReceiver = registeredReceiver as RegistreredReceiver;
                if (registreredReceiver != null && this._messageFilters.Contains(registreredReceiver))
                {
                    registreredReceiver.Unregistered = true;
                    this._messageFilters.Remove(registreredReceiver);
                }
                else
                {
                    LogUtil.Warn("Unable to find object to UnRegister");
                }
            }
        }


        /// <summary>
        /// 分发响应信息
        /// </summary>
        /// <param name="message"></param>
        public void PostResponseMessage(MessageResponse message)
        {
            List<RegistreredReceiver> list = new List<RegistreredReceiver>();
            List<RegistreredReceiver> messageFilters = this._messageFilters;
            lock (messageFilters)
            {
                foreach (RegistreredReceiver current in this._messageFilters)
                {
                    if (current.MessageFilter == null || current.MessageFilter.Match(message))
                    {
                        list.Add(current);
                    }
                }
            }
            foreach (RegistreredReceiver current2 in list)
            {
                try
                {
                    if (!current2.Unregistered)
                    {
                        object obj = current2.MessageReceiver.Invoke(message);
                        //if (obj != null)
                        //{
                        //}
                    }
                }
                catch (Exception exception)
                {
                    LogUtil.Error( exception, "PostResponseMessage");
                }
            }
        }

        /// <summary>
        /// 分发访问信息
        /// </summary>
        /// <param name="message"></param>
        public void PostRequestMessage(MessageRequest message)
        {
            List<RegistreredReceiver> list = new List<RegistreredReceiver>();
            List<RegistreredReceiver> messageFilters = this._messageFilters;
            lock (messageFilters)
            {
                foreach (RegistreredReceiver current in this._messageFilters)
                {
                    if (current.MessageFilter == null || current.MessageFilter.Match(message))
                    {
                        list.Add(current);
                    }
                }
            }
            foreach (RegistreredReceiver current2 in list)
            {
                try
                {
                    if (!current2.Unregistered)
                    {
                        object obj = current2.MessageReceiver.Invoke(message);
                        //if (obj != null)
                        //{
                        //}
                    }
                }
                catch (Exception exception)
                {
                    LogUtil.Error( exception, "PostRequestMessage");
                }
            }
        }

        /// <summary>
        /// 分发回调信息
        /// </summary>
        /// <param name="message"></param>
        public void PostCallbackMessage(MessageCallback message)
        {
            List<RegistreredReceiver> list = new List<RegistreredReceiver>();
            List<RegistreredReceiver> messageFilters = this._messageFilters;
            lock (messageFilters)
            {
                foreach (RegistreredReceiver current in this._messageFilters)
                {
                    if (current.MessageFilter == null || current.MessageFilter.Match(message))
                    {
                        list.Add(current);
                    }
                }
            }
            foreach (RegistreredReceiver current2 in list)
            {
                try
                {
                    if (!current2.Unregistered)
                    {
                        object obj = current2.MessageReceiver.Invoke(message);
                        //if (obj != null)
                        //{
                        //}
                    }
                }
                catch (Exception exception)
                {
                    LogUtil.Error( exception, "PostCallbackMessage");
                }
            }
        }
    }
}
