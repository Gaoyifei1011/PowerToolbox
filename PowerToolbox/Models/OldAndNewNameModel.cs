using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 文件名称模型
    /// </summary>
    internal sealed class OldAndNewNameModel : INotifyPropertyChanged
    {
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
        /// 文件的初始名称
        /// </summary>
        private string _originalFileName;

        internal string OriginalFileName
        {
            get { return _originalFileName; }

            set
            {
                if (!string.Equals(_originalFileName, value))
                {
                    _originalFileName = value;
                    PropertyChanged?.Invoke(this, new(nameof(OriginalFileName)));
                }
            }
        }

        /// <summary>
        /// 文件的初始路径
        /// </summary>
        private string _originalFilePath;

        internal string OriginalFilePath
        {
            get { return _originalFilePath; }

            set
            {
                if (!string.Equals(_originalFilePath, value))
                {
                    _originalFilePath = value;
                    PropertyChanged?.Invoke(this, new(nameof(OriginalFilePath)));
                }
            }
        }

        /// <summary>
        /// 文件新名称
        /// </summary>
        private string _newFileName;

        internal string NewFileName
        {
            get { return _newFileName; }

            set
            {
                if (!string.Equals(_newFileName, value))
                {
                    _newFileName = value;
                    PropertyChanged?.Invoke(this, new(nameof(NewFileName)));
                }
            }
        }

        /// <summary>
        /// 文件新名称
        /// </summary>
        private string _newFilePath;

        internal string NewFilePath
        {
            get { return _newFilePath; }

            set
            {
                if (!string.Equals(_newFilePath, value))
                {
                    _newFilePath = value;
                    PropertyChanged?.Invoke(this, new(nameof(NewFilePath)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
