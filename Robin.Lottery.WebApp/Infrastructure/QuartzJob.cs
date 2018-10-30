using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Quartz;
using Robin.Lottery.WebApp.IServices;
using Robin.Lottery.WebApp.Services;
using Robin.Lottery.WebApp.Utils;

namespace Robin.Lottery.WebApp.Infrastructure
{
    [DisallowConcurrentExecution]
    public sealed class QuartzJob : IJob
    {
        private readonly ILogger<QuartzJob> _logger;
        private readonly ICollectService _collectService;

        public QuartzJob(ILogger<QuartzJob> logger, ICollectService collectService)
        {
            _logger = logger;
            _collectService = collectService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;

            string ruleValue = jobDataMap.GetString("ruleValue");

            return Task.Factory.StartNew(state =>
            {
                try
                {
                    _collectService.Request(state.ToString());
                }
                catch (ApplicationException e)
                {
                    _logger.LogError(e.Message, e.InnerException);
                }
                catch (Exception e)
                {
                    _logger.LogError("发生未处理异常。", e);
                }
            }, ruleValue);
        }
    }
}