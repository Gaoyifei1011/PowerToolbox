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
        internal bool IsModifyingNow
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsModifyingNow)));
                }
            }
        }

        /// <summary>
        /// 文件解锁状态
        /// </summary>
        internal FileUnlockState FileUnlockState
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(FileUnlockState)));
                }
            }
        }

        internal int FileUnlockFinishedCount
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(FileUnlockFinishedCount)));
                }
            }
        }

        internal int FileUnlockProgressingPercentage
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
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
