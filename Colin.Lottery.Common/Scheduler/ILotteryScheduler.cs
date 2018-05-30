using System;

namespace Colin.Lottery.Common.Scheduler
{
    public interface ILotteryScheduler
    {
        /// <summary>
        /// 获取当前期号
        /// </summary>
        /// <returns>当前期号</returns>
        long GetPeriodNo();

        /// <summary>
        /// 获取给定时间的期号
        /// </summary>
        /// <returns>The period no.</returns>
        /// <param name="time">给定时间的期号</param>
        long GetPeriodNo(DateTime time);
    }
}
