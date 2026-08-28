namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// 异或加密算法实现
    /// </summary>
    internal class XOR
    {
        /// <summary>
        /// 异或加密
        /// </summary>
        internal static string XOREncrypt(string contentData, string secretKey)
        {
            if (string.IsNullOrEmpty(contentData) || string.IsNullOrEmpty(secretKey))
            {
                return default;
            }

            char[] data = contentData.ToCharArray();
            char[] key = secretKey.ToCharArray();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i % key.Length];
            }
            return new(data);
        }

        /// <summary>
        /// 异或解密
        /// </summary>
        internal static string XORDecrypt(string contentData, string secretKey)
        {
            if (string.IsNullOrEmpty(contentData) || string.IsNullOrEmpty(secretKey))
            {
                return default;
            }

            char[] key = secretKey.ToCharArray();
            char[] data = contentData.ToCharArray();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i % key.Length];
            }
            return new(data);
        }
    }
}
