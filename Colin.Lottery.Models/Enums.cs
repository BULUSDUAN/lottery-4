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

    /// <summary>
    /// 计划员
    /// </summary>
    public enum Planner
    {
        /// <summary>
        /// 计划员1
        /// </summary>
        Planner1=1,

        /// <summary>
        /// 计划员2
        /// </summary>
        Planner2=2
    }
}
