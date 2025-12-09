using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    public class RC6Transform(byte[] key, byte[] iv, bool encrypt, CipherMode mode, PaddingMode padding) : ICryptoTransform
    {
        private const int BLOCK_SIZE = 16;
        private const int R = 20;
        private const uint P32 = 0xB7E15163;
        private const uint Q32 = 0x9E3779B9;

        private readonly uint[] S = GenerateKeySchedule(key);
        private readonly bool encrypt = encrypt;
        private readonly byte[] IV = iv != null ? (byte[])iv.Clone() : new byte[BLOCK_SIZE];
        private readonly CipherMode mode = mode;
        private readonly PaddingMode padding = padding;

        public bool CanTransformMultipleBlocks => true;

        public bool CanReuseTransform => false;

        public int InputBlockSize => BLOCK_SIZE;

        public int OutputBlockSize => BLOCK_SIZE;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private static uint[] GenerateKeySchedule(byte[] key)
        {
            int c = key.Length / 4;
            if (key.Length % 4 != 0)
            {
                c++;
            }

            uint[] L = new uint[c];
            for (int i = 0; i < key.Length; i++)
            {
                L[i / 4] |= (uint)(key[i] << (8 * (i % 4)));
            }

            uint[] S = new uint[2 * R + 4];
            S[0] = P32;
            for (int i = 1; i < S.Length; i++)
            {
                S[i] = S[i - 1] + Q32;
            }

            uint A = 0, B = 0;
            int v = 3 * Math.Max(c, S.Length);
            int iIndex = 0, jIndex = 0;

            for (int i = 0; i < v; i++)
            {
                A = S[iIndex] = RotateLeft(S[iIndex] + A + B, 3);
                B = L[jIndex] = RotateLeft(L[jIndex] + A + B, (int)(A + B));
                iIndex = (iIndex + 1) % S.Length;
                jIndex = (jIndex + 1) % c;
            }

            return S;
        }

        private static uint RotateLeft(uint x, int y)
        {
            return (x << (y & 31)) | (x >> (32 - (y & 31)));
        }

        private static uint RotateRight(uint x, int y)
        {
            return (x >> (y & 31)) | (x << (32 - (y & 31)));
        }

        private void EncryptBlock(byte[] input, byte[] output)
        {
            uint A = BitConverter.ToUInt32(input, 0);
            uint B = BitConverter.ToUInt32(input, 4);
            uint C = BitConverter.ToUInt32(input, 8);
            uint D = BitConverter.ToUInt32(input, 12);

            B += S[0];
            D += S[1];

            for (int i = 1; i <= R; i++)
            {
                uint t = RotateLeft(B * (2 * B + 1), 5);
                uint u = RotateLeft(D * (2 * D + 1), 5);

                A = RotateLeft(A ^ t, (int)u) + S[2 * i];
                C = RotateLeft(C ^ u, (int)t) + S[2 * i + 1];

                uint temp = A; A = B; B = C; C = D; D = temp;
            }

            A += S[2 * R + 2];
            C += S[2 * R + 3];

            Array.Copy(BitConverter.GetBytes(A), 0, output, 0, 4);
            Array.Copy(BitConverter.GetBytes(B), 0, output, 4, 4);
            Array.Copy(BitConverter.GetBytes(C), 0, output, 8, 4);
            Array.Copy(BitConverter.GetBytes(D), 0, output, 12, 4);
        }

        private void DecryptBlock(byte[] input, byte[] output)
        {
            uint A = BitConverter.ToUInt32(input, 0);
            uint B = BitConverter.ToUInt32(input, 4);
            uint C = BitConverter.ToUInt32(input, 8);
            uint D = BitConverter.ToUInt32(input, 12);

            A -= S[2 * R + 2];
            C -= S[2 * R + 3];

            for (int i = R; i >= 1; i--)
            {
                uint temp = D; D = C; C = B; B = A; A = temp;

                uint t = RotateLeft(B * (2 * B + 1), 5);
                uint u = RotateLeft(D * (2 * D + 1), 5);

                C = RotateRight(C - S[2 * i + 1], (int)t) ^ u;
                A = RotateRight(A - S[2 * i], (int)u) ^ t;
            }

            B -= S[0];
            D -= S[1];

            Array.Copy(BitConverter.GetBytes(A), 0, output, 0, 4);
            Array.Copy(BitConverter.GetBytes(B), 0, output, 4, 4);
            Array.Copy(BitConverter.GetBytes(C), 0, output, 8, 4);
            Array.Copy(BitConverter.GetBytes(D), 0, output, 12, 4);
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            byte[] block = new byte[BLOCK_SIZE];
            Array.Copy(inputBuffer, inputOffset, block, 0, BLOCK_SIZE);

            byte[] outBlock = new byte[BLOCK_SIZE];

            switch (mode)
            {
                case CipherMode.ECB:
                    if (encrypt)
                    {
                        EncryptBlock(block, outBlock);
                    }
                    else
                    {
                        DecryptBlock(block, outBlock);
                    }

                    break;

                case CipherMode.CBC:
                    if (encrypt)
                    {
                        for (int i = 0; i < BLOCK_SIZE; i++)
                        {
                            block[i] ^= IV[i];
                        }

                        EncryptBlock(block, outBlock);
                        Buffer.BlockCopy(outBlock, 0, IV, 0, BLOCK_SIZE);
                    }
                    else
                    {
                        byte[] temp = new byte[BLOCK_SIZE];
                        Array.Copy(block, temp, BLOCK_SIZE);

                        DecryptBlock(block, outBlock);

                        for (int i = 0; i < BLOCK_SIZE; i++)
                        {
                            outBlock[i] ^= IV[i];
                        }

                        Buffer.BlockCopy(temp, 0, IV, 0, BLOCK_SIZE);
                    }
                    break;

                case CipherMode.CFB:
                    {
                        byte[] keystream = new byte[BLOCK_SIZE];
                        EncryptBlock(IV, keystream);

                        for (int i = 0; i < BLOCK_SIZE; i++)
                        {
                            outBlock[i] = (byte)(block[i] ^ keystream[i]);
                        }

                        Buffer.BlockCopy(encrypt ? outBlock : block, 0, IV, 0, BLOCK_SIZE);
                    }
                    break;

                case CipherMode.OFB:
                    {
                        EncryptBlock(IV, IV);
                        for (int i = 0; i < BLOCK_SIZE; i++)
                        {
                            outBlock[i] = (byte)(block[i] ^ IV[i]);
                        }
                    }
                    break;

                case CipherMode.CTS:
                    {
                        throw new NotSupportedException("CTS 请用 TransformFinalBlock 内部处理");
                    }

                default:
                    {
                        throw new CryptographicException("不支持的 CipherMode");
                    }
            }

            Buffer.BlockCopy(outBlock, 0, outputBuffer, outputOffset, BLOCK_SIZE);
            return BLOCK_SIZE;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (mode is CipherMode.CTS)
            {
                return ProcessCTS(inputBuffer, inputOffset, inputCount);
            }

            if (encrypt)
            {
                byte[] padded = ApplyPadding(inputBuffer, inputOffset, inputCount);
                byte[] result = new byte[padded.Length];

                for (int i = 0; i < padded.Length; i += BLOCK_SIZE)
                {
                    TransformBlock(padded, i, BLOCK_SIZE, result, i);
                }
                return result;
            }
            else
            {
                if (inputCount % BLOCK_SIZE != 0)
                {
                    throw new CryptographicException("密文长度不正确");
                }

                byte[] buffer = new byte[inputCount];

                for (int i = 0; i < inputCount; i += BLOCK_SIZE)
                {
                    TransformBlock(inputBuffer, inputOffset + i, BLOCK_SIZE, buffer, i);
                }

                return RemovePadding(buffer);
            }
        }

        private byte[] ApplyPadding(byte[] data, int offset, int count)
        {
            int pad = BLOCK_SIZE - count % BLOCK_SIZE;
            byte[] result = new byte[count + pad];
            Array.Copy(data, offset, result, 0, count);

            switch (padding)
            {
                case PaddingMode.Zeros:
                    {
                        break;
                    }
                case PaddingMode.PKCS7:
                    {
                        for (int i = count; i < result.Length; i++)
                        {
                            result[i] = (byte)pad;
                        }
                        break;
                    }

                case PaddingMode.ANSIX923:
                    {
                        result[result.Length - 1] = (byte)pad;
                        break;
                    }
                case PaddingMode.ISO10126:
                    {
                        using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
                        {
                            randomNumberGenerator.GetBytes(result, count, pad - 1);
                        }
                        result[result.Length - 1] = (byte)pad;
                    }
                    break;

                default:
                    {
                        throw new CryptographicException("不支持 PaddingMode.None 的自动填充");
                    }
            }

            return result;
        }

        private byte[] RemovePadding(byte[] data)
        {
            switch (padding)
            {
                case PaddingMode.None:
                    {
                        return data;
                    }

                case PaddingMode.Zeros:
                    {
                        int i = data.Length;
                        while (i > 0 && data[i - 1] == 0) i--;
                        byte[] r0 = new byte[i];
                        Array.Copy(data, r0, i);
                        return r0;
                    }
                case PaddingMode.PKCS7:
                case PaddingMode.ANSIX923:
                case PaddingMode.ISO10126:
                    {
                        int pad = data[data.Length - 1];
                        if (pad <= 0 || pad > BLOCK_SIZE)
                            throw new CryptographicException("Padding 无效");

                        byte[] r = new byte[data.Length - pad];
                        Array.Copy(data, r, r.Length);
                        return r;
                    }
                default:
                    {
                        throw new CryptographicException("未知 PaddingMode");
                    }
            }
        }

        private byte[] ProcessCTS(byte[] input, int offset, int count)
        {
            int full = count / BLOCK_SIZE * BLOCK_SIZE;
            int remain = count % BLOCK_SIZE;

            if (remain is 0)
            {
                remain = BLOCK_SIZE;
            }

            byte[] output = new byte[count];

            int pos = 0;
            for (; pos < full - BLOCK_SIZE; pos += BLOCK_SIZE)
            {
                TransformBlock(input, offset + pos, BLOCK_SIZE, output, pos);
            }

            byte[] last = new byte[BLOCK_SIZE];
            TransformBlock(input, offset + pos, BLOCK_SIZE, last, 0);

            if (remain < BLOCK_SIZE)
            {
                Array.Copy(last, 0, output, pos, remain);
                byte[] final = new byte[BLOCK_SIZE];
                Buffer.BlockCopy(input, offset + pos + remain, final, 0, BLOCK_SIZE);
                TransformBlock(final, 0, BLOCK_SIZE, output, pos + remain);
            }
            else
            {
                Buffer.BlockCopy(last, 0, output, pos, BLOCK_SIZE);
            }

            return output;
        }
    }
}
