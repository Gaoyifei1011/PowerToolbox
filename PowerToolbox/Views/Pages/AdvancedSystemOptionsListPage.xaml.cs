using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 高级系统选项——列表页面
    /// </summary>
    public sealed partial class AdvancedSystemOptionsListPage : Page
    {
        private AdvancedSystemOptionsPage advancedSystemOptionsPage;

        public AdvancedSystemOptionsListPage()
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

        #region 第二部分：高级系统选项——列表页面——挂载的事件

        /// <summary>
        /// 打开高级系统选项——个性化页面
        /// </summary>
        private void OnPersonalizationClicked(object sender, RoutedEventArgs args)
        {
            advancedSystemOptionsPage?.NavigateTo(advancedSystemOptionsPage.PageList[1], advancedSystemOptionsPage, true);
        }

        /// <summary>
        /// 打开高级系统选项——系统页面
        /// </summary>
        private void OnSystemClicked(object sender, RoutedEventArgs args)
        {
            advancedSystemOptionsPage?.NavigateTo(advancedSystemOptionsPage.PageList[2], advancedSystemOptionsPage, true);
        }

        #endregion 第二部分：高级系统选项——列表页面——挂载的事件
    }
}
