using System;

namespace PowerToolbox.Extensions.Hashing
{
    internal class Blake3Output
    {
        internal uint[] InputChainingValue;
        internal uint[] BlockWords;
        internal ulong Counter;
        internal uint BlockLen;
        internal uint Flags;

        internal uint[] ChainingValue()
        {
            return Blake3Functions.First8Words(Blake3Functions.Compress(InputChainingValue, BlockWords, Counter, BlockLen, Flags));
        }

        internal void RootOutputBytes(ref byte[] outSlice)
        {
            ulong outputBlockCounter = 0;
            for (int i = 0; i < outSlice.Length; i += 2 * (int)Blake3Constants.OutLen)
            {
                uint[] words = Blake3Functions.Compress(InputChainingValue, BlockWords, outputBlockCounter, BlockLen, Flags | Blake3Constants.Root);
                for (int j = 0, k = 0; j < words.Length && k < outSlice.Length; j++, k += 4)
                {
                    Array.Copy(words[j].ToLeBytes(), 0, outSlice, i + k, 4);
                }
                outputBlockCounter++;
            }
        }
    }
}
