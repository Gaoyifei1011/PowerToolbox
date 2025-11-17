using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// XXH32 校验实现
    /// </summary>
    public class XxHash32 : HashAlgorithm
    {
        const uint PRIME32_1 = 2654435761U;
        const uint PRIME32_2 = 2246822519U;
        const uint PRIME32_3 = 3266489917U;
        const uint PRIME32_4 = 668265263U;
        const uint PRIME32_5 = 374761393U;
        private uint value = 0;

        public override void Initialize()
        {
            value = 0;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            int index = 0;
            uint seed = 0;

            if (array.Length >= 16)
            {
                int limit = array.Length - 16;
                uint v1 = seed + PRIME32_1 + PRIME32_2;
                uint v2 = seed + PRIME32_2;
                uint v3 = seed + 0;
                uint v4 = seed - PRIME32_1;

                do
                {
                    v1 = CalcSubHash(v1, array, index);
                    index += 4;
                    v2 = CalcSubHash(v2, array, index);
                    index += 4;
                    v3 = CalcSubHash(v3, array, index);
                    index += 4;
                    v4 = CalcSubHash(v4, array, index);
                    index += 4;
                } while (index <= limit);

                value = RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);
            }
            else
            {
                value = seed + PRIME32_5;
            }

            value += (uint)array.Length;

            while (index <= array.Length - 4)
            {
                value += BitConverter.ToUInt32(array, index) * PRIME32_3;
                value = RotateLeft(value, 17) * PRIME32_4;
                index += 4;
            }

            while (index < array.Length)
            {
                value += array[index] * PRIME32_5;
                value = RotateLeft(value, 11) * PRIME32_1;
                index++;
            }

            value ^= value >> 15;
            value *= PRIME32_2;
            value ^= value >> 13;
            value *= PRIME32_3;
            value ^= value >> 16;
        }

        protected override byte[] HashFinal()
        {
            return BitConverter.GetBytes(value);
        }

        private static uint CalcSubHash(uint value, byte[] buf, int index)
        {
            uint read_value = BitConverter.ToUInt32(buf, index);
            value += read_value * PRIME32_2;
            value = RotateLeft(value, 13);
            value *= PRIME32_1;
            return value;
        }

        private static uint RotateLeft(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }
    }
}
