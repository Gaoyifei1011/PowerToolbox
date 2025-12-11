using System;

namespace PowerToolbox.Extensions.Hashing
{
    public static class Blake3Extensions
    {
        public static uint RotateRight(this uint self, int count)
        {
            return (self >> count) | (self << (32 - count));
        }

        public static T[] Slice<T>(this T[] self, int index, int length)
        {
            T[] slice = new T[length];
            Array.Copy(self, index, slice, 0, length);
            return slice;
        }

        public static uint FromLeBytes(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt32(bytes, 0);
            }

            return (uint)(bytes[3] << 24) | (uint)(bytes[2] << 16) | (uint)(bytes[1] << 8) | bytes[0];
        }

        public static byte[] ToLeBytes(this uint self)
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
