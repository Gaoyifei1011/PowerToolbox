﻿using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace SwitchTheme.WindowsAPI.ComTypes
{
    /// <summary>
    /// 表示包清单的对象模型，该模型提供用于访问清单元素和属性的方法。
    /// </summary>
    [ComImport, Guid("D06F67BC-B31D-4EBA-A8AF-638E73E77B4D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAppxManifestReader2
    {
        /// <summary>
        /// 获取清单中定义的包标识符。
        /// </summary>
        /// <param name="packageId">包标识符。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetPackageId(out IntPtr packageId);

        /// <summary>
        /// 获取清单中定义的包的属性。
        /// </summary>
        /// <param name="packageProperties">包的属性，如清单所述。</param>
        /// <returns>如果该方法成功，则返回 S_OK。</returns>
        [PreserveSig]
        int GetProperties(out IntPtr packageProperties);

        /// <summary>
        /// 获取循环访问清单中定义的依赖项的枚举器。
        /// </summary>
        /// <param name="dependencies">循环访问依赖项的枚举器。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetPackageDependencies(out IntPtr dependencies);

        /// <summary>
        /// 获取包请求的功能列表。
        /// </summary>
        /// <param name="capabilities">包请求的功能列表。 这是枚举值的按位组合。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetCapabilities(out uint capabilities);

        /// <summary>
        /// 获取一个枚举器，该枚举器循环访问清单中定义的资源。
        /// </summary>
        /// <param name="resources">循环访问资源的枚举器。</param>
        /// <returns>如果方法成功，则返回 S_OK。</returns>
        [PreserveSig]
        int GetResources(out IntPtr resources);

        /// <summary>
        /// 获取循环访问清单中定义的设备功能的枚举器。
        /// </summary>
        /// <param name="deviceCapabilities">循环访问设备功能的枚举器。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetDeviceCapabilities(out IntPtr deviceCapabilities);

        /// <summary>
        /// 获取包清单中定义的指定先决条件。
        /// </summary>
        /// <param name="name">先决条件的名称，“OSMinVersion”或“OSMaxVersionTested”。</param>
        /// <param name="value">指定的先决条件。 在清单中，点三表示形式为 Major.Minor.AppPlatform。 这将转换为 64 位值，如下所示：最高顺序字包含主要版本。 下一个单词包含 Minor 版本。 下一个单词包含可选的 AppPlatform 版本（如果指定）。</param>
        /// <returns>此方法可以返回其中一个值。</returns>
        [PreserveSig]
        int GetPrerequisite([MarshalAs(UnmanagedType.LPWStr)] string name, out ulong value);

        /// <summary>
        /// 获取一个枚举器，该枚举器循环访问清单中定义的应用程序。
        /// </summary>
        /// <param name="applications">循环访问应用程序的枚举器。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetApplications(out IntPtr applications);

        /// <summary>
        /// 获取由清单读取者分析和读取的原始 XML。
        /// </summary>
        /// <param name="manifestStream">表示清单的 XML 内容的只读流。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetStream(out IStream manifestStream);

        /// <summary>
        /// 获取一个枚举器，该枚举器循环访问清单中定义的限定资源。
        /// </summary>
        /// <param name="resources">循环访问限定资源的枚举器。</param>
        /// <returns>如果该方法成功，则返回 S_OK。</returns>
        [PreserveSig]
        int GetQualifiedResources(out IAppxManifestQualifiedResourcesEnumerator resources);
    }
}
