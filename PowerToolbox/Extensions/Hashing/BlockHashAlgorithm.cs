using System;
using System.Numerics;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// 表示所有区块哈希算法实现必须派生的基类
    /// </summary>
    internal abstract class BlockHashAlgorithm : HashAlgorithm
    {
        protected readonly int BlockSizeValue;
        protected PaddingType PaddingType = PaddingType.Custom;
        private readonly byte[] lastBlock;
        private int lastBlockLength;
        private BigInteger messageLength;

        /// <summary>
        /// Block hash algorithm ctor
        /// </summary>
        /// <param name="blockSize">size of the block for algorithm in bytes</param>
        internal BlockHashAlgorithm(int blockSize)
        {
            BlockSizeValue = blockSize;
            HashSizeValue = blockSize << 3; // * 8
            lastBlock = new byte[BlockSizeValue];
        }

        /// <summary>
        /// Size of algorithm block in bytes
        /// </summary>
        internal int BlockSize
        {
            get
            {
                return BlockSizeValue;
            }
        }

        /// <summary>
        /// Initialization algorithm variables.
        /// </summary>
        public override void Initialize()
        {
            messageLength = 0;
            Array.Clear(lastBlock, 0, lastBlock.Length);
            lastBlockLength = 0;
        }

        /// <summary>
        /// Processing block of bytes (size is @BlockSize)
        /// </summary>
        /// <param name="block">block of bytes</param>
        protected abstract void ProcessBlock(byte[] block, int offset);

        /// <summary>
        /// Generate padding blocks for hash algorithm
        /// </summary>
        /// <param name="lastBlock">last unaligned block that should be padded</param>
        /// <param name="messageLength">message length in bytes</param>
        protected virtual byte[] GeneratePaddingBlocks(byte[] lastBlock, int lastBlockLength, BigInteger messageLength)
        {
            if (lastBlock is null)
            {
                return default;
            }

            return PaddingType switch
            {
                PaddingType.Custom => throw new InvalidOperationException("Custom padding type should override GeneratePaddingBlocks method."),
                PaddingType.OneZeroFillAnd8BytesMessageLengthLittleEndian => GenerateOneZeroFillAnd8BytesMessageLengthLittleEndianPadding(lastBlock, lastBlockLength, messageLength),
                PaddingType.OneZeroFillAnd8BytesMessageLengthBigEndian => GenerateOneZeroFillAnd8BytesMessageLengthBigEndianPadding(lastBlock, lastBlockLength, messageLength),
                PaddingType.OneZeroFillAnd16BytesMessageLengthBigEndian => GenerateOneZeroFillAnd16BytesMessageLengthBigEndianPadding(lastBlock, lastBlockLength, messageLength),
                _ => throw new InvalidOperationException($"Unsupported padding type '{PaddingType}'."),
            };
        }

        protected abstract byte[] ProcessFinalBlock();

        /// <summary>
        /// Main hash procedure
        /// </summary>
        /// <param name="array">byte array</param>
        /// <param name="offset">offset in array</param>
        /// <param name="length">length of block for processing</param>
        protected override sealed void HashCore(byte[] array, int offset, int length)
        {
            if (array is null || length is 0)
            {
                return;
            }

            messageLength += length;

            if (lastBlockLength > 0)
            {
                int lastBlockRemaining = BlockSizeValue - lastBlockLength;
                if (length >= lastBlockRemaining)
                {
                    Array.Copy(array, offset, lastBlock, lastBlockLength, lastBlockRemaining);
                    ProcessBlock(lastBlock, 0);
                    offset += lastBlockRemaining;
                    length -= lastBlockRemaining;
                    lastBlockLength = 0;
                }
            }

            while (length >= BlockSizeValue)
            {
                ProcessBlock(array, offset);
                offset += BlockSizeValue;
                length -= BlockSizeValue;
            }

            if (length > 0)
            {
                Array.Copy(array, offset, lastBlock, lastBlockLength, length);
                lastBlockLength += length;
            }
        }

        /// <summary>
        /// Hash final block.
        /// </summary>
        /// <returns>hash value</returns>
        protected override sealed byte[] HashFinal()
        {
            if (lastBlockLength > lastBlock.Length)
            {
                throw new InvalidOperationException("lastBlockLength > lastBlock.Length");
            }

            byte[] padding = GeneratePaddingBlocks(lastBlock, lastBlockLength, messageLength);
            for (int ii = 0; ii < padding.Length; ii += BlockSizeValue)
            {
                ProcessBlock(padding, ii);
            }

            return ProcessFinalBlock();
        }

        private byte[] GenerateOneZeroFillAnd8BytesMessageLengthLittleEndianPadding(byte[] lastBlock, int lastBlockLength, BigInteger messageLength)
        {
            if (lastBlock is null)
            {
                return default;
            }

            int paddingBlocks = lastBlockLength + 8 >= BlockSizeValue ? 2 : 1;
            byte[] padding = new byte[paddingBlocks * BlockSizeValue];
            Array.Copy(lastBlock, 0, padding, 0, lastBlockLength);
            padding[lastBlockLength] = 0x80;
            byte[] messageLengthInBits = (messageLength << 3).ToByteArray();

            if (messageLengthInBits.Length > 8)
            {
                BigInteger supportedLength = BigInteger.Pow(2, 8 << 3) - 1;
                throw new InvalidOperationException($"Message is too long for this hash algorithm. Actual: {messageLength}, Max supported: {supportedLength} bytes.");
            }

            int endOffset = padding.Length - 8;
            for (int ii = 0; ii < messageLengthInBits.Length; ii++)
            {
                padding[endOffset + ii] = messageLengthInBits[ii];
            }

            return padding;
        }

        private byte[] GenerateOneZeroFillAnd8BytesMessageLengthBigEndianPadding(byte[] lastBlock, int lastBlockLength, BigInteger messageLength)
        {
            if (lastBlock is null)
            {
                return default;
            }

            int paddingBlocks = lastBlockLength + 8 >= BlockSizeValue ? 2 : 1;
            byte[] padding = new byte[paddingBlocks * BlockSizeValue];
            Array.Copy(lastBlock, 0, padding, 0, lastBlockLength);
            padding[lastBlockLength] = 0x80;
            byte[] messageLengthInBits = (messageLength << 3).ToByteArray();

            if (messageLengthInBits.Length > 8)
            {
                BigInteger supportedLength = BigInteger.Pow(2, 8 << 3) - 1;
                throw new InvalidOperationException($"Message is too long for this hash algorithm. Actual: {messageLength}, Max supported: {supportedLength} bytes.");
            }

            int endOffset = padding.Length - 8;
            for (int ii = 8 - messageLengthInBits.Length; ii < 8; ii++)
            {
                padding[endOffset + ii] = messageLengthInBits[7 - ii];
            }

            return padding;
        }

        private byte[] GenerateOneZeroFillAnd16BytesMessageLengthBigEndianPadding(byte[] lastBlock, int lastBlockLength, BigInteger messageLength)
        {
            if (lastBlock is null)
            {
                return default;
            }

            int paddingBlocks = lastBlockLength + 16 >= BlockSizeValue ? 2 : 1;
            byte[] padding = new byte[paddingBlocks * BlockSizeValue];
            Array.Copy(lastBlock, 0, padding, 0, lastBlockLength);
            padding[lastBlockLength] = 0x80;
            byte[] messageLengthInBits = (messageLength << 3).ToByteArray();

            if (messageLengthInBits.Length > 16)
            {
                BigInteger supportedLength = BigInteger.Pow(2, 16 << 3) - 1;
                throw new InvalidOperationException($"Message is too long for this hash algorithm. Actual: {messageLength}, Max supported: {supportedLength} bytes.");
            }

            int endOffset = padding.Length - 16;
            for (int ii = 16 - messageLengthInBits.Length; ii < 16; ii++)
            {
                padding[endOffset + ii] = messageLengthInBits[15 - ii];
            }

            return padding;
        }
    }
}
