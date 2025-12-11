namespace PowerToolbox.Extensions.Hashing
{
    public static class Blake3Constants
    {
        public const uint OutLen = 32;
        public const int KeyLen = 32;
        public const int BlockLen = 64;
        public const int ChunkLen = 1024;
        public const uint ChunkStart = 1 << 0;
        public const uint ChunkEnd = 1 << 1;
        public const uint Parent = 1 << 2;
        public const uint Root = 1 << 3;
        public const uint KeyedHash = 1 << 4;
        public const uint DeriveKyContext = 1 << 5;
        public const uint DeriveKeyMaterial = 1 << 6;

        public static readonly uint[] Iv =
        [
            0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A, 0x510E527F,
            0x9B05688C, 0x1F83D9AB, 0x5BE0CD19
        ];

        public static readonly uint[] MsgPermutation = [2, 6, 3, 10, 7, 0, 4, 13, 1, 11, 12, 5, 9, 14, 15, 8];
    }
}
