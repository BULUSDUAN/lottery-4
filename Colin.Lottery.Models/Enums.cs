using System;

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

        public static string ToStringName(this Pk10Rule rule)
        {
            switch (rule)
            {
                case Pk10Rule.Champion:
                    return "冠军";
                case Pk10Rule.Second:
                    return "亚军";
                case Pk10Rule.Third:
                    return "季军";
                case Pk10Rule.Fourth:
                    return "第4名";
                case Pk10Rule.BigOrSmall:
                    return "冠军大小";
                case Pk10Rule.OddOrEven:
                    return "冠军单双";
                case Pk10Rule.DragonOrTiger:
                    return "冠军龙虎";
                case Pk10Rule.Sum:
                    return "冠亚和值";
            }

            throw new ArgumentException($"北京PK10玩法 - “{rule}” 暂不支持");
        }

        public static string ToStringName(this CqsscRule rule)
        {
            switch (rule)
            {
                case CqsscRule.OddOrEven:
                    return "总和单双";
                case CqsscRule.BigOrSmall:
                    return "总和大小";
                case CqsscRule.DragonOrTiger:
                    return "龙虎";
                case CqsscRule.Last2Group:
                    return "后二组选";
                case CqsscRule.Last3Group:
                    return "后三组选";
                case CqsscRule.OneOddOrEven:
                    return "个位单双";
                case CqsscRule.OneBigOrSmall:
                    return "个位大小";
                case CqsscRule.One:
                    return "个位定位";
            }

            throw new ArgumentException($"重庆时时彩玩法 - “{rule}” 暂不支持");
        }

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

        public static int ToInt(this LotteryType lottery) => (int)lottery;

        public static int ToInt(this Plan plan) => (int)plan;

        public static int ToInt(this Pk10Rule rule) => (int)rule;

        public static int ToInt(this CqsscRule rule) => (int)rule;
    }
}
