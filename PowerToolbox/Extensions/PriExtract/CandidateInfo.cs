using PowerToolbox.Extensions.DataType.Enums;

namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class CandidateInfo
    {
        internal byte Type { get; set; }

        internal ResourceValueType ResourceValueType { get; set; }

        internal ushort SourceFileIndex { get; set; }

        internal ushort DataItemIndex { get; set; }
        internal ushort DataItemSection { get; set; }

        internal ushort DataLength { get; set; }

        internal uint DataOffset { get; set; }
    }
}
