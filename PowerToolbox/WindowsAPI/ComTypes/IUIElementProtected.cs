using Microsoft.UI.Input;
using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    [ComImport, Guid("8F69B9E9-1F00-5834-9BF1-A9257BED39F0"), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface IUIElementProtected
    {
        InputCursor ProtectedCursor { get; set; }
    }
}
