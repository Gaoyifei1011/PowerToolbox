namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class FolderInfo
    {
        internal ushort ParentFolder { get; set; }

        internal ushort NumFoldersInFolder { get; set; }

        internal ushort FirstFolderInFolder { get; set; }

        internal ushort NumFilesInFolder { get; set; }

        internal ushort FirstFileInFolder { get; set; }

        internal ushort FolderNameLength { get; set; }

        internal ushort FullPathLength { get; set; }

        internal uint FolderNameOffset { get; set; }
    }
}
