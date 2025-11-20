using System.Collections.Generic;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// ED2K 校验实现
    /// </summary>
    public class ED2K : HashAlgorithm
    {
        private const int BLOCKSIZE = 9500 * 1024;
        private readonly byte[] nullArray = [];
        private readonly byte[] nullMd4Hash;
        private readonly List<byte[]> md4HashBlocks = [];
        private readonly MD4 md4 = new();
        private int missing = BLOCKSIZE;
        private byte[] redHash;
        private byte[] blueHash;

        public ED2K()
        {
            nullMd4Hash = md4.ComputeHash(nullArray);
            md4.Initialize();
        }

        public override bool CanReuseTransform
        { get { return true; } }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            while (cbSize is not 0)
            {
                if (cbSize < missing)
                {
                    md4.TransformBlock(array, ibStart, cbSize, null, 0);
                    missing -= cbSize;
                    cbSize = 0;
                }
                else
                {
                    md4.TransformFinalBlock(array, ibStart, missing);
                    md4HashBlocks.Add(md4.Hash);
                    md4.Initialize();

                    cbSize -= missing;
                    ibStart += missing;
                    missing = BLOCKSIZE;
                }
            }
        }

        protected override byte[] HashFinal()
        {
            redHash = null;
            blueHash = null;

            if (md4HashBlocks.Count is 0)
            {
                md4.TransformFinalBlock(nullArray, 0, 0);
                blueHash = md4.Hash;
            }
            else if (md4HashBlocks.Count is 1 && missing is BLOCKSIZE)
            {
                blueHash = md4HashBlocks[0];

                md4.TransformBlock(md4HashBlocks[0], 0, 16, null, 0);
                md4.TransformFinalBlock(md4.ComputeHash(nullArray), 0, 16);
                redHash = md4.Hash;
            }
            else
            {
                if (missing is not BLOCKSIZE)
                {
                    md4.TransformFinalBlock(nullArray, 0, 0);
                    md4HashBlocks.Add(md4.Hash);
                }

                md4.Initialize();
                foreach (var md4HashBlock in md4HashBlocks) md4.TransformBlock(md4HashBlock, 0, 16, null, 0);
                var state = md4.GetState();

                md4.TransformFinalBlock(nullArray, 0, 0);
                blueHash = md4.Hash;

                if (missing is BLOCKSIZE)
                {
                    md4.Initialize(state);
                    md4.TransformFinalBlock(nullMd4Hash, 0, 16);
                    redHash = md4.Hash;
                }
            }

            return redHash ?? blueHash;
        }

        public override void Initialize()
        {
            missing = BLOCKSIZE;
            md4.Initialize();
            md4HashBlocks.Clear();
        }
    }
}
