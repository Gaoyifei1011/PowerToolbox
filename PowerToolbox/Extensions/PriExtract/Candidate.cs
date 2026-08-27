using PowerToolbox.Extensions.DataType.Enums;

namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class Candidate
    {
        internal ushort QualifierSet { get; set; }

        internal ResourceValueType Type { get; set; }

        internal int? SourceFileIndex { get; set; }

        internal (ushort DataItemSection, ushort DataItemIndex) DataItemSectionAndIndex { get; set; }

        internal ByteSpan Data { get; set; }
    }
}
