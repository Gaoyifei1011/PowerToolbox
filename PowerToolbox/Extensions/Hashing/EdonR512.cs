using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// EDON-R-512 校验实现
    /// EDON-R-512 hash algorithm
    /// Pure C# implementation compatible with .NET Framework
    /// Hash size : 512 bits / 64 bytes
    /// Block size: 1024 bits / 128 bytes
    /// Based on the EDON-R reference implementation by Aleksey Kravchenko
    /// </summary>
    public sealed class EdonR512 : HashAlgorithm
    {
        private const int BlockSize = 128;
        private const int HashSizeBytes = 64;
        private readonly ulong[] _hash = new ulong[16];
        private readonly byte[] _buffer = new byte[BlockSize];
        private int _bufferLength;
        private ulong _length;

        public EdonR512()
        {
            HashSizeValue = 512;
            Initialize();
        }

        /// <summary>
        /// Initializes the EDON-R-512 state
        /// </summary>
        public override void Initialize()
        {
            // EDONR512_H0 from the reference implementation:
            // 8081828384858687
            // 88898A8B8C8D8E8F
            // 9091929394959697
            // 98999A9B9C9D9E9F
            // A0A1A2A3A4A5A6A7
            // A8A9AAABACADAEAF
            // B0B1B2B3B4B5B6B7
            // B8B9BABBBCBDBEBF
            // C0C1C2C3C4C5C6C7
            // C8C9CACBCCCDCECF
            // D0D1D2D3D4D5D6D7
            // D8D9DADBDCDDDEDF
            // E0E1E2E3E4E5E6E7
            // E8E9EAEBECEDEEEF
            // F0F1F2F3F4F5F6F7
            // F8F9FAFBFCFDFEFF

            _hash[0] = 0x8081828384858687UL;
            _hash[1] = 0x88898A8B8C8D8E8FUL;
            _hash[2] = 0x9091929394959697UL;
            _hash[3] = 0x98999A9B9C9D9E9FUL;
            _hash[4] = 0xA0A1A2A3A4A5A6A7UL;
            _hash[5] = 0xA8A9AAABACADAEAFUL;
            _hash[6] = 0xB0B1B2B3B4B5B6B7UL;
            _hash[7] = 0xB8B9BABBBCBDBEBFUL;
            _hash[8] = 0xC0C1C2C3C4C5C6C7UL;
            _hash[9] = 0xC8C9CACBCCCDCECFUL;
            _hash[10] = 0xD0D1D2D3D4D5D6D7UL;
            _hash[11] = 0xD8D9DADBDCDDDEDFUL;
            _hash[12] = 0xE0E1E2E3E4E5E6E7UL;
            _hash[13] = 0xE8E9EAEBECEDEEEFUL;
            _hash[14] = 0xF0F1F2F3F4F5F6F7UL;
            _hash[15] = 0xF8F9FAFBFCFDFEFFUL;
            Array.Clear(_buffer, 0, _buffer.Length);
            _bufferLength = 0;
            _length = 0;
        }

        /// <summary>
        /// Processes a portion of the input data
        /// </summary>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (ibStart < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ibStart));
            }

            if (cbSize < 0 || cbSize > array.Length - ibStart)
            {
                throw new ArgumentOutOfRangeException(nameof(cbSize));
            }

            if (cbSize is 0)
            {
                return;
            }

            // RHash keeps the total message length in bytes
            _length = unchecked(_length + (ulong)cbSize);
            int offset = ibStart;
            int remaining = cbSize;

            // Complete a partial block first
            if (_bufferLength != 0)
            {
                int left = BlockSize - _bufferLength;
                int copy = remaining < left ? remaining : left;
                Buffer.BlockCopy(array, offset, _buffer, _bufferLength, copy);
                _bufferLength += copy;
                offset += copy;
                remaining -= copy;

                if (_bufferLength == BlockSize)
                {
                    ProcessBlock(_buffer, 0);
                    _bufferLength = 0;
                }

                if (remaining is 0)
                {
                    return;
                }
            }

            // Process complete 128-byte blocks
            // This is equivalent to RHash's:
            // size_t count = size / edonr512_block_size;
            // rhash_edonr512_process_block(..., count);
            while (remaining >= BlockSize)
            {
                ProcessBlock(array, offset);
                offset += BlockSize;
                remaining -= BlockSize;
            }

            // Save leftover bytes
            if (remaining is not 0)
            {
                Buffer.BlockCopy(array, offset, _buffer, 0, remaining);
                _bufferLength = remaining;
            }
        }

        /// <summary>
        /// Finalizes the hash calculation
        /// </summary>
        protected override byte[] HashFinal()
        {
            // RHash:
            // index = ((unsigned)ctx->length & 127) >> 3;
            // shift = ((unsigned)ctx->length & 7) * 8;
            // Since this implementation stores the remaining message as bytes, _bufferLength is exactly:
            // ctx->length & 127
            int index = _bufferLength;

            // Append 0x80.
            // The byte immediately following the message is always replaced with 0x80
            _buffer[index++] = 0x80;

            // If the padding byte consumed the entire final 64-bit word, process this block first.
            // RHash uses:
            // if (index == 16)
            // where index is measured in UInt64 words.
            // In byte representation that is index == 128.
            if (index == BlockSize)
            {
                ProcessBlock(_buffer, 0);
                Array.Clear(_buffer, 0, _buffer.Length);
                index = 0;
            }

            // The final 8 bytes contain the message length in bits
            // Therefore bytes [index, 119] are zero
            Array.Clear(_buffer, index, 120 - index);
            ulong bitLength = unchecked(_length << 3);

            // message[15] = ctx->length << 3
            WriteUInt64LE(_buffer, 120, bitLength);
            ProcessBlock(_buffer, 0);

            // RHash:
            // off = edonr512_block_size - digest_length;
            // 128 - 64 = 64
            // Therefore the result is hash[8] ... hash[15].
            byte[] result = new byte[HashSizeBytes];

            for (int i = 0; i < 8; i++)
            {
                WriteUInt64LE(result, i * 8, _hash[i + 8]);
            }

            return result;
        }

        /// <summary>
        /// Processes one 1024-bit block
        /// </summary>
        private void ProcessBlock(byte[] data, int offset)
        {
            if (data is null)
            {
                return;
            }

            ulong[] block = new ulong[16];

            for (int i = 0; i < 16; i++)
            {
                block[i] = ReadUInt64LE(data, offset + i * 8);
            }

            // p16 ... p23
            ulong[] p16 = new ulong[8];

            // p24 ... p31
            ulong[] p24 = new ulong[8];

            // First row of quasigroup e-transformations.
            // Q512(block[15] ... block[8], block[0]  ... block[7], p16 ... p23)
            Q512(block[15], block[14], block[13], block[12], block[11], block[10], block[9], block[8], block[0], block[1], block[2], block[3], block[4], block[5], block[6], block[7], p16);

            // Q512(p16 ... p23,block[8] ... block[15], p24 ... p31)
            Q512(p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], block[8], block[9], block[10], block[11], block[12], block[13], block[14], block[15], p24);

            // Second row
            Q512(_hash[8], _hash[9], _hash[10], _hash[11], _hash[12], _hash[13], _hash[14], _hash[15], p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], p16);
            Q512(p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], p24[0], p24[1], p24[2], p24[3], p24[4], p24[5], p24[6], p24[7], p24);

            // Third row
            Q512(p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], _hash[0], _hash[1], _hash[2], _hash[3], _hash[4], _hash[5], _hash[6], _hash[7], p16);
            Q512(p24[0], p24[1], p24[2], p24[3], p24[4], p24[5], p24[6], p24[7], p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], p24);

            // Fourth row
            Q512(block[7], block[6], block[5], block[4], block[3], block[2], block[1], block[0], p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], _hash);
            Q512(_hash[0], _hash[1], _hash[2], _hash[3], _hash[4], _hash[5], _hash[6], _hash[7], p24[0], p24[1], p24[2], p24[3], p24[4], p24[5], p24[6], p24[7], _hash, 8);
        }

        /// <summary>
        /// EDON-R Q512 transformation
        /// This is a direct translation of the Q512 macro from the EDON-R reference implementation
        /// </summary>
        private static void Q512(ulong x0, ulong x1, ulong x2, ulong x3, ulong x4, ulong x5, ulong x6, ulong x7, ulong y0, ulong y1, ulong y2, ulong y3, ulong y4, ulong y5, ulong y6, ulong y7, ulong[] z)
        {
            Q512(x0, x1, x2, x3, x4, x5, x6, x7, y0, y1, y2, y3, y4, y5, y6, y7, z, 0);
        }

        /// <summary>
        /// EDON-R Q512 transformation.
        /// </summary>
        private static void Q512(ulong x0, ulong x1, ulong x2, ulong x3, ulong x4, ulong x5, ulong x6, ulong x7, ulong y0, ulong y1, ulong y2, ulong y3, ulong y4, ulong y5, ulong y6, ulong y7, ulong[] z, int offset)
        {
            ulong t0;
            ulong t1;
            ulong t2;
            ulong t3;
            ulong t4;
            ulong t5;
            ulong t6;
            ulong t7;
            ulong t8;
            ulong t9;
            ulong t10;
            ulong t11;
            ulong t12;
            ulong t13;
            ulong t14;
            ulong t15;
            ulong t16;
            ulong t17;
            ulong t18;
            ulong t19;
            ulong t20;
            ulong t21;
            ulong t22;
            ulong t23;

            // First Latin Square
            t8 = unchecked(x0 + x4);
            t9 = unchecked(x1 + x7);
            t12 = unchecked(t8 + t9);
            t10 = unchecked(x2 + x3);
            t11 = unchecked(x5 + x6);
            t13 = unchecked(t10 + t11);
            t0 = unchecked(0xAAAAAAAAAAAAAAAAUL + t12 + x2);
            t1 = RotateLeft(unchecked(t12 + x3), 5);
            t2 = RotateLeft(unchecked(t12 + x6), 19);
            t3 = RotateLeft(unchecked(t13 + x7), 29);
            t4 = RotateLeft(unchecked(x1 + t13), 31);
            t5 = RotateLeft(unchecked(t8 + t10 + x5), 41);
            t6 = RotateLeft(unchecked(x0 + t9 + t11), 57);
            t7 = RotateLeft(unchecked(t13 + x4), 61);
            t16 = t0 ^ t4;
            t17 = t1 ^ t7;
            t18 = t2 ^ t3;
            t19 = t5 ^ t6;
            t8 = t3 ^ t19;
            t9 = t2 ^ t19;
            t10 = t18 ^ t5;
            t11 = t16 ^ t1;
            t12 = t16 ^ t7;
            t13 = t17 ^ t6;
            t14 = t18 ^ t4;
            t15 = t0 ^ t17;

            // Second Orthogonal Latin Square
            t16 = unchecked(y0 + y1);
            t17 = unchecked(y2 + y5);
            t20 = unchecked(t16 + t17);
            t18 = unchecked(y3 + y4);
            t22 = unchecked(t16 + t18);
            t19 = unchecked(y6 + y7);
            t21 = unchecked(t18 + t19);
            t23 = unchecked(t17 + t19);
            t0 = unchecked(0x5555555555555555UL + t20 + y7);
            t1 = RotateLeft(unchecked(t22 + y6), 3);
            t2 = RotateLeft(unchecked(t20 + y3), 17);
            t3 = RotateLeft(unchecked(y2 + t21), 23);
            t4 = RotateLeft(unchecked(t22 + y5), 31);
            t5 = RotateLeft(unchecked(t23 + y4), 37);
            t6 = RotateLeft(unchecked(y1 + t23), 45);
            t7 = RotateLeft(unchecked(y0 + t21), 59);
            t16 = t0 ^ t1;
            t17 = t2 ^ t5;
            t18 = t3 ^ t4;
            t19 = t6 ^ t7;

            // The order here is important
            // It is the exact z5,z6,z7,z0,z1,z2,z3,z4 order used by the reference Q512 macro
            z[offset + 5] = unchecked(t8 + (t18 ^ t6));
            z[offset + 6] = unchecked(t9 + (t17 ^ t7));
            z[offset + 7] = unchecked(t10 + (t4 ^ t19));
            z[offset + 0] = unchecked(t11 + (t16 ^ t5));
            z[offset + 1] = unchecked(t12 + (t2 ^ t19));
            z[offset + 2] = unchecked(t13 + (t16 ^ t3));
            z[offset + 3] = unchecked(t14 + (t0 ^ t18));
            z[offset + 4] = unchecked(t15 + (t1 ^ t17));
        }

        /// <summary>
        /// Rotate UInt64 left
        /// </summary>
        private static ulong RotateLeft(ulong value, int count)
        {
            return (value << count) | (value >> (64 - count));
        }

        /// <summary>
        /// Read UInt64 in little-endian byte order
        /// </summary>
        private static ulong ReadUInt64LE(byte[] buffer, int offset)
        {
            if (buffer is null)
            {
                return default;
            }

            return buffer[offset] | ((ulong)buffer[offset + 1] << 8) | ((ulong)buffer[offset + 2] << 16) | ((ulong)buffer[offset + 3] << 24) | ((ulong)buffer[offset + 4] << 32) | ((ulong)buffer[offset + 5] << 40) | ((ulong)buffer[offset + 6] << 48) | ((ulong)buffer[offset + 7] << 56);
        }

        /// <summary>
        /// Write UInt64 in little-endian byte order
        /// </summary>
        private static void WriteUInt64LE(byte[] buffer, int offset, ulong value)
        {
            if (buffer is null)
            {
                return;
            }

            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
            buffer[offset + 4] = (byte)(value >> 32);
            buffer[offset + 5] = (byte)(value >> 40);
            buffer[offset + 6] = (byte)(value >> 48);
            buffer[offset + 7] = (byte)(value >> 56);
        }
    }
}
