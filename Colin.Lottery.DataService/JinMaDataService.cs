using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colin.Lottery.Analyzers;
using Colin.Lottery.Common.Scheduler;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;
using Quartz;

namespace Colin.Lottery.DataService
{
    public class JinMaDataService : DataService<JinMaDataService>
    {
        public override event EventHandler<DataCollectedEventArgs> DataCollectedSuccess;
        public override event EventHandler<CollectErrorEventArgs> DataCollectedError;

        public override async Task Start()
        {
            await StartPk10(Pk10Rule.Champion);
            await StartPk10(Pk10Rule.Second);
            await StartPk10(Pk10Rule.Third);
            await StartPk10(Pk10Rule.Fourth);
            await StartPk10(Pk10Rule.BigOrSmall);
            await StartPk10(Pk10Rule.OddOrEven);
            await StartPk10(Pk10Rule.DragonOrTiger);
            await StartPk10(Pk10Rule.Sum);
        }

        public override void Start(Dictionary<LotteryType, List<int>> typeRules)
        {
            if (typeRules == null)
                return;

            foreach (var type in typeRules.Keys)
            {
                switch (type)
                {
                    case LotteryType.Cqssc:
                        typeRules[type].ForEach(r =>
                        {
                            switch ((CqsscRule) r)
                            {
                                case CqsscRule.OddOrEven:
                                    break;
                                case CqsscRule.BigOrSmall:
                                    break;
                                case CqsscRule.DragonOrTiger:
                                    break;
                                case CqsscRule.Last2Group:
                                    break;
                                case CqsscRule.Last3Group:
                                    break;
                                case CqsscRule.OneOddOrEven:
                                    break;
                                case CqsscRule.OneBigOrSmall:
                                    break;
                                case CqsscRule.One:
                                    break;
                            }
                        });
                        break;
                    case LotteryType.Pk10:
                        typeRules[type].ForEach(async r => await StartPk10((Pk10Rule) r));
                        break;
                }
            }
        }


        private async Task StartPk10(Pk10Rule rule)
        {
            var prefix = $"{LotteryType.Pk10}_{rule}";

            //大管家Job，负责创建每期的扫水Job
            var stewardJob = QuartzUtil.CreateSimpleJob($"{prefix}_Steward_Job", $"{LotteryType.Pk10}_JobGroup",
                Scan);

            /* 
             * 北京赛车时间为每天9:00到23:50，每5分钟开一期，共179期 (北京时间)
             * 
             * “0 1/5 9-23 * * ?”
             * 每天9点到23点，每5分钟的第1分钟第0秒。如 9:01:00,9:06:00 ... 9:56:00 ... 23:51:00,23:56:00
             *
             * 9-23(GMT+08:00)  =>  1-15(UTC)
            */

            var pk10Trigger = QuartzUtil.CreateTrigger(prefix, "JinMaTrigger",
                $"0 1/5 {1.ToLocalHour()}-{15.ToLocalHour()} * * ?");


            //启动定时扫水
            await QuartzUtil.GetScheduler().ScheduleJob(stewardJob, pk10Trigger);


            async void Scan()
            {
                var timestamp = DateTime.UtcNow;
                var periodNo = Pk10Scheduler.Instance.GetPeriodNo(timestamp);
                var locked = new RulePeriodLocked(rule, periodNo);

                var jobTrigger = $"{prefix}_Scan_{periodNo}".ToJobTrigger();
                var trigger = QuartzUtil.CreateTrigger(jobTrigger.Trigger, "0/5 * * * * ? *");
                var job = QuartzUtil.CreateSimpleJob(jobTrigger.Job, () =>
                {
                    //避免第一轮任务执行未完成时第二轮任务开始执行,锁定 同一种玩法同一期 任务
                    lock (locked)
                    {
                        //Job废弃
                        if (locked.Finished)
                            return;

                        //超时自毁
                        if ((DateTime.UtcNow - timestamp).TotalMinutes > 5)
                        {
                            QuartzUtil.TryDestroyJob(jobTrigger).Wait();
                            locked.Finish();
                            return;
                        }

                        //扫水
                        var task = JinMaAnalyzer.Instance.GetForecastData(LotteryType.Pk10, (int) rule);
                        task.Wait();
                        var plans = task.Result;
                        /*
                        * 如果目标网站接口正常，每次都可以扫到结果，即使没有更新最新期预测数据。所以如果没有扫水结果，说明目标网站接口出错或扫到到数据异常
                        */
                        if (plans == null)
                        {
                            DataCollectedError?.Invoke(this, new CollectErrorEventArgs(rule, "扫水异常或数据数据错误"));
                            return;
                        }

                        //扫到老数据
                        if (plans.Any(p => p.LastDrawnPeriod + 1 < periodNo))
                            return;

                        //成功扫到最新数据
                        JinMaAnalyzer.Instance.CalcuteScore(plans);
                        DataCollectedSuccess?.Invoke(this,
                            new DataCollectedEventArgs(LotteryType.Pk10, rule, plans));

                        //扫水成功自毁
                        QuartzUtil.TryDestroyJob(jobTrigger).Wait();
                        locked.Finish();
                    }
                });

                await QuartzUtil.GetScheduler().ScheduleJob(job, trigger);
            }
        }

        class RulePeriodLocked
        {
            public Pk10Rule Rule { get; }
            public long PeriodNo { get; }
            public bool Finished { get; set; }

            public RulePeriodLocked(Pk10Rule rule, long periodNo)
            {
                Rule = rule;
                PeriodNo = periodNo;
                Finished = false;
            }

            public void Finish() => Finished = true;
        }
    }
}