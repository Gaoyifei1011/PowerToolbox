using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// EDON-R-256 哈希校验实现
    /// EDON-R-256 Hash Algorithm
    /// Pure C# implementation compatible with .NET Framework
    /// Based on the EDON-R reference implementation used by RHash
    /// Output size: 256 bits / 32 bytes
    /// Block size: 512 bits / 64 bytes
    /// </summary>
    internal sealed class EdonR256 : HashAlgorithm
    {
        private const int BlockSize = 64;
        private const int HashSizeBytes = 32;
        private readonly uint[] _hash = new uint[16];
        private readonly byte[] _buffer = new byte[BlockSize];
        private int _bufferLength;
        private ulong _length;

        internal EdonR256()
        {
            HashSizeValue = 256;
            Initialize();
        }

        /// <summary>
        /// Reset the hash algorithm
        /// </summary>
        public override void Initialize()
        {
            _hash[0] = 0x40414243;
            _hash[1] = 0x44454647;
            _hash[2] = 0x48494A4B;
            _hash[3] = 0x4C4D4E4F;
            _hash[4] = 0x50515253;
            _hash[5] = 0x54555657;
            _hash[6] = 0x58595A5B;
            _hash[7] = 0x5C5D5E5F;
            _hash[8] = 0x60616263;
            _hash[9] = 0x64656667;
            _hash[10] = 0x68696A6B;
            _hash[11] = 0x6C6D6E6F;
            _hash[12] = 0x70717273;
            _hash[13] = 0x74757677;
            _hash[14] = 0x78797A7B;
            _hash[15] = 0x7C7D7E7F;
            Array.Clear(_buffer, 0, _buffer.Length);
            _bufferLength = 0;
            _length = 0;
        }

        /// <summary>
        /// Process input data.
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

            int offset = ibStart;
            int length = cbSize;

            _length += (ulong)length;

            // Fill existing partial block
            if (_bufferLength is not 0)
            {
                int needed = BlockSize - _bufferLength;

                if (length < needed)
                {
                    Buffer.BlockCopy(array, offset, _buffer, _bufferLength, length);
                    _bufferLength += length;
                    return;
                }

                Buffer.BlockCopy(array, offset, _buffer, _bufferLength, needed);
                ProcessBlock(_buffer, 0);
                _bufferLength = 0;
                offset += needed;
                length -= needed;
            }

            // Process complete blocks directly
            while (length >= BlockSize)
            {
                ProcessBlock(array, offset);
                offset += BlockSize;
                length -= BlockSize;
            }

            // Save remaining bytes
            if (length > 0)
            {
                Buffer.BlockCopy(array, offset, _buffer, 0, length);
                _bufferLength = length;
            }
        }

        /// <summary>
        /// Finalize the hash
        /// </summary>
        protected override byte[] HashFinal()
        {
            ulong messageLength = _length;

            // Append 0x80
            _buffer[_bufferLength++] = 0x80;

            // If there is not enough space for the 64-bit length, process this block first
            if (_bufferLength > 56)
            {
                Array.Clear(_buffer, _bufferLength, BlockSize - _bufferLength);
                ProcessBlock(_buffer, 0);
                _bufferLength = 0;
            }

            // Pad with zeros until byte 56
            Array.Clear(_buffer, _bufferLength, 56 - _bufferLength);

            // EDON-R-256 stores bit length as two little-endian uint32 values
            // message[14] = length << 3
            // message[15] = length >> 29
            ulong bitLength = messageLength << 3;
            WriteUInt32LE(_buffer, 56, (uint)bitLength);
            WriteUInt32LE(_buffer, 60, (uint)(messageLength >> 29));
            ProcessBlock(_buffer, 0);

            // EDON-R-256 output:
            // copy the last 32 bytes of the 64-byte internal state
            // The reference implementation uses little-endian output
            byte[] result = new byte[HashSizeBytes];

            for (int i = 0; i < 8; i++)
            {
                WriteUInt32LE(result, i * 4, _hash[i + 8]);
            }

            return result;
        }

        /// <summary>
        /// Process one 512-bit block
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

            // First row of quasigroup e-transformations
            Q256(block[15], block[14], block[13], block[12], block[11], block[10], block[9], block[8], block[0], block[1], block[2], block[3], block[4], block[5], block[6], block[7], p16);
            Q256(p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], block[8], block[9], block[10], block[11], block[12], block[13], block[14], block[15], p24);

            // Second row
            Q256(_hash[8], _hash[9], _hash[10], _hash[11], _hash[12], _hash[13], _hash[14], _hash[15], p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], p16);
            Q256(p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], p24[0], p24[1], p24[2], p24[3], p24[4], p24[5], p24[6], p24[7], p24);

            // Third row
            Q256(p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], _hash[0], _hash[1], _hash[2], _hash[3], _hash[4], _hash[5], _hash[6], _hash[7], p16);
            Q256(p24[0], p24[1], p24[2], p24[3], p24[4], p24[5], p24[6], p24[7], p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], p24);

            // Fourth row
            Q256(block[7], block[6], block[5], block[4], block[3], block[2], block[1], block[0], p16[0], p16[1], p16[2], p16[3], p16[4], p16[5], p16[6], p16[7], _hash, 0);
            Q256(_hash[0], _hash[1], _hash[2], _hash[3], _hash[4], _hash[5], _hash[6], _hash[7], p24[0], p24[1], p24[2], p24[3], p24[4], p24[5], p24[6], p24[7], _hash, 8);
        }

        /// <summary>
        /// Q256 quasigroup transformation
        /// </summary>
        private static void Q256(uint x0, uint x1, uint x2, uint x3, uint x4, uint x5, uint x6, uint x7, uint y0, uint y1, uint y2, uint y3, uint y4, uint y5, uint y6, uint y7, uint[] output)
        {
            Q256Core(x0, x1, x2, x3, x4, x5, x6, x7, y0, y1, y2, y3, y4, y5, y6, y7, output, 0);
        }

        /// <summary>
        /// Q256 quasigroup transformation with output offset
        /// </summary>
        private static void Q256(uint x0, uint x1, uint x2, uint x3, uint x4, uint x5, uint x6, uint x7, uint y0, uint y1, uint y2, uint y3, uint y4, uint y5, uint y6, uint y7, uint[] output, int outputOffset)
        {
            Q256Core(x0, x1, x2, x3, x4, x5, x6, x7, y0, y1, y2, y3, y4, y5, y6, y7, output, outputOffset);
        }

        private static void Q256Core(uint x0, uint x1, uint x2, uint x3, uint x4, uint x5, uint x6, uint x7, uint y0, uint y1, uint y2, uint y3, uint y4, uint y5, uint y6, uint y7, uint[] output, int outputOffset)
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

            uint t16;
            uint t17;
            uint t18;
            uint t19;

            uint t20;
            uint t21;
            uint t22;
            uint t23;

            // First Latin Square
            t8 = unchecked(x0 + x4);
            t9 = unchecked(x1 + x7);
            t12 = unchecked(t8 + t9);
            t10 = unchecked(x2 + x3);
            t11 = unchecked(x5 + x6);
            t13 = unchecked(t10 + t11);
            t0 = unchecked(0xAAAAAAAAu + t12 + x2);
            t1 = unchecked(t12 + x3);
            t1 = RotateLeft(t1, 5);
            t2 = unchecked(t12 + x6);
            t2 = RotateLeft(t2, 11);
            t3 = unchecked(t13 + x7);
            t3 = RotateLeft(t3, 13);
            t4 = unchecked(x1 + t13);
            t4 = RotateLeft(t4, 17);
            t5 = unchecked(t8 + t10 + x5);
            t5 = RotateLeft(t5, 19);
            t6 = unchecked(x0 + t9 + t11);
            t6 = RotateLeft(t6, 29);
            t7 = unchecked(t13 + x4);
            t7 = RotateLeft(t7, 31);
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
            uint t14 = t18 ^ t4;
            uint t15 = t0 ^ t17;

            // Second Orthogonal Latin Square
            t16 = unchecked(y0 + y1);
            t17 = unchecked(y2 + y5);
            t20 = unchecked(t16 + t17);
            t18 = unchecked(y3 + y4);
            t22 = unchecked(t16 + t18);
            t19 = unchecked(y6 + y7);
            t21 = unchecked(t18 + t19);
            t23 = unchecked(t17 + t19);
            t0 = unchecked(0x55555555u + t20 + y7);
            t1 = unchecked(t22 + y6);
            t1 = RotateLeft(t1, 3);
            t2 = unchecked(t20 + y3);
            t2 = RotateLeft(t2, 7);
            t3 = unchecked(y2 + t21);
            t3 = RotateLeft(t3, 11);
            t4 = unchecked(t22 + y5);
            t4 = RotateLeft(t4, 17);
            t5 = unchecked(t23 + y4);
            t5 = RotateLeft(t5, 19);
            t6 = unchecked(y1 + t23);
            t6 = RotateLeft(t6, 23);
            t7 = unchecked(y0 + t21);
            t7 = RotateLeft(t7, 29);
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
        /// Rotate left
        /// </summary>
        private static uint RotateLeft(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }

        /// <summary>
        /// Read UInt32 in little-endian format
        /// </summary>
        private static uint ReadUInt32LE(byte[] buffer, int offset)
        {
            return buffer[offset] | ((uint)buffer[offset + 1] << 8) | ((uint)buffer[offset + 2] << 16) | ((uint)buffer[offset + 3] << 24);
        }

        /// <summary>
        /// Write UInt32 in little-endian format
        /// </summary>
        private static void WriteUInt32LE(byte[] buffer, int offset, uint value)
        {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
        }
    }
}
