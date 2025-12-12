using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    public sealed class Shake256 : HashAlgorithm
    {
        // SHAKE256 parameters
        private const int RateBytes = 136; // 1088 bits / 8

        private const int StateLaneCount = 25;
        private readonly ulong[] state = new ulong[StateLaneCount];
        private readonly byte[] buffer = new byte[RateBytes];
        private int bufferPos;
        private bool squeezed;
        private readonly int outputLengthBytesRequested;

        public override int HashSize
        {
            get { return HashSizeValue; }
        }

        // Keccak round constants
        private static readonly ulong[] KeccakRoundConstants =
        [
            0x0000000000000001UL, 0x0000000000008082UL, 0x800000000000808aUL, 0x8000000080008000UL,
            0x000000000000808bUL, 0x0000000080000001UL, 0x8000000080008081UL, 0x8000000000008009UL,
            0x000000000000008aUL, 0x0000000000000088UL, 0x0000000080008009UL, 0x000000008000000aUL,
            0x000000008000808bUL, 0x800000000000008bUL, 0x8000000000008089UL, 0x8000000000008003UL,
            0x8000000000008002UL, 0x8000000000000080UL, 0x000000000000800aUL, 0x800000008000000aUL,
            0x8000000080008081UL, 0x8000000000008080UL, 0x0000000080000001UL, 0x8000000080008008UL
        ];

        // default 32 bytes output
        public Shake256() : this(32)
        {
        }

        public Shake256(int outputLengthBytes)
        {
            if (outputLengthBytes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(outputLengthBytes));
            }

            outputLengthBytesRequested = outputLengthBytes;
            HashSizeValue = outputLengthBytes * 8; // in bits
            Initialize();
        }

        public override void Initialize()
        {
            Array.Clear(state, 0, state.Length);
            Array.Clear(buffer, 0, buffer.Length);
            bufferPos = 0;
            squeezed = false;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (ibStart < 0 || cbSize < 0 || ibStart + cbSize > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(ibStart), "size is not valid");
            }

            if (squeezed)
            {
                throw new InvalidOperationException("Cannot absorb after squeezing (HashFinal called). Create a new instance or reinitialize.");
            }

            int offset = ibStart;
            int remaining = cbSize;

            while (remaining > 0)
            {
                int toCopy = Math.Min(remaining, RateBytes - bufferPos);
                Buffer.BlockCopy(array, offset, buffer, bufferPos, toCopy);
                bufferPos += toCopy;
                offset += toCopy;
                remaining -= toCopy;

                if (bufferPos == RateBytes)
                {
                    KeccakAbsorbBlock(buffer, 0);
                    bufferPos = 0;
                }
            }
        }

        protected override byte[] HashFinal()
        {
            // padding for SHAKE: domain separation 0x1F and final bit 0x80
            buffer[bufferPos] ^= 0x1F;
            buffer[RateBytes - 1] ^= 0x80;
            KeccakAbsorbBlock(buffer, 0);
            squeezed = true;
            return Squeeze(outputLengthBytesRequested);
        }

        private byte[] Squeeze(int outputLen)
        {
            if (!squeezed)
            {
                throw new InvalidOperationException("Must call HashFinal/ComputeHash before squeezing.");
            }

            byte[] output = new byte[outputLen];
            int outPos = 0;

            while (outPos < outputLen)
            {
                // extract up to RateBytes from state
                int take = Math.Min(outputLen - outPos, RateBytes);
                int written = 0;
                int laneIndex = 0;

                while (written < take)
                {
                    ulong lane = state[laneIndex];
                    int laneBytes = Math.Min(8, take - written);
                    for (int b = 0; b < laneBytes; b++)
                    {
                        output[outPos + written + b] = (byte)((lane >> (8 * b)) & 0xFFUL);
                    }
                    written += laneBytes;
                    laneIndex++;
                }

                outPos += take;
                if (outPos < outputLen)
                {
                    KeccakF1600();
                }
            }

            return output;
        }

        private void KeccakAbsorbBlock(byte[] block, int offset)
        {
            int laneIndex = 0;
            for (int i = 0; i < RateBytes; i += 8)
            {
                ulong v = 0;
                int limit = Math.Min(8, RateBytes - i);
                for (int b = 0; b < limit; b++)
                {
                    v |= ((ulong)block[offset + i + b]) << (8 * b);
                }
                state[laneIndex] ^= v;
                laneIndex++;
            }
            KeccakF1600();
        }

        /// <summary>
        /// Keccak-f[1600] permutation
        /// </summary>
        private void KeccakF1600()
        {
            // we'll operate on state directly
            for (int round = 0; round < 24; round++)
            {
                // θ
                ulong[] C = new ulong[5];
                for (int x = 0; x < 5; x++)
                {
                    C[x] = state[x] ^ state[x + 5] ^ state[x + 10] ^ state[x + 15] ^ state[x + 20];
                }
                ulong[] D = new ulong[5];
                for (int x = 0; x < 5; x++)
                {
                    D[x] = C[(x + 4) % 5] ^ Rol64(C[(x + 1) % 5], 1);
                }
                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        state[x + 5 * y] ^= D[x];
                    }
                }

                // ρ and π -> B
                ulong[] B = new ulong[25];
                int[,] r = new int[,]
                {
                    {0, 36, 3, 41, 18},
                    {1, 44, 10, 45, 2},
                    {62, 6, 43, 15, 61},
                    {28, 55, 25, 21, 56},
                    {27, 20, 39, 8, 14}
                };
                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        int newX = y;
                        int newY = (2 * x + 3 * y) % 5;
                        int srcIndex = x + 5 * y;
                        int dstIndex = newX + 5 * newY;
                        B[dstIndex] = Rol64(state[srcIndex], r[x, y]);
                    }
                }

                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        int idx = x + 5 * y;
                        state[idx] = B[idx] ^ ((~B[((x + 1) % 5) + 5 * y]) & B[((x + 2) % 5) + 5 * y]);
                    }
                }

                state[0] ^= KeccakRoundConstants[round];
            }
        }

        private static ulong Rol64(ulong value, int offset)
        {
            offset &= 63;
            if (offset is 0)
            {
                return value;
            }

            return (value << offset) | (value >> (64 - offset));
        }
    }
}
