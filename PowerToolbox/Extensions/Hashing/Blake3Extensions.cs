using System;

namespace PowerToolbox.Extensions.Hashing
{
    internal static class Blake3Extensions
    {
        internal static uint RotateRight(this uint self, int count)
        {
            return (self >> count) | (self << (32 - count));
        }

        internal static T[] Slice<T>(this T[] self, int index, int length)
        {
            if (self is null)
            {
                return default;
            }

            T[] slice = new T[length];
            Array.Copy(self, index, slice, 0, length);
            return slice;
        }

        internal static uint FromLeBytes(byte[] bytes)
        {
            if (bytes is null)
            {
                return default;
            }

            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt32(bytes, 0);
            }

            return (uint)(bytes[3] << 24) | (uint)(bytes[2] << 16) | (uint)(bytes[1] << 8) | bytes[0];
        }

        internal static byte[] ToLeBytes(this uint self)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.GetBytes(self);
            }

            return
            [
                (byte) ((self & 0xff000000) >> 24),
                (byte) ((self & 0x00ff0000) >> 16),
                (byte) ((self & 0x0000ff00) >> 8), (byte) (self & 0x000000ff)
            ];
        }
    }
}
