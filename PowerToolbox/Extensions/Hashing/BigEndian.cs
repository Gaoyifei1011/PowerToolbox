using System;

namespace PowerToolbox.Extensions.Hashing
{
    public static class BigEndian
    {
        public static byte[] ToByteArray(uint[] input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            byte[] result = new byte[input.Length * 4];
            Copy(input, 0, result, 0, input.Length);
            return result;
        }

        public static void Copy(uint[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            if (src is null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            if (dst is null)
            {
                throw new ArgumentNullException(nameof(dst));
            }

            if (srcOffset < 0 || count < 0 || srcOffset > src.Length - count)
            {
                throw new ArgumentOutOfRangeException(nameof(srcOffset));
            }

            if (dstOffset < 0 || dstOffset > dst.Length - count * 4)
            {
                throw new ArgumentOutOfRangeException(nameof(dstOffset));
            }

            for (int i = 0; i < count; i++)
            {
                Copy(src[srcOffset + i], dst, dstOffset + i * 4);
            }
        }

        public static void Copy(byte[] src, int srcOffset, uint[] dst, int dstOffset, int count)
        {
            if (src is null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            if (dst is null)
            {
                throw new ArgumentNullException(nameof(dst));
            }

            if (srcOffset < 0 || count < 0 || srcOffset > src.Length - count * 4)
            {
                throw new ArgumentOutOfRangeException(nameof(srcOffset));
            }

            if (dstOffset < 0 || dstOffset > dst.Length - count)
            {
                throw new ArgumentOutOfRangeException(nameof(dstOffset));
            }

            for (int i = 0; i < count; i++)
            {
                dst[dstOffset + i] = ToUInt32(src, srcOffset + i * 4);
            }
        }

        public static void Copy(uint input, byte[] bytes, int offset)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (offset < 0 || offset > bytes.Length - 4)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            bytes[offset] = (byte)(input >> 24);
            bytes[offset + 1] = (byte)(input >> 16);
            bytes[offset + 2] = (byte)(input >> 8);
            bytes[offset + 3] = (byte)input;
        }

        public static uint ToUInt32(byte[] bytes, int offset)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            return offset < 0 || offset > bytes.Length - 4 ? throw new ArgumentOutOfRangeException(nameof(offset))
                : bytes[offset + 3]
                | ((uint)bytes[offset + 2] << 8)
                | ((uint)bytes[offset + 1] << 16)
                | ((uint)bytes[offset] << 24);
        }
    }
}
