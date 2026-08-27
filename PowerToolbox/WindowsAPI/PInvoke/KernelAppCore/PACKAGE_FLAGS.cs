using System;

namespace PowerToolbox.WindowsAPI.PInvoke.KernelAppCore
{
    /// <summary>
    /// 指定如何处理包。
    /// </summary>
    [Flags]
    internal enum PACKAGE_FLAGS : uint
    {
        /// <summary>
        /// 包中应用的最大数目。
        /// </summary>
        PACKAGE_APPLICATIONS_MAX_COUNT = 100,

        /// <summary>
        /// 包中的最小应用数。
        /// </summary>
        PACKAGE_APPLICATIONS_MIN_COUNT = 0,

        /// <summary>
        /// 包可以包含的最大资源包数。
        /// </summary>
        PACKAGE_FAMILY_MAX_RESOURCE_PACKAGES = 512,

        /// <summary>
        /// 包可以具有的最小资源包数。
        /// </summary>
        PACKAGE_FAMILY_MIN_RESOURCE_PACKAGES = PACKAGE_APPLICATIONS_MIN_COUNT,

        /// <summary>
        /// 处理依赖项关系图中的所有包。这等效于 PACKAGE_FILTER_DIRECT PACKAGE_FILTER_HEAD。
        /// 注意 PACKAGE_FILTER_ALL_LOADED可能在Windows 8.1后更改或不可用。 请改用 PACKAGE_FILTER_HEADPACKAGE_FILTER_DIRECT。
        /// </summary>
        PACKAGE_FILTER_ALL_LOADED = PACKAGE_APPLICATIONS_MIN_COUNT,

        /// <summary>
        /// 处理包图中的捆绑包。
        /// </summary>
        PACKAGE_FILTER_BUNDLE = 0x00000080,

        /// <summary>
        /// 处理头 (依赖项关系图中第一个) 包的直接依赖包。
        /// </summary>
        PACKAGE_FILTER_DIRECT = 0x00000020,

        /// <summary>
        /// 动态添加到包图中的处理包。
        /// </summary>
        PACKAGE_FILTER_DYNAMIC = 0x00100000,

        /// <summary>
        /// 处理头 (依赖项关系图中的第一个) 包。
        /// </summary>
        PACKAGE_FILTER_HEAD = 0x00000010,

        /// <summary>
        /// 添加到包关系图的进程主机运行时依赖项包。
        /// </summary>
        PACKAGE_FILTER_HOSTRUNTIME = 0x00200000,

        /// <summary>
        /// 处理相关集中的包。
        /// </summary>
        PACKAGE_FILTER_IS_IN_RELATED_SET = 0x00040000,

        /// <summary>
        /// 处理包图中的可选包。
        /// </summary>
        PACKAGE_FILTER_OPTIONAL = 0x00020000,

        /// <summary>
        /// 处理包图中的资源包。
        /// </summary>
        PACKAGE_FILTER_RESOURCE = 0x00000040,

        /// <summary>
        /// 包图的最大大小。
        /// </summary>
        PACKAGE_GRAPH_MAX_SIZE = 1 + PACKAGE_MAX_DEPENDENCIES + PACKAGE_FAMILY_MAX_RESOURCE_PACKAGES,

        /// <summary>
        /// 包图的最小大小。
        /// </summary>
        PACKAGE_GRAPH_MIN_SIZE = 1,

        /// <summary>
        /// 检索基本信息。
        /// </summary>
        PACKAGE_INFORMATION_BASIC = PACKAGE_APPLICATIONS_MIN_COUNT,

        /// <summary>
        /// 检索完整信息。
        /// </summary>
        PACKAGE_INFORMATION_FULL = 0x00000100,

        /// <summary>
        /// 包所依赖的最大包数。
        /// </summary>
        PACKAGE_MAX_DEPENDENCIES = PACKAGE_FILTER_BUNDLE,

        /// <summary>
        /// 包所依赖的包的最小数目。
        /// </summary>
        PACKAGE_MIN_DEPENDENCIES = PACKAGE_APPLICATIONS_MIN_COUNT,

        /// <summary>
        /// 包是捆绑包。
        /// </summary>
        PACKAGE_PROPERTY_BUNDLE = 0x00000004,

        /// <summary>
        /// 包已注册到 DeploymentOptions 枚举。
        /// </summary>
        PACKAGE_PROPERTY_DEVELOPMENT_MODE = 0x00010000,

        /// <summary>
        /// 包是动态依赖项。
        /// </summary>
        PACKAGE_PROPERTY_DYNAMIC = PACKAGE_FILTER_DYNAMIC,

        /// <summary>
        /// 包是一个框架。
        /// </summary>
        PACKAGE_PROPERTY_FRAMEWORK = PACKAGE_GRAPH_MIN_SIZE,

        /// <summary>
        /// 包是主机运行时依赖项。
        /// </summary>
        PACKAGE_PROPERTY_HOSTRUNTIME = PACKAGE_FILTER_HOSTRUNTIME,

        /// <summary>
        /// 包位于相关集中。
        /// </summary>
        PACKAGE_PROPERTY_IS_IN_RELATED_SET = PACKAGE_FILTER_IS_IN_RELATED_SET,

        /// <summary>
        /// 包是可选包。
        /// </summary>
        PACKAGE_PROPERTY_OPTIONAL = 0x00000008,

        /// <summary>
        /// 包是资源包。
        /// </summary>
        PACKAGE_PROPERTY_RESOURCE = 0x00000002,

        /// <summary>
        /// 包是静态依赖项。
        /// </summary>
        PACKAGE_PROPERTY_STATIC = 0x00080000
    }
}
