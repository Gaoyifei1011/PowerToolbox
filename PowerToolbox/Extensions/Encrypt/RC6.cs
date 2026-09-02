using System;
using System.Security.Cryptography;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// RC6 对称加密算法实现
    /// </summary>
    internal sealed class RC6 : SymmetricAlgorithm
    {
        internal RC6()
        {
            BlockSizeValue = 128;
            KeySizeValue = 128;
            FeedbackSizeValue = 128;
            LegalKeySizesValue = [new(128, 256, 32)];
            LegalBlockSizesValue = [new(128, 128, 0)];
            ModeValue = CipherMode.CBC;
            PaddingValue = PaddingMode.PKCS7;
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            CheckKeyIV(rgbKey, rgbIV);
            return new RC6CryptoTransform(rgbKey, rgbIV, true, ModeValue, PaddingValue);
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            CheckKeyIV(rgbKey, rgbIV);
            return new RC6CryptoTransform(rgbKey, rgbIV, false, ModeValue, PaddingValue);
        }

        private void CheckKeyIV(byte[] key, byte[] iv)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (iv is null)
            {
                throw new ArgumentNullException(nameof(iv));
            }
        }

        public override void GenerateIV()
        {
            IVValue = new byte[BlockSizeValue / 8];
            using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(IVValue);
        }

        public override void GenerateKey()
        {
            KeyValue = new byte[KeySizeValue / 8];
            using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(KeyValue);
        }
    }
}
