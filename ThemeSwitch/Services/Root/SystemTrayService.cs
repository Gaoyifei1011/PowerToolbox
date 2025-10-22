using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ThemeSwitch.Services.Root
{
    /// <summary>
    /// 系统托盘菜单服务
    /// </summary>
    public static class SystemTrayService
    {
        private static NotifyIcon notifyIcon;

        public static event MouseEventHandler RightClick;

        public static event MouseEventHandler MouseDoubleClick;

        /// <summary>
        /// 初始化系统托盘
        /// </summary>
        public static void InitializeSystemTray(string title, string iconPath)
        {
            if (notifyIcon is null)
            {
                notifyIcon = new NotifyIcon
                {
                    Text = title,
                    Icon = Icon.ExtractAssociatedIcon(iconPath),
                    Visible = true,
                    ContextMenu = new ContextMenu()
                };

                notifyIcon.MouseClick += OnMouseClick;
                notifyIcon.MouseDoubleClick += OnMouseDoubleClick;
            }
        }

        /// <summary>
        /// 更新标题
        /// </summary>
        public static void UpdateTitle(string title)
        {
            if (notifyIcon is not null)
            {
                notifyIcon.Text = title;
            }
        }

        /// <summary>
        /// 关闭托盘菜单
        /// </summary>
        public static void CloseSystemTray()
        {
            if (notifyIcon is not null)
            {
                try
                {
                    notifyIcon.Visible = false;
                    notifyIcon.MouseDoubleClick -= OnMouseDoubleClick;
                    notifyIcon.Dispose();
                    notifyIcon = null;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(ThemeSwitch), nameof(SystemTrayService), nameof(CloseSystemTray), 1, e);
                }
            }
        }

        /// <summary>
        /// 处理托盘菜单鼠标双击事件
        /// </summary>
        private static void OnMouseClick(object sender, MouseEventArgs args)
        {
            if (args.Button is MouseButtons.Right)
            {
                RightClick?.Invoke(sender, args);
            }
        }

        /// <summary>
        /// 处理托盘菜单鼠标双击事件
        /// </summary>
        private static void OnMouseDoubleClick(object sender, MouseEventArgs args)
        {
            MouseDoubleClick?.Invoke(sender, args);
        }
    }
}
