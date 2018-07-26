using Colin.Lottery.Common.Models;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Colin.Lottery.Common.AutoBetSites
{
    /// <summary>
    /// www.da2088.com 自动投注
    /// </summary>
    public class SiteBet_Da2088 : SiteBetBase, ISiteBet
    {
        private string _account;
        private string _password;

        /// <summary>
        /// True 表示登录超时，或者异地登录被踢下线
        /// </summary>
        public bool LoginTimeout { get; set; }

        public SiteBet_Da2088(string account, string password) : base("https://www.da2088.com/")
        {
            this._account = account;
            this._password = password;
        }

        /// <summary>
        /// 登入系统
        /// </summary>
        /// <param name="account">账号名称</param>
        /// <param name="plainPassword">明文密码</param>
        public void Login()
        {
            if (string.IsNullOrWhiteSpace(_account) || string.IsNullOrWhiteSpace(_password))
            {
                throw new ArgumentNullException($"登录失败！账号或密码不能为空.");
            }

            // 1. 打开首页
            string content = _restHelper.Get("home/", null);

            // 2.登录
            //            account=song90273&password=c5cd3674f081bc66dc8e76dc2209a85d&pwdtext=200_daxl5306&loginSrc=0
            string md5Password = EncryptUtil.CreateMD5(_password.Trim());
            var parameters = new Dictionary<string, object>
            {
                ["account"] = _account,
                ["password"] = md5Password,
                ["pwdtext"] = _password,
                ["loginSrc"] = "0"
            };

            var result = _restHelper.Post<LoginResult>("api/login.do", parameters);

            if (result.State != 1)
            {
                throw new ArgumentException("登录失败，请检查用户名或密码是否正确。");
            }

            LoginTimeout = false;

            // 3. 同意协议
            _restHelper.Get("game/", null);
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
            LotteryData lotteryData = GetLotteryData();
            PrintLog($"{Environment.NewLine}即将下注，账户余额:￥ [{lotteryData.Balance}], 玩法: [{rule.GetAttributeValue()}],号码: [{number}], 下注金额: {money}.");

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
                    if (result.Msg.Contains("该账号在异地登陆"))
                    {
                        LoginTimeout = true;
                        throw new ArgumentException("该账号在异地登陆, 请重试！");
                    }
                    else
                    {
                        throw new ArgumentException($"{result.Msg}, 状态码: {result.Code}.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取账户余额等数据
        /// </summary>
        /// <returns></returns>
        public LotteryData GetLotteryData()
        {
            string url = $"game/getLotteryData.do?_t={DateTime.Now.Ticks}&gameId=50";

            LotteryData result = null;

            try
            {
                result = _restHelper.Get<LotteryData>(url);
            }
            catch (Exception ex)
            {
                result = new LotteryData();
                Console.WriteLine($">>> 获取账号余额失败，详情: {ex.Message}");
            }
            return result;
        }
    }
}