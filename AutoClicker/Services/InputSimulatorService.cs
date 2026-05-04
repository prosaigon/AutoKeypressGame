using System;
using System.Runtime.InteropServices;

namespace AutoClicker.Services
{
    public static class InputSimulatorService
    {
        #region Constants and Structures
        
        [Flags]
        public enum MouseEventFlags : uint
        {
            MOUSEEVENTF_MOVE = 0x0001,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            MOUSEEVENTF_ABSOLUTE = 0x8000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public int type;
            public InputUnion U;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;
        public const int INPUT_HARDWARE = 2;

        public const uint KEYEVENTF_KEYUP = 0x0002;
        public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        #endregion

        #region DLL Imports

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        #endregion

        #region Virtual Keys

        public static class VirtualKeys
        {
            public const ushort VK_LBUTTON = 0x01;
            public const ushort VK_RBUTTON = 0x02;
            public const ushort VK_CANCEL = 0x03;
            public const ushort VK_MBUTTON = 0x04;
            public const ushort VK_BACK = 0x08;
            public const ushort VK_TAB = 0x09;
            public const ushort VK_CLEAR = 0x0C;
            public const ushort VK_RETURN = 0x0D;
            public const ushort VK_SHIFT = 0x10;
            public const ushort VK_CONTROL = 0x11;
            public const ushort VK_MENU = 0x12;
            public const ushort VK_PAUSE = 0x13;
            public const ushort VK_CAPITAL = 0x14;
            public const ushort VK_ESCAPE = 0x1B;
            public const ushort VK_SPACE = 0x20;
            public const ushort VK_PRIOR = 0x21;
            public const ushort VK_NEXT = 0x22;
            public const ushort VK_END = 0x23;
            public const ushort VK_HOME = 0x24;
            public const ushort VK_LEFT = 0x25;
            public const ushort VK_UP = 0x26;
            public const ushort VK_RIGHT = 0x27;
            public const ushort VK_DOWN = 0x28;
            public const ushort VK_SELECT = 0x29;
            public const ushort VK_PRINT = 0x2A;
            public const ushort VK_EXECUTE = 0x2B;
            public const ushort VK_SNAPSHOT = 0x2C;
            public const ushort VK_INSERT = 0x2D;
            public const ushort VK_DELETE = 0x2E;
            public const ushort VK_HELP = 0x2F;
            public const ushort VK_0 = 0x30;
            public const ushort VK_1 = 0x31;
            public const ushort VK_2 = 0x32;
            public const ushort VK_3 = 0x33;
            public const ushort VK_4 = 0x34;
            public const ushort VK_5 = 0x35;
            public const ushort VK_6 = 0x36;
            public const ushort VK_7 = 0x37;
            public const ushort VK_8 = 0x38;
            public const ushort VK_9 = 0x39;
            public const ushort VK_A = 0x41;
            public const ushort VK_B = 0x42;
            public const ushort VK_C = 0x43;
            public const ushort VK_D = 0x44;
            public const ushort VK_E = 0x45;
            public const ushort VK_F = 0x46;
            public const ushort VK_G = 0x47;
            public const ushort VK_H = 0x48;
            public const ushort VK_I = 0x49;
            public const ushort VK_J = 0x4A;
            public const ushort VK_K = 0x4B;
            public const ushort VK_L = 0x4C;
            public const ushort VK_M = 0x4D;
            public const ushort VK_N = 0x4E;
            public const ushort VK_O = 0x4F;
            public const ushort VK_P = 0x50;
            public const ushort VK_Q = 0x51;
            public const ushort VK_R = 0x52;
            public const ushort VK_S = 0x53;
            public const ushort VK_T = 0x54;
            public const ushort VK_U = 0x55;
            public const ushort VK_V = 0x56;
            public const ushort VK_W = 0x57;
            public const ushort VK_X = 0x58;
            public const ushort VK_Y = 0x59;
            public const ushort VK_Z = 0x5A;
            public const ushort VK_LWIN = 0x5B;
            public const ushort VK_RWIN = 0x5C;
            public const ushort VK_APPS = 0x5D;
            public const ushort VK_SLEEP = 0x5F;
            public const ushort VK_NUMPAD0 = 0x60;
            public const ushort VK_NUMPAD1 = 0x61;
            public const ushort VK_NUMPAD2 = 0x62;
            public const ushort VK_NUMPAD3 = 0x63;
            public const ushort VK_NUMPAD4 = 0x64;
            public const ushort VK_NUMPAD5 = 0x65;
            public const ushort VK_NUMPAD6 = 0x66;
            public const ushort VK_NUMPAD7 = 0x67;
            public const ushort VK_NUMPAD8 = 0x68;
            public const ushort VK_NUMPAD9 = 0x69;
            public const ushort VK_MULTIPLY = 0x6A;
            public const ushort VK_ADD = 0x6B;
            public const ushort VK_SEPARATOR = 0x6C;
            public const ushort VK_SUBTRACT = 0x6D;
            public const ushort VK_DECIMAL = 0x6E;
            public const ushort VK_DIVIDE = 0x6F;
            public const ushort VK_F1 = 0x70;
            public const ushort VK_F2 = 0x71;
            public const ushort VK_F3 = 0x72;
            public const ushort VK_F4 = 0x73;
            public const ushort VK_F5 = 0x74;
            public const ushort VK_F6 = 0x75;
            public const ushort VK_F7 = 0x76;
            public const ushort VK_F8 = 0x77;
            public const ushort VK_F9 = 0x78;
            public const ushort VK_F10 = 0x79;
            public const ushort VK_F11 = 0x7A;
            public const ushort VK_F12 = 0x7B;
            public const ushort VK_F13 = 0x7C;
            public const ushort VK_F14 = 0x7D;
            public const ushort VK_F15 = 0x7E;
            public const ushort VK_F16 = 0x7F;
            public const ushort VK_F17 = 0x80;
            public const ushort VK_F18 = 0x81;
            public const ushort VK_F19 = 0x82;
            public const ushort VK_F20 = 0x83;
            public const ushort VK_F21 = 0x84;
            public const ushort VK_F22 = 0x85;
            public const ushort VK_F23 = 0x86;
            public const ushort VK_F24 = 0x87;
            public const ushort VK_NUMLOCK = 0x90;
            public const ushort VK_SCROLL = 0x91;
            public const ushort VK_LSHIFT = 0xA0;
            public const ushort VK_RSHIFT = 0xA1;
            public const ushort VK_LCONTROL = 0xA2;
            public const ushort VK_RCONTROL = 0xA3;
            public const ushort VK_LMENU = 0xA4;
            public const ushort VK_RMENU = 0xA5;
        }

