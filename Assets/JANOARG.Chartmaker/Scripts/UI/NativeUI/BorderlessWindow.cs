using System.Collections.Generic;
using JANOARG.Chartmaker.Behaviors.Runtime;
using JANOARG.Chartmaker.UI.Cursor;
using JANOARG.Chartmaker.Utils.NativeAPI;
using UnityEngine;

namespace JANOARG.Chartmaker.UI.NativeUI
{
    public class BorderlessWindow
    {
        public static bool IsFramed = true;
        public static bool IsMaximized;
        public static bool IsActive = true;
        public static WindowZone CurrentWindowZone = WindowZone.Client;
        public static List<RuntimeLogManager.LoggerEntry> Logger => RuntimeLogManager.Logger;

        static NativeWindow Window => NativeWindow.MainWindow;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void InitializeWindow()
        {
            Behaviors.Chartmaker.Chartmaker.PreferencesStorage = new("cm_prefs");
            Behaviors.Chartmaker.Chartmaker.Preferences.Load(Behaviors.Chartmaker.Chartmaker.PreferencesStorage);
            RuntimeLogManager.Initialize();

            if (!NativeWindow.IsApiAvailable) return;

            Window.Hook();
            Window.Title = "JANOARG Chartmaker";
            Window.MinSize = new Vector2Int(974, 607);
            Window.Style = Behaviors.Chartmaker.Chartmaker.Preferences.UseDefaultWindow
                ? WindowStyle.Native
                : WindowStyle.Custom;
            RefreshState();
        }

        public static Rect GetWindowRect()
        {
            RectInt rect = Window.Rect;
            return new Rect(rect.x, rect.y, rect.width, rect.height);
        }

        public static void RefreshState()
        {
            if (!NativeWindow.IsApiAvailable)
            {
                IsFramed = true;
                IsMaximized = false;
                IsActive = Application.isFocused;
                return;
            }

            IsFramed = Window.Style == WindowStyle.Native;
            IsMaximized = Window.State == WindowState.Maximized;
            IsActive = Window.IsActive;
            Window.SetHitTestZone((int)CurrentWindowZone);
        }

        public static void SetFramelessWindow()
        {
            if (NativeWindow.IsApiAvailable) Window.Style = WindowStyle.Custom;
            RefreshState();
        }

        public static void SetFramedWindow()
        {
            if (NativeWindow.IsApiAvailable) Window.Style = WindowStyle.Native;
            RefreshState();
        }

        public static void MinimizeWindow()
        {
            if (NativeWindow.IsApiAvailable) Window.State = WindowState.Minimized;
            RefreshState();
        }

        public static void MaximizeWindow()
        {
            if (NativeWindow.IsApiAvailable) Window.State = WindowState.Maximized;
            RefreshState();
        }

        public static void RestoreWindow()
        {
            if (NativeWindow.IsApiAvailable) Window.State = WindowState.Floating;
            RefreshState();
        }

        public static void MoveWindow(Vector2 pos, bool bRepaint = false)
        {
            if (NativeWindow.IsApiAvailable) Window.Position = Vector2Int.RoundToInt(pos);
            RefreshState();
        }

        public static void MoveWindowDelta(Vector2 posDelta, bool bRepaint = false)
        {
            if (NativeWindow.IsApiAvailable)
            {
                RectInt rect = Window.Rect;
                Window.Position = new Vector2Int(rect.x + Mathf.RoundToInt(posDelta.x), rect.y - Mathf.RoundToInt(posDelta.y));
            }
            RefreshState();
        }

        public static void ResizeWindow(int width, int height)
        {
            if (NativeWindow.IsApiAvailable) Window.Size = new Vector2Int(width, height);
            RefreshState();
        }

        public static void ResizeWindowDelta(int dWidth, int dHeight)
        {
            if (NativeWindow.IsApiAvailable)
            {
                RectInt rect = Window.Rect;
                Window.Size = new Vector2Int(rect.width + dWidth, rect.height + dHeight);
            }
            RefreshState();
        }

        public static void RenameWindow(string title)
        {
            if (NativeWindow.IsApiAvailable) Window.Title = title;
        }

        public static void UpdateCursor()
        {
            if (CursorManager.main) CursorManager.main.UpdateCursor();
        }
    }

    public enum CursorType
    {
        Arrow = 32512,
        Pointer = 32649,
        Busy = 32514,
        Text = 32513,
        SizeHorizontal = 32644,
        SizeVertical = 32645,
        Blocked = 32648,

        Grab = -1,
        Grabbing = -2,
        GrabbingBlocked = -3,
    }
}
