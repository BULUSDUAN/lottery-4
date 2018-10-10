using System;

namespace Colin.Lottery.Common.Scheduler
{
    public class Pk10Scheduler : LotteryScheduler<Pk10Scheduler>
    {
        //677148期 2018-4-24 9:00:00-9:05:00 
        private static readonly long StartPeriod = 678198;
        private static readonly DateTime StartDatetime = new DateTime(2018, 4, 24, 1, 0, 0);

        public override long GetPeriodNo()
        {
            return GetPeriodNo(DateTime.UtcNow);
        }

        public override long GetPeriodNo(DateTime time)
        {
            //每天23:55至次日9:00为休息时间，所以每隔一天时间差需要减去09:05
            var span = time - StartDatetime;
            span -= TimeSpan.FromMinutes((int)span.TotalDays * (9 * 60 + 5));
            return StartPeriod + (long)Math.Floor(span.TotalMinutes / 5);
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
