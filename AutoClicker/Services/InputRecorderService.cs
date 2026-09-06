using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AutoClicker.Models;

namespace AutoClicker.Services
{
    public static class InputRecorderService
    {
        #region Win32 Constants & Structs

        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_MBUTTONDOWN = 0x0207;

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        #endregion

        #region Win32 Imports

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        #region Fields & Events

        private static IntPtr _keyboardHookId = IntPtr.Zero;
        private static IntPtr _mouseHookId = IntPtr.Zero;

        private static HookProc _keyboardProc;
        private static HookProc _mouseProc;

        private static Stopwatch _stopwatch;
        private static long _lastActionTimeMs;

        public static bool IsRecording { get; private set; }

        public static event Action<ActionItem> ActionRecorded;

        #endregion

        #region Public Methods

        public static void StartRecording()
        {
            if (IsRecording) return;

            _keyboardProc = KeyboardHookCallback;
            _mouseProc = MouseHookCallback;

            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                IntPtr moduleHandle = GetModuleHandle(curModule?.ModuleName);
                _keyboardHookId = SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardProc, moduleHandle, 0);
                _mouseHookId = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, moduleHandle, 0);
            }

            _stopwatch = Stopwatch.StartNew();
            _lastActionTimeMs = 0;
            IsRecording = true;
        }

        public static void StopRecording()
        {
            if (!IsRecording) return;

            if (_keyboardHookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_keyboardHookId);
                _keyboardHookId = IntPtr.Zero;
            }

            if (_mouseHookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_mouseHookId);
                _mouseHookId = IntPtr.Zero;
            }

            _stopwatch?.Stop();
            IsRecording = false;
        }

        #endregion

        #region Hook Callbacks

        private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                var kbd = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                
                // Exclude hotkeys used for recording control (e.g. F7) if needed
                string keyName = ConvertVkCodeToKeyName((ushort)kbd.vkCode);

                if (!string.IsNullOrEmpty(keyName))
                {
                    long currentMs = _stopwatch.ElapsedMilliseconds;
                    int delay = (int)Math.Max(10, currentMs - _lastActionTimeMs);
                    _lastActionTimeMs = currentMs;

                    var action = new ActionItem
                    {
                        ActionType = ActionType.Keyboard,
                        KeyOrButton = keyName,
                        Delay = delay
                    };

                    ActionRecorded?.Invoke(action);
                }
            }

            return CallNextHookEx(_keyboardHookId, nCode, wParam, lParam);
        }

        private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MouseButtonType? btnType = null;
                int msg = wParam.ToInt32();

                if (msg == WM_LBUTTONDOWN) btnType = MouseButtonType.Left;
                else if (msg == WM_RBUTTONDOWN) btnType = MouseButtonType.Right;
                else if (msg == WM_MBUTTONDOWN) btnType = MouseButtonType.Middle;

                if (btnType.HasValue)
                {
                    var mouse = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

                    long currentMs = _stopwatch.ElapsedMilliseconds;
                    int delay = (int)Math.Max(10, currentMs - _lastActionTimeMs);
                    _lastActionTimeMs = currentMs;

                    var action = new ActionItem
                    {
                        ActionType = ActionType.MouseClick,
                        MouseButton = btnType.Value,
                        X = mouse.pt.x,
                        Y = mouse.pt.y,
                        Delay = delay
                    };

                    ActionRecorded?.Invoke(action);
                }
            }

            return CallNextHookEx(_mouseHookId, nCode, wParam, lParam);
        }

        public static string ConvertVkCodeToKeyName(ushort vkCode)
        {
            switch (vkCode)
            {
                case 0x0D: return "ENTER";
                case 0x20: return "SPACE";
                case 0x09: return "TAB";
                case 0x1B: return "ESCAPE";
                case 0x10: return "SHIFT";
                case 0x11: return "CTRL";
                case 0x12: return "ALT";
                case 0x25: return "LEFT";
                case 0x26: return "UP";
                case 0x27: return "RIGHT";
                case 0x28: return "DOWN";
                case 0x2E: return "DELETE";
                case 0x2D: return "INSERT";
                case 0x24: return "HOME";
                case 0x23: return "END";
                case 0x21: return "PAGEUP";
                case 0x22: return "PAGEDOWN";
                case 0x60: return "NUMPAD0";
                case 0x61: return "NUMPAD1";
                case 0x62: return "NUMPAD2";
                case 0x63: return "NUMPAD3";
                case 0x64: return "NUMPAD4";
                case 0x65: return "NUMPAD5";
                case 0x66: return "NUMPAD6";
                case 0x67: return "NUMPAD7";
                case 0x68: return "NUMPAD8";
                case 0x69: return "NUMPAD9";
                case 0x6A: return "*";
                case 0x6B: return "+";
                case 0x6C: return ",";
                case 0x6D: return "-";
                case 0x6E: return ".";
                case 0x6F: return "/";
                case 0x70: return "F1";
                case 0x71: return "F2";
                case 0x72: return "F3";
                case 0x73: return "F4";
                case 0x74: return "F5";
                case 0x75: return "F6";
                case 0x76: return "F7";
                case 0x77: return "F8";
                case 0x78: return "F9";
                case 0x79: return "F10";
                case 0x7A: return "F11";
                case 0x7B: return "F12";
                case 0x90: return "NUMLOCK";
                default:
                    if (vkCode >= 0x41 && vkCode <= 0x5A)
                        return ((char)vkCode).ToString();
                    if (vkCode >= 0x30 && vkCode <= 0x39)
                        return ((char)vkCode).ToString();
                    return ((char)vkCode).ToString();
            }
        }

        #endregion
    }
}
