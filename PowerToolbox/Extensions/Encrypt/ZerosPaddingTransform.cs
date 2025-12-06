using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// Zero填充
    /// </summary>
    /// <remarks>实例化</remarks>
    public sealed class ZerosPaddingTransform(ICryptoTransform transform, bool encryptMode) : ICryptoTransform
    {
        #region 属性

        private readonly ICryptoTransform _transform = transform;
        private readonly bool _encryptMode = encryptMode;

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

            if (!_encryptMode)
            {
                // 清除后面的填充
                int pads = 0;
                for (int i = OutputBlockSize - 1; i >= 0; i--)
                {
                    if (outputBuffer[outputOffset + i] is not 0) break;
                    pads++;
                }

                return pads is 0 ? count : count - pads;
            }

            return count;
        }

        /// <summary>
        /// 转换指定字节数组的指定区域
        /// </summary>
        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputCount is 0)
            {
                return [];
            }

            //todo !!! 仅能临时解决短密文填充清理问题
            if (_encryptMode && inputCount % InputBlockSize is not 0)
            {
                int paddingNeeded = InputBlockSize - (inputCount % InputBlockSize);
                byte[] padded = new byte[inputCount + paddingNeeded];
                Array.Copy(inputBuffer, inputOffset, padded, 0, inputCount);
                inputBuffer = padded;
                inputOffset = 0;
                inputCount += paddingNeeded;
            }

            return _transform.TransformFinalBlock(inputBuffer, inputOffset, inputCount);
        }
    }
}
