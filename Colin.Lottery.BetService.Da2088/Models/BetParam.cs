using System.Collections.Generic;
using Colin.Lottery.Models;
using Newtonsoft.Json;

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
        public static Dictionary<Pk10Rule, Dictionary<string, int>> BetRulePlayIdDict;

        static BetParam()
        {
            BetRulePlayIdDict = new Dictionary<Pk10Rule, Dictionary<string, int>>();

            /*
             * 初始化 单号1-10(仅冠军、亚军、第三名、第四名) 的 playId
             */
            int basePlayId = 501107;
            for (int x = (int) Pk10Rule.Champion; x < (int) Pk10Rule.BigOrSmall; x++)
            {
                var betNumPlayIdDict = new Dictionary<string, int>();
                for (int y = 0; y < 10; y++)
                {
                    int betNumber = y + 1; // 下注号码（1-9数字）
                    int playId = basePlayId + y; // playId 
                    betNumPlayIdDict.Add(betNumber.ToString(), playId);
                }

                BetRulePlayIdDict.Add((Pk10Rule) x, betNumPlayIdDict);

                // 个位重置到7，百分位加1，准备下一个玩法
                basePlayId += 100;
            }

            /*
             * 初始化 冠军大小、单双、龙虎 的 playId
             */
            basePlayId = 501100;
            BetRulePlayIdDict.Add(Pk10Rule.BigOrSmall, new Dictionary<string, int>
            {
                ["大"] = ++basePlayId,
                ["小"] = ++basePlayId
            });
            BetRulePlayIdDict.Add(Pk10Rule.OddOrEven, new Dictionary<string, int>
            {
                ["单"] = ++basePlayId,
                ["双"] = ++basePlayId
            });
            BetRulePlayIdDict.Add(Pk10Rule.DragonOrTiger, new Dictionary<string, int>
            {
                ["龙"] = ++basePlayId,
                ["虎"] = ++basePlayId
            });

            /*
             * 冠亚和值
             */
            basePlayId = 501005;
            var sumDict = new Dictionary<string, int>();
            for (int betNumber = 3; betNumber <= 19; betNumber++)
            {
                sumDict.Add(betNumber.ToString(), basePlayId);
                basePlayId++;
            }

            BetRulePlayIdDict.Add(Pk10Rule.Sum, sumDict);
        }

        public BetParam()
        {
        }

        public BetParam(long periodNo, Pk10Rule rule, string number, decimal money)
        {
            this.TurnNum = periodNo;
            
            string[] arrNums = number.Split(',');
            if (arrNums == null || arrNums.Length <= 0)
            {
                throw new ArgumentException();
            }
            
            switch (rule)
            {
                case Pk10Rule.Champion:
                case Pk10Rule.Second:
                case Pk10Rule.Third:
                case Pk10Rule.Fourth:
                   
                    BetParam.BetRulePlayIdDict[rule]
                    break;
            }
        }

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
        public int TotalMoney { get; set; }

        /// <summary>
        /// 含义暂时不明
        /// </summary>

        public int BetSrc { get; set; } = 0;

        /// <summary>
        /// 下注号码及金额
        /// </summary>
        [JsonIgnore]
        public List<BetBean> BetBeanList { get; set; }

        public class BetBean
        {
            public long playId { get; set; }
            public decimal money { get; set; }
        }
    }
}