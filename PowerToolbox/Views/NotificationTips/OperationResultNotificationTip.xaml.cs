using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Services.Root;
using System.ComponentModel;

namespace PowerToolbox.Views.NotificationTips
{
    /// <summary>
    /// 操作完成后应用内通知
    /// </summary>
    public sealed partial class OperationResultNotificationTip : TeachingTip, INotifyPropertyChanged
    {
        private bool _isSuccessOperation;

        public bool IsSuccessOperation
        {
            get { return _isSuccessOperation; }

            set
            {
                if (!Equals(_isSuccessOperation, value))
                {
                    _isSuccessOperation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSuccessOperation)));
                }
            }
        }

        private string _operationContent;

        public string OperationContent
        {
            get { return _operationContent; }

            set
            {
                if (!string.Equals(_operationContent, value))
                {
                    _operationContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OperationContent)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public OperationResultNotificationTip(OperationKind operationKind)
        {
            InitializeComponent();

            if (operationKind is OperationKind.AddDriverAllSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("AddDriverAllSuccessfully");
            }
            else if (operationKind is OperationKind.AddDriverFailed)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("AddDriverFailed");
            }
            else if (operationKind is OperationKind.AddDriverPartialSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("AddDriverPartialSuccessfully");
            }
            else if (operationKind is OperationKind.AddInstallDriverAllSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("AddInstallDriverAllSuccessfully");
            }
            else if (operationKind is OperationKind.AddInstallDriverFailed)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("AddInstallDriverFailed");
            }
            else if (operationKind is OperationKind.AddInstallDriverPartialSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("AddInstallDriverPartialSuccessfully");
            }
            else if (operationKind is OperationKind.CustomFileFilterTypeEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("CustomFileFilterTypeEmpty");
            }
            else if (operationKind is OperationKind.DeleteDriverAllSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("DeleteDriverAllSuccessfully");
            }
            else if (operationKind is OperationKind.DeleteDriverFailed)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("DeleteDriverFailed");
            }
            else if (operationKind is OperationKind.DeleteDriverPartialSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("DeleteDriverPartialSuccessfully");
            }
            else if (operationKind is OperationKind.DeleteDriverSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("DeleteDriverSuccessfully");
            }
            else if (operationKind is OperationKind.DeleteFileFailed)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("FileDeleteFailed");
            }
            else if (operationKind is OperationKind.DevicePositionInitializeFailed)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("DevicePositionInitializeFailed");
            }
            else if (operationKind is OperationKind.DevicePositionLoadFailed)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("DevicePositionLoadFailed");
            }
            else if (operationKind is OperationKind.DriveEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("DriveEmpty");
            }
            else if (operationKind is OperationKind.FileLost)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("FileLost");
            }
            else if (operationKind is OperationKind.ForceDeleteDriverAllSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("ForceDeleteDriverAllSuccessfully");
            }
            else if (operationKind is OperationKind.ForceDeleteDriverFailed)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("ForceDeleteDriverFailed");
            }
            else if (operationKind is OperationKind.ForceDeleteDriverPartialSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("ForceDeleteDriverPartialSuccessfully");
            }
            else if (operationKind is OperationKind.ForceDeleteDriverSuccessfully)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("ForceDeleteDriverSuccessfully");
            }
            else if (operationKind is OperationKind.IcoSizeNotSelected)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("IcoSizeNotSelected");
            }
            else if (operationKind is OperationKind.InsiderPreviewSettings)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("InsiderPreviewSuccessfully");
            }
            else if (operationKind is OperationKind.LanguageChange)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("LanguageChange");
            }
            else if (operationKind is OperationKind.ListEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("ListEmpty");
            }
            else if (operationKind is OperationKind.MenuDarkThemeIconPathEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("MenuDarkThemeIconPathEmpty");
            }
            else if (operationKind is OperationKind.MenuDefaultIconPathEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("MenuDefaultIconPathEmpty");
            }
            else if (operationKind is OperationKind.MenuLightThemeIconPathEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("MenuLightThemeIconPathEmpty");
            }
            else if (operationKind is OperationKind.MenuProgramPathEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("MenuProgramPathEmpty");
            }
            else if (operationKind is OperationKind.MenuMatchRuleEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("MenuMatchRuleEmpty");
            }
            else if (operationKind is OperationKind.MenuTitleEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("MenuTitleEmpty");
            }
            else if (operationKind is OperationKind.NoOperation)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("NoOperation");
            }
            else if (operationKind is OperationKind.RestoreContentEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("RestoreContentEmpty");
            }
            else if (operationKind is OperationKind.RestoreSpecificExtensionGroupsTypeEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("RestoreSpecificExtensionGroupsTypeEmpty");
            }
            else if (operationKind is OperationKind.SameDriveAndSelectFolder)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("SameDriveAndSelectFolder");
            }
            else if (operationKind is OperationKind.SelectDriverEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("SelectDriverEmpty");
            }
            else if (operationKind is OperationKind.SelectFolderEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("SelectFolderEmpty");
            }
            else if (operationKind is OperationKind.SelectLogFolderEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("SelectLogFolderEmpty");
            }
            else if (operationKind is OperationKind.ShellMenuNeedToRefreshData)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("ShellMenuNeedToRefreshData");
            }
            else if (operationKind is OperationKind.ThemeChangeSameTime)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("ThemeChangeSameTime");
            }
            else if (operationKind is OperationKind.ThemeSwitchSaveResult)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("ThemeSwitchSaveResult");
            }
            else if (operationKind is OperationKind.ThemeSwitchRestoreResult)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.NotificationTipResource.GetString("ThemeSwitchRestoreResult");
            }
            else if (operationKind is OperationKind.WinFRNotInstalled)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.NotificationTipResource.GetString("WinFRNotInstalled");
            }
        }

        public OperationResultNotificationTip(OperationKind operationKind, bool operationResult)
        {
            InitializeComponent();

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
            }
        }

        public OperationResultNotificationTip(OperationKind operationKind, int successItems, int failedItems)
        {
            InitializeComponent();

            if (operationKind is OperationKind.File)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0; ;
                OperationContent = failedItems is 0 ? string.Format(ResourceService.NotificationTipResource.GetString("FileResultSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("FileResultFailed"), successItems, failedItems);
            }
            else if (operationKind is OperationKind.IconExtract)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0;
                OperationContent = failedItems is 0 && successItems is not 0 ? string.Format(ResourceService.NotificationTipResource.GetString("IconExtractSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("IconExtractFailed"), successItems, failedItems);
            }
            else if (operationKind is OperationKind.ScheduledTaskDelete)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0;
                OperationContent = failedItems is 0 && successItems is not 0 ? string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskDeleteSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskDeleteFailed"), successItems, failedItems);
            }
            else if (operationKind is OperationKind.ScheduledTaskDisable)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0;
                OperationContent = failedItems is 0 && successItems is not 0 ? string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskDisableSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskDisableFailed"), successItems, failedItems);
            }
            else if (operationKind is OperationKind.ScheduledTaskEnable)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0;
                OperationContent = failedItems is 0 && successItems is not 0 ? string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskEnableSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskEnableFailed"), successItems, failedItems);
            }
            else if (operationKind is OperationKind.ScheduledTaskExport)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0;
                OperationContent = failedItems is 0 && successItems is not 0 ? string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskExportSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskExportFailed"), successItems, failedItems);
            }
            else if (operationKind is OperationKind.ScheduledTaskRun)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0;
                OperationContent = failedItems is 0 && successItems is not 0 ? string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskRunSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskRunFailed"), successItems, failedItems);
            }
            else if (operationKind is OperationKind.ScheduledTaskStop)
            {
                IsSuccessOperation = failedItems is 0 && successItems is not 0;
                OperationContent = failedItems is 0 && successItems is not 0 ? string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskStopSuccessfully"), successItems) : string.Format(ResourceService.NotificationTipResource.GetString("ScheduledTaskStopFailed"), successItems, failedItems);
            }
        }

        public OperationResultNotificationTip(OperationKind operationKind, int statusKind)
        {
            InitializeComponent();

            if (operationKind is OperationKind.CheckUpdate)
            {
                if (statusKind is 0)
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.NotificationTipResource.GetString("NotNewestVersion");
                }
                else if (statusKind is 1)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.NotificationTipResource.GetString("NewestVersion");
                }
                else if (statusKind is 2)
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.NotificationTipResource.GetString("UpdateCheckFailed");
                }
            }
        }
    }
}
