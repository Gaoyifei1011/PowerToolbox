using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// RC6 对称加密算法实现
    /// </summary>
    internal class RC6 : SymmetricAlgorithm
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
            return new RC6Transform(rgbKey, rgbIV, true, ModeValue, PaddingValue);
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new RC6Transform(rgbKey, rgbIV, false, ModeValue, PaddingValue);
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
