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

        internal IReadOnlyList<int> HierarchicalSchemaSectionsList { get; private set; }

        internal IReadOnlyList<int> DecisionInfoSectionsList { get; private set; }

        internal IReadOnlyList<int> ResourceMapSectionsList { get; private set; }

        internal IReadOnlyList<int> ReferencedFileSectionsList { get; private set; }

        internal IReadOnlyList<int> DataItemSectionsList { get; private set; }

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
            ushort numHierarchicalSchemaSections = binaryReader.ReadUInt16();
            ushort numDecisionInfoSections = binaryReader.ReadUInt16();
            ushort numResourceMapSections = binaryReader.ReadUInt16();
            ushort primaryResourceMapSection = binaryReader.ReadUInt16();

            PrimaryResourceMapSection = primaryResourceMapSection is not 0xFFFF ? primaryResourceMapSection : null;

            ushort numReferencedFileSections = binaryReader.ReadUInt16();
            ushort numDataItemSections = binaryReader.ReadUInt16();
            binaryReader.ExpectUInt16(0);

            List<int> hierarchicalSchemaSectionsList = new(numHierarchicalSchemaSections);

            for (int index = 0; index < numHierarchicalSchemaSections; index++)
            {
                hierarchicalSchemaSectionsList.Add(binaryReader.ReadUInt16());
            }

            HierarchicalSchemaSectionsList = hierarchicalSchemaSectionsList;

            List<int> decisionInfoSectionsList = new(numHierarchicalSchemaSections);

            for (int index = 0; index < numHierarchicalSchemaSections; index++)
            {
                decisionInfoSectionsList.Add(binaryReader.ReadUInt16());
            }

            DecisionInfoSectionsList = decisionInfoSectionsList;

            List<int> resourceMapSectionsList = new(numHierarchicalSchemaSections);

            for (int index = 0; index < numResourceMapSections; index++)
            {
                resourceMapSectionsList.Add(binaryReader.ReadUInt16());
            }

            ResourceMapSectionsList = resourceMapSectionsList;

            List<int> referencedFileSectionsList = new(numHierarchicalSchemaSections);

            for (int index = 0; index < numReferencedFileSections; index++)
            {
                referencedFileSectionsList.Add(binaryReader.ReadUInt16());
            }

            ReferencedFileSectionsList = referencedFileSectionsList;

            List<int> dataItemSectionsList = new(numDataItemSections);

            for (int index = 0; index < numDataItemSections; index++)
            {
                dataItemSectionsList.Add(binaryReader.ReadUInt16());
            }

            DataItemSectionsList = dataItemSectionsList;
        }
    }
}
