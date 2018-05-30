using System.Collections.Generic;
using Colin.Lottery.Models;

namespace Colin.Lottery.Common
{
    public static class Lotterys
    {
        public static IEnumerable<LotteryType> LotteryTypes { get; } =
            new List<LotteryType> {LotteryType.Pk10, LotteryType.Cqssc};
        
        public static IEnumerable<Plan> Plans { get; } = new List<Plan> { Plan.PlanA, Plan.PlanB };
        
        public static List<Pk10Rule> Pk10Rules { get; } = new List<Pk10Rule>
        {
            Pk10Rule.Champion,
            Pk10Rule.Second,
            Pk10Rule.Third,
            Pk10Rule.Fourth,
            Pk10Rule.BigOrSmall,
            Pk10Rule.OddOrEven,
            Pk10Rule.DragonOrTiger,
            Pk10Rule.Sum
        };
        
        public static IEnumerable<CqsscRule> CqsscRules { get; } = new List<CqsscRule>
        {
            CqsscRule.OddOrEven,
            CqsscRule.BigOrSmall,
            CqsscRule.DragonOrTiger,
            CqsscRule.Last2Group,
            CqsscRule.Last3Group,
            CqsscRule.OneOddOrEven,
            CqsscRule.OneBigOrSmall,
            CqsscRule.One
        };
    }
}
