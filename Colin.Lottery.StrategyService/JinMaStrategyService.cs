using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Quartz;

using Colin.Lottery.Analyzers;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;

namespace Colin.Lottery.StrategyService
{
    public class JinMaStrategyService : StrategyService<JinMaStrategyService>
    {
        ITrigger _pk10Trigger, _sscTrigger;

        public JinMaStrategyService()
        {
            //TODO:根据彩种完善Cron表达式

            var _pk10Trigger = QuartzUtil.CreateTrigger($"{LotteryType.PK10}", "JinMaTrigger", "", DateTime.Today.AddHours(9).AddMinutes(5));
            var _sscTrigger = QuartzUtil.CreateTrigger($"{LotteryType.PK10}", "JinMaTrigger", "", DateTime.Today.AddHours(10));
        }

        public async override void Start()
        {
            await StartPK10Champion();
        }

        public override void Start(Dictionary<LotteryType, List<int>> typeRules)
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
                                default:
                                    break;
                            }
                        });
                        break;
                    case LotteryType.PK10:
                        typeRules[type].ForEach(async r =>
                        {
                            switch ((PK10Rule)r)
                            {
                                case PK10Rule.Champion:
                                    await StartPK10Champion();
                                    break;
                                case PK10Rule.Second:
                                    break;
                                case PK10Rule.Third:
                                    break;
                                case PK10Rule.Fourth:
                                    break;
                                case PK10Rule.BigOrSmall:
                                    break;
                                case PK10Rule.OddOrEven:
                                    break;
                                case PK10Rule.DragonOrTiger:
                                    break;
                                case PK10Rule.Sum:
                                    break;
                                default:
                                    break;
                            }
                        });
                        break;
                    default:
                        break;
                }
            }
        }

        async Task StartPK10Champion(bool startWhenBreakGua = false)
        {

            var job = QuartzUtil.CreateSimpleJob($"{LotteryType.PK10}{PK10Rule.Champion}", $"{LotteryType.PK10}", async () =>
               {
                   var plans = await JinMaAnalyzer.Instance.GetForcastData();
                   JinMaAnalyzer.Instance.CalcuteScore(ref plans, startWhenBreakGua);

                   //TODO:分析数据分发
               });

            await QuartzUtil.GetScheduler().ScheduleJob(job, _pk10Trigger);
        }
    }
}
