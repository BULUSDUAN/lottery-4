using System;
using Autofac;
using Microsoft.Extensions.Logging;
using Quartz;
using Robin.Lottery.WebApp.Models;

namespace Robin.Lottery.WebApp.Infrastructure
{
    public class JobScheduler
    {
        private readonly AppConfig _config;
        private readonly ILogger<JobScheduler> _logger;
        private readonly IScheduler _scheduler;

        public JobScheduler(ILogger<JobScheduler> logger, AppConfig config, IScheduler scheduler)
        {
            _logger = logger;
            _config = config;
            _scheduler = scheduler;
        }

        public void Start(IContainer container)
        {
            _scheduler.JobFactory = new QuartzJobFactory(container);

            try
            {
                // 每个玩法创建一个任务
                var rules = (Pk10Rule[])Enum.GetValues(typeof(Pk10Rule));
                foreach (var rule in rules)
                {
                    var desc = rule.GetDescription();
                    const string group = "Win";
                    IJobDetail job = JobBuilder.Create<LotteryPlanJob>()
                        .WithIdentity($"job_{desc}", group)
                        .Build();

                    ITrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity($"trigger_{desc}", group)
                        .WithCronSchedule(_config.QuartzCronExp)
                        .Build();

                    job.JobDataMap.PutAsString("ruleValue", (int)rule);

                    _scheduler.ScheduleJob(job, trigger);
                }

                // 启动所有定时任务
                _scheduler.Start();

                _logger.LogInformation("Quartz Started....");
            }
            catch (Exception e)
            {
                _logger.LogError("Throwed exception when Quartz try to start... " + Environment.NewLine + e);
            }
        }
    }
}