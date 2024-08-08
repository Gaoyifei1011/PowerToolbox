﻿using System;
using WindowsTools.Extensions.DataType.Enums;

namespace WindowsTools.Extensions.PriExtract
{
    public sealed class Candidate
    {
        public ushort QualifierSet { get; set; }

        public ResourceValueType Type { get; set; }

        public int? SourceFileIndex { get; set; }

        public Tuple<ushort, ushort> DataItemSectionAndIndex { get; set; }

        public ByteSpan Data { get; set; }
    }
}
