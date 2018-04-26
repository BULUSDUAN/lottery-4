using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colin.Lottery.Models
{
    public abstract class ForcastModel : IForcastModel
    {
        public string PeriodRange { get; set; }
        public string ForcastNo { get; set; }
        public int ChaseTimes { get; set; }
        public bool? IsWin { get; set; }
        public long LastPeriod { get; set; }
        public string DrawNo { get; set; }
    }

    public abstract class ForcastPlanModel : IForcastPlanModel
    {
        public List<IForcastModel> ForcastData { get; set; }
        public long LastDrawedPeriod { get; set; }
        public string LastDrawNo { get; set; }
        public int TotalCount { get; set; }
        public int WinCount { get; set; }
        public int LoseCount { get; set; }
        public float WinProbability { get; set; }
        public float Score { get; set; }
        public int KeepGuaCnt { get; set; }
        public int KeepHisGuaCnt { get; set; }
    }
}
