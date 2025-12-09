using System;
using System.Security.Cryptography;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// 使用托管库计算输入数据的 RC4 哈希值
    /// </summary>
    public class RC4CryptoTransform : RC4, ICryptoTransform
    {
        private byte[] key;
        private byte[] state = new byte[256];
        private byte x;
        private byte y;
        private bool isDisposed;

        /// <summary>
        /// 获取或设置 RC4 算法的秘密密钥。
        /// </summary>
        public override byte[] Key
        {
            get
            {
                if (KeyValue is null)
                {
                    GenerateKey();
                }

                return KeyValue.Clone() as byte[];
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException("Key");
                }

                byte[] key = value.Clone() as byte[];
                base.Key = key;
                KeySetup(key);
            }
        }

        /// <summary>
        /// 获取或设置RC4算法的初始化向量
        /// </summary>
        public override byte[] IV
        {
            get
            {
                if (IVValue is null)
                {
                    GenerateIV();
                }

                return IVValue;
            }
        }

        /// <summary>
        /// 获取哈希算法的输入块大小
        /// </summary>
        public int InputBlockSize
        {
            get { return 1; }
        }

        /// <summary>
        /// 获取哈希算法的输出块大小
        /// </summary>
        public int OutputBlockSize
        {
            get { return 1; }
        }

        /// <summary>
        /// 获取一个值，该值指示是否可以转换多个块
        /// </summary>
        public bool CanTransformMultipleBlocks
        {
            get { return true; }
        }

        /// <summary>
        /// 获取一个值，该值指示是否可以重用当前转换
        /// </summary>
        public bool CanReuseTransform
        {
            get { return false; }
        }

        /// <summary>
        /// 计算输入字节数组指定区域的哈希值，并将输入字节数组的指定区域复制到输出字节数组的指定区域
        /// </summary>
        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            CheckInput(inputBuffer, inputOffset, inputCount);
            CheckOutput(outputBuffer, outputOffset, inputCount);
            return InternalTransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
        }

        /// <summary>
        /// 计算指定字节数组的指定区域的哈希值
        /// </summary>
        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            CheckInput(inputBuffer, inputOffset, inputCount);

            byte[] output = new byte[inputCount];
            InternalTransformBlock(inputBuffer, inputOffset, inputCount, output, 0);
            return output;
        }

        /// <summary>
        /// 创建一个具有指定的对称加密器对象 Key 属性和初始化向量。
        /// </summary>
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            Key = rgbKey;
            return this;
        }

        /// <summary>
        /// 创建一个具有指定的对称解密器对象 Key 属性和初始化向量。
        /// </summary>
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            Key = rgbKey;
            return this;
        }

        /// <summary>
        /// 生成一个用于RC4算法随机初始化向量
        /// </summary>
        public override void GenerateIV()
        {
            IVValue = [];
        }

        /// <summary>
        /// 生成一个用于RC4算法随机的加密密钥
        /// </summary>
        public override void GenerateKey()
        {
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            int keySize = KeySizeValue >> 3;
            byte[] key = new byte[keySize];
            randomNumberGenerator.GetBytes(key);
            KeyValue = key;
        }

        /// <summary>
        /// 释放 RC4 算法使用的非托管资源，并可选择释放托管资源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                x = 0;
                y = 0;
                if (key is not null)
                {
                    Array.Clear(key, 0, key.Length);
                    key = null;
                }
                Array.Clear(state, 0, state.Length);
                state = null;
                isDisposed = true;
            }
        }

        private void KeySetup(byte[] key)
        {
            byte index1 = 0;
            byte index2 = 0;

            for (int counter = 0; counter < 256; counter++)
            {
                state[counter] = (byte)counter;
            }

            x = 0;
            y = 0;
            for (int counter = 0; counter < 256; counter++)
            {
                index2 = (byte)(key[index1] + state[counter] + index2);
                SwapBytes(state, counter, index2);
                index1 = (byte)((index1 + 1) % key.Length);
            }
        }

        private int InternalTransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            byte xorIndex;
            for (int counter = 0; counter < inputCount; counter++)
            {
                x = (byte)(x + 1);
                y = (byte)(state[x] + y);
                SwapBytes(state, x, y);

                xorIndex = (byte)(state[x] + state[y]);
                outputBuffer[outputOffset + counter] = (byte)(inputBuffer[inputOffset + counter] ^ state[xorIndex]);
            }
            return inputCount;
        }

        private void SwapBytes(byte[] buffer, int index1, int index2)
        {
            byte tmp = buffer[index1];
            buffer[index1] = buffer[index2];
            buffer[index2] = tmp;
        }

        private void CheckInput(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputBuffer is null)
            {
                throw new ArgumentNullException(nameof(inputBuffer));
            }

            if (inputOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(inputOffset), "< 0");
            }

            if (inputCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(inputCount), "< 0");
            }

            if (inputOffset > inputBuffer.Length - inputCount)
            {
                throw new ArgumentException("Offset is outside the size of the input", nameof(inputBuffer));
            }
        }

        private static void CheckOutput(byte[] outputBuffer, int outputOffset, int inputCount)
        {
            if (outputBuffer is null)
            {
                throw new ArgumentNullException(nameof(outputBuffer));
            }

            if (outputOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(outputOffset), "< 0");
            }

            if (outputOffset > outputBuffer.Length - inputCount)
            {
                throw new ArgumentException("Requested output offset is outside the size of the output buffer", nameof(outputBuffer));
            }
        }
    }
}
