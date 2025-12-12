using System;
using System.Linq;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// 使用 Rabbit 算法对数据执行加密转换
    /// </summary>
    public class RabbitTransform : ICryptoTransform
    {
        /// <summary>
        /// 创建一个新实例
        /// </summary>
        /// <param name="rgbKey">128 位密钥</param>
        /// <param name="rgbIV">64 位 IV（可选）</param>
        /// <exception cref="ArgumentNullException">密钥不能为空</exception>
        /// <exception cref="ArgumentOutOfRangeException">密钥必须为128位（16字节）。-或- 初始向量（IV）必须为64位（8字节）</exception>
        public RabbitTransform(byte[] rgbKey, byte[] rgbIV, bool isEncrypt, PaddingMode paddingMode)
        {
            if (rgbKey is null)
            {
                throw new ArgumentNullException(nameof(rgbKey), "Key cannot be null.");
            }

            if (rgbKey.Length is not 16)
            {
                throw new ArgumentOutOfRangeException(nameof(rgbKey), "Key must be 128 bits (16 bytes).");
            }

            if ((rgbIV is not null) && (rgbIV.Length is not 8))
            {
                throw new ArgumentOutOfRangeException(nameof(rgbKey), "IV must be 64 bits (8 bytes).");
            }

            IsEncrypt = isEncrypt;
            PaddingMode = paddingMode;

            X = new DWord[8];
            C = new DWord[8];
            G = new DWord[8];
            S = new DWord[4];

            DecryptionBuffer = new byte[16];
            SetupKey(rgbKey);
            if (rgbIV is not null)
            {
                SetupIV(rgbIV);
            }
        }

        private readonly bool IsEncrypt;
        private readonly PaddingMode PaddingMode;

        #region ICryptoTransform

        public bool CanReuseTransform => false;

        public bool CanTransformMultipleBlocks => true;

        public int InputBlockSize => 16;  // in bytes

        public int OutputBlockSize => 16;  // in bytes

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            if (inputBuffer is null)
            {
                throw new ArgumentNullException(nameof(inputBuffer), "Input buffer cannot be null.");
            }

            if (outputBuffer is null)
            {
                throw new ArgumentNullException(nameof(outputBuffer), "Output buffer cannot be null.");
            }

            if (inputOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(inputOffset), "Input offset must be non-negative number.");
            }

            if (outputOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(outputOffset), "Output offset must be non-negative number.");
            }

            if ((inputCount <= 0) || (inputCount % 16 is not 0) || (inputCount > inputBuffer.Length))
            {
                throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input count.");
            }

            if ((inputBuffer.Length - inputCount) < inputOffset)
            {
                throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input length.");
            }

            if (outputOffset + inputCount > outputBuffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(outputOffset), "Insufficient output buffer.");
            }

            // 加密
            if (IsEncrypt)
            {
                for (int i = 0; i < inputCount; i += 16)
                {
                    ProcessBytes(inputBuffer, inputOffset + i, 16, outputBuffer, outputOffset + i);
                }
                return inputCount;
            }
            // 解密
            else
            {
                int bytesWritten = 0;

                if (DecryptionBufferInUse)
                {
                    // process the last block of previous round
                    ProcessBytes(DecryptionBuffer, 0, 16, outputBuffer, outputOffset);
                    DecryptionBufferInUse = false;
                    outputOffset += 16;
                    bytesWritten += 16;
                }

                for (int i = 0; i < inputCount - 16; i += 16)
                {
                    ProcessBytes(inputBuffer, inputOffset + i, 16, outputBuffer, outputOffset);
                    outputOffset += 16;
                    bytesWritten += 16;
                }

                if (PaddingMode is PaddingMode.None)
                {
                    ProcessBytes(inputBuffer, inputOffset + bytesWritten, inputCount - bytesWritten, outputBuffer, outputOffset);
                    return inputCount;
                }
                else
                {
                    // save last block without processing because decryption otherwise cannot detect padding in CryptoStream
                    Buffer.BlockCopy(inputBuffer, inputOffset + inputCount - 16, DecryptionBuffer, 0, 16);
                    DecryptionBufferInUse = true;
                }

                return bytesWritten;
            }
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputBuffer is null)
            {
                throw new ArgumentNullException(nameof(inputBuffer), "Input buffer cannot be null.");
            }

            if (inputOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(inputOffset), "Input offset must be non-negative number.");
            }

            if ((inputCount < 0) || (inputCount > inputBuffer.Length))
            {
                throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input count.");
            }

            if ((inputBuffer.Length - inputCount) < inputOffset)
            {
                throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input length.");
            }

            // 加密
            if (IsEncrypt)
            {
                int paddedLength;
                byte[] paddedInputBuffer;
                int paddedInputOffset;
                switch (PaddingMode)
                {
                    case PaddingMode.None:
                        {
                            paddedLength = inputCount;
                            paddedInputBuffer = inputBuffer;
                            paddedInputOffset = inputOffset;
                            break;
                        }
                    case PaddingMode.PKCS7:
                        {
                            paddedLength = inputCount / 16 * 16 + 16; //to round to next whole block
                            paddedInputBuffer = new byte[paddedLength];
                            paddedInputOffset = 0;
                            Buffer.BlockCopy(inputBuffer, inputOffset, paddedInputBuffer, 0, inputCount);
                            byte added = (byte)(paddedLength - inputCount);
                            for (int i = inputCount; i < inputCount + added; i++)
                            {
                                paddedInputBuffer[i] = added;
                            }
                            break;
                        }
                    case PaddingMode.Zeros:
                        {
                            paddedLength = (inputCount + 15) / 16 * 16; //to round to next whole block
                            paddedInputBuffer = new byte[paddedLength];
                            paddedInputOffset = 0;
                            Buffer.BlockCopy(inputBuffer, inputOffset, paddedInputBuffer, 0, inputCount);
                            break;
                        }
                    case PaddingMode.ANSIX923:
                        {
                            paddedLength = inputCount / 16 * 16 + 16; //to round to next whole block
                            paddedInputBuffer = new byte[paddedLength];
                            paddedInputOffset = 0;
                            Buffer.BlockCopy(inputBuffer, inputOffset, paddedInputBuffer, 0, inputCount);
                            paddedInputBuffer[paddedInputBuffer.Length - 1] = (byte)(paddedLength - inputCount);
                            break;
                        }
                    case PaddingMode.ISO10126:
                        {
                            paddedLength = inputCount / 16 * 16 + 16; //to round to next whole block
                            paddedInputBuffer = new byte[paddedLength];
                            using (RNGCryptoServiceProvider rngCryptoServiceProvider = new())
                            {
                                int fillLen = paddedLength - inputCount;
                                byte[] temp = new byte[fillLen];
                                rngCryptoServiceProvider.GetBytes(temp);
                                Buffer.BlockCopy(temp, 0, paddedInputBuffer, inputCount, fillLen);
                            }
                            paddedInputOffset = 0;
                            Buffer.BlockCopy(inputBuffer, inputOffset, paddedInputBuffer, 0, inputCount);
                            paddedInputBuffer[paddedInputBuffer.Length - 1] = (byte)(paddedLength - inputCount);
                            break;
                        }
                    default:
                        {
                            throw new CryptographicException("Unsupported padding mode.");
                        }
                }

                byte[] outputBuffer = new byte[paddedLength];
                int remainingBytes = 16;
                for (int i = 0; i < paddedLength; i += 16)
                {
                    if (PaddingMode is PaddingMode.None)
                    {
                        // padding None is special as it doesn't extend buffer
                        remainingBytes = (i + 16 > inputCount) ? inputCount % 16 : 16;
                    }

                    ProcessBytes(paddedInputBuffer, paddedInputOffset + i, remainingBytes, outputBuffer, i);
                }

                return outputBuffer;
            }
            // 解密
            else
            {
                byte[] outputBuffer;

                if (PaddingMode is PaddingMode.None)
                {
                    outputBuffer = new byte[inputCount];
                }
                else if (inputCount % 16 is not 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(inputCount), "Invalid input count.");
                }
                else
                {
                    outputBuffer = new byte[inputCount + (DecryptionBufferInUse ? 16 : 0)];
                }

                int outputOffset = 0;

                if (DecryptionBufferInUse)
                {
                    // process leftover padding buffer to keep CryptoStream happy
                    ProcessBytes(DecryptionBuffer, 0, 16, outputBuffer, 0);
                    DecryptionBufferInUse = false;
                    outputOffset = 16;
                }

                for (int i = 0; i < inputCount; i += 16)
                {
                    int remainingBytes = (i + 16 > inputCount) ? inputCount % 16 : 16;
                    ProcessBytes(inputBuffer, inputOffset + i, remainingBytes, outputBuffer, outputOffset + i);
                }

                return RemovePadding(outputBuffer, PaddingMode);
            }
        }

        #endregion ICryptoTransform

        #region IDisposable

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Array.Clear(X, 0, X.Length);
                Array.Clear(C, 0, C.Length);
                Carry = DWord.False;
                Array.Clear(G, 0, G.Length);
                Array.Clear(S, 0, S.Length);
                Array.Clear(DecryptionBuffer, 0, DecryptionBuffer.Length);
            }
        }

        #endregion IDisposable

        #region Helpers

        private readonly byte[] DecryptionBuffer; // used to store last block under decrypting as to work around CryptoStream implementation details
        private bool DecryptionBufferInUse;

        private void ProcessBytes(byte[] inputBuffer, int inputOffset, int count, byte[] outputBuffer, int outputOffset)
        {
            CounterUpdate();
            NextState();
            ExtractOutput();
            for (int i = 0; i < count; i++)
            {
                int s = (byte)(S[i / 4] >> (i % 4 * 8));
                outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ s);
            }
        }

        private static byte[] RemovePadding(byte[] outputBuffer, PaddingMode paddingMode)
        {
            if (paddingMode is PaddingMode.PKCS7)
            {
                int padding = outputBuffer[outputBuffer.Length - 1];
                if (padding is < 1 or > 16)
                {
                    throw new CryptographicException("Invalid padding.");
                }

                for (int i = outputBuffer.Length - padding; i < outputBuffer.Length; i++)
                {
                    if (outputBuffer[i] != padding) { throw new CryptographicException("Invalid padding."); }
                }

                byte[] newOutputBuffer = new byte[outputBuffer.Length - padding];
                Buffer.BlockCopy(outputBuffer, 0, newOutputBuffer, 0, newOutputBuffer.Length);
                return newOutputBuffer;
            }
            else if (paddingMode is PaddingMode.Zeros)
            {
                int newOutputLength = outputBuffer.Length;
                for (int i = outputBuffer.Length - 1; i >= Math.Max(outputBuffer.Length - 16, 0); i--)
                {
                    if (outputBuffer[i] is not 0)
                    {
                        newOutputLength = i + 1;
                        break;
                    }
                }

                if (newOutputLength == outputBuffer.Length)
                {
                    return outputBuffer;
                }
                else
                {
                    byte[] newOutputBuffer = new byte[newOutputLength];
                    Buffer.BlockCopy(outputBuffer, 0, newOutputBuffer, 0, newOutputBuffer.Length);
                    return newOutputBuffer;
                }
            }
            else if (paddingMode is PaddingMode.ANSIX923)
            {
                int padding = outputBuffer[outputBuffer.Length - 1];
                if (padding is < 1 or > 16)
                {
                    throw new CryptographicException("Invalid padding.");
                }

                for (int i = outputBuffer.Length - padding; i < outputBuffer.Length - 1; i++)
                {
                    if (outputBuffer[i] is not 0)
                    {
                        throw new CryptographicException("Invalid padding.");
                    }
                }

                byte[] newOutputBuffer = new byte[outputBuffer.Length - padding];
                Buffer.BlockCopy(outputBuffer, 0, newOutputBuffer, 0, newOutputBuffer.Length);
                return newOutputBuffer;
            }
            else if (paddingMode is PaddingMode.ISO10126)
            {
                int padding = outputBuffer[outputBuffer.Length - 1];
                if (padding is < 1 or > 16)
                {
                    throw new CryptographicException("Invalid padding.");
                }

                byte[] newOutputBuffer = new byte[outputBuffer.Length - padding];
                Buffer.BlockCopy(outputBuffer, 0, newOutputBuffer, 0, newOutputBuffer.Length);
                return newOutputBuffer;
            }
            else
            {  // None
                return outputBuffer;
            }
        }

        #endregion Helpers

        #region Implementation

        private readonly DWord[] X;   // state variables
        private readonly DWord[] C;   // counter variable
        private DWord Carry;          // counter carry - actually just a boolean
        private static readonly DWord[] A = [0x4D34D34D, 0xD34D34D3, 0x34D34D34, 0x4D34D34D, 0xD34D34D3, 0x34D34D34, 0x4D34D34D, 0xD34D34D3];
        private readonly DWord[] G;   // temporary state - to avoid allocating new array every time
        private readonly DWord[] S;   // temporary state - to avoid allocating new array every time

        private void SetupKey(byte[] key)
        {
            DWord[] k = new DWord[4];
            for (int j = 0; j < 4; j++)
            {
                k[j] = new DWord(key, j * 4);
            }

            for (int j = 0; j < 8; j++)
            {
                int j2 = j / 2;
                if (j % 2 is 0)
                {
                    X[j] = k[j2];
                    C[j] = k[(j2 + 2) & 0x3].RotateLeft(16);
                }
                else
                {
                    X[j] = (k[(j2 + 3) & 0x3] << 16) | (k[(j2 + 2) & 0x3] >> 16);
                    C[j] = k[j2].MaskUpper() | k[(j2 + 1) & 0x3].MaskLower();
                }
            }
            Carry = DWord.False;

            Array.Clear(k, 0, k.Length);  // we might as well clean-up temporary key state

            // advance a bit
            for (int n = 0; n < 4; n++)
            {
                CounterUpdate();
                NextState();
            }

            // reinitialize counter variables
            for (int j = 0; j < 8; j++)
            {
                C[j] = C[j] ^ X[(j + 4) & 0x7];
            }
        }

        private void SetupIV(byte[] iv)
        {
            DWord[] i = new DWord[4];
            i[0] = new DWord(iv, 0);
            i[2] = new DWord(iv, 4);
            i[1] = (i[0] >> 16) | i[2].MaskUpper();
            i[3] = (i[2] << 16) | i[0].MaskLower();

            for (int j = 0; j < 8; j++)
            {
                C[j] ^= i[j & 0x3];
            }

            Array.Clear(i, 0, i.Length);  // we might as well clean-up temporary IV expansion

            // advance a bit
            for (int n = 0; n < 4; n++)
            {
                CounterUpdate();
                NextState();
            }
        }

        private void CounterUpdate()
        {
            DWord b = Carry;
            DWord temp;
            for (int j = 0; j < 8; j++)
            {
                temp = C[j] + A[j] + b;
                b = (temp <= C[j]) ? DWord.True : DWord.False;  // same as temp div WORDSIZE
                C[j] = temp;
            }
            Carry = b;
        }

        private void NextState()
        {
            // g_func
            ulong temp;
            for (int i = 0; i < 8; i++)
            {
                temp = (uint)(X[i] + C[i]);
                temp *= temp;
                G[i] = (uint)(temp ^ (temp >> 32));
            }

            for (int j = 0; j < 8; j++)
            {
                X[j] = j % 2 is 0 ? G[j] + G[(j + 7) & 0x7].RotateLeft(16) + G[(j + 6) & 0x7].RotateLeft(16) : G[j] + G[(j + 7) & 0x7].RotateLeft(8) + G[(j + 6) & 0x7];
            }
        }

        private void ExtractOutput()
        {
            S[0] = X[0] ^ (X[5] >> 16) ^ (X[3] << 16);
            S[1] = X[2] ^ (X[7] >> 16) ^ (X[5] << 16);
            S[2] = X[4] ^ (X[1] >> 16) ^ (X[7] << 16);
            S[3] = X[6] ^ (X[3] >> 16) ^ (X[1] << 16);
        }

        #endregion Implementation
    }
}
