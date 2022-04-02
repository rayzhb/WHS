using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.NlogEx
{
    [Target("LimitedMemory")]
    public sealed class LimitedMemoryTarget : TargetWithLayout
    {

        private StringBuilder _stringBuilder;

        public LimitedMemoryTarget()
        {
            _stringBuilder = new StringBuilder();
            base.OptimizeBufferReuse = true;
            this.InitializeTarget();
        }

        public LimitedMemoryTarget(string name)
        : this()
        {
            base.Name = name;
        }

        public StringBuilder StringBuilderLogs
        {
            get
            {
                return _stringBuilder;
            }
        }
        public int MaxLimitLength { get; set; } = 6000;

        protected override void Write(LogEventInfo logEvent)
        {
            string msg = this.Layout.Render(logEvent);
            var length = msg.Length;
            var cap_length = _stringBuilder.Length;
            int remove = (cap_length + length) - MaxLimitLength;
            if (remove > 0)
            {
                if (remove > cap_length)
                {
                    _stringBuilder.Clear();
                }
                else
                {
                    _stringBuilder.Remove(0, remove);
                }

            }
            _stringBuilder.AppendLine(msg);

            OnFireLogHandler(_stringBuilder, msg);
        }

        private void OnFireLogHandler(StringBuilder stringBuilder, string newMessage)
        {
            if (FireLog != null)
                FireLog(stringBuilder, newMessage);
        }

        public delegate void FireLogHandler(StringBuilder stringBuilder, string newMessage);

        public event FireLogHandler FireLog;
    }
}
