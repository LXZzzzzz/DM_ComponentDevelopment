using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ToolsLibrary
{
    public class AESUtils
    {
        const string AES_KEY = "dhjkfhskajflwoxj";  //必须为32位，字母的话需要16个

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="content">明文</param>
        public static string Encrypt(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            byte[] contentBytes = Encoding.UTF8.GetBytes(content);
            byte[] keyBytes = Encoding.UTF8.GetBytes(AES_KEY);
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = keyBytes;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = rm.CreateEncryptor();
            byte[] resultBytes = ict.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
            return Convert.ToBase64String(resultBytes, 0, resultBytes.Length);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="str">密文</param>
        public static string Decrypt(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            byte[] contentBytes = Convert.FromBase64String(content);
            byte[] keyBytes = Encoding.UTF8.GetBytes(AES_KEY);
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = keyBytes;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = rm.CreateDecryptor();
            byte[] resultBytes = ict.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
            return Encoding.UTF8.GetString(resultBytes);
        }
    }
}
