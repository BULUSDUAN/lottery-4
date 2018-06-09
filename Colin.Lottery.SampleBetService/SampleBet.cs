using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

using Colin.Lottery.Models;
using Colin.Lottery.Models.BetService;
using Colin.Lottery.SampleBetService.DataModels;
using Colin.Lottery.Utils;

namespace Colin.Lottery.SampleBetService
{
    public static class SampleBet
    {
        public static async Task StartConnection()
        {
            var hubUrls = ConfigUtil.GetAppSettings<SourceHubUrls>("SourceHubUrls");
            var connection = new HubConnectionBuilder()
                .WithUrl(hubUrls.Pk10)
                .Build();
            connection.On<List<IForcastModel>>("ShowPlans", BetPk10);

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


        private static BetConfig betConfig= ConfigUtil.GetAppSettings<BetConfig>("BetConfig"); 
        private static void BetPk10(List<IForcastModel> forcastData)
        {
            foreach (var forcast in forcastData)
            {
                //跳过和值
                if(forcast.Rule.ToPk10Rule()==Pk10Rule.Sum)
                    continue;

                
                //每期倍投
//                new BeAllRecord(LotteryType.Pk10,(int)forcast.Rule.ToPk10Rule(),forcast.)
                
                //3期倍投

                //连挂结束倍投
            }
        }
    }
}