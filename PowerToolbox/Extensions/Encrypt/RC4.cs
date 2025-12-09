using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// RC4 对称加密算法实现
    /// </summary>
    public abstract class RC4 : SymmetricAlgorithm
    {
        protected RC4()
        {
            KeySizeValue = 128;
            BlockSizeValue = 64;
            FeedbackSizeValue = BlockSizeValue;
            LegalBlockSizesValue = [new KeySizes(64, 64, 0)];
            LegalKeySizesValue = [new KeySizes(40, 2048, 8)];
        }

        /// <summary>
        /// 创建加密对象的实例以执行RC4算法。
        /// </summary>
        public new static RC4 Create()
        {
            return new RC4CryptoTransform();
        }

        /// <summary>
        /// 创建加密对象的实例，以执行RC4算法的指定实现。
        /// </summary>
        public new static RC4 Create(string algName)
        {
            object alg = CryptoConfig.CreateFromName(algName);
            alg ??= new RC4CryptoTransform();
            return alg as RC4;
        }
    }
}
