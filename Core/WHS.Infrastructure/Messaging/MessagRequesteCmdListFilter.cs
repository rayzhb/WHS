using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Messaging
{
    public class MessagRequesteCmdListFilter : MessageFilter
    {
        private string[] _commandList;

        public MessagRequesteCmdListFilter(string[] commandList)
        {
            this._commandList = commandList;
        }

        public override bool Match(Message message)
        {
            string[] commandList = this._commandList;
            for (int i = 0; i < commandList.Length; i++)
            {
                string a = commandList[i];
                if (a == message.Action && message.RequestType == RequestType.Request)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
