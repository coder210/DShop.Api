using System.Security.Cryptography;
using System.Text;

namespace DShop.PluginShared
{
    public class Md5Helper
    {
        /// <summary>
        /// 计算字符串的MD5哈希值（32位小写十六进制）
        /// </summary>
        public static string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // 将字节数组转换为十六进制字符串
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // "x2"表示小写十六进制，两位
                }
                return sb.ToString();
            }
        }
    }
}
