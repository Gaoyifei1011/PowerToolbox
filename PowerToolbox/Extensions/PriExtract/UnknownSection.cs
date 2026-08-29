using PowerToolbox.Extensions.DataType.Methods;
using System.IO;
using System.Text;

namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class UnknownSection
    {
        internal uint SectionQualifier { get; private set; }

        internal uint Flags { get; private set; }

        internal uint SectionFlags { get; private set; }

        internal uint SectionLength { get; private set; }

        internal byte[] SectionContent { get; private set; }

        internal UnknownSection(string sectionIdentifier, BinaryReader binaryReader)
        {
            if (!new string(binaryReader.ReadChars(16)).Equals(sectionIdentifier))
            {
                throw new InvalidDataException("Unexpected section identifier.");
            }

            SectionQualifier = binaryReader.ReadUInt32();
            Flags = binaryReader.ReadUInt16();
            SectionFlags = binaryReader.ReadUInt16();
            SectionLength = binaryReader.ReadUInt32();
            binaryReader.ExpectUInt32(0);
            binaryReader.BaseStream.Seek(SectionLength - 16 - 24, SeekOrigin.Current);
            binaryReader.ExpectUInt32(0xDEF5FADE);
            binaryReader.ExpectUInt32(SectionLength);
            binaryReader.BaseStream.Seek(-8 - (SectionLength - 16 - 24), SeekOrigin.Current);

            using SubStream subStream = new(binaryReader.BaseStream, binaryReader.BaseStream.Position, (int)SectionLength - 16 - 24);
            using BinaryReader subBinaryReader = new(subStream, Encoding.ASCII);

            int contentLength = (int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position);
            SectionContent = binaryReader.ReadBytes(contentLength);
        }
    }
}
