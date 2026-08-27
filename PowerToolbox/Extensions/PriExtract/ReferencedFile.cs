using System.Collections.Generic;

namespace PowerToolbox.Extensions.PriExtract
{
    internal sealed class ReferencedFileOrFolder
    {
        internal string Name { get; set; }

        internal ReferencedFileOrFolder Parent { get; set; }

        internal IReadOnlyList<ReferencedFileOrFolder> Children { get; set; }
    }
}
