namespace PowerToolbox.Extensions.Hashing
{
    internal readonly struct Hash128(ulong low64, ulong high64)
    {
        internal readonly ulong Low64 = low64;
        internal readonly ulong High64 = high64;
    }
}
