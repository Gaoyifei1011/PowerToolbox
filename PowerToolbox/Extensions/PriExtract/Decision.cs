using System.Collections.Generic;

namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class Decision
    {
        internal ushort Index { get; set; }

        internal IReadOnlyList<QualifierSet> QualifierSetsList { get; set; }
    }
}
