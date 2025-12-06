namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// BlowfishBase 的 CBC 块密码模式实现
    /// </summary>
    public class BlowfishBaseCbc : BlowfishBase
    {
        private uint m_unIvHi;
        private uint m_unIvLo;

        public BlowfishBaseCbc(byte[] key, byte[] iv) : base(key)
        {
            Iv = iv;
        }

        public byte[] Iv
        {
            get
            {
                byte[] result = new byte[BLOCKSIZE];

                result[0] = (byte)(m_unIvHi >> 24);
                result[1] = (byte)(m_unIvHi >> 16);
                result[2] = (byte)(m_unIvHi >> 8);
                result[3] = (byte)m_unIvHi;
                result[4] = (byte)(m_unIvLo >> 24);
                result[5] = (byte)(m_unIvLo >> 16);
                result[6] = (byte)(m_unIvLo >> 8);
                result[7] = (byte)m_unIvLo;

                return result;
            }

            set
            {
                m_unIvHi = ((uint)value[0] << 24) |
                           ((uint)value[1] << 16) |
                           ((uint)value[2] << 8) |
                           value[3];

                m_unIvLo = ((uint)value[4] << 24) |
                           ((uint)value[5] << 16) |
                           ((uint)value[6] << 8) |
                           value[7];
            }
        }

        /// <summary>
        /// 删除所有内部数据结构并使此实例无效
        /// </summary>
        public override void Burn()
        {
            base.Burn();
            m_unIvHi = m_unIvLo = 0;
        }

        /// <summary>
        /// 加密单块
        /// </summary>
        public override void Encrypt(ref uint unHiRef, ref uint unLoRef)
        {
            unHiRef ^= m_unIvHi;
            unLoRef ^= m_unIvLo;

            BaseEncrypt(ref unHiRef, ref unLoRef);

            m_unIvHi = unHiRef;
            m_unIvLo = unLoRef;
        }

        /// <summary>
        /// 解密单块
        /// </summary>
        public override void Decrypt(ref uint unHiRef, ref uint unLoRef)
        {
            uint unBakHi = unHiRef;
            uint unBakLo = unLoRef;

            base.Decrypt(ref unHiRef, ref unLoRef);

            unHiRef ^= m_unIvHi;
            unLoRef ^= m_unIvLo;

            m_unIvHi = unBakHi;
            m_unIvLo = unBakLo;
        }
    }
}
