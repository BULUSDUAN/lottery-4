using System;
using Autofac;
using LotteryFun.Web.Models;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;

namespace LotteryFun.Web.Jobs
{
    public class LotteryJobScheduler
    {
        private readonly AppConfig _config;
        private readonly ILogger<LotteryJobScheduler> _logger;
        private readonly IScheduler _scheduler;

        public LotteryJobScheduler(ILogger<LotteryJobScheduler> logger, AppConfig config, IScheduler scheduler)
        {
            _logger = logger;
            _config = config;
            _scheduler = scheduler;
        }

        public void Start(IContainer container)
        {
            _scheduler.JobFactory = new LotteryJobFactory(container);

            try
            {
                //TODO 每个计划的每个玩法创建一个任务
                var planners = (Planner[])Enum.GetValues(typeof(Planner));
                var rules = (Pk10Rule[])Enum.GetValues(typeof(Pk10Rule));

                foreach (var planner in planners)
                {
                    string group = planner.ToString();
                    foreach (var rule in rules)
                    {
                        var ruleDesc = rule.GetDescription();
                        var key = $"{planner}_{ruleDesc}";
                        IJobDetail job = JobBuilder.Create<LotteryJob>()
                            .WithIdentity($"job_{key}", group)
                            .Build();

                        ITrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                            .WithIdentity($"trigger_{key}", group)
                            .WithCronSchedule(_config.QuartzCronExp)
                            .Build();

                        job.JobDataMap.PutAsString("rule", (int)rule);
                        job.JobDataMap.PutAsString("planner", (int)planner);

                        // Set up the listener
                        IJobListener listener = container.Resolve<IJobListener>();
                        IMatcher<JobKey> matcher = KeyMatcher<JobKey>.KeyEquals(job.Key);
                        _scheduler.ListenerManager.AddJobListener(listener, matcher);

                        _scheduler.ScheduleJob(job, trigger);
                    }
                }

                // 启动所有定时任务
                _scheduler.Start();

                _logger.LogInformation("Quartz 已启动....");
            }
            catch (Exception e)
            {
                _logger.LogError("Quartz 启动时抛出异常." + Environment.NewLine + e);
            }
        }
    }
}