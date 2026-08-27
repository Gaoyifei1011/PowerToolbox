using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace PowerToolboxShellExtension.WindowsAPI.ComTypes
{
    /// <summary>
    /// 公开检索有关文件夹显示选项的信息的方法，选择该文件夹中的指定项目，并设置文件夹的视图模式。
    /// </summary>
    [GeneratedComInterface, Guid("CDE725B0-CCC9-4519-917E-325D72FAB4CE")]
    internal partial interface IFolderView
    {
        /// <summary>
        /// 获取包含表示文件夹当前视图模式的值的地址。
        /// </summary>
        /// <param name="pViewMode">指向存储文件夹当前视图模式的内存位置的指针。 该地址处的值是以下 FOLDERVIEWMODE 值之一。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetCurrentViewMode(out uint pViewMode);

        /// <summary>
        /// 设置所选文件夹的视图模式。
        /// </summary>
        /// <param name="ViewMode">FOLDERVIEWMODE 枚举中的以下值之一。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetCurrentViewMode(uint ViewMode);

        /// <summary>
        /// 获取文件夹对象。
        /// </summary>
        /// <param name="riid">对表示文件夹的所需 IID 的引用。.</param>
        /// <param name="ppv">此方法返回时，包含 riid 中请求的接口指针。 这通常是 IShellFolder 或相关的接口。 这也可以是具有单个元素的 IShellItemArray 。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetFolder(in Guid riid, out nint ppv);

        /// <summary>
        /// 按索引获取文件夹视图中特定项的标识符。
        /// </summary>
        /// <param name="iItemIndex">视图中项的索引。</param>
        /// <param name="ppidl">指向包含项标识符信息的 PIDL 的指针的地址。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Item(int iItemIndex, out nint ppidl);

        /// <summary>
        /// 获取文件夹中的项目数。 这可以是所有项的数目，也可以是一个子集，例如选定项的数目。
        /// </summary>
        /// <param name="uFlags">来自_SVGIO枚举的标志，这些标志将计数限制为某些类型的项。</param>
        /// <param name="pcItems">指向一个整数的指针，该整数接收文件夹视图中显示的（文件和文件夹）项数。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int ItemCount(uint uFlags, out int pcItems);

        /// <summary>
        /// 根据文件夹视图中项的集合获取枚举对象的地址。
        /// </summary>
        /// <param name="uFlags">_SVGIO 将枚举限制为某些类型的项的值。</param>
        /// <param name="riid">对用于表示文件夹的所需 IID 的引用。</param>
        /// <param name="ppv">此方法返回时，包含 riid 中请求的接口指针。 这通常是 IEnumIDList、 IDataObject 或 IShellItemArray。 如果发生错误，此值为 NULL。</param>
        [PreserveSig]
        int Items(uint uFlags, in Guid riid, out nint ppv);

        /// <summary>
        /// 获取文件夹视图中已使用 IFolderView：：SelectItem 中的SVSI_SELECTIONMARK标记的项的索引。
        /// </summary>
        /// <param name="piItem">指向标记项的索引的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetSelectionMarkedItem(out int piItem);

        /// <summary>
        /// Gets the index of the item that currently has focus in the folder's view.
        /// </summary>
        /// <param name="piItem">A pointer to the index of the item.</param>
        [PreserveSig]
        int GetFocusedItem(out int piItem);

        /// <summary>
        /// 获取项目在文件夹视图中的位置。
        /// </summary>
        /// <param name="pidl">指向 ITEMIDLIST 接口的指针。</param>
        /// <param name="ppt">指向结构的指针，该结构接收项左上角的位置。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetItemPosition(nint pidl, out nint ppt);

        /// <summary>
        /// 获取一个 POINT 结构，该结构包含项的宽度 (x) 和高度 (y) 尺寸（包括周围的空白）。
        /// </summary>
        /// <param name="ppt">指向现有结构的指针，该结构要填充文件夹视图中项的当前大小调整维度。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetSpacing(out nint ppt);

        /// <summary>
        /// 获取指向 POINT 结构的指针，其中包含项的默认宽度 (x) 和高度 (y) 度量值，包括周围的空白。
        /// </summary>
        /// <param name="ppt">指向现有结构的指针，该结构使用文件夹视图中项的默认大小调整维度填充。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetDefaultSpacing(out nint ppt);

        /// <summary>
        /// 获取文件夹的自动排列模式的当前状态。
        /// </summary>
        /// <returns>如果文件夹处于自动排列模式，则返回S_OK;如果不是，则S_FALSE。</returns>
        [PreserveSig]
        int GetAutoArrange();

        /// <summary>
        /// 在文件夹的视图中选择一个项目。
        /// </summary>
        /// <param name="iItem">在文件夹视图中要选择的项的索引。</param>
        /// <param name="dwFlags">_SVSIF常量之一，用于指定要应用的选择类型。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SelectItem(int iItem, uint dwFlags);

        /// <summary>
        /// 允许选择和定位文件夹视图中可见的项目。
        /// </summary>
        /// <param name="cidl">要选择的项数。</param>
        /// <param name="apidl">指向大小 为 cidl 的数组的指针，该数组包含项的 PIDL。</param>
        /// <param name="apt">指向 cidl 结构的数组的指针，其中包含 apidl 中每个对应元素应定位的位置。</param>
        /// <param name="dwFlags">_SVSIF常量之一，用于指定要应用的选择类型。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SelectAndPositionItems(uint cidl, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] nint[] apidl, nint apt, uint dwFlags);
    }
}
