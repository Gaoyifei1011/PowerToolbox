using PowerToolbox.Extensions.DataType.Methods;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class ReferencedFileSection
    {
        internal uint SectionQualifier { get; private set; }

        internal uint Flags { get; private set; }

        internal uint SectionFlags { get; private set; }

        internal uint SectionLength { get; private set; }

        internal IReadOnlyList<ReferencedFileOrFolder> ReferencedFileList { get; private set; }

        internal ReferencedFileSection(string sectionIdentifier, BinaryReader binaryReader)
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

            ushort rootNum = binaryReader.ReadUInt16();
            ushort folderNum = binaryReader.ReadUInt16();
            ushort fileNum = binaryReader.ReadUInt16();
            binaryReader.ExpectUInt16(0);
            uint totalDataLength = binaryReader.ReadUInt32();

            List<FolderInfo> folderInfoList = [with(folderNum)];

            for (int i = 0; i < folderNum; i++)
            {
                binaryReader.ExpectUInt16(0);
                ushort parentFolder = binaryReader.ReadUInt16();
                ushort numFoldersInFolder = binaryReader.ReadUInt16();
                ushort firstFolderInFolder = binaryReader.ReadUInt16();
                ushort numFilesInFolder = binaryReader.ReadUInt16();
                ushort firstFileInFolder = binaryReader.ReadUInt16();
                ushort folderNameLength = binaryReader.ReadUInt16();
                ushort fullPathLength = binaryReader.ReadUInt16();
                uint folderNameOffset = binaryReader.ReadUInt32();
                folderInfoList.Add(new()
                {
                    ParentFolder = parentFolder,
                    NumFilesInFolder = numFilesInFolder,
                    FirstFileInFolder = firstFileInFolder,
                    NumFoldersInFolder = numFoldersInFolder,
                    FirstFolderInFolder = firstFolderInFolder,
                    FolderNameLength = folderNameLength,
                    FullPathLength = fullPathLength,
                    FolderNameOffset = folderNameOffset,
                });
            }

            List<FileInfo> fileInfoList = [with(fileNum)];

            for (int i = 0; i < fileNum; i++)
            {
                binaryReader.ReadUInt16();
                ushort parentFolder = binaryReader.ReadUInt16();
                ushort fullPathLength = binaryReader.ReadUInt16();
                ushort fileNameLength = binaryReader.ReadUInt16();
                uint fileNameOffset = binaryReader.ReadUInt32();
                fileInfoList.Add(new()
                {
                    ParentFolder = parentFolder,
                    FullPathLength = fileNameLength,
                    FileNameLength = fileNameLength,
                    FileNameOffset = fileNameOffset,
                });
            }

            long dataStartPosition = binaryReader.BaseStream.Position;
            List<ReferencedFileOrFolder> referencedFolderList = [with(folderNum)];

            for (int i = 0; i < folderNum; i++)
            {
                binaryReader.BaseStream.Seek(dataStartPosition + folderInfoList[i].FolderNameOffset * 2, SeekOrigin.Begin);
                string name = binaryReader.ReadString(Encoding.Unicode, folderInfoList[i].FolderNameLength);
                referencedFolderList.Add(new()
                {
                    Parent = null,
                    Name = name,
                    ChildrenList = null
                });
            }

            for (int i = 0; i < folderNum; i++)
            {
                if (folderInfoList[i].ParentFolder is not 0xFFFF)
                {
                    referencedFolderList[i].Parent = referencedFolderList[folderInfoList[i].ParentFolder];
                }
            }

            List<ReferencedFileOrFolder> referencedFileList = [with(fileNum)];

            for (int i = 0; i < fileNum; i++)
            {
                binaryReader.BaseStream.Seek(dataStartPosition + fileInfoList[i].FileNameOffset * 2, SeekOrigin.Begin);
                string name = binaryReader.ReadString(Encoding.Unicode, fileInfoList[i].FileNameLength);
                ReferencedFileOrFolder parentFolder = fileInfoList[i].ParentFolder is not 0xFFFF ? referencedFolderList[fileInfoList[i].ParentFolder] : null;
                referencedFileList.Add(new()
                {
                    Parent = parentFolder,
                    Name = name,
                });
            }

            for (int i = 0; i < folderNum; i++)
            {
                List<ReferencedFileOrFolder> childrenList = [with(folderInfoList[i].NumFoldersInFolder + folderInfoList[i].NumFilesInFolder)];

                for (int j = 0; j < folderInfoList[i].NumFoldersInFolder; j++)
                {
                    childrenList.Add(referencedFolderList[folderInfoList[i].FirstFolderInFolder + j]);
                }

                for (int j = 0; j < folderInfoList[i].NumFilesInFolder; j++)
                {
                    childrenList.Add(referencedFileList[folderInfoList[i].FirstFileInFolder + j]);
                }

                referencedFolderList[i].ChildrenList = childrenList;
            }

            ReferencedFileList = referencedFileList;
        }
    }
}
