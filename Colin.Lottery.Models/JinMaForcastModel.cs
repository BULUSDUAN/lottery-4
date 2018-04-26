using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colin.Lottery.Models
{
    public class JinMaForcastModel : ForcastModel
    {
        //public string ApiName { get; set; }

        public string ZhouQi { set { this.PeriodRange = value; } }

        public string Number { set { this.ForcastNo = value; } }

        public int Ready { set { this.ChaseTimes = value; } }

        public string Resalt { set { this.IsWin = value.Equals("..") ? null : (bool?)value.Equals("中"); } }

        public long? QiHao { set { this.LastPeriod = value ?? Convert.ToInt32(this.PeriodRange.Split('-').FirstOrDefault()) + this.ChaseTimes - 1; } }

        public string OpenCode { set { this.DrawNo = value; } }

        public long NowQiHao { get; set; }

        public int A { get; set; }

        public int B { get; set; }

        public int C { get; set; }

        public string L { get; set; }

    }

    public class JinMaForcastPlanModel : ForcastPlanModel
    {
        public List<JinMaForcastModel> APICode
        {
            set
            {
                this.ForcastData = new List<IForcastModel>();
                this.ForcastData.AddRange(value.Take(value.Count() - 1));
                var summary = value.LastOrDefault();
                this.LastDrawedPeriod = summary.NowQiHao;
                this.LastDrawNo = summary.DrawNo;
                this.TotalCount = summary.A;
                this.WinCount = summary.B;
                this.LoseCount = summary.C;
                this.WinProbability = float.Parse(summary.L.TrimEnd('%')) / 100;
            }
        }
    }
}
