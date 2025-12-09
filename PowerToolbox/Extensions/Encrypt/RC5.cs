using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// RC5 对称加密算法实现
    /// </summary>
    public class RC5 : SymmetricAlgorithm
    {
        private int rounds = 12;

        public RC5()
        {
            // RC5-32块大小 64位
            BlockSizeValue = 64;
            KeySizeValue = 128;
            // 默认128位（可更改）
            LegalBlockSizesValue = [new KeySizes(64, 64, 0)];
            // 允许密钥大小从8到2048位在8位步长（实际使用选择128/192/256等）
            LegalKeySizesValue = [new KeySizes(8, 2048, 8)];
            Mode = CipherMode.CBC;
            Padding = PaddingMode.PKCS7;
        }

        /// <summary>
        /// 轮数 (rounds)
        /// </summary>
        public int Rounds
        {
            get { return rounds; }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                rounds = value;
            }
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            CheckKeyIV(rgbKey, rgbIV);
            return new RC5CryptoTransform(rgbKey, rgbIV, true, Mode, Padding, rounds);
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            CheckKeyIV(rgbKey, rgbIV);
            return new RC5CryptoTransform(rgbKey, rgbIV, false, Mode, Padding, rounds);
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

            if (iv.Length * 8 != BlockSize)
            {
                throw new ArgumentException("IV length incorrect.", nameof(iv));
            }
        }

        public override void GenerateIV()
        {
            IVValue = new byte[BlockSize / 8];
            using RNGCryptoServiceProvider rng = new();
            rng.GetBytes(IVValue);
        }

        public override void GenerateKey()
        {
            int kb = KeySizeValue / 8;
            if (kb <= 0) kb = 16;
            KeyValue = new byte[kb];
            using RNGCryptoServiceProvider rng = new();
            rng.GetBytes(KeyValue);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
