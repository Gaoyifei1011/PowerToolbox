using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// XXH64 校验实现
    /// </summary>
    public sealed class XXHash64 : HashAlgorithm
    {
        private const ulong PRIME64_1 = 11400714785074694791UL;
        private const ulong PRIME64_2 = 14029467366897019727UL;
        private const ulong PRIME64_3 = 1609587929392839161UL;
        private const ulong PRIME64_4 = 9650029242287828579UL;
        private const ulong PRIME64_5 = 2870177450012600261UL;
        private readonly Func<byte[], int, uint> funcGetLittleEndianUInt32;
        private readonly Func<byte[], int, ulong> funcGetLittleEndianUInt64;
        private readonly Func<ulong, ulong> funcGetFinalHashUInt64;
        private readonly ulong seed64;
        private ulong ACC64_1;
        private ulong ACC64_2;
        private ulong ACC64_3;
        private ulong ACC64_4;
        private ulong hash64;
        private int remainingLength;
        private long totalLength;
        private int currentIndex;
        private byte[] currentArray;

        public XXHash64(uint seed)
        {
            HashSizeValue = 64;
            seed64 = seed;
            Initialize();

            if (BitConverter.IsLittleEndian)
            {
                funcGetLittleEndianUInt32 = new Func<byte[], int, uint>((x, i) =>
                {
                    unsafe
                    {
                        fixed (byte* array = x)
                        {
                            return *(uint*)(array + i);
                        }
                    }
                });
                funcGetLittleEndianUInt64 = new Func<byte[], int, ulong>((x, i) =>
                {
                    unsafe
                    {
                        fixed (byte* array = x)
                        {
                            return *(ulong*)(array + i);
                        }
                    }
                });
                funcGetFinalHashUInt64 = new Func<ulong, ulong>(i => (i & 0x00000000000000FFUL) << 56 | (i & 0x000000000000FF00UL) << 40 | (i & 0x0000000000FF0000UL) << 24 | (i & 0x00000000FF000000UL) << 8 | (i & 0x000000FF00000000UL) >> 8 | (i & 0x0000FF0000000000UL) >> 24 | (i & 0x00FF000000000000UL) >> 40 | (i & 0xFF00000000000000UL) >> 56);
            }
            else
            {
                funcGetLittleEndianUInt32 = new Func<byte[], int, uint>((x, i) =>
                {
                    unsafe
                    {
                        fixed (byte* array = x)
                        {
                            return (uint)(array[i++] | (array[i++] << 8) | (array[i++] << 16) | (array[i] << 24));
                        }
                    }
                });
                funcGetLittleEndianUInt64 = new Func<byte[], int, ulong>((x, i) =>
                {
                    unsafe
                    {
                        fixed (byte* array = x)
                        {
                            return array[i++] | ((ulong)array[i++] << 8) | ((ulong)array[i++] << 16) | ((ulong)array[i++] << 24) | ((ulong)array[i++] << 32) | ((ulong)array[i++] << 40) | ((ulong)array[i++] << 48) | ((ulong)array[i] << 56);
                        }
                    }
                });
                funcGetFinalHashUInt64 = new Func<ulong, ulong>(i => i);
            }
        }

        public override void Initialize()
        {
            ACC64_1 = seed64 + PRIME64_1 + PRIME64_2;
            ACC64_2 = seed64 + PRIME64_2;
            ACC64_3 = seed64 + 0;
            ACC64_4 = seed64 - PRIME64_1;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (State != 1) State = 1;
            int size = cbSize - ibStart;
            remainingLength = size & 31;
            if (cbSize >= 32)
            {
                int limit = size - remainingLength;
                do
                {
                    ACC64_1 = Round64(ACC64_1, funcGetLittleEndianUInt64(array, ibStart));
                    ibStart += 8;
                    ACC64_2 = Round64(ACC64_2, funcGetLittleEndianUInt64(array, ibStart));
                    ibStart += 8;
                    ACC64_3 = Round64(ACC64_3, funcGetLittleEndianUInt64(array, ibStart));
                    ibStart += 8;
                    ACC64_4 = Round64(ACC64_4, funcGetLittleEndianUInt64(array, ibStart));
                    ibStart += 8;
                } while (ibStart < limit);
            }
            totalLength += cbSize;
            if (remainingLength == 0) return;
            currentArray = array;
            currentIndex = ibStart;
        }

        protected override byte[] HashFinal()
        {
            if (totalLength >= 32)
            {
                hash64 = RotateLeft64_1(ACC64_1) + RotateLeft64_7(ACC64_2) + RotateLeft64_12(ACC64_3) + RotateLeft64_18(ACC64_4);
                hash64 = MergeRound64(hash64, ACC64_1);
                hash64 = MergeRound64(hash64, ACC64_2);
                hash64 = MergeRound64(hash64, ACC64_3);
                hash64 = MergeRound64(hash64, ACC64_4);
            }
            else
            {
                hash64 = seed64 + PRIME64_5;
            }

            hash64 += (ulong)totalLength;

            while (remainingLength >= 8)
            {
                hash64 = RotateLeft64_27(hash64 ^ Round64(0, funcGetLittleEndianUInt64(currentArray, currentIndex))) * PRIME64_1 + PRIME64_4;
                currentIndex += 8;
                remainingLength -= 8;
            }

            while (remainingLength >= 4)
            {
                hash64 = RotateLeft64_23(hash64 ^ (funcGetLittleEndianUInt32(currentArray, currentIndex) * PRIME64_1)) * PRIME64_2 + PRIME64_3;
                currentIndex += 4;
                remainingLength -= 4;
            }

            unsafe
            {
                fixed (byte* arrayPtr = currentArray)
                {
                    while (remainingLength-- >= 1)
                    {
                        hash64 = RotateLeft64_11(hash64 ^ (arrayPtr[currentIndex++] * PRIME64_5)) * PRIME64_1;
                    }
                }
            }

            hash64 = (hash64 ^ (hash64 >> 33)) * PRIME64_2;
            hash64 = (hash64 ^ (hash64 >> 29)) * PRIME64_3;
            hash64 ^= hash64 >> 32;

            totalLength = State = 0;
            return BitConverter.GetBytes(funcGetFinalHashUInt64(hash64));
        }

        private static ulong MergeRound64(ulong input, ulong value)
        {
            return (input ^ Round64(0, value)) * PRIME64_1 + PRIME64_4;
        }

        private static ulong Round64(ulong input, ulong value)
        {
            return RotateLeft64_31(input + (value * PRIME64_2)) * PRIME64_1;
        }

        private static ulong RotateLeft64_1(ulong value)
        {
            return (value << 1) | (value >> 63);
        }

        private static ulong RotateLeft64_7(ulong value)
        {
            return (value << 7) | (value >> 57);
        }

        private static ulong RotateLeft64_11(ulong value)
        {
            return (value << 11) | (value >> 53);
        }

        private static ulong RotateLeft64_12(ulong value)
        {
            return (value << 12) | (value >> 52);// ACC64_3
        }

        private static ulong RotateLeft64_18(ulong value)
        {
            return (value << 18) | (value >> 46);
        }

        private static ulong RotateLeft64_23(ulong value)
        {
            return (value << 23) | (value >> 41);
        }

        private static ulong RotateLeft64_27(ulong value)
        {
            return (value << 27) | (value >> 37);
        }

        private static ulong RotateLeft64_31(ulong value)
        {
            return (value << 31) | (value >> 33);
        }
    }
}
