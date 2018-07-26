using System;
using System.Collections.Generic;
using System.Linq;

namespace Colin.Lottery.Models.BetService
{
    public class BetConfig
    {
        /// <summary>
        /// 模拟投注起始余额
        /// </summary>
        public float StartBalance { get; set; }

        
        
        /// <summary>
        /// 每期跟投单注 起始单注金额 1～10单号
        /// </summary>
        public float SingleNoEveryBetMoney { get; set; }
        
        /// <summary>
        /// 每期跟投单注 起始单注金额 两面盘
        /// </summary>
        public float TwoSidesEveryBetMoney { get; set; }

        /// <summary>
        /// 挂结束大额倍投 起始单注金额
        /// </summary>
        public float HighEndGuaBetMoney { get; set; }
        
        /// <summary>
        /// 挂结束小额倍投 起始单注金额
        /// </summary>
        public float LowEndGuaBetMoney { get; set; }
        
        /// <summary>
        /// 相同号码跟投 起始单注金额
        /// </summary>
        public float SameNumberBetMoney { get; set; }

        /// <summary>
        /// 赔率
        /// </summary>
        public float Odds { get; set; }

        /// <summary>
        /// 起投最小胜率 1～10单号
        /// </summary>
        public float SingleNoMinProbability { get; set; }
        
        /// <summary>
        /// 起投最小胜率 两面盘
        /// </summary>
        public float TwoSidesMinProbability { get; set; }
        
        /// <summary>
        /// 起投连挂次数
        /// </summary>
        public int MinGua { get; set; }

        /// <summary>
        /// 追挂结束每段下注金额递减比例
        /// </summary>
        public float DeltaReduce { get;set; }

        private string _timesFormula;
        private IEnumerable<int> _times;
        /// <summary>
        /// 背投公式
        /// </summary>
        public string TimesFormula
        {
            get => _timesFormula;
            set
            {
                _timesFormula = value;
                _times = _timesFormula.Split(',').Select(t=>Convert.ToInt32(t));
            }
        }
        
        
        public int this[int index]
        {
            get
            {
                var time= index % _times.Count()-1;
                return _times != null && _times.Any() ? _times.ElementAt(time<0?_times.Count()-1:time) : 1; 
            }
        }
    }
}