using System.IO;
using log4net;
using log4net.Config;

namespace Colin.Lottery.Utils
{
    public static class LogUtil
    {
        private static readonly ILog Log;

        static LogUtil()
        {
            var repository = LogManager.CreateRepository("WebAppRepository");
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
            Log = LogManager.GetLogger(repository.Name, typeof(LogUtil));
        }


        public static void Debug(string message)
        {
            try
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    Log.Debug(message);
                });
            }
            catch
            {
                // ignored
            }
        }

        public static void Info(string message)
        {
            try
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    Log.Info(message);
                });
            }
            catch
            {
                // ignored
            }
        }

        public static void Warn(string message)
        {
            try
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    Log.Warn(message);
                });
            }
            catch
            {
                // ignored
            }
        }

        public static void Error(string message)
        {
            try
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    Log.Error(message);
                });
            }
            catch
            {
                // ignored
            }
        }

        public static void Fatal(string message)
        {
            try
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    Log.Fatal(message);
                });
            }
            catch
            {
                // ignored
            }
        }
    }
}
