using System.Security.Cryptography;
using System.Text;

namespace Colin.Lottery.Utils
{
    public static class EncryptUtil
    {
        /// <summary>
        /// 生成字符串的MD5值（32位小写）
        /// </summary>
        /// <param name="inputValue">要生成的字符串</param>
        /// <returns>MD5值</returns>
        public static string CreateMD5(string inputValue)
        {
            using (var md5 = MD5.Create())
            {
                var data = md5.ComputeHash(Encoding.UTF8.GetBytes(inputValue));
                StringBuilder builder = new StringBuilder();
                // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串
                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }
                string result = builder.ToString();

                return result;
            }
        }
    }
}