using System.Configuration;
using Xunit;
using Colin.Lottery.Models;

namespace Colin.Lottery.Collectors.Test
{
    public class JinMaCollecotrTest
    {
        [Fact]
        public async void GetDrawNoHistoryTest()
        {
            var collector = new JinMaCollector();
            var pkHis = await collector.GetDrawNoHistory(LotteryType.Pk10);
            var sshHis = await collector.GetDrawNoHistory(LotteryType.Cqssc);
        }

        [Fact]
        public async void GetForecastDataTest()
        {
            var pk11 = await JinMaCollector.GetForecastData(LotteryType.Pk10, Planner.Planner1,
                (int) Pk10Rule.Champion);
            var pk15 = await JinMaCollector.GetForecastData(LotteryType.Pk10, Planner.Planner1,
                (int) Pk10Rule.BigOrSmall);
            var pk16 = await JinMaCollector.GetForecastData(LotteryType.Pk10, Planner.Planner1,
                (int) Pk10Rule.OddOrEven);
            var pk17 = await JinMaCollector.GetForecastData(LotteryType.Pk10, Planner.Planner1,
                (int) Pk10Rule.DragonOrTiger);
            var pk18 = await JinMaCollector.GetForecastData(LotteryType.Pk10, Planner.Planner1, (int) Pk10Rule.Sum);
        }
    }
}