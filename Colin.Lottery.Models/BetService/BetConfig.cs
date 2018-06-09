using System;
using System.Collections.Generic;
using System.Linq;

namespace Colin.Lottery.Models.BetService
{
    public class BetConfig
    {
        /// <summary>
        /// 起始金额
        /// </summary>
        public int StartBalance { get; set; }
        /// <summary>
        /// 单注金额
        /// </summary>
        public int BetMoney { get; set; }

        /// <summary>
        /// 赔率
        /// </summary>
        public float Odds { get; set; }

        /// <summary>
        /// 最小胜率
        /// </summary>
        public float MinWinProbability { get; set; }

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
        
        
        public int this[int index] => _times != null && _times.Any() && index < _times.Count() ? _times.ElementAt(index) : 1;
    }
}