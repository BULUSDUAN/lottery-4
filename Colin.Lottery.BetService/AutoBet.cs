using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Colin.Lottery.BetService.DataModels;
using Colin.Lottery.Models;
using Colin.Lottery.Models.BetService;
using Colin.Lottery.Utils;


namespace Colin.Lottery.BetService
{
    public static class AutoBet
    {
        private static readonly BetContext Db = new BetContext();

        public static async Task StartConnection()
        {
            var hubUrls = ConfigUtil.GetAppSettings<SourceHubUrls>("SourceHubUrls");
            var connection = new HubConnectionBuilder()
                .WithUrl(hubUrls.Pk10)
                .Build();
            connection.ServerTimeout = TimeSpan.FromMinutes(10);

            connection.On<JArray>("ShowPlans", BetPk10);

            try
            {
                await connection.StartAsync();
                await connection.InvokeAsync("RegisterAllRules");
            }
            catch (Exception ex)
            {
                LogUtil.Fatal($"模拟下注启动失败,错误消息:{ex.Message}\r\n堆栈内容:{ex.StackTrace}");
            }
        }


        private static void BetPk10(JArray arg)
        {
            /*
            using (var db = new BetContext())
            {
                var lastRecord = await db.BetRecord.LastOrDefaultAsync();
                var balance = lastRecord?.Balance ?? (decimal) BetConfig.StartBalance;
                if (lastRecord?.Balance < 0)
                    return;

                var plans = JsonConvert.DeserializeObject<List<JinMaForcastModel>>(arg.ToString());
                foreach (IForcastModel plan in plans)
                {
                    var info = GetBetInfo(plan);
                    if (info.BetMoney <= 0)
                        continue;

                    if ((decimal) info.BetMoney > balance)
                        continue;

                    var betMoneyAll = (decimal) info.BetMoney * 5;
                    balance -= betMoneyAll;
                    await db.BetRecord.AddAsync(new BetRecord(LotteryType.Pk10, (int) plan.Rule.ToPk10Rule(),
                        plan.Plan, plan.LastPeriod, plan.ForcastNo, plan.ChaseTimes, betMoneyAll,
                        BetConfig.Odds, info.BetType, balance));


                    //TODO:下注  彩种、玩法、期号、号码、下注金额
                }

                await db.SaveChangesAsync();
            }
            */
            
            //加锁去异步，防止多线程并发下注引起余额等字段重入
            lock (Db)
            {
                var lastRecord =  Db.BetRecord.LastOrDefault();
                var balance = lastRecord?.Balance ?? (decimal) BetConfig.StartBalance;
                if (lastRecord?.Balance < 0)
                    return;

                var plans = JsonConvert.DeserializeObject<List<JinMaForcastModel>>(arg.ToString());
                foreach (IForcastModel plan in plans)
                {
                    var info = GetBetInfo(plan);
                    if (info.BetMoney <= 0)
                        continue;

                    if ((decimal) info.BetMoney > balance)
                        continue;

                    var betMoneyAll = (decimal) info.BetMoney * 5;
                    balance -= betMoneyAll;
                    Db.BetRecord.Add(new BetRecord(LotteryType.Pk10, (int) plan.Rule.ToPk10Rule(),
                        plan.Plan, plan.LastPeriod, plan.ForcastNo, plan.ChaseTimes, betMoneyAll,
                        BetConfig.Odds, info.BetType, balance));


                    //TODO:下注  彩种、玩法、期号、号码、下注金额
                }

                 Db.SaveChanges();
            }
        }

        private static readonly BetConfig BetConfig = ConfigUtil.GetAppSettings<BetConfig>("BetConfig");

        private static (float BetMoney, BetType BetType) GetBetInfo(IForcastModel plan)
        {
            float betMoney = 0, money;
            var type = BetType.Every;

            //跳过和值
            if (plan.Rule.ToPk10Rule() == Pk10Rule.Sum)
                return (betMoney, type);

            /*
             *  投注策略
             *
             * 1.胜率高于 MinWinProbability 则每期跟投(最多连续跟投9期)，出现9期未中(3连挂)停止跟投，等待连挂结束，采用追挂结束模式大额跟投
             * 2.胜率低于 MinWinProbability，如果符合连挂结束则采用追挂结束模式小额跟投
             * 
             * ---- 追连挂结束模式 ----
             *
             * 1)等待N连挂结束之后可以相对安全的跟投N-1期，如果N-1中再次出现挂则停止追挂结束模式
             * 2）N-1期倍投随跟投期数递减
             *
             * 3.前四名玩法中出现号码重复度100%时小额跟投3期
             * 4.如果一期同时满足以上多种情况，则采用以上最大的下注金额
             */

            if (plan.WinProbability >= BetConfig.MinWinProbability)
            {
                if (plan.KeepGuaCnt >= 3)
                {
                    if (plan.KeepGuaCnt > BetConfig.MinGua)
                    {
                        //HighEndGua
                        money = BetConfig.HighEndGuaBetMoney *
                                (1 - (plan.ChaseTimesAfterEndGua - 2) * BetConfig.DeltaReduce) *
                                BetConfig[plan.ChaseTimes];
                        CompareBetInfo(BetType.HighEndGua);
                    }
                }
                else
                {
                    if (plan.KeepGuaingCnt > 0)
                    {
                        if (plan.KeepGuaingCnt >= 3)
                        {
                            //超3连挂进行中，暂停投注等待挂结束
                        }
                        else
                        {
                            //Every
                            money = BetConfig.EveryBetMoney * BetConfig[plan.KeepGuaingCnt * 3 + plan.ChaseTimes];
                            CompareBetInfo(BetType.Every);
                        }
                    }
                    else
                    {
                        //Every
                        money = BetConfig.EveryBetMoney * BetConfig[plan.ChaseTimes];
                        CompareBetInfo(BetType.Every);
                    }
                }
            }
            else
            {
                if (plan.KeepGuaCnt >= BetConfig.MinGua)
                {
                    //LowEndGua
                    money = BetConfig.LowEndGuaBetMoney *
                            (1 - (plan.ChaseTimesAfterEndGua - 2) * BetConfig.DeltaReduce) * BetConfig[plan.ChaseTimes];
                    CompareBetInfo(BetType.LowEndGua);
                }
            }

            if (plan.RepetitionScore >= 100)
            {
                //SameNumber
                money = BetConfig[plan.ChaseTimes] * BetConfig.SameNumberBetMoney;
                CompareBetInfo(BetType.SameNumber);
            }

            return (betMoney, type);

            void CompareBetInfo(BetType betType)
            {
                if (betMoney >= money)
                    return;

                betMoney = money;
                type = betType;
            }
        }
    }
}