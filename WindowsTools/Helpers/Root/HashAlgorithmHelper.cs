﻿using System.Security.Cryptography;
using System.Text;

namespace WindowsTools.Helpers.Root
{
    /// <summary>
    /// 哈希算法计算辅助类
    /// </summary>
    public static class HashAlgorithmHelper
    {
        /// <summary>
        /// 获取计算所得的 SHA256 算法加密后的值
        /// </summary>
        public static byte[] ComputeSHA256Hash(string content)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
            sha256.Dispose();
            return hashBytes;
        }
    }
}
