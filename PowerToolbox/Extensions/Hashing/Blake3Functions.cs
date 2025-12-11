using System;

namespace PowerToolbox.Extensions.Hashing
{
    public static class Blake3Functions
    {
        public static void G(ref uint[] state, uint a, uint b, uint c, uint d, uint mx, uint my)
        {
            state[a] = state[a] + state[b] + mx;
            state[d] = (state[d] ^ state[a]).RotateRight(16);
            state[c] = state[c] + state[d];
            state[b] = (state[b] ^ state[c]).RotateRight(12);
            state[a] = state[a] + state[b] + my;
            state[d] = (state[d] ^ state[a]).RotateRight(8);
            state[c] = state[c] + state[d];
            state[b] = (state[b] ^ state[c]).RotateRight(7);
        }

        public static void Round(ref uint[] state, uint[] m)
        {
            G(ref state, 0, 4, 8, 12, m[0], m[1]);
            G(ref state, 1, 5, 9, 13, m[2], m[3]);
            G(ref state, 2, 6, 10, 14, m[4], m[5]);
            G(ref state, 3, 7, 11, 15, m[6], m[7]);
            G(ref state, 0, 5, 10, 15, m[8], m[9]);
            G(ref state, 1, 6, 11, 12, m[10], m[11]);
            G(ref state, 2, 7, 8, 13, m[12], m[13]);
            G(ref state, 3, 4, 9, 14, m[14], m[15]);
        }

        public static void Permute(ref uint[] m)
        {
            uint[] permuted = new uint[16];
            for (int i = 0; i < 16; i++)
            {
                permuted[i] = m[Blake3Constants.MsgPermutation[i]];
            }
            m = permuted;
        }

        public static uint[] Compress(uint[] chainingValue, uint[] blockWords, ulong counter, uint blockLen, uint flags)
        {
            uint[] state =
            [
                chainingValue[0], chainingValue[1], chainingValue[2],
                chainingValue[3], chainingValue[4], chainingValue[5],
                chainingValue[6], chainingValue[7], Blake3Constants.Iv[0],
                Blake3Constants.Iv[1], Blake3Constants.Iv[2],
                Blake3Constants.Iv[3], (uint) counter, (uint) (counter >> 32),
                blockLen, flags
            ];
            uint[] block = (uint[])blockWords.Clone();

            Round(ref state, block);
            Permute(ref block);
            Round(ref state, block);
            Permute(ref block);
            Round(ref state, block);
            Permute(ref block);
            Round(ref state, block);
            Permute(ref block);
            Round(ref state, block);
            Permute(ref block);
            Round(ref state, block);
            Permute(ref block);
            Round(ref state, block);

            for (int i = 0; i < 8; i++)
            {
                state[i] ^= state[i + 8];
                state[i + 8] ^= chainingValue[i];
            }
            return state;
        }

        public static uint[] First8Words(uint[] compressionOutput)
        {
            return compressionOutput.Slice(0, 8);
        }

        public static void WordsFromLittleEndianBytes(byte[] bytes, ref uint[] words)
        {
            for (int i = 0, j = 0; i < bytes.Length; i += 4, j++)
            {
                byte[] bytesBlock = bytes.Slice(i, 4);
                words[j] = Blake3Extensions.FromLeBytes(bytesBlock);
            }
        }

        public static Blake3Output ParentOutput(uint[] leftChildCv, uint[] rightChildCv, uint[] key, uint flags)
        {
            uint[] blockWords = new uint[16];
            Array.Copy(leftChildCv, 0, blockWords, 0, 8);
            Array.Copy(rightChildCv, 0, blockWords, 8, 8);
            return new Blake3Output
            {
                InputChainingValue = key,
                BlockWords = blockWords,
                Counter = 0,
                BlockLen = Blake3Constants.BlockLen,
                Flags = Blake3Constants.Parent | flags
            };
        }

        public static uint[] ParentCv(uint[] leftChildCv, uint[] rightChildCv, uint[] key, uint flags)
        {
            return ParentOutput(leftChildCv, rightChildCv, key, flags).ChainingValue();
        }
    }
}
