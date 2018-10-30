using System.Text;
using System.Text.RegularExpressions;

namespace Robin.Lottery.WebApp.Utils
{
    public static class StringExtension  
    {      
        #region unicode 字符转义  
        /// <summary>  
        /// 转换输入字符串中的任何转义字符。如：Unicode 的中文 \u8be5  
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
        /// 将字符串进行 unicode 编码  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static string UnicodeEncode(this string str)  
        {  
            if (string.IsNullOrWhiteSpace(str))  
                return str;  
            StringBuilder strResult = new StringBuilder();  
            if (!string.IsNullOrEmpty(str))  
            {  
                for (int i = 0; i < str.Length; i++)  
                {  
                    strResult.Append("\\u");  
                    strResult.Append(((int)str[i]).ToString("x4"));  
                }  
            }  
            return strResult.ToString();  
        }  
        #endregion  
    }
}