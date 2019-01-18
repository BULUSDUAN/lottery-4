using System;
using Colin.Lottery.Models;

namespace Colin.Lottery.Common.Scheduler
{
    public abstract class LotteryScheduler : ILotteryScheduler
    {
        public abstract long GetPeriodNo();

        public abstract long GetPeriodNo(DateTime time);
    }
}