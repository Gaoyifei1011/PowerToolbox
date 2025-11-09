using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 包含 ShellExecuteEx 使用的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SHELLEXECUTEINFO
    {
        /// <summary>
        /// 必需。 此结构的大小（以字节为单位）。
        /// </summary>
        public int cbSize;

        /// <summary>
        /// 以下一个或多个值的组合，这些值指示其他结构成员的内容和有效性：
        /// </summary>
        public ShellExecuteMaskFlags fMask;

        /// <summary>
        /// 可选。 所有者窗口的句柄，用于显示和定位执行此函数时系统可能生成的任何 UI。
        /// </summary>
        public nint hwnd;

        /// <summary>
        /// 一个字符串，称为 谓词，指定要执行的操作。 可用谓词集取决于特定的文件或文件夹。 通常，对象的快捷菜单中可用的操作是可用的谓词。 此参数可以为 NULL，在这种情况下，将使用默认谓词（如果可用）。 如果不是，则使用“open”谓词。 如果两个谓词都不可用，则系统会使用注册表中列出的第一个谓词。 除非有理由将操作限制为特定谓词，否则传递 NULL 以使用计算的默认值。
        /// </summary>
        public string lpVerb;

        /// <summary>
        /// 以 null 结尾的字符串的地址，指定 ShellExecuteExecuteEx 将对其执行 lpVerb 参数指定操作的文件或对象的名称。 ShellExecuteExecuteEx 函数支持的系统注册表谓词包括可执行文件和文档文件的“open”和“print”（对于已注册打印处理程序的文档文件）。 其他应用程序可能已通过系统注册表添加了 Shell 谓词，例如“播放”.avi 和.wav文件。 若要指定 Shell 命名空间对象，请传递完全限定分析名称，并在 fMask 参数中设置SEE_MASK_INVOKEIDLIST标志。
        /// </summary>
        public string lpFile;

        /// <summary>
        /// 可选。 包含应用程序参数的以 null 结尾的字符串的地址。 参数必须用空格分隔。 如果 lpFile 成员指定文档文件， 则 lpParameters 应为 NULL。
        /// </summary>
        public string lpParameters;

        /// <summary>
        /// 可选。 以 null 结尾的字符串的地址，该字符串指定工作目录的名称。 如果此成员为 NULL，则当前目录将用作工作目录。
        /// </summary>
        public string lpDirectory;

        /// <summary>
        /// 必需。 指定应用程序在打开时如何显示应用程序的标志;为 ShellExecute 函数列出的SW_值之一。 如果 lpFile 指定文档文件，则标志将直接传递给关联的应用程序。 由应用程序决定如何处理它。
        /// </summary>
        public int nShow;

        /// <summary>
        /// 如果设置了SEE_MASK_NOCLOSEPROCESS并且 ShellExecuteEx 调用 成功，则会将此成员设置为大于 32 的值。 如果函数失败，则将其设置为指示失败原因SE_ERR_XXX错误值。 尽管 hInstApp 声明为 HINSTANCE 以便与 16 位 Windows 应用程序兼容，但它并不是真正的 HINSTANCE。 它只能强制转换为 int ，与 32 或以下SE_ERR_XXX错误代码进行比较。
        /// </summary>
        public nint hInstApp;

        /// <summary>
        /// 绝对 ITEMIDLIST 结构的地址 (PCIDLIST_ABSOLUTE) 包含唯一标识要执行的文件的项目标识符列表。 如果 fMask 成员不包含 SEE_MASK_IDLIST或SEE_MASK_INVOKEIDLIST ，则忽略 此成员。
        /// </summary>
        public nint lpIDList;

        /// <summary>
        /// 以 null 结尾的字符串的地址。如果 fMask 不包含 SEE_MASK_CLASSNAME，则忽略此成员。
        /// </summary>
        public string lpClass;

        /// <summary>
        /// 文件类型的注册表项的句柄。 此注册表项的访问权限应设置为 KEY_READ。 如果 fMask 不包含 SEE_MASK_CLASSKEY，则忽略此成员。
        /// </summary>
        public nint hkeyClass;

        /// <summary>
        /// 要与应用程序关联的键盘快捷方式。 低序字是虚拟键代码，高阶字是 (HOTKEYF_) 修饰符标志。 有关修饰符标志的列表，请参阅 WM_SETHOTKEY 消息的说明。 如果 fMask 不包含 SEE_MASK_HOTKEY，则忽略此成员。
        /// </summary>
        public uint dwHotKey;

        /// <summary>
        /// 文件类型图标的句柄。 如果 fMask 不包含 SEE_MASK_ICON，则忽略此成员。 此值仅在 Windows XP 及更早版本中使用。 从 Windows Vista 开始，它将被忽略。
        /// </summary>
        public nint hIcon;

        /// <summary>
        /// 新启动的应用程序的句柄。 此成员在返回时设置，并且始终为 NULL ，除非 fMask 设置为 SEE_MASK_NOCLOSEPROCESS。 即使 fMask 设置为 SEE_MASK_NOCLOSEPROCESS，如果没有启动任何进程， hProcess 也将为 NULL 。
        /// </summary>
        public nint hProcess;
    }
}
