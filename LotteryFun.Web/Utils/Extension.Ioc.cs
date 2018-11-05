using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using LotteryFun.Web.Jobs;
using LotteryFun.Web.MessageService;
using LotteryFun.Web.MQ;
using LotteryFun.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Telegram.Bot;

namespace LotteryFun.Web.Utils
{
    public static class ExtensionIoc
    {
        public static IServiceProvider RegistAutofacServices(this IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterType<CollectService>().As<ICollectService>().SingleInstance();
            builder.RegisterType<LotteryPlanConsumer>().SingleInstance();
            builder.RegisterType<LotteryJobListener>().As<IJobListener>();
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
            builder.RegisterType<LotteryJob>().InstancePerLifetimeScope();
            builder.RegisterType<LotteryJobScheduler>().AsSelf();
        }

        private static void ConfigureScheduler(IContainer container)
        {
            var scheduler = container.Resolve<LotteryJobScheduler>();
            scheduler.Start(container);
        }
    }
}