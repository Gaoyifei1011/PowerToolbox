namespace PowerToolbox.Extensions.DataType.Class
{
    /// <summary>
    /// 为 ExecuteRequested 事件提供事件数据。
    /// </summary>
    internal class ExecuteRequestedEventArgs(object parameter)
    {
        internal object Parameter { get; } = parameter;
    }
}
