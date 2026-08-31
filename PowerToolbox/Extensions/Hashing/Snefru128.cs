namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// Snefru 128 校验实现
    /// Ralph C. Merkle (1990). "A fast software one-way hash function"
    /// </summary>
    internal sealed class Snefru128 : SnefruBase
    {
        internal Snefru128() : base(SnefruOutputSize.Output4)
        {
            HashSizeValue = 128;
        }
    }
}
