using System.Collections.Generic;

namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class QualifierSet
    {
        internal ushort Index { get; set; }

        internal IReadOnlyList<Qualifier> QualifiersList { get; set; }
    }
}
