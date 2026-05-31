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
        private nint display;
        private nint currentWindow;
        private Dictionary<CursorStyle, nint> cursorCache = new ();
        private Dictionary<nint, WindowStyle> windowStyles = new();

        public nint GetMainWindowHandle()
        {
            var h = Process.GetCurrentProcess().MainWindowHandle;
            return h;
        }

        bool EnsureDisplay()
        {
            if (display == 0)
            {
                display = LibX11.XOpenDisplay(0);
            }
            if (currentWindow == 0) 
            {
                currentWindow = Process.GetCurrentProcess().MainWindowHandle;
            }
            return display != 0 && currentWindow != 0;
        }

        bool CanUseWindow(nint windowHandle)
        {
            return EnsureDisplay()
                && windowHandle != 0
                && LibX11.XGetWindowAttributes(display, windowHandle, out _) != 0;
        }

        nint Atom(string name, bool onlyIfExists = false)
        {
            if (!EnsureDisplay()) return 0;
            return LibX11.XInternAtom(display, name, onlyIfExists);
        }

        public bool HookWindow(nint windowHandle) 
        { 
            return EnsureDisplay(); 
        }

        public bool UnhookWindow(nint windowHandle) 
        { 
            return true; 
        }

        public string GetWindowName(nint windowHandle)
        {
            if (!CanUseWindow(windowHandle)) return string.Empty;
            nint namePtr = 0;
            if (LibX11.XFetchName(display, windowHandle, ref namePtr) == 0 || namePtr == 0) return string.Empty;
            string name = Marshal.PtrToStringAnsi(namePtr) ?? string.Empty;
            LibX11.XFree(namePtr);
            return name;
        }

        public bool SetWindowName(nint windowHandle, string name)
        {
            if (!CanUseWindow(windowHandle)) return false;
            LibX11.XStoreName(display, windowHandle, name);
            LibX11.XFlush(display);
            return true;
        }

        public bool GetWindowActive(nint windowHandle)
        {
            return Application.isFocused;
        }

        public WindowState GetWindowState(nint windowHandle)
        {
            if (HasWindowState(windowHandle, "_NET_WM_STATE_MAXIMIZED_VERT")
                && HasWindowState(windowHandle, "_NET_WM_STATE_MAXIMIZED_HORZ"))
                return WindowState.Maximized;
            return WindowState.Floating;
        }

        public bool SetWindowState(nint windowHandle, WindowState state)
        {
            if (!CanUseWindow(windowHandle)) return false;
            switch (state)
            {
                case WindowState.Minimized: 
                    return LibX11.XIconifyWindow(display, windowHandle, LibX11.XDefaultScreen(display)) != 0;
                case WindowState.Maximized: 
                    return SendNetWmState(windowHandle, 1, "_NET_WM_STATE_MAXIMIZED_VERT", "_NET_WM_STATE_MAXIMIZED_HORZ");
                case WindowState.Floating: 
                    return SendNetWmState(windowHandle, 0, "_NET_WM_STATE_MAXIMIZED_VERT", "_NET_WM_STATE_MAXIMIZED_HORZ");
                default:
                    return false;
            }
        }

        public WindowStyle GetWindowStyle(nint windowHandle)
        {
            return windowStyles.GetValueOrDefault(windowHandle, WindowStyle.Native);
        }
        
        public bool SetWindowStyle(nint windowHandle, WindowStyle style) 
        { 
            if (!CanUseWindow(windowHandle)) return false;

            nint motifHints = Atom("_MOTIF_WM_HINTS");
            if (motifHints == 0) return false;

            var hints = new MotifWmHints
            {
                flags = 2,
                decorations = style == WindowStyle.Native ? 1u : 0u,
            };
            LibX11.XChangeProperty(display, windowHandle, motifHints, motifHints, 32, 0, ref hints, 5);
            LibX11.XFlush(display);
            windowStyles[windowHandle] = style;
            return true;
        }

        public RectInt GetWindowRect(nint windowHandle)
        {
            if (!CanUseWindow(windowHandle)) return new(0, 0, 0, 0);

            if (LibX11.XGetWindowAttributes(display, windowHandle, out var attrs) == 0)
                return new(0, 0, 0, 0);

            nint child;
            int rootX = attrs.x;
            int rootY = attrs.y;
            nint root = attrs.root != 0 ? attrs.root : LibX11.XDefaultRootWindow(display);
            LibX11.XTranslateCoordinates(display, windowHandle, root, 0, 0, out rootX, out rootY, out child);
            return new RectInt(rootX, rootY, attrs.width, attrs.height);
        }

        public bool SetWindowRect(nint windowHandle, RectInt rect)
        {
            bool moved = MoveWindow(windowHandle, new Vector2Int(rect.x, rect.y));
            bool resized = ResizeWindow(windowHandle, new Vector2Int(rect.width, rect.height));
            return moved && resized;
        }

        public bool MoveWindow(nint windowHandle, Vector2Int position)
        {
            if (!CanUseWindow(windowHandle)) return false;
            int result = LibX11.XMoveWindow(display, windowHandle, position.x, position.y);
            LibX11.XFlush(display);
            return result != 0;
        }

        public bool ResizeWindow(nint windowHandle, Vector2Int size)
        {
            if (!CanUseWindow(windowHandle)) return false;
            int result = LibX11.XResizeWindow(display, windowHandle, (uint)System.Math.Max(size.x, 1), (uint)System.Math.Max(size.y, 1));
            LibX11.XFlush(display);
            return result != 0;
        }

        public Vector2Int GetWindowMinSize(nint windowHandle)
        {
            if (!CanUseWindow(windowHandle)) return new Vector2Int(0, 0);
            var hintsPtr = LibX11.XAllocSizeHints();
            long supplied = 0;
            if (hintsPtr != 0 && LibX11.XGetWMNormalHints(display, windowHandle, hintsPtr, ref supplied) != 0)
            {
                var hints = Marshal.PtrToStructure<XSizeHints>(hintsPtr);
                if ((hints.flags & (long)XSizeHintsFlags.PMinSize) != 0)
                {
                    return new Vector2Int(hints.min_width, hints.min_height);
                }
            }
            if (hintsPtr != 0) LibX11.XFree(hintsPtr);
            return new Vector2Int(0, 0);
        }

        public bool SetWindowMinSize(nint windowHandle, Vector2Int minSize)
        {
            if (!CanUseWindow(windowHandle)) return false;
            var hintsPtr = LibX11.XAllocSizeHints();
            if (hintsPtr == 0) return false;
            long supplied = 0;
            LibX11.XGetWMNormalHints(display, windowHandle, hintsPtr, ref supplied);
            var hints = new XSizeHints
            {
                flags = supplied | (long)XSizeHintsFlags.PMinSize,
                min_width = minSize.x,
                min_height = minSize.y
            };
            Marshal.StructureToPtr(hints, hintsPtr, false);
            LibX11.XSetWMNormalHints(display, windowHandle, hintsPtr);
            LibX11.XFree(hintsPtr);
            LibX11.XFlush(display);
            return true;
        }

        public Vector2Int GetWindowMaxSize(nint windowHandle)
        {
            if (!CanUseWindow(windowHandle)) return new Vector2Int(0, 0);
            var hintsPtr = LibX11.XAllocSizeHints();
            long supplied = 0;
            if (hintsPtr != 0 && LibX11.XGetWMNormalHints(display, windowHandle, hintsPtr, ref supplied) != 0)
            {
                var hints = Marshal.PtrToStructure<XSizeHints>(hintsPtr);
                if ((hints.flags & (long)XSizeHintsFlags.PMaxSize) != 0)
                {
                    return new Vector2Int(hints.max_width, hints.max_height);
                }
            }
            if (hintsPtr != 0) LibX11.XFree(hintsPtr);
            return new Vector2Int(0, 0);
        }

        public bool SetWindowMaxSize(nint windowHandle, Vector2Int maxSize)
        {
            if (!CanUseWindow(windowHandle)) return false;
            var hintsPtr = LibX11.XAllocSizeHints();
            if (hintsPtr == 0) return false;
            long supplied = 0;
            LibX11.XGetWMNormalHints(display, windowHandle, hintsPtr, ref supplied);
            var hints = new XSizeHints
            {
                flags = supplied | (long)XSizeHintsFlags.PMaxSize,
                max_width = maxSize.x,
                max_height = maxSize.y
            };
            Marshal.StructureToPtr(hints, hintsPtr, false);
            LibX11.XSetWMNormalHints(display, windowHandle, hintsPtr);
            LibX11.XFree(hintsPtr);
            LibX11.XFlush(display);
            return true;
        }

        public bool SetWindowCursor(nint windowHandle, CursorStyle cursor, bool bestEffort)
        {
            if (!CanUseWindow(windowHandle)) return false;
            if (cursor == CursorStyle.None)
            {
                LibX11.XUndefineCursor(display, windowHandle);
                LibX11.XFlush(display);
                return true;
            }

            if (cursorCache.ContainsKey(cursor))
            {
                if (LibX11.XDefineCursor(display, windowHandle, cursorCache[cursor]) == 0)
                {
                    LibX11.XFlush(display);
                    return true;
                }
                else
                {
                    cursorCache.Remove(cursor);
                }
            }

            string[] cursorNames = X11Convert.CursorNames.GetValueOrDefault(cursor, new string[] {});
            foreach (string name in cursorNames)
            {
                nint cursorPtr = LibXCursor.XcursorLibraryLoadCursor(display, name);
                if (cursorPtr != 0)
                {
                    cursorCache[cursor] = cursorPtr;
                    bool result = LibX11.XDefineCursor(display, windowHandle, cursorCache[cursor]) == 0;
                    LibX11.XFlush(display);
                    return result;
                }
            }

            return false;
        }

        public bool SetWindowHitTestZone(nint windowHandle, int zone)
        {
            return false;
        }

        bool SendNetWmState(nint windowHandle, nint action, string firstState, string secondState)
        {
            nint netWmState = Atom("_NET_WM_STATE");
            nint state1 = Atom(firstState);
            nint state2 = Atom(secondState);
            if (netWmState == 0 || state1 == 0) return false;

            var ev = new XEvent
            {
                type = 33,
                clientMessage = new XClientMessageEvent
                {
                    type = 33,
                    display = display,
                    window = windowHandle,
                    message_type = netWmState,
                    format = 32,
                    data0 = action,
                    data1 = state1,
                    data2 = state2,
                    data3 = 1,
                }
            };
            int result = LibX11.XSendEvent(display, LibX11.XDefaultRootWindow(display), false, (nint)(0x00080000 | 0x00100000), ref ev);
            LibX11.XFlush(display);
            return result != 0;
        }

        bool HasWindowState(nint windowHandle, string stateName)
        {
            if (!CanUseWindow(windowHandle)) return false;
            nint netWmState = Atom("_NET_WM_STATE", true);
            nint targetState = Atom(stateName, true);
            nint atomType = Atom("ATOM", true);
            if (netWmState == 0 || targetState == 0) return false;

            int result = LibX11.XGetWindowProperty(display, windowHandle, netWmState, 0, 1024, false, atomType, out _, out int format, out nint itemCount, out _, out nint prop);
            if (result != 0 || prop == 0) return false;

            try
            {
                if (format != 32) return false;
                int count = (int)itemCount;
                for (int i = 0; i < count; i++)
                {
                    nint atom = Marshal.ReadIntPtr(prop, i * IntPtr.Size);
                    if (atom == targetState) return true;
                }
            }
            finally
            {
                LibX11.XFree(prop);
            }
            return false;
        }
    }
}
