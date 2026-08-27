using System;

// 抑制 CA1069 警告
#pragma warning disable CA1069

namespace PowerToolbox.WindowsAPI.PInvoke.Shlwapi
{
    /// <summary>
    /// STGM 常量是指示创建和删除对象的条件以及对象的访问模式的标志。 STGM 常量包含在 IStorage、 IStream 和 IPropertySetStorage 接口以及 StgCreateDocfile、 StgCreateStorageEx、 StgCreateDocfileOnILockBytes、 StgOpenStorage 和 StgOpenStorageEx 函数中。
    /// </summary>
    [Flags]
    internal enum STGM : uint
    {
        /// <summary>
        /// 指示在直接模式下，每次对存储或流元素的更改都会在发生时写入。 如果既未指定 STGM_DIRECT ，也未指定 STGM_TRANSACTED ，则这是默认值。
        /// </summary>
        STGM_DIRECT = 0x00000000,

        /// <summary>
        /// 如果存在具有指定名称的现有对象，则会导致创建操作失败。 在这种情况下，返回 STG_E_FILEALREADYEXISTS 。 这是默认创建模式;也就是说，如果未指定其他创建标志，则隐式 STGM_FAILIFTHERE 。
        /// </summary>
        STGM_FAILIFTHERE = 0x0000000,

        /// <summary>
        /// 指示对象为只读，这意味着无法进行修改。 例如，如果使用 STGM_READ 打开流对象，可以调用 ISequentialStream：：Read 方法，但 ISequentialStream：：Write 方法可能不调用。 同样，如果使用 STGM_READ 打开存储对象，可以调用 IStorage：：OpenStream 和 IStorage：：OpenStorage 方法，但 IStorage：：CreateStream 和 IStorage：：CreateStorage 方法可能不调用。
        /// </summary>
        STGM_READ = 0x00000000,

        /// <summary>
        /// 允许您保存对对象的更改，但不允许访问其数据。 提供的 IPropertyStorage 和 IPropertySetStorage 接口的实现不支持此只写模式。
        /// </summary>
        STGM_WRITE = 0x00000001,

        /// <summary>
        /// 启用对对象数据的访问和修改。 例如，如果在此模式下创建或打开流对象，可以同时调用 IStream：：Read 和 IStream：：Write。 请注意，此常量不是STGM_WRITE和STGM_READ元素的简单二进制 OR 运算。
        /// </summary>
        STGM_READWRITE = 0x00000002,

        /// <summary>
        /// 防止其他人随后以任何模式打开对象。 请注意，此值不是STGM_SHARE_DENY_READ和STGM_SHARE_DENY_WRITE值的简单按位 OR 运算。 在事务处理模式下，共享 STGM_SHARE_DENY_WRITE 或 STGM_SHARE_EXCLUSIVE 可以显著提高性能，因为它们不需要快照。
        /// </summary>
        STGM_SHARE_EXCLUSIVE = 0x00000010,

        /// <summary>
        /// 防止其他人随后打开对象进行 STGM_WRITE 或 STGM_READWRITE 访问。 在事务处理模式下，共享 STGM_SHARE_DENY_WRITE 或 STGM_SHARE_EXCLUSIVE 可以显著提高性能，因为它们不需要快照。
        /// </summary>
        STGM_SHARE_DENY_WRITE = 0x00000020,

        /// <summary>
        /// 阻止其他人随后在 STGM_READ 模式下打开对象。 它通常用于根存储对象。
        /// </summary>
        STGM_SHARE_DENY_READ = 0x00000030,

        /// <summary>
        /// 指定不拒绝对象的后续打开读取或写入访问。 如果未指定共享组中的标志，则假定此标志。
        /// </summary>
        STGM_SHARE_DENY_NONE = 0x00000040,

        /// <summary>
        /// 指示应在新对象替换现有存储对象或流之前删除它。 仅当成功删除现有对象时才指定此标志时，才会创建新对象。
        /// </summary>
        STGM_CREATE = 0x00001000,

