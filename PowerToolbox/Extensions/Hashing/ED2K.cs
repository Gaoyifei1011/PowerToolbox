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

        private bool blueIsRed;

        public bool BlueIsRed
        {
            get { return blueIsRed; }
        }

        private byte[] redHash;

        public byte[] RedHash
        {
            get { byte[] hash = Hash; return redHash != null ? (byte[])redHash.Clone() : hash; }
        }

        private byte[] blueHash;

        public byte[] BlueHash
        {
            get { byte[] hash = Hash; return blueHash != null ? (byte[])blueHash.Clone() : hash; }
        }

        public ED2K()
        {
            nullMd4Hash = md4.ComputeHash(nullArray);
            md4.Initialize();
        }

        public override bool CanReuseTransform
        { get { return true; } }

        protected override void HashCore(byte[] b, int offset, int length)
        {
            while (length != 0)
            {
                if (length < missing)
                {
                    md4.TransformBlock(b, offset, length, null, 0);
                    missing -= length;
                    length = 0;
                }
                else
                {
                    md4.TransformFinalBlock(b, offset, missing);
                    md4HashBlocks.Add(md4.Hash);
                    md4.Initialize();

                    length -= missing;
                    offset += missing;
                    missing = BLOCKSIZE;
                }
            }
        }

        protected override byte[] HashFinal()
        {
            blueIsRed = false;
            redHash = null;
            blueHash = null;

            if (md4HashBlocks.Count == 0)
            {
                md4.TransformFinalBlock(nullArray, 0, 0);
                blueHash = md4.Hash;
            }
            else if (md4HashBlocks.Count == 1 && missing == BLOCKSIZE)
            {
                blueHash = md4HashBlocks[0];

                md4.TransformBlock(md4HashBlocks[0], 0, 16, null, 0);
                md4.TransformFinalBlock(md4.ComputeHash(nullArray), 0, 16);
                redHash = md4.Hash;
            }
            else
            {
                if (missing != BLOCKSIZE)
                {
                    md4.TransformFinalBlock(nullArray, 0, 0);
                    md4HashBlocks.Add(md4.Hash);
                }

                md4.Initialize();
                foreach (var md4HashBlock in md4HashBlocks) md4.TransformBlock(md4HashBlock, 0, 16, null, 0);
                var state = md4.GetState();

                md4.TransformFinalBlock(nullArray, 0, 0);
                blueHash = md4.Hash;

                if (missing == BLOCKSIZE)
                {
                    md4.Initialize(state);
                    md4.TransformFinalBlock(nullMd4Hash, 0, 16);
                    redHash = md4.Hash;
                }
            }

            if (redHash == null) blueIsRed = true;
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
