namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// BlowfishBase ECB 的实现
    /// </summary>
    /// <remarks>使用该类加密或解密字节数组或单个块与BlowfishBase在ECB（电子代码本）模式，他们的密钥长度可以灵活从零到56字节。</remarks>
    public class BlowfishBase
    {
        public const int MAXKEYLENGTH = 56;
        public const int BLOCKSIZE = 8;
        private const int PBOX_ENTRIES = 18;
        private const int SBOX_ENTRIES = 256;

        private static readonly byte[] TEST_KEY = { 0x1c, 0x58, 0x7f, 0x1c, 0x13, 0x92, 0x4f, 0xef };
        private static readonly uint[] TEST_VECTOR_PLAIN = { 0x30553228, 0x6d6f295a };
        private static readonly uint[] TEST_VECTOR_CIPHER = { 0x55cb3774, 0xd13ef201 };

        private readonly uint[] m_pbox;
        private readonly uint[] m_sbox1;
        private readonly uint[] m_sbox2;
        private readonly uint[] m_sbox3;
        private readonly uint[] m_sbox4;

        private int m_nIsWeakKey;

        public BlowfishBase(byte[] key)
        {
            int nI;
            m_pbox = new uint[PBOX_ENTRIES];

            for (nI = 0; nI < PBOX_ENTRIES; nI++)
            {
                m_pbox[nI] = BlowfishTables.pbox_init[nI];
            }

            m_sbox1 = new uint[SBOX_ENTRIES];
            m_sbox2 = new uint[SBOX_ENTRIES];
            m_sbox3 = new uint[SBOX_ENTRIES];
            m_sbox4 = new uint[SBOX_ENTRIES];

            for (nI = 0; nI < SBOX_ENTRIES; nI++)
            {
                m_sbox1[nI] = BlowfishTables.sbox_init_1[nI];
                m_sbox2[nI] = BlowfishTables.sbox_init_2[nI];
                m_sbox3[nI] = BlowfishTables.sbox_init_3[nI];
                m_sbox4[nI] = BlowfishTables.sbox_init_4[nI];
            }

            int nLen = key.Length;
            if (nLen is 0) return;
            int nKeyPos = 0;
            uint unBuild = 0;

            for (nI = 0; nI < PBOX_ENTRIES; nI++)
            {
                for (int nJ = 0; nJ < 4; nJ++)
                {
                    unBuild = (unBuild << 8) | key[nKeyPos];

                    if (++nKeyPos == nLen)
                    {
                        nKeyPos = 0;
                    }
                }
                m_pbox[nI] ^= unBuild;
            }

            uint unZeroHi = 0;
            uint unZeroLo = 0;

            for (nI = 0; nI < PBOX_ENTRIES; nI += 2)
            {
                BaseEncrypt(ref unZeroHi, ref unZeroLo);
                m_pbox[nI] = unZeroHi;
                m_pbox[nI + 1] = unZeroLo;
            }
            for (nI = 0; nI < SBOX_ENTRIES; nI += 2)
            {
                BaseEncrypt(ref unZeroHi, ref unZeroLo);
                m_sbox1[nI] = unZeroHi;
                m_sbox1[nI + 1] = unZeroLo;
            }
            for (nI = 0; nI < SBOX_ENTRIES; nI += 2)
            {
                BaseEncrypt(ref unZeroHi, ref unZeroLo);
                m_sbox2[nI] = unZeroHi;
                m_sbox2[nI + 1] = unZeroLo;
            }
            for (nI = 0; nI < SBOX_ENTRIES; nI += 2)
            {
                BaseEncrypt(ref unZeroHi, ref unZeroLo);
                m_sbox3[nI] = unZeroHi;
                m_sbox3[nI + 1] = unZeroLo;
            }
            for (nI = 0; nI < SBOX_ENTRIES; nI += 2)
            {
                BaseEncrypt(ref unZeroHi, ref unZeroLo);
                m_sbox4[nI] = unZeroHi;
                m_sbox4[nI + 1] = unZeroLo;
            }

            m_nIsWeakKey = -1;
        }

        /// <summary>
        /// 检查所使用的密钥是否弱，这意味着最终攻击比简单的暴力破解更容易应用于密钥；由于随机性，这种情况不太可能发生
        /// </summary>
        public bool IsWeakKey
        {
            get
            {
                // 出于性能原因，我们不会在初始化期间进行弱键检查，而只在需要时进行
                if (m_nIsWeakKey is -1)
                {
                    m_nIsWeakKey = 0;

                    int nI, nJ;
                    for (nI = 0; nI < SBOX_ENTRIES - 1; nI++)
                    {
                        nJ = nI + 1;
                        while (nJ < SBOX_ENTRIES)
                        {
                            if ((m_sbox1[nI] == m_sbox1[nJ]) |
                                (m_sbox2[nI] == m_sbox2[nJ]) |
                                (m_sbox3[nI] == m_sbox3[nJ]) |
                                (m_sbox4[nI] == m_sbox4[nJ])) break;
                            nJ++;
                        }
                        if (nJ < SBOX_ENTRIES)
                        {
                            m_nIsWeakKey = 1;
                            break;
                        }
                    }
                }

                return m_nIsWeakKey is 1;
            }
        }

        /// <summary>
        /// 删除所有内部数据结构并使此实例无效
        /// </summary>
        public virtual void Burn()
        {
            int nI;

            for (nI = 0; nI < PBOX_ENTRIES; nI++)
            {
                m_pbox[nI] = 0;
            }

            for (nI = 0; nI < SBOX_ENTRIES; nI++)
            {
                m_sbox1[nI] = m_sbox2[nI] = m_sbox3[nI] = m_sbox4[nI] = 0;
            }
        }

        /// <summary>
        /// 执行自测
        /// </summary>
        /// <remarks>
        public static bool SelfTest()
        {
            uint unHi = TEST_VECTOR_PLAIN[0];
            uint unLo = TEST_VECTOR_PLAIN[1];
            BlowfishBase blowfishBase = new(TEST_KEY);
            blowfishBase.Encrypt(ref unHi, ref unLo);

            if ((unHi != TEST_VECTOR_CIPHER[0]) || (unLo != TEST_VECTOR_CIPHER[1]))
            {
                return false;
            }

            blowfishBase.Decrypt(ref unHi, ref unLo);

            if ((unHi != TEST_VECTOR_PLAIN[0]) || (unLo != TEST_VECTOR_PLAIN[1]))
            {
                return false;
            }

            return true;
        }

        protected void BaseEncrypt(ref uint unHiRef, ref uint unLoRef)
        {
            uint unHi = unHiRef;
            uint unLo = unLoRef;
            uint[] sbox1 = m_sbox1;
            uint[] sbox2 = m_sbox2;
            uint[] sbox3 = m_sbox3;
            uint[] sbox4 = m_sbox4;
            uint[] pbox = m_pbox;

            unHi ^= pbox[0];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[1];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[2];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[3];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[4];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[5];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[6];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[7];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[8];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[9];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[10];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[11];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[12];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[13];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[14];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[15];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[16];

            unLoRef = unHi;
            unHiRef = unLo ^ pbox[17];
        }

        /// <summary>
        /// 加密单块
        /// </summary>
        public virtual void Encrypt(ref uint unHiRef, ref uint unLoRef)
        {
            BaseEncrypt(ref unHiRef, ref unLoRef);
        }

        /// <summary>
        /// 加密单个字节
        /// </summary>
        public int Encrypt(byte[] dataIn, byte[] dataOut, int nPosIn, int nPosOut, int nCount)
        {
            nCount >>= 3;

            for (int nI = 0; nI < nCount; nI++)
            {
                uint unHi = ((uint)dataIn[nPosIn] << 24) |
                           ((uint)dataIn[nPosIn + 1] << 16) |
                           ((uint)dataIn[nPosIn + 2] << 8) |
                           dataIn[nPosIn + 3];

                uint unLo = ((uint)dataIn[nPosIn + 4] << 24) |
                           ((uint)dataIn[nPosIn + 5] << 16) |
                           ((uint)dataIn[nPosIn + 6] << 8) |
                           dataIn[nPosIn + 7];

                Encrypt(ref unHi, ref unLo);

                dataOut[nPosOut] = (byte)(unHi >> 24);
                dataOut[nPosOut + 1] = (byte)(unHi >> 16);
                dataOut[nPosOut + 2] = (byte)(unHi >> 8);
                dataOut[nPosOut + 3] = (byte)unHi;
                dataOut[nPosOut + 4] = (byte)(unLo >> 24);
                dataOut[nPosOut + 5] = (byte)(unLo >> 16);
                dataOut[nPosOut + 6] = (byte)(unLo >> 8);
                dataOut[nPosOut + 7] = (byte)unLo;

                nPosIn += 8;
                nPosOut += 8;
            }

            return nCount;
        }

        /// <summary>
        /// 解密单块
        /// </summary>
        public virtual void Decrypt(ref uint unHiRef, ref uint unLoRef)
        {
            uint unHi = unHiRef;
            uint unLo = unLoRef;
            uint[] sbox1 = m_sbox1;
            uint[] sbox2 = m_sbox2;
            uint[] sbox3 = m_sbox3;
            uint[] sbox4 = m_sbox4;
            uint[] pbox = m_pbox;

            unHi ^= pbox[17];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[16];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[15];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[14];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[13];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[12];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[11];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[10];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[9];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[8];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[7];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[6];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[5];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[4];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[3];
            unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox[2];
            unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^
                      sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox[1];

            unLoRef = unHi;
            unHiRef = unLo ^ pbox[0];
        }

        /// <summary>
        /// 解密单个字节
        /// </summary>
        public int Decrypt(byte[] dataIn, byte[] dataOut, int nPosIn, int nPosOut, int nCount)
        {
            nCount >>= 3;

            for (int nI = 0; nI < nCount; nI++)
            {
                uint unHi = ((uint)dataIn[nPosIn] << 24) |
                           ((uint)dataIn[nPosIn + 1] << 16) |
                           ((uint)dataIn[nPosIn + 2] << 8) |
                           dataIn[nPosIn + 3];

                uint unLo = ((uint)dataIn[nPosIn + 4] << 24) |
                           ((uint)dataIn[nPosIn + 5] << 16) |
                           ((uint)dataIn[nPosIn + 6] << 8) |
                           dataIn[nPosIn + 7];

                Decrypt(ref unHi, ref unLo);

                dataOut[nPosOut] = (byte)(unHi >> 24);
                dataOut[nPosOut + 1] = (byte)(unHi >> 16);
                dataOut[nPosOut + 2] = (byte)(unHi >> 8);
                dataOut[nPosOut + 3] = (byte)unHi;
                dataOut[nPosOut + 4] = (byte)(unLo >> 24);
                dataOut[nPosOut + 5] = (byte)(unLo >> 16);
                dataOut[nPosOut + 6] = (byte)(unLo >> 8);
                dataOut[nPosOut + 7] = (byte)unLo;

                nPosIn += 8;
                nPosOut += 8;
            }

            return nCount;
        }
    }
}
