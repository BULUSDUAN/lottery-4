using System;

namespace Colin.Lottery.BetService.Da2088.Models
{
    public class LoginResult
    {
//        {"token":"0pzcWWi8/lXHygqsHs9K6YgUwHm6n7+4a7E/P/JHPETCCowBUegeZA==","serverTime":"2018-07-23 15:47:24",
// "userId":474397,"userName":"song90273","fullName":"宋兴征","loginTime":"2018-07-23 15:47:24","lastLoginTime":"2018-07-23 15:04:45",
// "money":0.0,"email":"","rechLevel":"0","hasFundPwd":true,"testFlag":0,"updatePw":0,"updatePayPw":0,"state":1}

        public string Token { get; set; }
        public DateTime ServerTime { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public decimal Money { get; set; }
        public bool HasFundPwd { get; set; }
        public int State { get; set; }
    }
}