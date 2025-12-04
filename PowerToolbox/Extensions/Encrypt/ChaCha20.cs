using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// ChaCha20 加密算法实现
    /// 密钥长度：256位 (32字节)
    /// 初始向量(IV)长度：96位 (12字节) 或 128位 (16字节，其中后12字节有效)
    /// </summary>
    public sealed class ChaCha20 : SymmetricAlgorithm
    {
        public ChaCha20()
        {
            // 设置合法的密钥和块大小[citation:1]
            LegalKeySizesValue = [new KeySizes(256, 256, 0)]; // 仅支持256位密钥
            LegalBlockSizesValue = [new KeySizes(512, 512, 0)]; // ChaCha20块大小为512位

            // 设置默认值
            BlockSizeValue = 512;
            KeySizeValue = 256;
            ModeValue = CipherMode.ECB; // ChaCha20是流密码，使用ECB模式
            PaddingValue = PaddingMode.None;
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new ChaCha20Transform(rgbKey, rgbIV);
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            // ChaCha20是对称的，解密使用相同的变换[citation:1]
            return new ChaCha20Transform(rgbKey, rgbIV);
        }

        public override void GenerateKey()
        {
            KeyValue = new byte[32]; // 256位 = 32字节
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(KeyValue);
        }

        public override void GenerateIV()
        {
            IVValue = new byte[12]; // 推荐使用12字节（96位）IV[citation:2]
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(IVValue);
        }
    }
}
