using WHS.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure
{
    internal class RegistreredReceiver
    {
        private MessageReceiver _messageReceiver;

        private MessageFilter _messageFilter;

        private bool _unregistered;

        internal MessageReceiver MessageReceiver
        {
            get
            {
                return this._messageReceiver;
            }
        }

        internal MessageFilter MessageFilter
        {
            get
            {
                return this._messageFilter;
            }
        }

        public bool Unregistered
        {
            get
            {
                return this._unregistered;
            }
            set
            {
                this._unregistered = value;
            }
        }

        internal RegistreredReceiver(MessageReceiver messageReceiver, MessageFilter messageFilter)
        {
            this._messageFilter = messageFilter;
            this._messageReceiver = messageReceiver;
            this._unregistered = false;
        }
    }
}
