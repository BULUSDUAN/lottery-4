using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using static Colin.Lottery.Models.Pk10Rule;

namespace Colin.Lottery.Models
{
    /// <summary>
    /// 彩种
    /// </summary>
    public enum LotteryType
    {
        /// <summary>
        /// 重庆时时彩
        /// </summary>
        Cqssc,

        /// <summary>
        /// 北京赛车PK10
        /// </summary>
        Pk10
    }

    public enum CqsscRule
    {
        /// <summary>
        /// 单双
        /// </summary>
        OddOrEven = 1,
        /// <summary>
        /// 大小
        /// </summary>
        BigOrSmall = 2,
        /// <summary>
        /// 龙虎
        /// </summary>
        DragonOrTiger = 3,
        /// <summary>
        /// 后二组选
        /// </summary>
        Last2Group = 4,
        /// <summary>
        /// 后三组选
        /// </summary>
        Last3Group = 5,
        /// <summary>
        /// 个位单双
        /// </summary>
        OneOddOrEven = 6,
        /// <summary>
        /// 个位大小
        /// </summary>
        OneBigOrSmall = 7,
        /// <summary>
        /// 个位
        /// </summary>
        One = 8
    }

    /// <summary>
    /// 北京赛车玩法
    /// </summary>
    public enum Pk10Rule
    {
        /// <summary>
        /// 冠军
        /// </summary>
        Champion = 1,
        /// <summary>
        /// 亚军
        /// </summary>
        Second = 2,
        /// <summary>
        /// 季军
        /// </summary>
        Third = 3,
        /// <summary>
        /// 第四名
        /// </summary>
        Fourth = 4,
        /// <summary>
        /// 大小
        /// </summary>
        BigOrSmall = 5,
        /// <summary>
        /// 单双
        /// </summary>
        OddOrEven = 6,
        /// <summary>
        /// 冠军龙虎
        /// </summary>
        DragonOrTiger = 7,
        /// <summary>
        /// 冠亚军和值
        /// </summary>
        Sum = 8
    }

    public enum Plan
    {
        PlanA,
        PlanB
    }

    /// <summary>
    /// 计划员
    /// </summary>
    public enum Planner
    {
        /// <summary>
        /// 计划员1
        /// </summary>
        Planner1 = 1,

        /// <summary>
        /// 计划员2
        /// </summary>
        Planner2 = 2
    }

    /// <summary>
    /// 邮件内容格式
    /// </summary>
    public enum MailContentType
    {
        /// <summary>
        /// 文本格式
        /// </summary>
        Plain,
        /// <summary>
        /// HTML格式
        /// </summary>
        Html
    }

    public static class EnumExt
    {
        public static string ToStringName(this LotteryType lottery)
        {
            switch (lottery)
            {
                case LotteryType.Pk10: return "北京PK10";
                case LotteryType.Cqssc: return "重庆时时彩";
            }

            throw new ArgumentException($"彩种 - “{lottery}” 暂不支持");
        }


        private static readonly Dictionary<Pk10Rule, string> Pk10Rules = new Dictionary<Pk10Rule, string>
        {
            [Champion] = "冠军",
            [Second]="亚军",
            [Third]="季军",
            [Fourth]="第4名",
            [BigOrSmall]="冠军大小",
            [OddOrEven]="冠军单双",
            [DragonOrTiger]="冠军龙虎",
            [Sum]="冠亚和值"
        };
        private static readonly Dictionary<CqsscRule,string> CqsscRules=new Dictionary<CqsscRule, string>
        {
            [CqsscRule.OddOrEven]="总和单双",
            [CqsscRule.BigOrSmall]="总和大小",
            [CqsscRule.DragonOrTiger]="龙虎",
            [CqsscRule.Last2Group]="后二组选",
            [CqsscRule.Last3Group]="后三组选",
            [CqsscRule.OneOddOrEven]="个位单双",
            [CqsscRule.OneBigOrSmall]="个位大小",
            [CqsscRule.One]="个位定位"
        };

        public static string ToStringName(this Pk10Rule rule) => Pk10Rules[rule];
        public static string ToStringName(this CqsscRule rule) => CqsscRules[rule];
        public static string ToStringName(this int rule, LotteryType lottery)
        {
            switch (lottery)
            {
                case LotteryType.Pk10:
                    return ((Pk10Rule)rule).ToStringName();
                case LotteryType.Cqssc:
                    return ((CqsscRule)rule).ToStringName();
            }

            throw new ArgumentException($"彩种 - “{lottery}” 暂不支持");
        }
        
        public static Pk10Rule ToPk10Rule(this string rule)=>Pk10Rules.Keys.FirstOrDefault(r => Pk10Rules[r] == rule);
        public static CqsscRule ToCqsscRule(this string rule)=>CqsscRules.Keys.FirstOrDefault(r => CqsscRules[r] == rule);

        private static readonly Dictionary<Planner, Plan> PlannerPlan=new Dictionary<Planner, Plan>
        {
            [Planner.Planner1]=Plan.PlanA,
            [Planner.Planner2]=Plan.PlanB
        };
        public static Plan GetPlan(this Planner planner) => PlannerPlan[planner];
        
        public static int ToInt(this LotteryType lottery) => (int)lottery;

        public static int ToInt(this Plan plan) => (int)plan;

        public static int ToInt(this Pk10Rule rule) => (int)rule;

        public static int ToInt(this CqsscRule rule) => (int)rule;
    }
}
