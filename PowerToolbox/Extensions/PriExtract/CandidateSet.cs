using System.Collections.Generic;

namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class CandidateSet
    {
        internal (int SchemaSectionIndex, int ResourceMapItemIndex) ResourceMapSectionAndIndex { get; set; }

        internal ushort DecisionIndex { get; set; }

        internal IReadOnlyList<Candidate> CandidatesList { get; set; }
    }
}
