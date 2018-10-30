using System;
using Autofac;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;

namespace Robin.Lottery.WebApp.Infrastructure
{
    public class QuartzJobFactory : IJobFactory
    {
        private readonly IContainer _container;
        private readonly ILogger<QuartzJobFactory> _logger;

        private IJob _job;

        public QuartzJobFactory(IContainer container)
        {
            _container = container;
            _logger = container.Resolve<ILogger<QuartzJobFactory>>();
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                _job = _container.Resolve(bundle.JobDetail.JobType) as IJob;
                return _job;
            }
            catch (Exception e)
            {
                _logger.LogError("返回 NewJob 时发生异常：" + Environment.NewLine + e);
                throw e;
            }
        }

        public void ReturnJob(IJob job)
        {
            (_job as IDisposable)?.Dispose();
        }
    }
}