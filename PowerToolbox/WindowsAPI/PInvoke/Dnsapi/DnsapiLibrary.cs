using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace PowerToolbox.WindowsAPI.PInvoke.Dnsapi
{
    /// <summary>
    /// Dnsapi.dll 函数库
    /// </summary>
    internal static class DnsapiLibrary
    {
        private const string Dnsapi = "dnsapi.dll";

        [DllImport(Dnsapi, CharSet = CharSet.Unicode, EntryPoint = "DnsFlushResolverCache", PreserveSig = true, SetLastError = false)]
        internal static extern bool DnsFlushResolverCache();
    }
}
