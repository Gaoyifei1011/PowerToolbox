using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Services.Root;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 高级系统选项——个性化页面
    /// </summary>
    public sealed partial class AdvancedSystemOptionsPersonalizationPage : Page, INotifyPropertyChanged
    {
        private AdvancedSystemOptionsPage advancedSystemOptionsPage;

        private bool _isRebuildingIconCache;

        public bool IsRebuildingIconCache
        {
            get { return _isRebuildingIconCache; }

            set
            {
                if (!Equals(_isRebuildingIconCache, value))
                {
                    _isRebuildingIconCache = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRebuildingIconCache)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AdvancedSystemOptionsPersonalizationPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (args.Parameter is AdvancedSystemOptionsPage targetPage && !Equals(advancedSystemOptionsPage, targetPage))
            {
                advancedSystemOptionsPage = targetPage;
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：高级系统选项——个性化页面——挂载的事件

        /// <summary>
        /// 重建图标缓存
        /// </summary>
        private async void OnRebuildIconCacheClicked(object sender, RoutedEventArgs args)
        {
            if (!IsRebuildingIconCache)
            {
                IsRebuildingIconCache = true;
                await Task.Run(async () =>
                {
                    try
                    {
                        Process taskKillProcess = Process.Start(new ProcessStartInfo
                        {
                            FileName = "taskkill",
                            Arguments = "/IM explorer.exe /F",
                            Verb = "open",
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                        });
                        taskKillProcess.WaitForExit();
                        taskKillProcess.Dispose();
                        while (Process.GetProcessesByName("explorer").FirstOrDefault() is not null)
                        {
                            await Task.Delay(1000);
                        }

                        string iconCacheDbFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IconCache.db");
                        if (File.Exists(iconCacheDbFile))
                        {
                            File.Delete(iconCacheDbFile);
                        }
                        string explorerFolder = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Windows\Explorer"));
                        foreach (FileInfo fileInfo in from file in new DirectoryInfo(explorerFolder).EnumerateFiles() where file.Name.Contains("iconcache") || file.Name.Contains("thumbcache") select file)
                        {
                            fileInfo.Delete();
                        }
                    }
                    catch (Win32Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnRebuildIconCacheClicked), 1, e);
                    }
                    finally
                    {
                        try
                        {
                            Process explorerProcess = Process.Start(new ProcessStartInfo
                            {
                                FileName = "explorer.exe",
                                Verb = "open",
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                            });
                            explorerProcess.Dispose();
                        }
                        catch (Win32Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPage), nameof(OnRebuildIconCacheClicked), 2, e);
                        }
                    }
                });
                IsRebuildingIconCache = false;
            }
        }

        #endregion 第二部分：高级系统选项——个性化页面——挂载的事件
    }
}
