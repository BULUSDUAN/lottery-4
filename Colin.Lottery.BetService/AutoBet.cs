using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colin.Lottery.Models;
using Colin.Lottery.Models.BetService;
using Colin.Lottery.Utils;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Colin.Lottery.BetService
{
    public static class AutoBet
    {
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


        private static readonly BetConfig BetConfig = ConfigUtil.GetAppSettings<BetConfig>("BetConfig");
        private static async void BetPk10(JArray arg)
        {
            var plans = JsonConvert.DeserializeObject<List<JinMaForcastModel>>(arg.ToString());
            foreach (var plan in plans)
            {
                //跳过和值
                if (plan.Rule.ToPk10Rule() == Pk10Rule.Sum)
                    return;
                
                /*
                 *  投注策略
                 *
                 * 1.胜率高于 MinWinProbability 则每期跟投(最多连续跟投9期)，出现9期未中(3连挂)停止跟投，等待连挂结束，采用追挂结束模式大额跟投
                 * 2.胜率低于 MinWinProbability，如果符合连挂结束则采用追挂结束模式小额跟投
                 * 
                 * ---- 追连挂结束模式 ----
                 * 
                 */
            }
            
           

            //foreach (var forcast in forcastData)
            //{
            //    //跳过和值
            //    if (forcast.Rule.ToPk10Rule() == Pk10Rule.Sum)
            //        continue;

            //    using (var db = new SampleBetContext())
            //    {
            //        if (forcast.WinProbability >= BetConfig.MinWinProbability)
            //        {
            //            //1.每期倍投
            //            var betMoneyAll = (decimal)0.01 * BetConfig.BetMoney * BetConfig[forcast.CurrentGuaCnt * 3 + forcast.ChaseTimes];
            //            var balanceAll = db.BetAll.Any() ? db.BetAll.LastOrDefault().Balance : BetConfig.StartBalance - betMoneyAll;
            //            if (balanceAll > 0)
            //                db.BetAll.Add(new BetAllRecord(LotteryType.Pk10, forcast.Rule.ToPk10Rule().ToInt(), forcast.Plan, forcast.LastPeriod,
            //                    forcast.ForcastNo, forcast.ChaseTimes, betMoneyAll, BetConfig.Odds, balanceAll));


            //            //2.三期倍投
            //            var betMoneyAll3 = BetConfig.BetMoney * BetConfig[forcast.ChaseTimes];
            //            var balanceAll3 = db.BetAll3.Any() ? db.BetAll3.LastOrDefault().Balance : BetConfig.StartBalance - betMoneyAll3;
            //            if (balanceAll3 > 0)
            //                db.BetAll3.Add(new BetAll3Record(LotteryType.Pk10, forcast.Rule.ToPk10Rule().ToInt(), forcast.Plan, forcast.LastPeriod,
            //                    forcast.ForcastNo, forcast.ChaseTimes, betMoneyAll3, BetConfig.Odds, balanceAll3));
            //        }

            //        if (forcast.KeepGuaCnt > 0)
            //        {
            //            //3.连挂结束倍投
            //            var betMoneyGua = 10 * BetConfig.BetMoney * BetConfig[forcast.ChaseTimes];
            //            var balanceGua = db.BetGua.Any() ? db.BetGua.LastOrDefault().Balance : BetConfig.StartBalance - betMoneyGua;
            //            if (balanceGua > 0)
            //                db.BetGua.Add(new BetGuaRecord(LotteryType.Pk10, forcast.Rule.ToPk10Rule().ToInt(), forcast.Plan, forcast.LastPeriod,
            //                    forcast.ForcastNo, forcast.ChaseTimes, betMoneyGua, BetConfig.Odds, balanceGua));
            //        }

            //        await db.SaveChangesAsync();
            //    }
            //}
        }
    }
}