using System;
using System.Collections.Generic;
using Colin.Lottery.BetService.Da2088.Models;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;

namespace Colin.Lottery.BetService.Da2088
{
    public class AutoBetMain
    {
        private RestClientHelper _restHelper;

        public AutoBetMain()
        {
            string domain = "https://www.da2088.com/";
            _restHelper = new RestClientHelper(domain);
        }

        /// <summary>
        /// 登入系统
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Login()
        {
            var a = BetParam.BetRulePlayIdDict;

            // 1. 打开首页
            string content = _restHelper.Get("home/", null);

            // 2.登录
//            account=song90273&password=c5cd3674f081bc66dc8e76dc2209a85d&pwdtext=200_daxl5306&loginSrc=0
            var parameters = new Dictionary<string, object>
            {
                ["account"] = "song90273",
                ["password"] = "c5cd3674f081bc66dc8e76dc2209a85d",
                ["pwdtext"] = "200_daxl5306",
                ["loginSrc"] = "0"
            };

            var result = _restHelper.Post<LoginResult>("api/login.do", parameters);

            if (result.State != 1)
            {
                throw new ArgumentException("登录失败，请检查用户名或密码是否正确。");
            }


            // 3. 同意协议
            _restHelper.Get("game/", null);

            Console.WriteLine($"{DateTime.Now}\t登录成功! 余额：￥{result.Money}");
        }

        /// <summary>
        /// 下注
        /// </summary>
        /// <param name="periodNo">期号</param>
        /// <param name="rule">玩法枚举</param>
        /// <param name="number">下注号码</param>
        /// <param name="money">单个号码金额</param>
        public void Bet(long periodNo, Pk10Rule rule, string number, decimal money)
        {
            string url = $"bet/bet.do?_t={DateTime.Now.Ticks}";

            BetParam betParam = new BetParam(periodNo, rule, number, money);
            
        }

        private void BetSingleNumber()
        {
        }
    }
}