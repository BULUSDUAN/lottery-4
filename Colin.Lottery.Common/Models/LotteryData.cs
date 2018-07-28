namespace Colin.Lottery.Common.Models
{
    public class LotteryData
    {
        /// <summary>
        /// 账号余额
        /// </summary>
        public float Balance { get; set; }

        /// <summary>
        /// 未结金额
        /// </summary>
        public float UnbalancedMoney { get; set; }

        /// <summary>
        /// 当前彩种输赢
        /// </summary>
        public float TotalTotalMoney { get; set; }
        public object UserBetWinList { get; set; }
        public object UserBetNew { get; set; }
        public object PushMessage { get; set; }
        public int UserNoticeMsg { get; set; }
    }
}