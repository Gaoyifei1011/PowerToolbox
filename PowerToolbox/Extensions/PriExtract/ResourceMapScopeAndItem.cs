using System.Collections.Generic;

namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class ResourceMapScopeAndItem
    {
        internal ushort Index { get; set; }

        internal ResourceMapScopeAndItem Parent { get; set; }

        internal string Name { get; set; }

        internal IReadOnlyList<ResourceMapScopeAndItem> Children { get; set; }
    }
}
