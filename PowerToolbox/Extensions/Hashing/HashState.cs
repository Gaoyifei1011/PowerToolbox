namespace PowerToolbox.Extensions.Hashing
{
    internal sealed class HashState
    {
        internal uint A;
        internal uint B;
        internal uint C;
        internal uint D;

        internal HashState()
        {
            Initialize();
        }

        internal void Initialize()
        {
            A = 0x67452301;
            B = 0xefcdab89;
            C = 0x98badcfe;
            D = 0x10325476;
        }

        internal byte[] ToByteArray()
        {
            byte[] result = new byte[16];

            WriteUInt32LE(result, 0, A);
            WriteUInt32LE(result, 4, B);
            WriteUInt32LE(result, 8, C);
            WriteUInt32LE(result, 12, D);

            return result;
        }

        private static void WriteUInt32LE(byte[] output, int offset, uint value)
        {
            if (output is null)
            {
                return;
            }

            output[offset] = (byte)value;
            output[offset + 1] = (byte)(value >> 8);
            output[offset + 2] = (byte)(value >> 16);
            output[offset + 3] = (byte)(value >> 24);
        }
    }
}
