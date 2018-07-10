using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using Colin.Lottery.Analyzers;
using Colin.Lottery.Common.Notification;
using Colin.Lottery.Common.Scheduler;
using Colin.Lottery.Models;
using Colin.Lottery.Models.Notification;
using Colin.Lottery.Utils;
using Colin.Lottery.WebApp.Hubs;

namespace Colin.Lottery.WebApp.Services
{
    public class JinMaStrategyService : StrategyService<JinMaStrategyService>
    {
        private readonly IHubContext<PK10Hub> _pk10Context;

        public JinMaStrategyService()
        {
            _pk10Context = Startup.GetService<IHubContext<PK10Hub>>();
        }

        public override async Task Start(bool startWhenBreakGua = false)
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

        public override void Start(Dictionary<LotteryType, List<int>> typeRules, bool startWhenBreakGua = false)
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
                var ng = tempJob.JobAndTriggerNames();

                await QuartzUtil.ScheduleSimpleJob(tempJob, async () =>
                {
                    //超时自毁
                    if ((DateTime.Now - timestamp).TotalMinutes > 5)
                        await QuartzUtil.DeleteJob(ng.JobName, ng.JobGroup);

                    //扫水
                    var plans = await JinMaAnalyzer.Instance.GetForcastData(LotteryType.Pk10, (int)rule);
                    /*
                    * 如果目标网站接口正常，每次都可以扫到结果，即使没有更新最新期预测数据。所以如果没有扫水结果，说明目标网站接口出错
                    */
                    if (plans == null || plans.Count < 2 || plans.Any(p => p == null))
                    {
                        //await _PK10Context.Clients.Group(rule.ToString()).SendAsync("NoResult");
                        await _pk10Context.Clients.Groups(new List<string> { rule.ToString(), "AllRules" }).SendAsync("NoResult", rule.ToStringName());
                        LogUtil.Warn("目标网站扫水接口异常，请尽快检查恢复");
                        return;
                    }

                    JinMaAnalyzer.Instance.CalcuteScore(plans, startWhenBreakGua);

                    if (plans.Any(p => p.LastDrawedPeriod + 1 < periodNo)) return;

                    //1.推送完整15期计划
                    await _pk10Context.Clients.Group(rule.ToString()).SendAsync("ShowPlans", plans);

                    //2.推送最新期计划
                    await _pk10Context.Clients.Group("AllRules").SendAsync("ShowPlans", plans.Select(p => p.ForcastData.LastOrDefault()));

                    /* 暂停推送 连挂邮件通知 和 浏览器通知

                    //3.推送连挂消息
                    if (rule != Pk10Rule.Sum)
                    {
                        plans.ForEach(async p =>
                        {
                            if (p.KeepGuaCnt > 1)
                                await MailNotify.NotifyAsync(new MailNotifyModel(LotteryType.Pk10, (int)rule, Plan.PlanA, p, p.ForcastDrawNo));
                        });
                    }

                    //4.广播通知消息
                    plans.ForEach(async p => await NotifyContext.Clients.Clients(NotifyHub.GetConnectionIds(p.Score))
                            .SendAsync("Notify", new List<string> { CreateNotification(LotteryType.Pk10, (int)rule, Plan.PlanA, p.ForcastDrawNo, p.Score) }));
                            
                     */

                    await QuartzUtil.DeleteJob(ng.JobName, ng.JobGroup);
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
