﻿using Microsoft.Win32;
using System;
using System.Diagnostics.Tracing;
using ThemeSwitch.Services.Root;

namespace ThemeSwitch.Helpers.Root
{
    /// <summary>
    /// 注册表读取辅助类
    /// </summary>
    public static class RegistryHelper
    {
        /// <summary>
        /// 读取注册表指定项的内容
        /// </summary>
        public static T ReadRegistryKey<T>(RegistryKey rootRegistryKey, string rootKey, string key)
        {
            T value = default;
            try
            {
                if ((Equals(rootRegistryKey, Registry.ClassesRoot) || Equals(rootRegistryKey, Registry.CurrentConfig) || Equals(rootRegistryKey, Registry.CurrentUser) || Equals(rootRegistryKey, Registry.LocalMachine) || Equals(rootRegistryKey, Registry.PerformanceData) || Equals(rootRegistryKey, Registry.Users)) && rootRegistryKey.OpenSubKey(rootKey, false) is RegistryKey registryKey && registryKey.GetValue(key) is object getValue)
                {
                    // 读取布尔值
                    if (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?))
                    {
                        value = (T)(object)Convert.ToBoolean(getValue);
                    }
                    else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
                    {
                        value = (T)(object)Convert.ToInt32(getValue);
                    }
                    else if (typeof(T) == typeof(uint) || typeof(T) == typeof(uint?))
                    {
                        value = (T)(object)Convert.ToUInt32(getValue);
                    }
                    else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
                    {
                        value = (T)(object)Convert.ToInt64(getValue);
                    }
                    else if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?))
                    {
                        value = (T)(object)Convert.ToUInt64(getValue);
                    }
                    else if (typeof(T) == typeof(string))
                    {
                        value = (T)(object)Convert.ToString(getValue);
                    }
                    else
                    {
                        value = typeof(T) == typeof(byte[]) || typeof(T) == typeof(string[]) ? (T)getValue : (T)getValue;
                    }

                    registryKey.Close();
                    registryKey.Dispose();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, string.Format("Read Registry rootKey {0} and key {1} value failed", rootKey, key), e);
            }
            return value;
        }

        /// <summary>
        /// 保存注册表指定项的内容
        /// </summary>
        public static bool SaveRegistryKey<T>(RegistryKey rootRegistryKey, string rootKey, string key, T value, bool isExpandString = false)
        {
            try
            {
                if ((Equals(rootRegistryKey, Registry.ClassesRoot) || Equals(rootRegistryKey, Registry.CurrentConfig) || Equals(rootRegistryKey, Registry.CurrentUser) || Equals(rootRegistryKey, Registry.LocalMachine) || Equals(rootRegistryKey, Registry.PerformanceData) || Equals(rootRegistryKey, Registry.Users)) && rootRegistryKey.CreateSubKey(rootKey, true) is RegistryKey registryKey)
                {
                    // 存储 32 位整数类型或者布尔值
                    if (typeof(T) == typeof(bool) || typeof(T) == typeof(int) || typeof(T) == typeof(uint))
                    {
                        registryKey.SetValue(key, Convert.ToInt32(value), RegistryValueKind.DWord);
                    }
                    // 存储 64 位整数类型
                    else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
                    {
                        registryKey.SetValue(key, Convert.ToInt32(value), RegistryValueKind.QWord);
                    }
                    // 存储字符串类型
                    else if (typeof(T) == typeof(string))
                    {
                        registryKey.SetValue(key, value, isExpandString ? RegistryValueKind.ExpandString : RegistryValueKind.String);
                    }
                    // 存储二进制数据
                    else if (typeof(T) == typeof(byte[]))
                    {
                        registryKey.SetValue(key, value, RegistryValueKind.Binary);
                    }
                    // 存储字符串数组
                    else if (typeof(T) == typeof(string[]))
                    {
                        registryKey.SetValue(key, value, RegistryValueKind.MultiString);
                    }
                    // 其他类型
                    else
                    {
                        registryKey.SetValue(key, value);
                    }

                    registryKey.Close();
                    registryKey.Dispose();
                }
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, string.Format("Save Registry rootKey {0} and key {1} value failed", rootKey, key), e);
                return false;
            }
        }

        /// <summary>
        /// 移除注册表指定项
        /// </summary>

        public static bool RemoveRegistryKey(RegistryKey rootRegistryKey, string rootKey, string key = null)
        {
            try
            {
                if ((Equals(rootRegistryKey, Registry.ClassesRoot) || Equals(rootRegistryKey, Registry.CurrentConfig) || Equals(rootRegistryKey, Registry.CurrentUser) || Equals(rootRegistryKey, Registry.LocalMachine) || Equals(rootRegistryKey, Registry.PerformanceData) || Equals(rootRegistryKey, Registry.Users)) && rootRegistryKey.CreateSubKey(rootKey, true) is RegistryKey registryKey)
                {
                    // 删除整项
                    if (key is null)
                    {
                        rootRegistryKey.DeleteSubKeyTree(rootKey, false);
                    }
                    // 删除项下的某一个键值
                    else
                    {
                        registryKey.DeleteValue(key);
                    }

                    registryKey.Close();
                    registryKey.Dispose();
                }
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, string.Format("Remove registry rootkey {0} failed", rootKey), e);
                return false;
            }
        }
    }
}
