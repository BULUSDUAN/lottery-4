using Colin.Lottery.Models;
using System;
using Xunit;

namespace Colin.Lottery.Analyzers.Test
{
    public class JinMaAnalyzerTest
    {
        [Fact]
        public async void GetForcastDataTest()
        {
            var plans = await JinMaAnalyzer.Instance.GetForcastData();
            ShowPlan(plans.Plan1);
            ShowPlan(plans.Plan2);
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
            JinMaAnalyzer.Instance.CalcuteScore(ref plans, true);
        }
    }
}
