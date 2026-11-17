using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Extensions.DataType.Methods;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class PriDescriptorSection
    {
        internal uint SectionQualifier { get; private set; }

        internal uint Flags { get; private set; }

        internal uint SectionFlags { get; private set; }

        internal uint SectionLength { get; private set; }

        internal PriDescriptorFlags PriFlags { get; private set; }

        internal IReadOnlyList<int> HierarchicalSchemaSectionList { get; private set; }

        internal IReadOnlyList<int> DecisionInfoSectionList { get; private set; }

        internal IReadOnlyList<int> ResourceMapSectionList { get; private set; }

        internal IReadOnlyList<int> ReferencedFileSectionList { get; private set; }

        internal IReadOnlyList<int> DataItemSectionList { get; private set; }

        internal int? PrimaryResourceMapSection { get; private set; }

        internal PriDescriptorSection(string sectionIdentifier, BinaryReader binaryReader)
        {
            if (!string.Equals(new(binaryReader.ReadChars(16)), sectionIdentifier))
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

            PriFlags = (PriDescriptorFlags)binaryReader.ReadUInt16();
            ushort includedFileListSection = binaryReader.ReadUInt16();
            binaryReader.ExpectUInt16(0);
            ushort hierarchicalSchemaSectionNum = binaryReader.ReadUInt16();
            ushort decisionInfoSectionNum = binaryReader.ReadUInt16();
            ushort resourceMapSectionNum = binaryReader.ReadUInt16();
            ushort primaryResourceMapSection = binaryReader.ReadUInt16();

            PrimaryResourceMapSection = primaryResourceMapSection is not 0xFFFF ? primaryResourceMapSection : null;
            ushort referencedFileSectionNum = binaryReader.ReadUInt16();
            ushort dataItemSectionNum = binaryReader.ReadUInt16();
            binaryReader.ExpectUInt16(0);

            List<int> hierarchicalSchemaSectionList = [with(hierarchicalSchemaSectionNum)];

            for (int index = 0; index < hierarchicalSchemaSectionNum; index++)
            {
                hierarchicalSchemaSectionList.Add(binaryReader.ReadUInt16());
            }

            HierarchicalSchemaSectionList = hierarchicalSchemaSectionList;
            List<int> decisionInfoSectionList = [with(hierarchicalSchemaSectionNum)];

            for (int index = 0; index < hierarchicalSchemaSectionNum; index++)
            {
                decisionInfoSectionList.Add(binaryReader.ReadUInt16());
            }

            DecisionInfoSectionList = decisionInfoSectionList;
            List<int> resourceMapSectionList = [with(hierarchicalSchemaSectionNum)];

            for (int index = 0; index < resourceMapSectionNum; index++)
            {
                resourceMapSectionList.Add(binaryReader.ReadUInt16());
            }

            ResourceMapSectionList = resourceMapSectionList;
            List<int> referencedFileSectionList = [with(hierarchicalSchemaSectionNum)];

            for (int index = 0; index < referencedFileSectionNum; index++)
            {
                referencedFileSectionList.Add(binaryReader.ReadUInt16());
            }

            ReferencedFileSectionList = referencedFileSectionList;
            List<int> dataItemSectionList = [with(dataItemSectionNum)];

            for (int index = 0; index < dataItemSectionNum; index++)
            {
                dataItemSectionList.Add(binaryReader.ReadUInt16());
            }

            DataItemSectionList = dataItemSectionList;
        }
    }
}
