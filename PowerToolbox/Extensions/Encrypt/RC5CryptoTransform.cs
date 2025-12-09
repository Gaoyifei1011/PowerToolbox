using System;
using System.Security.Cryptography;

namespace PowerToolbox.Extensions.Encrypt
{
    public class RC5CryptoTransform : ICryptoTransform
    {
        private readonly RC5Engine engine;
        private readonly bool encrypting;
        private readonly CipherMode mode;
        private readonly PaddingMode padding;
        private readonly int blockSize = RC5Engine.BlockSizeBytes;
        private readonly byte[] iv;           // original IV copy
        private readonly byte[] feedback;     // chaining register (for CBC/CFB/OFB)
        private bool disposed = false;
        private byte[] buffer;       // 在 TransformBlock 调用之间的部分数据缓冲区
        private int bufferCount = 0;
        private readonly RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();

        public RC5CryptoTransform(byte[] key, byte[] iv, bool encrypting, CipherMode mode, PaddingMode padding, int rounds)
        {
            engine = new RC5Engine(key, rounds);
            this.encrypting = encrypting;
            this.mode = mode;
            this.padding = padding;

            if (iv is null)
            {
                throw new ArgumentNullException(nameof(iv));
            }

            if (iv.Length != RC5Engine.BlockSizeBytes)
            {
                throw new ArgumentException("IV must be 8 bytes.", nameof(iv));
            }

            this.iv = new byte[blockSize];
            Buffer.BlockCopy(iv, 0, this.iv, 0, blockSize);
            feedback = new byte[blockSize];
            Buffer.BlockCopy(this.iv, 0, feedback, 0, blockSize);

            buffer = new byte[blockSize];
            bufferCount = 0;

            // CTS：仅在mode == CBC且padding == None时允许
            if (this.mode is CipherMode.CTS && this.padding is not PaddingMode.None)
            {
                throw new CryptographicException("CTS mode requires PaddingMode.None in this implementation.");
            }
        }

        public bool CanReuseTransform => false;

        public bool CanTransformMultipleBlocks => true;

        public int InputBlockSize => blockSize;

        public int OutputBlockSize => blockSize;

        /// <summary>
        /// 处理尽可能多的完整块，缓冲剩余部分
        /// </summary>
        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("RC5CryptoTransform");
            }

            if (inputCount is 0)
            {
                return 0;
            }

            if (inputBuffer is null)
            {
                throw new ArgumentNullException(nameof(inputBuffer));
            }

            if (outputBuffer is null)
            {
                throw new ArgumentNullException(nameof(outputBuffer));
            }

