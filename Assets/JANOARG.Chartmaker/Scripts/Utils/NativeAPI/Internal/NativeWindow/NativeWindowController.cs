
using Unity.VisualScripting;
using UnityEngine;

namespace JANOARG.Chartmaker.Utils.NativeAPI.Internal.NativeWindow
{
    internal class NativeWindowController : NativeAPIController<NativeWindowController>
    {
        public INativeWindowProvider Provider;

        public override bool IsSupported
        {
            get
            {
                EnsureInitialized();
                return Provider != null;
            }
        }

        protected override bool Initialize()
        {
            if (Provider != null) return true;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Provider = new Windows.WindowsNativeWindowProvider();
#elif UNITY_STANDALONE_LINUX && !UNITY_EDITOR
            Provider = new LinuxX11.LinuxX11NativeWindowProvider();
#endif

            return Provider != null;
        }

        public nint GetMainWindowHandle()
        {
            if (IsSupported) return Provider.GetMainWindowHandle();
            else throw new System.Exception("API is not supported on this platform.");
        }

        public string GetWindowName(nint windowHandle)
        {
            if (IsSupported) return Provider.GetWindowName(windowHandle);
            else throw new System.Exception("API is not supported on this platform.");
        }

        public void SetWindowName(nint windowHandle, string name)
        {
            if (IsSupported) Provider.SetWindowName(windowHandle, name);
            else throw new System.Exception("API is not supported on this platform.");
        }

        public RectInt GetWindowRect(nint windowHandle)
        {
            if (IsSupported) return Provider.GetWindowRect(windowHandle);
            else throw new System.Exception("API is not supported on this platform.");
        }

        public WindowState GetWindowState(nint windowHandle)
        {
            if (IsSupported) return Provider.GetWindowState(windowHandle);
            else throw new System.Exception("API is not supported on this platform.");
        }

        public WindowStyle GetWindowStyle(nint windowHandle)
        {
            if (IsSupported) return Provider.GetWindowStyle(windowHandle);
            else throw new System.Exception("API is not supported on this platform.");
        }

        public void SetWindowStyle(nint windowHandle, WindowStyle style)
        {
            if (IsSupported) Provider.SetWindowStyle(windowHandle, style);
            else throw new System.Exception("API is not supported on this platform.");
        }

        public void HookWindow(nint windowHandle)
        {
            if (IsSupported) Provider.HookWindow(windowHandle);
            else throw new System.Exception("API is not supported on this platform.");
        }

        public void MoveWindow(nint windowHandle, Vector2Int position)
        {
            if (IsSupported) Provider.MoveWindow(windowHandle, position);
            else throw new System.Exception("API is not supported on this platform.");
        }

        public CursorStyle PeekWindowCursor(nint windowHandle)
        {
            if (IsSupported) return Provider.PeekWindowCursor(windowHandle);
            else throw new System.Exception("API is not supported on this platform.");
        }

        public CursorStyle PopWindowCursor(nint windowHandle)
        {
            if (IsSupported) return Provider.PopWindowCursor(windowHandle);
            else throw new System.Exception("API is not supported on this platform.");
        }

        public void PushWindowCursor(nint windowHandle, CursorStyle cursor)
        {
            if (IsSupported) Provider.PushWindowCursor(windowHandle, cursor);
            else throw new System.Exception("API is not supported on this platform.");
        }

        public void ResizeWindow(nint windowHandle, Vector2Int size)
        {
            if (IsSupported) Provider.ResizeWindow(windowHandle, size);
            else throw new System.Exception("API is not supported on this platform.");
        }

        public void SetWindowRect(nint windowHandle, RectInt rect)
        {
            if (IsSupported) Provider.SetWindowRect(windowHandle, rect);
            else throw new System.Exception("API is not supported on this platform.");
        }

        public void SetWindowState(nint windowHandle, WindowState state)
        {
            if (IsSupported) Provider.SetWindowState(windowHandle, state);
            else throw new System.Exception("API is not supported on this platform.");
        }

        public void UnhookWindow(nint windowHandle)
        {
            if (IsSupported) Provider.UnhookWindow(windowHandle);
            else throw new System.Exception("API is not supported on this platform.");
        }
        
    }
}