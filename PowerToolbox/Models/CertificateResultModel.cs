using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 文件证书操作结果数据模型
    /// </summary>
    internal sealed class CertificateResultModel : INotifyPropertyChanged
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
        /// 文件名称
        /// </summary>
        private string _fileName;

        internal string FileName
        {
            get { return _fileName; }

            set
            {
                if (!string.Equals(_fileName, value))
                {
                    _fileName = value;
                    PropertyChanged?.Invoke(this, new(nameof(FileName)));
                }
            }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        private string _filePath;

        internal string FilePath
        {
            get { return _filePath; }

            set
            {
                if (!string.Equals(_filePath, value))
                {
                    _filePath = value;
                    PropertyChanged?.Invoke(this, new(nameof(FilePath)));
                }
            }
        }

        /// <summary>
        /// 操作结果
        /// </summary>
        private bool _result;

        internal bool Result
        {
            get { return _result; }

            set
            {
                if (!Equals(_result, value))
                {
                    _result = value;
                    PropertyChanged?.Invoke(this, new(nameof(Result)));
                }
            }
        }

        /// <summary>
        /// 文件证书是否已经修改过
        /// </summary>
        private bool _isModified;

        internal bool IsModified
        {
            get { return _isModified; }

            set
            {
                if (!Equals(_isModified, value))
                {
                    _isModified = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsModified)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
