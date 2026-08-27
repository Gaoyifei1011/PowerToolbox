using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 文件属性模型
    /// </summary>
    internal sealed class OldAndNewPropertiesModel : INotifyPropertyChanged
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
        /// 文件属性
        /// </summary>
        private string _fileProperties;

        internal string FileProperties
        {
            get { return _fileProperties; }

            set
            {
                if (!string.Equals(_fileProperties, value))
                {
                    _fileProperties = value;
                    PropertyChanged?.Invoke(this, new(nameof(FileProperties)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
