using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace JANOARG.Chartmaker.Utils.NativeAPI.Internal.NativeWindow.LinuxX11
{
    internal class LinuxX11NativeWindowProvider : INativeWindowProvider
    {
        readonly Dictionary<nint, Stack<CursorStyle>> cursorStacks = new();

        [DllImport("libX11.so.6")]
        static extern IntPtr XOpenDisplay(IntPtr display);

        [DllImport("libX11.so.6")]
        static extern IntPtr XDefaultRootWindow(IntPtr display);

        [DllImport("libX11.so.6")]
        static extern int XMapWindow(IntPtr display, IntPtr window);

        [DllImport("libX11.so.6")]
        static extern int XMoveWindow(IntPtr display, IntPtr window, int x, int y);

        [DllImport("libX11.so.6")]
        static extern int XResizeWindow(IntPtr display, IntPtr window, uint width, uint height);

        [DllImport("libX11.so.6")]
        static extern int XFetchName(IntPtr display, IntPtr window, ref IntPtr window_name);

        [DllImport("libX11.so.6")]
        static extern int XStoreName(IntPtr display, IntPtr window, string window_name);

        [DllImport("libX11.so.6")]
        static extern int XDestroyWindow(IntPtr display, IntPtr window);

        [DllImport("libX11.so.6")]
        static extern int XCloseDisplay(IntPtr display);

        private IntPtr display;
        private IntPtr currentWindow;

        public nint GetMainWindowHandle()
        {
            var h = Process.GetCurrentProcess().MainWindowHandle;
            return (nint)h;
        }

        void EnsureDisplay()
        {
            if (display == IntPtr.Zero)
            {
                display = XOpenDisplay(IntPtr.Zero);
            }
            if (currentWindow == IntPtr.Zero) currentWindow = Process.GetCurrentProcess().MainWindowHandle;
        }

        public void HookWindow(nint windowHandle) { EnsureDisplay(); }
        public void UnhookWindow(nint windowHandle) { /* no-op */ }

        public string GetWindowName(nint windowHandle)
        {
            EnsureDisplay();
            IntPtr namePtr = IntPtr.Zero;
            XFetchName(display, ToIntPtr(windowHandle), ref namePtr);
            return Marshal.PtrToStringAnsi(namePtr) ?? string.Empty;
        }

        public void SetWindowName(nint windowHandle, string name)
        {
            EnsureDisplay();
            XStoreName(display, ToIntPtr(windowHandle), name);
        }

        public WindowState GetWindowState(nint windowHandle)
        {
            // X11 doesn't expose state easily here; return Floating as default
            return WindowState.Floating;
        }

        public void SetWindowState(nint windowHandle, WindowState state)
        {
            // Best-effort: map minimize/restore via unmap/map
            EnsureDisplay();
            var h = ToIntPtr(windowHandle);
            switch (state)
            {
                case WindowState.Minimized: XDestroyWindow(display, h); break;
                case WindowState.Maximized: /* no-op */ break;
                case WindowState.Floating: /* no-op */ break;
            }
        }

        public WindowStyle GetWindowStyle(nint windowHandle) => WindowStyle.Native;
        public void SetWindowStyle(nint windowHandle, WindowStyle style) { /* no-op */ }

        public RectInt GetWindowRect(nint windowHandle)
        {
            // Not implementing a full query here; return zero rect
            return new(0, 0, 0, 0);
        }

        public void SetWindowRect(nint windowHandle, RectInt rect)
        {
            MoveWindow(windowHandle, new Vector2Int(rect.x, rect.y));
            ResizeWindow(windowHandle, new Vector2Int(rect.width, rect.height));
        }

        public void MoveWindow(nint windowHandle, Vector2Int position)
        {
            EnsureDisplay();
            XMoveWindow(display, ToIntPtr(windowHandle), position.x, position.y);
        }

        public void ResizeWindow(nint windowHandle, Vector2Int size)
        {
            EnsureDisplay();
            XResizeWindow(display, ToIntPtr(windowHandle), (uint)size.x, (uint)size.y);
        }

        public CursorStyle PeekWindowCursor(nint windowHandle)
        {
            if (cursorStacks.TryGetValue(windowHandle, out var st) && st.Count > 0) return st.Peek();
            return CursorStyle.None;
        }

        public CursorStyle PopWindowCursor(nint windowHandle)
        {
            if (cursorStacks.TryGetValue(windowHandle, out var st) && st.Count > 0)
            {
                return st.Pop();
            }
            return CursorStyle.None;
        }

        public void PushWindowCursor(nint windowHandle, CursorStyle cursor)
        {
            if (!cursorStacks.TryGetValue(windowHandle, out var st))
            {
                st = new Stack<CursorStyle>();
                cursorStacks[windowHandle] = st;
            }
            st.Push(cursor);
        }

        private IntPtr ToIntPtr(nint v) => new IntPtr(v);
    }
}
