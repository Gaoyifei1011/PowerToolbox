namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// 异或加密算法实现
    /// </summary>
    public class XOR
    {
        /// <summary>
        /// 异或加密
        /// </summary>
        public static string XOREncrypt(string contentData, string secretKey)
        {
            char[] data = contentData.ToCharArray();
            char[] key = secretKey.ToCharArray();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i % key.Length];
            }
            return new string(data);
        }

        /// <summary>
        /// 异或解密
        /// </summary>
        public static string XORDecrypt(string contentData, string secretKey)
        {
            char[] key = secretKey.ToCharArray();
            char[] data = contentData.ToCharArray();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i % key.Length];
            }
            return new string(data);
        }

        private static char[] Encrypt(string content, string secretKey)
        {
            char[] data = content.ToCharArray();
            char[] key = secretKey.ToCharArray();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i % key.Length];
            }

            return data;
        }

        private static string Decrypt(char[] data, string secretKey)
        {
            char[] key = secretKey.ToCharArray();

            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i % key.Length];
            }

            return new string(data);
        }
    }
}
