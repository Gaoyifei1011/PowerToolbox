using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// XXH3 128位 校验实现
    /// </summary>
    internal sealed unsafe class XxHash3_128Bits
    {
        internal const int StripeLengthBytes = 64;
        internal const int SecretLengthBytes = 192;
        internal const int SecretSizeMin = 136;
        internal const int SecretLastAccStartBytes = 7;
        internal const int SecretConsumeRateBytes = 8;
        internal const int SecretMergeAccsStartBytes = 11;
        internal const int NumStripesPerBlock = (SecretLengthBytes - StripeLengthBytes) / SecretConsumeRateBytes;
        internal const int AccumulatorCount = StripeLengthBytes / sizeof(ulong);
        internal const int MidSizeMaxBytes = 240;
        internal const int InternalBufferStripes = InternalBufferLengthBytes / StripeLengthBytes;
        internal const int InternalBufferLengthBytes = 256;

        // 将 DefaultSecret byte[] 转换为 ulong[]（对应关系见上文）
        internal const ulong DefaultSecretUInt64_0 = 0xBE4BA423396CFEB8;

        internal const ulong DefaultSecretUInt64_1 = 0x1CAD21F72C81017C;
        internal const ulong DefaultSecretUInt64_2 = 0xDB979083E96DD4DE;
        internal const ulong DefaultSecretUInt64_3 = 0x1F67B3B7A4A44072;
        internal const ulong DefaultSecretUInt64_4 = 0x78E5C0CC4EE679CB;
        internal const ulong DefaultSecretUInt64_5 = 0x2172FFCC7DD05A82;
        internal const ulong DefaultSecretUInt64_6 = 0x8E2443F7744608B8;
        internal const ulong DefaultSecretUInt64_7 = 0x4C263A81E69035E0;
        internal const ulong DefaultSecretUInt64_8 = 0xCB00C391BB52283C;
        internal const ulong DefaultSecretUInt64_9 = 0xA32E531B8B65D088;
        internal const ulong DefaultSecretUInt64_10 = 0x4EF90DA297486471;
        internal const ulong DefaultSecretUInt64_11 = 0xD8ACDEA946EF1938;
        internal const ulong DefaultSecretUInt64_12 = 0x3F349CE33F76FAA8;
        internal const ulong DefaultSecretUInt64_13 = 0x1D4F0BC7C7BBDCF9;
        internal const ulong DefaultSecretUInt64_14 = 0x3159B4CD4BE0518A;
        internal const ulong DefaultSecretUInt64_15 = 0x647378D9C97E9FC8;

        // 将 DefaultSecret 的偏移量偏移 3 字节，byte[] => ulong[]
        internal const ulong DefaultSecret3UInt64_0 = 0x81017CBE4BA42339;

        internal const ulong DefaultSecret3UInt64_1 = 0x6DD4DE1CAD21F72C;
        internal const ulong DefaultSecret3UInt64_2 = 0xA44072DB979083E9;
        internal const ulong DefaultSecret3UInt64_3 = 0xE679CB1F67B3B7A4;
        internal const ulong DefaultSecret3UInt64_4 = 0xD05A8278E5C0CC4E;
        internal const ulong DefaultSecret3UInt64_5 = 0x4608B82172FFCC7D;
        internal const ulong DefaultSecret3UInt64_6 = 0x9035E08E2443F774;
        internal const ulong DefaultSecret3UInt64_7 = 0x52283C4C263A81E6;
        internal const ulong DefaultSecret3UInt64_8 = 0x65D088CB00C391BB;
        internal const ulong DefaultSecret3UInt64_9 = 0x486471A32E531B8B;
        internal const ulong DefaultSecret3UInt64_10 = 0xEF19384EF90DA297;
        internal const ulong DefaultSecret3UInt64_11 = 0x76FAA8D8ACDEA946;
        internal const ulong DefaultSecret3UInt64_12 = 0xBBDCF93F349CE33F;
        internal const ulong DefaultSecret3UInt64_13 = 0xE0518A1D4F0BC7C7;

        internal const ulong Prime64_1 = 0x9E3779B185EBCA87UL;
        internal const ulong Prime64_2 = 0xC2B2AE3D27D4EB4FUL;
        internal const ulong Prime64_3 = 0x165667B19E3779F9UL;
        internal const ulong Prime64_4 = 0x85EBCA77C2B2AE63UL;
        internal const ulong Prime64_5 = 0x27D4EB2F165667C5UL;

        internal const uint Prime32_1 = 0x9E3779B1U;
        internal const uint Prime32_2 = 0x85EBCA77U;
        internal const uint Prime32_3 = 0xC2B2AE3DU;
        internal const uint Prime32_4 = 0x27D4EB2FU;
        internal const uint Prime32_5 = 0x165667B1U;

        /// <summary>
        /// 获取在未提供种子时的默认密钥
        /// </summary>
        /// <remarks>这与从种子0导出的自定义秘密相同</remarks>
        internal static readonly byte[] DefaultSecret =
        [
            0xb8, 0xfe, 0x6c, 0x39, 0x23, 0xa4, 0x4b, 0xbe, // DefaultSecretUInt64_0
            0x7c, 0x01, 0x81, 0x2c, 0xf7, 0x21, 0xad, 0x1c, // DefaultSecretUInt64_1
            0xde, 0xd4, 0x6d, 0xe9, 0x83, 0x90, 0x97, 0xdb, // DefaultSecretUInt64_2
            0x72, 0x40, 0xa4, 0xa4, 0xb7, 0xb3, 0x67, 0x1f, // DefaultSecretUInt64_3
            0xcb, 0x79, 0xe6, 0x4e, 0xcc, 0xc0, 0xe5, 0x78, // DefaultSecretUInt64_4
            0x82, 0x5a, 0xd0, 0x7d, 0xcc, 0xff, 0x72, 0x21, // DefaultSecretUInt64_5
            0xb8, 0x08, 0x46, 0x74, 0xf7, 0x43, 0x24, 0x8e, // DefaultSecretUInt64_6
            0xe0, 0x35, 0x90, 0xe6, 0x81, 0x3a, 0x26, 0x4c, // DefaultSecretUInt64_7
            0x3c, 0x28, 0x52, 0xbb, 0x91, 0xc3, 0x00, 0xcb, // DefaultSecretUInt64_8
            0x88, 0xd0, 0x65, 0x8b, 0x1b, 0x53, 0x2e, 0xa3, // DefaultSecretUInt64_9
            0x71, 0x64, 0x48, 0x97, 0xa2, 0x0d, 0xf9, 0x4e, // DefaultSecretUInt64_10
            0x38, 0x19, 0xef, 0x46, 0xa9, 0xde, 0xac, 0xd8, // DefaultSecretUInt64_11
            0xa8, 0xfa, 0x76, 0x3f, 0xe3, 0x9c, 0x34, 0x3f, // DefaultSecretUInt64_12
            0xf9, 0xdc, 0xbb, 0xc7, 0xc7, 0x0b, 0x4f, 0x1d, // DefaultSecretUInt64_13
            0x8a, 0x51, 0xe0, 0x4b, 0xcd, 0xb4, 0x59, 0x31, // DefaultSecretUInt64_14
            0xc8, 0x9f, 0x7e, 0xc9, 0xd9, 0x78, 0x73, 0x64, // DefaultSecretUInt64_15
            0xea, 0xc5, 0xac, 0x83, 0x34, 0xd3, 0xeb, 0xc3, // DefaultSecretUInt64_16
            0xc5, 0x81, 0xa0, 0xff, 0xfa, 0x13, 0x63, 0xeb, // DefaultSecretUInt64_17
            0x17, 0x0d, 0xdd, 0x51, 0xb7, 0xf0, 0xda, 0x49, // DefaultSecretUInt64_18
            0xd3, 0x16, 0x55, 0x26, 0x29, 0xd4, 0x68, 0x9e, // DefaultSecretUInt64_19
            0x2b, 0x16, 0xbe, 0x58, 0x7d, 0x47, 0xa1, 0xfc, // DefaultSecretUInt64_20
            0x8f, 0xf8, 0xb8, 0xd1, 0x7a, 0xd0, 0x31, 0xce, // DefaultSecretUInt64_21
            0x45, 0xcb, 0x3a, 0x8f, 0x95, 0x16, 0x04, 0x28, // DefaultSecretUInt64_22
            0xaf, 0xd7, 0xfb, 0xca, 0xbb, 0x4b, 0x40, 0x7e, // DefaultSecretUInt64_23
        ];

        /// <summary>
        /// XXH128 生成 16 字节的哈希
        /// </summary>
        private const int HashLengthInBytes = 16;

        private State _state;

        /// <summary>
        /// 使用默认种子值 0 初始化 XxHash3_128Bits 类的新实例
        /// </summary>
        internal XxHash3_128Bits() : this(0)
        {
        }

        /// <summary>
        /// 使用指定的种子初始化 XxHash3_128Bits 类的新实例
        /// </summary>
        internal XxHash3_128Bits(long seed)
        {
            Initialize(ref _state, (ulong)seed);
        }

        /// <summary>
        /// 使用另一个实例的状态初始化 XxHash3_128Bits 类的新实例
        /// </summary>
        private XxHash3_128Bits(State state)
        {
            _state = state;
        }

        internal static void Initialize(ref State state, ulong seed)
        {
            state.Seed = seed;

            fixed (byte* secret = state.Secret)
            {
                if (seed is 0)
                {
                    for (int i = 0; i < SecretLengthBytes; i++)
                    {
                        secret[i] = DefaultSecret[i];
                    }
                }
                else
                {
                    DeriveSecretFromSeed(secret, seed);
                }
            }

            Reset(ref state);
        }

        internal static void Reset(ref State state)
        {
            state.BufferedCount = 0;
            state.StripesProcessedInCurrentBlock = 0;
            state.TotalLength = 0;

            fixed (ulong* accumulators = state.Accumulators)
            {
                InitializeAccumulators(accumulators);
            }
        }

        internal static void Append(ref State state, byte[] source)
        {
            if (source is null)
            {
                return;
            }

            state.TotalLength += (uint)source.Length;

            fixed (byte* buffer = state.Buffer)
            {
                // Small input: just copy the data to the buffer.
                if (source.Length <= InternalBufferLengthBytes - state.BufferedCount)
                {
                    fixed (byte* sourcePtr = source)
                    {
                        Buffer.MemoryCopy(sourcePtr, buffer + state.BufferedCount, source.Length, source.Length);
                    }

                    state.BufferedCount += (uint)source.Length;
                    return;
                }

                fixed (byte* secret = state.Secret)
                fixed (ulong* accumulators = state.Accumulators)
                fixed (byte* sourcePtr = source)
                {
                    // Internal buffer is partially filled (always, except at beginning). Complete it, then consume it.
                    int sourceIndex = 0;
                    if (state.BufferedCount is not 0)
                    {
                        int loadSize = InternalBufferLengthBytes - (int)state.BufferedCount;
                        Buffer.MemoryCopy(sourcePtr, buffer + state.BufferedCount, loadSize, loadSize);
                        sourceIndex = loadSize;
                        ConsumeStripes(accumulators, ref state.StripesProcessedInCurrentBlock, NumStripesPerBlock, buffer, InternalBufferStripes, secret);
                        state.BufferedCount = 0;
                    }

                    // Large input to consume: ingest per full block.
                    if (source.Length - sourceIndex > NumStripesPerBlock * StripeLengthBytes)
                    {
                        ulong stripes = (ulong)(source.Length - sourceIndex - 1) / StripeLengthBytes;

                        // Join to current block's end.
                        ulong stripesToEnd = NumStripesPerBlock - state.StripesProcessedInCurrentBlock;
                        Accumulate(accumulators, sourcePtr + sourceIndex, secret + ((int)state.StripesProcessedInCurrentBlock * SecretConsumeRateBytes), (int)stripesToEnd);
                        ScrambleAccumulators(accumulators, secret + (SecretLengthBytes - StripeLengthBytes));
                        state.StripesProcessedInCurrentBlock = 0;
                        sourceIndex += (int)stripesToEnd * StripeLengthBytes;
                        stripes -= stripesToEnd;

                        // Consume entire blocks.
                        while (stripes >= NumStripesPerBlock)
                        {
                            Accumulate(accumulators, sourcePtr + sourceIndex, secret, NumStripesPerBlock);
                            ScrambleAccumulators(accumulators, secret + (SecretLengthBytes - StripeLengthBytes));
                            sourceIndex += NumStripesPerBlock * StripeLengthBytes;
                            stripes -= NumStripesPerBlock;
                        }

                        // Consume complete stripes in the last partial block.
                        Accumulate(accumulators, sourcePtr + sourceIndex, secret, (int)stripes);
                        sourceIndex += (int)stripes * StripeLengthBytes;
                        state.StripesProcessedInCurrentBlock = stripes;

                        // Copy the last stripe into the end of the buffer so it is available to GetCurrentHashCore when processing the "stripe from the end".
                        Buffer.MemoryCopy(sourcePtr + sourceIndex - StripeLengthBytes, buffer + InternalBufferLengthBytes - StripeLengthBytes, StripeLengthBytes, StripeLengthBytes);
                    }
                    else if (source.Length - sourceIndex > InternalBufferLengthBytes)
                    {
                        // Content to consume <= block size. Consume source by a multiple of internal buffer size.
                        do
                        {
                            ConsumeStripes(accumulators, ref state.StripesProcessedInCurrentBlock, NumStripesPerBlock, sourcePtr + sourceIndex, InternalBufferStripes, secret);
                            sourceIndex += InternalBufferLengthBytes;
                        }
                        while (source.Length - sourceIndex > InternalBufferLengthBytes);

                        // Copy the last stripe into the end of the buffer so it is available to GetCurrentHashCore when processing the "stripe from the end".
                        Buffer.MemoryCopy(sourcePtr + sourceIndex - StripeLengthBytes, buffer + InternalBufferLengthBytes - StripeLengthBytes, StripeLengthBytes, StripeLengthBytes);
                    }

                    // Buffer the remaining input.
                    Buffer.MemoryCopy(sourcePtr + sourceIndex, buffer, source.Length - sourceIndex, source.Length - sourceIndex);
                    state.BufferedCount = (uint)(source.Length - sourceIndex);
                }
            }
        }

        /// <summary>
        /// This is a stronger avalanche, preferable when input has not been previously mixed
        /// </summary>
        internal static ulong Rrmxmx(ulong hash, uint length)
        {
            hash ^= BitOperations.RotateLeft(hash, 49) ^ BitOperations.RotateLeft(hash, 24);
            hash *= 0x9FB21C651E98DF25;
            hash ^= (hash >> 35) + length;
            hash *= 0x9FB21C651E98DF25;
            return XorShift(hash, 28);
        }

        internal static void HashInternalLoop(ulong* accumulators, byte* source, uint length, byte* secret)
        {
            const int StripesPerBlock = (SecretLengthBytes - StripeLengthBytes) / SecretConsumeRateBytes;
            const int BlockLen = StripeLengthBytes * StripesPerBlock;
            int blocksNum = (int)((length - 1) / BlockLen);

            Accumulate(accumulators, source, secret, StripesPerBlock, true, blocksNum);
            int offset = BlockLen * blocksNum;

            int stripesNumber = (int)((length - 1 - offset) / StripeLengthBytes);
            Accumulate(accumulators, source + offset, secret, stripesNumber);
            Accumulate512(accumulators, source + length - StripeLengthBytes, secret + (SecretLengthBytes - StripeLengthBytes - SecretLastAccStartBytes));
        }

        internal static void ConsumeStripes(ulong* accumulators, ref ulong stripesSoFar, ulong stripesPerBlock, byte* source, ulong stripes, byte* secret)
        {
            ulong stripesToEndOfBlock = stripesPerBlock - stripesSoFar;
            if (stripesToEndOfBlock <= stripes)
            {
                // need a scrambling operation
                ulong stripesAfterBlock = stripes - stripesToEndOfBlock;
                Accumulate(accumulators, source, secret + ((int)stripesSoFar * SecretConsumeRateBytes), (int)stripesToEndOfBlock);
                ScrambleAccumulators(accumulators, secret + (SecretLengthBytes - StripeLengthBytes));
                Accumulate(accumulators, source + ((int)stripesToEndOfBlock * StripeLengthBytes), secret, (int)stripesAfterBlock);
                stripesSoFar = stripesAfterBlock;
            }
            else
            {
                Accumulate(accumulators, source, secret + ((int)stripesSoFar * SecretConsumeRateBytes), (int)stripes);
                stripesSoFar += stripes;
            }
        }

        internal static void CopyAccumulators(ref State state, ulong* accumulators)
        {
            fixed (ulong* stateAccumulators = state.Accumulators)
            {
                {
                    for (int i = 0; i < 8; i++)
                    {
                        accumulators[i] = stateAccumulators[i];
                    }
                }
            }
        }

        internal static void DigestLong(ref State state, ulong* accumulators, byte* secret)
        {
            fixed (byte* buffer = state.Buffer)
            {
                byte* accumulateData;
                if (state.BufferedCount >= StripeLengthBytes)
                {
                    uint stripes = (state.BufferedCount - 1) / StripeLengthBytes;
                    ulong stripesSoFar = state.StripesProcessedInCurrentBlock;
                    ConsumeStripes(accumulators, ref stripesSoFar, NumStripesPerBlock, buffer, stripes, secret);
                    accumulateData = buffer + state.BufferedCount - StripeLengthBytes;
                }
                else
                {
                    byte* lastStripe = stackalloc byte[StripeLengthBytes];
                    int catchupSize = StripeLengthBytes - (int)state.BufferedCount;
                    Buffer.MemoryCopy(buffer + InternalBufferLengthBytes - catchupSize, lastStripe, StripeLengthBytes, catchupSize);
                    Buffer.MemoryCopy(buffer, lastStripe + catchupSize, (int)state.BufferedCount, (int)state.BufferedCount);
                    accumulateData = lastStripe;
                }

                Accumulate512(accumulators, accumulateData, secret + (SecretLengthBytes - StripeLengthBytes - SecretLastAccStartBytes));
            }
        }

        internal static void InitializeAccumulators(ulong* accumulators)
        {
            accumulators[0] = Prime32_3;
            accumulators[1] = Prime64_1;
            accumulators[2] = Prime64_2;
            accumulators[3] = Prime64_3;
            accumulators[4] = Prime64_4;
            accumulators[5] = Prime32_2;
            accumulators[6] = Prime64_5;
            accumulators[7] = Prime32_1;
        }

        internal static ulong MergeAccumulators(ulong* accumulators, byte* secret, ulong start)
        {
            ulong result64 = start;

            result64 += Multiply64To128ThenFold(accumulators[0] ^ ReadUInt64LE(secret), accumulators[1] ^ ReadUInt64LE(secret + 8));
            result64 += Multiply64To128ThenFold(accumulators[2] ^ ReadUInt64LE(secret + 16), accumulators[3] ^ ReadUInt64LE(secret + 24));
            result64 += Multiply64To128ThenFold(accumulators[4] ^ ReadUInt64LE(secret + 32), accumulators[5] ^ ReadUInt64LE(secret + 40));
            result64 += Multiply64To128ThenFold(accumulators[6] ^ ReadUInt64LE(secret + 48), accumulators[7] ^ ReadUInt64LE(secret + 56));

            return Avalanche(result64);
        }

        internal static ulong Mix16Bytes(byte* source, ulong secretLow, ulong secretHigh, ulong seed)
        {
            return Multiply64To128ThenFold(ReadUInt64LE(source) ^ (secretLow + seed), ReadUInt64LE(source + sizeof(ulong)) ^ (secretHigh - seed));
        }

        /// <summary>
        /// Calculates a 32-bit to 64-bit long multiply
        /// </summary>
        internal static ulong Multiply32To64(uint v1, uint v2)
        {
            return (ulong)v1 * v2;
        }

        /// <summary>
        /// This is a fast avalanche stage, suitable when input bits are already partially mixed
        /// </summary>
        internal static ulong Avalanche(ulong hash)
        {
            hash = XorShift(hash, 37);
            hash *= 0x165667919E3779F9;
            hash = XorShift(hash, 32);
            return hash;
        }

        internal static ulong Multiply64To128(ulong left, ulong right, out ulong lower)
        {
            ulong lowerLow = Multiply32To64((uint)left, (uint)right);
            ulong higherLow = Multiply32To64((uint)(left >> 32), (uint)right);
            ulong lowerHigh = Multiply32To64((uint)left, (uint)(right >> 32));
            ulong higherHigh = Multiply32To64((uint)(left >> 32), (uint)(right >> 32));

            ulong cross = (lowerLow >> 32) + (higherLow & 0xFFFFFFFF) + lowerHigh;
            ulong upper = (higherLow >> 32) + (cross >> 32) + higherHigh;
            lower = (cross << 32) | (lowerLow & 0xFFFFFFFF);
            return upper;
        }

        /// <summary>
        /// 计算一个 64 位到 128 位的乘法，然后进行 XOR 折叠
        /// </summary>
        internal static ulong Multiply64To128ThenFold(ulong left, ulong right)
        {
            ulong upper = Multiply64To128(left, right, out ulong lower);
            return lower ^ upper;
        }

        internal static void DeriveSecretFromSeed(byte* destinationSecret, ulong seed)
        {
            fixed (byte* defaultSecret = DefaultSecret)
            {
                for (int i = 0; i < SecretLengthBytes; i += sizeof(ulong) * 2)
                {
                    WriteUInt64LE(destinationSecret + i, ReadUInt64LE(defaultSecret + i) + seed);
                    WriteUInt64LE(destinationSecret + i + 8, ReadUInt64LE(defaultSecret + i + 8) - seed);
                }
            }
        }

        /// <summary>
        /// 循环遍历 Accumulate512 的优化版本
        /// </summary>
        private static void Accumulate(ulong* accumulators, byte* source, byte* secret, int stripesToProcess, bool scramble = false, int blockCount = 1)
        {
            byte* secretForScramble = secret + (SecretLengthBytes - StripeLengthBytes);

            for (int j = 0; j < blockCount; j++)
            {
                for (int i = 0; i < stripesToProcess; i++)
                {
                    Accumulate512Inlined(accumulators, source, secret + (i * SecretConsumeRateBytes));
                    source += StripeLengthBytes;
                }

                if (scramble)
                {
                    ScrambleAccumulators(accumulators, secretForScramble);
                }
            }
        }

        internal static void Accumulate512(ulong* accumulators, byte* source, byte* secret)
        {
            Accumulate512Inlined(accumulators, source, secret);
        }

        private static void Accumulate512Inlined(ulong* accumulators, byte* source, byte* secret)
        {
            for (int i = 0; i < AccumulatorCount; i++)
            {
                ulong sourceVal = ReadUInt64LE(source + (8 * i));
                ulong sourceKey = sourceVal ^ ReadUInt64LE(secret + (i * 8));

                accumulators[i ^ 1] += sourceVal; // swap adjacent lanes
                accumulators[i] += Multiply32To64((uint)sourceKey, (uint)(sourceKey >> 32));
            }
        }

        private static void ScrambleAccumulators(ulong* accumulators, byte* secret)
        {
            for (int i = 0; i < AccumulatorCount; i++)
            {
                ulong xorShift = XorShift(*accumulators, 47);
                ulong xorWithKey = xorShift ^ ReadUInt64LE(secret);
                *accumulators = xorWithKey * Prime32_1;

                accumulators++;
                secret += sizeof(ulong);
            }
        }

        internal static ulong XorShift(ulong value, int shift)
        {
            return value ^ (value >> shift);
        }

        internal static uint ReadUInt32LE(byte* data)
        {
            return BitConverter.IsLittleEndian ? ReadUnaligned<uint>(data) : BinaryPrimitives.ReverseEndianness(ReadUnaligned<uint>(data));
        }

        internal static ulong ReadUInt64LE(byte* data)
        {
            return BitConverter.IsLittleEndian ? ReadUnaligned<ulong>(data) : BinaryPrimitives.ReverseEndianness(ReadUnaligned<ulong>(data));
        }

        private static void WriteUInt64LE(byte* data, ulong value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            WriteUnaligned(data, value);
        }

        private static T ReadUnaligned<T>(void* source) where T : unmanaged
        {
            T t;
            Buffer.MemoryCopy(source, &t, sizeof(T), sizeof(T));
            return t;
        }

        private static void WriteUnaligned<T>(void* destination, T value) where T : unmanaged
        {
            Buffer.MemoryCopy(&value, destination, sizeof(T), sizeof(T));
        }

        /// <summary>
        /// 计算提供的源数据的 XXH128 哈希
        /// </summary>
        /// <param name="source">要哈希的数据</param>
        /// <returns>所提供数据的 XXH128 128 位哈希码</returns>
        internal static byte[] Hash(byte[] source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Hash(source, seed: 0);
        }

        /// <summary>
        /// 使用提供的种子计算所提供数据的 XXH128 哈希值
        /// </summary>
        /// <param name="source">要哈希的数据</param>
        /// <param name="seed">The seed value for this hash computation</param>
        /// <returns>所提供数据的 XXH128 128 位哈希码</returns>
        internal static byte[] Hash(byte[] source, long seed)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            byte[] result = new byte[HashLengthInBytes];
            Hash(source, result, seed);
            return result;
        }

        /// <summary>
        /// 使用可选提供的种子，将提供的源数据的 XXH128 哈希计算到指定的目标中
        /// </summary>
        /// <param name="source">要哈希的数据</param>
        /// <param name="destination">接收计算得出的128位哈希码的缓冲区</param>
        /// <param name="seed">此哈希计算的种子值。默认值为零</param>
        /// <returns>写入目标的字节数</returns>
        internal static int Hash(byte[] source, byte[] destination, long seed = 0)
        {
            if (source is null || destination is null)
            {
                return default;
            }

            if (!TryHash(source, destination, out int bytesWritten, seed))
            {
                ThrowDestinationTooShort();
            }

            return bytesWritten;
        }

        /// <summary>
        /// 尝试使用可选提供的种子，将提供的源数据计算为指定目标的 XXH128 哈希
        /// </summary>
        /// <param name="source">要哈希的数据</param>
        /// <param name="destination">接收计算得出的128位哈希码的缓冲区</param>
        /// <param name="bytesWritten">当此方法返回时，包含写入目标的字节数</param>
        /// <param name="seed">此哈希计算的种子值。默认值为零</param>
        /// <returns><see langword="true"/> if destination is long enough to receive the computed hash value (16 bytes); otherwise, <see langword="false"/>.</returns>
        internal static bool TryHash(byte[] source, byte[] destination, out int bytesWritten, long seed = 0)
        {
            if (source is null || destination is null)
            {
                bytesWritten = default;
                return default;
            }

            if (destination.Length >= sizeof(ulong) * 2)
            {
                Hash128 hash = HashToHash128(source, seed);
                WriteBigEndian128(hash, destination);
                bytesWritten = HashLengthInBytes;
                return true;
            }

            bytesWritten = 0;
            return false;
        }

        private static Hash128 HashToHash128(byte[] source, long seed = 0)
        {
            if (source is null)
            {
                return default;
            }

            uint length = (uint)source.Length;
            fixed (byte* sourcePtr = source)
            {
                return HashToHash128(sourcePtr, length, seed);
            }
        }

        private static Hash128 HashToHash128(byte* sourcePtr, uint length, long seed = 0)
        {
            if (length <= 16)
            {
                return HashLength0To16(sourcePtr, length, (ulong)seed);
            }

            if (length <= 128)
            {
                return HashLength17To128(sourcePtr, length, (ulong)seed);
            }

            if (length <= MidSizeMaxBytes)
            {
                return HashLength129To240(sourcePtr, length, (ulong)seed);
            }

            return HashLengthOver240(sourcePtr, length, (ulong)seed);
        }

        internal void Reset()
        {
            Reset(ref _state);
        }

        /// <summary>
        /// 将源的内容追加到当前哈希计算已处理的数据中
        /// </summary>
        internal void Append(Stream stream, int bufferSize = 1024 * 64)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream must be readable.", nameof(stream));
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            byte[] buffer = new byte[bufferSize];

            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (bytesRead == buffer.Length)
                {
                    Append(buffer);
                }
                else
                {
                    byte[] partial = new byte[bytesRead];
                    Buffer.BlockCopy(buffer, 0, partial, 0, bytesRead);
                    Append(partial);
                }
            }
        }

        /// <summary>
        /// 将源的内容追加到当前哈希计算已处理的数据中
        /// </summary>
        /// <param name="source">要处理的数据</param>
        internal void Append(byte[] source)
        {
            Append(ref _state, source);
        }

        internal byte[] GetCurrentHash()
        {
            byte[] ret = new byte[HashLengthInBytes];
            GetCurrentHashCore(ret);
            return ret;
        }

        private void GetCurrentHashCore(byte[] destination)
        {
            Hash128 current = GetCurrentHashAsHash128();
            WriteBigEndian128(current, destination);
        }

        private Hash128 GetCurrentHashAsHash128()
        {
            Hash128 current;

            if (_state.TotalLength > MidSizeMaxBytes)
            {
                // Digest on a local copy to ensure the accumulators remain unaltered.
                ulong* accumulators = stackalloc ulong[AccumulatorCount];
                CopyAccumulators(ref _state, accumulators);

                fixed (byte* secret = _state.Secret)
                {
                    DigestLong(ref _state, accumulators, secret);
                    current = new(low64: MergeAccumulators(accumulators, secret + SecretMergeAccsStartBytes, _state.TotalLength * Prime64_1), high64: MergeAccumulators(accumulators, secret + SecretLengthBytes - (AccumulatorCount * sizeof(ulong)) - SecretMergeAccsStartBytes, ~(_state.TotalLength * Prime64_2)));
                }
            }
            else
            {
                fixed (byte* buffer = _state.Buffer)
                {
                    current = HashToHash128(buffer, (uint)(int)_state.TotalLength, (long)_state.Seed);
                }
            }

            return current;
        }

        private static void WriteBigEndian128(in Hash128 hash, byte[] destination)
        {
            if (destination is null)
            {
                return;
            }

            ulong low = hash.Low64;
            ulong high = hash.High64;
            if (BitConverter.IsLittleEndian)
            {
                low = BinaryPrimitives.ReverseEndianness(low);
                high = BinaryPrimitives.ReverseEndianness(high);
            }

            fixed (byte* dest0 = destination)
            {
                *(ulong*)(void*)dest0 = high;
                *((ulong*)(void*)dest0 + 1) = low;
            }
        }

        private static Hash128 HashLength0To16(byte* source, uint length, ulong seed)
        {
            if (length > 8)
            {
                return HashLength9To16(source, length, seed);
            }

            if (length >= 4)
            {
                return HashLength4To8(source, length, seed);
            }

            if (length is not 0)
            {
                return HashLength1To3(source, length, seed);
            }

            const ulong BitFlipL = DefaultSecretUInt64_8 ^ DefaultSecretUInt64_9;
            const ulong BitFlipH = DefaultSecretUInt64_10 ^ DefaultSecretUInt64_11;
            return new(XxHash64Avalanche(seed ^ BitFlipL), XxHash64Avalanche(seed ^ BitFlipH));
        }

        private static ulong XxHash64Avalanche(ulong hash)
        {
            hash ^= hash >> 33;
            hash *= Prime64_2;
            hash ^= hash >> 29;
            hash *= Prime64_3;
            hash ^= hash >> 32;
            return hash;
        }

        private static Hash128 HashLength1To3(byte* source, uint length, ulong seed)
        {
            // When source.Length == 1, c1 == source[0], c2 == source[0], c3 == source[0]
            // When source.Length == 2, c1 == source[0], c2 == source[1], c3 == source[1]
            // When source.Length == 3, c1 == source[0], c2 == source[1], c3 == source[2]
            byte c1 = *source;
            byte c2 = source[length >> 1];
            byte c3 = source[length - 1];

            uint combinedl = ((uint)c1 << 16) | ((uint)c2 << 24) | c3 | (length << 8);
            uint combinedh = BitOperations.RotateLeft(BinaryPrimitives.ReverseEndianness(combinedl), 13);
            const uint SecretXorL = unchecked((uint)DefaultSecretUInt64_0) ^ (uint)(DefaultSecretUInt64_0 >> 32);
            const uint SecretXorH = unchecked((uint)DefaultSecretUInt64_1) ^ (uint)(DefaultSecretUInt64_1 >> 32);
            ulong bitflipl = SecretXorL + seed;
            ulong bitfliph = SecretXorH - seed;
            ulong keyedLo = combinedl ^ bitflipl;
            ulong keyedHi = combinedh ^ bitfliph;

            return new(XxHash64Avalanche(keyedLo), XxHash64Avalanche(keyedHi));
        }

        private static Hash128 HashLength4To8(byte* source, uint length, ulong seed)
        {
            seed ^= (ulong)BinaryPrimitives.ReverseEndianness((uint)seed) << 32;

            uint inputLo = ReadUInt32LE(source);
            uint inputHi = ReadUInt32LE(source + length - 4);
            ulong input64 = inputLo + ((ulong)inputHi << 32);
            ulong bitflip = (DefaultSecretUInt64_2 ^ DefaultSecretUInt64_3) + seed;
            ulong keyed = input64 ^ bitflip;
            ulong m128High = Multiply64To128(keyed, Prime64_1 + (length << 2), out ulong m128Low);

            m128High += m128Low << 1;
            m128Low ^= m128High >> 3;

            m128Low = XorShift(m128Low, 35);
            m128Low *= 0x9FB21C651E98DF25UL;
            m128Low = XorShift(m128Low, 28);
            m128High = Avalanche(m128High);

            return new(m128Low, m128High);
        }

        private static Hash128 HashLength9To16(byte* source, uint length, ulong seed)
        {
            ulong bitflipl = (DefaultSecretUInt64_4 ^ DefaultSecretUInt64_5) - seed;
            ulong bitfliph = (DefaultSecretUInt64_6 ^ DefaultSecretUInt64_7) + seed;
            ulong inputLo = ReadUInt64LE(source);
            ulong inputHi = ReadUInt64LE(source + length - 8);
            ulong m128High = Multiply64To128(inputLo ^ inputHi ^ bitflipl, Prime64_1, out ulong m128Low);

            m128Low += (ulong)(length - 1) << 54;
            inputHi ^= bitfliph;

            m128High += sizeof(void*) < sizeof(ulong) ? (inputHi & 0xFFFFFFFF00000000UL) + Multiply32To64((uint)inputHi, Prime32_2) : inputHi + Multiply32To64((uint)inputHi, Prime32_2 - 1);
            m128Low ^= BinaryPrimitives.ReverseEndianness(m128High);

            ulong h128High = Multiply64To128(m128Low, Prime64_2, out ulong h128Low);
            h128High += m128High * Prime64_2;

            h128Low = Avalanche(h128Low);
            h128High = Avalanche(h128High);
            return new(h128Low, h128High);
        }

        private static Hash128 HashLength17To128(byte* source, uint length, ulong seed)
        {
            ulong accLow = length * Prime64_1;
            ulong accHigh = 0;

            switch ((length - 1) / 32)
            {
                default: // case 3
                    {
                        Mix32Bytes(ref accLow, ref accHigh, source + 48, source + length - 64, DefaultSecretUInt64_12, DefaultSecretUInt64_13, DefaultSecretUInt64_14, DefaultSecretUInt64_15, seed);
                        goto case 2;
                    }
                case 2:
                    {
                        Mix32Bytes(ref accLow, ref accHigh, source + 32, source + length - 48, DefaultSecretUInt64_8, DefaultSecretUInt64_9, DefaultSecretUInt64_10, DefaultSecretUInt64_11, seed);
                        goto case 1;
                    }
                case 1:
                    {
                        Mix32Bytes(ref accLow, ref accHigh, source + 16, source + length - 32, DefaultSecretUInt64_4, DefaultSecretUInt64_5, DefaultSecretUInt64_6, DefaultSecretUInt64_7, seed);
                        goto case 0;
                    }
                case 0:
                    {
                        Mix32Bytes(ref accLow, ref accHigh, source, source + length - 16, DefaultSecretUInt64_0, DefaultSecretUInt64_1, DefaultSecretUInt64_2, DefaultSecretUInt64_3, seed);
                        break;
                    }
            }

            return AvalancheHash(accLow, accHigh, length, seed);
        }

        private static Hash128 HashLength129To240(byte* source, uint length, ulong seed)
        {
            ulong accLow = length * Prime64_1;
            ulong accHigh = 0;

            Mix32Bytes(ref accLow, ref accHigh, source + (32 * 0), source + (32 * 0) + 16, DefaultSecretUInt64_0, DefaultSecretUInt64_1, DefaultSecretUInt64_2, DefaultSecretUInt64_3, seed);
            Mix32Bytes(ref accLow, ref accHigh, source + (32 * 1), source + (32 * 1) + 16, DefaultSecretUInt64_4, DefaultSecretUInt64_5, DefaultSecretUInt64_6, DefaultSecretUInt64_7, seed);
            Mix32Bytes(ref accLow, ref accHigh, source + (32 * 2), source + (32 * 2) + 16, DefaultSecretUInt64_8, DefaultSecretUInt64_9, DefaultSecretUInt64_10, DefaultSecretUInt64_11, seed);
            Mix32Bytes(ref accLow, ref accHigh, source + (32 * 3), source + (32 * 3) + 16, DefaultSecretUInt64_12, DefaultSecretUInt64_13, DefaultSecretUInt64_14, DefaultSecretUInt64_15, seed);

            accLow = Avalanche(accLow);
            accHigh = Avalanche(accHigh);

            uint bound = (length - (32 * 4)) / 32;
            if (bound is not 0)
            {
                Mix32Bytes(ref accLow, ref accHigh, source + (32 * 4), source + (32 * 4) + 16, DefaultSecret3UInt64_0, DefaultSecret3UInt64_1, DefaultSecret3UInt64_2, DefaultSecret3UInt64_3, seed);
                if (bound >= 2)
                {
                    Mix32Bytes(ref accLow, ref accHigh, source + (32 * 5), source + (32 * 5) + 16, DefaultSecret3UInt64_4, DefaultSecret3UInt64_5, DefaultSecret3UInt64_6, DefaultSecret3UInt64_7, seed);
                    if (bound is 3)
                    {
                        Mix32Bytes(ref accLow, ref accHigh, source + (32 * 6), source + (32 * 6) + 16, DefaultSecret3UInt64_8, DefaultSecret3UInt64_9, DefaultSecret3UInt64_10, DefaultSecret3UInt64_11, seed);
                    }
                }
            }

            Mix32Bytes(ref accLow, ref accHigh, source + length - 16, source + length - 32, 0x4F0BC7C7BBDCF93F, 0x59B4CD4BE0518A1D, 0x7378D9C97E9FC831, 0xEBD33483ACC5EA64, 0 - seed);
            return AvalancheHash(accLow, accHigh, length, seed);
        }

        private static Hash128 HashLengthOver240(byte* source, uint length, ulong seed)
        {
            fixed (byte* defaultSecret = DefaultSecret)
            {
                byte* secret = defaultSecret;
                if (seed is not 0)
                {
                    byte* customSecret = stackalloc byte[SecretLengthBytes];
                    DeriveSecretFromSeed(customSecret, seed);
                    secret = customSecret;
                }

                ulong* accumulators = stackalloc ulong[AccumulatorCount];
                InitializeAccumulators(accumulators);
                HashInternalLoop(accumulators, source, length, secret);

                return new(low64: MergeAccumulators(accumulators, secret + SecretMergeAccsStartBytes, length * Prime64_1), high64: MergeAccumulators(accumulators, secret + SecretLengthBytes - (AccumulatorCount * sizeof(ulong)) - SecretMergeAccsStartBytes, ~(length * Prime64_2)));
            }
        }

        private static Hash128 AvalancheHash(ulong accLow, ulong accHigh, uint length, ulong seed)
        {
            ulong h128Low = accLow + accHigh;
            ulong h128High = (accLow * Prime64_1) + (accHigh * Prime64_4) + ((length - seed) * Prime64_2);
            h128Low = Avalanche(h128Low);
            h128High = 0ul - Avalanche(h128High);
            return new(h128Low, h128High);
        }

        private static void Mix32Bytes(ref ulong accLow, ref ulong accHigh, byte* input1, byte* input2, ulong secret1, ulong secret2, ulong secret3, ulong secret4, ulong seed)
        {
            accLow += Mix16Bytes(input1, secret1, secret2, seed);
            accLow ^= ReadUInt64LE(input2) + ReadUInt64LE(input2 + 8);
            accHigh += Mix16Bytes(input2, secret3, secret4, seed);
            accHigh ^= ReadUInt64LE(input1) + ReadUInt64LE(input1 + 8);
        }

        private static void ThrowDestinationTooShort()
        {
            throw new ArgumentException("Destination is too short", "destination");
        }

        [StructLayout(LayoutKind.Auto)]
        internal struct State
        {
            /// <summary>
            /// The accumulators. Length is AccumulatorCount
            /// </summary>
            internal fixed ulong Accumulators[AccumulatorCount];

            /// <summary>
            /// Used to store a custom secret generated from a seed. Length is SecretLengthBytes
            /// </summary>
            internal fixed byte Secret[SecretLengthBytes];

            /// <summary>
            /// The internal buffer. Length is InternalBufferLengthBytes
            /// </summary>
            internal fixed byte Buffer[InternalBufferLengthBytes];

            /// <summary>
            /// The amount of memory in Buffer
            /// </summary>
            internal uint BufferedCount;

            /// <summary>
            /// Number of stripes processed in the current block
            /// </summary>
            internal ulong StripesProcessedInCurrentBlock;

            /// <summary>
            /// Total length hashed
            /// </summary>
            internal ulong TotalLength;

            /// <summary>
            /// The seed employed (possibly 0)
            /// </summary>
            internal ulong Seed;
        }
    }
}
