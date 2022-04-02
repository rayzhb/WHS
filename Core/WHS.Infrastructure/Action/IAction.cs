using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Action
{
    public interface IAction
    {
        void ExecuteAction(Messaging.MessageRequest messageRequest);
    }
}
