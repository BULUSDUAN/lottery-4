using System.Text;
using System.Text.RegularExpressions;

namespace Robin.Lottery.WebApp.Utils
{
    public static class StringExtension
    {
        #region 转换 UTF8-BOM 为 UTF-8

        public static string ToUtf8WithoutBom(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return string.Empty;
            if (buffer.Length <= 3) return Encoding.UTF8.GetString(buffer);

            byte[] bomBuffer = {0xef, 0xbb, 0xbf};

            if (buffer[0] == bomBuffer[0]
                && buffer[1] == bomBuffer[1]
                && buffer[2] == bomBuffer[2])
                return new UTF8Encoding(false).GetString(buffer, 3, buffer.Length - 3);

            return Encoding.UTF8.GetString(buffer);
        }

        #endregion

        #region Unicode 字符转义  

        /// <summary>
        ///     转换输入字符串中的任何转义字符。如：Unicode 的中文 \u8be5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnicodeDencode(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            return Regex.Unescape(str);
        }

        /// <summary>
        ///     将字符串进行 unicode 编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnicodeEncode(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            var strResult = new StringBuilder();
            if (!string.IsNullOrEmpty(str))
                for (var i = 0; i < str.Length; i++)
                {
                    strResult.Append("\\u");
                    strResult.Append(((int) str[i]).ToString("x4"));
                }

            return strResult.ToString();
        }

        #endregion
    }
}