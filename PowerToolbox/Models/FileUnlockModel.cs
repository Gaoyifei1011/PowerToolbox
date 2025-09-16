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
        /// 操作状态
        /// </summary>
        private string _fileFolderStatus;

        public string FileFolderStatus
        {
            get { return _fileFolderStatus; }

            set
            {
                if (!string.Equals(_fileFolderStatus, value))
                {
                    _fileFolderStatus = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileFolderStatus)));
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
