namespace PowerToolbox.WindowsAPI.PInvoke.KernelAppCore
{
    /// <summary>
    /// 包的处理器体系结构。
    /// </summary>
    internal enum PROCESSOR_ARCHITECTURE : uint
    {
        /// <summary>
        /// x86 处理器体系结构。
        /// </summary>
        PROCESSOR_ARCHITECTURE_X86 = 0,

        /// <summary>
        /// ARM 处理器体系结构。
        /// </summary>
        PROCESSOR_ARCHITECTURE_ARM = 5,

        /// <summary>
        /// x64 处理器体系结构。
        /// </summary>
        PROCESSOR_ARCHITECTURE_AMD64 = 9,

        /// <summary>
        /// 非特定处理器体系结构。
        /// </summary>
        PROCESSOR_ARCHITECTURE_NEUTRAL = 11,

        /// <summary>
        /// Arm64 处理器体系结构。
        /// </summary>
        PROCESSOR_ARCHITECTURE_ARM64 = 12,

        /// <summary>
        /// 模拟 X86 体系结构的 Arm64 处理器体系结构
        /// </summary>
        PROCESSOR_ARCHITECTURE_IA32_ON_ARM64 = 14,

        /// <summary>
        /// 未知的处理器体系结构。
        /// </summary>
        PROCESSOR_ARCHITECTURE_UNKNOWN = 65535
    }
}
