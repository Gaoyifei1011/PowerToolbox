using Windows.Foundation.Collections;

namespace PowerToolbox.Extensions.Collections
{
    /// <summary>
    /// 向量更改时触发的事件参数
    /// </summary>
    public class VectorChangedEventArgs : IVectorChangedEventArgs
    {
        public CollectionChange CollectionChange { get; set; }

        public uint Index { get; set; }
    }
}
