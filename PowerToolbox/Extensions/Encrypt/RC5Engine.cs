using System;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// RC5 引擎（RC5-32）实现
    /// </summary>
    public class RC5Engine
    {
        private readonly int r;
        private readonly uint[] S;
        private const uint P32 = 0xB7E15163;
        private const uint Q32 = 0x9E3779B9;
        public const int BlockSizeBytes = 8;

        public RC5Engine(byte[] key, int rounds)
        {
            if (rounds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rounds));
            }

            r = rounds;
            int t = 2 * (r + 1);
            S = new uint[t];
            KeySchedule(key ?? []);
        }

        private void KeySchedule(byte[] key)
        {
            int b = key.Length;
            int c = Math.Max(1, (b + 4 - 1) / 4);
            uint[] L = new uint[c];
            // 低位优先转换
            for (int i = b - 1; i >= 0; i--)
            {
                int idx = i / 4;
                L[idx] = (L[idx] << 8) + key[i];
            }

            int t = S.Length;
            S[0] = P32;
            for (int i = 1; i < t; i++) S[i] = S[i - 1] + Q32;

            uint A = 0, B = 0;
            int iIdx = 0, jIdx = 0;
            int n = 3 * Math.Max(t, c);
            for (int k = 0; k < n; k++)
            {
                A = S[iIdx] = RotL(S[iIdx] + A + B, 3);
                B = L[jIdx] = RotL(L[jIdx] + A + B, (int)((A + B) & 0x1F));
                iIdx = (iIdx + 1) % t;
                jIdx = (jIdx + 1) % c;
            }

            Array.Clear(L, 0, L.Length);
        }

        public void EncryptBlock(byte[] input, int inOff, byte[] output, int outOff)
        {
            uint A = ToUInt32LE(input, inOff);
            uint B = ToUInt32LE(input, inOff + 4);
            A += S[0];
            B += S[1];
            for (int i = 1; i <= r; i++)
            {
                A = RotL(A ^ B, (int)(B & 0x1F)) + S[2 * i];
                B = RotL(B ^ A, (int)(A & 0x1F)) + S[2 * i + 1];
            }
            WriteUInt32LE(output, outOff, A);
            WriteUInt32LE(output, outOff + 4, B);
        }

        public void DecryptBlock(byte[] input, int inOff, byte[] output, int outOff)
        {
            uint A = ToUInt32LE(input, inOff);
            uint B = ToUInt32LE(input, inOff + 4);
            for (int i = r; i >= 1; i--)
            {
                B = RotR(B - S[2 * i + 1], (int)(A & 0x1F)) ^ A;
                A = RotR(A - S[2 * i], (int)(B & 0x1F)) ^ B;
            }
            B -= S[1];
            A -= S[0];
            WriteUInt32LE(output, outOff, A);
            WriteUInt32LE(output, outOff + 4, B);
        }

        private static uint RotL(uint x, int y)
        {
            y &= 31; return (x << y) | (x >> (32 - y));
        }

        private static uint RotR(uint x, int y)
        {
            y &= 31; return (x >> y) | (x << (32 - y));
        }

        private static uint ToUInt32LE(byte[] buf, int off)
        {
            return (uint)(buf[off] | (buf[off + 1] << 8) | (buf[off + 2] << 16) | (buf[off + 3] << 24));
        }

        private static void WriteUInt32LE(byte[] buf, int off, uint val)
        {
            buf[off] = (byte)(val & 0xFF);
            buf[off + 1] = (byte)((val >> 8) & 0xFF);
            buf[off + 2] = (byte)((val >> 16) & 0xFF);
            buf[off + 3] = (byte)((val >> 24) & 0xFF);
        }
    }
}
