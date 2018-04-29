using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        CQSSC,

        /// <summary>
        /// 北京赛车PK10
        /// </summary>
        PK10
    }

    public enum CQSSCRule
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
    public enum PK10Rule
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

    public static class EnumExt
    {
        public static string ToStringName(this LotteryType lottery)
        {
            switch (lottery)
            {
                case LotteryType.PK10: return "北京PK10";
                case LotteryType.CQSSC: return "重庆时时彩";
            }

            throw new ArgumentException($"彩种 - “{lottery}” 暂不支持");
        }

        public static string ToStringName(this PK10Rule rule)
        {
            switch (rule)
            {
                case PK10Rule.Champion:
                    return "冠军";
                case PK10Rule.Second:
                    return "亚军";
                case PK10Rule.Third:
                    return "季军";
                case PK10Rule.Fourth:
                    return "第四名";
                case PK10Rule.BigOrSmall:
                    return "冠军大小";
                case PK10Rule.OddOrEven:
                    return "冠军单双";
                case PK10Rule.DragonOrTiger:
                    return "冠军龙虎";
                case PK10Rule.Sum:
                    return "冠亚和值";
            }

            throw new ArgumentException($"北京PK10玩法 - “{rule}” 暂不支持");
        }

        public static string ToStringName(this CQSSCRule rule)
        {
            switch (rule)
            {
                case CQSSCRule.OddOrEven:
                    return "总和单双";
                case CQSSCRule.BigOrSmall:
                    return "总和大小";
                case CQSSCRule.DragonOrTiger:
                    return "龙虎";
                case CQSSCRule.Last2Group:
                    return "后二组选";
                case CQSSCRule.Last3Group:
                    return "后三组选";
                case CQSSCRule.OneOddOrEven:
                    return "个位单双";
                case CQSSCRule.OneBigOrSmall:
                    return "个位大小";
                case CQSSCRule.One:
                    return "个位定位";
            }

            throw new ArgumentException($"重庆时时彩玩法 - “{rule}” 暂不支持");
        }

        public static string ToStringName(this int rule, LotteryType lottery)
        {
            switch (lottery)
            {
                case LotteryType.PK10:
                    return ((PK10Rule)rule).ToStringName();
                case LotteryType.CQSSC:
                    return ((CQSSCRule)rule).ToStringName();
            }

            throw new ArgumentException($"彩种 - “{lottery}” 暂不支持");
        }
    }
}
