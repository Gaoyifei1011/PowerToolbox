using System;
using WINSATLib;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 实现 IWinSATInitiateEvents 的类。实现这个类来获取进度信息和完成通知。
    /// </summary>
    internal class CWinSATCallbacks : IWinSATInitiateEvents
    {
        /// <summary>
        /// 评估的返回值。
        /// </summary>
        internal WINSAT_RESULT Result { get; private set; }

        /// <summary>
        /// 完成状态的说明。
        /// </summary>
        internal string Description { get; private set; }

        /// <summary>
        /// 评估的当前进度计时周期。
        /// </summary>
        internal uint CurrentTick { get; private set; }

        /// <summary>
        /// 评估的进度计时周期总数。
        /// </summary>
        internal uint TickTotal { get; private set; }

        /// <summary>
        /// 包含评估的当前状态的字符串。 此字符串在此回调的生命周期内有效。 在回调返回后，如果需要，请复制该字符串。
        /// </summary>
        internal string CurrentState { get; private set; }

        // 评估完成时触发的事件
        internal event EventHandler<EventArgs> StatusCompleted;

        // 评估取得进展时触发的事件
        internal event EventHandler<EventArgs> StatusUpdated;

        /// <summary>
        /// 在评估成功、失败或取消时接收通知。
        /// </summary>
        /// <param name="hresult">评估的返回值。 </param>
        /// <param name="strDescription">完成状态的说明。 此字符串在此回调的生命周期内有效。 如果在回调返回后需要，请复制字符串。</param>
        public void WinSATComplete(int hresult, string strDescription)
        {
            try
            {
                Result = (WINSAT_RESULT)hresult;
            }
            catch (Exception)
            {
                Result = WINSAT_RESULT.WINSAT_ERROR_COMPLETED_ERROR;
            }

            Description = strDescription;
            StatusCompleted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 在评估取得进展时接收通知。
        /// </summary>
        /// <param name="uCurrentTick">评估的当前进度计时周期</param>
        /// <param name="uTickTotal">评估的进度计时周期总数</param>
        /// <param name="strCurrentState">包含评估的当前状态的字符串。 此字符串在此回调的生命周期内有效。 在回调返回后，如果需要，请复制该字符串</param>
        public void WinSATUpdate(uint uCurrentTick, uint uTickTotal, string strCurrentState)
        {
            CurrentTick = uCurrentTick;
            TickTotal = uTickTotal;
            CurrentState = strCurrentState;
            StatusUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
