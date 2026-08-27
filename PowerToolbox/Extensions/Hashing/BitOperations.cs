namespace PowerToolbox.Extensions.Hashing
{
    internal static class BitOperations
    {
        internal static uint RotateLeft(uint value, int offset)
        {
            return (value << offset) | (value >> (32 - offset));
        }

        internal static ulong RotateLeft(ulong value, int offset)
        {
            return (value << offset) | (value >> (64 - offset));
        }

        /// <summary>
        /// 将指定的值右旋转指定的位数
        /// 类似于 x86 指令ROR
        /// </summary>
        internal static uint RotateRight(uint value, int offset)
        {
            return (value >> offset) | (value << (32 - offset));
        }
    }
}
