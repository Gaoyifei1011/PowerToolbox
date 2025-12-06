using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// SM4（国密4）对称加密算法实现
    /// </summary>
    public class SM4 : SymmetricAlgorithm
    {
        public SM4()
        {
            KeySizeValue = 128;
            BlockSizeValue = 128;
            FeedbackSizeValue = BlockSizeValue;
            LegalBlockSizesValue = [new KeySizes(128, 128, 0)];
            LegalKeySizesValue = [new KeySizes(128, 128, 0)];
            Mode = CipherMode.ECB;
            Padding = PaddingMode.PKCS7;
        }

        /// <summary>
        /// 生成IV
        /// </summary>
        public override void GenerateIV()
        {
            byte[] buf = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buf);
            }
            IV = buf;
        }

        /// <summary>
        /// 生成密钥
        /// </summary>
        public override void GenerateKey()
        {
            byte[] buf = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buf);
            }
            Key = buf;
        }

        /// <summary>
        /// 生成加密器
        /// </summary>
        public override ICryptoTransform CreateEncryptor(byte[] key, byte[] iv)
        {
            return CreateTransform(key, iv, true);
        }

        /// <summary>
        /// 生成解密器
        /// </summary>
        public override ICryptoTransform CreateDecryptor(byte[] key, byte[] iv)
        {
            return CreateTransform(key, iv, false);
        }

        private ICryptoTransform CreateTransform(byte[] rgbKey, byte[] rgbIV, bool encryptMode)
        {
            ICryptoTransform transform = new SM4Transform(rgbKey, rgbIV, encryptMode);
            switch (Mode)
            {
                case CipherMode.ECB:
                    break;

                case CipherMode.CBC:
                    transform = new CbcTransform(transform, rgbIV, encryptMode);
                    break;

                default:
                    throw new NotSupportedException("Only CBC/ECB is supported");
            }

            switch (PaddingValue)
            {
                case PaddingMode.None:
                    break;

                case PaddingMode.PKCS7:
                case PaddingMode.ISO10126:
                case PaddingMode.ANSIX923:
                    transform = new PKCS7PaddingTransform(transform, PaddingValue, encryptMode);
                    break;

                case PaddingMode.Zeros:
                    transform = new ZerosPaddingTransform(transform, encryptMode);
                    break;

                default:
                    throw new NotSupportedException("Only PKCS#7 padding is supported");
            }

            return transform;
        }
    }
}
