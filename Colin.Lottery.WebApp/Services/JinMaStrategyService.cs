using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.SignalR;

using Quartz;

using Colin.Lottery.Analyzers;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;
using Colin.Lottery.Common;

using Colin.Lottery.WebApp.Hubs;

namespace Colin.Lottery.WebApp
{
    public class JinMaStrategyService : StrategyService<JinMaStrategyService>
    {
        readonly IHubContext<PK10Hub> _PK10Context;

        public JinMaStrategyService()
        {
            _PK10Context = Startup.GetService<IHubContext<PK10Hub>>();
        }

        public async override Task Start(bool startWhenBreakGua = false)
        {
            await StartPK10(PK10Rule.Champion, startWhenBreakGua);
            await StartPK10(PK10Rule.Second, startWhenBreakGua);
            await StartPK10(PK10Rule.Third, startWhenBreakGua);
            await StartPK10(PK10Rule.Fourth, startWhenBreakGua);
            await StartPK10(PK10Rule.BigOrSmall, startWhenBreakGua);
            await StartPK10(PK10Rule.OddOrEven, startWhenBreakGua);
            await StartPK10(PK10Rule.DragonOrTiger, startWhenBreakGua);
            await StartPK10(PK10Rule.Sum, startWhenBreakGua);
        }

        public override void Start(Dictionary<LotteryType, List<int>> typeRules, bool startWhenBreakGua = false)
        {
            if (typeRules == null)
                return;

            foreach (var type in typeRules.Keys)
            {
                switch (type)
                {
                    case LotteryType.CQSSC:
                        typeRules[type].ForEach(r =>
                        {
                            switch ((CQSSCRule)r)
                            {
                                case CQSSCRule.OddOrEven:
                                    break;
                                case CQSSCRule.BigOrSmall:
                                    break;
                                case CQSSCRule.DragonOrTiger:
                                    break;
                                case CQSSCRule.Last2Group:
                                    break;
                                case CQSSCRule.Last3Group:
                                    break;
                                case CQSSCRule.OneOddOrEven:
                                    break;
                                case CQSSCRule.OneBigOrSmall:
                                    break;
                                case CQSSCRule.One:
                                    break;
                            }
                        });
                        break;
                    case LotteryType.PK10:
                        typeRules[type].ForEach(async r => await StartPK10((PK10Rule)r, startWhenBreakGua));
                        break;
                }
            }
        }

        async Task StartPK10(PK10Rule rule, bool startWhenBreakGua)
        {
            var prefix = $"{LotteryType.PK10}_{rule}";
            //大管家Job，负责创建每期的扫水Job
            var stewardJob = QuartzUtil.CreateSimpleJob($"{prefix}_Steward_Job", $"{LotteryType.PK10}_JobGroup", async () =>
            {
                var timestamp = DateTime.Now;
                var periodNo = PK10Scheduler.Instance.GetPeriodNo(timestamp);
                var tempJob = $"{prefix}_Scan_{periodNo}";
                var ng = tempJob.JobAndTriggerNames();

                await QuartzUtil.ScheduleSimpleJob(tempJob, async () =>
                {
                    //超时自毁
                    if ((DateTime.Now - timestamp).TotalMinutes > 5)
                        await QuartzUtil.DeleteJob(ng.JobName, ng.JobGroup);

                    //扫水
                    var plans = await JinMaAnalyzer.Instance.GetForcastData(LotteryType.PK10, (int)rule);
                    JinMaAnalyzer.Instance.CalcuteScore(ref plans, startWhenBreakGua);
                    (IForcastPlanModel planA, IForcastPlanModel planB) = plans;

                    if (planA.LastDrawedPeriod + 1 >= periodNo && planB.LastDrawedPeriod + 1 >= periodNo)
                    {
                        //推送到所有关注PK10当前玩法的客户端
                        await _PK10Context.Clients.Clients(PK10Hub.GetConnectionIds(rule)).SendAsync("ShowPlans", plans);

                        //广播通知消息
                        await NotifyContext.Clients.Clients(NotifyHub.GetConnectionIds(planA.Score)).SendAsync("Notify", new List<string> { CreateNotification(LotteryType.PK10, (int)rule, Plan.PlanA, planA.ForcastDrawNo, planA.Score) });
                        await NotifyContext.Clients.Clients(NotifyHub.GetConnectionIds(planB.Score)).SendAsync("Notify", new List<string> { CreateNotification(LotteryType.PK10, (int)rule, Plan.PlanB, planB.ForcastDrawNo, planB.Score) });

                        await QuartzUtil.DeleteJob(ng.JobName, ng.JobGroup);
                    }
                }, "0/5 * * * * ? *");
            });

            /* 
             * 北京赛车时间为每天9:00到23:50，每5分钟开一期，共179期
             * 
             * “0 1/5 9-23 * * ?”
             * 每天9点到23点，每5分钟的第1分钟第0秒。如 9:01:00,9:06:00 ... 9:56:00 ... 23:51:00,23:56:00
            */
            var _pk10Trigger = QuartzUtil.CreateTrigger($"{LotteryType.PK10}_{Guid.NewGuid()}", "JinMaTrigger", "0 1/5 9-23 * * ?");

            await QuartzUtil.GetScheduler().ScheduleJob(stewardJob, _pk10Trigger);
        }
    }
}
