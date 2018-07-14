using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

using Colin.Lottery.Models;
using Colin.Lottery.Models.BetService;
using Colin.Lottery.SampleBetService.DataModels;
using Colin.Lottery.Utils;

namespace Colin.Lottery.SampleBetService
{
    public static class SampleBet
    {
        public static async Task<Action> StartConnection()
        {
            var hubUrls = ConfigUtil.GetAppSettings<SourceHubUrls>("SourceHubUrls");
            var connection = new HubConnectionBuilder()
                .WithUrl(hubUrls.Pk10)
                //.AddMessagePackProtocol()
                .Build();
            connection.ServerTimeout = TimeSpan.FromMinutes(10);

            connection.On<List<IForcastModel>>("ShowPlans", obj => Console.WriteLine("Fuck"));
            connection.On<DateTime>("ShowDate", time => Console.WriteLine(time));

            try
            {
                await connection.StartAsync();
                await connection.InvokeAsync("RegisterAllRules");
                await connection.InvokeAsync("GetDate");
            }
            catch (Exception ex)
            {
                LogUtil.Fatal($"模拟下注启动失败,错误消息:{ex.Message}\r\n堆栈内容:{ex.StackTrace}");
            }

           Action func=async () => await connection.InvokeAsync("GetDate");
            return func;
        }


        private static readonly BetConfig BetConfig = ConfigUtil.GetAppSettings<BetConfig>("BetConfig");
        private static async void BetPk10(List<IForcastModel> forcastData)
        {
            Console.WriteLine("ShowPlans");
            return;

            foreach (var forcast in forcastData)
            {
                //跳过和值
                if (forcast.Rule.ToPk10Rule() == Pk10Rule.Sum)
                    continue;

                using (var db = new SampleBetContext())
                {
                    if (forcast.WinProbability >= BetConfig.MinWinProbability)
                    {
                        //1.每期倍投
                        var betMoneyAll = (decimal)0.01 * BetConfig.BetMoney * BetConfig[forcast.CurrentGuaCnt * 3 + forcast.ChaseTimes];
                        var balanceAll = db.BetAll.Any() ? db.BetAll.LastOrDefault().Balance : BetConfig.StartBalance - betMoneyAll;
                        if (balanceAll > 0)
                            db.BetAll.Add(new BetAllRecord(LotteryType.Pk10, forcast.Rule.ToPk10Rule().ToInt(), forcast.Plan, forcast.LastPeriod,
                                forcast.ForcastNo, forcast.ChaseTimes, betMoneyAll, BetConfig.Odds, balanceAll));


                        //2.三期倍投
                        var betMoneyAll3 = BetConfig.BetMoney * BetConfig[forcast.ChaseTimes];
                        var balanceAll3 = db.BetAll3.Any() ? db.BetAll3.LastOrDefault().Balance : BetConfig.StartBalance - betMoneyAll3;
                        if (balanceAll3 > 0)
                            db.BetAll3.Add(new BetAll3Record(LotteryType.Pk10, forcast.Rule.ToPk10Rule().ToInt(), forcast.Plan, forcast.LastPeriod,
                                forcast.ForcastNo, forcast.ChaseTimes, betMoneyAll3, BetConfig.Odds, balanceAll3));
                    }

                    if (forcast.KeepGuaCnt > 0)
                    {
                        //3.连挂结束倍投
                        var betMoneyGua = 10 * BetConfig.BetMoney * BetConfig[forcast.ChaseTimes];
                        var balanceGua = db.BetGua.Any() ? db.BetGua.LastOrDefault().Balance : BetConfig.StartBalance - betMoneyGua;
                        if (balanceGua > 0)
                            db.BetGua.Add(new BetGuaRecord(LotteryType.Pk10, forcast.Rule.ToPk10Rule().ToInt(), forcast.Plan, forcast.LastPeriod,
                                forcast.ForcastNo, forcast.ChaseTimes, betMoneyGua, BetConfig.Odds, balanceGua));
                    }

                    await db.SaveChangesAsync();
                }
            }
        }
    }
}