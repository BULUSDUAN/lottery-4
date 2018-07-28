using Colin.Lottery.Models;

namespace Colin.Lottery.Common.AutoBetSites
{
    public interface ISiteBet
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        bool Login();

        /// <summary>
        /// 投注
        /// </summary>
        /// <param name="periodNo">期号</param>
        /// <param name="rule">玩法</param>
        /// <param name="number">下注号码</param>
        /// <param name="money">单注下注金额</param>
        void Bet(long periodNo, Pk10Rule rule, string number, decimal money);
    }
}
