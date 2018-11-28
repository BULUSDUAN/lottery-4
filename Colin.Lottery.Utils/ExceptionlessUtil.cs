using System;
using Exceptionless;
using Exceptionless.Logging;

namespace Colin.Lottery.Utils
{
    public static class ExceptionlessUtil
    {
        public static void Trace(string message)
        {
            Trace(new Exception(message));
        }

        public static void Trace(Exception ex, string title = null, bool traceException = false)
        {
            Log(ex, LogLevel.Trace, title, traceException);
        }

        public static void Debug(string message)
        {
            Debug(new Exception(message));
        }

        public static void Debug(Exception ex, string title = null, bool traceException = false)
        {
            Log(ex, LogLevel.Debug, title, traceException);
        }

        public static void Info(string message)
        {
            Info(new Exception(message));
        }

        public static void Info(Exception ex, string title = null, bool traceException = false)
        {
            Log(ex, LogLevel.Info, title, traceException);
        }

        public static void Warn(string message)
        {
            Warn(new Exception(message));
        }

        public static void Warn(Exception ex, string title = null, bool traceException = false)
        {
            Log(ex, LogLevel.Warn, title, traceException);
        }

        public static void Error(string message)
        {
            Error(new Exception(message));
        }

        public static void Error(Exception ex, string title = null, bool traceException = false)
        {
            Log(ex, LogLevel.Error, title, traceException);
        }

        public static void Fatal(string message)
        {
            Fatal(new Exception(message));
        }

        public static void Fatal(Exception ex, string title = null, bool traceException = false)
        {
            Log(ex, LogLevel.Fatal, title, traceException);
        }

        public static void Other(string message)
        {
            Other(new Exception(message));
        }

        public static void Other(Exception ex, string title = null, bool traceException = false)
        {
            Log(ex, LogLevel.Other, title, traceException);
        }


        /// <summary>
        /// 记录Log
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="logLevel">日志级别</param>
        /// <param name="traceException">是否追踪异常。true则提交Exception,否则只提交Log</param>
        public static void Log(Exception ex, LogLevel logLevel, string title = null, bool traceException = false)
        {
            ExceptionlessClient.Default.CreateLog(ex.TargetSite.GetType().FullName,
                    string.IsNullOrWhiteSpace(title) ? string.Empty : $"{title},错误消息:" + ex.Message, logLevel)
                .AddTags(ex.GetType().FullName).Submit();

            if (traceException)
                TraceException(ex);
        }

        /// <summary>
        /// 追踪Exception
        /// </summary>
        /// <param name="ex"></param>
        public static void TraceException(Exception ex)
        {
            ex.ToExceptionless().Submit();
        }
    }
}