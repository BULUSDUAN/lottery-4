using Colin.Lottery.Models;
using Colin.Lottery.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colin.Lottery.Common.Models
{
    /// <summary>
    /// 下注参数
    /// </summary>
    public class BetParam
    {
        #region Constructors

        /// <param name="periodNo">期号</param>
        /// <param name="rule">玩法枚举</param>
        /// <param name="number">下注号码</param>
        /// <param name="money">单个号码金额</param>
        public BetParam(long periodNo, Pk10Rule rule, string number, decimal money)
        {
            this.TurnNum = periodNo;

            string[] arrNums = CheckNumberValid(rule, number);

            BetBeanList = new List<BetBean>();
            foreach (string num in arrNums)
            {
                RulePlayId rulePlayId = Da2088Helper.GetPlayId(rule, num);
                if (rulePlayId == null)
                {
                    throw new ArgumentOutOfRangeException($"玩法 {rule.GetAttributeValue()}， 下注号码: {num}， 下注号码不合规则！");
                }

                BetBeanList.Add(new BetBean(rulePlayId.PlayId, money));
            }

            // 初始化总金额
            TotalNums = arrNums.Length;
            TotalMoney = arrNums.Length * money;
        }

        #endregion Constructors

        #region Private Methods

        /// <summary>
        /// 检查下注号码是否有效
        /// </summary>
        /// <param name="number"></param>
        /// <param name="arrNums"></param>
        private string[] CheckNumberValid(Pk10Rule rule, string number)
        {
            string[] arrNums = number.Split(',');

            if (arrNums == null || arrNums.Length <= 0)
            {
                throw new ArgumentException($"下注失败! \t 详情: 下注号码({number})为空.");
            }

            // 委托，测试号码是否有效
            Func<string, bool> IsValidNumberFunc = (n) =>
            {
                if (rule >= Pk10Rule.BigOrSmall && rule <= Pk10Rule.DragonOrTiger) return true;

                bool parseResult = int.TryParse(n, out int m);
                if (!parseResult) return false;

                if (m < 1 || m > 19) return false;
                return true;
            };

            if (!arrNums.Any(IsValidNumberFunc))
            {
                throw new ArgumentException($"下注失败! \t 详情: 下注号码({number})格式不正确，请检查后重试!");
            }

            return arrNums;
        }

        #endregion Private Methods

        #region Public Properties

        /// <summary>
        /// 50 - PK10
        /// </summary>
        public int GameId { get; set; } = 50;

        /// <summary>
        /// 下注期号
        /// </summary>
        public long TurnNum { get; set; }

        /// <summary>
        /// 总共下注号码的数量
        /// </summary>
        public int TotalNums { get; set; }

        /// <summary>
        /// 总下注金额
        /// </summary>
        public decimal TotalMoney { get; set; }

        /// <summary>
        /// 含义暂时不明
        /// </summary>

        public int BetSrc { get; set; } = 0;

        /// <summary>
        /// 下注号码及金额
        /// </summary>
        public List<BetBean> BetBeanList { get; }

        #endregion Public Properties

        public class BetBean
        {
            public BetBean(uint playId, decimal money)
            {
                this.PlayId = playId;
                this.Money = money;
            }

            public uint PlayId { get; set; }
            public decimal Money { get; set; }
        }
    }
}