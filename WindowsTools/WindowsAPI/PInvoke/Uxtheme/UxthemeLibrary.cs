﻿using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.PInvoke.Uxtheme
{
    /// <summary>
    /// Uxtheme.dll 函数库
    /// </summary>
    public static class UxthemeLibrary
    {
        private const string Uxtheme = "uxtheme.dll";

        /// <summary>
        /// 设置 win32 右键菜单的样式
        /// </summary>
        /// <param name="preferredAppMode">菜单样式</param>
        [DllImport(Uxtheme, CharSet = CharSet.Unicode, EntryPoint = "#135", SetLastError = false)]
        public static extern int SetPreferredAppMode(PreferredAppMode preferredAppMode);

        /// <summary>
        /// 刷新右键菜单样式
        /// </summary>
        [DllImport(Uxtheme, CharSet = CharSet.Unicode, EntryPoint = "#136", SetLastError = false)]
        public static extern int FlushMenuThemes();
    }
}
