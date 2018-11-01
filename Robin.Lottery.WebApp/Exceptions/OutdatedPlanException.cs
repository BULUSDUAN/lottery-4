using System;

namespace Robin.Lottery.WebApp.Exceptions
{
    /// <summary>
    /// 表示计划已过期的异常
    /// </summary>
    public class OutdatedPlanException : Exception
    {
        public OutdatedPlanException(string message)
            : base(message)
        {

        }
    }
}