            if (inputOffset < 0 || inputCount < 0 || (inputOffset + inputCount) > inputBuffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(inputOffset), "inputOffset and inputContent length is not right");
            }

            int total = bufferCount + inputCount;
            int toProcess = (total / blockSize) * blockSize; // 现在可以处理的字节数（完整块）
            int write = 0;

            if (toProcess is 0)
            {
                // 对于一个块来说不够，只是缓冲区
                Buffer.BlockCopy(inputBuffer, inputOffset, buffer, bufferCount, inputCount);
                bufferCount += inputCount;
                return 0;
            }

            byte[] work = new byte[toProcess];
            // copy buffered + incoming into work
            if (bufferCount > 0)
            {
                Buffer.BlockCopy(buffer, 0, work, 0, bufferCount);
                Buffer.BlockCopy(inputBuffer, inputOffset, work, bufferCount, toProcess - bufferCount);
            }
            else
            {
                Buffer.BlockCopy(inputBuffer, inputOffset, work, 0, toProcess);
            }

            // save remainder back to buffer
            int rem = total - toProcess;
            if (rem > 0)
            {
                Buffer.BlockCopy(inputBuffer, inputOffset + (inputCount - rem), buffer, 0, rem);
            }
            bufferCount = rem;

            // process block by block
            for (int pos = 0; pos < toProcess; pos += blockSize)
            {
                if (mode is CipherMode.ECB)
                {
                    if (encrypting)
                    {
                        engine.EncryptBlock(work, pos, outputBuffer, outputOffset + write);
                    }
                    else
                    {
                        engine.DecryptBlock(work, pos, outputBuffer, outputOffset + write);
                    }
                }
                else if (mode is CipherMode.CBC)
                {
                    if (encrypting)
                    {
                        // XOR plaintext with feedback, encrypt -> ciphertext, update feedback
                        byte[] x = new byte[blockSize];
                        for (int i = 0; i < blockSize; i++) x[i] = (byte)(work[pos + i] ^ feedback[i]);
                        engine.EncryptBlock(x, 0, outputBuffer, outputOffset + write);
                        Buffer.BlockCopy(outputBuffer, outputOffset + write, feedback, 0, blockSize);
                        Array.Clear(x, 0, blockSize);
                    }
                    else
                    {
                        // decrypt to temp, then XOR with feedback -> plaintext, update feedback with ciphertext (input)
                        byte[] tmp = new byte[blockSize];
                        engine.DecryptBlock(work, pos, tmp, 0);
                        for (int i = 0; i < blockSize; i++)
                            outputBuffer[outputOffset + write + i] = (byte)(tmp[i] ^ feedback[i]);
                        // update feedback to current ciphertext
                        Buffer.BlockCopy(work, pos, feedback, 0, blockSize);
                        Array.Clear(tmp, 0, blockSize);
                    }
                }
                else if (mode is CipherMode.CFB)
                {
                    // Full-block CFB (CFB-64)
                    if (encrypting)
                    {
                        byte[] enc = new byte[blockSize];
                        engine.EncryptBlock(feedback, 0, enc, 0);
                        for (int i = 0; i < blockSize; i++)
                        {
                            byte c = (byte)(enc[i] ^ work[pos + i]);
                            outputBuffer[outputOffset + write + i] = c;
                        }
                        // feedback = ciphertext
                        Buffer.BlockCopy(outputBuffer, outputOffset + write, feedback, 0, blockSize);
                        Array.Clear(enc, 0, blockSize);
                    }
                    else
                    {
                        byte[] enc = new byte[blockSize];
                        engine.EncryptBlock(feedback, 0, enc, 0);
                        for (int i = 0; i < blockSize; i++)
                        {
                            byte p = (byte)(enc[i] ^ work[pos + i]);
                            outputBuffer[outputOffset + write + i] = p;
                        }
                        // feedback = ciphertext (work)
                        Buffer.BlockCopy(work, pos, feedback, 0, blockSize);
                        Array.Clear(enc, 0, blockSize);
                    }
                }
                else if (mode is CipherMode.OFB)
                {
                    // OFB: feedback = E(feedback); keystream = feedback; plaintext XOR keystream
                    byte[] enc = new byte[blockSize];
                    engine.EncryptBlock(feedback, 0, enc, 0);
                    // update feedback to enc
                    Buffer.BlockCopy(enc, 0, feedback, 0, blockSize);
                    for (int i = 0; i < blockSize; i++)
                    {
                        outputBuffer[outputOffset + write + i] = (byte)(work[pos + i] ^ enc[i]);
                    }
                    Array.Clear(enc, 0, blockSize);
                }
                else if (mode is CipherMode.CTS)
                {
                    // CTS processing cannot be done here reliably because it needs the final blocks.
                    // So we buffer everything: move processed blocks back into our internal buffer and do nothing.
                    // To keep logic simple: append processed bytes back to buffer and return 0.
                    // (TransformFinalBlock will handle actual CTS processing.)
                    int remain = toProcess - pos;
                    // move remaining into buffer (prepend already-processed blocks)
                    byte[] newBuf = new byte[remain + rem];
                    Buffer.BlockCopy(work, pos, newBuf, 0, remain);
                    if (rem > 0)
                    {
                        Buffer.BlockCopy(buffer, 0, newBuf, remain, rem);
                    }

                    buffer = newBuf;
                    bufferCount = newBuf.Length;
                    Array.Clear(work, 0, work.Length);
                    return 0;
                }
                else
                {
                    throw new NotSupportedException("Cipher mode not supported.");
                }

                write += blockSize;
            }

            Array.Clear(work, 0, work.Length);
            return write;
        }

        /// <summary>
        /// 完成填充/ CTS处理
        /// </summary>
        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("RC5CryptoTransform");
            }

            inputBuffer ??= [];

            // 收集所有剩余数据
            int total = bufferCount + inputCount;
            byte[] all = new byte[total];
            if (bufferCount > 0)
            {
                Buffer.BlockCopy(buffer, 0, all, 0, bufferCount);
            }

            if (inputCount > 0)
            {
                Buffer.BlockCopy(inputBuffer, inputOffset, all, bufferCount, inputCount);
            }

            byte[] result = null;

            if (mode is CipherMode.CTS)
            {
                // Only supported with CBC and padding == None in this implementation
                if (padding is not PaddingMode.None)
                {
                    throw new CryptographicException("CTS requires PaddingMode.None in this implementation.");
                }

                if (mode is not CipherMode.CTS)
                {
                    throw new CryptographicException("Unexpected mode.");
                }
                // For simplicity treat `mode==CTS` as CBC with ciphertext stealing.
                // Actually in SymmetricAlgorithm .Mode set to CTS indicates CTS variant of CBC.
                // Implement CBC-CTS now:

                // If total < blockSize -> treat like CBC without stealing (encrypt single block, then truncate)
                if (encrypting)
                {
                    if (total <= blockSize)
                    {
                        // pad with zeros to full block for encryption, then truncate output to total length
                        byte[] tmp = new byte[blockSize];
                        if (total > 0)
                        {
                            Buffer.BlockCopy(all, 0, tmp, 0, total);
                        }

                        byte[] outblock = new byte[blockSize];
                        // CBC: XOR with feedback then encrypt
                        byte[] x = new byte[blockSize];
                        for (int i = 0; i < blockSize; i++)
                        {
                            x[i] = (byte)(tmp[i] ^ feedback[i]);
                        }

                        engine.EncryptBlock(x, 0, outblock, 0);
                        // update feedback
                        Buffer.BlockCopy(outblock, 0, feedback, 0, blockSize);
                        result = new byte[total];
                        Buffer.BlockCopy(outblock, 0, result, 0, total);
                        Array.Clear(tmp, 0, tmp.Length);
                        Array.Clear(x, 0, x.Length);
                    }
                    else
                    {
                        // total > blockSize
                        int nFull = total / blockSize;
                        int lastSize = total % blockSize;
                        if (lastSize is 0)
                        {
                            // behaves like normal CBC
                            result = new byte[total];
                            int pos = 0;
                            for (int i = 0; i < total; i += blockSize)
                            {
                                byte[] x = new byte[blockSize];
                                for (int j = 0; j < blockSize; j++)
                                {
                                    x[j] = (byte)(all[i + j] ^ feedback[j]);
                                }

                                engine.EncryptBlock(x, 0, result, pos);
                                Buffer.BlockCopy(result, pos, feedback, 0, blockSize);
                                pos += blockSize;
                                Array.Clear(x, 0, blockSize);
                            }
                        }
                        else
                        {
                            // steal last partial
                            int m = nFull; // number of full blocks before final partial
                                           // process blocks 0..m-2 normally (if m>=2)
                            int posOut = 0;
                            int pos = 0;
                            for (int i = 0; i < (m - 1) * blockSize; i += blockSize)
                            {
                                byte[] x = new byte[blockSize];
                                for (int j = 0; j < blockSize; j++)
                                {
                                    x[j] = (byte)(all[pos + j] ^ feedback[j]);
                                }

                                engine.EncryptBlock(x, 0, all, pos); // reuse all[] as temp
                                Buffer.BlockCopy(all, pos, feedback, 0, blockSize);
                                posOut += blockSize;
                                pos += blockSize;
                                Array.Clear(x, 0, blockSize);
                            }

                            // Now handle last full block P_{m-1} (at pos) and partial P_m (size = lastSize)
                            byte[] Pm1 = new byte[blockSize];
                            Buffer.BlockCopy(all, pos, Pm1, 0, blockSize);
                            // XOR with feedback
                            for (int j = 0; j < blockSize; j++)
                            {
                                Pm1[j] ^= feedback[j];
                            }

                            byte[] Ctemp = new byte[blockSize];
                            engine.EncryptBlock(Pm1, 0, Ctemp, 0); // C_{m-1}'
                                                                   // For P_m (partial)
                            byte[] Pm = new byte[blockSize]; // zero-padded
                            Buffer.BlockCopy(all, pos + blockSize, Pm, 0, lastSize);

                            // Compute S = encrypt( (Pm zero-padded) ^ Ctemp )
                            byte[] X = new byte[blockSize];
                            for (int j = 0; j < blockSize; j++)
                            {
                                X[j] = (byte)(Pm[j] ^ Ctemp[j]);
                            }

                            byte[] Clast = new byte[blockSize];
                            engine.EncryptBlock(X, 0, Clast, 0);

                            // Output:
                            // - C_0..C_{m-2} as computed earlier (they are in all[] overwritten)
                            // - C_{m-1} is first `lastSize` bytes taken from Clast
                            // - C_m is Ctemp (full block)
                            int outLen = (m - 1) * blockSize + lastSize + blockSize;
                            result = new byte[outLen];
                            // copy previous blocks
                            if (posOut > 0) Buffer.BlockCopy(all, 0, result, 0, posOut);
                            // C_{m-1} (stolen) = first lastSize bytes of Clast
                            Buffer.BlockCopy(Clast, 0, result, posOut, lastSize);
                            // C_m = Ctemp
                            Buffer.BlockCopy(Ctemp, 0, result, posOut + lastSize, blockSize);

                            // update feedback with C_m for completeness
                            Buffer.BlockCopy(Ctemp, 0, feedback, 0, blockSize);

                            // clear temps
                            Array.Clear(Pm1, 0, Pm1.Length);
                            Array.Clear(Pm, 0, Pm.Length);
                            Array.Clear(X, 0, X.Length);
                            Array.Clear(Ctemp, 0, Ctemp.Length);
                            Array.Clear(Clast, 0, Clast.Length);
                        }
                    }
                }
                else
                {
                    // decryption for CTS
                    if (total <= blockSize)
                    {
                        // single partial or full block — decrypt padded zero block then truncate
                        byte[] inBlock = new byte[blockSize];
                        Buffer.BlockCopy(all, 0, inBlock, 0, total);
                        // CBC decrypt: decrypt then XOR with feedback
                        byte[] tmp = new byte[blockSize];
                        engine.DecryptBlock(inBlock, 0, tmp, 0);
                        byte[] plain = new byte[blockSize];
                        for (int i = 0; i < blockSize; i++)
                        {
                            plain[i] = (byte)(tmp[i] ^ feedback[i]);
                        }

                        result = new byte[total];
                        Buffer.BlockCopy(plain, 0, result, 0, total);
                        Array.Clear(inBlock, 0, inBlock.Length);
                        Array.Clear(tmp, 0, tmp.Length);
                        Array.Clear(plain, 0, plain.Length);
                    }
                    else
                    {
                        // Need to detect whether last ciphertext length corresponds to stolen format
                        // We assume input is produced by the encryption branch above.
                        // Let total = (m-1)*B + s + B  (m>=2, 0 < s < B)
                        // So ciphertext consists of:
                        // C0..C_{m-2}, C_{m-1}'(s bytes), C_m (full B bytes)
                        // Reconstruct C_{m-1} full block as: take C_m as last block, then build C_{m-1} by combining C_{m-1}' and trailing bytes of C_m.
                        int len = total;
                        // find if last portion is partial: we detect by checking if (len % blockSize) != 0
                        int lastPartial = len % blockSize;
                        if (lastPartial is 0)
                        {
                            // no stealing - normal CBC
                            result = new byte[len];
                            int pos = 0;
                            for (int i = 0; i < len; i += blockSize)
                            {
                                byte[] dec = new byte[blockSize];
                                engine.DecryptBlock(all, i, dec, 0);
                                for (int j = 0; j < blockSize; j++)
                                {
                                    result[pos + j] = (byte)(dec[j] ^ feedback[j]);
                                }

                                Buffer.BlockCopy(all, i, feedback, 0, blockSize);
                                pos += blockSize;
                                Array.Clear(dec, 0, blockSize);
                            }
                        }
                        else
                        {
                            int s = lastPartial; // stolen size
                            if (len < blockSize + s) throw new CryptographicException("Invalid CTS ciphertext.");
                            int mMinus1Bytes = len - (s + blockSize);
                            // C0..C_{m-2} occupy mMinus1Bytes bytes
                            int outPos = 0;
                            // process C0..C_{m-3} if any
                            for (int i = 0; i < mMinus1Bytes; i += blockSize)
                            {
                                byte[] dec = new byte[blockSize];
                                engine.DecryptBlock(all, i, dec, 0);
                                for (int j = 0; j < blockSize; j++)
                                {
                                    result = EnsureAndCopy(result, outPos + blockSize); // ensure size
                                }

                                Array.Clear(dec, 0, blockSize);
                            }
                            // For simplicity, and to keep code manageable, we will decrypt CTS with a simpler reconstruction:
                            // Reconstruct full C_{m-1} as: take C_m (last full block) and append the s bytes that were stolen (C_{m-1}' bytes) as the first s bytes of reconstructed block,
                            // and take the remaining (B-s) bytes from C_m's trailing bytes. This mirrors the encryption construction earlier.
                            // Implementation of a full robust CTS decryption is lengthy; in most scenarios it's better to avoid CTS or use tested libraries.
                            throw new CryptographicException("CTS decryption of multi-block ciphertext is not fully implemented in this sample.");
                        }
                    }
                }

                return result;
            }

            // Non-CTS modes: apply padding (for encryption) or remove padding (for decryption)
            if (encrypting)
            {
                // Apply padding per PaddingMode
                byte[] toProcess;
                if (padding is PaddingMode.None)
                {
                    if ((total % blockSize) is not 0)
                    {
                        throw new CryptographicException("Data not block aligned and padding is None.");
                    }

                    toProcess = all;
                }
                else if (padding is PaddingMode.PKCS7)
                {
                    int pad = blockSize - (total % blockSize);
                    if (pad is 0)
                    {
                        pad = blockSize;
                    }

                    toProcess = new byte[total + pad];
                    if (total > 0)
                    {
                        Buffer.BlockCopy(all, 0, toProcess, 0, total);
                    }

                    for (int i = total; i < toProcess.Length; i++)
                    {
                        toProcess[i] = (byte)pad;
                    }
                }
                else if (padding is PaddingMode.Zeros)
                {
                    int pad = blockSize - (total % blockSize);
                    if (pad is 0)
                    {
                        pad = 0; // no extra block when already aligned
                    }

                    toProcess = new byte[total + pad];
                    if (total > 0)
                    {
                        Buffer.BlockCopy(all, 0, toProcess, 0, total);
                    }
                    // Zero-fill happens by default
                }
                else if (padding is PaddingMode.ANSIX923)
                {
                    int pad = blockSize - (total % blockSize);
                    if (pad is 0)
                    {
                        pad = blockSize;
                    }

                    toProcess = new byte[total + pad];
                    if (total > 0)
                    {
                        Buffer.BlockCopy(all, 0, toProcess, 0, total);
                    }
                    // last byte = pad, other pad bytes = 0
                    toProcess[toProcess.Length - 1] = (byte)pad;
                }
                else if (padding is PaddingMode.ISO10126)
                {
                    int pad = blockSize - (total % blockSize);
                    if (pad is 0)
                    {
                        pad = blockSize;
                    }

                    toProcess = new byte[total + pad];
                    if (total > 0)
                    {
                        Buffer.BlockCopy(all, 0, toProcess, 0, total);
                    }

                    byte[] rndBytes = new byte[pad - 1];
                    randomNumberGenerator.GetBytes(rndBytes);
                    Buffer.BlockCopy(rndBytes, 0, toProcess, total, rndBytes.Length);
                    toProcess[toProcess.Length - 1] = (byte)pad;
                }
                else
                {
                    throw new NotSupportedException("Unsupported padding mode.");
                }

                // process all blocks in toProcess
                result = new byte[toProcess.Length];
                int posOut = 0;
                for (int pos = 0; pos < toProcess.Length; pos += blockSize)
                {
                    if (mode is CipherMode.ECB)
                    {
                        engine.EncryptBlock(toProcess, pos, result, posOut);
                    }
                    else if (mode is CipherMode.CBC)
                    {
                        byte[] x = new byte[blockSize];
                        for (int j = 0; j < blockSize; j++)
                        {
                            x[j] = (byte)(toProcess[pos + j] ^ feedback[j]);
                        }

                        engine.EncryptBlock(x, 0, result, posOut);
                        Buffer.BlockCopy(result, posOut, feedback, 0, blockSize);
                        Array.Clear(x, 0, blockSize);
                    }
                    else if (mode is CipherMode.CFB)
                    {
                        byte[] enc = new byte[blockSize];
                        engine.EncryptBlock(feedback, 0, enc, 0);
                        for (int j = 0; j < blockSize; j++)
                        {
                            result[posOut + j] = (byte)(enc[j] ^ toProcess[pos + j]);
                        }

                        Buffer.BlockCopy(result, posOut, feedback, 0, blockSize);
                        Array.Clear(enc, 0, blockSize);
                    }
                    else if (mode is CipherMode.OFB)
                    {
                        byte[] enc = new byte[blockSize];
                        engine.EncryptBlock(feedback, 0, enc, 0);
                        Buffer.BlockCopy(enc, 0, feedback, 0, blockSize);
                        for (int j = 0; j < blockSize; j++)
                        {
                            result[posOut + j] = (byte)(toProcess[pos + j] ^ enc[j]);
                        }

                        Array.Clear(enc, 0, blockSize);
                    }
                    else
                    {
                        throw new NotSupportedException("Cipher mode not supported for encryption in this implementation.");
                    }
                    posOut += blockSize;
                }
            }
            else
            {
                // Decrypting: input must be multiple of block size (except in CFB/OFB stream cases where we may accept partial)
                if ((total % blockSize) != 0)
                {
                    // In stream modes CFB/OFB we can process partial bytes as keystream XOR
                    if (mode is CipherMode.CFB || mode is CipherMode.OFB)
                    {
                        // process all full blocks
                        int full = (total / blockSize) * blockSize;
                        result = new byte[total];
                        int posOut = 0;
                        for (int pos = 0; pos < full; pos += blockSize)
                        {
                            if (mode is CipherMode.CFB)
                            {
                                byte[] enc = new byte[blockSize];
                                engine.EncryptBlock(feedback, 0, enc, 0);
                                for (int j = 0; j < blockSize; j++)
                                {
                                    result[posOut + j] = (byte)(enc[j] ^ all[pos + j]);
                                }

                                Buffer.BlockCopy(all, pos, feedback, 0, blockSize);
                                Array.Clear(enc, 0, blockSize);
                            }
                            else // OFB
                            {
                                byte[] enc = new byte[blockSize];
                                engine.EncryptBlock(feedback, 0, enc, 0);
                                Buffer.BlockCopy(enc, 0, feedback, 0, blockSize);
                                for (int j = 0; j < blockSize; j++)
                                {
                                    result[posOut + j] = (byte)(all[pos + j] ^ enc[j]);
                                }

                                Array.Clear(enc, 0, blockSize);
                            }
                            posOut += blockSize;
                        }
                        // process remainder bytes
                        int rem = total - full;
                        if (rem > 0)
                        {
                            byte[] enc = new byte[blockSize];
                            engine.EncryptBlock(feedback, 0, enc, 0);
                            if (mode is CipherMode.CFB)
                            {
                                for (int j = 0; j < rem; j++)
                                {
                                    result[posOut + j] = (byte)(enc[j] ^ all[full + j]);
                                }
                                // update feedback with ciphertext bytes (partial)
                                // For full-block CFB we usually shift in full ciphertext block; partial update isn't standard here.
                            }
                            else // OFB
                            {
                                for (int j = 0; j < rem; j++)
                                {
                                    result[posOut + j] = (byte)(all[full + j] ^ enc[j]);
                                }
                                // update feedback = enc (full)
                                Buffer.BlockCopy(enc, 0, feedback, 0, blockSize);
                            }
                            Array.Clear(enc, 0, blockSize);
                        }
                        // No padding removal (because stream mode)
                    }
                    else
                    {
                        throw new CryptographicException("Invalid cipher text length.");
                    }
                }
                else
                {
                    // total multiple of block size
                    result = new byte[total];
                    int posOut = 0;
                    for (int pos = 0; pos < total; pos += blockSize)
                    {
                        if (mode is CipherMode.ECB)
                        {
                            engine.DecryptBlock(all, pos, result, posOut);
                        }
                        else if (mode is CipherMode.CBC)
                        {
                            byte[] tmp = new byte[blockSize];
                            engine.DecryptBlock(all, pos, tmp, 0);
                            for (int j = 0; j < blockSize; j++)
                            {
                                result[posOut + j] = (byte)(tmp[j] ^ feedback[j]);
                            }

                            Buffer.BlockCopy(all, pos, feedback, 0, blockSize);
                            Array.Clear(tmp, 0, blockSize);
                        }
                        else if (mode is CipherMode.CFB)
                        {
                            byte[] enc = new byte[blockSize];
                            engine.EncryptBlock(feedback, 0, enc, 0);
                            for (int j = 0; j < blockSize; j++)
                            {
                                result[posOut + j] = (byte)(enc[j] ^ all[pos + j]);
                            }

                            Buffer.BlockCopy(all, pos, feedback, 0, blockSize);
                            Array.Clear(enc, 0, blockSize);
                        }
                        else if (mode is CipherMode.OFB)
                        {
                            byte[] enc = new byte[blockSize];
                            engine.EncryptBlock(feedback, 0, enc, 0);
                            Buffer.BlockCopy(enc, 0, feedback, 0, blockSize);
                            for (int j = 0; j < blockSize; j++)
                            {
                                result[posOut + j] = (byte)(all[pos + j] ^ enc[j]);
                            }

                            Array.Clear(enc, 0, blockSize);
                        }
                        else
                        {
                            throw new NotSupportedException("Cipher mode not supported for decryption in this implementation.");
                        }
                        posOut += blockSize;
                    }

                    // Now remove padding if padding != None and mode is block-oriented (ECB/CBC)
                    if (padding is PaddingMode.None || mode is CipherMode.CFB || mode is CipherMode.OFB)
                    {
                        // no padding removal
                    }
                    else if (padding is PaddingMode.PKCS7)
                    {
                        if (result.Length is 0)
                        {
                            throw new CryptographicException("Invalid data.");
                        }

                        int pad = result[result.Length - 1];
                        if (pad <= 0 || pad > blockSize)
                        {
                            throw new CryptographicException("Invalid padding.");
                        }

                        for (int i = result.Length - pad; i < result.Length; i++)
                        {
                            if (result[i] != (byte)pad)
                            {
                                throw new CryptographicException("Invalid padding.");
                            }
                        }

                        byte[] tmp = new byte[result.Length - pad];
                        Buffer.BlockCopy(result, 0, tmp, 0, tmp.Length);
                        result = tmp;
                    }
                    else if (padding is PaddingMode.Zeros)
                    {
                        int trim = result.Length;
                        while (trim > 0 && result[trim - 1] is 0) trim--;
                        byte[] tmp = new byte[trim];
                        Buffer.BlockCopy(result, 0, tmp, 0, tmp.Length);
                        result = tmp;
                    }
                    else if (padding is PaddingMode.ANSIX923)
                    {
                        if (result.Length is 0)
                        {
                            throw new CryptographicException("Invalid data.");
                        }

                        int pad = result[result.Length - 1];
                        if (pad <= 0 || pad > blockSize)
                        {
                            throw new CryptographicException("Invalid padding.");
                        }

                        for (int i = result.Length - pad; i < result.Length - 1; i++)
                        {
                            if (result[i] is not 0)
                            {
                                throw new CryptographicException("Invalid padding.");
                            }
                        }

                        byte[] tmp = new byte[result.Length - pad];
                        Buffer.BlockCopy(result, 0, tmp, 0, tmp.Length);
                        result = tmp;
                    }
                    else if (padding is PaddingMode.ISO10126)
                    {
                        if (result.Length is 0)
                        {
                            throw new CryptographicException("Invalid data.");
                        }

                        int pad = result[result.Length - 1];
                        if (pad <= 0 || pad > blockSize)
                        {
                            throw new CryptographicException("Invalid padding.");
                        }
                        // can't verify random bytes, just trim
                        byte[] tmp = new byte[result.Length - pad];
                        Buffer.BlockCopy(result, 0, tmp, 0, tmp.Length);
                        result = tmp;
                    }
                    else
                    {
                        throw new NotSupportedException("Unsupported padding mode.");
                    }
                }
            }

            // Clear internal buffer
            Array.Clear(all, 0, all.Length);
            Array.Clear(buffer, 0, buffer.Length);
            bufferCount = 0;

            return result;
        }

        /// <summary>
        /// helper ensuring capacity (used in a limited place)
        /// </summary>
        private static byte[] EnsureAndCopy(byte[] arr, int required)
        {
            if (arr is null || arr.Length < required)
            {
                byte[] n = new byte[required];
                if (arr is not null)
                {
                    Buffer.BlockCopy(arr, 0, n, 0, Math.Min(arr.Length, n.Length));
                }

                return n;
            }
            return arr;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                if (feedback is not null)
                {
                    Array.Clear(feedback, 0, feedback.Length);
                }

                if (iv is not null)
                {
                    Array.Clear(iv, 0, iv.Length);
                }

                if (buffer is not null)
                {
                    Array.Clear(buffer, 0, buffer.Length);
                }

                randomNumberGenerator.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
