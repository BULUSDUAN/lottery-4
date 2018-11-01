using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Robin.Lottery.WebApp.MQ;
using Robin.Lottery.WebApp.Quartz;
using Robin.Lottery.WebApp.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ExtensionIoc
    {
        public static IServiceProvider RegistAutofacServices(this IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterType<CollectService>().As<ICollectService>().SingleInstance();
            //builder.RegisterType<BusBuilder>().As<IBusBuilder>().SingleInstance();
            builder.RegisterType<LotteryPlanConsumer>().SingleInstance();
            builder.RegisterEasyNetQ("host=localhost");

            // 注册定时任务
            RegisterScheduler(builder);

            var container = builder.Build();

            // 配置并启动定时任务
            ConfigureScheduler(container);

            return new AutofacServiceProvider(container);
        }

        private static void RegisterScheduler(ContainerBuilder builder)
        {
            // configure and register Quartz
            builder.Register(x => new StdSchedulerFactory().GetScheduler().Result).As<IScheduler>();
            builder.RegisterType<LotteryPlanJob>().InstancePerLifetimeScope();
            builder.RegisterType<JobScheduler>().AsSelf();
        }

        private static void ConfigureScheduler(IContainer container)
        {
            var scheduler = container.Resolve<JobScheduler>();
            scheduler.Start(container);
        }
    }
}