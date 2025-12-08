using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// ChaCha20 对称加密算法实现
    /// </summary>
    public class ChaCha20
    {
        // ChaCha20 生成 64 字节的块
        private const int BlockSize = 64;

        /// <summary>
        /// 生成随机密钥
        /// </summary>
        public static byte[] GenerateKey()
        {
            byte[] key = new byte[32];
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(key);
            randomNumberGenerator.Dispose();
            return key;
        }

        /// <summary>
        /// 生成初始化向量
        /// </summary>
        public static byte[] GenerateIV()
        {
            byte[] iv = new byte[12];
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(iv);
            randomNumberGenerator.Dispose();
            return iv;
        }

        /// <summary>
        /// 加密/解密：密钥（32字节），nonce（12字节），可选的初始计数器（默认0）
        /// </summary>
        public static byte[] Encrypt(byte[] key, byte[] nonce, byte[] plaintext, uint counter = 0)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (nonce is null)
            {
                throw new ArgumentNullException(nameof(nonce));
            }

            if (plaintext is null)
            {
                throw new ArgumentNullException(nameof(plaintext));
            }

            if (key.Length is not 32)
            {
                throw new ArgumentException("Key must be 32 bytes.", nameof(key));
            }

            if (nonce.Length is not 12)
            {
                throw new ArgumentException("Nonce must be 12 bytes.", nameof(nonce));
            }

            byte[] output = new byte[plaintext.Length];
            byte[] block = new byte[BlockSize];
            uint blockCount = 0;
            int offset = 0;

            while (offset < plaintext.Length)
            {
                GenerateBlock(key, nonce, counter + blockCount, block);

                int bytesToXor = Math.Min(BlockSize, plaintext.Length - offset);
                for (int i = 0; i < bytesToXor; i++)
                {
                    output[offset + i] = (byte)(plaintext[offset + i] ^ block[i]);
                }

                offset += bytesToXor;
                blockCount++;
            }

            // 清除敏感数据
            Array.Clear(block, 0, block.Length);
            return output;
        }

        /// <summary>
        /// 解密与流密码的加密相同
        /// </summary>
        public static byte[] Decrypt(byte[] key, byte[] nonce, byte[] ciphertext, uint counter = 0)
        {
            return Encrypt(key, nonce, ciphertext, counter);
        }

        /// <summary>
        /// 生成单个 64 字节的 ChaCha20 密钥流块
        /// </summary>
        private static void GenerateBlock(byte[] key, byte[] nonce, uint counter, byte[] output)
        {
            uint[] state = new uint[16];
            uint[] working = new uint[16];

            // Constants "expand 32-byte k"
            state[0] = 0x61707865; // "expa"
            state[1] = 0x3320646e; // "nd 3"
            state[2] = 0x79622d32; // "2-by"
            state[3] = 0x6b206574; // "te k"

            // Key (32 bytes) -> state[4..11]
            for (int i = 0; i < 8; i++)
            {
                state[4 + i] = ToUInt32LittleEndian(key, i * 4);
            }

            // Counter -> state[12]
            state[12] = counter;

            // Nonce (12 bytes) -> state[13..15]
            state[13] = ToUInt32LittleEndian(nonce, 0);
            state[14] = ToUInt32LittleEndian(nonce, 4);
            state[15] = ToUInt32LittleEndian(nonce, 8);

            Array.Copy(state, working, 16);

            // 20 rounds: 10 column + diagonal rounds
            for (int i = 0; i < 10; i++)
            {
                // Column rounds
                QuarterRound(ref working[0], ref working[4], ref working[8], ref working[12]);
                QuarterRound(ref working[1], ref working[5], ref working[9], ref working[13]);
                QuarterRound(ref working[2], ref working[6], ref working[10], ref working[14]);
                QuarterRound(ref working[3], ref working[7], ref working[11], ref working[15]);
                // Diagonal rounds
                QuarterRound(ref working[0], ref working[5], ref working[10], ref working[15]);
                QuarterRound(ref working[1], ref working[6], ref working[11], ref working[12]);
                QuarterRound(ref working[2], ref working[7], ref working[8], ref working[13]);
                QuarterRound(ref working[3], ref working[4], ref working[9], ref working[14]);
            }

            // Add original state
            for (int i = 0; i < 16; i++)
            {
                uint result = working[i] + state[i];
                // write result in little-endian to output
                WriteUInt32LittleEndian(output, i * 4, result);
            }

            // Clear working/state for safety
            Array.Clear(working, 0, working.Length);
            Array.Clear(state, 0, state.Length);
        }

        private static void QuarterRound(ref uint a, ref uint b, ref uint c, ref uint d)
        {
            a += b; d ^= a; d = RotateLeft(d, 16);
            c += d; b ^= c; b = RotateLeft(b, 12);
            a += b; d ^= a; d = RotateLeft(d, 8);
            c += d; b ^= c; b = RotateLeft(b, 7);
        }

        private static uint RotateLeft(uint value, int bits)
        {
            return (value << bits) | (value >> (32 - bits));
        }

        private static uint ToUInt32LittleEndian(byte[] buf, int offset)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt32(buf, offset);
            }
            else
            {
                // Big-endian machine: reverse 4 bytes
                return (uint)buf[offset]
                     | (uint)buf[offset + 1] << 8
                     | (uint)buf[offset + 2] << 16
                     | (uint)buf[offset + 3] << 24;
            }
        }

        private static void WriteUInt32LittleEndian(byte[] buf, int offset, uint value)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] tmp = BitConverter.GetBytes(value);
                Buffer.BlockCopy(tmp, 0, buf, offset, 4);
            }
            else
            {
                buf[offset] = (byte)(value & 0xFF);
                buf[offset + 1] = (byte)((value >> 8) & 0xFF);
                buf[offset + 2] = (byte)((value >> 16) & 0xFF);
                buf[offset + 3] = (byte)((value >> 24) & 0xFF);
            }
        }
    }
}
