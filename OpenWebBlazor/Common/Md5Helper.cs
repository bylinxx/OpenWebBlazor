using System.Security.Cryptography;
using System.Text;
namespace OpenWebBlazor.Common
{
    public static class Md5Helper
    {
        /// <summary>
        /// 将字符串加密为 MD5 哈希值（十六进制字符串）
        /// </summary>
        public static string ComputeMd5(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("输入字符串不能为空");

            // 将输入字符串转换为 UTF-8 字节数组
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // 计算 MD5 哈希值
            byte[] hashBytes = MD5.HashData(inputBytes);

            // 将字节数组转换为十六进制字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2")); // "x2" 表示两位小写十六进制
            }

            return sb.ToString();
        }
    }
}
