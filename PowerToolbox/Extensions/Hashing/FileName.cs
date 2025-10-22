using PowerToolbox.Extensions.Buffers;
using PowerToolbox.Extensions.Memory;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    ///   表示一种非加密哈希算法。
    /// </summary>
    public abstract class NonCryptographicHashAlgorithm
    {
        /// <summary>
        ///   获取此哈希算法生成的字节数。
        /// </summary>
        /// <value>此哈希算法生成的字节数。</value>

        public int HashLengthInBytes { get; }

        /// <summary>
        ///   从派生类的构造函数调用以初始化
        ///   <see cref="NonCryptographicHashAlgorithm"/> 类。
        /// </summary>
        /// <param name="hashLengthInBytes">
        ///   此哈希算法生成的字节数。
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="hashLengthInBytes"/> 小于 1。
        /// </exception>
        protected NonCryptographicHashAlgorithm(int hashLengthInBytes)
        {
            if (hashLengthInBytes < 1)
                throw new ArgumentOutOfRangeException(nameof(hashLengthInBytes));

            HashLengthInBytes = hashLengthInBytes;
        }

        /// <summary>
        ///   当在派生类中重写时，
        ///   将 <paramref name="source"/> 的内容附加到当前哈希计算中已处理的数据。
        /// </summary>
        /// <param name="source">要处理的数据。</param>
        public abstract void Append(ReadOnlySpan<byte> source);

        /// <summary>
        ///   当在派生类中重写时，
        ///   将哈希计算重置为初始状态。
        /// </summary>
        public abstract void Reset();

        /// <summary>
        ///   当在派生类中重写时，
        ///   将计算出的哈希值写入 <paramref name="destination"/>
        ///   而不修改累积状态。
        /// </summary>
        /// <param name="destination">接收计算哈希值的缓冲区。</param>
        /// <remarks>
        ///   <para>
        ///     此方法的实现必须写入
        ///     <see cref="HashLengthInBytes"/> 字节到 <paramref name="destination"/>。
        ///     不要假设缓冲区已被零初始化。
        ///   </para>
        ///   <para>
        ///     <see cref="NonCryptographicHashAlgorithm"/> 类在调用此方法之前验证
        ///     缓冲区的大小，并将切片的跨度
        ///     降至正好 <see cref="HashLengthInBytes"/> 的长度。
        ///   </para>
        /// </remarks>
        protected abstract void GetCurrentHashCore(Span<byte> destination);

        /// <summary>
        ///   将 <paramref name="source"/> 的内容附加到当前哈希计算中已处理的数据。
        /// </summary>
        /// <param name="source">要处理的数据。</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="source"/> 为 <see langword="null"/>。
        /// </exception>
        public void Append(byte[] source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Append(new ReadOnlySpan<byte>(source));
        }

        /// <summary>
        ///   将 <paramref name="stream"/> 的内容附加到当前哈希计算中已处理的数据。
        /// </summary>
        /// <param name="stream">要处理的数据。</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="stream"/> 为 <see langword="null"/>。
        /// </exception>
        public void Append(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            byte[] buffer = ArrayPool<byte>.Shared.Rent(4096);

            while (true)
            {
                int read = stream.Read(buffer, 0, buffer.Length);

                if (read == 0)
                {
                    break;
                }

                Append(new ReadOnlySpan<byte>(buffer, 0, read));
            }

            ArrayPool<byte>.Shared.Return(buffer);
        }

        /// <summary>
        ///   异步读取 <paramref name="stream"/> 的内容
        ///   并将其附加到当前哈希计算中已处理的数据。
        /// </summary>
        /// <param name="stream">要处理的数据。</param>
        /// <param name="cancellationToken">
        ///   用于监视取消请求的令牌。
        ///   默认值为 <see cref="CancellationToken.None"/>。
        /// </param>
        /// <returns>
        ///   表示异步附加操作的任务。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="stream"/> 为 <see langword="null"/>。
        /// </exception>

        public Task AppendAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return AppendAsyncCore(stream, cancellationToken);
        }

        private async Task AppendAsyncCore(Stream stream, CancellationToken cancellationToken)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(4096);

            while (true)
            {
                int read = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);

                if (read == 0)
                {
                    break;
                }

                Append(new ReadOnlySpan<byte>(buffer, 0, read));
            }

            ArrayPool<byte>.Shared.Return(buffer);
        }

        /// <summary>
        ///   获取当前计算的哈希值而不修改累积状态。
        /// </summary>
        /// <returns>
        ///   已提供数据的哈希值。
        /// </returns>
        public byte[] GetCurrentHash()
        {
            byte[] ret = new byte[HashLengthInBytes];
            GetCurrentHashCore(ret);
            return ret;
        }

        /// <summary>
        ///   尝试将计算出的哈希值写入 <paramref name="destination"/>
        ///   而不修改累积状态。
        /// </summary>
        /// <param name="destination">接收计算哈希值的缓冲区。</param>
        /// <param name="bytesWritten">
        ///   成功时，接收写入 <paramref name="destination"/> 的字节数。
        /// </param>
        /// <returns>
        ///   <see langword="true"/> 如果 <paramref name="destination"/> 足够长以接收
        ///   计算的哈希值；否则 <see langword="false"/>。
        /// </returns>
        public bool TryGetCurrentHash(Span<byte> destination, out int bytesWritten)
        {
            if (destination.Length < HashLengthInBytes)
            {
                bytesWritten = 0;
                return false;
            }

            GetCurrentHashCore(destination.Slice(0, HashLengthInBytes));
            bytesWritten = HashLengthInBytes;
            return true;
        }

        /// <summary>
        ///   将计算出的哈希值写入 <paramref name="destination"/>
        ///   而不修改累积状态。
        /// </summary>
        /// <param name="destination">接收计算哈希值的缓冲区。</param>
        /// <returns>
        ///   写入 <paramref name="destination"/> 的字节数，
        ///   始终为 <see cref="HashLengthInBytes"/>。
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   <paramref name="destination"/> 短于 <see cref="HashLengthInBytes"/>。
        /// </exception>
        public int GetCurrentHash(Span<byte> destination)
        {
            if (destination.Length < HashLengthInBytes)
            {
                ThrowDestinationTooShort();
            }

            GetCurrentHashCore(destination.Slice(0, HashLengthInBytes));
            return HashLengthInBytes;
        }

        /// <summary>
        ///   获取当前计算的哈希值并清除累积状态。
        /// </summary>
        /// <returns>
        ///   已提供数据的哈希值。
        /// </returns>
        public byte[] GetHashAndReset()
        {
            byte[] ret = new byte[HashLengthInBytes];
            GetHashAndResetCore(ret);
            return ret;
        }

        /// <summary>
        ///   尝试将计算出的哈希值写入 <paramref name="destination"/>。
        ///   如果成功，清除累积状态。
        /// </summary>
        /// <param name="destination">接收计算哈希值的缓冲区。</param>
        /// <param name="bytesWritten">
        ///   成功时，接收写入 <paramref name="destination"/> 的字节数。
        /// </param>
        /// <returns>
        ///   <see langword="true"/> 并清除累积状态
        ///   如果 <paramref name="destination"/> 足够长以接收
        ///   计算的哈希值；否则 <see langword="false"/>。
        /// </returns>
        public bool TryGetHashAndReset(Span<byte> destination, out int bytesWritten)
        {
            if (destination.Length < HashLengthInBytes)
            {
                bytesWritten = 0;
                return false;
            }

            GetHashAndResetCore(destination.Slice(0, HashLengthInBytes));
            bytesWritten = HashLengthInBytes;
            return true;
        }

        /// <summary>
        ///   将计算出的哈希值写入 <paramref name="destination"/>
        ///   然后清除累积状态。
        /// </summary>
        /// <param name="destination">接收计算哈希值的缓冲区。</param>
        /// <returns>
        ///   写入 <paramref name="destination"/> 的字节数，
        ///   始终为 <see cref="HashLengthInBytes"/>。
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   <paramref name="destination"/> 短于 <see cref="HashLengthInBytes"/>。
        /// </exception>
        public int GetHashAndReset(Span<byte> destination)
        {
            if (destination.Length < HashLengthInBytes)
            {
                ThrowDestinationTooShort();
            }

            GetHashAndResetCore(destination.Slice(0, HashLengthInBytes));
            return HashLengthInBytes;
        }

        /// <summary>
        ///   将计算出的哈希值写入 <paramref name="destination"/>
        ///   然后清除累积状态。
        /// </summary>
        /// <param name="destination">接收计算哈希值的缓冲区。</param>
        /// <remarks>
        ///   <para>
        ///     此方法的实现必须写入
        ///     <see cref="HashLengthInBytes"/> 字节到 <paramref name="destination"/>。
        ///     不要假设缓冲区已被零初始化。
        ///   </para>
        ///   <para>
        ///     <see cref="NonCryptographicHashAlgorithm"/> 类在调用此方法之前验证
        ///     缓冲区的大小，并将切片的跨度
        ///     降至正好 <see cref="HashLengthInBytes"/> 的长度。
        ///   </para>
        ///   <para>
        ///     此方法的默认实现调用
        ///     <see cref="GetCurrentHashCore"/> 然后 <see cref="Reset"/>。
        ///     此方法的重写不需要调用这两个方法，
        ///     但必须确保调用者无法观察到行为的差异。
        ///   </para>
        /// </remarks>
        protected virtual void GetHashAndResetCore(Span<byte> destination)
        {
            Debug.Assert(destination.Length == HashLengthInBytes);

            GetCurrentHashCore(destination);
            Reset();
        }

        /// <summary>
        ///   此方法不受支持且不应被调用。
        ///   请改用 <see cref="GetCurrentHash()"/> 或 <see cref="GetHashAndReset()"/>
        ///   。
        /// </summary>
        /// <returns>此方法将始终引发 <see cref="NotSupportedException"/>。</returns>
        /// <exception cref="NotSupportedException">在所有情况下。</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            throw new NotSupportedException("Use GetCurrentHash() to retrieve the computed hash code.");
        }

        protected static void ThrowDestinationTooShort()
        {
            throw new ArgumentException("destination");
        }
    }
}
