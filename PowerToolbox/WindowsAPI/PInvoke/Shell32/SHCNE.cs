using System;

namespace PowerToolbox.WindowsAPI.PInvoke.Shell32
{
    [Flags]
    public enum SHCNE : uint
    {
        /// <summary>
        /// 非文件夹项的名称已更改。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含项的上一个 PIDL 或名称。 dwItem2 包含项的新 PIDL 或名称。
        /// </summary>
        SHCNE_RENAMEITEM = 0x00000001,

        /// <summary>
        /// 已创建非文件夹项。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含已创建的项。 未使用 dwItem2，应为 NULL。
        /// </summary>
        SHCNE_CREATE = 0x00000002,

        /// <summary>
        /// 已删除非文件夹项。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含已删除的项。 未使用 dwItem2，应为 NULL。
        /// </summary>
        SHCNE_DELETE = 0x00000004,

        /// <summary>
        /// 已创建文件夹。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含已创建的文件夹。 未使用 dwItem2，应为 NULL。
        /// </summary>
        SHCNE_MKDIR = 0x00000008,

        /// <summary>
        /// 已删除文件夹。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含已删除的文件夹。 未使用 dwItem2，应为 NULL。
        /// </summary>
        SHCNE_RMDIR = 0x00000010,

        /// <summary>
        /// 存储介质已插入驱动器。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含包含新媒体的驱动器的根目录。 未使用 dwItem2，应为 NULL。
        /// </summary>
        SHCNE_MEDIAINSERTED = 0x00000020,

        /// <summary>
        /// 已从驱动器中删除存储介质。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含从中删除介质的驱动器的根目录。 未使用 dwItem2，应为 NULL。
        /// </summary>
        SHCNE_MEDIAREMOVED = 0x00000040,

        /// <summary>
        /// 已删除驱动器。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含已删除的驱动器的根目录。 未使用 dwItem2，应为 NULL。
        /// </summary>
        SHCNE_DRIVEREMOVED = 0x00000080,

        /// <summary>
        /// 已添加驱动器。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含已添加的驱动器的根。 未使用 dwItem2，应为 NULL。
        /// </summary>
        SHCNE_DRIVEADD = 0x00000100,

        /// <summary>
        /// 正在通过网络共享本地计算机上的文件夹。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含正在共享的文件夹。 未使用 dwItem2，应为 NULL。
        /// </summary>
        SHCNE_NETSHARE = 0x00000200,

        /// <summary>
        /// 本地计算机上的文件夹不再通过网络共享。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含不再共享的文件夹。 未使用 dwItem2，应为 NULL。
        /// </summary>
        SHCNE_NETUNSHARE = 0x00000400,

        /// <summary>
        /// 项目或文件夹的属性已更改。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含已更改的项目或文件夹。 未使用 dwItem2，应为 NULL。
        /// </summary>
        SHCNE_ATTRIBUTES = 0x00000800,

        /// <summary>
        /// 现有文件夹的内容已更改，但该文件夹仍然存在且尚未重命名。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含已更改的文件夹。 未使用 dwItem2，应为 NULL。 如果已创建、删除或重命名文件夹，请分别使用 SHCNE_MKDIR、 SHCNE_RMDIR或 SHCNE_RENAMEFOLDER。
        /// </summary>
        SHCNE_UPDATEDIR = 0x00001000,

        /// <summary>
        /// 文件夹或非文件夹 (现有项) 已更改，但该项仍然存在且尚未重命名。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含已更改的项。 未使用 dwItem2，应为 NULL。 如果已创建、删除或重命名非文件夹项，请分别使用 SHCNE_CREATE、 SHCNE_DELETE或 SHCNE_RENAMEITEM。
        /// </summary>
        SHCNE_UPDATEITEM = 0x00002000,

        /// <summary>
        /// 计算机已与服务器断开连接。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含从中断开计算机连接的服务器。 未使用 dwItem2，应为 NULL。
        /// </summary>
        SHCNE_SERVERDISCONNECT = 0x00004000,

        /// <summary>
        /// 系统映像列表中的映像已更改。 必须在uFlags 中指定SHCNF_DWORD。dwItem2 包含系统映像列表中已更改的索引。 未使用 dwItem1，应为 NULL。
        /// </summary>
        SHCNE_UPDATEIMAGE = 0x00008000,

        /// <summary>
        /// Windows XP 及更高版本：未使用。
        /// </summary>
        SHCNE_DRIVEADDGUI = 0x00010000,

        /// <summary>
        /// 文件夹的名称已更改。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含以前的 PIDL 或文件夹名称。 dwItem2 包含文件夹的新 PIDL 或名称。
        /// </summary>
        SHCNE_RENAMEFOLDER = 0x00020000,

        /// <summary>
        /// 驱动器上的可用空间量已更改。 必须在uFlags 中指定SHCNF_IDLIST或SHCNF_PATH。 dwItem1 包含可用空间已更改的驱动器的根目录。 未使用 dwItem2，应为 NULL。
        /// </summary>
        SHCNE_FREESPACE = 0x00040000,

        /// <summary>
        /// 当前未使用。
        /// </summary>
        SHCNE_EXTENDED_EVENT = 0x04000000,

        /// <summary>
        /// 文件类型关联已更改。 必须在uFlags 参数中指定SHCNF_IDLIST。 不使用 dwItem1 和 dwItem2，必须为 NULL。 还应为已注册的协议发送此事件。
        /// </summary>
        SHCNE_ASSOCCHANGED = 0x08000000,

        /// <summary>
        /// 指定所有磁盘事件标识符的组合。
        /// </summary>
        SHCNE_DISKEVENTS = 0x0002381F,

        /// <summary>
        /// 指定所有全局事件标识符的组合。
        /// </summary>
        SHCNE_GLOBALEVENTS = 0x0C0581E0,

        /// <summary>
        /// 所有事件都已发生。
        /// </summary>
        SHCNE_ALLEVENTS = 0x7FFFFFFF,

        /// <summary>
        /// 指定的事件由于系统中断而发生。 由于此值会修改其他事件值，因此不能单独使用。
        /// </summary>
        SHCNE_INTERRUPT = 0x80000000,
    }
}
