using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHS.Infrastructure.NlogEx
{
    /// <summary>
    /// 基于NLOG框架的日志工具类,添加了自定义的Process方法其他与NLOG无区别
    /// </summary>
    public static class LogUtil
    {
        private readonly static ILogger logger = null;

        static LogUtil()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        public static void Info(string msg)
        {
            try
            {
                logger.Info(msg);
            }
            catch
            { }
        }

        public static void Error(Exception ex)
        {
            try
            {
                logger.Error(ex);
            }
            catch
            { }
        }

        public static void Error(Exception ex,string message)
        {
            try
            {
                logger.Error(ex, message);
            }
            catch
            { }
        }


        public static void Error(string msg)
        {
            try
            {
                Log(LogLevel.Error, msg);
            }
            catch
            { }
        }

        public static void Warn(string msg)
        {
            try
            {
                logger.Warn(msg);
            }
            catch
            { }
        }

        public static void Log(LogLevel level, string msg)
        {
            try
            {
                logger.Log(level, msg);
            }
            catch
            { }
        }

        public static void Process(string message)
        {
            try
            {
                logger.Process(message);
            }
            catch
            { }
        }

    }
}
