namespace PowerToolbox.WindowsAPI.PInvoke.KernelAppCore
{
    /// <summary>
    /// 指示要在查询中检索路径或有关包的其他信息的文件夹路径的类型。
    /// </summary>
    internal enum PackagePathType
    {
        /// <summary>
        /// 检索应用程序的原始安装文件夹中的包路径。
        /// </summary>
		PackagePathType_Install = 0,

        /// <summary>
        /// 如果应用程序在包清单中声明为可变，则检索应用程序的可变安装文件夹中的包路径。
        /// </summary>
        PackagePathType_Mutable = 1,

        /// <summary>
        /// 如果应用程序在包清单中声明为可变，则检索可变文件夹中的包路径;如果应用程序不可变，则检索原始安装文件夹中的包路径。
        /// </summary>
        PackagePathType_Effective = 2,

        PackagePathType_MachineExternal = 3,

        PackagePathType_UserExternal = 4,

        PackagePathType_EffectiveExternal = 5,
    }
}
