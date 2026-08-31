namespace PowerToolbox.Extensions.Hashing
{
    /// <summary>
    /// Snefru 256 校验实现
    /// </summary>
    internal sealed class Snefru256 : SnefruBase
    {
        internal Snefru256() : base(SnefruOutputSize.Output8)
        {
            HashSizeValue = 256;
        }
    }
}
