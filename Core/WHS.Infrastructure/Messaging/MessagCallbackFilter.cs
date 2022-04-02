using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Messaging
{
    public class MessagCallbackFilter : MessageFilter
    {
        public MessagCallbackFilter()
        {
        }

        public override bool Match(Message message)
        {
            if (message.RequestType == RequestType.Callback)
            {
                return true;
            }
            return false;
        }
    }
}
