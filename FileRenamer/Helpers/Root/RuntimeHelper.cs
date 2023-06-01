﻿using FileRenamer.WindowsAPI.PInvoke.Kernel32;

namespace FileRenamer.Helpers.Root
{
    /// <summary>
    /// 运行时辅助类
    /// </summary>
    public static class RuntimeHelper
    {
        /// <summary>
        /// 判断应用是否在 Msix 容器中运行
        /// </summary>
        public static unsafe bool IsMsix()
        {
            int length = 0;
            return Kernel32Library.GetCurrentPackageFullName(ref length, null) != Kernel32Library.APPMODEL_ERROR_NO_PACKAGE;
        }
    }
}
