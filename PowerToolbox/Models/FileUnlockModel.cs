using PowerToolbox.Extensions.DataType.Enums;
using System.Collections.Generic;
using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 文件解锁数据类型
    /// </summary>
    public class FileUnlockModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 是否为目录
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// 文件 / 文件夹名称
        /// </summary>
        public string FileFolderName { get; set; }

        /// <summary>
        /// 文件 / 文件夹路径
        /// </summary>
        public string FileFolderPath { get; set; }

        /// <summary>
        /// 文件 / 文件夹类型
        /// </summary>
        public string FileFolderType { get; set; }

        /// <summary>
        /// 文件 / 文件夹数量
        /// </summary>
        public string FileFolderAmount { get; set; }

        /// <summary>
        /// 是否处于修改状态中
        /// </summary>
        private bool _isModifyingNow;

        public bool IsModifyingNow
        {
            get { return _isModifyingNow; }

            set
            {
                if (!Equals(_isModifyingNow, value))
                {
                    _isModifyingNow = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsModifyingNow)));
                }
            }
        }

        /// <summary>
        /// 文件解锁状态
        /// </summary>
        private FileUnlockState _fileUnlockState;

        public FileUnlockState FileUnlockState
        {
            get { return _fileUnlockState; }

            set
            {
                if (!Equals(_fileUnlockState, value))
                {
                    _fileUnlockState = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileUnlockState)));
                }
            }
        }

        private int _fileUnlockFinishedCount;

        public int FileUnlockFinishedCount
        {
            get { return _fileUnlockFinishedCount; }

            set
            {
                if (!Equals(_fileUnlockFinishedCount, value))
                {
                    _fileUnlockFinishedCount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileUnlockFinishedCount)));
                }
            }
        }

        private int _fileUnlockProcessingPercentage;

        public int FileUnlockProgressingPercentage
        {
            get { return _fileUnlockProcessingPercentage; }

            set
            {
                if (!Equals(_fileUnlockProcessingPercentage, value))
                {
                    _fileUnlockProcessingPercentage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileUnlockProgressingPercentage)));
                }
            }
        }

        /// <summary>
        /// 子文件列表
        /// </summary>
        public List<string> SubFileList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
