using System;
using WUApiLib;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 包含处理异步搜索操作完成通知的方法。 此接口由调用 IUpdateSearcher.BeginSearch 方法的程序员实现。
    /// </summary>
    internal class SearchCompletedCallback : ISearchCompletedCallback
    {
        internal ISearchJob SearchJob { get; private set; }

        internal ISearchCompletedCallbackArgs CallbackArgs { get; private set; }

        internal event EventHandler<EventArgs> SearchCompleted;

        /// <summary>
        /// 处理通过调用 IUpdateSearcher.BeginSearch 方法启动的异步搜索完成通知。
        /// </summary
        public void Invoke(ISearchJob searchJob, ISearchCompletedCallbackArgs callbackArgs)
        {
            SearchJob = searchJob;
            CallbackArgs = callbackArgs;
            SearchCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
