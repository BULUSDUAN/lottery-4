using System.Linq;
using System.Collections.Generic;

namespace Colin.Lottery.Models
{
    public abstract class ForcastModel : IForcastModel
    {
        public string Rule { get; set; }
        public string PeriodRange { get; set; }
        public string ForcastNo { get; set; }
        public int ChaseTimes { get; set; }
        public bool? IsWin { get; set; }
        public long LastPeriod { get; set; }
        public string DrawNo { get; set; }
        public int KeepGuaCnt { get; set; }
        public float GuaScore { get; set; }
        public float RepetitionScore { get; set; }
        public float BetChaseScore { get; set; }
        public float Score { get; set; }
        public float WinProbability { get; set; }
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
        public float GuaScore { get; set; }
        public float RepetitionScore { get; set; }
        public float BetChaseScore { get; set; }
        public float Score { get; set; }
        public bool KeepGua { get; set; }
        public int KeepGuaCnt { get; set; }
        public int KeepHisGuaCnt { get; set; }

        public string ForcastDrawNo => ForcastData.LastOrDefault()?.ForcastNo;
    }
}
