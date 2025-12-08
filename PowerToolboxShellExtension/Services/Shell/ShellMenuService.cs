using PowerToolboxShellExtension.Extensions.Registry;
using PowerToolboxShellExtension.Extensions.ShellMenu;
using PowerToolboxShellExtension.Helpers.Root;
using PowerToolboxShellExtension.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;

namespace PowerToolboxShellExtension.Services.Shell
{
    /// <summary>
    /// 自定义扩展菜单服务
    /// </summary>
    public static class ShellMenuService
    {
        // 对应的键值：Software\PowerToolbox\Personalize\ShellMenu
        private static readonly string shellMenuKey = @"Software\PowerToolbox\ShellMenu";

        public static DirectoryInfo ShellMenuConfigDirectory { get; private set; }

        public static ShellMenuItem RootShellMenuItem { get; private set; }

        public static List<string> FileMatchRuleList { get; } = ["None", "Name", "NameRegex", "Extension", "All"];

        /// <summary>
        /// 初始化自定义扩展菜单配置
        /// </summary>
        public static void InitializeShellMenu()
        {
            Shell32Library.SHGetKnownFolderPath(new("F1B32785-6FBA-4FCF-9D55-7B8E7F157091"), KNOWN_FOLDER_FLAG.KF_FLAG_FORCE_APP_DATA_REDIRECTION, nint.Zero, out string localAppDataPath);

            if (!string.IsNullOrEmpty(localAppDataPath))
            {
                try
                {
                    if (Directory.Exists(localAppDataPath))
                    {
                        string shellMenuPath = Path.Combine(localAppDataPath, "ShellMenu");
                        ShellMenuConfigDirectory = Directory.CreateDirectory(shellMenuPath);
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    return;
                }
            }

            RootShellMenuItem = GetShellMenuItem();
        }

        /// <summary>
        /// 获取菜单项
        /// </summary>
        public static ShellMenuItem GetShellMenuItem()
        {
            // 获取根菜单项下的所有子项（包括递归后的项）
            RegistryEnumKeyItem shellMenuRegistryKeyItem = RegistryHelper.EnumSubKey(shellMenuKey);

            return shellMenuRegistryKeyItem.SubRegistryKeyList.Count is 1 ? EnumShellMenuItem(shellMenuRegistryKeyItem.SubRegistryKeyList[0]) : null;
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
        /// 获取菜单项信息
        /// </summary>
        private static ShellMenuItem GetShellItemInfo(string menuKey)
        {
            ShellMenuItem shellMenuItem = new();

            string menuTitleText = RegistryHelper.ReadRegistryKey<string>(menuKey, "MenuTitleText");
            string menuGuid = RegistryHelper.ReadRegistryKey<string>(menuKey, "MenuGuid");
            bool? useIcon = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "UseIcon");
            bool? useProgramIcon = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "UseProgramIcon");
            bool? useThemeIcon = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "UseThemeIcon");
            string defaultIconPath = RegistryHelper.ReadRegistryKey<string>(menuKey, "DefaultIconPath");
            string lightThemeIconPath = RegistryHelper.ReadRegistryKey<string>(menuKey, "LightThemeIconPath");
            string darkThemeIconPath = RegistryHelper.ReadRegistryKey<string>(menuKey, "DarkThemeIconPath");
            string menuProgramPathText = RegistryHelper.ReadRegistryKey<string>(menuKey, "MenuProgramPath");
            string menuParameter = RegistryHelper.ReadRegistryKey<string>(menuKey, "MenuParameter");
            bool? isAlwaysRunAsAdministrator = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "IsAlwaysRunAsAdministrator");
            bool? folderBackground = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "FolderBackground");
            bool? folderDesktop = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "FolderDesktop");
            bool? folderDirectory = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "FolderDirectory");
            bool? folderDrive = RegistryHelper.ReadRegistryKey<bool?>(menuKey, "FolderDrive");
            string menuFileMatchRule = RegistryHelper.ReadRegistryKey<string>(menuKey, "MenuFileMatchRule");
            string menuFileMatchFormatText = RegistryHelper.ReadRegistryKey<string>(menuKey, "MenuFileMatchFormatText");
            int? menuIndex = RegistryHelper.ReadRegistryKey<int?>(menuKey, "MenuIndex");

            shellMenuItem.MenuKey = menuKey;
            shellMenuItem.MenuGuid = string.IsNullOrEmpty(menuGuid) ? Guid.Empty : new Guid(menuGuid);
            shellMenuItem.MenuTitleText = menuTitleText;
            shellMenuItem.UseIcon = useIcon.HasValue && useIcon.Value;
            shellMenuItem.UseProgramIcon = useProgramIcon.HasValue && useProgramIcon.Value;
            shellMenuItem.UseThemeIcon = useThemeIcon.HasValue && useThemeIcon.Value;
            shellMenuItem.DefaultIconPath = defaultIconPath;
            shellMenuItem.LightThemeIconPath = lightThemeIconPath;
            shellMenuItem.DarkThemeIconPath = darkThemeIconPath;
            shellMenuItem.MenuProgramPath = menuProgramPathText;
            shellMenuItem.DarkThemeIconPath = darkThemeIconPath;
            shellMenuItem.MenuProgramPath = menuProgramPathText;
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
    }
}
