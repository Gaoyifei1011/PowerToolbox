using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// EDON-R-224 哈希校验实现
    /// EDON-R-224 Hash Algorithm
    /// Pure C# implementation for .NET Framework
    /// Hash size: 224 bits / 28 bytes
    /// Block size: 512 bits / 64 bytes
    /// </summary>
    public sealed class EdonR224 : HashAlgorithm
    {
        private const int BlockSize = 64;
        private const int HashSizeBytes = 28;
        private readonly uint[] _hash = new uint[16];
        private readonly byte[] _buffer = new byte[BlockSize];
        private int _bufferLength;
        private ulong _length;

        public EdonR224()
        {
            HashSizeValue = 224;
            Initialize();
        }

        /// <summary>
        /// Initializes the EDON-R-224 state
        /// </summary>
        public override void Initialize()
        {
            _hash[0] = 0x00010203;
            _hash[1] = 0x04050607;
            _hash[2] = 0x08090A0B;
            _hash[3] = 0x0C0D0E0F;
            _hash[4] = 0x10111213;
            _hash[5] = 0x14151617;
            _hash[6] = 0x18191A1B;
            _hash[7] = 0x1C1D1E1F;
            _hash[8] = 0x20212223;
            _hash[9] = 0x24252627;
            _hash[10] = 0x28292A2B;
            _hash[11] = 0x2C2D2E2F;
            _hash[12] = 0x30313233;
            _hash[13] = 0x24353637;
            _hash[14] = 0x38393A3B;
            _hash[15] = 0x3C3D3E3F;
            Array.Clear(_buffer, 0, _buffer.Length);
            _bufferLength = 0;
            _length = 0;
        }

        /// <summary>
        /// Processes input data
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

            // Complete an existing partial block
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

            // Process complete blocks
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
        /// Finalizes the hash calculation
        /// </summary>
        protected override byte[] HashFinal()
        {
            ulong messageLength = _length;

            // Append the mandatory padding bit
            _buffer[_bufferLength++] = 0x80;

            // If there is no room for the message length process the current block first
            if (_bufferLength > 56)
            {
                Array.Clear(_buffer, _bufferLength, BlockSize - _bufferLength);
                ProcessBlock(_buffer, 0);
                _bufferLength = 0;
            }

            // Zero padding
            Array.Clear(_buffer, _bufferLength, 56 - _bufferLength);

            // EDON-R uses message length in bits
            ulong bitLength = messageLength << 3;
            WriteUInt32LE(_buffer, 56, (uint)bitLength);
            WriteUInt32LE(_buffer, 60, (uint)(bitLength >> 32));
            ProcessBlock(_buffer, 0);

            // EDON-R-224 returns the last 28 bytes of the 64-byte internal state
            byte[] result = new byte[HashSizeBytes];

            for (int i = 0; i < 7; i++)
            {
                WriteUInt32LE(result, i * 4, _hash[i + 9]);
            }

            return result;
        }

        /// <summary>
        /// Processes a single 512-bit block
        /// </summary>
        private void ProcessBlock(byte[] data, int offset)
        {
            if (data is null)
            {
                return;
            }

            uint[] block = new uint[16];

            for (int i = 0; i < 16; i++)
            {
                block[i] = ReadUInt32LE(data, offset + (i * 4));
            }

            uint[] p16 = new uint[8];
            uint[] p24 = new uint[8];

            // First transformation
            Q256(block[15], block[14], block[13], block[12], block[11], block[10], block[9], block[8], block[0], block[1], block[2], block[3], block[4], block[5], block[6], block[7], p16, 0);
            Q256(p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], block[8], block[9], block[10], block[11], block[12], block[13], block[14], block[15], p24, 0);

            // Second transformation
            Q256(_hash[8], _hash[9], _hash[10], _hash[11], _hash[12], _hash[13], _hash[14], _hash[15], p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], p16, 0);
            Q256(p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], p24[0], p24[1], p24[2], p24[3], p24[4], p24[5], p24[6], p24[7], p24, 0);

            // Third transformation
            Q256(p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], _hash[0], _hash[1], _hash[2], _hash[3], _hash[4], _hash[5], _hash[6], _hash[7], p16, 0);
            Q256(p24[0], p24[1], p24[2], p24[3], p24[4], p24[5], p24[6], p24[7], p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], p24, 0);

            // Final transformation
            Q256(block[7], block[6], block[5], block[4], block[3], block[2], block[1], block[0], p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], _hash, 0);
            Q256(_hash[0], _hash[1], _hash[2], _hash[3], _hash[4], _hash[5], _hash[6], _hash[7], p24[0], p24[1], p24[2], p24[3], p24[4], p24[5], p24[6], p24[7], _hash, 8);
        }

        /// <summary>
        /// EDON-R Q256 transformation
        /// </summary>
        private static void Q256(uint x0, uint x1, uint x2, uint x3, uint x4, uint x5, uint x6, uint x7, uint y0, uint y1, uint y2, uint y3, uint y4, uint y5, uint y6, uint y7, uint[] output, int outputOffset)
        {
            uint t0;
            uint t1;
            uint t2;
            uint t3;
            uint t4;
            uint t5;
            uint t6;
            uint t7;
            uint t8;
            uint t9;
            uint t10;
            uint t11;
            uint t12;
            uint t13;
            uint t14;
            uint t15;
            uint t16;
            uint t17;
            uint t18;
            uint t19;
            uint t20;
            uint t21;
            uint t22;
            uint t23;

            // First quasigroup transformation
            t8 = unchecked(x0 + x4);
            t9 = unchecked(x1 + x7);
            t12 = unchecked(t8 + t9);
            t10 = unchecked(x2 + x3);
            t11 = unchecked(x5 + x6);
            t13 = unchecked(t10 + t11);
            t0 = unchecked(0xAAAAAAAAu + t12 + x2);
            t1 = RotateLeft(unchecked(t12 + x3), 5);
            t2 = RotateLeft(unchecked(t12 + x6), 11);
            t3 = RotateLeft(unchecked(t13 + x7), 13);
            t4 = RotateLeft(unchecked(x1 + t13), 17);
            t5 = RotateLeft(unchecked(t8 + t10 + x5), 19);
            t6 = RotateLeft(unchecked(x0 + t9 + t11), 29);
            t7 = RotateLeft(unchecked(t13 + x4), 31);
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

            // Second quasigroup transformation
            t16 = unchecked(y0 + y1);
            t17 = unchecked(y2 + y5);
            t20 = unchecked(t16 + t17);
            t18 = unchecked(y3 + y4);
            t22 = unchecked(t16 + t18);
            t19 = unchecked(y6 + y7);
            t21 = unchecked(t18 + t19);
            t23 = unchecked(t17 + t19);
            t0 = unchecked(0x55555555u + t20 + y7);
            t1 = RotateLeft(unchecked(t22 + y6), 3);
            t2 = RotateLeft(unchecked(t20 + y3), 7);
            t3 = RotateLeft(unchecked(y2 + t21), 11);
            t4 = RotateLeft(unchecked(t22 + y5), 17);
            t5 = RotateLeft(unchecked(t23 + y4), 19);
            t6 = RotateLeft(unchecked(y1 + t23), 23);
            t7 = RotateLeft(unchecked(y0 + t21), 29);
            t16 = t0 ^ t1;
            t17 = t2 ^ t5;
            t18 = t3 ^ t4;
            t19 = t6 ^ t7;

            output[outputOffset + 0] = unchecked(t11 + (t16 ^ t5));
            output[outputOffset + 1] = unchecked(t12 + (t2 ^ t19));
            output[outputOffset + 2] = unchecked(t13 + (t16 ^ t3));
            output[outputOffset + 3] = unchecked(t14 + (t0 ^ t18));
            output[outputOffset + 4] = unchecked(t15 + (t1 ^ t17));
            output[outputOffset + 5] = unchecked(t8 + (t18 ^ t6));
            output[outputOffset + 6] = unchecked(t9 + (t17 ^ t7));
            output[outputOffset + 7] = unchecked(t10 + (t4 ^ t19));
        }

        /// <summary>
        /// Rotates a 32-bit unsigned integer to the left
        /// </summary>
        private static uint RotateLeft(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }

        /// <summary>
        /// Reads UInt32 in little-endian byte order
        /// </summary>
        private static uint ReadUInt32LE(byte[] buffer, int offset)
        {
            if (buffer is null)
            {
                return default;
            }

            return buffer[offset] | ((uint)buffer[offset + 1] << 8) | ((uint)buffer[offset + 2] << 16) | ((uint)buffer[offset + 3] << 24);
        }

        /// <summary>
        /// Writes UInt32 in little-endian byte order.
        /// </summary>
        private static void WriteUInt32LE(byte[] buffer, int offset, uint value)
        {
            if (buffer is null)
            {
                return;
            }

            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
        }
    }
}
