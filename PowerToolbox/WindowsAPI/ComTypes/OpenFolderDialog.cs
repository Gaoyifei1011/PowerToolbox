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
    public class OpenFolderDialog(nint handle) : IDisposable
    {
        private readonly Guid CLSID_FileOpenDialog = new("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7");
        private IFileOpenDialog fileOpenDialog;

        private nint Handle { get; } = handle is 0 ? throw new Win32Exception("窗口句柄不可以为空") : handle;

        public string Description { get; set; } = string.Empty;

        public string SelectedPath { get; set; } = string.Empty;

        public Environment.SpecialFolder RootFolder { get; set; } = Environment.SpecialFolder.Desktop;

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
                fileOpenDialog = (IFileOpenDialog)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_FileOpenDialog));
                fileOpenDialog.SetOptions(FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS | FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM);
                fileOpenDialog.SetTitle(Description);
                Shell32Library.SHCreateItemFromParsingName(Environment.GetFolderPath(RootFolder), null, typeof(IShellItem).GUID, out IShellItem initialFolder);
                fileOpenDialog.SetFolder(initialFolder);
                Marshal.ReleaseComObject(initialFolder);

                if (fileOpenDialog is not null)
                {
                    int result = fileOpenDialog.Show(Handle);

                    if (result is not 0)
                    {
                        return DialogResult.Cancel;
                    }

                    fileOpenDialog.GetResult(out IShellItem pItem);
                    pItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out nint pszString);
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
                if (fileOpenDialog is not null)
                {
                    Marshal.FinalReleaseComObject(fileOpenDialog);
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
                if (fileOpenDialog is not null)
                {
                    Marshal.FinalReleaseComObject(fileOpenDialog);
                }

                fileOpenDialog = null;
            }
        }
    }
}
