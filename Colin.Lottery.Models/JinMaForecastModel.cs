using System;
using System.Collections.Generic;
using System.Linq;

namespace Colin.Lottery.Models
{
    public class JinMaForecastModel : ForecastModel
    {
        #region 15期数据

        public string ZhouQi { set => PeriodRange = value; }

        public string ApiName { set => Rule = value; }
        
        public string Number { set => ForecastNo = value; }

        public int Ready { set => ChaseTimes = value; }

        public string Resalt { set => IsWin = value.Equals("..") ? null : (bool?)value.Equals("中"); }

        public long? QiHao { set => LastPeriod = value ?? Convert.ToInt32(PeriodRange.Split('-').FirstOrDefault()) + ChaseTimes - 1; }

        public string OpenCode { set => DrawNo = value; }

        #endregion

        #region 汇总数据

        public long NowQiHao { get; set; }

        public int A { get; set; }

        public int B { get; set; }

        public int C { get; set; }

        public string L { get; set; }        

        #endregion
    }

    public class JinMaForecastPlanModel : ForecastPlanModel
    {
        public List<JinMaForecastModel> ApiCode
        {
            set
            {
                ForecastData = new List<IForecastModel>();
                ForecastData.AddRange(value.Take(value.Count() - 1));
                
                var summary = value.LastOrDefault();
                LastDrawnPeriod = summary.NowQiHao;
                LastDrawNo = summary.DrawNo;
                TotalCount = summary.A;
                WinCount = summary.B;
                LoseCount = summary.C;
                WinProbability = float.Parse(summary.L.TrimEnd('%')) / 100;
            }
        }
    }
}
