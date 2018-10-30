using System;

namespace Colin.Lottery.Utils
{
    public static class UtcTimeUtil
    {
        public static int ToLocalHour(this int utcHour)
        {
            var delta = (int) Math.Ceiling((DateTime.Now - DateTime.UtcNow).TotalHours);
            utcHour += delta;
            if (utcHour < 0)
                utcHour += 24;
            else if (utcHour >= 24)
                utcHour -= 24;

            return utcHour;
        }
    }
}