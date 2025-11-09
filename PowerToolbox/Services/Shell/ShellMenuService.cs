using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace PowerToolbox.Services.Shell
{
    /// <summary>
    /// 自定义扩展菜单服务
    /// </summary>
    public static class ShellMenuService
    {
        // Stable Software\PowerToolbox\Personalize\ShellMenu
        private static readonly string shellMenuKey = @"Software\PowerToolbox\ShellMenu";

        public static DirectoryInfo ShellMenuConfigDirectory { get; private set; }

        /// <summary>
        /// 初始化自定义扩展菜单配置
        /// </summary>
        public static void InitializeShellMenu()
        {
            Shell32Library.SHGetKnownFolderPath(new("F1B32785-6FBA-4FCF-9D55-7B8E7F157091"), KNOWN_FOLDER_FLAG.KF_FLAG_FORCE_APP_DATA_REDIRECTION, 0, out string localAppdataPath);

            if (!string.IsNullOrEmpty(localAppdataPath))
            {
                try
                {
                    if (Directory.Exists(localAppdataPath))
                    {
                        string shellMenuPath = Path.Combine(localAppdataPath, "ShellMenu");
                        ShellMenuConfigDirectory = Directory.CreateDirectory(shellMenuPath);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuService), nameof(InitializeShellMenu), 1, e);
                }
            }
        }

        /// <summary>
        /// 获取菜单项
        /// </summary>
        public static ShellMenuItem GetShellMenuItem()
        {
            // 获取根菜单项下的所有子项（包括递归后的项）
            RegistryEnumKeyItem shellMenuRegistryKeyItem = RegistryHelper.EnumSubKey(Registry.CurrentUser, shellMenuKey);

            return shellMenuRegistryKeyItem.SubRegistryKeyList.Count is 1 ? EnumShellMenuItem(shellMenuRegistryKeyItem.SubRegistryKeyList[0]) : null;
        }

        /// <summary>
        /// 保存菜单项设置
        /// </summary>
        public static void SaveShellMenuItem(string menuKey, ShellMenuItem shellMenuItem)
        {
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "MenuGuid", Convert.ToString(shellMenuItem.MenuGuid));
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "MenuTitleText", shellMenuItem.MenuTitleText);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "UseIcon", shellMenuItem.UseIcon);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "UseProgramIcon", shellMenuItem.UseProgramIcon);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "UseThemeIcon", shellMenuItem.UseThemeIcon);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "DefaultIconPath", shellMenuItem.DefaultIconPath);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "LightThemeIconPath", shellMenuItem.LightThemeIconPath);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "DarkThemeIconPath", shellMenuItem.DarkThemeIconPath);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "MenuProgramPath", shellMenuItem.MenuProgramPath);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "MenuParameter", shellMenuItem.MenuParameter);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "IsAlwaysRunAsAdministrator", shellMenuItem.IsAlwaysRunAsAdministrator);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "FolderBackground", shellMenuItem.FolderBackground);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "FolderDesktop", shellMenuItem.FolderDesktop);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "FolderDirectory", shellMenuItem.FolderDirectory);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "FolderDrive", shellMenuItem.FolderDrive);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "MenuFileMatchRule", shellMenuItem.MenuFileMatchRule);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "MenuFileMatchFormatText", shellMenuItem.MenuFileMatchFormatText);
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, menuKey, "MenuIndex", shellMenuItem.MenuIndex);
        }

        /// <summary>
        /// 删除菜单项
        /// </summary>
        public static void RemoveShellMenuItem(string menuKey)
        {
            Task.Run(() =>
            {
                // 获取当前菜单项下的所有子项（包括递归后的项）
                RegistryEnumKeyItem menuKeyItem = RegistryHelper.EnumSubKey(Registry.CurrentUser, menuKey);

                EnumRemoveMenuItem(menuKeyItem);
                RegistryHelper.RemoveRegistryKey(Registry.CurrentUser, menuKey);
            });
        }

        /// <summary>
        /// 枚举并递归所有子项
        /// </summary>
        private static ShellMenuItem EnumShellMenuItem(RegistryEnumKeyItem registryEnumKeyItem)
        {
            // 获取该项菜单内容
            ShellMenuItem currentMenuItem = GetShellItemInfo(registryEnumKeyItem.RootKey);
            ShellMenuItem shellMenuItem = new()
            {
                MenuKey = currentMenuItem.MenuKey,
                MenuGuid = currentMenuItem.MenuGuid,
                MenuTitleText = currentMenuItem.MenuTitleText,
                UseIcon = currentMenuItem.UseIcon,
                UseProgramIcon = currentMenuItem.UseProgramIcon,
                UseThemeIcon = currentMenuItem.UseThemeIcon,
                DefaultIconPath = currentMenuItem.DefaultIconPath,
                LightThemeIconPath = currentMenuItem.LightThemeIconPath,
                DarkThemeIconPath = currentMenuItem.DarkThemeIconPath,
                MenuProgramPath = currentMenuItem.MenuProgramPath,
                MenuParameter = currentMenuItem.MenuParameter,
                IsAlwaysRunAsAdministrator = currentMenuItem.IsAlwaysRunAsAdministrator,
                FolderBackground = currentMenuItem.FolderBackground,
                FolderDesktop = currentMenuItem.FolderDesktop,
                FolderDirectory = currentMenuItem.FolderDirectory,
                FolderDrive = currentMenuItem.FolderDrive,
                MenuFileMatchRule = currentMenuItem.MenuFileMatchRule,
                MenuFileMatchFormatText = currentMenuItem.MenuFileMatchFormatText,
                MenuIndex = currentMenuItem.MenuIndex
            };

            // 获取子菜单项
            if (registryEnumKeyItem.SubRegistryKeyList.Count > 0)
            {
                foreach (RegistryEnumKeyItem subRegistryKeyItem in registryEnumKeyItem.SubRegistryKeyList)
                {
                    shellMenuItem.SubShellMenuItem.Add(EnumShellMenuItem(subRegistryKeyItem));
                }

                // 递归获取完子列表后，根据列表的索引进行排序
                shellMenuItem.SubShellMenuItem.Sort((item1, item2) => item1.MenuIndex.CompareTo(item2.MenuIndex));
            }

            return shellMenuItem;
        }

        /// <summary>
        /// 枚举并递归删除所有该项及子项的资源文件
        /// </summary>
        private static void EnumRemoveMenuItem(RegistryEnumKeyItem registryEnumKeyItem)
        {
            // 获取该项菜单内容
            ShellMenuItem currentMenuItem = GetShellItemInfo(registryEnumKeyItem.RootKey);

            // 删除该项菜单的所有图标文件
            if (File.Exists(currentMenuItem.DefaultIconPath))
            {
                try
                {
                    FileSystem.DeleteFile(currentMenuItem.DefaultIconPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuService), nameof(EnumRemoveMenuItem), 1, e);
                }
            }

            if (File.Exists(currentMenuItem.LightThemeIconPath))
            {
                try
                {
                    FileSystem.DeleteFile(currentMenuItem.LightThemeIconPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuService), nameof(EnumRemoveMenuItem), 2, e);
                }
            }

            if (File.Exists(currentMenuItem.DarkThemeIconPath))
            {
                try
                {
                    FileSystem.DeleteFile(currentMenuItem.DarkThemeIconPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuService), nameof(EnumRemoveMenuItem), 3, e);
                }
            }

            // 查找图标对应的文件夹路径，并删除文件夹
            try
            {
                if (!string.IsNullOrEmpty(currentMenuItem.DefaultIconPath))
                {
                    string defaultIconDirectoryPath = Path.GetDirectoryName(currentMenuItem.DefaultIconPath);

                    if (Directory.Exists(defaultIconDirectoryPath))
                    {
                        FileSystem.DeleteDirectory(defaultIconDirectoryPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuService), nameof(EnumRemoveMenuItem), 4, e);
            }

            try
            {
                if (!string.IsNullOrEmpty(currentMenuItem.LightThemeIconPath))
                {
                    string lightThemeIconDirectoryPath = Path.GetDirectoryName(currentMenuItem.LightThemeIconPath);

                    if (Directory.Exists(lightThemeIconDirectoryPath))
                    {
                        FileSystem.DeleteDirectory(lightThemeIconDirectoryPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuService), nameof(EnumRemoveMenuItem), 5, e);
            }

            try
            {
                if (!string.IsNullOrEmpty(currentMenuItem.DarkThemeIconPath))
                {
                    string darkThemeIconDirectoryPath = Path.GetDirectoryName(currentMenuItem.DarkThemeIconPath);

                    if (Directory.Exists(darkThemeIconDirectoryPath))
                    {
                        FileSystem.DeleteDirectory(darkThemeIconDirectoryPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuService), nameof(EnumRemoveMenuItem), 6, e);
            }

            // 获取子菜单项
            if (registryEnumKeyItem.SubRegistryKeyList.Count > 0)
            {
                foreach (RegistryEnumKeyItem subRegistryKeyItem in registryEnumKeyItem.SubRegistryKeyList)
                {
                    EnumRemoveMenuItem(subRegistryKeyItem);
                }
            }
        }

        /// <summary>
        /// 获取菜单项信息
        /// </summary>
        private static ShellMenuItem GetShellItemInfo(string menuKey)
        {
            ShellMenuItem shellMenuItem = new();

            string menuTitleText = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, menuKey, "MenuTitleText");
            string menuGuid = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, menuKey, "MenuGuid");
            bool? useIcon = RegistryHelper.ReadRegistryKey<bool?>(Registry.CurrentUser, menuKey, "UseIcon");
            bool? useProgramIcon = RegistryHelper.ReadRegistryKey<bool?>(Registry.CurrentUser, menuKey, "UseProgramIcon");
            bool? useThemeIcon = RegistryHelper.ReadRegistryKey<bool?>(Registry.CurrentUser, menuKey, "UseThemeIcon");
            string defaultIconPath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, menuKey, "DefaultIconPath");
            string lightThemeIconPath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, menuKey, "LightThemeIconPath");
            string darkThemeIconPath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, menuKey, "DarkThemeIconPath");
            string menuProgramPath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, menuKey, "MenuProgramPath");
            string menuParameter = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, menuKey, "MenuParameter");
            bool? isAlwaysRunAsAdministrator = RegistryHelper.ReadRegistryKey<bool?>(Registry.CurrentUser, menuKey, "IsAlwaysRunAsAdministrator");
            bool? folderBackground = RegistryHelper.ReadRegistryKey<bool?>(Registry.CurrentUser, menuKey, "FolderBackground");
            bool? folderDesktop = RegistryHelper.ReadRegistryKey<bool?>(Registry.CurrentUser, menuKey, "FolderDesktop");
            bool? folderDirectory = RegistryHelper.ReadRegistryKey<bool?>(Registry.CurrentUser, menuKey, "FolderDirectory");
            bool? folderDrive = RegistryHelper.ReadRegistryKey<bool?>(Registry.CurrentUser, menuKey, "FolderDrive");
            string menuFileMatchRule = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, menuKey, "MenuFileMatchRule");
            string menuFileMatchFormatText = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, menuKey, "MenuFileMatchFormatText");
            int? menuIndex = RegistryHelper.ReadRegistryKey<int?>(Registry.CurrentUser, menuKey, "MenuIndex");

            shellMenuItem.MenuKey = menuKey;
            shellMenuItem.MenuGuid = string.IsNullOrEmpty(menuGuid) ? Guid.Empty : new Guid(menuGuid);
            shellMenuItem.MenuTitleText = menuTitleText;
            shellMenuItem.UseIcon = useIcon.HasValue && useIcon.Value;
            shellMenuItem.UseProgramIcon = useProgramIcon.HasValue && useProgramIcon.Value;
            shellMenuItem.UseThemeIcon = useThemeIcon.HasValue && useThemeIcon.Value;
            shellMenuItem.DefaultIconPath = defaultIconPath;
            shellMenuItem.LightThemeIconPath = lightThemeIconPath;
            shellMenuItem.DarkThemeIconPath = darkThemeIconPath;
            shellMenuItem.MenuProgramPath = menuProgramPath;
            shellMenuItem.DarkThemeIconPath = darkThemeIconPath;
            shellMenuItem.MenuProgramPath = menuProgramPath;
            shellMenuItem.MenuParameter = menuParameter;
            shellMenuItem.IsAlwaysRunAsAdministrator = isAlwaysRunAsAdministrator.HasValue && isAlwaysRunAsAdministrator.Value;
            shellMenuItem.FolderBackground = folderBackground.HasValue && folderBackground.Value;
            shellMenuItem.FolderDesktop = folderDesktop.HasValue && folderDesktop.Value;
            shellMenuItem.FolderDirectory = folderDirectory.HasValue && folderDirectory.Value;
            shellMenuItem.FolderDrive = folderDrive.HasValue && folderDrive.Value;
            shellMenuItem.MenuFileMatchRule = menuFileMatchRule;
            shellMenuItem.MenuFileMatchFormatText = menuFileMatchFormatText;
            shellMenuItem.MenuIndex = menuIndex ?? 0;

            return shellMenuItem;
        }

        /// <summary>
        /// 获取上次更新的时间
        /// </summary>
        public static DateTimeOffset GetLastUpdateTime()
        {
            int? lastUpdateTime = RegistryHelper.ReadRegistryKey<int?>(Registry.CurrentUser, "ShellMenu", "LastUpdateTime");
            return lastUpdateTime.HasValue ? DateTimeOffset.FromUnixTimeSeconds(lastUpdateTime.Value) : DateTimeOffset.FromUnixTimeSeconds(0);
        }

        /// <summary>
        /// 更新上次更新的时间
        /// </summary>
        public static void UpdateLastUpdateTime()
        {
            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, "ShellMenu", "LastUpdateTime", DateTimeOffset.Now.ToUnixTimeSeconds());
        }
    }
}
