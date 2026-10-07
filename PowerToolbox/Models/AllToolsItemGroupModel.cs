using System.Collections.Generic;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 所有工具页面项目分组模型
    /// </summary>
    internal sealed class AllToolsItemGroupModel
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 主页面项目数据列表
        /// </summary>
        public List<AllToolsItemModel> AllToolsItemList { get; set; }
    }
}
