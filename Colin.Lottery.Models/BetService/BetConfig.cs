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
        public int StartBalance { get; set; }

        /// <summary>
        /// 挂结束大额倍投 起始单注金额
        /// </summary>
        public float HighGuaEndBet { get; set; }

        /// <summary>
        /// 赔率
        /// </summary>
        public float Odds { get; set; }

        /// <summary>
        /// 起投最小胜率
        /// </summary>
        public float MinWinProbability { get; set; }
        
        /// <summary>
        /// 起投连挂次数
        /// </summary>
        public int MinGua { get; set; }

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
        
        
        public int this[int index] => _times != null && _times.Any() ? _times.ElementAt(index%_times.Count()) : 1;
    }
}