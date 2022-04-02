using WHS.Infrastructure.Messaging;
using WHS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHS.Infrastructure.Action;
using System.Windows.Media;
using WHS.Infrastructure.Config;
using WHS.Infrastructure.Interceptor;

namespace WHS.Infrastructure
{
    public abstract class PluginDefinition : ViewModel
    {
        private Dictionary<string, ActionBase> _dic_actions;

        private IPluginInterceptor _pluginInterceptor;
        private object _registerMessager;

        protected Dictionary<string, ActionBase> Actions
        {
            get
            {
                return this._dic_actions;
            }
        }

        public virtual NLog.ILogger Log
        {
            get
            {
                return NLog.LogManager.GetLogger(this.Name + "_Pulugin");
            }
        }

        public abstract Guid Id
        {
            get;
        }

        public abstract string Name
        {
            get;
        }

        public virtual string VersionString
        {
            get
            {
                return "1.0.0.0";
            }
        }

        public virtual string Manufacturer
        {
            get
            {
                return "Your Company name goes here";
            }
        }

        public abstract System.Drawing.Bitmap Icon { get; }

        public abstract Type ViewModel { get; }

        /// <summary>
        /// 文件
        /// </summary>
        public string FileName { get; set; }
        public bool EnableHotReload { get; internal set; }


        private bool _IsChecked;
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                _IsChecked = value;
                RaisePropertyChangedEvent("IsChecked");
            }
        }

        /// <summary>
        /// 显示的多语言设置，默认使用当前插件的Strings资源，命名PluginDisplayText，找不到则使用插件定义的Name
        /// </summary>
        /// <returns></returns>
        public virtual string GetLocalization(string key = "PluginDisplayText", string resourceFileName = "Strings")
        {
            string displaytext = LocalizationHelp.GetLocalizedString(this.GetType().Assembly, key, resourceFileName);
            if (string.IsNullOrEmpty(displaytext))
                return Name;
            else
                return displaytext;
        }

        /// <summary>
        /// 处理通信关闭
        /// </summary>
        public HandleCommunicationClose handleCommunicationClose { get; private set; }
        public virtual void Init()
        {
            //此代码必须添加
            Caliburn.Micro.AssemblySource.Instance.Add(ViewModel.Assembly);
            handleCommunicationClose = new HandleCommunicationClose(OnHandleCommunicationClose);
            _dic_actions = new Dictionary<string, ActionBase>();
            var assembly = System.Reflection.Assembly.GetCallingAssembly();
            var list = assembly.GetImplementedObjectsByInterface<ActionBase>(typeof(ActionBase));

            List<string> list_commands = new List<string>();
            foreach (var item in list)
            {
                var result = item as ActionBase;
                if (!_dic_actions.ContainsKey(result.Name))
                {
                    _dic_actions.Add(result.Name, result);
                    list_commands.Add(result.Name);
                    this.Log.Info("加载命令:" + result.Name);
                }
                else
                {
                    this.Log.Info("文件: " + result.GetType().Name + "=> 定义的命令(" + result.Name + ") 已经存在，请联系开发者修改");
                }
            }
            _registerMessager = EnvironmentManager.Instance.RegisterReceiver(new MessageReceiver(ExecuteMessage), new MessagRequesteCmdListFilter(list_commands.ToArray()));
        }

        private object ExecuteMessage(Message message)
        {
            MessageRequest requestmessage = message as MessageRequest;
            if (_pluginInterceptor != null)
            {
                if (!_pluginInterceptor.PreHandle(requestmessage))
                {
                    return null;
                }
            }
            if (_dic_actions.ContainsKey(message.Action))
            {
                if (requestmessage == null)
                {
                    Console.WriteLine("the message is not MessageRequest");
                    return null;
                }
                this.Log.Info("ExecuteMessage ACTION:" + message.Action);

                var action = _dic_actions[message.Action];

                action.ExecuteAction(requestmessage);
            }
            _pluginInterceptor?.AfterHandle();
            return null;
        }

        public virtual void Close()
        {
            if (_registerMessager != null)
                EnvironmentManager.Instance.UnRegisterReceiver(_registerMessager);
            _dic_actions?.Clear();
            Caliburn.Micro.AssemblySource.Instance.Remove(ViewModel.Assembly);
        }

        protected void RegistPulginInterceptor(IPluginInterceptor pluginInterceptor)
        {
            this._pluginInterceptor = pluginInterceptor;
        }

        protected virtual void OnHandleCommunicationClose(string channelid)
        {

        }
    }
}
