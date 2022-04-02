using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHS.Infrastructure.Messaging;

namespace WHS.Infrastructure.Interceptor
{
    public interface IPluginInterceptor
    {
        bool PreHandle(MessageRequest requestmessage);

        void AfterHandle();
    }
}
