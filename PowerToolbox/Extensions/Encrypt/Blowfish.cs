using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// BlowFish 加密算法实现
    /// </summary>
    public class Blowfish : SymmetricAlgorithm, ICryptoTransform
    {
        private readonly bool m_blIsEncryptor;
        private readonly BlowfishBase mBlowfishBase;
        private readonly BlowfishBaseCbc mBlowfishBaseCbc;
        private RNGCryptoServiceProvider rngCryptoServiceProvider;

        public Blowfish()
        {
            mBlowfishBase = null;
            mBlowfishBaseCbc = null;
            IVValue = null;
            KeyValue = null;
            KeySizeValue = BlowfishBase.MAXKEYLENGTH * 8;
            LegalBlockSizesValue = new KeySizes[1];
            LegalBlockSizesValue[0] = new KeySizes(BlockSize, BlockSize, 8);
            LegalKeySizesValue = new KeySizes[1];
            LegalKeySizesValue[0] = new KeySizes(0, BlowfishBase.MAXKEYLENGTH * 8, 8);
            ModeValue = CipherMode.ECB;
            rngCryptoServiceProvider = null;
        }

        private Blowfish(byte[] key, byte[] iv, bool blCBC, bool blIsEncryptor)
        {
            if (key is null)
            {
                GenerateKey();
            }
            else
            {
                Key = key;
            }

            if (blCBC)
            {
                if (iv is null)
                {
                    GenerateIV();
                }
                else
                {
                    IV = iv;
                }

                mBlowfishBase = null;
                mBlowfishBaseCbc = new BlowfishBaseCbc(KeyValue, IVValue);
            }
            else
            {
                mBlowfishBase = new BlowfishBase(KeyValue);
                mBlowfishBaseCbc = null;
            }

            m_blIsEncryptor = blIsEncryptor;
        }

        /// <summary>
        /// BlowfishBase块大小，只能设置为相同的值
        /// </summary>
        public override int BlockSize
        {
            get { return BlowfishBase.BLOCKSIZE * 8; }

            set
            {
                // 仅支持 64 bit 大小
                if (BlowfishBase.BLOCKSIZE * 8 != value)
                {
                    throw new CryptographicException("illegal blocksize");
                }
            }
        }

        /// <summary>
        /// 初始化向量，在工厂中使用
        /// </summary>
        public override byte[] IV
        {
            get { return IVValue; }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(IV));
                }
                if (value.Length is not BlowfishBase.BLOCKSIZE)
                {
                    throw new CryptographicException("illegal IV length");
                }
                IVValue = value;
            }
        }

        /// <summary>
        ///     the key, used in the factory
        /// </summary>
        public override byte[] Key
        {
            get { return KeyValue; }

            set
            {
                KeyValue = value ?? throw new ArgumentNullException(nameof(Key), "key cannot be null");
            }
        }

        /// <summary>
        ///     the key length (for auto generation only)
        /// </summary>
        public override int KeySize
        {
            get { return KeySizeValue; }

            set
            {
                KeySizes ks = LegalKeySizes[0];
                if ((0 != value % ks.SkipSize) || (value > ks.MaxSize) || (value < ks.MinSize))
                {
                    throw new CryptographicException("invalid key size");
                }
                KeySizeValue = value;
            }
        }

        public override KeySizes[] LegalBlockSizes
        {
            get { return LegalBlockSizesValue; }
        }

        public override KeySizes[] LegalKeySizes
        {
            get { return LegalKeySizesValue; }
        }

        public override CipherMode Mode
        {
            get { return ModeValue; }

            set
            {
                // FIXME: 仅支持 ECB 和 CBC 块密码模式
                if (value is not CipherMode.CBC && value is not CipherMode.ECB)
                {
                    throw new CryptographicException("only ECB and CBC are supported");
                }
                ModeValue = value;
            }
        }

        public bool CanReuseTransform
        {
            get { return true; }
        }

        public bool CanTransformMultipleBlocks
        {
            get { return true; }
        }

        public int InputBlockSize
        {
            get { return BlowfishBase.BLOCKSIZE; }
        }

        public int OutputBlockSize
        {
            get { return BlowfishBase.BLOCKSIZE; }
        }

        public int TransformBlock(byte[] bufIn, int nOfsIn, int nCount, byte[] bufOut, int nOfsOut)
        {
            int nResult;
            if (mBlowfishBaseCbc is not null)
            {
                nResult = m_blIsEncryptor ? mBlowfishBaseCbc.Encrypt(bufIn, bufOut, nOfsIn, nOfsOut, nCount) : mBlowfishBaseCbc.Decrypt(bufIn, bufOut, nOfsIn, nOfsOut, nCount);
            }
            else if (mBlowfishBase is not null)
            {
                nResult = m_blIsEncryptor ? mBlowfishBase.Encrypt(bufIn, bufOut, nOfsIn, nOfsOut, nCount) : mBlowfishBase.Decrypt(bufIn, bufOut, nOfsIn, nOfsOut, nCount);
            }
            else
            {
                nResult = 0;
            }

            return nResult * BlowfishBase.BLOCKSIZE;
        }

        public byte[] TransformFinalBlock(byte[] inBuf, int nOfs, int nCount)
        {
            byte[] result;

            if (m_blIsEncryptor)
            {
                // 我们需要接管我们得到的一切，用正确的方案填充它，然后加密或解密
                int nRest = nCount % BlowfishBase.BLOCKSIZE;

                // 填充方案可能导致不同的数据长度，而零填充只填充最后一个块PKCS7可能需要一个额外的块来存储其长度信息
                int nBufSize = nCount - nRest;
                int nFill;

                if (PaddingValue is PaddingMode.PKCS7)
                {
                    nBufSize += BlowfishBase.BLOCKSIZE;
                    nFill = BlowfishBase.BLOCKSIZE - nRest;
                }
                else
                {
                    if (0 < nRest) nBufSize += BlowfishBase.BLOCKSIZE;
                    nFill = 0;
                }

                result = new byte[nBufSize];
                Array.Copy(inBuf, nOfs, result, 0, nCount);

                for (var nI = nCount; nI < nBufSize; nI++)
                {
                    result[nI] = (byte)nFill;
                }

                TransformBlock(result, 0, nBufSize, result, 0);
            }
            else
            {
                byte[] lastBlocks = new byte[nCount];
                if (0 < nCount)
                {
                    TransformBlock(inBuf, nOfs, nCount, lastBlocks, 0);

                    if (PaddingMode.PKCS7 == PaddingValue)
                    {
                        nCount -= lastBlocks[nCount - 1];
                    }

                    result = new byte[nCount];
                    Array.Copy(lastBlocks, 0, result, 0, nCount);
                }
                else
                {
                    result = lastBlocks;
                }
            }

            return result;
        }

        /// <summary>
        /// 检查一个密钥是否弱（也许不应该使用）
        /// </summary>
        public static bool IsWeakKey(byte[] key)
        {
            Blowfish bfAlg = new(key, null, false, true);
            return bfAlg.mBlowfishBase.IsWeakKey;
        }

        public override ICryptoTransform CreateEncryptor(byte[] key, byte[] iv)
        {
            Blowfish result = new(key, iv, ModeValue is CipherMode.CBC, true)
            {
                Padding = Padding
            };
            return result;
        }

        public override ICryptoTransform CreateDecryptor(byte[] key, byte[] iv)
        {
            Blowfish result = new(key, iv, ModeValue is CipherMode.CBC, false)
            {
                Padding = Padding
            };
            return result;
        }

        public override void GenerateKey()
        {
            if (null == rngCryptoServiceProvider) rngCryptoServiceProvider = new RNGCryptoServiceProvider();

            KeyValue = new byte[KeySizeValue / 8];
            rngCryptoServiceProvider.GetBytes(KeyValue);
        }

        public override void GenerateIV()
        {
            if (null == rngCryptoServiceProvider) rngCryptoServiceProvider = new RNGCryptoServiceProvider();

            IVValue = new byte[BlowfishBase.BLOCKSIZE];
            rngCryptoServiceProvider.GetBytes(IVValue);
        }
    }
}
