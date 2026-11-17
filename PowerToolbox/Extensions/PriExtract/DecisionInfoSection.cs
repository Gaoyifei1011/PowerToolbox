using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Extensions.DataType.Methods;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class DecisionInfoSection
    {
        internal uint SectionQualifier { get; private set; }

        internal uint Flags { get; private set; }

        internal uint SectionFlags { get; private set; }

        internal uint SectionLength { get; private set; }

        internal IReadOnlyList<Decision> DecisionList { get; private set; }

        internal IReadOnlyList<QualifierSet> QualifierSetList { get; private set; }

        internal IReadOnlyList<Qualifier> QualifierList { get; private set; }

        internal DecisionInfoSection(string sectionIdentifier, BinaryReader binaryReader)
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

            ushort distinctQualifierNum = binaryReader.ReadUInt16();
            ushort qualifierNum = binaryReader.ReadUInt16();
            ushort qualifierSetNum = binaryReader.ReadUInt16();
            ushort decisionNum = binaryReader.ReadUInt16();
            ushort indexTableEntryNum = binaryReader.ReadUInt16();
            ushort totalDataLength = binaryReader.ReadUInt16();

            List<DecisionInfo> decisionInfoList = [with(decisionNum)];
            for (int i = 0; i < decisionNum; i++)
            {
                ushort firstQualifierSetIndexIndex = binaryReader.ReadUInt16();
                ushort numQualifierSetsInDecision = binaryReader.ReadUInt16();
                decisionInfoList.Add(new()
                {
                    FirstQualifierSetIndexIndex = firstQualifierSetIndexIndex,
                    NumQualifierSetsInDecision = numQualifierSetsInDecision,
                });
            }

            List<QualifierSetInfo> qualifierSetInfoList = [with(qualifierSetNum)];
            for (int i = 0; i < qualifierSetNum; i++)
            {
                ushort firstQualifierIndexIndex = binaryReader.ReadUInt16();
                ushort numQualifiersInSet = binaryReader.ReadUInt16();
                qualifierSetInfoList.Add(new()
                {
                    FirstQualifierIndexIndex = firstQualifierIndexIndex,
                    NumQualifiersInSet = numQualifiersInSet,
                });
            }

            List<QualifierInfo> qualifierInfoList = [with(qualifierNum)];
            for (int i = 0; i < qualifierNum; i++)
            {
                ushort index = binaryReader.ReadUInt16();
                ushort priority = binaryReader.ReadUInt16();
                ushort fallbackScore = binaryReader.ReadUInt16();
                binaryReader.ExpectUInt16(0);
                qualifierInfoList.Add(new()
                {
                    Index = index,
                    Priority = priority,
                    FallbackScore = fallbackScore
                });
            }

            List<DistinctQualifierInfo> distinctQualifierInfoList = [with(distinctQualifierNum)];
            for (int i = 0; i < distinctQualifierNum; i++)
            {
                binaryReader.ReadUInt16();
                QualifierType qualifierType = (QualifierType)binaryReader.ReadUInt16();
                binaryReader.ReadUInt16();
                binaryReader.ReadUInt16();
                uint operandValueOffset = binaryReader.ReadUInt32();
                distinctQualifierInfoList.Add(new()
                {
                    QualifierType = qualifierType,
                    OperandValueOffset = operandValueOffset,
                });
            }

            ushort[] indexTableArray = new ushort[indexTableEntryNum];

            for (int i = 0; i < indexTableEntryNum; i++)
            {
                indexTableArray[i] = binaryReader.ReadUInt16();
            }

            long dataStartOffset = binaryReader.BaseStream.Position;
            List<Qualifier> qualifierList = [with(qualifierNum)];

            for (int i = 0; i < qualifierNum; i++)
            {
                DistinctQualifierInfo distinctQualifierInfo = distinctQualifierInfoList[qualifierInfoList[i].Index];
                binaryReader.BaseStream.Seek(dataStartOffset + distinctQualifierInfo.OperandValueOffset * 2, SeekOrigin.Begin);
                string value = binaryReader.ReadNullTerminatedString(Encoding.Unicode);

                qualifierList.Add(new()
                {
                    Index = (ushort)i,
                    Type = distinctQualifierInfo.QualifierType,
                    Priority = qualifierInfoList[i].Priority,
                    FallbackScore = qualifierInfoList[i].FallbackScore,
                    Value = value
                });
            }

            QualifierList = qualifierList;
            List<QualifierSet> qualifierSetList = [with(qualifierSetNum)];

            for (int i = 0; i < qualifierSetNum; i++)
            {
                List<Qualifier> qualifiersInSet = [with(qualifierSetInfoList[i].NumQualifiersInSet)];

                for (int j = 0; j < qualifierSetInfoList[i].NumQualifiersInSet; j++)
                {
                    qualifiersInSet.Add(qualifierList[indexTableArray[qualifierSetInfoList[i].FirstQualifierIndexIndex + j]]);
                }

                qualifierSetList.Add(new()
                {
                    Index = (ushort)i,
                    QualifierList = qualifiersInSet
                });
            }

            QualifierSetList = qualifierSetList;
            List<Decision> decisionList = [with(decisionNum)];

            for (int i = 0; i < decisionNum; i++)
            {
                List<QualifierSet> qualifierSetsInDecisionList = [with(decisionInfoList[i].NumQualifierSetsInDecision)];

                for (int j = 0; j < decisionInfoList[i].NumQualifierSetsInDecision; j++)
                {
                    qualifierSetsInDecisionList.Add(qualifierSetList[indexTableArray[decisionInfoList[i].FirstQualifierSetIndexIndex + j]]);
                }

                decisionList.Add(new()
                {
                    Index = (ushort)i,
                    QualifierSetList = qualifierSetsInDecisionList
                });
            }

            DecisionList = decisionList;
        }
    }
}
