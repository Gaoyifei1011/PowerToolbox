namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class TocEntry
    {
        internal string SectionIdentifier { get; set; }

        internal ushort Flags { get; set; }

        internal ushort SectionFlags { get; set; }

        internal uint SectionQualifier { get; set; }

        internal uint SectionOffset { get; set; }

        internal uint SectionLength { get; set; }
    }
}
