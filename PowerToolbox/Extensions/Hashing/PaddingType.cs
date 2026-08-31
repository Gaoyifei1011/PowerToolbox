namespace PowerToolbox.Extensions.Hashing
{
    internal enum PaddingType
    {
        Custom,
        OneZeroFillAnd8BytesMessageLengthLittleEndian,
        OneZeroFillAnd8BytesMessageLengthBigEndian,
        OneZeroFillAnd16BytesMessageLengthBigEndian
    }
}
