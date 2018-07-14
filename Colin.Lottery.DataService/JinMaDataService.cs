using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Colin.Lottery.Analyzers;
using Colin.Lottery.Common.Scheduler;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;

namespace Colin.Lottery.DataService
{
    public class JinMaDataService : DataService<JinMaDataService>
    {
        public override event EventHandler<DataCollectedEventArgs> DataCollectedSuccess;
        public override event EventHandler<CollectErrorEventArgs> DataCollectedError;

        public override async Task Start(bool startWhenBreakGua = true)
        {
            await StartPk10(Pk10Rule.Champion, startWhenBreakGua);
            await StartPk10(Pk10Rule.Second, startWhenBreakGua);
            await StartPk10(Pk10Rule.Third, startWhenBreakGua);
            await StartPk10(Pk10Rule.Fourth, startWhenBreakGua);
            await StartPk10(Pk10Rule.BigOrSmall, startWhenBreakGua);
            await StartPk10(Pk10Rule.OddOrEven, startWhenBreakGua);
            await StartPk10(Pk10Rule.DragonOrTiger, startWhenBreakGua);
            await StartPk10(Pk10Rule.Sum, startWhenBreakGua);
        }

        public override void Start(Dictionary<LotteryType, List<int>> typeRules, bool startWhenBreakGua = true)
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
                            switch ((CqsscRule)r)
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
                        typeRules[type].ForEach(async r => await StartPk10((Pk10Rule)r, startWhenBreakGua));
                        break;
                }
            }
        }

        private async Task StartPk10(Pk10Rule rule, bool startWhenBreakGua)
        {
            var prefix = $"{LotteryType.Pk10}_{rule}";
            //大管家Job，负责创建每期的扫水Job
            var stewardJob = QuartzUtil.CreateSimpleJob($"{prefix}_Steward_Job", $"{LotteryType.Pk10}_JobGroup", async () =>
            {
                var timestamp = DateTime.Now;
                var periodNo = Pk10Scheduler.Instance.GetPeriodNo(timestamp);
                var tempJob = $"{prefix}_Scan_{periodNo}";
                var (JobName, JobGroup, _, _) = tempJob.JobAndTriggerNames();

                await QuartzUtil.ScheduleSimpleJob(tempJob, async () =>
                {
                    //超时自毁
                    if ((DateTime.Now - timestamp).TotalMinutes > 5)
                        await QuartzUtil.DeleteJob(JobName, JobGroup);

                    //扫水
                    var plans = await JinMaAnalyzer.Instance.GetForcastData(LotteryType.Pk10, (int)rule);
                    /*
                    * 如果目标网站接口正常，每次都可以扫到结果，即使没有更新最新期预测数据。所以如果没有扫水结果，说明目标网站接口出错
                    */
                    if (plans == null || plans.Count < 2 || plans.Any(p => p == null))
                    {
                        DataCollectedError?.Invoke(this, new CollectErrorEventArgs(rule,"目标网站扫水接口异常，请尽快检查恢复"));
                        return;
                    }

                    JinMaAnalyzer.Instance.CalcuteScore(plans, startWhenBreakGua);

                    if (plans.Any(p => p.LastDrawedPeriod + 1 < periodNo)) return;

                    DataCollectedSuccess?.Invoke(this, new DataCollectedEventArgs(rule,plans));

                    await QuartzUtil.DeleteJob(JobName, JobGroup);
                }, "0/5 * * * * ? *");
            });

            /* 
             * 北京赛车时间为每天9:00到23:50，每5分钟开一期，共179期
             * 
             * “0 1/5 9-23 * * ?”
             * 每天9点到23点，每5分钟的第1分钟第0秒。如 9:01:00,9:06:00 ... 9:56:00 ... 23:51:00,23:56:00
            */
            var pk10Trigger = QuartzUtil.CreateTrigger($"{LotteryType.Pk10}_{Guid.NewGuid()}", "JinMaTrigger", "0 1/5 9-23 * * ?");

            await QuartzUtil.GetScheduler().ScheduleJob(stewardJob, pk10Trigger);
        }
    }
}
