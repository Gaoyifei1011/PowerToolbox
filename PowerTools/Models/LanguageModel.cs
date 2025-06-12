﻿using System.Collections.Generic;
using System.ComponentModel;

namespace PowerTools.Models
{
    /// <summary>
    /// 语言模型
    /// </summary>
    public sealed partial class LanguageModel : INotifyPropertyChanged
    {
        public KeyValuePair<string, string> LangaugeInfo { get; set; }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (!Equals(_isChecked, value))
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
