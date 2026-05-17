using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace JANOARG.Chartmaker.Utils.NativeAPI.Internal.NativeWindow.Windows
{
    internal static class User32
    {
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();
        
        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(uint dwThreadId, EnumWinProc lpEnumFunc, IntPtr lParam);
        delegate bool EnumWinProc(IntPtr hWnd, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    
        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll")]
        static extern IntPtr FindWindowA(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetWindowText(nint hWnd, string lpString);
        
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        static extern IntPtr SetWindowLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(nint hWnd, out WinRect lpRect);
        [DllImport("user32.dll")]
        public static extern bool MoveWindow(nint hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(nint hWnd, WinWindowState nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(nint hWnd);
        [DllImport("user32.dll")]
        public static extern bool IsZoomed(nint hWnd);

        [DllImport("user32.dll")]   
        public static extern nint SetCursor(nint hCursor);
        [DllImport("user32.dll")]
        public static extern nint LoadCursor(nint hInstance, nint lpCursorName);
    }

    internal static class Win32Convert
    {
        public static CursorStyle FromPlatformCursor(nint style)
        {
            return style switch
            {
                WinCursorStyle.None => CursorStyle.None,

                WinCursorStyle.Arrow => CursorStyle.Arrow,

                WinCursorStyle.Text => CursorStyle.Text,
                WinCursorStyle.Crosshair => CursorStyle.Crosshair,

                WinCursorStyle.Busy => CursorStyle.Busy,
                WinCursorStyle.BackgroundBusy => CursorStyle.BackgroundBusy,

                WinCursorStyle.HandPointing => CursorStyle.HandPointing,

                WinCursorStyle.ResizeHorizontal => CursorStyle.ResizeHorizontal,
                WinCursorStyle.ResizeVertical => CursorStyle.ResizeVertical,
                WinCursorStyle.ResizeDiagonalTopLeft => CursorStyle.ResizeDiagonalTopLeft,
                WinCursorStyle.ResizeDiagonalTopRight => CursorStyle.ResizeDiagonalTopRight,

                _ => CursorStyle.Arrow,
            };
        }

        public static nint ToPlatformCursor(CursorStyle style)
        {
            return style switch
            {
                CursorStyle.None => WinCursorStyle.None,

                CursorStyle.Arrow => WinCursorStyle.Arrow,

                CursorStyle.Text => WinCursorStyle.Text,
                CursorStyle.Crosshair => WinCursorStyle.Crosshair,
                CursorStyle.Blocked => WinCursorStyle.Blocked,

                CursorStyle.Busy => WinCursorStyle.Busy,
                CursorStyle.BackgroundBusy => WinCursorStyle.BackgroundBusy,

                CursorStyle.HandPointing => WinCursorStyle.HandPointing,

                CursorStyle.ResizeHorizontal => WinCursorStyle.ResizeHorizontal,
                CursorStyle.ResizeVertical => WinCursorStyle.ResizeVertical,
                CursorStyle.ResizeDiagonalTopLeft => WinCursorStyle.ResizeDiagonalTopLeft,
                CursorStyle.ResizeDiagonalTopRight => WinCursorStyle.ResizeDiagonalTopRight,

                _ => WinCursorStyle.Unknown,
            };
        }

        public static nint ToPlatformCursorBestEffort(CursorStyle style)
        {
            return style switch
            {
                CursorStyle.TextVertical => WinCursorStyle.Text,
                CursorStyle.HandGrabReady => WinCursorStyle.Arrow,
                CursorStyle.HandGrabbing => WinCursorStyle.Arrow,

                CursorStyle.ResizeLeft => WinCursorStyle.ResizeHorizontal,
                CursorStyle.ResizeRight => WinCursorStyle.ResizeHorizontal,
                CursorStyle.ResizeTop => WinCursorStyle.ResizeVertical,
                CursorStyle.ResizeBottom => WinCursorStyle.ResizeVertical,
                CursorStyle.ResizeTopLeft => WinCursorStyle.ResizeDiagonalTopLeft,
                CursorStyle.ResizeTopRight => WinCursorStyle.ResizeDiagonalTopRight,
                CursorStyle.ResizeBottomLeft => WinCursorStyle.ResizeDiagonalTopRight,
                CursorStyle.ResizeBottomRight => WinCursorStyle.ResizeDiagonalTopLeft,

                _ => ToPlatformCursor(style),
            };
        }

        public static WindowState FromPlatformWindowState(WinWindowState state)
        {
            return state switch
            {
                WinWindowState.Minimized => WindowState.Minimized,
                WinWindowState.Maximized => WindowState.Maximized,
                WinWindowState.Floating => WindowState.Floating,

                _ => WindowState.Floating,
            };
        }

        public static WinWindowState ToPlatformWindowState(WindowState state)
        {
            return state switch
            {
                WindowState.Minimized => WinWindowState.Minimized,
                WindowState.Maximized => WinWindowState.Maximized,
                WindowState.Floating => WinWindowState.Floating,

                _ => WinWindowState.Floating,
            };
        }
    }

    internal struct WinRect { public int left, top, right, bottom; }

    internal enum WinWindowState : int
    {
        Minimized = 6,
        Maximized = 3,
        Floating = 9,
    }

    internal static class WinCursorStyle
    {
        public const nint Unknown = -1;
        public const nint None = 0;

        public const nint Arrow = 32512; 

        public const nint Text = 32513;

        public const nint Busy = 32514;
        public const nint BackgroundBusy = 32650;
        public const nint Blocked = 32648;

        public const nint Crosshair = 32515;

        public const nint HandPointing = 32649;

        public const nint ResizeHorizontal = 32644;
        public const nint ResizeVertical = 32645;
        public const nint ResizeDiagonalTopLeft = 32642;
        public const nint ResizeDiagonalTopRight = 32643;
    }
}
