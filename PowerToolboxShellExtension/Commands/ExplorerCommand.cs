using PowerToolboxShellExtension.Extensions.ShellMenu;
using PowerToolboxShellExtension.Helpers.Root;
using PowerToolboxShellExtension.Services.Controls.Settings;
using PowerToolboxShellExtension.Services.Shell;
using PowerToolboxShellExtension.WindowsAPI.ComTypes;
using PowerToolboxShellExtension.WindowsAPI.PInvoke.Shell32;
using PowerToolboxShellExtension.WindowsAPI.PInvoke.Shlwapi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text.RegularExpressions;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace PowerToolboxShellExtension.Commands
{
    /// <summary>
    /// 菜单项
    /// </summary>
    [GeneratedComClass, Guid("C169E7D7-21E1-A102-602E-3F3936BEBB58")]
    public partial class ExplorerCommand(ShellMenuItem shellMenuItem) : IExplorerCommand, IObjectWithSite
    {
        private readonly Guid SID_SFolderView = new("CDE725B0-CCC9-4519-917E-325D72FAB4CE");
        private readonly Guid IID_IUnknown = new("00000000-0000-0000-C000-000000000046");
        private nint site = nint.Zero;

        [GeneratedRegex("""{files-split:'([\s\S]*?)'}""", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-us")]
        private static partial Regex MultiFileRegex { get; }

        [GeneratedRegex("""{folders-split:'([\s\S]*?)'}""")]
        private static partial Regex MultiFolderRegex { get; }

        /// <summary>
        /// 根菜单标题
        /// </summary>
        public int GetTitle(IShellItemArray psiItemArray, out string ppszName)
        {
            if (shellMenuItem is not null)
            {
                ppszName = shellMenuItem.MenuTitleText;
                return 0;
            }
            else
            {
                ppszName = string.Empty;
                return unchecked((int)0x80004001);
            }
        }

        /// <summary>
        /// 根菜单图标路径
        /// </summary>
        public int GetIcon(IShellItemArray psiItemArray, out string ppszIcon)
        {
            if (shellMenuItem is not null)
            {
                // 是否使用图标
                if (shellMenuItem.UseIcon)
                {
                    // 是否使用应用程序图标
                    if (shellMenuItem.UseProgramIcon)
                    {
                        if (File.Exists(shellMenuItem.MenuProgramPath))
                        {
                            ppszIcon = shellMenuItem.MenuProgramPath;
                            return 0;
                        }
                        else
                        {
                            ppszIcon = string.Empty;
                            return unchecked((int)0x80004001);
                        }
                    }
                    else
                    {
                        // 是否使用主题图标
                        if (shellMenuItem.UseThemeIcon)
                        {
                            // 浅色主题图标
                            if (ThemeHelper.AppsUseLightTheme)
                            {
                                if (File.Exists(shellMenuItem.LightThemeIconPath))
                                {
                                    ppszIcon = shellMenuItem.LightThemeIconPath;
                                    return 0;
                                }
                                else
                                {
                                    ppszIcon = string.Empty;
                                    return unchecked((int)0x80004001);
                                }
                            }
                            else
                            {
                                // 深色主题图标
                                if (File.Exists(shellMenuItem.DarkThemeIconPath))
                                {
                                    ppszIcon = shellMenuItem.DarkThemeIconPath;
                                    return 0;
                                }
                                else
                                {
                                    ppszIcon = string.Empty;
                                    return unchecked((int)0x80004001);
                                }
                            }
                        }
                        else
                        {
                            // 默认图标
                            if (File.Exists(shellMenuItem.DefaultIconPath))
                            {
                                ppszIcon = shellMenuItem.DefaultIconPath;
                                return 0;
                            }
                            else
                            {
                                ppszIcon = string.Empty;
                                return unchecked((int)0x80004001);
                            }
                        }
                    }
                }
                else
                {
                    ppszIcon = string.Empty;
                    return unchecked((int)0x80004001);
                }
            }
            else
            {
                ppszIcon = string.Empty;
                return unchecked((int)0x80004001);
            }
        }

        /// <summary>
        /// 根菜单工具提示
        /// </summary>
        public int GetToolTip(IShellItemArray psiItemArray, out string ppszInfotip)
        {
            ppszInfotip = string.Empty;
            return unchecked((int)0x80004001);
        }

        /// <summary>
        /// 根菜单命令 ID
        /// </summary>
        public int GetCanonicalName(out Guid pguidCommandName)
        {
            pguidCommandName = typeof(ExplorerCommand).GUID;
            return 0;
        }

        /// <summary>
        /// 根菜单命令状态（根据应用设置来决定是否显示菜单）
        /// </summary>
        public unsafe int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState)
        {
            if (FileShellMenuService.GetFileShellMenu() && shellMenuItem is not null)
            {
                // Directory\Background
                if (psiItemArray is null && !site.Equals(nint.Zero))
                {
                    string folderPath = string.Empty;

                    // 查询点击背景时对应的文件夹路径
                    Marshal.QueryInterface(site, typeof(WindowsAPI.ComTypes.IServiceProvider).GUID, out nint serviceProviderPtr);
                    WindowsAPI.ComTypes.IServiceProvider serviceProvider = ComInterfaceMarshaller<WindowsAPI.ComTypes.IServiceProvider>.ConvertToManaged((void*)serviceProviderPtr);

                    serviceProvider.QueryService(SID_SFolderView, typeof(IFolderView).GUID, out nint folderViewPtr);
                    if (!folderViewPtr.Equals(nint.Zero))
                    {
                        IFolderView folderView = ComInterfaceMarshaller<IFolderView>.ConvertToManaged((void*)folderViewPtr);
                        folderView.GetFolder(typeof(IShellItem).GUID, out nint iShellItemPtr);

                        IShellItem shellItem = ComInterfaceMarshaller<IShellItem>.ConvertToManaged((void*)iShellItemPtr);
                        shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out folderPath);
                    }

                    // 检查点击背景右键菜单是来自桌面的还是普通目录
                    if (!string.IsNullOrEmpty(folderPath))
                    {
                        string standardDesktopPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                        string clickedPath = Path.GetFullPath(folderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                        pCmdState = string.Equals(standardDesktopPath, clickedPath, StringComparison.OrdinalIgnoreCase) ? EnumerateMatchFolderDesktop(shellMenuItem) : EnumerateMatchFolderDirectory(shellMenuItem);
                    }
                    else
                    {
                        pCmdState = EXPCMDSTATE.ECS_HIDDEN;
                    }
                }
                // 其他：文件或目录
                else
                {
                    if (psiItemArray.GetCount(out uint count) is 0 && count >= 1 && psiItemArray.GetItemAt(0, out IShellItem shellItem) is 0)
                    {
                        shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string filePath);
                        FileInfo fileInfo = new(filePath);

                        // 选中的第一个是目录
                        if ((fileInfo.Attributes & FileAttributes.Directory) is not 0)
                        {
                            string rootPath = Path.GetPathRoot(filePath);

                            // 相同，为驱动器目录
                            pCmdState = string.Equals(filePath, rootPath, StringComparison.OrdinalIgnoreCase) ? EnumerateMatchFolderDrive(shellMenuItem) : EnumerateMatchFolderDirectory(shellMenuItem);
                        }
                        // 选中的第一个是文件
                        else
                        {
                            pCmdState = EnumerateMatchFile(shellMenuItem, filePath);
                        }
                    }
                    else
                    {
                        pCmdState = EXPCMDSTATE.ECS_HIDDEN;
                    }
                }
            }
            else
            {
                pCmdState = EXPCMDSTATE.ECS_HIDDEN;
            }

            return 0;
        }

        /// <summary>
        /// 根菜单命令响应处理
        /// </summary>
        public unsafe int Invoke(IShellItemArray psiItemArray, nint pbc)
        {
            // 没有子菜单，该菜单可以直接调用命令
            if (shellMenuItem is not null && shellMenuItem.SubShellMenuItem.Count is 0)
            {
                int fileMatchedCount = 0;
                int folderMatchedCount = 0;
                string selectedFilePath = string.Empty;
                string selectedFolderPath = string.Empty;
                List<string> fileList = [];
                List<string> folderList = [];
                List<string> multiFileMatchedStringList = [];
                List<string> multiFolderMatchedStringList = [];
                List<string> replaceFileStringList = [];
                List<string> replaceFolderStringList = [];

                // psiItemArray 为空，可能为背景。查询点击背景时对应的文件夹路径
                if (psiItemArray is null)
                {
                    Marshal.QueryInterface(site, typeof(WindowsAPI.ComTypes.IServiceProvider).GUID, out nint serviceProviderPtr);
                    WindowsAPI.ComTypes.IServiceProvider serviceProvider = ComInterfaceMarshaller<WindowsAPI.ComTypes.IServiceProvider>.ConvertToManaged((void*)serviceProviderPtr);

                    serviceProvider.QueryService(SID_SFolderView, typeof(IFolderView).GUID, out nint folderViewPtr);
                    if (!folderViewPtr.Equals(nint.Zero))
                    {
                        IFolderView folderView = ComInterfaceMarshaller<IFolderView>.ConvertToManaged((void*)folderViewPtr);
                        folderView.GetFolder(typeof(IShellItem).GUID, out nint iShellItemPtr);

                        IShellItem shellItem = ComInterfaceMarshaller<IShellItem>.ConvertToManaged((void*)iShellItemPtr);
                        shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string filePath);
                        selectedFolderPath = filePath;
                        folderList.Add(filePath);
                    }
                }
                // psiItemArray不为空，正常获取所有选中的文件或目录
                else if (psiItemArray.GetCount(out uint count) is 0)
                {
                    for (uint index = 0; index < count; index++)
                    {
                        if (psiItemArray.GetItemAt(index, out IShellItem shellItem) is 0)
                        {
                            shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string filePath);
                            FileInfo fileInfo = new(filePath);

                            // 当前项是目录
                            if ((fileInfo.Attributes & FileAttributes.Directory) is not 0)
                            {
                                folderList.Add(filePath);
                                if (index is 0)
                                {
                                    selectedFolderPath = filePath;
                                }
                            }
                            // 当前项是文件
                            else
                            {
                                fileList.Add(filePath);
                                if (index is 0)
                                {
                                    selectedFilePath = filePath;
                                }
                            }
                        }
                    }
                }
                // 读取参数
                string parameter = string.IsNullOrEmpty(shellMenuItem.MenuParameter) ? string.Empty : shellMenuItem.MenuParameter;

                // 替换模板参数
                parameter = parameter.Replace("{file}", selectedFilePath);
                parameter = parameter.Replace("{folder}", selectedFolderPath);

                MatchCollection multiFileMatchCollection = MultiFileRegex.Matches(parameter);
                MatchCollection multiFolderMatchCollection = MultiFolderRegex.Matches(parameter);

                // 匹配文件
                foreach (Match multiFileMatchItem in multiFileMatchCollection)
                {
                    GroupCollection multiFileGroupCollection = multiFileMatchItem.Groups;

                    if (multiFileGroupCollection.Count is 2)
                    {
                        multiFileMatchedStringList.Add(multiFileGroupCollection[0].Value);
                        replaceFileStringList.Add(multiFileGroupCollection[1].Value);
                        fileMatchedCount++;
                    }
                }

                // 匹配目录
                foreach (Match multiFolderMatchItem in multiFolderMatchCollection)
                {
                    GroupCollection multiFolderGroupCollection = multiFolderMatchItem.Groups;

                    if (multiFolderGroupCollection.Count is 2)
                    {
                        multiFolderMatchedStringList.Add(multiFolderGroupCollection[0].Value);
                        replaceFolderStringList.Add(multiFolderGroupCollection[1].Value);
                        folderMatchedCount++;
                    }
                }

                // 替换匹配到的所有文件
                for (int index = 0; index < fileMatchedCount; index++)
                {
                    string fileString = string.Join(replaceFileStringList[index], fileList);
                    parameter = parameter.Replace(multiFileMatchedStringList[index], fileString);
                }

                // 替换匹配到的所有目录
                for (int index = 0; index < folderMatchedCount; index++)
                {
                    string folderString = string.Join(replaceFolderStringList[index], folderList);
                    parameter = parameter.Replace(multiFolderMatchedStringList[index], folderString);
                }

                ShlwapiLibrary.IUnknown_GetWindow(site, out nint hwnd);
                if (!string.IsNullOrEmpty(selectedFilePath))
                {
                    if (shellMenuItem.IsAlwaysRunAsAdministrator)
                    {
                        Shell32Library.ShellExecute(hwnd, "runas", shellMenuItem.MenuProgramPath, parameter, Path.GetDirectoryName(selectedFilePath), 1);
                    }
                    else
                    {
                        Shell32Library.ShellExecute(hwnd, "open", shellMenuItem.MenuProgramPath, parameter, Path.GetDirectoryName(selectedFilePath), 1);
                    }
                }

                if (!string.IsNullOrEmpty(selectedFolderPath))
                {
                    if (shellMenuItem.IsAlwaysRunAsAdministrator)
                    {
                        Shell32Library.ShellExecute(hwnd, "runas", shellMenuItem.MenuProgramPath, parameter, selectedFolderPath, 1);
                    }
                    else
                    {
                        Shell32Library.ShellExecute(hwnd, "open", shellMenuItem.MenuProgramPath, parameter, selectedFolderPath, 1);
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 根菜单命令关联标志
        /// </summary>
        public int GetFlags(out EXPCMDFLAGS pFlags)
        {
            if (shellMenuItem is not null)
            {
                pFlags = shellMenuItem.SubShellMenuItem.Count is 0 ? EXPCMDFLAGS.ECF_DEFAULT : EXPCMDFLAGS.ECF_HASSUBCOMMANDS;
                return 0;
            }
            else
            {
                pFlags = EXPCMDFLAGS.ECF_DEFAULT;
                return unchecked((int)0x80004001);
            }
        }

        /// <summary>
        /// 根菜单子命令遍历
        /// </summary>
        public int EnumSubCommands(out IEnumExplorerCommand ppEnum)
        {
            if (shellMenuItem is not null)
            {
                ExplorerCommand[] subExplorerCommandArray = new ExplorerCommand[shellMenuItem.SubShellMenuItem.Count];
                for (int index = 0; index < shellMenuItem.SubShellMenuItem.Count; index++)
                {
                    subExplorerCommandArray[index] = new ExplorerCommand(shellMenuItem.SubShellMenuItem[index]);
                }

                ppEnum = new EnumExplorerCommand(subExplorerCommandArray);
                return 0;
            }
            else
            {
                ppEnum = new EnumExplorerCommand([]);
                return unchecked((int)0x80004001);
            }
        }

        /// <summary>
        /// 允许容器向对象传递指向其站点接口的指针。
        /// </summary>
        public int SetSite(nint pUnkSite)
        {
            return pUnkSite.Equals(nint.Zero) ? unchecked((int)0x80004002) : Marshal.QueryInterface(pUnkSite, IID_IUnknown, out site);
        }

        /// <summary>
        /// 检索使用 SetSite 传递的最新站点。
        /// </summary>
        public int GetSite(in Guid riid, out nint ppvSite)
        {
            if (!site.Equals(nint.Zero))
            {
                return Marshal.QueryInterface(site, IID_IUnknown, out ppvSite);
            }
            else
            {
                ppvSite = nint.Zero;
                return unchecked((int)0x80004005);
            }
        }

        /// <summary>
        /// 枚举子菜单项选项，检查是否有符合桌面目录的选项
        /// </summary>
        private EXPCMDSTATE EnumerateMatchFolderDesktop(ShellMenuItem shellMenuItem)
        {
            if (shellMenuItem.SubShellMenuItem.Count is 0)
            {
                return shellMenuItem.FolderDesktop ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
            }
            else
            {
                bool isMatched = false;
                foreach (ShellMenuItem subShellMenuItem in shellMenuItem.SubShellMenuItem)
                {
                    if (EnumerateMatchFolderDesktop(subShellMenuItem) is EXPCMDSTATE.ECS_ENABLED)
                    {
                        isMatched = true;
                        break;
                    }
                }

                return isMatched ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
            }
        }

        /// <summary>
        /// 枚举子菜单项选项，检查是否有符合普通目录的选项
        /// </summary>
        private EXPCMDSTATE EnumerateMatchFolderDirectory(ShellMenuItem shellMenuItem)
        {
            if (shellMenuItem.SubShellMenuItem.Count is 0)
            {
                return shellMenuItem.FolderDirectory ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
            }
            else
            {
                bool isMatched = false;
                foreach (ShellMenuItem subShellMenuItem in shellMenuItem.SubShellMenuItem)
                {
                    if (EnumerateMatchFolderDirectory(subShellMenuItem) is EXPCMDSTATE.ECS_ENABLED)
                    {
                        isMatched = true;
                        break;
                    }
                }

                return isMatched ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
            }
        }

        /// <summary>
        /// 枚举子菜单项选项，检查是否有符合驱动器的选项
        /// </summary>
        private EXPCMDSTATE EnumerateMatchFolderDrive(ShellMenuItem shellMenuItem)
        {
            if (shellMenuItem.SubShellMenuItem.Count is 0)
            {
                return shellMenuItem.FolderDrive ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
            }
            else
            {
                bool isMatched = false;
                foreach (ShellMenuItem subShellMenuItem in shellMenuItem.SubShellMenuItem)
                {
                    if (EnumerateMatchFolderDrive(subShellMenuItem) is EXPCMDSTATE.ECS_ENABLED)
                    {
                        isMatched = true;
                        break;
                    }
                }

                return isMatched ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
            }
        }

        /// <summary>
        /// 枚举子菜单项选项，检查是否有符合文件的选项
        /// </summary>
        private EXPCMDSTATE EnumerateMatchFile(ShellMenuItem shellMenuItem, string filePath)
        {
            if (shellMenuItem.SubShellMenuItem.Count is 0)
            {
                // 匹配文件格式
                if (string.Equals(shellMenuItem.MenuFileMatchRule, ShellMenuService.FileMatchRuleList[0]))
                {
                    return EXPCMDSTATE.ECS_HIDDEN;
                }
                else if (string.Equals(shellMenuItem.MenuFileMatchRule, ShellMenuService.FileMatchRuleList[1]))
                {
                    string[] fileMatchFormatArray = shellMenuItem.MenuFileMatchFormatText.Split('|');

                    bool isMatched = false;
                    string fileName = Path.GetFileName(filePath);
                    foreach (string fileMatchFormat in fileMatchFormatArray)
                    {
                        if (string.Equals(fileName, fileMatchFormat.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            isMatched = true;
                            break;
                        }
                    }

                    return isMatched ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
                }
                else if (string.Equals(shellMenuItem.MenuFileMatchRule, ShellMenuService.FileMatchRuleList[2]))
                {
                    string[] fileMatchFormatArray = shellMenuItem.MenuFileMatchFormatText.Split('|');
                    bool isMatched = false;

                    foreach (string fileMatchFormat in fileMatchFormatArray)
                    {
                        Regex fileMatchRegex = new(fileMatchFormat.Trim());
                        string fileName = Path.GetFileName(filePath);

                        if (fileMatchRegex.IsMatch(fileName))
                        {
                            isMatched = true;
                            break;
                        }
                    }

                    return isMatched ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
                }
                else if (string.Equals(shellMenuItem.MenuFileMatchRule, ShellMenuService.FileMatchRuleList[3]))
                {
                    string[] fileMatchFormatArray = shellMenuItem.MenuFileMatchFormatText.Split('|');
                    bool isMatched = false;
                    string extensionName = Path.GetExtension(filePath);

                    foreach (string fileMatchFormat in fileMatchFormatArray)
                    {
                        if (string.Equals(extensionName, fileMatchFormat.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            isMatched = true;
                            break;
                        }
                    }

                    return isMatched ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
                }
                else if (string.Equals(shellMenuItem.MenuFileMatchRule, ShellMenuService.FileMatchRuleList[4]))
                {
                    return EXPCMDSTATE.ECS_ENABLED;
                }
                else
                {
                    return EXPCMDSTATE.ECS_HIDDEN;
                }
            }
            else
            {
                bool isMatched = false;
                foreach (ShellMenuItem subShellMenuItem in shellMenuItem.SubShellMenuItem)
                {
                    if (EnumerateMatchFile(subShellMenuItem, filePath) is EXPCMDSTATE.ECS_ENABLED)
                    {
                        isMatched = true;
                        break;
                    }
                }

                return isMatched ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;
            }
        }
    }
}
