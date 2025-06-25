﻿using PowerToolbox.Extensions.DataType.Enums;

namespace PowerToolbox.Extensions.PriExtract
{
    public sealed class Qualifier
    {
        public ushort Index { get; set; }

        public QualifierType Type { get; set; }

        public ushort Priority { get; set; }

        public float FallbackScore { get; set; }

        public string Value { get; set; }
    }
}
