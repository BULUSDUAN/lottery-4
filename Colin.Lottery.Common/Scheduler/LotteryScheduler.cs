using System;

using Colin.Lottery.Models;

namespace Colin.Lottery.Common
{
    public abstract class LotteryScheduler<T>
        : Singleton<T>, ILotteryScheduler
        where T : LotteryScheduler<T>, new()
    {
        public abstract long GetPeriodNo();

        public abstract long GetPeriodNo(DateTime time);
    }
}
