using PowerToolboxShellExtension.Extensions.Registry;
using PowerToolboxShellExtension.WindowsAPI.PInvoke.Advapi32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace PowerToolboxShellExtension.Helpers.Root
{
    /// <summary>
    /// 注册表读取辅助类
    /// </summary>
    internal static class RegistryHelper
    {
        /// <summary>
        /// 读取注册表指定项的内容
        /// </summary>
        internal static T ReadRegistryKey<T>(string rootKey, string key)
        {
            T value = default;
            try
            {
                if (Advapi32Library.RegOpenKeyEx(ReservedKeyHandles.HKEY_CURRENT_USER, rootKey, 0, RegistryAccessRights.KEY_READ, out nuint hKey) is 0)
                {
                    int length = 0;

                    // 读取数据大小
                    if (Advapi32Library.RegQueryValueEx(hKey, key, 0, out _, null, ref length) is 0)
                    {
                        byte[] data = new byte[length];

                        // 根据数据大小读取数据内容
                        if (Advapi32Library.RegQueryValueEx(hKey, key, 0, out RegistryValueKind kind, data, ref length) is 0)
                        {
                            // 字符串类型
                            if (kind is RegistryValueKind.REG_SZ)
                            {
                                string regValue = Encoding.Unicode.GetString(data, 0, length);
                                value = (T)(object)regValue[..^1];
                            }
                            // 字符串类型
                            else if (kind is RegistryValueKind.REG_EXPAND_SZ)
                            {
                                value = (T)(object)Environment.ExpandEnvironmentVariables(Encoding.Unicode.GetString(data, 0, length));
                            }
                            // 二进制数据类型
                            else if (kind is RegistryValueKind.REG_BINARY)
                            {
                                value = (T)(object)data;
                            }
                            // 32 位数字
                            else if (kind is RegistryValueKind.REG_DWORD)
                            {
                                int regValue = data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24);

                                value = Equals(typeof(T), typeof(bool)) || Equals(typeof(T), typeof(bool?)) ? (T)(object)Convert.ToBoolean(regValue) : (T)(object)regValue;
                            }
                            // 采用 big-endian 格式的 32 位数字
                            else if (kind is RegistryValueKind.REG_DWORD_BIG_ENDIAN)
                            {
                                int regValue = data[3] | (data[2] << 8) | (data[1] << 16) | (data[0] << 24);

                                value = Equals(typeof(T), typeof(bool)) || Equals(typeof(T), typeof(bool?)) ? (T)(object)Convert.ToBoolean(regValue) : (T)(object)value;
                            }
                            // 字符串序列
                            else if (kind is RegistryValueKind.REG_MULTI_SZ)
                            {
                                List<string> stringList = [];
                                string packed = Encoding.Unicode.GetString(data, 0, length);
                                int start = 0;
                                int end = packed.IndexOf('\0', start);
                                while (end > start)
                                {
                                    stringList.Add(packed[start..end]);
                                    start = end + 1;
                                    end = packed.IndexOf('\0', start);
                                }
                                value = (T)(object)stringList.ToArray();
                            }
                            // 64 位数字
                            else if (kind is RegistryValueKind.REG_QWORD)
                            {
                                uint numLow = (uint)(data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24));
                                uint numHigh = (uint)(data[4] | (data[5] << 8) | (data[6] << 16) | (data[7] << 24));
                                long keyValue = (long)(((ulong)numHigh << 32) | numLow);
                                value = (T)(object)keyValue;
                            }
                        }
                    }
                }
                Advapi32Library.RegCloseKey(hKey);
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            return value;
        }

        /// <summary>
        /// 枚举并递归当前注册表项的所有子项
        /// </summary>
        internal static RegistryEnumKeyItem EnumSubKey(string rootKey)
        {
            RegistryEnumKeyItem registryEnumKeyItem = new();

            try
            {
                if (Advapi32Library.RegOpenKeyEx(ReservedKeyHandles.HKEY_CURRENT_USER, rootKey, 0, RegistryAccessRights.KEY_READ, out nuint hKey) is 0)
                {
                    // 添加当前项信息
                    registryEnumKeyItem.RootKey = rootKey;

                    // 获取当前项的所有子项列表
                    List<string> subKeyList = [];
                    char[] name = new char[256];

                    int result;
                    int nameLength = name.Length;

                    while ((result = Advapi32Library.RegEnumKeyEx(hKey, subKeyList.Count, name, ref nameLength, null, null, null, null)) is not 259)
                    {
                        if (result is 0)
                        {
                            subKeyList.Add(new(name[..nameLength]));
                            nameLength = name.Length;
                        }
                        else if (result is 2) // ERROR_FILE_NOT_FOUND
                        {
                            continue;
                        }
                        else if (result is 5) // ERROR_ACCESS_DENIED
                        {
                            continue;
                        }
                    }

                    foreach (string subKey in subKeyList)
                    {
                        registryEnumKeyItem.SubRegistryKeyList.Add(EnumSubKey(Path.Combine(rootKey, subKey)));
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            return registryEnumKeyItem;
        }
    }
}
