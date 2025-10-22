using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 文件夹选取框
    /// </summary>
    public class OpenFolderDialog : IDisposable
    {
        private readonly Guid CLSID_FileOpenDialog = new("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7");
        private IFileOpenDialog FileOpenDialog;
        private Form parentForm;

        public string Description { get; set; } = string.Empty;

        public string SelectedPath { get; set; } = string.Empty;

        public Environment.SpecialFolder RootFolder { get; set; } = Environment.SpecialFolder.Desktop;

        public OpenFolderDialog(Form form = null)
        {
            if (form is null)
            {
                if (Form.ActiveForm is null)
                {
                    throw new Win32Exception("没有处于激活的窗口");
                }
                parentForm = Form.ActiveForm;
            }
            else
            {
                parentForm = form;
            }
        }

        ~OpenFolderDialog()
        {
            Dispose(false);
        }

        /// <summary>
        /// 显示文件夹选取对话框
        /// </summary>
        public DialogResult ShowDialog()
        {
            try
            {
                FileOpenDialog = (IFileOpenDialog)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_FileOpenDialog));
                FileOpenDialog.SetOptions(FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS | FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM);
                FileOpenDialog.SetTitle(Description);
                Shell32Library.SHCreateItemFromParsingName(Environment.GetFolderPath(RootFolder), null, typeof(IShellItem).GUID, out IShellItem initialFolder);
                FileOpenDialog.SetFolder(initialFolder);
                Marshal.ReleaseComObject(initialFolder);

                if (FileOpenDialog is not null)
                {
                    int result = FileOpenDialog.Show(parentForm.Handle);

                    if (result is not 0)
                    {
                        return DialogResult.Cancel;
                    }

                    FileOpenDialog.GetResult(out IShellItem pItem);
                    pItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out IntPtr pszString);
                    SelectedPath = Marshal.PtrToStringUni(pszString);
                    Marshal.ReleaseComObject(pItem);
                    return DialogResult.OK;
                }
                else
                {
                    return DialogResult.Cancel;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(OpenFolderDialog), nameof(ShowDialog), 1, e);
                if (FileOpenDialog is not null)
                {
                    Marshal.FinalReleaseComObject(FileOpenDialog);
                }
                return DialogResult.Cancel;
            }
        }

        /// <summary>
        /// 释放打开文件夹选取对话框所需的资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            lock (this)
            {
                if (FileOpenDialog is not null)
                {
                    Marshal.FinalReleaseComObject(FileOpenDialog);
                }

                parentForm = null;
                FileOpenDialog = null;
            }
        }
    }
}
