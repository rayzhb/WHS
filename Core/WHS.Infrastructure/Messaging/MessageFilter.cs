using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Messaging
{
    /// <summary>
    /// 抽象类，子类集成用于定义消息过滤
    /// </summary>
    public abstract class MessageFilter
    {
        public virtual bool Match(Message message)
        {
            return true;
        }
    }
}
