namespace System
{
    public static class ConverterExtensions
    {
        public static int SafeToInt32(this string number, int defaultValue)
        {
            if (number == null) return defaultValue;
            if (int.TryParse(number, out var v)) return v;

            return defaultValue;
        }
    }
}