//using System;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
//using Quartz;
//using Robin.Lottery.WebApp.Infrastructure;
//using Robin.Lottery.WebApp.Models;
//
//namespace Robin.Lottery.WebApp.Services
//{
//    public class MainService
//    {
//        private readonly AppConfig _config;
//        private readonly IScheduler _scheduler;
//        private readonly ILogger<MainService> _logger;
//
//        public MainService(AppConfig config, IScheduler scheduler, ILogger<MainService> logger)
//        {
//            _config = config;
//            _scheduler = scheduler;
//            _logger = logger;
//        }
////
////        /// <summary>
////        /// 初始化，包括 Quartz 任务
////        /// </summary>
////        public void Init()
////        {
////            Task.Run(async () => { await EnableQuartzJob(); });
////        }
////
////        /// <summary>
////        /// 创建并启用定时扫水任务
////        /// </summary>
////        /// <returns></returns>
////        private async Task EnableQuartzJob()
////        {
////            
////        }
//    }
//}