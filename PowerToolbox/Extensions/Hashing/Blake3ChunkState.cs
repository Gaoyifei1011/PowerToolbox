using System;

namespace PowerToolbox.Extensions.Hashing
{
    public class Blake3ChunkState(uint[] key, ulong chunkCounter, uint flags)
    {
        public readonly ulong ChunkCounter = chunkCounter;
        private byte[] _block = new byte[Blake3Constants.BlockLen];
        private byte _blockLen = 0;
        private byte _blocksCompressed = 0;

        public int Len
        {
            get { return Blake3Constants.BlockLen * _blocksCompressed + _blockLen; }
        }

        public uint StartFlag
        {
            get { return _blocksCompressed is 0 ? Blake3Constants.ChunkStart : 0; }
        }

        public void Update(byte[] input)
        {
            while (input.Length > 0)
            {
                if (_blockLen is Blake3Constants.BlockLen)
                {
                    uint[] blockWords = new uint[16];
                    Blake3Functions.WordsFromLittleEndianBytes(_block, ref blockWords);
                    key = Blake3Functions.First8Words(Blake3Functions.Compress(key, blockWords, ChunkCounter, Blake3Constants.BlockLen, flags | StartFlag));
                    _blocksCompressed++;
                    _block = new byte[Blake3Constants.BlockLen];
                    _blockLen = 0;
                }

                int want = Blake3Constants.BlockLen - _blockLen;
                int take = Math.Min(want, input.Length);
                Array.Copy(input, 0, _block, _blockLen, take);
                _blockLen += (byte)take;
                input = input.Slice(take, input.Length - take);
            }
        }

        public Blake3Output Output()
        {
            uint[] blockWords = new uint[16];
            Blake3Functions.WordsFromLittleEndianBytes(_block, ref blockWords);
            return new Blake3Output
            {
                InputChainingValue = key,
                BlockWords = blockWords,
                Counter = ChunkCounter,
                BlockLen = _blockLen,
                Flags = flags | StartFlag | Blake3Constants.ChunkEnd
            };
        }
    }
}
