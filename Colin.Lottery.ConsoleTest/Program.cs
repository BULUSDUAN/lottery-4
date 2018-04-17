using Colin.Lottery.Analyzers;
using Colin.Lottery.Models;
using System;

namespace Colin.Lottery.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //GetForcastData();
            JinMaAnalyzer.Instance.Start();

            Console.ReadKey();
        }

        static async void GetForcastData()
        {
            var (Plan1, Plan2) = await JinMaAnalyzer.Instance.GetForcastData();
            Console.WriteLine("期号范围\t预测号码\t最大期号\t开奖号码\t根投次数\t是否中奖");
            ShowPlan(Plan1);
            ShowPlan(Plan2);
        }

        static void ShowPlan(IForcastPlanModel plan)
        {
            plan.ForcastData.ForEach(i =>
            {
                Console.WriteLine($"{i.PeriodRange}\t[{i.ForcastNo}]\t{i.LastPeriod}\t{i.DrawNo}\t{i.ChaseTimes}\t[{i.IsWin}]");
            });
            Console.WriteLine($"统计共{plan.TotalCount}期 ,中奖:{plan.WinCount}, 未中奖:{plan.LoseCount}, 中奖概率:{plan.WinProbability}\r\n");
        }
    }
}
