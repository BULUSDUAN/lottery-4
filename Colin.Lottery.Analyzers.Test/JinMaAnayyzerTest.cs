using System;
using System.Linq;
using Xunit;

using Colin.Lottery.Models;

namespace Colin.Lottery.Analyzers.Test
{
    public class JinMaAnalyzerTest
    {
        [Fact]
        public async void GetForecastDataTest()
        {
            var plans = await JinMaAnalyzer.Instance.GetForecastData();
            ShowPlan(plans.FirstOrDefault());
            ShowPlan(plans.LastOrDefault());
        }

        void ShowPlan(IForecastPlanModel plan)
        {
            plan.ForecastData.ForEach(i =>
            {
                Console.WriteLine($"{i.PeriodRange} [{i.ForecastNo}] {i.LastPeriod} {i.DrawNo} {i.ChaseTimes} [{i.IsWin}]");
            });
        }

        [Fact]
        public async void CalculateScoreTest()
        {
            var plans = await JinMaAnalyzer.Instance.GetForecastData();
            JinMaAnalyzer.Instance.CalcuteScore(plans);
        }
    }
}