        #endregion

        #region Methods

        public static void PressKey(string key)
        {
            ushort vkCode = GetVirtualKeyCode(key);
            
            INPUT[] inputs = new INPUT[2];
            
            // Key down
            inputs[0] = new INPUT
            {
                type = INPUT_KEYBOARD,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = vkCode,
                        wScan = 0,
                        dwFlags = 0,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };
            
            // Key up
            inputs[1] = new INPUT
            {
                type = INPUT_KEYBOARD,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = vkCode,
                        wScan = 0,
                        dwFlags = KEYEVENTF_KEYUP,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };
            
            SendInput(2, inputs, Marshal.SizeOf<INPUT>());
        }

        public static void ClickMouse(int x, int y, Models.MouseButtonType button)
        {
            // Move cursor to position
            SetCursorPos(x, y);
            
            uint downFlag, upFlag;
            
            switch (button)
            {
                case Models.MouseButtonType.Left:
                    downFlag = (uint)MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
                    upFlag = (uint)MouseEventFlags.MOUSEEVENTF_LEFTUP;
                    break;
                case Models.MouseButtonType.Right:
                    downFlag = (uint)MouseEventFlags.MOUSEEVENTF_RIGHTDOWN;
                    upFlag = (uint)MouseEventFlags.MOUSEEVENTF_RIGHTUP;
                    break;
                case Models.MouseButtonType.Middle:
                    downFlag = (uint)MouseEventFlags.MOUSEEVENTF_MIDDLEDOWN;
                    upFlag = (uint)MouseEventFlags.MOUSEEVENTF_MIDDLEUP;
                    break;
                default:
                    downFlag = (uint)MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
                    upFlag = (uint)MouseEventFlags.MOUSEEVENTF_LEFTUP;
                    break;
            }
            
            // Small delay after moving
            System.Threading.Thread.Sleep(10);
            
            // Mouse down
            mouse_event(downFlag, 0, 0, 0, UIntPtr.Zero);
            
            // Small delay
            System.Threading.Thread.Sleep(10);
            
            // Mouse up
            mouse_event(upFlag, 0, 0, 0, UIntPtr.Zero);
        }

        public static void GetCursorPosition(out int x, out int y)
        {
            POINT point;
            if (GetCursorPos(out point))
            {
                x = point.X;
                y = point.Y;
            }
            else
            {
                x = 0;
                y = 0;
            }
        }

