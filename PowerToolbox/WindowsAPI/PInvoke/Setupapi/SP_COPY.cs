namespace PowerToolbox.WindowsAPI.PInvoke.Setupapi
{
    internal enum SP_COPY
    {
        /// <summary>
        /// 无任何标志
        /// </summary>
        SP_COPY_NONE = 0x0,

        /// <summary>
        /// 成功复制后删除源文件。 如果删除操作失败，则不会通知调用方。
        /// </summary>
        SP_COPY_DELETESOURCE = 0x0000001,

        /// <summary>
        /// 仅当这样做会覆盖目标路径上的文件时，才会复制该文件。 如果目标不存在，则函数返回 FALSE。
        /// </summary>
        SP_COPY_REPLACEONLY = 0x0000002,

        /// <summary>
        /// 检查正在复制的每个文件，以查看其版本资源是否指示它是否与目标上的现有副本相同或不更新。
        /// 版本检查期间使用的文件版本信息是在 dwFileVersionMS 中指定的，dwFileVersionLSVS_FIXEDFILEINFO 结构的成员，由版本函数填充。 如果其中一个文件没有版本资源，或者它们具有相同的版本信息，则源文件被视为较新。
        /// 如果源文件在版本中不是较新或相等的，并且指定了 CopyMsgHandler，则会通知调用方并可以取消复制操作。 如果未指定 CopyMsgHandler，则不会复制该文件。
        /// </summary>
        SP_COPY_NEWER = 0x0000004,

        /// <summary>
        /// 检查正在复制的每个文件，以查看其版本资源是否指示它是否与目标上的现有副本相同或不更新。
        /// 版本检查期间使用的文件版本信息是在 dwFileVersionMS 中指定的，dwFileVersionLSVS_FIXEDFILEINFO 结构的成员，由版本函数填充。 如果其中一个文件没有版本资源，或者它们具有相同的版本信息，则源文件被视为较新。
        /// 如果源文件在版本中不是较新或相等的，并且指定了 CopyMsgHandler，则会通知调用方并可以取消复制操作。 如果未指定 CopyMsgHandler，则不会复制该文件。
        /// </summary>
        SP_COPY_NEWER_OR_SAME = SP_COPY_NEWER,

        /// <summary>
        /// 检查目标文件是否存在，如果是，请通知可能否决副本的调用方。 如果未指定 CopyMsgHandler，则不会覆盖该文件。
        /// </summary>
        SP_COPY_NOOVERWRITE = 0x0000008,

        /// <summary>
        /// 请勿解压缩文件。 设置此标志时，不会向目标文件提供源名称的未压缩形式（如果适用）。 例如，将 F：\x86\cmd.ex_ 复制到 \\install\temp 会导致目标文件 \\install\temp\cmd.ex_。 如果未指定SP_COPY_NODECOMP标志，则会解压缩该文件，并将 \\install\temp\cmd.exe调用目标。 DestinationName的文件名部分（如果指定）将被剥离并替换为源文件的文件名。 指定SP_COPY_NODECOMP时，无法检查任何语言或版本信息。
        /// </summary>
        SP_COPY_NODECOMP = 0x0000010,

        /// <summary>
        /// 检查正在复制的每个文件，以查看其语言是否与目标上已有的任何现有文件的语言不同。 如果是这样，并且 指定 CopyMsgHandler，则会通知调用方并可以取消副本。 如果未指定 CopyMsgHandler，则不会复制该文件。
        /// </summary>
        SP_COPY_LANGUAGEAWARE = 0x0000020,

        /// <summary>
        /// SourceFile 是完整的源路径。 请勿在 INF 文件的 SourceDisksNames 节中查找它。
        /// </summary>
        SP_COPY_SOURCE_ABSOLUTE = 0x0000040,

        /// <summary>
        /// SourcePathRoot 是源文件的完整路径部分。 忽略 INF 文件的 SourceDisksNames 节中指定的相对源，该源位于该文件所在的源媒体。 如果指定了SP_COPY_SOURCE_ABSOLUTE，则忽略此标志。
        /// </summary>
        SP_COPY_SOURCEPATH_ABSOLUTE = 0x0000080,

        /// <summary>
        /// 如果在复制操作期间使用该文件，则通知用户需要重新启动系统。 仅当稍后 调用 setupPromptReboot 或 SetupScanFileQueue时，才使用此标志。
        /// </summary>
        SP_COPY_IN_USE_NEEDS_REBOOT = 0x0000100,

        /// <summary>
        /// 如果目标存在，则行为就像正在使用一样，并将文件排入队列，以便在下一次系统重启时进行复制。
        /// </summary>
        SP_COPY_FORCE_IN_USE = 0x0000200,

        /// <summary>
        /// 不要向用户提供跳过文件的选项。
        /// </summary>
        SP_COPY_NOSKIP = 0x0000400,

        /// <summary>
        /// 当前源文件在另一个内阁文件中继续。
        /// </summary>
        SP_FLAG_CABINETCONTINUATION = 0x0000800,

        /// <summary>
        /// 检查目标文件是否存在，如果是这样，则不会覆盖该文件。 不通知调用方。
        /// </summary>
        SP_COPY_FORCE_NOOVERWRITE = 0x0001000,

        /// <summary>
        /// 检查正在复制的每个文件，以查看其版本资源（或非映像文件的时间戳）是否指示它不是目标上的现有副本。 如果正在复制的文件不是较新的，则不会复制该文件。 不通知调用方。
        /// </summary>
        SP_COPY_FORCE_NEWER = 0x0002000,

        /// <summary>
        /// 如果用户尝试跳过文件，请警告他们跳过文件可能会影响安装。 （用于系统关键文件。）
        /// </summary>
        SP_COPY_WARNIFSKIP = 0x0004000,

        /// <summary>
        /// 请勿向用户提供浏览选项。
        /// </summary>
        SP_COPY_NOBROWSE = 0x0008000,

        /// <summary>
        /// 检查正在复制的每个文件，以查看其版本资源是否指示它不是目标上的现有副本。 如果源文件较新，但在版本上与现有目标不相等，则会复制该文件。
        /// </summary>
        SP_COPY_NEWER_ONLY = 0x0010000,

        SP_COPY_SOURCE_SIS_MASTER = 0x0020000,

        /// <summary>
        /// 指定的 .inf 文件的相应目录文件将复制到 %windir%\Inf。 如果指定了此标志，则如果 Inf 目录中已存在指定的 .inf 文件，则会在成功返回时输入目标文件名信息。
        /// </summary>
        SP_COPY_OEMINF_CATALOG_ONLY = 0x0040000,

        /// <summary>
        /// 文件必须在重新启动时存在
        /// </summary>
        SP_COPY_REPLACE_BOOT_FILE = 0x0080000,

        /// <summary>
        /// 永远不要修剪这个文件
        /// </summary>
        SP_COPY_NOPRUNE = 0x0100000
    }
}
