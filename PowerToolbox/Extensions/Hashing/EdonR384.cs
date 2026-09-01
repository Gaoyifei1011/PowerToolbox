using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// EDON-R-384 哈希校验实现
    /// EDON-R-384 Hash Algorithm
    /// Pure C# implementation compatible with .NET Framework.
    /// Based on the EDON-R reference implementation.
    /// Hash size: 384 bits / 48 bytes
    /// Block size: 1024 bits / 128 bytes
    /// </summary>
    public sealed class EdonR384 : HashAlgorithm
    {
        private const int BlockSize = 128;
        private const int HashSizeBytes = 48;
        private readonly ulong[] _hash = new ulong[16];
        private readonly byte[] _buffer = new byte[BlockSize];
        private int _bufferLength;
        private ulong _length;

        public EdonR384()
        {
            HashSizeValue = 384;
            Initialize();
        }

        /// <summary>
        /// Initializes the EDON-R-384 state
        /// </summary>
        public override void Initialize()
        {
            _hash[0] = 0x0001020304050607UL;
            _hash[1] = 0x08090A0B0C0D0E0FUL;
            _hash[2] = 0x1011121314151617UL;
            _hash[3] = 0x18191A1B1C1D1E1FUL;
            _hash[4] = 0x2021222324252627UL;
            _hash[5] = 0x28292A2B2C2D2E2FUL;
            _hash[6] = 0x3031323324353637UL;
            _hash[7] = 0x38393A3B3C3D3E3FUL;
            _hash[8] = 0x4041424344454647UL;
            _hash[9] = 0x48494A4B4C4D4E4FUL;
            _hash[10] = 0x5051525354555657UL;
            _hash[11] = 0x58595A5B5C5D5E5FUL;
            _hash[12] = 0x6061626364656667UL;
            _hash[13] = 0x68696A6B6C6D6E6FUL;
            _hash[14] = 0x7071727374757677UL;
            _hash[15] = 0x78797A7B7C7D7E7FUL;
            Array.Clear(_buffer, 0, _buffer.Length);
            _bufferLength = 0;
            _length = 0;
        }

        /// <summary>
        /// Processes input data.
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

            _length += (ulong)cbSize;
            int offset = ibStart;
            int length = cbSize;

            // Fill an existing partial block
            if (_bufferLength > 0)
            {
                int required = BlockSize - _bufferLength;

                if (length < required)
                {
                    Buffer.BlockCopy(array, offset, _buffer, _bufferLength, length);
                    _bufferLength += length;
                    return;
                }

                Buffer.BlockCopy(array, offset, _buffer, _bufferLength, required);
                ProcessBlock(_buffer, 0);
                _bufferLength = 0;
                offset += required;
                length -= required;
            }

            // Process complete 128-byte blocks
            while (length >= BlockSize)
            {
                ProcessBlock(array, offset);
                offset += BlockSize;
                length -= BlockSize;
            }

            // Store remaining bytes
            if (length > 0)
            {
                Buffer.BlockCopy(array, offset, _buffer, 0, length);
                _bufferLength = length;
            }
        }

        /// <summary>
        /// Finalizes the EDON-R-384 hash
        /// </summary>
        protected override byte[] HashFinal()
        {
            int index = _bufferLength;

            // Append 0x80
            // EDON-R uses little-endian byte ordering internally
            _buffer[index++] = 0x80;

            // The reference implementation checks whether the padding byte consumed the final 64-bit word
            if (index > 120)
            {
                Array.Clear(_buffer, index, BlockSize - index);
                ProcessBlock(_buffer, 0);
                Array.Clear(_buffer, 0, _buffer.Length);
                index = 0;
            }

            // Fill with zeros until the final 64-bit word
            Array.Clear(_buffer, index, 120 - index);

            // Store message length in bits
            // Reference implementation:
            // message[15] = ctx->length << 3
            ulong bitLength = unchecked(_length << 3);
            WriteUInt64LE(_buffer, 120, bitLength);

            ProcessBlock(_buffer, 0);

            // EDON-R-384 output is the last 48 bytes of the 128-byte internal state
            // Internal state:
            // 16 x UInt64 = 128 bytes
            // 128 - 48 = 80 bytes
            // 80 / 8 = index 10
            // Therefore copy hash[10] ... hash[15]
            byte[] result = new byte[HashSizeBytes];

            for (int i = 0; i < 6; i++)
            {
                WriteUInt64LE(result, i * 8, _hash[i + 10]);
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
                block[i] = ReadUInt64LE(data, offset + (i * 8));
            }

            ulong[] p16 = new ulong[8];
            ulong[] p24 = new ulong[8];

            // First row of quasigroup e-transformations
            Q512(block[15], block[14], block[13], block[12], block[11], block[10], block[9], block[8], block[0], block[1], block[2], block[3], block[4], block[5], block[6], block[7], p16, 0);
            Q512(p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], block[8], block[9], block[10], block[11], block[12], block[13], block[14], block[15], p24, 0);

            // Second row of quasigroup e-transformations
            Q512(_hash[8], _hash[9], _hash[10], _hash[11], _hash[12], _hash[13], _hash[14], _hash[15], p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], p16, 0);
            Q512(p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], p24[0], p24[1], p24[2], p24[3], p24[4], p24[5], p24[6], p24[7], p24, 0);

            // Third row of quasigroup e-transformations
            Q512(p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], _hash[0], _hash[1], _hash[2], _hash[3], _hash[4], _hash[5], _hash[6], _hash[7], p16, 0);
            Q512(p24[0], p24[1], p24[2], p24[3], p24[4], p24[5], p24[6], p24[7], p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], p24, 0);

            // Fourth row of quasigroup e-transformations
            Q512(block[7], block[6], block[5], block[4], block[3], block[2], block[1], block[0], p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], _hash, 0);
            Q512(_hash[0], _hash[1], _hash[2], _hash[3], _hash[4], _hash[5], _hash[6], _hash[7], p24[0], p24[1], p24[2], p24[3], p24[4], p24[5], p24[6], p24[7], _hash, 8);
        }

        /// <summary>
        /// EDON-R Q512 quasigroup transformation
        /// </summary>
        private static void Q512(ulong x0, ulong x1, ulong x2, ulong x3, ulong x4, ulong x5, ulong x6, ulong x7, ulong y0, ulong y1, ulong y2, ulong y3, ulong y4, ulong y5, ulong y6, ulong y7, ulong[] output, int outputOffset)
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

            output[outputOffset + 5] = unchecked(t8 + (t18 ^ t6));
            output[outputOffset + 6] = unchecked(t9 + (t17 ^ t7));
            output[outputOffset + 7] = unchecked(t10 + (t4 ^ t19));
            output[outputOffset + 0] = unchecked(t11 + (t16 ^ t5));
            output[outputOffset + 1] = unchecked(t12 + (t2 ^ t19));
            output[outputOffset + 2] = unchecked(t13 + (t16 ^ t3));
            output[outputOffset + 3] = unchecked(t14 + (t0 ^ t18));
            output[outputOffset + 4] = unchecked(t15 + (t1 ^ t17));
        }

        /// <summary>
        /// Rotates a UInt64 value to the left
        /// </summary>
        private static ulong RotateLeft(ulong value, int count)
        {
            return (value << count) | (value >> (64 - count));
        }

        /// <summary>
        /// Reads UInt64 in little-endian format
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
        /// Writes UInt64 in little-endian format
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
