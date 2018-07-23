using Xunit;
using Colin.Lottery.Models;

namespace Colin.Lottery.Collectors.Test
{
    public class JinMaCollecotrTest
    {
        [Fact]
        public async void GetDrawNoHistoryTest()
        {
            var pkHis = await JinMaCollector.Instance.GetDrawNoHistory(LotteryType.Pk10);
            var sshHis = await JinMaCollector.Instance.GetDrawNoHistory(LotteryType.Cqssc);
        }

        [Fact]
        public async void GetForcastDataTest()
        {
            var pk11 = await JinMaCollector.GetForcastData(LotteryType.Pk10, Planner.Planner1, (int)Pk10Rule.Champion);
            var pk15 = await JinMaCollector.GetForcastData(LotteryType.Pk10, Planner.Planner1, (int)Pk10Rule.BigOrSmall);
            var pk16 = await JinMaCollector.GetForcastData(LotteryType.Pk10, Planner.Planner1, (int)Pk10Rule.OddOrEven);
            var pk17 = await JinMaCollector.GetForcastData(LotteryType.Pk10, Planner.Planner1, (int)Pk10Rule.DragonOrTiger);
            var pk18 = await JinMaCollector.GetForcastData(LotteryType.Pk10, Planner.Planner1, (int)Pk10Rule.Sum);
        }
    }
}
