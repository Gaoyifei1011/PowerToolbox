using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WINSATLib;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 系统评估页面
    /// </summary>
    internal sealed partial class WinSATPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string ErrorMessageString = ResourceService.WinSATResource.GetString("ErrorMessage");
        private readonly string SuccessMessageString = ResourceService.WinSATResource.GetString("SuccessMessage");
        private readonly string WEIContentString = ResourceService.WinSATResource.GetString("WEITipContent");
        private readonly string WEIString = ResourceService.WinSATResource.GetString("WEI");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private readonly CInitiateWinSAT cInitiateWinSAT = new();
        private readonly Guid CLSID_ProgressDialog = new("F8383852-FCD3-11d1-A6B9-006097DF5BD4");
        private bool isInitialized;
        private _RemotableHandle _RemotableHandle = new();
        private WinSATInitiateEvents winSATInitiateEvents;
        private IProgressDialog progressDialog;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private string ProcessorSubScore
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(ProcessorSubScore)));
                }
            }
        }

        private string MemorySubScore
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(MemorySubScore)));
                }
            }
        }

        private string GraphicsSubScore
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(GraphicsSubScore)));
                }
            }
        }

        private string GamingGraphicsSubScore
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(GamingGraphicsSubScore)));
                }
            }
        }

        private string PrimaryDiskSubScore
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(PrimaryDiskSubScore)));
                }
            }
        }

        private string BasicScore
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(BasicScore)));
                }
            }
        }

        private bool BasicScoreExisted
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(BasicScoreExisted)));
                }
            }
        }

        private bool IsNotRunningAssessment
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsNotRunningAssessment)));
                }
            }
        }

        private InfoBarSeverity ResultSeverity
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(ResultSeverity)));
                }
            }
        }

        private string ResultMessage
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(ResultMessage)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal WinSATPage()
        {
            InitializeComponent();
            IsNotRunningAssessment = true;
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                await GetWinSATInfoAsync();
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 运行评估
        /// </summary>
        private void OnRunAssessmentClicked(object sender, RoutedEventArgs args)
        {
            IsNotRunningAssessment = false;
            RunAssessment();
        }

        /// <summary>
        /// 打开评估日志目录
        /// </summary>
        private void OnOpenAssessmentLogFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenAssessmentLogFolder();
        }

        /// <summary>
        /// 了解系统评估
        /// </summary>
        private void OnLearnSystemAssessmentClicked(object sender, RoutedEventArgs args)
        {
            LearnSystemAssessment();
        }

        /// <summary>
        /// 评估取得进展时触发的事件
        /// </summary>
        private void OnStatusUpdated(object sender, EventArgs args)
        {
            if (winSATInitiateEvents is not null && progressDialog is not null)
            {
                synchronizationContext.Post(_ =>
                {
                    try
                    {
                        if (progressDialog is not null)
                        {
                            // 用户主动取消了操作
                            if (progressDialog.HasUserCanceled())
                            {
                                progressDialog.StopProgressDialog();
                                cInitiateWinSAT.CancelAssessment();
                                winSATInitiateEvents.StatusCompleted -= OnStatusUpdated;
                                winSATInitiateEvents.StatusUpdated -= OnStatusUpdated;
                                winSATInitiateEvents = null;
                                progressDialog = null;
                                IsNotRunningAssessment = true;
                                return;
                            }

                            progressDialog.SetLine(1, winSATInitiateEvents.CurrentState, false, 0);
                            progressDialog.SetProgress(winSATInitiateEvents.CurrentTick, winSATInitiateEvents.TickTotal);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinSATPage), nameof(OnStatusUpdated), 1, e);
                        winSATInitiateEvents = null;
                        progressDialog = null;
                        IsNotRunningAssessment = true;
                    }
                }, null);
            }
        }

        /// <summary>
        /// 评估完成时触发的事件
        /// </summary>
        private void OnStatusCompleted(object sender, EventArgs args)
        {
            if (winSATInitiateEvents is not null && progressDialog is not null)
            {
                synchronizationContext.Post(async _ =>
                {
                    try
                    {
                        if (progressDialog is not null)
                        {
                            progressDialog.StopProgressDialog();
                            winSATInitiateEvents.StatusCompleted -= OnStatusUpdated;
                            winSATInitiateEvents.StatusUpdated -= OnStatusUpdated;
                            winSATInitiateEvents = null;
                            progressDialog = null;
                            await GetWinSATInfoAsync();
                            IsNotRunningAssessment = true;
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinSATPage), nameof(OnStatusCompleted), 1, e);
                        winSATInitiateEvents = null;
                        progressDialog = null;
                        IsNotRunningAssessment = true;
                    }
                }, null);
            }
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 加载系统评估信息
        /// </summary>
        private async Task GetWinSATInfoAsync()
        {
            double basicScore = 0.0;
            double processorSubScore = 0.0;
            double memorySubScore = 0.0;
            double graphicsSubScore = 0.0;
            double gamingGraphicsSubScore = 0.0;
            double primaryDiskSubScore = 0.0;
            dynamic assessmentDate = null;

            await Task.Run(() =>
            {
                CQueryWinSAT queryWinSAT = new();

                try
                {
                    basicScore = queryWinSAT.Info.SystemRating;
                    processorSubScore = queryWinSAT.Info.GetAssessmentInfo(WINSAT_ASSESSMENT_TYPE.WINSAT_ASSESSMENT_CPU).Score;
                    memorySubScore = queryWinSAT.Info.GetAssessmentInfo(WINSAT_ASSESSMENT_TYPE.WINSAT_ASSESSMENT_MEMORY).Score;
                    graphicsSubScore = queryWinSAT.Info.GetAssessmentInfo(WINSAT_ASSESSMENT_TYPE.WINSAT_ASSESSMENT_GRAPHICS).Score;
                    gamingGraphicsSubScore = queryWinSAT.Info.GetAssessmentInfo(WINSAT_ASSESSMENT_TYPE.WINSAT_ASSESSMENT_D3D).Score;
                    primaryDiskSubScore = queryWinSAT.Info.GetAssessmentInfo(WINSAT_ASSESSMENT_TYPE.WINSAT_ASSESSMENT_DISK).Score;
                    assessmentDate = queryWinSAT.Info.AssessmentDateTime;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinSATPage), nameof(GetWinSATInfoAsync), 1, e);
                }
            });

            BasicScoreExisted = basicScore is not 0.0;
            BasicScore = basicScore is 0.0 ? "N/A" : basicScore.ToString("F1");
            ProcessorSubScore = processorSubScore is 0.0 ? "N/A" : processorSubScore.ToString("F1");
            MemorySubScore = memorySubScore is 0.0 ? "N/A" : memorySubScore.ToString("F1");
            GraphicsSubScore = graphicsSubScore is 0.0 ? "N/A" : graphicsSubScore.ToString("F1");
            GamingGraphicsSubScore = gamingGraphicsSubScore is 0.0 ? "N/A" : gamingGraphicsSubScore.ToString("F1");
            PrimaryDiskSubScore = primaryDiskSubScore is 0.0 ? "N/A" : primaryDiskSubScore.ToString("F1");
            ResultMessage = basicScore is 0.0 ? ErrorMessageString : string.Format(SuccessMessageString, assessmentDate is null ? string.Empty : assessmentDate);
            ResultSeverity = basicScore is 0.0 ? InfoBarSeverity.Warning : InfoBarSeverity.Success;
        }

        /// <summary>
        /// 运行评估
        /// </summary>
        private void RunAssessment()
        {
            try
            {
                winSATInitiateEvents = new();

                if (winSATInitiateEvents is not null)
                {
                    winSATInitiateEvents.StatusUpdated += OnStatusUpdated;
                    winSATInitiateEvents.StatusCompleted += OnStatusCompleted;
                }

                cInitiateWinSAT.InitiateFormalAssessment(winSATInitiateEvents, ref _RemotableHandle);
                progressDialog = (IProgressDialog)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ProgressDialog));

                if (progressDialog is not null)
                {
                    progressDialog.SetTitle(WEIString);
                    progressDialog.SetLine(2, WEIContentString, false, 0);
                    progressDialog.StartProgressDialog((nint)MainWindow.Current.AppWindow.Id.Value, null, PROGDLG.PROGDLG_MODAL | PROGDLG.PROGDLG_NOMINIMIZE, 0);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinSATPage), nameof(RunAssessment), 1, e);
                winSATInitiateEvents = null;
                progressDialog = null;
            }
        }

        /// <summary>
        /// 打开评估日志目录
        /// </summary>
        private void OpenAssessmentLogFolder()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"performance\winsat\datastore"));
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinSATPage), nameof(OpenAssessmentLogFolder), 1, e);
                }
            });
        }

        /// <summary>
        /// 了解系统评估
        /// </summary>
        private void LearnSystemAssessment()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("https://learn.microsoft.com/zh-cn/windows-hardware/manufacture/desktop/configure-windows-system-assessment-test-scores");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(WinSATPage), nameof(LearnSystemAssessment), 1, e);
                }
            });
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
