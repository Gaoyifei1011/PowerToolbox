using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// Rabbit 对称加密算法实现
    /// </summary>
    internal sealed class Rabbit : SymmetricAlgorithm
    {
        private const int KeySizeInBits = 128;
        private const int IVSizeInBits = 64;
        private const int BlockSizeInBits = 64;

        /// <summary>
        /// 创建一个新实例
        /// </summary>
        internal Rabbit() : base()
        {
            KeySizeValue = KeySizeInBits;
            BlockSizeValue = BlockSizeInBits;
            FeedbackSizeValue = BlockSizeValue;
            LegalBlockSizesValue = [new(BlockSizeInBits, BlockSizeInBits, 0)];  // 128-bit
            LegalKeySizesValue = [new(KeySizeInBits, KeySizeInBits, 0)];  // 128-bit
            Mode = CipherMode.CBC;  // same as default
            Padding = PaddingMode.None;
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new RabbitCryptoTransform(rgbKey, rgbIV, false, Padding);
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new RabbitCryptoTransform(rgbKey, rgbIV, true, Padding);
        }

        public override void GenerateIV()
        {
            IVValue = new byte[IVSizeInBits / 8];  // IV is always 64 bits
            using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            RandomNumberGenerator.Create().GetBytes(IVValue);
        }

        public override void GenerateKey()
        {
            KeyValue = new byte[KeySizeInBits / 8];  // Key is always 128 bits
            using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            RandomNumberGenerator.Create().GetBytes(KeyValue);
        }

        public override int BlockSize
        {
            get { return base.BlockSize; }

            set
            {
                if (value is not BlockSizeInBits)
                {
                    throw new CryptographicException("Block size must be 128 bits.");
                }
                BlockSizeValue = value;
            }
        }

        public override int FeedbackSize
        {
            get { return base.FeedbackSize; }

            set
            {
                throw new CryptographicException("Feedback not supported.");
            }
        }

        public override CipherMode Mode
        {
            get { return base.Mode; }

            set
            {
                // stream cipher is closest to CBC
                if (value is not CipherMode.CBC)
                {
                    throw new CryptographicException("Cipher mode is not supported.");
                }
                ModeValue = value;
            }
        }

        public override PaddingMode Padding
        {
            get { return base.Padding; }

            set
            {
                base.Padding = value switch
                {
                    PaddingMode.None => value,
                    PaddingMode.PKCS7 => value,
                    PaddingMode.Zeros => value,
                    PaddingMode.ANSIX923 => value,
                    PaddingMode.ISO10126 => value,
                    _ => throw new CryptographicException("Padding mode is not supported."),
                };
            }
        }
    }
}
