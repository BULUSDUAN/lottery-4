using System;

namespace System
{
    public static class Extension_Convert
    {
        public static int SafeToInt32(this string number, int defaultValue)
        {
            if (number == null) return defaultValue;
            if (int.TryParse(number, out int v))
            {
                return v;
            }

            return defaultValue;
        }
    }
}