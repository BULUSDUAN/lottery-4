using System;

namespace Colin.Lottery.Common
{
    public class PK10Scheduler : LotteryScheduler<PK10Scheduler>
    {
        //677148期 2018-4-24 9:00:00-9:05:00 
        static readonly long _START_PERIOD = 678198;
        static readonly DateTime _START_DATETIME = new DateTime(2018, 4, 24, 9, 0, 0);

        public override long GetPeriodNo()
        {
            return GetPeriodNo(DateTime.Now);
        }

        public override long GetPeriodNo(DateTime time)
        {
            //每天23:55至次日9:00为休息时间，所以每隔一天时间差需要减去09:05
            var span = time - _START_DATETIME;
            span -= TimeSpan.FromMinutes((int)span.TotalDays * (9 * 60 + 5));
            return _START_PERIOD + (long)Math.Floor(span.TotalMinutes / 5);
        }
    }

    public static class LotterySchedulerTool
    {
        public static int ToShort(this long periodNo)
        {
            return (int)periodNo % 1000;
        }
    }
}
