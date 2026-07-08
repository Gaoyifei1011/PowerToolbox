namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 要运行应用的 GPU 首选项。
    /// </summary>
    public enum DXGI_GPU_PREFERENCE
    {
        /// <summary>
        /// 无 GPU 首选项。
        /// </summary>
        DXGI_GPU_PREFERENCE_UNSPECIFIED = 0,

        /// <summary>
        /// 首选最低功率 GPU (，例如集成图形处理器或 iGPU) 。
        /// </summary>
        DXGI_GPU_PREFERENCE_MINIMUM_POWER = 1,

        /// <summary>
        /// 首选性能最高的 GPU，例如离散图形处理器 (dGPU) 或外部图形处理器 (xGPU) 。
        /// </summary>
        DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE = 2
    }
}
