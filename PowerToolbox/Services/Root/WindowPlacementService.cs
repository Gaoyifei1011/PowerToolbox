using Microsoft.Win32;
using PowerToolbox.Helpers.Root;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace PowerToolbox.Services.Root
{
    /// <summary>
    /// 窗口信息服务
    /// </summary>
    internal static class WindowPlacementService
    {
        private static readonly string windowInfoRegistryPath = @"Software\PowerToolbox\WindowPlacement";

        /// <summary>
        /// 获取窗口位置
        /// </summary>
        internal static unsafe WINDOWPLACEMENT? GetWindowPlacement(string windowName)
        {
            if (!string.IsNullOrEmpty(windowName))
            {
                try
                {
                    var Result = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, windowInfoRegistryPath, windowName);
                    if (RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, windowInfoRegistryPath, windowName) is string windowPlacementRoot && !string.IsNullOrEmpty(windowPlacementRoot))
                    {
                        XmlDocument xmlDocument = new();
                        xmlDocument.LoadXml(windowPlacementRoot);

                        XmlElement windowPlacementRootElement = xmlDocument.DocumentElement;
                        List<RECT> monitorInfoList = GetAllMonitorInfoList();

                        // 检查显示器布局自我们记录位置以来是否有变化
                        XmlNodeList monitorNodeList = windowPlacementRootElement.SelectNodes("Monitor/MonitorItem");
                        foreach (XmlElement monitorElement in monitorNodeList)
                        {
                            if (!int.TryParse(monitorElement.GetAttribute("Left"), out int left) || !int.TryParse(monitorElement.GetAttribute("Top"), out int top) || !int.TryParse(monitorElement.GetAttribute("Right"), out int right) || !int.TryParse(monitorElement.GetAttribute("Bottom"), out int bottom))
                            {
                                continue;
                            }

                            foreach (RECT monitorInfo in monitorInfoList)
                            {
                                if (!Equals(monitorInfo.left, left) || !Equals(monitorInfo.top, top) || !Equals(monitorInfo.right, right) || !Equals(monitorInfo.bottom, bottom))
                                {
                                    return null;
                                }
                            }
                        }

                        // 读取窗口位置
                        XmlElement windowPlacementElement = windowPlacementRootElement.SelectSingleNode("WindowPlacement") as XmlElement;
                        WINDOWPLACEMENT windowPlacement = new()
                        {
                            length = sizeof(WINDOWPLACEMENT)
                        };
                        if (Enum.TryParse(windowPlacementElement.GetAttribute("Flags"), out WINDOWPLACEMENTFLAGS windowPlacementFlags))
                        {
                            windowPlacement.flags = windowPlacementFlags;
                        }
                        if (Enum.TryParse(windowPlacementElement.GetAttribute("ShowCmd"), out SHOWWINDOWCMD showWindowCmd))
                        {
                            windowPlacement.showCmd = showWindowCmd;
                        }
                        windowPlacement.ptMinPosition.X = int.TryParse(windowPlacementElement.GetAttribute("MinPositionX"), out int minPositionX) ? minPositionX : default;
                        windowPlacement.ptMinPosition.Y = int.TryParse(windowPlacementElement.GetAttribute("MinPositionY"), out int minPositionY) ? minPositionY : default;
                        windowPlacement.ptMaxPosition.X = int.TryParse(windowPlacementElement.GetAttribute("MaxPositionX"), out int maxPositionX) ? maxPositionX : default;
                        windowPlacement.ptMaxPosition.Y = int.TryParse(windowPlacementElement.GetAttribute("MaxPositionY"), out int maxPositionY) ? maxPositionY : default;
                        windowPlacement.rcNormalPosition.left = int.TryParse(windowPlacementElement.GetAttribute("NormalPositionLeft"), out int normalPositionLeft) ? normalPositionLeft : default;
                        windowPlacement.rcNormalPosition.top = int.TryParse(windowPlacementElement.GetAttribute("NormalPositionTop"), out int normalPositionTop) ? normalPositionTop : default;
                        windowPlacement.rcNormalPosition.right = int.TryParse(windowPlacementElement.GetAttribute("NormalPositionRight"), out int normalPositionRight) ? normalPositionRight : default;
                        windowPlacement.rcNormalPosition.bottom = int.TryParse(windowPlacementElement.GetAttribute("NormalPositionBottom"), out int normalPositionBottom) ? normalPositionBottom : default;
                        if (windowPlacement.showCmd is SHOWWINDOWCMD.SW_SHOWMINIMIZED && windowPlacement.flags is WINDOWPLACEMENTFLAGS.WPF_RESTORETOMAXIMIZED)
                        {
                            windowPlacement.showCmd = SHOWWINDOWCMD.SW_MAXIMIZE;
                        }
                        else if (windowPlacement.showCmd is not SHOWWINDOWCMD.SW_MAXIMIZE)
                        {
                            windowPlacement.showCmd = SHOWWINDOWCMD.SW_NORMAL;
                        }
                        return windowPlacement;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WindowPlacementService), nameof(GetWindowPlacement), 1, e);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 保存窗口位置
        /// </summary>
        internal static void SaveWindowPlacement(string windowName, WINDOWPLACEMENT windowPlacement)
        {
            if (!string.IsNullOrEmpty(windowName))
            {
                try
                {
                    XmlDocument xmlDocument = new();
                    XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                    xmlDocument.AppendChild(xmlDeclaration);
                    XmlElement windowPlacementRootElement = xmlDocument.CreateElement("WindowPlacementRoot");
                    xmlDocument.AppendChild(windowPlacementRootElement);

                    // 获取显示器信息
                    XmlElement monitorElement = xmlDocument.CreateElement("Monitor");
                    List<RECT> monitorInfoList = GetAllMonitorInfoList();
                    foreach (RECT monitorInfo in monitorInfoList)
                    {
                        XmlElement monitorItemElement = xmlDocument.CreateElement("MonitorItem");
                        monitorItemElement.SetAttribute("Left", Convert.ToString(monitorInfo.left));
                        monitorItemElement.SetAttribute("Top", Convert.ToString(monitorInfo.top));
                        monitorItemElement.SetAttribute("Right", Convert.ToString(monitorInfo.right));
                        monitorItemElement.SetAttribute("Bottom", Convert.ToString(monitorInfo.bottom));
                        monitorElement.AppendChild(monitorItemElement);
                    }
                    windowPlacementRootElement.AppendChild(monitorElement);

                    XmlElement windowPlacementElement = xmlDocument.CreateElement("WindowPlacement");

                    // 存储窗口位置
                    windowPlacementElement.SetAttribute("Flags", Convert.ToString(windowPlacement.flags));
                    windowPlacementElement.SetAttribute("ShowCmd", Convert.ToString(windowPlacement.showCmd));
                    windowPlacementElement.SetAttribute("MinPositionX", Convert.ToString(windowPlacement.ptMinPosition.X));
                    windowPlacementElement.SetAttribute("MinPositionY", Convert.ToString(windowPlacement.ptMinPosition.Y));
                    windowPlacementElement.SetAttribute("MaxPositionX", Convert.ToString(windowPlacement.ptMaxPosition.X));
                    windowPlacementElement.SetAttribute("MaxPositionY", Convert.ToString(windowPlacement.ptMinPosition.Y));
                    windowPlacementElement.SetAttribute("NormalPositionLeft", Convert.ToString(windowPlacement.rcNormalPosition.left));
                    windowPlacementElement.SetAttribute("NormalPositionTop", Convert.ToString(windowPlacement.rcNormalPosition.top));
                    windowPlacementElement.SetAttribute("NormalPositionBottom", Convert.ToString(windowPlacement.rcNormalPosition.bottom));
                    windowPlacementElement.SetAttribute("NormalPositionRight", Convert.ToString(windowPlacement.rcNormalPosition.right));
                    windowPlacementRootElement.AppendChild(windowPlacementElement);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, windowInfoRegistryPath, windowName, xmlDocument.OuterXml);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WindowPlacementService), nameof(SaveWindowPlacement), 1, e);
                }
            }
        }

        /// <summary>
        /// 获取所有显示器信息列表
        /// </summary>
        private static unsafe List<RECT> GetAllMonitorInfoList()
        {
            List<RECT> monitorList = [];

            bool MonitorEnumProc(nint hMonitor, nint hdc, nint lprcMonitor, nint dwData)
            {
                monitorList.Add((*(RECT*)lprcMonitor));
                return true;
            }

            User32Library.EnumDisplayMonitors(0, 0, MonitorEnumProc, 0);
            return monitorList;
        }
    }
}
