namespace PowerToolbox.Extensions.DataType.Enums
{
    public enum UacLevel
    {
        /// <summary>
        /// 从不通知
        /// </summary>
        NeverNotify,

        /// <summary>
        /// 尝试更改时通知(不降低桌面亮度)
        /// </summary>
        NotifyWithoutDimming,

        /// <summary>
        /// 尝试更改时通知
        /// </summary>
        Notify,

        /// <summary>
        /// 始终通知
        /// </summary>
        AlwaysNotify
    }
}
