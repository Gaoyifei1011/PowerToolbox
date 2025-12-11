using System;
using static PowerToolbox.Extensions.Hashing.XxHashShared;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// XXH128 校验实现
    /// </summary>
    public sealed unsafe class XxHash128
    {
        /// <summary>
        /// XXH128 生成 16 字节的哈希
        /// </summary>
        private const int HashLengthInBytes = 16;

        private State _state;

        /// <summary>
        /// 使用默认种子值 0 初始化 XxHash128 类的新实例
        /// </summary>
        public XxHash128() : this(0)
        {
        }

        /// <summary>
        /// 使用指定的种子初始化 XxHash128 类的新实例
        /// </summary>
        public XxHash128(long seed)
        {
            Initialize(ref _state, (ulong)seed);
        }

        /// <summary>
        /// 使用另一个实例的状态初始化 XxHash128 类的新实例
        /// </summary>
        private XxHash128(State state)
        {
            _state = state;
        }

        /// <summary>
        /// 计算提供的源数据的 XXH128 哈希
        /// </summary>
        /// <param name="source">要哈希的数据</param>
        /// <returns>所提供数据的 XXH128 128 位哈希码</returns>
        public static byte[] Hash(byte[] source)
        {
            return Hash(source, seed: 0);
        }

        /// <summary>
        /// 使用提供的种子计算所提供数据的 XXH128 哈希值
        /// </summary>
        /// <param name="source">要哈希的数据</param>
        /// <param name="seed">The seed value for this hash computation</param>
        /// <returns>所提供数据的 XXH128 128 位哈希码</returns>
        public static byte[] Hash(byte[] source, long seed)
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
        public static int Hash(byte[] source, byte[] destination, long seed = 0)
        {
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
        public static bool TryHash(byte[] source, byte[] destination, out int bytesWritten, long seed = 0)
        {
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

        public void Reset()
        {
            XxHashShared.Reset(ref _state);
        }

        /// <summary>
        /// 将源的内容追加到当前哈希计算已处理的数据中
        /// </summary>
        /// <param name="source">要处理的数据</param>
        public void Append(byte[] source)
        {
            XxHashShared.Append(ref _state, source);
        }

        public byte[] GetCurrentHash()
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
                    current = new Hash128(
                        low64: MergeAccumulators(accumulators, secret + SecretMergeAccsStartBytes, _state.TotalLength * Prime64_1),
                        high64: MergeAccumulators(accumulators, secret + SecretLengthBytes - (AccumulatorCount * sizeof(ulong)) - SecretMergeAccsStartBytes, ~(_state.TotalLength * Prime64_2)));
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
            return new Hash128(XxHash64Avalanche(seed ^ BitFlipL), XxHash64Avalanche(seed ^ BitFlipH));
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

            return new Hash128(XxHash64Avalanche(keyedLo), XxHash64Avalanche(keyedHi));
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

            return new Hash128(m128Low, m128High);
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
            return new Hash128(h128Low, h128High);
        }

        private static Hash128 HashLength17To128(byte* source, uint length, ulong seed)
        {
            ulong accLow = length * Prime64_1;
            ulong accHigh = 0;

            switch ((length - 1) / 32)
            {
                default: // case 3
                    Mix32Bytes(ref accLow, ref accHigh, source + 48, source + length - 64, DefaultSecretUInt64_12, DefaultSecretUInt64_13, DefaultSecretUInt64_14, DefaultSecretUInt64_15, seed);
                    goto case 2;
                case 2:
                    Mix32Bytes(ref accLow, ref accHigh, source + 32, source + length - 48, DefaultSecretUInt64_8, DefaultSecretUInt64_9, DefaultSecretUInt64_10, DefaultSecretUInt64_11, seed);
                    goto case 1;
                case 1:
                    Mix32Bytes(ref accLow, ref accHigh, source + 16, source + length - 32, DefaultSecretUInt64_4, DefaultSecretUInt64_5, DefaultSecretUInt64_6, DefaultSecretUInt64_7, seed);
                    goto case 0;
                case 0:
                    Mix32Bytes(ref accLow, ref accHigh, source, source + length - 16, DefaultSecretUInt64_0, DefaultSecretUInt64_1, DefaultSecretUInt64_2, DefaultSecretUInt64_3, seed);
                    break;
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

                return new Hash128(
                    low64: MergeAccumulators(accumulators, secret + SecretMergeAccsStartBytes, length * Prime64_1),
                    high64: MergeAccumulators(accumulators, secret + SecretLengthBytes - (AccumulatorCount * sizeof(ulong)) - SecretMergeAccsStartBytes, ~(length * Prime64_2)));
            }
        }

        private static Hash128 AvalancheHash(ulong accLow, ulong accHigh, uint length, ulong seed)
        {
            ulong h128Low = accLow + accHigh;
            ulong h128High = (accLow * Prime64_1) + (accHigh * Prime64_4) + ((length - seed) * Prime64_2);
            h128Low = Avalanche(h128Low);
            h128High = 0ul - Avalanche(h128High);
            return new Hash128(h128Low, h128High);
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

        private readonly struct Hash128(ulong low64, ulong high64)
        {
            public readonly ulong Low64 = low64;
            public readonly ulong High64 = high64;
        }
    }
}
