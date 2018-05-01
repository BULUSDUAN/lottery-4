using System;
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

        public static void Debug(string msg) => log.Debug(msg);
        public static void Info(string msg) => log.Info(msg);
        public static void Warn(string msg) => log.Warn(msg);
        public static void Error(string msg) => log.Error(msg);
        public static void Fatal(string msg) => log.Fatal(msg);
    }
}
