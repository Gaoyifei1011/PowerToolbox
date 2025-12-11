using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// Blake3 校验实现
    /// </summary>
    public class Blake3 : KeyedHashAlgorithm
    {
        private Blake3ChunkState _chunkState;
        private uint[] _key;
        private readonly uint[][] _cvStack;
        private byte _cvStackLen;
        private readonly uint _flags;

        private Blake3(uint[] key, uint flags)
        {
            HashSizeValue = (int)Blake3Constants.OutLen * 8;
            State = 0;

            _chunkState = new Blake3ChunkState(key, 0, flags);
            _key = key;
            _cvStack = new uint[54][];
            for (int i = 0; i < 54; i++)
            {
                _cvStack[i] = new uint[8];
            }
            _cvStackLen = 0;
            _flags = flags;
        }

        public Blake3() : this(Blake3Constants.Iv, 0)
        {
        }

        public Blake3(byte[] key) : this(KeyWordsFromKey(key), Blake3Constants.KeyedHash)
        {
            KeyValue = key;
        }

        #region Overrides

        public override byte[] Hash
        {
            get { return HashFinal(); }
        }

        public new int HashSize
        {
            get { return HashSizeValue; }
            set { HashSizeValue = value; }
        }

        public override byte[] Key
        {
            set
            {
                if (State is not 0)
                {
                    throw new CryptographicException(
                        "Tried to set key on a non-clean hasher");
                }

                KeyValue = value;
                _key = KeyWordsFromKey(value);
            }
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (State is 0)
            {
                State = 1;
            }

            int roof = ibStart + cbSize;
            int i = ibStart;
            while (i < roof)
            {
                if (_chunkState.Len is Blake3Constants.ChunkLen)
                {
                    uint[] chunkCv = _chunkState.Output().ChainingValue();
                    ulong totalChunks = _chunkState.ChunkCounter + 1;
                    AddChunkChainingValue(chunkCv, totalChunks);
                    _chunkState = new Blake3ChunkState(_key, totalChunks, _flags);
                }

                int want = Blake3Constants.ChunkLen - _chunkState.Len;
                int take = Math.Min(want, roof - i);
                byte[] input = array.Slice(i, take);
                _chunkState.Update(input);
                i += take;
            }
        }

        protected override byte[] HashFinal()
        {
            Blake3Output output = _chunkState.Output();
            int parentNodesRemaining = _cvStackLen;
            while (parentNodesRemaining > 0)
            {
                parentNodesRemaining--;
                output = Blake3Functions.ParentOutput(_cvStack[parentNodesRemaining], output.ChainingValue(), _key, _flags);
            }
            byte[] ret = new byte[HashSizeValue / 8];
            output.RootOutputBytes(ref ret);
            return ret;
        }

        public override void Initialize()
        {
            State = 0;

            _chunkState = new Blake3ChunkState(_key, 0, _flags);
            for (int i = 0; i < 54; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    _cvStack[i][j] = 0;
                }
            }
            _cvStackLen = 0;
        }

        #endregion Overrides

        private static uint[] KeyWordsFromKey(byte[] key)
        {
            if (key.Length is not Blake3Constants.KeyLen)
            {
                throw new CryptographicException($"Expected a {Blake3Constants.KeyLen} bytes long key, got a {key.Length} long one");
            }

            uint[] keyWords = new uint[8];
            Blake3Functions.WordsFromLittleEndianBytes(key, ref keyWords);
            return keyWords;
        }

        private void PushStack(uint[] cv)
        {
            _cvStack[_cvStackLen] = cv;
            _cvStackLen++;
        }

        private uint[] PopStack()
        {
            _cvStackLen--;
            return _cvStack[_cvStackLen];
        }

        private void AddChunkChainingValue(uint[] newCv, ulong totalChunks)
        {
            while ((totalChunks & 1) is 0)
            {
                newCv = Blake3Functions.ParentCv(PopStack(), newCv, _key, _flags);
                totalChunks >>= 1;
            }
            PushStack(newCv);
        }
    }
}
