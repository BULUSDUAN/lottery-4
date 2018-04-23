using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace Colin.Lottery.Utils
{
    public static class QuartzUtil
    {
        /// <summary>
        /// 获取全局唯一Scheduler(GlobleScheduler)
        /// </summary>
        /// <returns></returns>
        public static IScheduler GetScheduler()
        {
            return Scheduler.Singleton;
        }

        /// <summary>
        /// 构建Scheduler配置
        /// </summary>
        /// <returns></returns>
        public static NameValueCollection BuildSchedulerProperties(string instanceName, int threadCount = 5)
        {
            var properties = new NameValueCollection
            {
                ["quartz.scheduler.instanceName"] = instanceName,

                // 设置线程池
                ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
                ["quartz.threadPool.threadCount"] = threadCount.ToString(),
                ["quartz.threadPool.threadPriority"] = "Normal"
            };

            return properties;
        }

        /// <summary>
        /// 创建一个通用Job
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="group">分组</param>
        /// <param name="jobDelegate">Job要执行的方法</param>
        /// <returns></returns>
        public static IJobDetail CreateSimpleJob(string name, string group, Action jobDelegate)
        {
            return JobBuilder.Create<SimpleJob>()
                 .WithIdentity(name, group)
                 .UsingJobData(new JobDataMap((IDictionary<string, object>)new Dictionary<string, object> { { "jobDelegate", jobDelegate } }))
                 .Build();
        }

        /// <summary>
        /// 删除指定名称Job
        /// </summary>
        /// <param name="name">Job名称</param>
        /// <returns>true if the Job was found and deleted.</returns>
        public static async Task<bool> DeleteJob(string name)
        {
            return await GetScheduler().DeleteJob(new JobKey(name));
        }

        /// <summary>
        /// 删除指定名称和分组的Job
        /// </summary>
        /// <param name="name">Job名称</param>
        /// <param name="group">Job分组名称</param>
        /// <returns>true if the Job was found and deleted.</returns>
        public static async Task<bool> DeleteJob(string name, string group)
        {
            return await GetScheduler().DeleteJob(new JobKey(name, group));
        }

        /// <summary>
        /// 删除指定分组的Jobs
        /// </summary>
        /// <param name="group">Job分组名称</param>
        /// <param name="compareWith">分组名称匹配规则</param>
        /// <returns>true if all of the Jobs were found and deleted, false if one or more were not</returns>
        public static async Task<bool> DeleteJobs(string group, StringOperator compareWith)
        {
            GroupMatcher<JobKey> matcher;
            if (compareWith == StringOperator.Contains)
                matcher = GroupMatcher<JobKey>.GroupContains(group);
            else if (compareWith == StringOperator.EndsWith)
                matcher = GroupMatcher<JobKey>.GroupEndsWith(group);
            else if (compareWith == StringOperator.Equality)
                matcher = GroupMatcher<JobKey>.GroupEquals(group);
            else if (compareWith == StringOperator.StartsWith)
                matcher = GroupMatcher<JobKey>.GroupStartsWith(group);
            else
                matcher = GroupMatcher<JobKey>.AnyGroup();

            return await GetScheduler().DeleteJobs((await GetScheduler().GetJobKeys(matcher)).ToList());
        }

        /// <summary>
        /// 创建一个触发器
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="group">分组</param>
        /// <param name="cron">Cron表达式</param>
        /// <param name="startTime">开始时间</param>
        /// <returns></returns>
        public static ITrigger CreateTrigger(string name, string group, string cron, DateTime startTime)
        {
            return TriggerBuilder.Create()
                .WithIdentity(name, group)
                .WithCronSchedule(cron)
                .StartAt(startTime)
                .Build();
        }

        public static ITrigger CreateTrigger(string name, string group, string cron)
        {
            return CreateTrigger(name, group, cron, DateTime.Now);
        }

        /// <summary>
        /// 创建一个触发器(仅执行一次)
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="group">分组</param>
        /// <param name="startTime">开始时间</param>
        /// <returns></returns>
        public static ITrigger CreateTrigger(string name, string group, DateTime startTime)
        {
            return TriggerBuilder.Create().WithIdentity(name, group).StartAt(startTime).Build();
        }

        ///// <summary>
        ///// 创建一组触发器
        ///// </summary>
        ///// <param name="nameKeyWord">触发器name关键字</param>
        ///// <param name="crons">cron表达式集合</param>
        ///// <returns></returns>
        //public static Quartz.Collection.ISet<ITrigger> CreateTriggerGroup(string nameKeyWord, params string[] crons)
        //{
        //    string namePrefix = $"{nameKeyWord}_Trigger_";
        //    string triggerGroup = $"{nameKeyWord}_TriggerGroup";

        //    var triggers = new Quartz.Collection.HashSet<ITrigger>();
        //    for (int i = 0; i < crons.Length; i++)
        //        triggers.Add(CreateTrigger(namePrefix + i, triggerGroup, crons[i]));
        //    return triggers;
        //}

        /// <summary>
        /// 计划简单任务
        /// </summary>
        /// <param name="nameKeyword">Job名称关键字</param>
        /// <param name="jobDelegate">Job内容</param>
        /// <param name="cron">Trigger Cron表达式</param>
        public static async Task ScheduleSimpleJob(string nameKeyword, Action jobDelegate, string cron)
        {
            (string jobName, string jobGroup, string triggerName, string triggerGroup) = nameKeyword.JobAndTriggerNames();
            await DeleteJob(jobName, jobGroup);

            var job = CreateSimpleJob(jobName, jobGroup, jobDelegate);
            var trigger = CreateTrigger(triggerName, triggerGroup, cron);
            await GetScheduler().ScheduleJob(job, trigger);
        }

        /// <summary>
        /// 按照$"{keyword}_Job", $"{keyword}_JobGroup", $"{keyword}_Trigger", $"{keyword}_TriggerGroup"
        /// 规则生成默认Job和Trigger名称和分组名称
        /// </summary>
        /// <param name="keyword">名称关键字</param>
        /// <returns></returns>
        public static (string JobName, string JobGroup, string TriggerName, string TriggerGroup) JobAndTriggerNames(this string keyword)
        {
            return (
                string.Format("{0}_Job", keyword),
                string.Format("{0}_JobGroup", keyword),
                string.Format("{0}_Trigger", keyword),
                string.Format("{0}_TriggerGroup", keyword)
                );
        }

        ///// <summary>
        ///// 按照$"{keyword}_Job", $"{keyword}_JobGroup", $"{keyword}_Trigger", $"{keyword}_TriggerGroup"
        ///// 规则生成默认Job和Trigger名称和分组名称
        ///// </summary>
        ///// <param name="keyword">名称关键字</param>
        ///// <returns></returns>
        //public static (string JobName, string JobGroup, string TriggerName, string TriggerGroup) TodayJobAndTriggerNames(this string keyword)
        //{
        //    string suffix = DateTime.Today.ToJobAndTriggerSuffix();
        //    return ($"{keyword}_Job_{suffix}", $"{keyword}_JobGroup_{suffix}", $"{keyword}_Trigger_{suffix}", $"{keyword}_TriggerGroup_{suffix}");
        //}

        /// <summary>
        /// 将当前日期转换为对应Job、Trigger、Group后缀
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToJobAndTriggerSuffix(this DateTime date)
        {
            return date.ToShortDateString();
        }

        public static async Task ModeifyTriggerTime(string triggerName, string triggerGroupName)
        {
            var sched = GetScheduler();
            ITrigger trigger = await sched.GetTrigger(new TriggerKey(triggerName, triggerGroupName));
            var key = new TriggerKey(triggerName, triggerGroupName);
            await sched.ResumeTrigger(key);
        }
    }

    /// <summary>
    /// Scheduler单例
    /// </summary>
    class Scheduler
    {
        public static IScheduler Singleton;
        private Scheduler()
        {
        }
        static Scheduler()
        {
            async void Initialize()
            {
                Singleton = await new StdSchedulerFactory(QuartzUtil.BuildSchedulerProperties("GlobleSchedulerClient")).GetScheduler();
                await Singleton.Start();
            }
            Initialize();
        }
    }

    class SimpleJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            if (!(context.JobDetail.JobDataMap["jobDelegate"] is Action job))
                return;

            await Task.Run(job);
        }
    }
}
