using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// Crc32 校验实现
    /// </summary>
    public class Crc32 : HashAlgorithm
    {
        private uint crc32Result = 0xFFFFFFFF;
        private readonly uint[] crc32Table = new uint[256];

        public Crc32()
        {
            for (uint i = 0; i < 256; i++)
            {
                uint crc32 = i;
                for (int j = 8; j > 0; j--)
                {
                    if ((crc32 & 1) == 1)
                    {
                        crc32 = (crc32 >> 1) ^ 0xEDB88320;
                    }
                    else
                    {
                        crc32 >>= 1;
                    }
                }
                crc32Table[i] = crc32;
            }
        }

        public override void Initialize()
        {
            crc32Result = 0xFFFFFFFF;
        }

        protected override void HashCore(byte[] array, int start, int size)
        {
            int end = start + size;
            for (int i = start; i < end; i++)
            {
                crc32Result = (crc32Result >> 8) ^ crc32Table[array[i] ^ (crc32Result & 0x000000FF)];
            }
        }

        protected override byte[] HashFinal()
        {
            crc32Result = ~crc32Result;
            return BitConverter.GetBytes(crc32Result);
        }
    }
}
