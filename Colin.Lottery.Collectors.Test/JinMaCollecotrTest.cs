using System;
using Xunit;

using Colin.Lottery.Collectors;
using Colin.Lottery.Models;

namespace Colin.Lottery.Collectors.Test
{
    public class JinMaCollecotrTest
    {
        [Fact]
        public async void GetDrawNoHistoryTest()
        {
            var pkHis = await JinMaCollector.Instance.GetDrawNoHistory(LotteryType.PK10);
            var sshHis = await JinMaCollector.Instance.GetDrawNoHistory(LotteryType.CQSSC);
        }

        [Fact]
        public async void GetForcastDataTest()
        {
            var pk11 = await JinMaCollector.Instance.GetForcastData(LotteryType.PK10, Planner.Planner1, (int)PK10Rule.Champion);
            var pk15 = await JinMaCollector.Instance.GetForcastData(LotteryType.PK10, Planner.Planner1, (int)PK10Rule.BigOrSmall);
            var pk16 = await JinMaCollector.Instance.GetForcastData(LotteryType.PK10, Planner.Planner1, (int)PK10Rule.OddOrEven);
            var pk17 = await JinMaCollector.Instance.GetForcastData(LotteryType.PK10, Planner.Planner1, (int)PK10Rule.DragonOrTiger);
            var pk18 = await JinMaCollector.Instance.GetForcastData(LotteryType.PK10, Planner.Planner1, (int)PK10Rule.Sum);
        }
    }
}
