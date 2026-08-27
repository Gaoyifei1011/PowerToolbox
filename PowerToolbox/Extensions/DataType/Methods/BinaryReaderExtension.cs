using System;
using System.IO;
using System.Text;

namespace PowerToolbox.Extensions.DataType.Methods
{
    /// <summary>
    /// BinaryReader 类的扩展方法
    /// </summary>
    internal static class BinaryReaderExtension
    {
        internal static void ExpectUInt16(this BinaryReader reader, ushort expectedValue)
        {
            if (!reader.ReadUInt16().Equals(expectedValue))
            {
                throw new InvalidDataException("Unexpected value read.");
            }
        }

        internal static void ExpectUInt32(this BinaryReader reader, uint expectedValue)
        {
            if (!reader.ReadUInt32().Equals(expectedValue))
            {
                throw new InvalidDataException("Unexpected value read.");
            }
        }

        internal static void ExpectString(this BinaryReader reader, string str)
        {
            if (!string.Equals(new(reader.ReadChars(str.Length)), str))
            {
                throw new InvalidDataException("Unexpected value read.");
            }
        }

        internal static string ReadString(this BinaryReader reader, Encoding encoding, int length)
        {
            using BinaryReader binaryReader = new(reader.BaseStream, encoding, true);
            return new(binaryReader.ReadChars(length));
        }

        internal static string ReadNullTerminatedString(this BinaryReader reader, Encoding encoding)
        {
            using BinaryReader binaryReader = new(reader.BaseStream, encoding, true);
            StringBuilder result = new();
            char c;
            while ((c = binaryReader.ReadChar()) is not '\0')
            {
                result.Append(c);
            }
            return Convert.ToString(result);
        }
    }
}
