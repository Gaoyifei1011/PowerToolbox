using PowerToolbox.Extensions.DataType.Enums;

namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class Qualifier
    {
        internal ushort Index { get; set; }

        internal QualifierType Type { get; set; }

        internal ushort Priority { get; set; }

        internal float FallbackScore { get; set; }

        internal string Value { get; set; }
    }
}
