using System.Collections.Generic;

namespace PowerToolbox.Extensions.PriExtract
{
    public sealed class ReferencedFileOrFolder
    {
        public string Name { get; set; }

        public ReferencedFileOrFolder Parent { get; set; }

        public IReadOnlyList<ReferencedFileOrFolder> Children { get; set; }
    }
}
