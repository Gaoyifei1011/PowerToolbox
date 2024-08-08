﻿using System.Collections.Generic;

namespace WindowsTools.Extensions.PriExtract
{
    public sealed class ReferencedFileOrFolder
    {
        public string Name { get; set; }

        public ReferencedFileOrFolder Parent { get; set; }

        public IReadOnlyList<ReferencedFileOrFolder> Children { get; internal set; }
    }
}
