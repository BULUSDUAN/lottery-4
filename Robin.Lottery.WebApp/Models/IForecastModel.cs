namespace Robin.Lottery.WebApp.Models
{
    /// <summary>
    /// 计划号码相关数据
    /// </summary>
    public class ForecastModel
    {
        /// <summary>
        /// 期号范围
        /// </summary>
        public string PeriodRange { get; set; }

        /// <summary>
        /// 预测号码
        /// </summary>
        public string ForecastNo { get; set; }

        /// <summary>
        /// 追号次数
        /// </summary>
        public int ChaseTimes { get; set; }

        /// <summary>
        /// 是否中奖
        /// </summary>
        public bool? IsWin { get; set; }

        /// <summary>
        /// 截至期号
        /// </summary>
        public long LastPeriod { get; set; }

        /// <summary>
        /// 开奖号码
        /// </summary>
        public string OpenNo { get; set; }
    }
}