using Microsoft.UI.Xaml.Media;
using System;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 网络回环数据模型
    /// </summary>
    internal sealed class LoopbackModel
    {
        /// <summary>
        /// 应用图标
        /// </summary>
        internal ImageSource AppIcon { get; set; }

        /// <summary>
        /// 应用图标路径
        /// </summary>
        internal Uri PackageIconUri { get; set; }

        internal bool IsOldChecked { get; set; }

        /// <summary>
        /// 应用程序运行的二进制路径
        /// </summary>
        internal string AppBinariesPath { get; set; }

        /// <summary>
        /// 应用容器的全局唯一名称
        /// </summary>
        internal string AppContainerName { get; set; }

        /// <summary>
        /// 应用容器的友好名称
        /// </summary>
        internal string DisplayName { get; set; }

        /// <summary>
        /// 应用容器其用途的说明、使用该容器的应用程序的目标等
        /// </summary>
        internal string Description { get; set; }

        internal string PackageFullName { get; set; }

        /// <summary>
        /// 应用容器的工作目录
        /// </summary>
        internal string WorkingDirectory { get; set; }

        /// <summary>
        /// 应用容器所属用户的名称
        /// </summary>
        internal string AppContainerUserName { get; set; }

        /// <summary>
        /// 应用容器的包标识符
        /// </summary>
        internal nint AppContainerSID { get; set; }

        /// <summary>
        /// 应用容器的包标识符名称
        /// </summary>
        internal string AppContainerSIDName { get; set; }
    }
}
