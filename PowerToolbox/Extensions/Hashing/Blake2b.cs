using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// Blake2b 校验实现
    /// </summary>
    public sealed class Blake2b : HashAlgorithm
    {
        private const int BlockBytes = 128;
        private const int MaxHashBytes = 64;

        private static readonly ulong[] IV =
        [
            0x6A09E667F3BCC908UL, 0xBB67AE8584CAA73BUL,
            0x3C6EF372FE94F82BUL, 0xA54FF53A5F1D36F1UL,
            0x510E527FADE682D1UL, 0x9B05688C2B3E6C1FUL,
            0x1F83D9ABFB41BD6BUL, 0x5BE0CD19137E2179UL
        ];

        private static readonly byte[,] Sigma = new byte[,]
        {
            {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15},
            {14,10,4,8,9,15,13,6,1,12,0,2,11,7,5,3},
            {11,8,12,0,5,2,15,13,10,14,3,6,7,1,9,4},
            {7,9,3,1,13,12,11,14,2,6,5,10,4,0,15,8},
            {9,0,5,7,2,4,10,15,14,1,11,12,6,8,3,13},
            {2,12,6,10,0,11,8,3,4,13,7,5,15,14,1,9},
            {12,5,1,15,14,13,4,10,0,7,6,3,9,2,8,11},
            {13,11,7,14,12,1,3,9,5,0,15,4,8,6,2,10},
            {6,15,14,9,11,3,0,8,12,2,13,7,1,4,10,5},
            {10,2,8,4,7,6,1,5,15,11,9,14,3,12,13,0}
        };

        // state
        private readonly ulong[] h = new ulong[8];

        private readonly ulong[] m = new ulong[16];
        private readonly ulong[] v = new ulong[16];

        private readonly byte[] buffer = new byte[BlockBytes];
        private int bufferPos;
        private ulong totalBytes;

        private readonly int hashLength;
        private readonly byte[] key;

        public Blake2b(int hashBytes = 64, byte[] key = null)
        {
            if (hashBytes <= 0 || hashBytes > MaxHashBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(hashBytes));
            }

            if (key is not null && key.Length > MaxHashBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(key), "Key length must be <= 64 bytes for Blake2b.");
            }

            hashLength = hashBytes;
            this.key = key is not null && key.Length > 0 ? (byte[])key.Clone() : null;

            HashSizeValue = hashBytes * 8;
            Initialize();
        }

        public override void Initialize()
        {
            // Initialize hash state with IV
            Array.Copy(IV, h, IV.Length);

            // Parameter block as in reference implementations: fanout=1, depth=1; leaf_length,node_offset,node_depth,node_leaf_length = 0
            // Parameter word: 0x01010000 ^ (keylen << 8) ^ outlen
            ulong param0 = 0x01010000UL ^ ((ulong)(key?.Length ?? 0) << 8) ^ (ulong)hashLength;
            h[0] ^= param0;

            bufferPos = 0;
            totalBytes = 0;

            if (key is not null && key.Length > 0)
            {
                // If key provided, process a single block with key padded with zeros
                byte[] block = new byte[BlockBytes];
                Array.Copy(key, 0, block, 0, key.Length);
                // feed as if first message block
                Update(block, 0, block.Length);
            }
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            Update(array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            // finalization: pad remaining buffer with zeros and compress with final flag
            // totalBytes currently counts bytes already compressed

            // Create final block
            byte[] block = new byte[bufferPos];
            Array.Copy(buffer, 0, block, 0, bufferPos);

            // Compress remaining bytes
            // pad block to full and call compress with isLast = true
            byte[] padded = new byte[bufferPos];
            Array.Copy(block, 0, padded, 0, bufferPos);

            // compress last
            Compress(padded, 0, true);

            // produce digest little-endian
            byte[] outBytes = new byte[hashLength];
            for (int i = 0; i < hashLength; i++)
            {
                outBytes[i] = (byte)(h[i / 8] >> (8 * (i % 8)));
            }

            // reset internal state for potential reuse
            Initialize();
            return outBytes;
        }

        private void Update(byte[] data, int offset, int count)
        {
            int pos = offset;
            int remaining = count;
            while (remaining > 0)
            {
                int take = Math.Min(remaining, BlockBytes - bufferPos);
                Array.Copy(data, pos, buffer, bufferPos, take);
                bufferPos += take;
                pos += take;
                remaining -= take;

                if (bufferPos == BlockBytes)
                {
                    // compress full block
                    Compress(buffer, 0, false);
                    totalBytes += (ulong)BlockBytes;
                    bufferPos = 0;
                }
            }
        }

        private static ulong ROR(ulong x, int n) => (x >> n) | (x << (64 - n));

        private void Compress(byte[] block, int offset, bool isLast)
        {
            // load message words m[0..15] as little-endian
            for (int i = 0; i < 16; i++)
            {
                int j = offset + i * 8;
                ulong v0 = 0UL;
                for (int b = 0; b < 8; b++) v0 |= ((ulong)block.Length > (uint)j + (uint)b ? (ulong)block[j + b] : 0UL) << (8 * b);
                m[i] = v0;
            }

            // initialize v
            for (int i = 0; i < 8; i++) v[i] = h[i];
            for (int i = 0; i < 8; i++) v[i + 8] = IV[i];

            // counter t: total bytes compressed so far + block length
            // we use little-endian 128-bit counter split into v[12], v[13]
            ulong t0 = totalBytes + (ulong)(block.Length);
            ulong t1 = 0UL; // we don't expect >2^64 bytes in practical use
            v[12] ^= t0;
            v[13] ^= t1;

            if (isLast)
            {
                v[14] = ~v[14];
            }

            // 12 rounds
            for (int round = 0; round < 12; round++)
            {
                // column step
                G(0, 4, 8, 12, 0, 1, round);
                G(1, 5, 9, 13, 2, 3, round);
                G(2, 6, 10, 14, 4, 5, round);
                G(3, 7, 11, 15, 6, 7, round);
                // diagonal step
                G(0, 5, 10, 15, 8, 9, round);
                G(1, 6, 11, 12, 10, 11, round);
                G(2, 7, 8, 13, 12, 13, round);
                G(3, 4, 9, 14, 14, 15, round);
            }

            for (int i = 0; i < 8; i++)
                h[i] ^= v[i] ^ v[i + 8];
        }

        /// <summary>
        /// The G mixing function as per spec, but we index message words by sigma matrix
        /// </summary>
        private void G(int a, int b, int c, int d, int xIdx, int yIdx, int round)
        {
            // sigma selection
            int r = round % 10;
            byte s0 = Sigma[r, xIdx];
            byte s1 = Sigma[r, yIdx];

            unchecked
            {
                v[a] = v[a] + v[b] + m[s0];
                v[d] = ROR(v[d] ^ v[a], 32);
                v[c] = v[c] + v[d];
                v[b] = ROR(v[b] ^ v[c], 24);
                v[a] = v[a] + v[b] + m[s1];
                v[d] = ROR(v[d] ^ v[a], 16);
                v[c] = v[c] + v[d];
                v[b] = ROR(v[b] ^ v[c], 63);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (buffer is not null)
            {
                Array.Clear(buffer, 0, buffer.Length);
            }

            if (m is not null)
            {
                Array.Clear(m, 0, m.Length);
            }

            if (v is not null)
            {
                Array.Clear(v, 0, v.Length);
            }

            if (h is not null)
            {
                Array.Clear(h, 0, h.Length);
            }
        }
    }
}
