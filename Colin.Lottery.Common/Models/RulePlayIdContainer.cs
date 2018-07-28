using Colin.Lottery.Common.AutoBetSites;
using Colin.Lottery.Models;
using System.Collections.Generic;
using System.Linq;

namespace Colin.Lottery.Common.Models
{
    public class Da2088Helper
    {
        /// <summary>
        /// {
        ///    "下注玩法" :
        ///         {
        ///             "下注号码" : playId
        ///         }
        /// }
        /// </summary>
        private static List<RulePlayId> RulePlayIdContainer = new List<RulePlayId>();

        static Da2088Helper()
        {
            InitList();
        }

        private static void InitList()
        {
            /*
            * 初始化 单号1-10(仅冠军、亚军、第三名、第四名) 的 playId
            */
            uint basePlayId = 501107;
            for (ushort x = (ushort)Pk10Rule.Champion; x < (ushort)Pk10Rule.BigOrSmall; x++)
            {
                for (uint y = 0; y < 10; y++)
                {
                    uint betNumber = y + 1; // 下注号码（1-9数字）
                    uint playId = basePlayId + y; // playId

                    RulePlayIdContainer.Add(new RulePlayId((Pk10Rule)x, betNumber.ToString(), playId));
                }

                // 个位重置到7，百分位加1，准备下一个玩法
                basePlayId += 100;
            }

            /*
             * 初始化 冠军大小、单双、龙虎 的 playId
             */
            basePlayId = 501100;

            RulePlayIdContainer.Add(new RulePlayId(Pk10Rule.BigOrSmall, "大", ++basePlayId));
            RulePlayIdContainer.Add(new RulePlayId(Pk10Rule.BigOrSmall, "小", ++basePlayId));

            RulePlayIdContainer.Add(new RulePlayId(Pk10Rule.OddOrEven, "单", ++basePlayId));
            RulePlayIdContainer.Add(new RulePlayId(Pk10Rule.OddOrEven, "双", ++basePlayId));

            RulePlayIdContainer.Add(new RulePlayId(Pk10Rule.DragonOrTiger, "龙", ++basePlayId));
            RulePlayIdContainer.Add(new RulePlayId(Pk10Rule.DragonOrTiger, "虎", ++basePlayId));

            /*
             * 冠亚和值
             */
            basePlayId = 501005;
            var sumDict = new Dictionary<string, uint>();
            for (int betNumber = 3; betNumber <= 19; betNumber++)
            {
                RulePlayIdContainer.Add(new RulePlayId(Pk10Rule.Sum, betNumber.ToString(), basePlayId));

                basePlayId++;
            }
        }

        public static SiteBet_Da2088 SiteBetDa2088 = new SiteBet_Da2088("", "");

        public static RulePlayId GetPlayId(Pk10Rule rule, string num)
        {
            return RulePlayIdContainer.FirstOrDefault(x => x.Rule == rule && x.Number == num);
        }

        public static RulePlayId GetRuleAndNum(uint playId)
        {
            return RulePlayIdContainer.FirstOrDefault(x => x.PlayId == playId);
        }

    }

    public class RulePlayId
    {
        public RulePlayId(Pk10Rule rule, string num, uint playId)
        {
            this.Rule = rule;
            this.Number = num;
            this.PlayId = playId;
        }

        public Pk10Rule Rule { get; set; }
        public string Number { get; set; }
        public uint PlayId { get; set; }
    }
}