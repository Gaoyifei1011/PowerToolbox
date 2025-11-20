using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// SHA3 256 算法实现
    /// </summary>
    public class Sha3_256 : HashAlgorithm
    {
        private const int rateBytes = 136;
        private const int outputBytes = 32;
        private int bufferPos;
        private readonly ulong[] state = new ulong[25];
        private readonly byte[] buffer = new byte[rateBytes];

        private static readonly int[] rhoOffsets =
        [
             0,  1, 62, 28, 27,
            36, 44,  6, 55, 20,
             3, 10, 43, 25, 39,
            41, 45, 15, 21,  8,
            18,  2, 61, 56, 14
        ];

        private static readonly ulong[] RoundConstants =
        [
            0x0000000000000001UL, 0x0000000000008082UL,
            0x800000000000808aUL, 0x8000000080008000UL,
            0x000000000000808bUL, 0x0000000080000001UL,
            0x8000000080008081UL, 0x8000000000008009UL,
            0x000000000000008aUL, 0x0000000000000088UL,
            0x0000000080008009UL, 0x000000008000000aUL,
            0x000000008000808bUL, 0x800000000000008bUL,
            0x8000000000008089UL, 0x8000000000008003UL,
            0x8000000000008002UL, 0x8000000000000080UL,
            0x000000000000800aUL, 0x800000008000000aUL,
            0x8000000080008081UL, 0x8000000000008080UL,
            0x0000000080000001UL, 0x8000000080008008UL
        ];

        public Sha3_256()
        {
            HashSizeValue = 256;
            Initialize();
        }

        public override void Initialize()
        {
            Array.Clear(state, 0, state.Length);
            Array.Clear(buffer, 0, buffer.Length);
            bufferPos = 0;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (ibStart < 0 || cbSize < 0 || ibStart + cbSize > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(ibStart), nameof(cbSize), "Invaild parameter");
            }

            int offset = ibStart;
            int remaining = cbSize;

            if (bufferPos > 0)
            {
                int toCopy = Math.Min(remaining, rateBytes - bufferPos);
                Buffer.BlockCopy(array, offset, buffer, bufferPos, toCopy);
                bufferPos += toCopy;
                offset += toCopy;
                remaining -= toCopy;

                if (bufferPos is rateBytes)
                {
                    AbsorbBlock(buffer, 0);
                    bufferPos = 0;
                }
            }

            while (remaining >= rateBytes)
            {
                AbsorbBlock(array, offset);
                offset += rateBytes;
                remaining -= rateBytes;
            }

            if (remaining > 0)
            {
                Buffer.BlockCopy(array, offset, buffer, bufferPos, remaining);
                bufferPos += remaining;
            }
        }

        protected override byte[] HashFinal()
        {
            byte[] block = new byte[rateBytes];
            Array.Copy(buffer, 0, block, 0, bufferPos);
            block[bufferPos] ^= 0x06;
            block[rateBytes - 1] ^= 0x80;
            AbsorbBlock(block, 0);

            byte[] hash = new byte[outputBytes];
            for (int i = 0; i < outputBytes / 8; i++)
            {
                WriteULongLE(state[i], hash, i * 8);
            }

            Initialize();
            return hash;
        }

        private void AbsorbBlock(byte[] buffer, int offset)
        {
            for (int i = 0; i < rateBytes / 8; i++)
            {
                ulong lane = ReadULongLE(buffer, offset + i * 8);
                state[i] ^= lane;
            }
            KeccakF1600(state);
        }

        private static void KeccakF1600(ulong[] A)
        {
            for (int round = 0; round < 24; round++)
            {
                ulong[] C = new ulong[5];
                for (int x = 0; x < 5; x++)
                {
                    C[x] = A[x] ^ A[x + 5] ^ A[x + 10] ^ A[x + 15] ^ A[x + 20];
                }
                for (int x = 0; x < 5; x++)
                {
                    ulong d = C[(x + 4) % 5] ^ Rotl(C[(x + 1) % 5], 1);
                    for (int y = 0; y < 25; y += 5)
                    {
                        A[x + y] ^= d;
                    }
                }

                ulong[] B = new ulong[25];
                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        int index = x + 5 * y;
                        int newX = y;
                        int newY = (2 * x + 3 * y) % 5;
                        int newIndex = newX + 5 * newY;
                        B[newIndex] = Rotl(A[index], rhoOffsets[index]);
                    }
                }

                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        int i = x + 5 * y;
                        A[i] = B[i] ^ ((~B[((x + 1) % 5) + 5 * y]) & B[((x + 2) % 5) + 5 * y]);
                    }
                }

                A[0] ^= RoundConstants[round];
            }
        }

        private static ulong Rotl(ulong x, int shift)
        {
            shift &= 63;
            return shift is 0 ? x : (x << shift) | (x >> (64 - shift));
        }

        private static ulong ReadULongLE(byte[] b, int offset)
        {
            return b[offset]
                | ((ulong)b[offset + 1] << 8)
                | ((ulong)b[offset + 2] << 16)
                | ((ulong)b[offset + 3] << 24)
                | ((ulong)b[offset + 4] << 32)
                | ((ulong)b[offset + 5] << 40)
                | ((ulong)b[offset + 6] << 48)
                | ((ulong)b[offset + 7] << 56);
        }

        private static void WriteULongLE(ulong v, byte[] b, int offset)
        {
            b[offset + 0] = (byte)(v & 0xFF);
            b[offset + 1] = (byte)((v >> 8) & 0xFF);
            b[offset + 2] = (byte)((v >> 16) & 0xFF);
            b[offset + 3] = (byte)((v >> 24) & 0xFF);
            b[offset + 4] = (byte)((v >> 32) & 0xFF);
            b[offset + 5] = (byte)((v >> 40) & 0xFF);
            b[offset + 6] = (byte)((v >> 48) & 0xFF);
            b[offset + 7] = (byte)((v >> 56) & 0xFF);
        }
    }
}
