using PowerToolbox.Extensions.DataType.Enums;
using System.Collections.Generic;
using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 文件解锁数据类型
    /// </summary>
    internal sealed class FileUnlockModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 是否为目录
        /// </summary>
        internal bool IsDirectory { get; set; }

        /// <summary>
        /// 文件 / 文件夹名称
        /// </summary>
        internal string FileFolderName { get; set; }

        /// <summary>
        /// 文件 / 文件夹路径
        /// </summary>
        internal string FileFolderPath { get; set; }

        /// <summary>
        /// 文件 / 文件夹类型
        /// </summary>
        internal string FileFolderType { get; set; }

        /// <summary>
        /// 文件 / 文件夹数量
        /// </summary>
        internal string FileFolderAmount { get; set; }

        /// <summary>
        /// 是否处于修改状态中
        /// </summary>
        private bool _isModifyingNow;

        internal bool IsModifyingNow
        {
            get { return _isModifyingNow; }

            set
            {
                if (!Equals(_isModifyingNow, value))
                {
                    _isModifyingNow = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsModifyingNow)));
                }
            }
        }

        /// <summary>
        /// 文件解锁状态
        /// </summary>
        private FileUnlockState _fileUnlockState;

        internal FileUnlockState FileUnlockState
        {
            get { return _fileUnlockState; }

            set
            {
                if (!Equals(_fileUnlockState, value))
                {
                    _fileUnlockState = value;
                    PropertyChanged?.Invoke(this, new(nameof(FileUnlockState)));
                }
            }
        }

        private int _fileUnlockFinishedCount;

        internal int FileUnlockFinishedCount
        {
            get { return _fileUnlockFinishedCount; }

            set
            {
                if (!Equals(_fileUnlockFinishedCount, value))
                {
                    _fileUnlockFinishedCount = value;
                    PropertyChanged?.Invoke(this, new(nameof(FileUnlockFinishedCount)));
                }
            }
        }

        private int _fileUnlockProcessingPercentage;

        internal int FileUnlockProgressingPercentage
        {
            get { return _fileUnlockProcessingPercentage; }

            set
            {
                if (!Equals(_fileUnlockProcessingPercentage, value))
                {
                    _fileUnlockProcessingPercentage = value;
                    PropertyChanged?.Invoke(this, new(nameof(FileUnlockProgressingPercentage)));
                }
            }
        }

        /// <summary>
        /// 子文件列表
        /// </summary>
        internal List<string> SubFileList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
