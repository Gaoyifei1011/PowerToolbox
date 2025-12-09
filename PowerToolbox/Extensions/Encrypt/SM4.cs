using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// SM4 SymmetricAlgorithm实现。
    /// 块大小：128位。密钥大小：128位。
    /// 支持ECB， CBC， CFB， OFB, CTS （CTS是通过CBC偷回模拟-见注释）
    /// 支持PaddingMode: None， PKCS7, Zeros, ANSIX923, ISO10126。
    /// </summary>
    public class SM4 : SymmetricAlgorithm
    {
        public SM4()
        {
            // SM4 参数
            BlockSizeValue = 128;
            KeySizeValue = 128;
            LegalBlockSizesValue = [new KeySizes(128, 128, 0)];
            LegalKeySizesValue = [new KeySizes(128, 128, 0)];

            // 默认参数
            Mode = CipherMode.CBC;
            Padding = PaddingMode.PKCS7;
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return CreateTransform(rgbKey, rgbIV, false);
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return CreateTransform(rgbKey, rgbIV, true);
        }

        public override void GenerateIV()
        {
            byte[] iv = new byte[BlockSize / 8];
            using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(iv);
            }
            IVValue = iv;
        }

        public override void GenerateKey()
        {
            byte[] key = new byte[KeySize / 8];
            using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(key);
            }
            KeyValue = key;
        }

        private SM4CryptoTransform CreateTransform(byte[] key, byte[] iv, bool encrypting)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.Length * 8 != KeySize)
            {
                throw new CryptographicException($"Key must be {KeySize} bits.");
            }

            if (iv is null)
            {
                byte[] t = new byte[BlockSize / 8];
                using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
                {
                    randomNumberGenerator.GetBytes(t);
                }

                iv = t;
            }
            return iv.Length * 8 != BlockSize ? throw new CryptographicException($"IV must be {BlockSize} bits.") : new SM4CryptoTransform(key, iv, Mode, Padding, encrypting);
        }
    }
}
