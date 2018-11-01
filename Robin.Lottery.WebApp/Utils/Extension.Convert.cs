namespace System
{
    public static class ConverterExtensions
    {
        /// <summary>
        /// 将字符串转为int, 如果转换失败, 用返回默认值
        /// </summary>
        /// <param name="number">要转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换后的整型数值</returns>
        public static int SafeToInt32(this string number, int defaultValue = default(int))
        {
            if (number == null) return defaultValue;
            if (int.TryParse(number, out var v)) return v;

            return defaultValue;
        }
    }
}