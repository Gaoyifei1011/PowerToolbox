using Microsoft.Win32;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Services.Root;
using System;
using System.Diagnostics;

namespace PowerToolbox.Helpers.Root
{
    /// <summary>
    /// 用户账户控制辅助类
    /// </summary>
    public static class UACHelper
    {
        /// <summary>
        /// 获取用户账户控制通知模式
        /// </summary>
        public static UacLevel GetUacLevel()
        {
            try
            {
                int consentPromptBehaviorAdmin = RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin");
                int secureDesktop = RegistryHelper.ReadRegistryKey<int>(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop");

                if (consentPromptBehaviorAdmin is 0)
                {
                    return UacLevel.NeverNotify;
                }
                else if (consentPromptBehaviorAdmin is 5 && secureDesktop is 0)
                {
                    return UacLevel.NotifyWithoutDimming;
                }
                else if (consentPromptBehaviorAdmin is 5 && secureDesktop is 1)
                {
                    return UacLevel.Notify;
                }
                else if (consentPromptBehaviorAdmin is 2 && secureDesktop is 1)
                {
                    return UacLevel.AlwaysNotify;
                }
                else
                {
                    return UacLevel.AlwaysNotify;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UACHelper), nameof(GetUacLevel), 1, e);
            }

            return UacLevel.AlwaysNotify;
        }

        /// <summary>
        /// 设置用户账户控制通知模式
        /// </summary>
        public static void SetUacLevel(UacLevel uacLevel)
        {
            try
            {
                switch (uacLevel)
                {
                    case UacLevel.NeverNotify:
                        {
                            RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 0);
                            RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 0);
                            break;
                        }
                    case UacLevel.NotifyWithoutDimming:
                        {
                            RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 5);
                            RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 0);
                            break;
                        }
                    case UacLevel.Notify:
                        {
                            RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 5);
                            RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 1);
                            break;
                        }
                    case UacLevel.AlwaysNotify:
                        {
                            RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 2);
                            RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 1);
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UACHelper), nameof(SetUacLevel), 1, e);
            }
        }
    }
}
