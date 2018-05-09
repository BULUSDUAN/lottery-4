using System.IO;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace Colin.Lottery.Utils
{
    public static class LogUtil
    {
        static ILoggerRepository repository;
        public static ILog log;

        static LogUtil()
        {
            repository = LogManager.CreateRepository("WebAppRepository");
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
            log = LogManager.GetLogger(repository.Name, typeof(LogUtil));
        }


        public static void Debug(string message)
        {
            try
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    log.Debug(message);
                });
            }
            catch { }
        }

        public static void Info(string message)
        {
            try
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    log.Info(message);
                });
            }
            catch { }
        }

        public static void Warn(string message)
        {
            try
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    log.Warn(message);
                });
            }
            catch { }
        }

        public static void Error(string message)
        {
            try
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    log.Error(message);
                });
            }
            catch { }
        }

        public static void Fatal(string message)
        {
            try
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    log.Fatal(message);
                });
            }
            catch { }
        }
    }
}
