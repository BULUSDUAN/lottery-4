using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Robin.Lottery.WebApp.Models;

namespace Robin.Lottery.WebApp.Jobs
{
    public class LotteryJobListener : IJobListener
    {
        private readonly ILogger<LotteryJobListener> _logger;

        public LotteryJobListener(ILogger<LotteryJobListener> logger)
        {
            _logger = logger;
            Name = nameof(LotteryJobListener);
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;

            Pk10Rule rule = (Pk10Rule)jobDataMap.GetIntValue("rule");
            Planner planner = (Planner)jobDataMap.GetIntValue("planner");

            _logger.LogDebug($"JobToBeExecuted : {planner}计划-{rule.GetDescription()}");
            return Task.FromResult(true);
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;

            Pk10Rule rule = (Pk10Rule)jobDataMap.GetIntValue("rule");
            Planner planner = (Planner)jobDataMap.GetIntValue("planner");

            _logger.LogDebug($"JobExecutionVetoed : {planner}计划-{rule.GetDescription()}");
            return Task.FromResult(true);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException,
            CancellationToken cancellationToken = new CancellationToken())
        {
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;

            Pk10Rule rule = (Pk10Rule)jobDataMap.GetIntValue("rule");
            Planner planner = (Planner)jobDataMap.GetIntValue("planner");

            _logger.LogDebug($"JobWasExecuted : 「{planner}计划-{rule.GetDescription()}」, " +
                             $"是否发生异常: {jobException != null}, 是否立即重新触发: {jobException?.RefireImmediately ?? false}");

            return Task.FromResult(true);
        }

        public string Name { get; }
    }
}