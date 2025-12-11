using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// XXH32 校验实现
    /// </summary>
    public class XxHash32 : HashAlgorithm
    {
        private const uint PRIME32_1 = 2654435761U;
        private const uint PRIME32_2 = 2246822519U;
        private const uint PRIME32_3 = 3266489917U;
        private const uint PRIME32_4 = 668265263U;
        private const uint PRIME32_5 = 374761393U;

        private static readonly Func<byte[], int, uint> FuncGetLittleEndianUInt32;
        private static readonly Func<uint, uint> FuncGetFinalHashUInt32;

        private uint _Seed32;

        private uint _ACC32_1;
        private uint _ACC32_2;
        private uint _ACC32_3;
        private uint _ACC32_4;

        private uint _Hash32;

        private int _RemainingLength;
        private long _TotalLength = 0;
        private int _CurrentIndex;
        private byte[] _CurrentArray;

        static XxHash32()
        {
            if (BitConverter.IsLittleEndian)
            {
                FuncGetLittleEndianUInt32 = new Func<byte[], int, uint>((x, i) =>
                {
                    unsafe
                    {
                        fixed (byte* array = x)
                        {
                            return *(uint*)(array + i);
                        }
                    }
                });
                FuncGetFinalHashUInt32 = new Func<uint, uint>(i => (i & 0x000000FFU) << 24 | (i & 0x0000FF00U) << 8 | (i & 0x00FF0000U) >> 8 | (i & 0xFF000000U) >> 24);
            }
            else
            {
                FuncGetLittleEndianUInt32 = new Func<byte[], int, uint>((x, i) =>
                {
                    unsafe
                    {
                        fixed (byte* array = x)
                        {
                            return (uint)(array[i++] | (array[i++] << 8) | (array[i++] << 16) | (array[i] << 24));
                        }
                    }
                });
                FuncGetFinalHashUInt32 = new Func<uint, uint>(i => i);
            }
        }

        /// <summary>
        /// 使用默认种子 0 初始化 XxHash32 类的新实例
        /// </summary>
        public XxHash32()
        {
            Initialize(0);
        }

        /// <summary>
        /// 初始化 XxHash32 类的新实例，并将种子设置为指定的值
        /// </summary>
        /// <param name="seed">表示用于 XxHash32 计算的种子</param>
        public XxHash32(uint seed)
        {
            Initialize(seed);
        }

        /// <summary>
        /// 获取计算后的哈希码的值
        /// </summary>
        public uint HashUInt32
        {
            get { return State is 0 ? _Hash32 : throw new InvalidOperationException("Hash computation has not yet completed."); }
        }

        /// <summary>
        /// 获取或设置 xxHash32 算法使用的种子值
        /// </summary>
        public uint Seed
        {
            get { return _Seed32; }

            set
            {
                if (value != _Seed32)
                {
                    if (State is not 0)
                    {
                        throw new InvalidOperationException("Hash computation has not yet completed.");
                    }

                    _Seed32 = value;
                    Initialize();
                }
            }
        }

        /// <summary>
        /// 初始化此实例以进行新的哈希计算
        /// </summary>
        public override void Initialize()
        {
            _ACC32_1 = _Seed32 + PRIME32_1 + PRIME32_2;
            _ACC32_2 = _Seed32 + PRIME32_2;
            _ACC32_3 = _Seed32 + 0;
            _ACC32_4 = _Seed32 - PRIME32_1;
        }

        /// <summary>
        /// 将写入对象的数据路由到哈希算法以计算哈希值
        /// </summary>
        /// <param name="array">用于计算哈希码的输入</param>
        /// <param name="ibStart">开始使用数据的字节数组偏移量</param>
        /// <param name="cbSize">字节数组中用作数据的字节数</param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (State is not 1)
            {
                State = 1;
            }

            int size = cbSize - ibStart;
            _RemainingLength = size & 15;
            if (cbSize >= 16)
            {
                int limit = size - _RemainingLength;
                do
                {
                    _ACC32_1 = Round32(_ACC32_1, FuncGetLittleEndianUInt32(array, ibStart));
                    ibStart += 4;
                    _ACC32_2 = Round32(_ACC32_2, FuncGetLittleEndianUInt32(array, ibStart));
                    ibStart += 4;
                    _ACC32_3 = Round32(_ACC32_3, FuncGetLittleEndianUInt32(array, ibStart));
                    ibStart += 4;
                    _ACC32_4 = Round32(_ACC32_4, FuncGetLittleEndianUInt32(array, ibStart));
                    ibStart += 4;
                } while (ibStart < limit);
            }
            _TotalLength += cbSize;

            if (_RemainingLength != 0)
            {
                _CurrentArray = array;
                _CurrentIndex = ibStart;
            }
        }

        /// <summary>
        /// 调用此方法将数据输入哈希
        /// </summary>
        public void FeedInput(byte[] array, int start, int count)
        {
            HashCore(array, start, count);
        }

        /// <summary>
        /// 一旦使用 FeedInput 提供了所有输入，就调用此方法以获取最终计算的哈希值
        /// </summary>
        public void ComputeResult(out uint result)
        {
            _Hash32 = _TotalLength >= 16 ? RotateLeft32(_ACC32_1, 1) + RotateLeft32(_ACC32_2, 7) + RotateLeft32(_ACC32_3, 12) + RotateLeft32(_ACC32_4, 18) : _Seed32 + PRIME32_5;
            _Hash32 += (uint)_TotalLength;

            while (_RemainingLength >= 4)
            {
                _Hash32 = RotateLeft32(_Hash32 + FuncGetLittleEndianUInt32(_CurrentArray, _CurrentIndex) * PRIME32_3, 17) * PRIME32_4;
                _CurrentIndex += 4;
                _RemainingLength -= 4;
            }
            unsafe
            {
                fixed (byte* arrayPtr = _CurrentArray)
                {
                    while (_RemainingLength-- >= 1)
                    {
                        _Hash32 = RotateLeft32(_Hash32 + arrayPtr[_CurrentIndex++] * PRIME32_5, 11) * PRIME32_1;
                    }
                }
            }
            _Hash32 = (_Hash32 ^ (_Hash32 >> 15)) * PRIME32_2;
            _Hash32 = (_Hash32 ^ (_Hash32 >> 13)) * PRIME32_3;
            _Hash32 ^= _Hash32 >> 16;

            _TotalLength = State = 0;
            result = FuncGetFinalHashUInt32(_Hash32);
        }

        /// <summary>
        /// 在加密流对象处理完最后的数据后，完成哈希计算
        /// </summary>
        /// <returns>计算得出的哈希码</returns>
        protected override byte[] HashFinal()
        {
            ComputeResult(out uint final);
            return BitConverter.GetBytes(final);
        }

        private static uint Round32(uint input, uint value)
        {
            return RotateLeft32(input + (value * PRIME32_2), 13) * PRIME32_1;
        }

        private static uint RotateLeft32(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }

        private void Initialize(uint seed)
        {
            HashSizeValue = 32;
            _Seed32 = seed;
            Initialize();
        }
    }
}
