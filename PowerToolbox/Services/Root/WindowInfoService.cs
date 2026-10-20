using Microsoft.Win32;
using PowerToolbox.Helpers.Root;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System.Drawing;

namespace PowerToolbox.Services.Root
{
    /// <summary>
    /// 窗口信息服务
    /// </summary>
    internal static class WindowInfoService
    {
        private static bool isInitialized;
        private static string windowInfoRegistryPath;

        /// <summary>
        /// 初始化窗口信息服务
        /// </summary>
        internal static void Initialize(string windowName)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                windowInfoRegistryPath = string.Format(@"Software\PowerToolbox\{0}\{1}", "WindowInfo", windowName);
            }
        }

        /// <summary>
        /// 检查窗口信息是否存在
        /// </summary>
        internal static bool IsWindowInformationExisted()
        {
            return isInitialized && RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, windowInfoRegistryPath, "WindowInformationExisted");
        }

        /// <summary>
        /// 获取窗口信息
        /// </summary>
        internal static (bool, Point, Size) GetWindowInfo()
        {
            if (IsWindowInformationExisted())
            {
                bool isWindowMaximized = RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, windowInfoRegistryPath, "IsWindowMaximized");
                int x = RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, windowInfoRegistryPath, "X");
                int y = RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, windowInfoRegistryPath, "Y");
                int width = RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, windowInfoRegistryPath, "Width");
                int height = RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, windowInfoRegistryPath, "Height");
                return (isWindowMaximized, new(x, y), new(width, height));
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 保存窗口信息
        /// </summary>
        internal static void SaveWindowInfo(nint hWnd, bool isWindowMaximized)
        {
            User32Library.GetWindowRect(hWnd, out RECT rect);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, windowInfoRegistryPath, "WindowInformationExisted", true);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, windowInfoRegistryPath, "X", rect.left);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, windowInfoRegistryPath, "Y", rect.top);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, windowInfoRegistryPath, "Width", rect.right - rect.left);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, windowInfoRegistryPath, "Height", rect.bottom - rect.top);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, windowInfoRegistryPath, "IsWindowMaximized", isWindowMaximized);
        }
    }
}
