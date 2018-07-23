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
            var plans = JsonConvert.DeserializeObject<List<JinMaForcastModel>>(arg.ToString());

            //加锁去异步，防止多线程并发下注引起余额等字段重入
            lock (Db)
            {
                //自动开奖
                //上期所有玩法开奖
                var records = Db.BetRecord.Where(p =>
                    !p.IsDrawed && p.PeriodNo == plans.FirstOrDefault().LastDrawedPeriod);
                if (records.Any())
                {
                    foreach (var record in records)
                    {
                        record.DrawNo = plans.FirstOrDefault().LastDrawNo;
                        var isWin = CheckIsWin((Pk10Rule)record.Rule, record.BetNo, record.DrawNo);
                        record.IsWin = isWin;
                        if (isWin)
                        {
                            var addMoney = record.BetMoney * (decimal)record.Odds;
                            record.WinMoney = addMoney - record.BetMoney;
                            //中奖后将中奖金额加到最后一条记录的可用余额上
                            Db.BetRecord.LastOrDefault().Balance += addMoney;
                        }
                        else
                        {
                            record.WinMoney = -record.BetMoney;
                        }

                        record.IsDrawed = true;
                        record.DrawTime = DateTime.Now;
                    }
                }

                //自动下注
                var balance = Db.BetRecord.LastOrDefault()?.Balance ?? (decimal)BetConfig.StartBalance;
                foreach (IForcastModel plan in plans)
                {
                    //var lastPeriod = Db.BetRecord.LastOrDefault();
                    //var balance = lastPeriod?.Balance ?? (decimal)BetConfig.StartBalance;

                    //余额不足
                    if (balance < 0)
                        continue;

                    //暂不下注
                    var info = GetBetInfo(plan);
                    if (info.BetMoney <= 0)
                        continue;

                    //余额不足
                    if ((decimal)info.BetMoney > balance)
                        continue;

                    var betMoneyAll = (decimal)info.BetMoney * 5;
                    balance -= betMoneyAll;

                    //TODO:下注  彩种、玩法、期号、号码、下注金额


                    Db.BetRecord.Add(new BetRecord(LotteryType.Pk10, (int)plan.Rule.ToPk10Rule(),
                        plan.Plan, plan.LastDrawedPeriod + 1, plan.ForcastNo, plan.ChaseTimes, betMoneyAll,
                        BetConfig.Odds, info.BetType, balance));
                }

                Db.SaveChanges();
            }
        }

        private static bool CheckIsWin(Pk10Rule rule, string betNo, string drawNo)
        {
            if (string.IsNullOrWhiteSpace(betNo) || string.IsNullOrWhiteSpace(drawNo))
                return false;

            var betNos = betNo.Split(',');
            var winNos = drawNo.Split(',');
            if (!betNos.Any() || winNos.Length < 10)
                return false;
            var champion = Convert.ToInt32(winNos[0]);
            var second = Convert.ToInt32(winNos[1]);
            var tenth = Convert.ToInt32(winNos[9]);

            switch (rule)
            {
                case Pk10Rule.Champion:
                case Pk10Rule.Second:
                case Pk10Rule.Third:
                case Pk10Rule.Fourth:
                    return betNos.Contains(winNos[(int)rule-1]);
                case Pk10Rule.BigOrSmall:
                    return string.Equals(betNos[0], champion > 5 ? "大" : "小");
                case Pk10Rule.OddOrEven:
                    return string.Equals(betNos[0], champion % 2 != 0 ? "单" : "双");
                case Pk10Rule.DragonOrTiger:
                    return string.Equals(betNos[0], champion > tenth ? "龙" : "虎");
                case Pk10Rule.Sum:
                    return betNos.Contains((champion + second).ToString().PadLeft(2, '0'));
            }

            return false;
        }

        private static readonly BetConfig BetConfig = ConfigUtil.GetAppSettings<BetConfig>("BetConfig");

        private static (float BetMoney, BetType BetType) GetBetInfo(IForcastModel plan)
        {
            float betMoney = 0, money;
            var type = BetType.Every;

            //跳过和值
            var ruleType = plan.Rule.ToPk10Rule().ToPk10RuleType();
            if (ruleType == Pk10RuleType.FirstAndSecondGroup)
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

            var probability=ruleType == Pk10RuleType.SingleNo ? BetConfig.SingleNoMinProbability:BetConfig.TwoSidesMinProbability;
            if (plan.WinProbability >= probability)
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
                    var everyBetMoney = ruleType == Pk10RuleType.SingleNo
                        ? BetConfig.SingleNoEveryBetMoney
                        : BetConfig.TwoSidesEveryBetMoney;
                    if (plan.KeepGuaingCnt > 0)
                    {
                        if (plan.KeepGuaingCnt >= 3)
                        {
                            //超3连挂进行中，暂停投注等待挂结束
                        }
                        else
                        {
                            //Every
                            money = everyBetMoney * BetConfig[plan.KeepGuaingCnt * 3 + plan.ChaseTimes];
                            CompareBetInfo(BetType.Every);
                        }
                    }
                    else
                    {
                        //Every
                        money = everyBetMoney * BetConfig[plan.ChaseTimes];
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
                if (ruleType==Pk10RuleType.SingleNo)
                {
                    //SameNumber
                    money = BetConfig[plan.ChaseTimes] * BetConfig.SameNumberBetMoney;
                    CompareBetInfo(BetType.SameNumber);
                }
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