        /// <summary>
        /// 创建新对象，同时保留名为“Contents”的流中的现有数据。 对于存储对象或字节数组，无论现有文件或字节数组当前是否包含分层存储对象，旧数据都会格式化为流。 此标志只能在创建根存储对象时使用。 它不能在存储对象中使用;例如，在 IStorage：：CreateStream 中。 同时使用此标志和 STGM_DELETEONRELEASE 标志也无效。
        /// </summary>
        STGM_CONVERT = 0x0002000,

        /// <summary>
        /// 指示在事务处理模式下，仅当调用显式提交操作时，才会缓冲和写入更改。 若要忽略这些更改，请在 IStream、IStorage 或 IPropertyStorage 接口中调用 Revert 方法。 IStorage 的 COM 复合文件实现不支持事务处理流，这意味着只能在直接模式下打开流，并且不能还原更改它们，但支持事务处理存储。 IPropertySetStorage 的复合文件、独立和 NTFS 文件系统实现同样不支持事务处理简单属性集，因为这些属性集存储在流中。 但是，支持非简单属性集的事务处理，可以通过在 IPropertySetStorage：：Create 的 grfFlags 参数中指定 PROPSETFLAG_NONSIMPLE 标志来创建。
        /// </summary>
        STGM_TRANSACTED = 0x00010000,

        /// <summary>
        /// 打开具有对最近提交的版本的独占访问权限的存储对象。 因此，在优先级模式下打开对象时，其他用户无法提交对对象的更改。 你可以获得复制操作的性能优势，但会阻止其他人提交更改。 限制对象在优先级模式下打开的时间。 必须使用优先级模式指定 STGM_DIRECT 和 STGM_READ ，并且不能指定 STGM_DELETEONRELEASE。 STGM_DELETEONRELEASE 仅在创建根对象时有效，例如使用 StgCreateStorageEx。 打开现有根对象（例如 使用 StgOpenStorageEx）时，它无效。 创建或打开子元素（例如 使用 IStorage：：OpenStorage）时，它也无效。
        /// </summary>
        STGM_PRIORITY = 0x00040000,

        /// <summary>
        /// 指示在事务处理模式下，通常使用临时暂存文件来保存修改，直到调用 Commit 方法。 指定 STGM_NOSCRATCH 允许将原始文件的未使用部分用作工作区，而不是为此创建新文件。 这不会影响原始文件中的数据，在某些情况下可以提高性能。 如果不同时指定 STGM_TRANSACTED，则指定此标志无效，并且此标志只能在根打开的情况下使用。 有关 NoScratch 模式的详细信息，请参阅备注部分。
        /// </summary>
        STGM_NOSCRATCH = 0x00100000,

        /// <summary>
        /// 当使用 STGM_TRANSACTED 且不使用 STGM_SHARE_EXCLUSIVE 或 STGM_SHARE_DENY_WRITE 打开存储对象时 ，使用此标志。 在这种情况下，指定STGM_NOSNAPSHOT可防止系统提供的实现创建文件快照副本。 相反，对文件的更改将写入文件的末尾。 除非在提交期间执行合并，并且文件上只有一个当前编写器，否则不会回收未使用的空间。 在没有快照模式下打开文件时，如果不指定STGM_NOSNAPSHOT，则无法执行另一个打开操作。 此标志只能在根打开操作中使用。 有关 NoSnapshot 模式的详细信息，请参阅备注部分。
        /// </summary>
        STGM_NOSNAPSHOT = 0x00200000,

        /// <summary>
        /// 支持单编写器、多读取程序文件操作的直接模式。 有关详细信息，请参见“备注”部分。
        /// </summary>
        STGM_DIRECT_SWMR = 0x00400000,

        /// <summary>
        /// 在有限但经常使用的情况下，提供复合文件的更快实现。 有关详细信息，请参见“备注”部分。
        /// </summary>
        STGM_SIMPLE = 0x08000000,

        /// <summary>
        /// 指示释放根存储对象时，基础文件将被自动销毁。 此功能最适用于创建临时文件。 此标志只能在创建根对象时使用，例如使用 StgCreateStorageEx。 在打开根对象（例如使用 StgOpenStorageEx）或创建或打开子元素（例如使用 IStorage：：CreateStream）时，它无效。 同时使用此标志和STGM_CONVERT标志也无效。
        /// </summary>
        STGM_DELETEONRELEASE = 0x04000000
    }
}
