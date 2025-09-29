using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 计划任务管理页面
    /// </summary>
    public sealed partial class ScheduledTaskManagerPage : Page, INotifyPropertyChanged
    {
        private string _scheduledTaskDescription;

        public string ScheduledTaskDescription
        {
            get { return _scheduledTaskDescription; }

            set
            {
                if (!string.Equals(_scheduledTaskDescription, value))
                {
                    _scheduledTaskDescription = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScheduledTaskDescription)));
                }
            }
        }

        private ScheduledTaskResultKind _scheduledTaskResultKind;

        public ScheduledTaskResultKind ScheduledTaskResultKind
        {
            get { return _scheduledTaskResultKind; }

            set
            {
                if (!Equals(_scheduledTaskResultKind, value))
                {
                    _scheduledTaskResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScheduledTaskResultKind)));
                }
            }
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!string.Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }

        private List<ScheduledTaskModel> ScheduledTaskList { get; } = [];

        public List<ScheduledTaskModel> ScheduledTaskCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public ScheduledTaskManagerPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 搜索驱动名称
        /// </summary>
        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText) && ScheduledTaskResultKind is not ScheduledTaskResultKind.Loading && ScheduledTaskList.Count > 0)
            {
                ScheduledTaskResultKind = ScheduledTaskResultKind.Loading;
                ScheduledTaskCollection.Clear();
                //foreach (DriverModel driverItem in DriverList)
                //{
                //    if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                //    {
                //        driverItem.IsSelected = false;
                //        DriverCollection.Add(driverItem);
                //    }
                //}

                ScheduledTaskResultKind = ScheduledTaskCollection.Count is 0 ? ScheduledTaskResultKind.Failed : ScheduledTaskResultKind.Successfully;
                //ScheduledTaskFailedContent = ScheduledTaskCollection.Count is 0 ? ScheduledTaskEmptyWithConditionDescriptionString : string.Empty;
                //ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, ScheduledTaskCollection.Count(item => item.IsSelected));
            }
        }

        /// <summary>
        /// 搜索驱动名称内容发生变化事件
        /// </summary>
        private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchText = sender.Text;
            if (!string.IsNullOrEmpty(SearchText) && ScheduledTaskResultKind is not ScheduledTaskResultKind.Loading && ScheduledTaskList.Count > 0)
            {
                ScheduledTaskResultKind = ScheduledTaskResultKind.Loading;
                ScheduledTaskCollection.Clear();
                //foreach (DriverModel driverItem in DriverList)
                //{
                //    if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                //    {
                //        driverItem.IsSelected = false;
                //        DriverCollection.Add(driverItem);
                //    }
                //}

                ScheduledTaskResultKind = ScheduledTaskCollection.Count is 0 ? ScheduledTaskResultKind.Failed : ScheduledTaskResultKind.Successfully;
                //ScheduledTaskFailedContent = ScheduledTaskCollection.Count is 0 ? ScheduledTaskEmptyWithConditionDescriptionString : string.Empty;
                //ScheduledTaskDescription = string.Format(ScheduledTaskInformationString, ScheduledTaskCollection.Count, ScheduledTaskCollection.Count(item => item.IsSelected));
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (ScheduledTaskModel scheduledTaskItem in ScheduledTaskCollection)
            {
                scheduledTaskItem.IsSelected = true;
            }

            //DriverDescription = string.Format(DriverInformationString, ScheduledTaskCollection.Count, ScheduledTaskCollection.Count(item => item.IsSelected));
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (ScheduledTaskModel scheduledTaskItem in ScheduledTaskCollection)
            {
                scheduledTaskItem.IsSelected = false;
            }

            //DriverDescription = string.Format(DriverInformationString, DriverCollection.Count, DriverCollection.Count(item => item.IsSelected));
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 添加计划任务
        /// </summary>
        private void OnAddScheduledTaskClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 删除计划任务
        /// </summary>
        private void OnDeleteScheduledTaskClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 使用说明
        /// </summary>
        private void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 打开计划任务程序
        /// </summary>
        private void OnOpenScheduledTaskProgramClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("taskschd.msc");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ScheduledTaskManagerPage), nameof(OnOpenScheduledTaskProgramClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 获取是否正在加载中或操作中
        /// </summary>
        private bool GetIsLoadingOrOperating(ScheduledTaskResultKind scheduledTaskResultKind)
        {
            return !(scheduledTaskResultKind is ScheduledTaskResultKind.Loading || scheduledTaskResultKind is ScheduledTaskResultKind.Operating);
        }
    }
}
