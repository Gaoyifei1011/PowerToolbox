namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class HierarchicalSchemaVersion
    {
        internal ushort MajorVersion { get; set; }

        internal ushort MinorVersion { get; set; }

        internal uint Checksum { get; set; }

        internal uint NumScopes { get; set; }

        internal uint NumItems { get; set; }
    }
}
