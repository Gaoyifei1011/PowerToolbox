﻿using System.ComponentModel;

namespace WindowsTools.Models
{
    /// <summary>
    /// 文件证书操作结果数据模型
    /// </summary>
    public sealed class CertificateResultModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        private string _fileName;

        public string FileName
        {
            get { return _fileName; }

            set
            {
                if (!Equals(_fileName, value))
                {
                    _fileName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileName)));
                }
            }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }

            set
            {
                if (!Equals(_filePath, value))
                {
                    _filePath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilePath)));
                }
            }
        }

        /// <summary>
        /// 操作结果
        /// </summary>
        private bool _result;

        public bool Result
        {
            get { return _result; }

            set
            {
                if (!Equals(_result, value))
                {
                    _result = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
                }
            }
        }

        /// <summary>
        /// 文件证书是否已经修改过
        /// </summary>
        private bool _isModified;

        public bool IsModified
        {
            get { return _isModified; }

            set
            {
                if (!Equals(_isModified, value))
                {
                    _isModified = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsModified)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
