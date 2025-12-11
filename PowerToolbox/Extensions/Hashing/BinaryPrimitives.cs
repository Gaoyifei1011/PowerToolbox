namespace PowerToolbox.Extensions.Hashing
{
    public static class BinaryPrimitives
    {
        public static uint ReverseEndianness(uint value)
        {
            // 这利用了 JIT 可以检测 ROL32 / ROR32 模式并输出正确内置函数的事实
            //
            // Input: value = [ ww xx yy zz ]
            //
            // First line generates : [ ww xx yy zz ]
            //                      & [ 00 FF 00 FF ]
            //                      = [ 00 xx 00 zz ]
            //             ROR32(8) = [ zz 00 xx 00 ]
            //
            // Second line generates: [ ww xx yy zz ]
            //                      & [ FF 00 FF 00 ]
            //                      = [ ww 00 yy 00 ]
            //             ROL32(8) = [ 00 yy 00 ww ]
            //
            //                (sum) = [ zz yy xx ww ]
            //
            // 测试显示，如果在执行 ROL / ROR 之前进行 AND，吞吐量会提高

            return BitOperations.RotateRight(value & 0x00FF00FFu, 8) // xx zz
                + BitOperations.RotateLeft(value & 0xFF00FF00u, 8); // ww yy
        }

        public static ulong ReverseEndianness(ulong value)
        {
            // 对 32 位值的操作比对 64 位值的操作吞吐量更高，因此需要分解
            return ((ulong)ReverseEndianness((uint)value) << 32) + ReverseEndianness((uint)(value >> 32));
        }
    }
}
