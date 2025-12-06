using System;
using System.Linq;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// ChaCha20 的 ICryptoTransform 实现
    /// </summary>
    public sealed class ChaCha20Transform : ICryptoTransform
    {
        private readonly uint[] _state = new uint[16];
        private readonly byte[] _keyStreamBlock = new byte[64];
        private int _keyStreamPosition = 64; // 初始化为块大小，表示需要生成新密钥流

        public bool CanReuseTransform => false;

        public bool CanTransformMultipleBlocks => true;

        public int InputBlockSize => 1;

        public int OutputBlockSize => 1;

        public ChaCha20Transform(byte[] key, byte[] iv)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.Length is not 32)
            {
                throw new ArgumentException("密钥长度必须为32字节（256位）", nameof(key));
            }

            if (iv is null)
            {
                throw new ArgumentNullException(nameof(iv));
            }

            if (iv.Length is not 12 and not 16)
            {
                throw new ArgumentException("IV长度必须为12字节（96位）或16字节", nameof(iv));
            }

            // 初始化ChaCha20状态
            // 常量 "expand 32-byte k" 的ASCII表示
            _state[0] = 0x61707865;  // "expa"
            _state[1] = 0x3320646e;  // "nd 3"
            _state[2] = 0x79622d32;  // "2-by"
            _state[3] = 0x6b206574;  // "te k"

            // 密钥（256位）
            for (int i = 0; i < 8; i++)
            {
                _state[4 + i] = BitConverter.ToUInt32(key, i * 4);
            }

            // 计数器
            _state[12] = 0;
            _state[13] = 0;

            // 初始向量（IV）
            byte[] effectiveIV = iv.Length is 16 ? new ArraySegment<byte>(iv, 4, 12).ToArray() : iv;
            _state[14] = BitConverter.ToUInt32(effectiveIV, 0);
            _state[15] = BitConverter.ToUInt32(effectiveIV, 4);
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount,
                                 byte[] outputBuffer, int outputOffset)
        {
            if (inputBuffer is null)
            {
                throw new ArgumentNullException(nameof(inputBuffer));
            }

            if (outputBuffer is null)
            {
                throw new ArgumentNullException(nameof(outputBuffer));
            }

            if (inputOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(inputOffset));
            }

            if (inputCount <= 0 || inputCount > inputBuffer.Length - inputOffset)
            {
                throw new ArgumentException("无效的输入参数", nameof(inputCount));
            }

            for (int i = 0; i < inputCount; i++)
            {
                if (_keyStreamPosition >= 64)
                {
                    GenerateKeyStreamBlock();
                    _keyStreamPosition = 0;

                    // 递增计数器[citation:2]
                    _state[12]++;
                    if (_state[12] is 0) // 处理溢出
                    {
                        _state[13]++;
                    }
                }

                // XOR加密：明文 ⊕ 密钥流[citation:1]
                outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ _keyStreamBlock[_keyStreamPosition]);
                _keyStreamPosition++;
            }

            return inputCount;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputCount > 0)
            {
                byte[] output = new byte[inputCount];
                TransformBlock(inputBuffer, inputOffset, inputCount, output, 0);
                return output;
            }
            return [];
        }

        public void Dispose()
        {
            Array.Clear(_state, 0, _state.Length);
            Array.Clear(_keyStreamBlock, 0, _keyStreamBlock.Length);
        }

        /// <summary>
        /// 生成64字节的密钥流块[citation:2]
        /// </summary>
        private void GenerateKeyStreamBlock()
        {
            // 复制状态到工作数组
            uint[] x = new uint[16];
            Array.Copy(_state, x, 16);

            // ChaCha20轮函数：20轮，每轮4次QuarterRound操作
            for (int i = 0; i < 10; i++) // 10轮双循环 = 20轮
            {
                // 对列进行QuarterRound操作
                QuarterRound(x, 0, 4, 8, 12);
                QuarterRound(x, 1, 5, 9, 13);
                QuarterRound(x, 2, 6, 10, 14);
                QuarterRound(x, 3, 7, 11, 15);

                // 对对角线进行QuarterRound操作
                QuarterRound(x, 0, 5, 10, 15);
                QuarterRound(x, 1, 6, 11, 12);
                QuarterRound(x, 2, 7, 8, 13);
                QuarterRound(x, 3, 4, 9, 14);
            }

            // 将结果加到原始状态上
            for (int i = 0; i < 16; i++)
            {
                x[i] += _state[i];
            }

            // 转换为字节数组
            for (int i = 0; i < 16; i++)
            {
                byte[] bytes = BitConverter.GetBytes(x[i]);
                Array.Copy(bytes, 0, _keyStreamBlock, i * 4, 4);
            }
        }

        /// <summary>
        /// ChaCha20核心操作：四分之一轮[citation:2]
        /// </summary>
        private static void QuarterRound(uint[] x, int a, int b, int c, int d)
        {
            x[a] += x[b]; x[d] = RotateLeft(x[d] ^ x[a], 16);
            x[c] += x[d]; x[b] = RotateLeft(x[b] ^ x[c], 12);
            x[a] += x[b]; x[d] = RotateLeft(x[d] ^ x[a], 8);
            x[c] += x[d]; x[b] = RotateLeft(x[b] ^ x[c], 7);
        }

        /// <summary>
        /// 循环左移操作[citation:2]
        /// </summary>
        private static uint RotateLeft(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }
    }
}
