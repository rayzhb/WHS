using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Messaging
{
    public class MessagResponseFilter : MessageFilter
    {
        public MessagResponseFilter()
        {
        }

        public override bool Match(Message message)
        {
            if (message.RequestType == RequestType.Response)
            {
                return true;
            }
            return false;
        }
    }
}
