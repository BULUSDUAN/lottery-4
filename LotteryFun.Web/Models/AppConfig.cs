namespace LotteryFun.Web.Models
{
    public sealed class AppConfig
    {
        public string QuartzCronExp { get; set; }

        /// <summary>
        /// 一字长龙个数
        /// </summary>
        public int MinLongQueue { get; set; }

        /// <summary>
        /// 连挂个数
        /// </summary>
        public int MinGuaQueue { get; set; }

        public string SourceSiteDomain { get; set; }

        public string TgBotToken { get; set; }
        
        public string TgChatId { get; set; }
    }
}