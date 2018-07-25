using Colin.Lottery.BetService.Da2088.Models;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;
using System;
using System.Collections.Generic;
using System.Text;

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

            PrintLog($"{DateTime.Now}\t登录成功! 余额：￥{result.Money}");
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
            PrintLog($"{Environment.NewLine}即将下注，玩法: {rule.GetAttributeValue()},号码: {number}, 金额: {money}.");

            string url = $"bet/bet.do?_t={DateTime.Now.Ticks}";

            var betParam = new BetParam(periodNo, rule, number, money);

            var postBodyBuilder = new StringBuilder();
            postBodyBuilder.Append($"gameId={betParam.GameId}&turnNum={betParam.TurnNum}&totalNums={betParam.TotalNums}");
            postBodyBuilder.Append($"&totalMoney={betParam.TotalMoney}&betSrc={betParam.BetSrc}");

            for (int idx = 0; idx < betParam.BetBeanList.Count; idx++)
            {
                var bean = betParam.BetBeanList[idx];
                postBodyBuilder.Append($"&betBean[{idx}].playId={bean.PlayId}&betBean[{idx}].money={bean.Money}");
            }

            try
            {
                BetResult result = _restHelper.Post<BetResult>(url, postBodyBuilder.ToString());

                if (!result.Success)
                {
                    PrintLog($"ERROR : {result.Msg}, 状态码: {result.Code}.");
                }
                else
                {
                    PrintLog($"SUCCESS : 下注接口返回消息: {result.Msg}");
                }
            }
            catch (Exception ex)
            {
                PrintLog($"EXCEPTION : 下注出现异常，导致下注失败, 详情: {ex.Message}");
            }
        }

        private void PrintLog(string logFormat, params object[] args)
        {
            Console.WriteLine(string.Format(logFormat, args));
        }
    }
}