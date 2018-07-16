using System;
using System.Linq;
using Xunit;

using Colin.Lottery.Models;

namespace Colin.Lottery.Analyzers.Test
{
    public class JinMaAnalyzerTest
    {
        [Fact]
        public async void GetForcastDataTest()
        {
            var plans = await JinMaAnalyzer.Instance.GetForcastData();
            ShowPlan(plans.FirstOrDefault());
            ShowPlan(plans.LastOrDefault());
        }

        void ShowPlan(IForcastPlanModel plan)
        {
            plan.ForcastData.ForEach(i =>
            {
                Console.WriteLine($"{i.PeriodRange} [{i.ForcastNo}] {i.LastPeriod} {i.DrawNo} {i.ChaseTimes} [{i.IsWin}]");
            });
        }

        [Fact]
        public async void CalcuteScoreTest()
        {
            var plans = await JinMaAnalyzer.Instance.GetForcastData();
            JinMaAnalyzer.Instance.CalcuteScore(plans);
        }
    }
}
