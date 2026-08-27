namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class FileInfo
    {
        internal ushort ParentFolder { get; set; }

        internal ushort FullPathLength { get; set; }

        internal ushort FileNameLength { get; set; }

        internal uint FileNameOffset { get; set; }
    }
}
