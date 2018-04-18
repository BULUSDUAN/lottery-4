using System;

namespace Colin.Lottery.Common
{
    public class PK10Scheduler : ILotteryScheduler
    {
        //677148期 2018-4-18 11:00:00-11:05:00 
        static readonly long _START_PERIOD = 677148;
        static readonly DateTime _START_DATETIME = new DateTime(2018, 4, 18, 11, 0, 0);

        public static long GetPeriodNo()
        {
            return GetPeriodNo(DateTime.Now);
        }

        public static long GetPeriodNo(DateTime time)
        {
            return _START_PERIOD + (long)Math.Floor((time - _START_DATETIME).TotalMinutes / 5);
        }
    }
}
