using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// XXH3 64位 校验实现
    /// </summary>
    internal sealed class XxHash3_64 : HashAlgorithm
    {
        private const uint PRIME32_1 = 0x9E3779B1U;
        private const uint PRIME32_2 = 0x85EBCA77U;
        private const uint PRIME32_3 = 0xC2B2AE3DU;
        private const ulong PRIME64_1 = 0x9E3779B185EBCA87UL;
        private const ulong PRIME64_2 = 0xC2B2AE3D27D4EB4FUL;
        private const ulong PRIME64_3 = 0x165667B19E3779F9UL;
        private const ulong PRIME64_4 = 0x85EBCA77C2B2AE63UL;
        private const ulong PRIME64_5 = 0x27D4EB2F165667C5UL;
        private const int STRIPE_LEN = 64;
        private const int ACC_NB = STRIPE_LEN / 8;
        private const int SECRET_DEFAULT_SIZE = 192;
        private const int SECRET_CONSUME_RATE = 8;
        private const int MIDSIZE_MAX = 240;
        private const int SECRET_LASTACC_START = 7;
        private const int SECRET_MERGEACCS_START = 11;
        private readonly ulong _seed;
        private MemoryStream _stream;

        // Official XXH3 default secret
        private static readonly byte[] DefaultSecret =
        [
            0xb8, 0xfe, 0x6c, 0x39, 0x23, 0xa4, 0x4b, 0xbe,
            0x7c, 0x01, 0x81, 0x2c, 0xf7, 0x21, 0xad, 0x1c,
            0xde, 0xd4, 0x6d, 0xe9, 0x83, 0x90, 0x97, 0xdb,
            0x72, 0x40, 0xa4, 0xa4, 0xb7, 0xb3, 0x67, 0x1f,
            0xcb, 0x79, 0xe6, 0x4e, 0xcc, 0xc0, 0xe5, 0x78,
            0x82, 0x5a, 0xd0, 0x7d, 0xcc, 0xff, 0x72, 0x21,
            0xb8, 0x08, 0x46, 0x74, 0xf7, 0x43, 0x24, 0x8e,
            0xe0, 0x35, 0x90, 0xe6, 0x81, 0x3a, 0x26, 0x4c,
            0x3c, 0x28, 0x52, 0xbb, 0x91, 0xc3, 0x00, 0xcb,
            0x88, 0xd0, 0x65, 0x8b, 0x1b, 0x53, 0x2e, 0xa3,
            0x71, 0x64, 0x48, 0x97, 0xa2, 0x0d, 0xf9, 0x4e,
            0x38, 0x19, 0xef, 0x46, 0xa9, 0xde, 0xac, 0xd8,
            0xa8, 0xfa, 0x76, 0x3f, 0xe3, 0x9c, 0x34, 0x3f,
            0xf9, 0xdc, 0xbb, 0xc7, 0xc7, 0x0b, 0x4f, 0x1d,
            0x8a, 0x51, 0xe0, 0x4b, 0xcd, 0xb4, 0x59, 0x31,
            0xc8, 0x9f, 0x7e, 0xc9, 0xd9, 0x78, 0x73, 0x64,
            0xea, 0xc5, 0xac, 0x83, 0x34, 0xd3, 0xeb, 0xc3,
            0xc5, 0x81, 0xa0, 0xff, 0xfa, 0x13, 0x63, 0xeb,
            0x17, 0x0d, 0xdd, 0x51, 0xb7, 0xf0, 0xda, 0x49,
            0xd3, 0x16, 0x55, 0x26, 0x29, 0xd4, 0x68, 0x9e,
            0x2b, 0x16, 0xbe, 0x58, 0x7d, 0x47, 0xa1, 0xfc,
            0x8f, 0xf8, 0xb8, 0xd1, 0x7a, 0xd0, 0x31, 0xce,
            0x45, 0xcb, 0x3a, 0x8f, 0x95, 0x16, 0x04, 0x28,
            0xaf, 0xd7, 0xfb, 0xca, 0xbb, 0x4b, 0x40, 0x7e
        ];

        internal XxHash3_64() : this(0)
        {
        }

        internal XxHash3_64(ulong seed)
        {
            _seed = seed;
            _stream = new MemoryStream();
            HashSizeValue = 64;
        }

        public override void Initialize()
        {
            _stream?.Dispose();
            _stream = new MemoryStream();
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            _stream.Write(array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            ulong hash = Hash(_stream.ToArray(), _seed);

            // HashAlgorithm output uses big-endian byte order here.
            byte[] result =
            [
                (byte)(hash >> 56),
                (byte)(hash >> 48),
                (byte)(hash >> 40),
                (byte)(hash >> 32),
                (byte)(hash >> 24),
                (byte)(hash >> 16),
                (byte)(hash >> 8),
                (byte)hash,
            ];
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream?.Dispose();
                _stream = null;
            }

            base.Dispose(disposing);
        }

        internal new static ulong Hash(byte[] data)
        {
            return Hash(data, 0);
        }

        internal new static ulong Hash(byte[] data, ulong seed)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return Hash(data, 0, data.Length, seed);
        }

        internal new static ulong Hash(byte[] data, int offset, int length)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return Hash(data, offset, length, 0);
        }

        internal new static ulong Hash(byte[] data, int offset, int length, ulong seed)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (offset > data.Length - length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "offset is valid");
            }

            if (length <= 16)
            {
                return HashLen0To16(data, offset, length, seed);
            }

            if (length <= 128)
            {
                return HashLen17To128(data, offset, length, seed);
            }

            if (length <= MIDSIZE_MAX)
            {
                return HashLen129To240(data, offset, length, seed);
            }

            return HashLong(data, offset, length, seed);
        }

        internal static ulong HashString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            return HashString(value, Encoding.UTF8, 0);
        }

        internal static ulong HashString(string value, ulong seed)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            return HashString(value, Encoding.UTF8, seed);
        }

        internal static ulong HashString(string value, Encoding encoding, ulong seed)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (encoding is null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            byte[] data = encoding.GetBytes(value);
            return Hash(data, seed);
        }

        public static ulong HashStream(Stream stream)
        {
            if (stream is null)
            {
                return default;
            }

            return HashStream(stream, 0);
        }

        public static ulong HashStream(Stream stream, ulong seed)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using MemoryStream memoryStream = new();
            stream.CopyTo(memoryStream);
            return Hash(memoryStream.GetBuffer(), 0, (int)memoryStream.Length, seed);
        }

        public static ulong HashFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return default;
            }

            return HashFile(fileName, 0);
        }

        public static ulong HashFile(string fileName, ulong seed)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using FileStream stream = new(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return HashStream(stream, seed);
        }

        /// <summary>
        /// 0 - 16 bytes
        /// </summary>
        private static ulong HashLen0To16(byte[] input, int offset, int len, ulong seed)
        {
            if (input is null)
            {
                return default;
            }

            if (len > 8)
            {
                return HashLen9To16(input, offset, len, seed);
            }

            if (len >= 4)
            {
                return HashLen4To8(input, offset, len, seed);
            }

            if (len > 0)
            {
                return HashLen1To3(input, offset, len, seed);
            }

            ulong bitflip = Read64(DefaultSecret, 56) ^ Read64(DefaultSecret, 64);
            return Avalanche(seed ^ bitflip);
        }

        private static ulong HashLen1To3(byte[] input, int offset, int len, ulong seed)
        {
            if (input is null)
            {
                return default;
            }

            byte c1 = input[offset];
            byte c2 = input[offset + (len >> 1)];
            byte c3 = input[offset + len - 1];
            uint combined = ((uint)c1 << 16) | ((uint)c2 << 24) | c3 | ((uint)len << 8);
            ulong bitflip = ((ulong)Read32(DefaultSecret, 0) ^ Read32(DefaultSecret, 4)) + seed;
            ulong keyed = combined ^ bitflip;
            return XXH64Avalanche(keyed);
        }

        private static ulong HashLen4To8(byte[] input, int offset, int len, ulong seed)
        {
            if (input is null)
            {
                return default;
            }

            seed ^= (ulong)Swap32((uint)seed) << 32;
            uint input1 = Read32(input, offset);
            uint input2 = Read32(input, offset + len - 4);
            ulong input64 = ((ulong)input2 << 32) | input1;
            ulong bitflip = (Read64(DefaultSecret, 8) ^ Read64(DefaultSecret, 16)) - seed;
            ulong keyed = input64 ^ bitflip;
            return Rrmxmx(keyed, (uint)len);
        }

        private static ulong HashLen9To16(byte[] input, int offset, int len, ulong seed)
        {
            if (input is null)
            {
                return default;
            }

            ulong bitflip1 = (Read64(DefaultSecret, 24) ^ Read64(DefaultSecret, 32)) + seed;
            ulong bitflip2 = (Read64(DefaultSecret, 40) ^ Read64(DefaultSecret, 48)) - seed;
            ulong inputLo = Read64(input, offset) ^ bitflip1;
            ulong inputHi = Read64(input, offset + len - 8) ^ bitflip2;
            ulong acc = (ulong)len + Swap64(inputLo) + inputHi + Mul128Fold64(inputLo, inputHi);
            return Avalanche(acc);
        }

        /// <summary>
        /// 17 - 128 bytes
        /// </summary>
        private static ulong HashLen17To128(byte[] input, int offset, int len, ulong seed)
        {
            if (input is null)
            {
                return default;
            }

            ulong acc = (ulong)len * PRIME64_1;

            if (len > 32)
            {
                if (len > 64)
                {
                    if (len > 96)
                    {
                        acc += Mix16B(input, offset + 48, DefaultSecret, 96, seed);
                        acc += Mix16B(input, offset + len - 64, DefaultSecret, 112, seed);
                    }

                    acc += Mix16B(input, offset + 32, DefaultSecret, 64, seed);
                    acc += Mix16B(input, offset + len - 48, DefaultSecret, 80, seed);
                }

                acc += Mix16B(input, offset + 16, DefaultSecret, 32, seed);
                acc += Mix16B(input, offset + len - 32, DefaultSecret, 48, seed);
            }

            acc += Mix16B(input, offset, DefaultSecret, 0, seed);
            acc += Mix16B(input, offset + len - 16, DefaultSecret, 16, seed);
            return Avalanche(acc);
        }

        /// <summary>
        /// 129 - 240 bytes
        /// </summary>
        private static ulong HashLen129To240(byte[] input, int offset, int len, ulong seed)
        {
            if (input is null)
            {
                return default;
            }

            ulong acc = (ulong)len * PRIME64_1;
            int i;

            // First 8 rounds
            for (i = 0; i < 8; i++)
            {
                acc += Mix16B(input, offset + (16 * i), DefaultSecret, 16 * i, seed);
            }

            acc = Avalanche(acc);

            // Remaining rounds
            for (i = 8; i < len / 16; i++)
            {
                acc += Mix16B(input, offset + (16 * i), DefaultSecret, 16 * (i - 8) + 3, seed);
            }

            // Last 16 bytes
            acc += Mix16B(input, offset + len - 16, DefaultSecret, SECRET_DEFAULT_SIZE - 17, seed);
            return Avalanche(acc);
        }

        /// <summary>
        /// Long Input
        /// </summary>
        private static ulong HashLong(byte[] input, int offset, int len, ulong seed)
        {
            if (input is null)
            {
                return default;
            }

            byte[] secret = seed is 0 ? DefaultSecret : InitCustomSecret(seed);
            ulong[] acc = [PRIME32_3, PRIME64_1, PRIME64_2, PRIME64_3, PRIME64_4, PRIME32_2, PRIME64_5, PRIME32_1];
            HashLongInternalLoop(acc, input, offset, len, secret);

            // XXH_SECRET_MERGEACCS_START = 11
            return MergeAccs(acc, secret, SECRET_MERGEACCS_START, (ulong)len * PRIME64_1);
        }

        private static void HashLongInternalLoop(ulong[] acc, byte[] input, int offset, int len, byte[] secret)
        {
            if (acc is null || input is null || secret is null)
            {
                return;
            }

            int stripesPerBlock = (SECRET_DEFAULT_SIZE - STRIPE_LEN) / SECRET_CONSUME_RATE;
            int blockLen = STRIPE_LEN * stripesPerBlock;
            int nbBlocks = (len - 1) / blockLen;
            int inputOffset = offset;
            int block;

            // Process full blocks
            for (block = 0; block < nbBlocks; block++)
            {
                Accumulate(acc, input, inputOffset, secret, 0, stripesPerBlock);
                ScrambleAcc(acc, secret, SECRET_DEFAULT_SIZE - STRIPE_LEN);
                inputOffset += blockLen;
            }

            // Process remaining stripes
            {
                int nbStripes = (len - 1 - (blockLen * nbBlocks)) / STRIPE_LEN;

                if (nbStripes > 0)
                {
                    Accumulate(acc, input, inputOffset, secret, 0, nbStripes);
                }
            }

            /*
             * Last stripe
             *
             * Official:
             *
             * secret + secretSize
             * - STRIPE_LEN
             * - XXH_SECRET_LASTACC_START
             *
             * = 192 - 64 - 7
             * = 121
             */
            Accumulate512(acc, input, offset + len - STRIPE_LEN, secret, SECRET_DEFAULT_SIZE - STRIPE_LEN - SECRET_LASTACC_START);
        }

        /// <summary>
        /// Long Input Accumulator
        /// </summary>
        private static void Accumulate(ulong[] acc, byte[] input, int inputOffset, byte[] secret, int secretOffset, int nbStripes)
        {
            if (acc is null || input is null || secret is null)
            {
                return;
            }

            for (int stripe = 0; stripe < nbStripes; stripe++)
            {
                Accumulate512(acc, input, inputOffset + stripe * STRIPE_LEN, secret, secretOffset + stripe * SECRET_CONSUME_RATE);
            }
        }

        private static void Accumulate512(ulong[] acc, byte[] input, int inputOffset, byte[] secret, int secretOffset)
        {
            if (acc is null || input is null || secret is null)
            {
                return;
            }

            for (int i = 0; i < ACC_NB; i++)
            {
                ulong dataVal = Read64(input, inputOffset + (8 * i));
                ulong dataKey = dataVal ^ Read64(secret, secretOffset + (8 * i));

                /*
                 * Official scalar accumulator:
                 *
                 * acc[i ^ 1] += dataVal;
                 * acc[i] += (u32)dataKey * (dataKey >> 32);
                 */
                acc[i ^ 1] += dataVal;
                acc[i] += (ulong)(uint)dataKey * (uint)(dataKey >> 32);
            }
        }

        private static void ScrambleAcc(ulong[] acc, byte[] secret, int secretOffset)
        {
            if (acc is null || secret is null)
            {
                return;
            }

            for (int i = 0; i < ACC_NB; i++)
            {
                ulong acc64 = acc[i];
                ulong key64 = Read64(secret, secretOffset + (8 * i));
                acc64 ^= acc64 >> 47;
                acc64 ^= key64;
                acc64 *= PRIME32_1;
                acc[i] = acc64;
            }
        }

        /// <summary>
        /// Merge Accumulators
        /// </summary>
        private static ulong MergeAccs(ulong[] acc, byte[] secret, int secretOffset, ulong start)
        {
            if (acc is null || secret is null)
            {
                return default;
            }

            ulong result = start;
            result += Mix2Accs(acc[0], acc[1], secret, secretOffset);
            result += Mix2Accs(acc[2], acc[3], secret, secretOffset + 16);
            result += Mix2Accs(acc[4], acc[5], secret, secretOffset + 32);
            result += Mix2Accs(acc[6], acc[7], secret, secretOffset + 48);
            return Avalanche(result);
        }

        private static ulong Mix2Accs(ulong accLow, ulong accHigh, byte[] secret, int secretOffset)
        {
            if (secret is null)
            {
                return default;
            }

            ulong lhs = accLow ^ Read64(secret, secretOffset);
            ulong rhs = accHigh ^ Read64(secret, secretOffset + 8);
            return Mul128Fold64(lhs, rhs);
        }

        /// <summary>
        /// Seed Secret
        /// </summary>
        private static byte[] InitCustomSecret(ulong seed)
        {
            byte[] secret = new byte[SECRET_DEFAULT_SIZE];

            for (int i = 0; i < SECRET_DEFAULT_SIZE; i += 16)
            {
                ulong lo = Read64(DefaultSecret, i) + seed;
                ulong hi = Read64(DefaultSecret, i + 8) - seed;
                Write64(secret, i, lo);
                Write64(secret, i + 8, hi);
            }

            return secret;
        }

        /// <summary>
        /// Mix16B
        /// </summary>
        private static ulong Mix16B(byte[] input, int inputOffset, byte[] secret, int secretOffset, ulong seed)
        {
            if (input is null)
            {
                return default;
            }

            ulong inputLo = Read64(input, inputOffset);
            ulong inputHi = Read64(input, inputOffset + 8);
            ulong secretLo = Read64(secret, secretOffset);
            ulong secretHi = Read64(secret, secretOffset + 8);
            ulong lhs = inputLo ^ (secretLo + seed);
            ulong rhs = inputHi ^ (secretHi - seed);
            return Mul128Fold64(lhs, rhs);
        }

        /// <summary>
        /// Avalanche
        /// </summary>
        private static ulong Avalanche(ulong h64)
        {
            h64 ^= h64 >> 37;
            h64 *= 0x165667919E3779F9UL;
            h64 ^= h64 >> 32;
            return h64;
        }

        private static ulong XXH64Avalanche(ulong h64)
        {
            h64 ^= h64 >> 33;
            h64 *= PRIME64_2;
            h64 ^= h64 >> 29;
            h64 *= PRIME64_3;
            h64 ^= h64 >> 32;
            return h64;
        }

        private static ulong Rrmxmx(ulong h64, uint len)
        {
            h64 ^= RotateLeft(h64, 49) ^ RotateLeft(h64, 24);
            h64 *= 0x9FB21C651E98DF25UL;
            h64 ^= (h64 >> 35) + len;
            h64 *= 0x9FB21C651E98DF25UL;
            h64 ^= h64 >> 28;
            return h64;
        }

        /// <summary>
        /// 64 x 64 -> 128
        /// Calculates:
        /// (lhs* rhs) low64 XOR high64
        /// Does not require UInt128.
        /// </summary>
        private static ulong Mul128Fold64(ulong lhs, ulong rhs)
        {
            ulong lhsLo = (uint)lhs;
            ulong lhsHi = lhs >> 32;
            ulong rhsLo = (uint)rhs;
            ulong rhsHi = rhs >> 32;
            ulong productLoLo = lhsLo * rhsLo;
            ulong productHiLo = lhsHi * rhsLo;
            ulong productLoHi = lhsLo * rhsHi;
            ulong productHiHi = lhsHi * rhsHi;
            ulong cross = (productLoLo >> 32) + (uint)productHiLo + productLoHi;
            ulong low64 = (productLoLo & 0xFFFFFFFFUL) | (cross << 32);
            ulong high64 = productHiHi + (productHiLo >> 32) + (cross >> 32);
            return low64 ^ high64;
        }

        /// <summary>
        /// Read / Write
        /// </summary>
        private static uint Read32(byte[] buffer, int offset)
        {
            if (buffer is null)
            {
                return default;
            }

            return buffer[offset] | ((uint)buffer[offset + 1] << 8) | ((uint)buffer[offset + 2] << 16) | ((uint)buffer[offset + 3] << 24);
        }

        private static ulong Read64(byte[] buffer, int offset)
        {
            if (buffer is null)
            {
                return default;
            }

            return
                buffer[offset]
                |
                ((ulong)buffer[offset + 1] << 8)
                |
                ((ulong)buffer[offset + 2] << 16)
                |
                ((ulong)buffer[offset + 3] << 24)
                |
                ((ulong)buffer[offset + 4] << 32)
                |
                ((ulong)buffer[offset + 5] << 40)
                |
                ((ulong)buffer[offset + 6] << 48)
                |
                ((ulong)buffer[offset + 7] << 56);
        }

        private static void Write64(byte[] buffer, int offset, ulong value)
        {
            if (buffer is null)
            {
                return;
            }

            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
            buffer[offset + 4] = (byte)(value >> 32);
            buffer[offset + 5] = (byte)(value >> 40);
            buffer[offset + 6] = (byte)(value >> 48);
            buffer[offset + 7] = (byte)(value >> 56);
        }

        /// <summary>
        /// Bit Operations
        /// </summary>
        private static ulong RotateLeft(ulong value, int count)
        {
            return (value << count) | (value >> (64 - count));
        }

        private static uint Swap32(uint value)
        {
            return
                ((value & 0x000000FFU) << 24)
                |
                ((value & 0x0000FF00U) << 8)
                |
                ((value & 0x00FF0000U) >> 8)
                |
                ((value & 0xFF000000U) >> 24);
        }

        private static ulong Swap64(ulong value)
        {
            return
                ((value & 0x00000000000000FFUL) << 56)
                |
                ((value & 0x000000000000FF00UL) << 40)
                |
                ((value & 0x0000000000FF0000UL) << 24)
                |
                ((value & 0x00000000FF000000UL) << 8)
                |
                ((value & 0x000000FF00000000UL) >> 8)
                |
                ((value & 0x0000FF0000000000UL) >> 24)
                |
                ((value & 0x00FF000000000000UL) >> 40)
                |
                ((value & 0xFF00000000000000UL) >> 56);
        }
    }
}
