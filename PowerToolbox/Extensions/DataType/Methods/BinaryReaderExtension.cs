﻿using System;
using System.IO;
using System.Text;

namespace PowerToolbox.Extensions.DataType.Methods
{
    /// <summary>
    /// BinaryReader 类的扩展方法
    /// </summary>
    public static class BinaryReaderExtension
    {
        public static void ExpectUInt16(this BinaryReader reader, ushort expectedValue)
        {
            if (!reader.ReadUInt16().Equals(expectedValue))
            {
                throw new InvalidDataException("Unexpected value read.");
            }
        }

        public static void ExpectUInt32(this BinaryReader reader, uint expectedValue)
        {
            if (!reader.ReadUInt32().Equals(expectedValue))
            {
                throw new InvalidDataException("Unexpected value read.");
            }
        }

        public static void ExpectString(this BinaryReader reader, string str)
        {
            if (!string.Equals(new string(reader.ReadChars(str.Length)), str))
            {
                throw new InvalidDataException("Unexpected value read.");
            }
        }

        public static string ReadString(this BinaryReader reader, Encoding encoding, int length)
        {
            using BinaryReader binaryReader = new(reader.BaseStream, encoding, true);
            return new string(binaryReader.ReadChars(length));
        }

        public static string ReadNullTerminatedString(this BinaryReader reader, Encoding encoding)
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
