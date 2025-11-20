using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// SM3 算法实现
    /// </summary>
    public class SM3 : HashAlgorithm
    {
        private int bufferFilled;
        private ulong byteCount;
        private readonly uint[] V = new uint[8];
        private readonly byte[] buffer = new byte[64];

        private readonly uint[] IV =
        [
            0x7380166F, 0x4914B2B9, 0x172442D7, 0xDA8A0600,
            0xA96F30BC, 0x163138AA, 0xE38DEE4D, 0xB0FB0E4E
        ];

        public SM3()
        {
            HashSizeValue = 256;
            Initialize();
        }

        public override void Initialize()
        {
            Array.Copy(IV, V, 8);
            bufferFilled = 0;
            byteCount = 0;
            Array.Clear(buffer, 0, buffer.Length);
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (cbSize == 0) return;
            int offset = ibStart;
            int remaining = cbSize;
            byteCount += (ulong)cbSize;

            if (bufferFilled > 0)
            {
                int need = 64 - bufferFilled;
                if (remaining < need)
                {
                    Buffer.BlockCopy(array, offset, buffer, bufferFilled, remaining);
                    bufferFilled += remaining;
                    return;
                }
                else
                {
                    Buffer.BlockCopy(array, offset, buffer, bufferFilled, need);
                    ProcessBlock(buffer, 0);
                    offset += need;
                    remaining -= need;
                    bufferFilled = 0;
                }
            }

            while (remaining >= 64)
            {
                ProcessBlock(array, offset);
                offset += 64;
                remaining -= 64;
            }

            if (remaining > 0)
            {
                Buffer.BlockCopy(array, offset, buffer, 0, remaining);
                bufferFilled = remaining;
            }
        }

        protected override byte[] HashFinal()
        {
            ulong bitLen = byteCount * 8UL;
            int k = (56 - ((bufferFilled + 1) % 64) + 64) % 64;
            int padLen = 1 + k + 8;
            byte[] finalBytes = new byte[bufferFilled + padLen];
            if (bufferFilled > 0)
            {
                Buffer.BlockCopy(buffer, 0, finalBytes, 0, bufferFilled);
            }

            finalBytes[bufferFilled] = 0x80;
            for (int i = 0; i < 8; i++)
            {
                finalBytes[finalBytes.Length - 1 - i] = (byte)((bitLen >> (8 * i)) & 0xFF);
            }

            int off = 0;
            while (off + 64 <= finalBytes.Length)
            {
                ProcessBlock(finalBytes, off);
                off += 64;
            }

            byte[] digest = new byte[32];
            for (int i = 0; i < 8; i++)
            {
                digest[i * 4 + 0] = (byte)((V[i] >> 24) & 0xFF);
                digest[i * 4 + 1] = (byte)((V[i] >> 16) & 0xFF);
                digest[i * 4 + 2] = (byte)((V[i] >> 8) & 0xFF);
                digest[i * 4 + 3] = (byte)(V[i] & 0xFF);
            }

            return digest;
        }

        private static uint Rotl(uint x, int n)
        {
            int s = n & 31;
            if (s == 0) return x;
            return (x << s) | (x >> (32 - s));
        }

        private static uint P0(uint x)
        {
            return x ^ Rotl(x, 9) ^ Rotl(x, 17);
        }

        private static uint P1(uint x)
        {
            return x ^ Rotl(x, 15) ^ Rotl(x, 23);
        }

        private static uint FF(int j, uint x, uint y, uint z)
        {
            return (j >= 0 && j <= 15) ? (x ^ y ^ z) : ((x & y) | (x & z) | (y & z));
        }

        private static uint GG(int j, uint x, uint y, uint z)
        {
            return (j >= 0 && j <= 15) ? (x ^ y ^ z) : ((x & y) | (~x & z));
        }

        private void ProcessBlock(byte[] block, int offset)
        {
            uint[] W = new uint[68];
            uint[] W1 = new uint[64];

            for (int j = 0; j < 16; j++)
            {
                int i = offset + j * 4;
                W[j] = ((uint)block[i] << 24) | ((uint)block[i + 1] << 16) | ((uint)block[i + 2] << 8) | ((uint)block[i + 3]);
            }

            for (int j = 16; j <= 67; j++)
            {
                uint x = W[j - 16] ^ W[j - 9] ^ Rotl(W[j - 3], 15);
                W[j] = P1(x) ^ Rotl(W[j - 13], 7) ^ W[j - 6];
            }

            for (int j = 0; j <= 63; j++)
            {
                W1[j] = W[j] ^ W[j + 4];
            }

            uint A = V[0], B = V[1], C = V[2], D = V[3];
            uint E = V[4], F = V[5], G = V[6], H = V[7];

            for (int j = 0; j <= 63; j++)
            {
                uint Tj = (uint)(j <= 15 ? 0x79cc4519 : 0x7a879d8a);
                uint SS1 = Rotl((Rotl(A, 12) + E + Rotl(Tj, j)), 7);
                uint SS2 = SS1 ^ Rotl(A, 12);
                uint TT1 = FF(j, A, B, C) + D + SS2 + W1[j];
                uint TT2 = GG(j, E, F, G) + H + SS1 + W[j];

                D = C;
                C = Rotl(B, 9);
                B = A;
                A = TT1;
                H = G;
                G = Rotl(F, 19);
                F = E;
                E = P0(TT2);
            }

            V[0] ^= A;
            V[1] ^= B;
            V[2] ^= C;
            V[3] ^= D;
            V[4] ^= E;
            V[5] ^= F;
            V[6] ^= G;
            V[7] ^= H;
        }
    }
}
