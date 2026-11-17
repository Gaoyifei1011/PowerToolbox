using System.Collections.Generic;

namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class Decision
    {
        internal ushort Index { get; set; }

        internal IReadOnlyList<QualifierSet> QualifierSetList { get; set; }
    }
}
