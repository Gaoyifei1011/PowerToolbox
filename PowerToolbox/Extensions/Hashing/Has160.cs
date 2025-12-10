using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// Has160 校验实现
    /// </summary>
    public sealed class Has160 : HashAlgorithm
    {
        private const int BlockSizeBytes = 64; // 512 bits
        private readonly byte[] _buffer = new byte[BlockSizeBytes];
        private int _bufferPos;                 // number bytes currently in buffer
        private ulong _byteCount;               // total message bytes processed
        private readonly uint[] _h = new uint[5]; // chaining variables H0..H4

        // rotation sequence for A (20 values, repeated each block)
        private static readonly byte[] A_ROT =
        [
            5,11,7,15,6,13,8,14,7,12,9,11,8,15,6,12,9,14,5,13
        ];

        // rotation for B per round (round 0..3)
        private static readonly byte[] B_ROT = [10, 17, 25, 30];

        // round constants
        private static readonly uint[] K =
        [
            0x00000000u, 0x5A827999u, 0x6ED9EBA1u, 0x8F1BBCDCu
        ];

        // message orders per round (20 indices each)
        private static readonly int[][] MSG_ORDER =
        [
            [18,0,1,2,3,19,4,5,6,7,16,8,9,10,11,17,12,13,14,15],
            [18,3,6,9,12,19,15,2,5,8,16,11,14,1,4,17,7,10,13,0],
            [18,12,5,14,7,19,0,9,2,11,16,4,13,6,15,17,8,1,10,3],
            [18,7,2,13,8,19,3,14,9,4,16,15,10,5,0,17,11,6,1,12]
        ];

        // X16..X19 generation indices per round: groups of four per round (for X16..X19)
        private static readonly int[][] X16_GROUPS =
        [
            [0,1,2,3],    // round 0
            [3,6,9,12],   // round 1
            [12,5,14,7],  // round 2
            [7,2,13,8]    // round 3
        ];

        private static readonly int[][] X17_GROUPS =
        [
            [4,5,6,7],
            [15,2,5,8],
            [0,9,2,11],
            [3,14,9,4]
        ];

        private static readonly int[][] X18_GROUPS =
        [
            [8,9,10,11],
            [11,14,1,4],
            [4,13,6,15],
            [15,10,5,0]
        ];

        private static readonly int[][] X19_GROUPS =
        [
            [12,13,14,15],
            [7,10,13,0],
            [8,1,10,3],
            [11,6,1,12]
        ];

        public Has160()
        {
            // HashSizeValue is in bits
            HashSizeValue = 160;
            Initialize();
        }

        public override void Initialize()
        {
            // HAS-160 uses same initial words as SHA-1
            _h[0] = 0x67452301u;
            _h[1] = 0xEFCDAB89u;
            _h[2] = 0x98BADCFEu;
            _h[3] = 0x10325476u;
            _h[4] = 0xC3D2E1F0u;

            _bufferPos = 0;
            _byteCount = 0;
            Array.Clear(_buffer, 0, _buffer.Length);
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (ibStart < 0 || cbSize < 0 || ibStart + cbSize > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(ibStart), "parameters are out of range");
            }

            int offset = ibStart;
            int remaining = cbSize;

            _byteCount += (ulong)cbSize;

            // fill buffer to full blocks
            if (_bufferPos > 0)
            {
                int need = BlockSizeBytes - _bufferPos;
                if (remaining < need)
                {
                    Buffer.BlockCopy(array, offset, _buffer, _bufferPos, remaining);
                    _bufferPos += remaining;
                    return;
                }

                Buffer.BlockCopy(array, offset, _buffer, _bufferPos, need);
                offset += need;
                remaining -= need;
                ProcessBlock(_buffer, 0);
                _bufferPos = 0;
            }

            // process full blocks directly from input
            while (remaining >= BlockSizeBytes)
            {
                ProcessBlock(array, offset);
                offset += BlockSizeBytes;
                remaining -= BlockSizeBytes;
            }

            // leftover
            if (remaining > 0)
            {
                Buffer.BlockCopy(array, offset, _buffer, 0, remaining);
                _bufferPos = remaining;
            }
        }

        protected override byte[] HashFinal()
        {
            // Padding: append 0x80, then zeros, then 64-bit length.
            // HAS-160 uses little-endian word ordering. We'll append length (bits) as little-endian 64-bit.
            ulong bitLength = _byteCount * 8UL;

            // append 0x80
            byte[] pad;
            int padLen = (BlockSizeBytes - (_bufferPos + 1 + 8)) % BlockSizeBytes;
            if (padLen < 0)
            {
                padLen += BlockSizeBytes;
            }
            // total padding = 1 (0x80) + padLen zeros + 8 bytes length
            pad = new byte[1 + padLen + 8];
            pad[0] = 0x80;

            // append length in little-endian (HAS-160 uses little-endian ordering for words)
            ulong len = bitLength;
            for (int i = 0; i < 8; i++)
            {
                pad[1 + padLen + i] = (byte)(len & 0xFF);
                len >>= 8;
            }

            // feed padding
            HashCore(pad, 0, pad.Length);

            // produce digest: H0..H4 written out in little-endian order
            byte[] result = new byte[20];
            int pos = 0;
            for (int i = 0; i < 5; i++)
            {
                uint v = _h[i];
                result[pos++] = (byte)(v & 0xFF);
                result[pos++] = (byte)((v >> 8) & 0xFF);
                result[pos++] = (byte)((v >> 16) & 0xFF);
                result[pos++] = (byte)((v >> 24) & 0xFF);
            }

            // reset for reuse
            Initialize();

            return result;
        }

        /// <summary>
        /// Process 64-byte block starting at offset.
        /// Implements HAS-160 step/round logic as described in Randombit.
        /// </summary>
        private void ProcessBlock(byte[] block, int offset)
        {
            // load 16 words W[0..15] in little-endian
            uint[] W = new uint[16];
            for (int i = 0; i < 16; i++)
            {
                int idx = offset + i * 4;
                W[i] = (uint)(block[idx] | (block[idx + 1] << 8) | (block[idx + 2] << 16) | (block[idx + 3] << 24));
            }

            // working variables
            uint A = _h[0];
            uint B = _h[1];
            uint C = _h[2];
            uint D = _h[3];
            uint E = _h[4];

            // step counter across block 0..79 for A rotation lookup
            int stepIdx = 0;

            // for each of 4 rounds
            for (int r = 0; r < 4; r++)
            {
                // compute X[16..19] for this round by XORing specified words
                uint x16 = W[X16_GROUPS[r][0]] ^ W[X16_GROUPS[r][1]] ^ W[X16_GROUPS[r][2]] ^ W[X16_GROUPS[r][3]];
                uint x17 = W[X17_GROUPS[r][0]] ^ W[X17_GROUPS[r][1]] ^ W[X17_GROUPS[r][2]] ^ W[X17_GROUPS[r][3]];
                uint x18 = W[X18_GROUPS[r][0]] ^ W[X18_GROUPS[r][1]] ^ W[X18_GROUPS[r][2]] ^ W[X18_GROUPS[r][3]];
                uint x19 = W[X19_GROUPS[r][0]] ^ W[X19_GROUPS[r][1]] ^ W[X19_GROUPS[r][2]] ^ W[X19_GROUPS[r][3]];

                // construct X array of 20 words for this round
                uint[] X = new uint[20];
                for (int i = 0; i < 16; i++) X[i] = W[i];
                X[16] = x16;
                X[17] = x17;
                X[18] = x18;
                X[19] = x19;

                int[] order = MSG_ORDER[r];
                byte bRot = B_ROT[r];
                uint k = K[r];

                // 20 steps in the round
                for (int i = 0; i < 20; i++, stepIdx++)
                {
                    uint Xi = X[order[i]];
                    // choose f based on round
                    uint f;
                    if (r is 0)
                    {
                        // f0(x,y,z) = (x & y) | ((~x) & z)
                        f = (B & C) | ((~B) & D);
                    }
                    else if (r is 1)
                    {
                        // f1 = x ^ y ^ z
                        f = B ^ C ^ D;
                    }
                    else if (r is 2)
                    {
                        // f2 = y XOR (x OR (NOT z))
                        f = C ^ (B | (~D));
                    }
                    else // r is 3
                    {
                        f = B ^ C ^ D;
                    }

                    // rotation amounts
                    byte aRot = A_ROT[stepIdx % A_ROT.Length];

                    // temp = ROTL(A, aRot) + f + E + Xi + k
                    uint temp = RotateLeft(A, aRot);
                    unchecked
                    {
                        temp = temp + f + E + Xi + k;
                    }

                    E = D;
                    D = C;
                    C = RotateLeft(B, bRot);
                    B = A;
                    A = temp;
                }
            }

            // add to chaining variables
            unchecked
            {
                _h[0] += A;
                _h[1] += B;
                _h[2] += C;
                _h[3] += D;
                _h[4] += E;
            }
        }

        private static uint RotateLeft(uint x, int n)
        {
            return (x << n) | (x >> (32 - n));
        }
    }
}
