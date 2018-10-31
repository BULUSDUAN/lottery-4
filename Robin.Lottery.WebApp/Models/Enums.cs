using System.ComponentModel;

namespace Robin.Lottery.WebApp.Models
{
    /// <summary>
    ///     北京赛车玩法
    /// </summary>
    public enum Pk10Rule
    {
        /// <summary>
        /// 冠军
        /// </summary>
//        [Description("冠军")] Champion = 1,

        /// <summary>
        /// 亚军
        /// </summary>
//        [Description("亚军")] Second = 2,

        /// <summary>
        /// 季军
        /// </summary>
//        [Description("季军")] Third = 3,

        /// <summary>
        /// 第四名
        /// </summary>
//        [Description("第四名")] Fourth = 4,

        /// <summary>
        ///     冠军大小
        /// </summary>
        [Description("冠军大小")] BigOrSmall = 5,

        /// <summary>
        ///     冠军单双
        /// </summary>
        [Description("冠军单双")] OddOrEven = 6,

        /// <summary>
        ///     冠军龙虎
        /// </summary>
        [Description("冠军龙虎")] DragonOrTiger = 7

        /// <summary>
        /// 冠亚军和值
        /// </summary>
//        [Desc("冠亚军和值")] Sum = 8
    }

    /// <summary>
    /// 计划员
    /// </summary>
    public enum Planner
    {
        [Description("计划A")] A = 1,

        [Description("计划B")] B = 2
    }
}