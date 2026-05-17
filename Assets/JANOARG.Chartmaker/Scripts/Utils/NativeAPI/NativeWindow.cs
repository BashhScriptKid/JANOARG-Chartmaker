using System;
using JANOARG.Chartmaker.Utils.NativeAPI.Internal.NativeWindow;
using UnityEditor.PackageManager.UI;

namespace JANOARG.Chartmaker.Utils.NativeAPI
{
    public class NativeWindow
    {
        private static NativeWindowController Controller;
        private static NativeWindow CachedMainWindow;

        private nint WindowHandle;

        private NativeWindow(nint handle)
        {
            WindowHandle = handle;
        }

        public static NativeWindow MainWindow
        {
            get
            {
                return CachedMainWindow ??= new NativeWindow(Controller.Provider.GetMainWindowHandle());
            }
        }
    }

    public enum WindowState
    {
        Minimized,
        Floating,
        Maximized,
    }

    public enum WindowStyle
    {
        Native,
        Custom,
    }

    public enum CursorStyle
    {
        None = -1,

        Arrow,
        Crosshair,
        Text,
        TextVertical,

        Busy,
        BackgroundBusy,
        Blocked,

        HandPointing,
        HandGrabReady,
        HandGrabbing,

        ResizeLeft,
        ResizeRight,
        ResizeTop,
        ResizeBottom,
        ResizeTopLeft,
        ResizeTopRight,
        ResizeBottomLeft,
        ResizeBottomRight,
        ResizeVertical,
        ResizeHorizontal,
        ResizeDiagonalTopLeft,
        ResizeDiagonalTopRight,
    }

    public enum CursorMode
    {
        PreferCustom,
        PreferNative,
        PreferNativeBestEffort,
    }
}