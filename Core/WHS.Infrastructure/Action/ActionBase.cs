using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHS.Infrastructure.Messaging;

namespace WHS.Infrastructure.Action
{
    public abstract class ActionBase : IAction
    {
        public virtual NLog.ILogger Log
        {
            get
            {
                return NLog.LogManager.GetLogger(this.Name);
            }
        }
        public abstract string Name { get; }
        public abstract void ExecuteAction(MessageRequest messageRequest);

        private System.Threading.ManualResetEventSlim _syncSlim = new System.Threading.ManualResetEventSlim(false);

        public System.Threading.ManualResetEventSlim SyncSlim
        {
            get
            {
                return _syncSlim;
            }
        }

        public object SyncObject { get; set; }
    }
}
