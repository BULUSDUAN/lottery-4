using Colin.Lottery.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colin.Lottery.BetService.Da2088.Models
{
    /// <summary>
    /// 下注参数
    /// </summary>
    public class BetParam
    {
        /// <summary>
        /// {
        ///    "下注玩法" :
        ///         {
        ///             "下注号码" : playId
        ///         }
        /// }
        /// </summary>
        private static Dictionary<Pk10Rule, Dictionary<string, uint>> BetRulePlayIdDict;

        #region Constructors

        static BetParam()
        {
            BetRulePlayIdDict = new Dictionary<Pk10Rule, Dictionary<string, uint>>();

            /*
             * 初始化 单号1-10(仅冠军、亚军、第三名、第四名) 的 playId
             */
            uint basePlayId = 501107;
            for (ushort x = (ushort)Pk10Rule.Champion; x < (ushort)Pk10Rule.BigOrSmall; x++)
            {
                var betNumPlayIdDict = new Dictionary<string, uint>();
                for (uint y = 0; y < 10; y++)
                {
                    uint betNumber = y + 1; // 下注号码（1-9数字）
                    uint playId = basePlayId + y; // playId
                    betNumPlayIdDict.Add(betNumber.ToString(), playId);
                }

                BetRulePlayIdDict.Add((Pk10Rule)x, betNumPlayIdDict);

                // 个位重置到7，百分位加1，准备下一个玩法
                basePlayId += 100;
            }

            /*
             * 初始化 冠军大小、单双、龙虎 的 playId
             */
            basePlayId = 501100;
            BetRulePlayIdDict.Add(Pk10Rule.BigOrSmall, new Dictionary<string, uint>
            {
                ["大"] = ++basePlayId,
                ["小"] = ++basePlayId
            });
            BetRulePlayIdDict.Add(Pk10Rule.OddOrEven, new Dictionary<string, uint>
            {
                ["单"] = ++basePlayId,
                ["双"] = ++basePlayId
            });
            BetRulePlayIdDict.Add(Pk10Rule.DragonOrTiger, new Dictionary<string, uint>
            {
                ["龙"] = ++basePlayId,
                ["虎"] = ++basePlayId
            });

            /*
             * 冠亚和值
             */
            basePlayId = 501005;
            var sumDict = new Dictionary<string, uint>();
            for (int betNumber = 3; betNumber <= 19; betNumber++)
            {
                sumDict.Add(betNumber.ToString(), basePlayId);
                basePlayId++;
            }

            BetRulePlayIdDict.Add(Pk10Rule.Sum, sumDict);
        }

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
                var numberPlayIdDict = BetParam.BetRulePlayIdDict[rule];
                if (!numberPlayIdDict.ContainsKey(num))
                {
                    throw new ArgumentOutOfRangeException($"不支持下注单个下注号码必须在1~10单个数字");
                }
                uint playId = numberPlayIdDict[num];
                BetBeanList.Add(new BetBean(playId, money));
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

                if (m < 1 || m > 10) return false;
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