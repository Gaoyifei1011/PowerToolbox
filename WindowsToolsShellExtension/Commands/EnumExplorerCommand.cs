﻿using System.Runtime.InteropServices.Marshalling;
using WindowsToolsShellExtension.WindowsAPI.ComTypes;

namespace WindowsToolsShellExtension.Commands
{
    /// <summary>
    /// 命令栏的命令枚举接口实现类
    /// </summary>
    [GeneratedComClass]
    public partial class EnumExplorerCommand(ExplorerCommand[] explorerCommands) : IEnumExplorerCommand
    {
        private readonly ExplorerCommand[] subExplorerCommandArray = explorerCommands is null ? ([]) : explorerCommands;
        private int index = 0;

        /// <summary>
        /// 目前尚未实现。
        /// </summary>
        public int Clone(out IEnumExplorerCommand ppenum)
        {
            EnumExplorerCommand enumExplorerCommand = new(subExplorerCommandArray)
            {
                index = index
            };
            ppenum = enumExplorerCommand;
            return 0;
        }

        /// <summary>
        /// 检索指定数量的直接跟随当前元素的元素。
        /// </summary>
        public int Next(uint celt, IExplorerCommand[] pUICommand, out uint pceltFetched)
        {
            pceltFetched = 0;

            if (subExplorerCommandArray.Length is 0)
            {
                return unchecked((int)0x80004001);
            }

            int start = index;
            for (int i = 0; i < celt && start + i < explorerCommands.Length; i++)
            {
                pUICommand[i] = subExplorerCommandArray[index];
                index++;
            }
            pceltFetched = (uint)(index - start);

            return index - start == celt ? 0 : 1;
        }

        /// <summary>
        /// 将枚举重置为 0。
        /// </summary>
        public int Reset()
        {
            index = 0;
            return 0;
        }

        /// <summary>
        /// 目前尚未实现。
        /// </summary>
        public int Skip(uint celt)
        {
            return unchecked((int)0x80004001);
        }
    }
}
