using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// PKCS7填充
    /// </summary>
    public sealed class PKCS7PaddingTransform : ICryptoTransform
    {
        #region 属性

        private readonly ICryptoTransform _transform;
        private readonly byte[] _lastBlock;
        private readonly PaddingMode _mode;
        private readonly bool _encryptMode;
        private bool _hasWithheldBlock;

        /// <summary>
        /// 获取一个值，该值指示是否可重复使用当前转换
        /// </summary>
        public bool CanReuseTransform => _transform.CanReuseTransform;

        /// <summary>
        /// 获取一个值，该值指示是否可以转换多个块
        /// </summary>
        public bool CanTransformMultipleBlocks => _transform.CanTransformMultipleBlocks;

        /// <summary>
        /// 获取输入块大小
        /// </summary>
        public int InputBlockSize => _transform.InputBlockSize;

        /// <summary>
        /// 获取输出块大小
        /// </summary>
        public int OutputBlockSize => _transform.OutputBlockSize;

        #endregion 属性

        #region 构造

        /// <summary>
        /// 实例化
        /// </summary>
        public PKCS7PaddingTransform(ICryptoTransform transform, PaddingMode mode, bool encryptMode)
        {
            _mode = mode;
            _transform = transform;
            _encryptMode = encryptMode;

            if (mode is not PaddingMode.ISO10126 and not PaddingMode.ANSIX923 and not PaddingMode.PKCS7)
            {
                throw new NotSupportedException();
            }

            if (transform.InputBlockSize > byte.MaxValue || transform.OutputBlockSize > byte.MaxValue || transform.InputBlockSize is 0 || transform.OutputBlockSize is 0)
            {
                throw new CryptographicException("Padding can only be used with block ciphers with block size of [2,255]");
            }

            _lastBlock = new byte[encryptMode ? OutputBlockSize : InputBlockSize];
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _transform.Dispose();
        }

        #endregion 构造

        /// <summary>
        /// 转换输入字节数组的指定区域，并将所得到的转换复制到输出字节数组的指定区域
        /// </summary>
        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            int count = _transform.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
            if (_encryptMode)
            {
                return count;
            }

            //todo !!! 仅能临时解决短密文填充清理问题
            if (!_encryptMode && count <= OutputBlockSize)
            {
                // 最后一块
                if (count == OutputBlockSize)
                {
                    // 清除后面的填充
                    byte last = outputBuffer[outputOffset + count - 1];
                    if (last < count)
                    {
                        int pads = 0;
                        for (int i = OutputBlockSize - 1; i >= 0; i--)
                        {
                            if (outputBuffer[outputOffset + i] != last) break;
                            pads++;
                        }

                        return pads != last ? count : count - pads;
                    }
                }

                return count;
            }

            if (_hasWithheldBlock)
            {
                byte[] lastBlock = new byte[OutputBlockSize];
                Array.Copy(outputBuffer, outputOffset + count - OutputBlockSize, lastBlock, 0, OutputBlockSize);
                Array.Copy(outputBuffer, outputOffset, outputBuffer, outputOffset + OutputBlockSize, count - OutputBlockSize);
                Array.Copy(_lastBlock, 0, outputBuffer, outputOffset, OutputBlockSize);
                Array.Copy(lastBlock, 0, _lastBlock, 0, OutputBlockSize);
            }
            else
            {
                Array.Copy(outputBuffer, outputOffset + count - OutputBlockSize, _lastBlock, 0, OutputBlockSize);
                _hasWithheldBlock = true;
                count -= OutputBlockSize;
            }

            return count;
        }

        /// <summary>
        /// 转换指定字节数组的指定区域
        /// </summary>
        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (_encryptMode)
            {
                if (inputCount is 0)
                {
                    return [];
                }

                int paddingLength = InputBlockSize - (inputCount % InputBlockSize);
                int paddingValue = _mode switch
                {
                    PaddingMode.ANSIX923 => 0,
                    PaddingMode.ISO10126 => (GetHashCode() & 0xFF) ^ paddingLength,
                    PaddingMode.PKCS7 => paddingLength,
                    _ => throw new Exception()
                };
                byte[] cipherBlock = new byte[inputCount + paddingLength];
                Array.Copy(inputBuffer, inputOffset, cipherBlock, 0, inputCount);
                for (int i = InputBlockSize; i >= 1; i--)
                {
                    int posMask = ~(paddingLength - i) >> 31;
                    cipherBlock[cipherBlock.Length - i] &= (byte)~posMask;
                    cipherBlock[cipherBlock.Length - i] |= (byte)(paddingValue & posMask);
                }

                if (cipherBlock.Length <= InputBlockSize || CanTransformMultipleBlocks)
                    return _transform.TransformFinalBlock(cipherBlock, 0, cipherBlock.Length);

                int remainingBlocks = cipherBlock.Length / InputBlockSize;
                byte[] returnData = new byte[(remainingBlocks - 1) * OutputBlockSize];
                for (int i = 0; i < remainingBlocks - 1; i++)
                    _transform.TransformBlock(cipherBlock, i * InputBlockSize, InputBlockSize, returnData, i * OutputBlockSize);

                byte[] lastBlock = _transform.TransformFinalBlock(cipherBlock, cipherBlock.Length - InputBlockSize, InputBlockSize);
                Array.Resize(ref returnData, returnData.Length + lastBlock.Length);
                Array.Copy(lastBlock, 0, returnData, OutputBlockSize, lastBlock.Length);
                return returnData;
            }
            else
            {
                if (inputCount is 0 && !_hasWithheldBlock)
                {
                    return [];
                }

                byte[] data = _transform.TransformFinalBlock(inputBuffer, inputOffset, inputCount);
                if (_hasWithheldBlock)
                {
                    Array.Resize(ref data, data.Length + OutputBlockSize);
                    Array.Copy(data, 0, data, OutputBlockSize, data.Length - OutputBlockSize);
                    Array.Copy(_lastBlock, 0, data, 0, OutputBlockSize);
                }

                if (data.Length < 1)
                    throw new CryptographicException("Invalid padding");

                int paddingLength = data[data.Length - 1];
                int paddingValue = _mode is PaddingMode.ANSIX923 ? 0 : paddingLength;
                int paddingError = 0;
                if (_mode != PaddingMode.ISO10126)
                {
                    for (int i = OutputBlockSize; i >= 1; i--)
                    {
                        int posMask = ~(paddingLength - i) >> 31;
                        paddingError |= (paddingValue ^ data[data.Length - i]) & posMask;
                    }
                }

                if (paddingError is not 0 || paddingLength is 0 || paddingLength > OutputBlockSize)
                {
                    throw new CryptographicException("Invalid padding");
                }

                Array.Resize(ref data, data.Length - paddingLength);
                return data;
            }
        }
    }
}
