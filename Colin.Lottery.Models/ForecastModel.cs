using System.Linq;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace Colin.Lottery.Models
{
    public abstract class ForecastModel : ForecastSharedModel, IForecastModel
    {
        public string PeriodRange { get; set; }
        public override string Rule { get; set; }
        public string ForecastNo { get; set; }
        public int ChaseTimes { get; set; }
        public bool? IsWin { get; set; }
        public long LastPeriod { get; set; }
        public string DrawNo { get; set; }

        public override long LastDrawnPeriod { get; set; }
        public override string LastDrawNo { get; set; }
        public override int TotalCount { get; set; }
        public override int WinCount { get; set; }
        public override int LoseCount { get; set; }
        public override float WinProbability { get; set; }

        public override Plan Plan { get; set; }

        public override int KeepGuaCnt { get; set; }
        public override int ChaseTimesAfterEndGua { get; set; }
        public override int KeepGuaingCnt { get; set; }
        public override float GuaScore { get; set; }
        public override float RepetitionScore { get; set; }
        public override float BetChaseScore { get; set; }
        public override float Score { get; set; }
    }

    public abstract class ForecastPlanModel : ForecastSharedModel, IForecastPlanModel
    {
        public List<IForecastModel> ForecastData { get; set; }
        private IForecastModel CurrentForecast => ForecastData?.LastOrDefault();

        private long _lastDrawnPeriod;

        public override long LastDrawnPeriod
        {
            get => _lastDrawnPeriod;
            set
            {
                _lastDrawnPeriod = value;
                CurrentForecast.LastDrawnPeriod = value;
            }
        }

        private string _lastDrawNo;

        public override string LastDrawNo
        {
            get => _lastDrawNo;
            set
            {
                _lastDrawNo = value;
                CurrentForecast.LastDrawNo = value;
            }
        }

        private int _totalCount;

        public override int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                CurrentForecast.TotalCount = value;
            }
        }

        private int _winCount;

        public override int WinCount
        {
            get => _winCount;
            set
            {
                _winCount = value;
                CurrentForecast.WinCount = value;
            }
        }

        private int _lostCount;

        public override int LoseCount
        {
            get => _lostCount;
            set
            {
                _lostCount = value;
                CurrentForecast.LoseCount = value;
            }
        }

        private float _winProbability;

        public override float WinProbability
        {
            get => _winProbability;
            set
            {
                _winProbability = value;
                CurrentForecast.WinProbability = value;
            }
        }

        public override string Rule
        {
            get => CurrentForecast.Rule;
            set { }
        }

        private Plan _plan;

        public override Plan Plan
        {
            get => _plan;
            set
            {
                _plan = value;
                CurrentForecast.Plan = value;
            }
        }

        private float _guaScore;

        public override float GuaScore
        {
            get => _guaScore;
            set
            {
                _guaScore = value;
                CurrentForecast.GuaScore = value;
            }
        }

        private float _repetitionScore;

        public override float RepetitionScore
        {
            get => _repetitionScore;
            set
            {
                _repetitionScore = value;
                CurrentForecast.RepetitionScore = value;
            }
        }

        private float _betChaseScore;

        public override float BetChaseScore
        {
            get => _betChaseScore;
            set
            {
                _betChaseScore = value;
                CurrentForecast.BetChaseScore = value;
            }
        }

        private float _score;

        public override float Score
        {
            get => _score;
            set
            {
                _score = value;
                CurrentForecast.Score = value;
            }
        }

        private int _keepGuaCnt;

        public override int KeepGuaCnt
        {
            get => _keepGuaCnt;
            set
            {
                _keepGuaCnt = value;
                CurrentForecast.KeepGuaCnt = value;
            }
        }

        private int _chaseTimesAfterEndGua;

        public override int ChaseTimesAfterEndGua
        {
            get => _chaseTimesAfterEndGua;
            set
            {
                _chaseTimesAfterEndGua = value;
                CurrentForecast.ChaseTimesAfterEndGua = value;
            }
        }

        private int _keepGuaingCnt;

        public override int KeepGuaingCnt
        {
            get => _keepGuaingCnt;
            set
            {
                _keepGuaingCnt = value;
                CurrentForecast.KeepGuaingCnt = value;
            }
        }


        public string ForecastDrawNo => CurrentForecast?.ForecastNo;
    }

    public abstract class ForecastSharedModel : IForecastSharedModel
    {
        public abstract long LastDrawnPeriod { get; set; }
        public abstract string LastDrawNo { get; set; }
        public abstract int TotalCount { get; set; }
        public abstract int WinCount { get; set; }
        public abstract int LoseCount { get; set; }
        public abstract float WinProbability { get; set; }

        public abstract string Rule { get; set; }
        public abstract Plan Plan { get; set; }

        public abstract float GuaScore { get; set; }
        public abstract float RepetitionScore { get; set; }
        public abstract float BetChaseScore { get; set; }
        public abstract float Score { get; set; }
        public abstract int KeepGuaCnt { get; set; }
        public abstract int ChaseTimesAfterEndGua { get; set; }
        public abstract int KeepGuaingCnt { get; set; }
    }
}