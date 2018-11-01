using System;
using Autofac;
using Microsoft.Extensions.Logging;
using Quartz;
using Robin.Lottery.WebApp.Models;

namespace Robin.Lottery.WebApp.Quartz
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
                //TODO 每个计划的每个玩法创建一个任务
                var planners = (Planner[]) Enum.GetValues(typeof(Planner));
                var rules = (Pk10Rule[]) Enum.GetValues(typeof(Pk10Rule));

                foreach (var planner in planners)
                {
                    string group = planner.ToString();
                    foreach (var rule in rules)
                    {
                        var ruleDesc = rule.GetDescription();
                        var key = $"{planner}_{ruleDesc}";
                        IJobDetail job = JobBuilder.Create<LotteryPlanJob>()
                            .WithIdentity($"job_{key}", group)
                            .Build();

                        ITrigger trigger = (ICronTrigger) TriggerBuilder.Create()
                            .WithIdentity($"trigger_{key}", group)
                            .WithCronSchedule(_config.QuartzCronExp)
                            .Build();

                        job.JobDataMap.PutAsString("rule", (int) rule);
                        job.JobDataMap.PutAsString("planner", (int) planner);

                        _scheduler.ScheduleJob(job, trigger);
                    }
                }

                // 启动所有定时任务
                _scheduler.Start();

                _logger.LogInformation("Quartz Started....");
            }
            catch (Exception e)
            {
                _logger.LogError("Quartz 启动时抛出异常." + Environment.NewLine + e);
            }
        }
    }
}