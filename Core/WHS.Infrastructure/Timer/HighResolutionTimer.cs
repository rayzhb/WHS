using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Timer
{
    /// <summary>
    /// 高精度定时器，用于在需要高精度使用的情况下，如视频获取帧
    /// </summary>
    public class HighResolutionTimer
    {
        private readonly Thread _Thread;
        private readonly long _MaxDelayInTicks;  // do not run if the delay was too long
        private long _nextWakeUpTickTime;
        private long _cycleTimeInTicks;

        public event EventHandler OnTimer;
        public event EventHandler OnSkipped;
        public event EventHandler OnStoped;

        /// <summary>
        /// High Resolution Timer 
        /// </summary>
        /// <param name="cycle">cycle in milliseconds</param>
        /// <param name="maxDelay">maxDelay in milliseconds</param>
        public HighResolutionTimer(double cycle, double maxDelay)
        {
            _cycleTimeInTicks = (long)((cycle / 1000) * Stopwatch.Frequency);

            _Thread = new Thread(new ThreadStart(Loop));
            _Thread.Priority = ThreadPriority.Highest;
            _Thread.Name = "HighResolutionTimerLoop";
            _Thread.IsBackground = true;
            _MaxDelayInTicks = (long)(maxDelay * Stopwatch.Frequency) / 1000L; // 3 ms;
        }

        public int Start()
        {
            if ((_Thread.ThreadState & System.Threading.ThreadState.Unstarted) == 0) return -1;
            _Thread.Start();
            return _Thread.ManagedThreadId;
        }

        public void Stop()
        {
            _Thread.Interrupt();
        }

        private void Loop()
        {
            _nextWakeUpTickTime = Stopwatch.GetTimestamp();

            try
            {
                while (true)
                {
                    _nextWakeUpTickTime += _cycleTimeInTicks;

                    while (true)
                    {
                        long ticks = _nextWakeUpTickTime - Stopwatch.GetTimestamp();
                        if (ticks <= 0L) break;
                        long diff = (ticks * 1000) / Stopwatch.Frequency; // cycle in milliseconds

                        if (diff >= 100)
                            Thread.Sleep(20);
                        else if (diff >= 40)
                            Thread.Sleep(10);
                        else if (diff >= 25)
                            Thread.Sleep(2);
                        else if (diff >= 15)
                            Thread.Sleep(1);
                        else if (diff >= 5)
                            Thread.SpinWait(200);
                        else if (diff > 1)
                            Thread.SpinWait(100);
                        else
                            Thread.SpinWait(10);
                    }

                    long lDelay = Stopwatch.GetTimestamp() - _nextWakeUpTickTime;

                    if (lDelay < _MaxDelayInTicks)
                        OnTimer?.Invoke(null, null);
                    else
                        OnSkipped?.Invoke(null, null);
                }
            }
            catch (ThreadInterruptedException) { }
            catch (Exception) { Console.WriteLine("Exiting timer thread."); }

            OnStoped?.Invoke(null, null);
        }
    }
}
