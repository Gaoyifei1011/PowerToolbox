namespace PowerToolbox.Extensions.Encrypt
{
    public readonly struct DWord
    {
        private readonly uint Value;

        public DWord(uint value) : this()
        {
            Value = value;
        }

        public DWord(ulong value) : this()
        {
            Value = (uint)value;
        }

        public DWord(byte[] buffer, int offset) : this()
        {
            Value = (uint)((buffer[offset + 3] << 24) | (buffer[offset + 2] << 16) | (buffer[offset + 1] << 8) | buffer[offset]);
        }

        public static implicit operator DWord(uint value)
        {
            return new DWord(value);
        }

        public static explicit operator uint(DWord value)
        {
            return value.Value;
        }

        public static bool operator <=(DWord left, DWord right)
        {
            return left.Value <= right.Value;
        }

        public static bool operator >=(DWord left, DWord right)
        {
            return left.Value >= right.Value;
        }

        public static DWord operator +(DWord left, DWord right)
        {
            return unchecked(left.Value + right.Value);
        }

        public static DWord operator &(DWord left, DWord right)
        {
            return left.Value & right.Value;
        }

        public static DWord operator |(DWord left, DWord right)
        {
            return left.Value | right.Value;
        }

        public static DWord operator ^(DWord left, DWord right)
        {
            return left.Value ^ right.Value;
        }

        public static DWord operator <<(DWord value, int shiftAmount)
        {
            return value.Value << shiftAmount;
        }

        public static DWord operator >>(DWord value, int shiftAmount)
        {
            return value.Value >> shiftAmount;
        }

        public static readonly DWord False = 0;
        public static readonly DWord True = 1;

        public DWord RotateLeft(int n)
        {
            return (Value << n) | (Value >> (32 - n));
        }

        public DWord MaskUpper()
        {
            return Value & 0xFFFF0000;
        }

        public DWord MaskLower()
        {
            return Value & 0x0000FFFF;
        }
    }
}
