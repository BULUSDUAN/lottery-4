using System;
using System.IO;
using System.Linq;
using Colin.Lottery.Analyzers;
using Colin.Lottery.Common;
using Colin.Lottery.Common.Notification;
using Colin.Lottery.Models;
using Colin.Lottery.Models.Notification;
using Colin.Lottery.Utils;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;

namespace Colin.Lottery.AutoBetService
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
            
            Console.WriteLine("OK");
            
            Console.ReadKey();
        }

        private static async void Test()
        {
            var plans = await JinMaAnalyzer.Instance.GetForcastData(LotteryType.Pk10, 2);
            JinMaAnalyzer.Instance.CalcuteScore(ref plans, true);
            await MailNotify.NotifyAsync(new MailNotifyModel(LotteryType.Pk10, 2, Plan.PlanB,plans.Plan2, plans.Plan1.ForcastDrawNo));
        }
    }
}
