namespace Colin.Lottery.Common.Models
{
    public class BetResult
    {
//        {"success":false,"msg":"余额不足","info":"","code":400}

        public bool Success { get; set; }
        public string Msg { get; set; }
        public string Info { get; set; }
        
        /// <summary>
        /// 状态码。
        /// 400 - 余额不足
        /// </summary>
        public int Code { get; set; }
    }
}