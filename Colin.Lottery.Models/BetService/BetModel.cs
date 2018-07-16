using System;

namespace Colin.Lottery.Models.BetService
{
    public class BetRecord
    {
        public int Id { get; set; }

        public LotteryType Lottery { get; set; }

        public int Rule { get; set; }

        public Plan Plan { get; set; }

        public long PeriodNo { get; set; }

        public string BetNo { get; set; }

        public int ChaseTimes { get; set; }

        public decimal BetMoney { get; set; }

        public float Odds { get; set; }
        
        public BetType BetType { get; set; }

        public string DrawNo { get; set; }

        public bool IsWin { get; set; }

        public decimal WinMoney { get; set; }

        public decimal Balance { get; set; }

        public DateTime BetTime { get; set; }

        public bool IsDrawed { get; set; }

        public DateTime? DrawTime { get; set; }

        public BetRecord(LotteryType lottery,int rule,Plan plan,long periodNo,string betNo,int chaseTimes,decimal betMoney,float odds,BetType betType,decimal balance)
        {
            Lottery = lottery;
            Rule = rule;
            Plan = plan;
            PeriodNo = periodNo;
            BetNo = string.Join(",", betNo.Split(' '));
            ChaseTimes = chaseTimes;
            BetMoney = betMoney;
            Odds = odds;
            BetType = betType;
            Balance = balance;
            BetTime = DateTime.Now;
            IsDrawed = false;
        }
    }
}