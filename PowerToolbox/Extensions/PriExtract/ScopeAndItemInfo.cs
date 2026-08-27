namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class ScopeAndItemInfo
    {
        internal ushort Parent { get; set; }

        internal ushort FullPathLength { get; set; }

        internal bool IsScope { get; set; }

        internal bool NameInAscii { get; set; }

        internal uint NameOffset { get; set; }

        internal ushort Index { get; set; }
    }
}
