using System;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 控制进度对话框操作的标志。
    /// </summary>
    [Flags]
    internal enum PROGDLG : uint
    {
        /// <summary>
        /// 正常进度对话框行为。
        /// </summary>
        PROGDLG_NORMAL = 0x00000000,

        /// <summary>
        /// 进度对话框将与 hwndParent 指定的窗口模式化。 默认情况下，进度对话框是无模式的。
        /// </summary>
        PROGDLG_MODAL = 0x00000001,

        /// <summary>自动估计剩余时间，并在第 3 行显示估算值。 如果设置了此标志， 则 IProgressDialog：：SetLine 只能用于显示第 1 行和第 2 行的文本。</summary>
        PROGDLG_AUTOTIME = 0x00000002,

        /// <summary>
        /// 不显示“剩余时间”文本。
        /// </summary>
        PROGDLG_NOTIME = 0x00000004,

        /// <summary>
        /// 不要在对话框的描述文字栏上显示最小化按钮。
        /// </summary>
        PROGDLG_NOMINIMIZE = 0x00000008,

        /// <summary>
        /// 不显示进度栏。 通常，应用程序可以定量地确定剩余的操作量，并定期将该值传递给 IProgressDialog：：SetProgress。 进度对话框使用此信息更新其进度栏。 当调用应用程序必须等待操作完成，但没有任何可用于更新对话框的定量信息时，通常会设置此标志。
        /// </summary>
        PROGDLG_NOPROGRESSBAR = 0x00000010,

        /// <summary>
        /// Windows Vista 及更高版本。 将进度栏设置为选框模式。 这会导致进度栏水平滚动，类似于选择显示。 如果希望指示正在进行进度，但操作所需的时间未知，请使用此选项。
        /// </summary>
        PROGDLG_MARQUEEPROGRESS = 0x00000020,

        /// <summary>
        /// Windows Vista 及更高版本。 不显示“取消”按钮。 无法取消操作。 仅当绝对必要时才使用此选项。
        /// </summary>
        PROGDLG_NOCANCEL = 0x00000040
    }
}
