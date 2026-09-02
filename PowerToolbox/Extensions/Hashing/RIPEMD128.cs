namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// RIPEMD-128 校验实现
    /// RIPEMD-128 is a plug-in substitute for RIPEMD (or MD4 and MD5, for that matter) with a 128-bit result.
    /// </summary>
    internal sealed class RIPEMD128 : BlockHashAlgorithm
    {
        private static readonly uint[] Constants1 =
        [
            0x00000000,
            0x5a827999,
            0x6ed9eba1,
            0x8f1bbcdc
        ];

        private static readonly uint[] WordOrders1 =
        [
            00, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15,
            07, 04, 13, 01, 10, 06, 15, 03, 12, 00, 09, 05, 02, 14, 11, 08,
            03, 10, 14, 04, 09, 15, 08, 01, 02, 07, 00, 06, 13, 11, 05, 12,
            01, 09, 11, 10, 00, 08, 12, 04, 13, 03, 07, 15, 14, 05, 06, 02
        ];

        private static readonly int[] Shifts1 =
        [
            11, 14, 15, 12, 05, 08, 07, 09, 11, 13, 14, 15, 06, 07, 09, 08,
            07, 06, 08, 13, 11, 09, 07, 15, 07, 12, 15, 09, 11, 07, 13, 12,
            11, 13, 06, 07, 14, 09, 13, 15, 14, 08, 13, 06, 05, 12, 07, 05,
            11, 12, 14, 15, 14, 15, 09, 08, 09, 14, 05, 06, 08, 06, 05, 12
        ];

        private static readonly uint[] Constants2 =
        [
            0x50a28be6,
            0x5c4dd124,
            0x6d703ef3,
            0x00000000
        ];

        private static readonly uint[] WordOrders2 =
        [
            05, 14, 07, 00, 09, 02, 11, 04, 13, 06, 15, 08, 01, 10, 03, 12,
            06, 11, 03, 07, 00, 13, 05, 10, 14, 15, 08, 12, 04, 09, 01, 02,
            15, 05, 01, 03, 07, 14, 06, 09, 11, 08, 12, 02, 10, 00, 04, 13,
            08, 06, 04, 01, 03, 11, 15, 00, 05, 12, 02, 13, 09, 07, 10, 14
        ];

        private static readonly int[] Shifts2 =
        [
            08, 09, 09, 11, 13, 15, 15, 05, 07, 07, 08, 11, 14, 14, 12, 06,
            09, 13, 15, 07, 12, 08, 09, 11, 07, 07, 12, 07, 06, 15, 13, 11,
            09, 07, 15, 11, 08, 06, 06, 14, 12, 13, 05, 14, 13, 13, 07, 05,
            15, 05, 08, 11, 14, 14, 06, 14, 06, 09, 12, 09, 12, 05, 15, 08
        ];

        private readonly HashState state = new();

        private readonly uint[] buffer = new uint[16];

        internal RIPEMD128() : base(64)
        {
            HashSizeValue = 128;
            PaddingType = PaddingType.OneZeroFillAnd8BytesMessageLengthLittleEndian;
        }

        public override void Initialize()
        {
            base.Initialize();
            state.Initialize();
        }

        protected override void ProcessBlock(byte[] block, int offset)
        {
            if (block is null)
            {
                return;
            }

            for (int i = 0; i < 16; i++)
            {
                int index = offset + (i << 2);
                buffer[i] = (uint)(block[index] | (block[index + 1] << 8) | (block[index + 2] << 16) | (block[index + 3] << 24));
            }

            uint a1 = state.A;
            uint b1 = state.B;
            uint c1 = state.C;
            uint d1 = state.D;
            uint a2 = state.A;
            uint b2 = state.B;
            uint c2 = state.C;
            uint d2 = state.D;
            MDTransform1(ref a1, ref b1, ref c1, ref d1);
            MDTransform2(ref a2, ref b2, ref c2, ref d2);
            uint t = state.B + c1 + d2;
            state.B = state.C + d1 + a2;
            state.C = state.D + a1 + b2;
            state.D = state.A + b1 + c2;
            state.A = t;
        }

        protected override byte[] ProcessFinalBlock()
        {
            return state.ToByteArray();
        }

        private void MDTransform1(ref uint a, ref uint b, ref uint c, ref uint d)
        {
            // Round 1
            for (int ii = 0; ii < 16; ii += 4)
            {
                a += (b ^ c ^ d);
                a += Constants1[0] + buffer[WordOrders1[ii]];
                a = (a << Shifts1[ii]) | (a >> (32 - Shifts1[ii]));
                d += (a ^ b ^ c);
                d += Constants1[0] + buffer[WordOrders1[ii + 1]];
                d = (d << Shifts1[ii + 1]) | (d >> (32 - Shifts1[ii + 1]));
                c += (d ^ a ^ b);
                c += Constants1[0] + buffer[WordOrders1[ii + 2]];
                c = (c << Shifts1[ii + 2]) | (c >> (32 - Shifts1[ii + 2]));
                b += (c ^ d ^ a);
                b += Constants1[0] + buffer[WordOrders1[ii + 3]];
                b = (b << Shifts1[ii + 3]) | (b >> (32 - Shifts1[ii + 3]));
            }

            // Round 2
            for (int ii = 16; ii < 32; ii += 4)
            {
                a += (b & c) | (~b & d);
                a += Constants1[1] + buffer[WordOrders1[ii]];
                a = (a << Shifts1[ii]) | (a >> (32 - Shifts1[ii]));
                d += (a & b) | (~a & c);
                d += Constants1[1] + buffer[WordOrders1[ii + 1]];
                d = (d << Shifts1[ii + 1]) | (d >> (32 - Shifts1[ii + 1]));
                c += (d & a) | (~d & b);
                c += Constants1[1] + buffer[WordOrders1[ii + 2]];
                c = (c << Shifts1[ii + 2]) | (c >> (32 - Shifts1[ii + 2]));
                b += (c & d) | (~c & a);
                b += Constants1[1] + buffer[WordOrders1[ii + 3]];
                b = (b << Shifts1[ii + 3]) | (b >> (32 - Shifts1[ii + 3]));
            }

            // Round 3
            for (int ii = 32; ii < 48; ii += 4)
            {
                a += (b | ~c) ^ d;
                a += Constants1[2] + buffer[WordOrders1[ii]];
                a = (a << Shifts1[ii]) | (a >> (32 - Shifts1[ii]));
                d += (a | ~b) ^ c;
                d += Constants1[2] + buffer[WordOrders1[ii + 1]];
                d = (d << Shifts1[ii + 1]) | (d >> (32 - Shifts1[ii + 1]));
                c += (d | ~a) ^ b;
                c += Constants1[2] + buffer[WordOrders1[ii + 2]];
                c = (c << Shifts1[ii + 2]) | (c >> (32 - Shifts1[ii + 2]));
                b += (c | ~d) ^ a;
                b += Constants1[2] + buffer[WordOrders1[ii + 3]];
                b = (b << Shifts1[ii + 3]) | (b >> (32 - Shifts1[ii + 3]));
            }

            // Round 4
            for (int ii = 48; ii < 64; ii += 4)
            {
                a += (b & d) | (c & ~d);
                a += Constants1[3] + buffer[WordOrders1[ii]];
                a = (a << Shifts1[ii]) | (a >> (32 - Shifts1[ii]));
                d += (a & c) | (b & ~c);
                d += Constants1[3] + buffer[WordOrders1[ii + 1]];
                d = (d << Shifts1[ii + 1]) | (d >> (32 - Shifts1[ii + 1]));
                c += (d & b) | (a & ~b);
                c += Constants1[3] + buffer[WordOrders1[ii + 2]];
                c = (c << Shifts1[ii + 2]) | (c >> (32 - Shifts1[ii + 2]));
                b += (c & a) | (d & ~a);
                b += Constants1[3] + buffer[WordOrders1[ii + 3]];
                b = (b << Shifts1[ii + 3]) | (b >> (32 - Shifts1[ii + 3]));
            }
        }

        private void MDTransform2(ref uint a, ref uint b, ref uint c, ref uint d)
        {
            // Round 1
            for (int ii = 0; ii < 16; ii += 4)
            {
                a += (b & d) | (c & ~d);
                a += Constants2[0] + buffer[WordOrders2[ii]];
                a = (a << Shifts2[ii]) | (a >> (32 - Shifts2[ii]));
                d += (a & c) | (b & ~c);
                d += Constants2[0] + buffer[WordOrders2[ii + 1]];
                d = (d << Shifts2[ii + 1]) | (d >> (32 - Shifts2[ii + 1]));
                c += (d & b) | (a & ~b);
                c += Constants2[0] + buffer[WordOrders2[ii + 2]];
                c = (c << Shifts2[ii + 2]) | (c >> (32 - Shifts2[ii + 2]));
                b += (c & a) | (d & ~a);
                b += Constants2[0] + buffer[WordOrders2[ii + 3]];
                b = (b << Shifts2[ii + 3]) | (b >> (32 - Shifts2[ii + 3]));
            }

            // Round 2
            for (int ii = 16; ii < 32; ii += 4)
            {
                a += (b | ~c) ^ d;
                a += Constants2[1] + buffer[WordOrders2[ii]];
                a = (a << Shifts2[ii]) | (a >> (32 - Shifts2[ii]));
                d += (a | ~b) ^ c;
                d += Constants2[1] + buffer[WordOrders2[ii + 1]];
                d = (d << Shifts2[ii + 1]) | (d >> (32 - Shifts2[ii + 1]));
                c += (d | ~a) ^ b;
                c += Constants2[1] + buffer[WordOrders2[ii + 2]];
                c = (c << Shifts2[ii + 2]) | (c >> (32 - Shifts2[ii + 2]));
                b += (c | ~d) ^ a;
                b += Constants2[1] + buffer[WordOrders2[ii + 3]];
                b = (b << Shifts2[ii + 3]) | (b >> (32 - Shifts2[ii + 3]));
            }

            // Round 3
            for (int ii = 32; ii < 48; ii += 4)
            {
                a += (b & c) | (~b & d);
                a += Constants2[2] + buffer[WordOrders2[ii]];
                a = (a << Shifts2[ii]) | (a >> (32 - Shifts2[ii]));
                d += (a & b) | (~a & c);
                d += Constants2[2] + buffer[WordOrders2[ii + 1]];
                d = (d << Shifts2[ii + 1]) | (d >> (32 - Shifts2[ii + 1]));
                c += (d & a) | (~d & b);
                c += Constants2[2] + buffer[WordOrders2[ii + 2]];
                c = (c << Shifts2[ii + 2]) | (c >> (32 - Shifts2[ii + 2]));
                b += (c & d) | (~c & a);
                b += Constants2[2] + buffer[WordOrders2[ii + 3]];
                b = (b << Shifts2[ii + 3]) | (b >> (32 - Shifts2[ii + 3]));
            }

            // Round 4
            for (int ii = 48; ii < 64; ii += 4)
            {
                a += (b ^ c ^ d);
                a += Constants2[3] + buffer[WordOrders2[ii]];
                a = (a << Shifts2[ii]) | (a >> (32 - Shifts2[ii]));
                d += (a ^ b ^ c);
                d += Constants2[3] + buffer[WordOrders2[ii + 1]];
                d = (d << Shifts2[ii + 1]) | (d >> (32 - Shifts2[ii + 1]));
                c += (d ^ a ^ b);
                c += Constants2[3] + buffer[WordOrders2[ii + 2]];
                c = (c << Shifts2[ii + 2]) | (c >> (32 - Shifts2[ii + 2]));
                b += (c ^ d ^ a);
                b += Constants2[3] + buffer[WordOrders2[ii + 3]];
                b = (b << Shifts2[ii + 3]) | (b >> (32 - Shifts2[ii + 3]));
            }
        }
    }
}
