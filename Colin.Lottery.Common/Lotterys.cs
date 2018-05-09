using System;
using System.Collections.Generic;
using Colin.Lottery.Models;

namespace Colin.Lottery.Common
{
    public static class Lotterys
    {
        public static List<LotteryType> LotteryTypes { get; set; } = new List<LotteryType> { LotteryType.PK10, LotteryType.CQSSC };
        public static List<Plan> Plans { get; set; } = new List<Plan> { Plan.PlanA, Plan.PlanB };
        public static List<PK10Rule> PK10Rules { get; set; } = new List<PK10Rule> { PK10Rule.Champion, PK10Rule.Second, PK10Rule.Third, PK10Rule.Fourth, PK10Rule.BigOrSmall, PK10Rule.OddOrEven, PK10Rule.DragonOrTiger, PK10Rule.Sum };
        public static List<CQSSCRule> CQSSCRules { get; set; } = new List<CQSSCRule> { CQSSCRule.OddOrEven, CQSSCRule.BigOrSmall, CQSSCRule.DragonOrTiger, CQSSCRule.Last2Group, CQSSCRule.Last3Group, CQSSCRule.OneOddOrEven, CQSSCRule.OneBigOrSmall, CQSSCRule.One };
    }
}
