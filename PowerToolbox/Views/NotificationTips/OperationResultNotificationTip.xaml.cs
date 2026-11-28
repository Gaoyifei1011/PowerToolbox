using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Services.Root;
using System.ComponentModel;

namespace PowerToolbox.Views.NotificationTips
{
    /// <summary>
    /// 操作完成后应用内通知
    /// </summary>
    internal sealed partial class OperationResultNotificationTip : TeachingTip, INotifyPropertyChanged
    {
        #region 第一部分：属性、列表与事件

        internal bool IsInitialized { get; private set; }

        internal bool IsSuccessOperation
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSuccessOperation)));
                }
            }
        }

        internal string OperationContent
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(OperationContent)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第一部分：属性、列表与事件

        internal OperationResultNotificationTip(OperationKind operationKind)
        {
            InitializeComponent();
            InitializeOperationResultNotification(operationKind);
        }

        #region 第二部分：构造函数

        internal OperationResultNotificationTip(OperationKind operationKind, bool operationResult)
        {
            InitializeComponent();
            InitializeOperationResultNotification(operationKind, operationResult);
        }

        internal OperationResultNotificationTip(OperationKind operationKind, int successItems, int failedItems)
        {
            InitializeComponent();
            InitializeOperationResultNotification(operationKind, successItems, failedItems);
        }

        internal OperationResultNotificationTip(OperationKind operationKind, int statusKind)
        {
            InitializeComponent();
            InitializeOperationResultNotification(operationKind, statusKind);
        }

        #endregion 第二部分：构造函数

        #region 第三部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化操作完成后应用内通知
        /// </summary>
        private void InitializeOperationResultNotification(OperationKind operationKind)
        {
            if (operationKind is OperationKind.AddDriverAllSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("AddDriverAllSuccessfully");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.AddDriverFailed)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("AddDriverFailed");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.AddDriverPartialSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("AddDriverPartialSuccessfully");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.AddInstallDriverAllSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("AddInstallDriverAllSuccessfully");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.AddInstallDriverFailed)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("AddInstallDriverFailed");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.AddInstallDriverPartialSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("AddInstallDriverPartialSuccessfully");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.CustomFileFilterTypeEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("CustomFileFilterTypeEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.DeleteDriverAllSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("DeleteDriverAllSuccessfully");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.DeleteDriverFailed)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("DeleteDriverFailed");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.DeleteDriverPartialSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("DeleteDriverPartialSuccessfully");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.DeleteDriverSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("DeleteDriverSuccessfully");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.DeleteFileFailed)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("FileDeleteFailed");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.DevicePositionInitializeFailed)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("DevicePositionInitializeFailed");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.DevicePositionLoadFailed)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("DevicePositionLoadFailed");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.DriveEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("DriveEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.FileLost)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("FileLost");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ForceDeleteDriverAllSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("ForceDeleteDriverAllSuccessfully");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ForceDeleteDriverFailed)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("ForceDeleteDriverFailed");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ForceDeleteDriverPartialSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("ForceDeleteDriverPartialSuccessfully");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ForceDeleteDriverSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("ForceDeleteDriverSuccessfully");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.IcoSizeNotSelected)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("IcoSizeNotSelected");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.InsiderPreviewSettings)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("InsiderPreviewSuccessfully");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.LanguageChange)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("LanguageChange");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ListEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("ListEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.MenuDarkThemeIconPathEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("MenuDarkThemeIconPathEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.MenuDefaultIconPathEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("MenuDefaultIconPathEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.MenuLightThemeIconPathEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("MenuLightThemeIconPathEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.MenuProgramPathEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("MenuProgramPathEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.MenuMatchRuleEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("MenuMatchRuleEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.MenuTitleEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("MenuTitleEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.NoOperation)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("NoOperation");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.RestoreContentEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("RestoreContentEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.RestoreSpecificExtensionGroupsTypeEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("RestoreSpecificExtensionGroupsTypeEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.SameDriveAndSelectFolder)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("SameDriveAndSelectFolder");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.SelectDriverEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("SelectDriverEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.SelectFolderEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("SelectFolderEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.SelectLogFolderEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("SelectLogFolderEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ShellMenuNeedToRefreshData)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("ShellMenuNeedToRefreshData");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ThemeChangeSameTime)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("ThemeChangeSameTime");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ThemeSwitchSaveResult)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("ThemeSwitchSaveResult");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ThemeSwitchRestoreResult)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("ThemeSwitchRestoreResult");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.WinFRNotInstalled)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("WinFRNotInstalled");
                IsInitialized = true;
            }
        }

        /// <summary>
        /// 初始化操作完成后应用内通知
        /// </summary>
        private void InitializeOperationResultNotification(OperationKind operationKind, bool operationResult)
        {
            if (operationKind is OperationKind.CleanUpdateHistory)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.NotificationTipResource.GetString("CleanUpdateHistorySuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.NotificationTipResource.GetString("CleanUpdateHistoryFailed");
                }
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ContextMenuUpdate)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.NotificationTipResource.GetString("ContextMenuUpdateSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.NotificationTipResource.GetString("ContextMenuUpdateFailed");
                }
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.CopyToClipBoard)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.NotificationTipResource.GetString("CopyToClipboardSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.NotificationTipResource.GetString("CopyToClipboardFailed");
                }
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.Desktop)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.NotificationTipResource.GetString("DesktopShortcutSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.NotificationTipResource.GetString("DesktopShortcutFailed");
                }
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.LogClean)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.NotificationTipResource.GetString("LogCleanSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.NotificationTipResource.GetString("LogCleanFailed");
                }
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.LoopbackSetResult)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.NotificationTipResource.GetString("LoopbackSetResultSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.NotificationTipResource.GetString("LoopbackSetResultFailed");
                }
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.StartScreen)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.NotificationTipResource.GetString("StartScreenSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.NotificationTipResource.GetString("StartScreenFailed");
                }
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.Taskbar)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.NotificationTipResource.GetString("TaskbarSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.NotificationTipResource.GetString("TaskbarFailed");
                }
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.TerminateProcess)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.NotificationTipResource.GetString("TerminateProcessSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.NotificationTipResource.GetString("TerminateProcessFailed");
                }
                IsInitialized = true;
            }
        }

        /// <summary>
        /// 初始化操作完成后应用内通知
        /// </summary>
        private void InitializeOperationResultNotification(OperationKind operationKind, int successItems, int failedItems)
        {
            if (operationKind is OperationKind.File)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0; ;
                OperationContent = failedItems is 0 ? string.Format(ResourceService.NotificationTipResource.GetString("FileResultSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("FileResultFailed"), successItems, failedItems);
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.IconExtract)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0;
                OperationContent = failedItems is 0 && successItems is not 0 ? string.Format(ResourceService.NotificationTipResource.GetString("IconExtractSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("IconExtractFailed"), successItems, failedItems);
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ScheduledTaskDelete)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0;
                OperationContent = failedItems is 0 && successItems is not 0 ? string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskDeleteSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskDeleteFailed"), successItems, failedItems);
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ScheduledTaskDisable)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0;
                OperationContent = failedItems is 0 && successItems is not 0 ? string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskDisableSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskDisableFailed"), successItems, failedItems);
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ScheduledTaskEnable)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0;
                OperationContent = failedItems is 0 && successItems is not 0 ? string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskEnableSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskEnableFailed"), successItems, failedItems);
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ScheduledTaskExport)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0;
                OperationContent = failedItems is 0 && successItems is not 0 ? string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskExportSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskExportFailed"), successItems, failedItems);
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ScheduledTaskRun)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0;
                OperationContent = failedItems is 0 && successItems is not 0 ? string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskRunSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskRunFailed"), successItems, failedItems);
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.ScheduledTaskStop)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0;
                OperationContent = failedItems is 0 && successItems is not 0 ? string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskStopSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskStopFailed"), successItems, failedItems);
                IsInitialized = true;
            }
        }

        /// <summary>
        /// 初始化操作完成后应用内通知
        /// </summary>
        private void InitializeOperationResultNotification(OperationKind operationKind, int statusKind)
        {
            if (operationKind is OperationKind.CheckUpdate)
            {
                if (statusKind is 0)
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.NotificationTipResource.GetString("NotNewestVersion");
                    IsInitialized = true;
                }
                else if (statusKind is 1)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.NotificationTipResource.GetString("NewestVersion");
                    IsInitialized = true;
                }
                else if (statusKind is 2)
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.NotificationTipResource.GetString("UpdateCheckFailed");
                    IsInitialized = true;
                }
            }
        }

        #endregion 第三部分：数据操作与业务逻辑
    }
}
