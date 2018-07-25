using System;
using System.Collections.Generic;
using System.Text;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;
using Colin.Lottery.Common.Models;

namespace Colin.Lottery.Common.AutoBetSites
{
    /// <summary>
    /// www.da2088.com 自动投注
    /// </summary>
    public class SiteBet_Da2088 : SiteBetBase, ISiteBet
    {
        public SiteBet_Da2088() : base("https://www.da2088.com/")
        {
        }

        /// <summary>
        /// 登入系统
        /// </summary>
        /// <param name="account">账号名称</param>
        /// <param name="plainPassword">明文密码</param>
        public void Login(string account, string password)
        {
            if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException($"登录失败！账号或密码不能为空.");
            }

            // 1. 打开首页
            string content = _restHelper.Get("home/", null);

            // 2.登录
            //            account=song90273&password=c5cd3674f081bc66dc8e76dc2209a85d&pwdtext=200_daxl5306&loginSrc=0
            string md5Password = EncryptUtil.CreateMD5(password.Trim());
            var parameters = new Dictionary<string, object>
            {
                ["account"] = account,
                ["password"] = md5Password,
                ["pwdtext"] = password,
                ["loginSrc"] = "0"
            };

            var result = _restHelper.Post<LoginResult>("api/login.do", parameters);

            if (result.State != 1)
            {
                throw new ArgumentException("登录失败，请检查用户名或密码是否正确。");
            }

            // 3. 同意协议
            _restHelper.Get("game/", null);

            PrintLog($"{DateTime.Now}\t用户 {result.UserName} 登录成功! 余额：￥{result.Money}");
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
            PrintLog($"{Environment.NewLine}即将下注，玩法: [{rule.GetAttributeValue()}],号码: [{number}], 金额: {money}.");

            string url = $"bet/bet.do?_t={DateTime.Now.Ticks}";

            // 构建表单参数
            var betParam = new BetParam(periodNo, rule, number, money);

            var postBodyBuilder = new StringBuilder();
            postBodyBuilder.Append($"gameId={betParam.GameId}&turnNum={betParam.TurnNum}&totalNums={betParam.TotalNums}");
            postBodyBuilder.Append($"&totalMoney={betParam.TotalMoney}&betSrc={betParam.BetSrc}");

            for (int idx = 0; idx < betParam.BetBeanList.Count; idx++)
            {
                var bean = betParam.BetBeanList[idx];
                postBodyBuilder.Append($"&betBean[{idx}].playId={bean.PlayId}&betBean[{idx}].money={bean.Money}");
            }

            // 提交投注
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

    }
}
