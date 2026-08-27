namespace PowerToolbox.Extensions.Hashing
{
    internal static class Blake3Constants
    {
        internal const uint OutLen = 32;
        internal const int KeyLen = 32;
        internal const int BlockLen = 64;
        internal const int ChunkLen = 1024;
        internal const uint ChunkStart = 1 << 0;
        internal const uint ChunkEnd = 1 << 1;
        internal const uint Parent = 1 << 2;
        internal const uint Root = 1 << 3;
        internal const uint KeyedHash = 1 << 4;
        internal const uint DeriveKyContext = 1 << 5;
        internal const uint DeriveKeyMaterial = 1 << 6;

        internal static readonly uint[] Iv =
        [
            0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A, 0x510E527F,
            0x9B05688C, 0x1F83D9AB, 0x5BE0CD19
        ];

        internal static readonly uint[] MsgPermutation = [2, 6, 3, 10, 7, 0, 4, 13, 1, 11, 12, 5, 9, 14, 15, 8];
    }
}
