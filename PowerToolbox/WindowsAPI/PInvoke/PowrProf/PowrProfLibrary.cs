using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace PowerToolbox.WindowsAPI.PInvoke.PowrProf
{
    /// <summary>
    /// PowrProf.dll 函数库
    /// </summary>
    public static class PowrProfLibrary
    {
        private const string PowrProf = "powrProf.dll";

        [DllImport(PowrProf, CharSet = CharSet.Unicode, EntryPoint = "SetSuspendState", PreserveSig = true, SetLastError = false)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);
    }
}
