using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Robin.Lottery.WebApp.Models
{
    /// <summary>
    ///     计划号码
    /// </summary>
    public class PlanNo
    {
        /// <summary>
        ///     追号号段
        /// </summary>
        public string Zhouqi { get; set; }

        /// <summary>
        ///     玩法（如：冠军单双）
        /// </summary>
        public string Apiname { get; set; }

        /// <summary>
        ///     下注号码 (或下一期预测号码)
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        ///     追号次数
        /// </summary>
        public int? Ready { get; set; }

        /// <summary>
        ///     结果（中 或 挂）
        /// </summary>
        public string Resalt { get; set; }

        /// <summary>
        ///     期号
        /// </summary>
        public int? Qihao { get; set; }

        /// <summary>
        ///     开奖号码
        /// </summary>
        public string Opencode { get; set; }

        #region 统计字段(当前期, 上期开奖号码, 胜率等)

        public int? NowQihao { get; set; }

        //public string A { get; set; }
        //public string B { get; set; }
        //public string C { get; set; }
        //public string L { get; set; }

        #endregion
    }
}