        public static bool IsKeyDown(ushort keyCode)
        {
            return (GetAsyncKeyState(keyCode) & 0x8000) != 0;
        }

        public static ushort GetVirtualKeyCode(string key)
        {
            string upperKey = key.ToUpper().Trim();
            
            switch (upperKey)
            {
                case "A": return VirtualKeys.VK_A;
                case "B": return VirtualKeys.VK_B;
                case "C": return VirtualKeys.VK_C;
                case "D": return VirtualKeys.VK_D;
                case "E": return VirtualKeys.VK_E;
                case "F": return VirtualKeys.VK_F;
                case "G": return VirtualKeys.VK_G;
                case "H": return VirtualKeys.VK_H;
                case "I": return VirtualKeys.VK_I;
                case "J": return VirtualKeys.VK_J;
                case "K": return VirtualKeys.VK_K;
                case "L": return VirtualKeys.VK_L;
                case "M": return VirtualKeys.VK_M;
                case "N": return VirtualKeys.VK_N;
                case "O": return VirtualKeys.VK_O;
                case "P": return VirtualKeys.VK_P;
                case "Q": return VirtualKeys.VK_Q;
                case "R": return VirtualKeys.VK_R;
                case "S": return VirtualKeys.VK_S;
                case "T": return VirtualKeys.VK_T;
                case "U": return VirtualKeys.VK_U;
                case "V": return VirtualKeys.VK_V;
                case "W": return VirtualKeys.VK_W;
                case "X": return VirtualKeys.VK_X;
                case "Y": return VirtualKeys.VK_Y;
                case "Z": return VirtualKeys.VK_Z;
                case "0": return VirtualKeys.VK_0;
                case "1": return VirtualKeys.VK_1;
                case "2": return VirtualKeys.VK_2;
                case "3": return VirtualKeys.VK_3;
                case "4": return VirtualKeys.VK_4;
                case "5": return VirtualKeys.VK_5;
                case "6": return VirtualKeys.VK_6;
                case "7": return VirtualKeys.VK_7;
                case "8": return VirtualKeys.VK_8;
                case "9": return VirtualKeys.VK_9;
                case "ENTER": return VirtualKeys.VK_RETURN;
                case "SPACE": return VirtualKeys.VK_SPACE;
                case "TAB": return VirtualKeys.VK_TAB;
                case "ESCAPE": return VirtualKeys.VK_ESCAPE;
                case "SHIFT": return VirtualKeys.VK_SHIFT;
                case "CTRL": return VirtualKeys.VK_CONTROL;
                case "ALT": return VirtualKeys.VK_MENU;
                case "LEFT": return VirtualKeys.VK_LEFT;
                case "UP": return VirtualKeys.VK_UP;
                case "RIGHT": return VirtualKeys.VK_RIGHT;
                case "DOWN": return VirtualKeys.VK_DOWN;
                case "DELETE": return VirtualKeys.VK_DELETE;
                case "INSERT": return VirtualKeys.VK_INSERT;
                case "HOME": return VirtualKeys.VK_HOME;
                case "END": return VirtualKeys.VK_END;
                case "PAGEUP": return VirtualKeys.VK_PRIOR;
                case "PAGEDOWN": return VirtualKeys.VK_NEXT;
                case "F1": return VirtualKeys.VK_F1;
                case "F2": return VirtualKeys.VK_F2;
                case "F3": return VirtualKeys.VK_F3;
                case "F4": return VirtualKeys.VK_F4;
                case "F5": return VirtualKeys.VK_F5;
                case "F6": return VirtualKeys.VK_F6;
                case "F7": return VirtualKeys.VK_F7;
                case "F8": return VirtualKeys.VK_F8;
                case "F9": return VirtualKeys.VK_F9;
                case "F10": return VirtualKeys.VK_F10;
                case "F11": return VirtualKeys.VK_F11;
                case "F12": return VirtualKeys.VK_F12;
                default:
                    // Try to get first character
                    if (upperKey.Length > 0)
                    {
                        char c = upperKey[0];
                        if (c >= 'A' && c <= 'Z')
                            return (ushort)(VirtualKeys.VK_A + (c - 'A'));
                        if (c >= '0' && c <= '9')
                            return (ushort)(VirtualKeys.VK_0 + (c - '0'));
                    }
                    return VirtualKeys.VK_A; // Default
            }
        }

        #endregion
    }
}
