using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using AutoClicker.Services;
using AutoClicker.ViewModels;

namespace AutoClicker
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private HwndSource _hwndSource;
        private const int HOTKEY_START_STOP_ID = 9000;
        private const int HOTKEY_RECORD_ID = 9001;
        private const int HOTKEY_PAUSE_RESUME_ID = 9002;
        private const int HOTKEY_CLEAR_ACTIONS_ID = 9003;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;

        public MainWindow()
        {
            InitializeComponent();
            
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _hwndSource = HwndSource.FromHwnd(helper.Handle);
            _hwndSource?.AddHook(HwndHook);
            
            RegisterHotkeys();
        }

        protected override void OnClosed(EventArgs e)
        {
            _hwndSource?.RemoveHook(HwndHook);
            IntPtr handle = new WindowInteropHelper(this).Handle;
            if (handle != IntPtr.Zero)
            {
                UnregisterHotKey(handle, HOTKEY_START_STOP_ID);
                UnregisterHotKey(handle, HOTKEY_RECORD_ID);
                UnregisterHotKey(handle, HOTKEY_PAUSE_RESUME_ID);
                UnregisterHotKey(handle, HOTKEY_CLEAR_ACTIONS_ID);
            }
            base.OnClosed(e);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.StartStopHotkey) || 
                e.PropertyName == nameof(MainViewModel.RecordHotkey) ||
                e.PropertyName == nameof(MainViewModel.PauseResumeHotkey) ||
                e.PropertyName == nameof(MainViewModel.ClearActionsHotkey))
            {
                RegisterHotkeys();
            }
        }

        private void RegisterHotkeys()
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            if (handle == IntPtr.Zero) return;

            UnregisterHotKey(handle, HOTKEY_START_STOP_ID);
            UnregisterHotKey(handle, HOTKEY_RECORD_ID);
            UnregisterHotKey(handle, HOTKEY_PAUSE_RESUME_ID);
            UnregisterHotKey(handle, HOTKEY_CLEAR_ACTIONS_ID);

            RegisterSingleHotkey(handle, HOTKEY_START_STOP_ID, _viewModel.StartStopHotkey);
            RegisterSingleHotkey(handle, HOTKEY_RECORD_ID, _viewModel.RecordHotkey);
            RegisterSingleHotkey(handle, HOTKEY_PAUSE_RESUME_ID, _viewModel.PauseResumeHotkey);
            RegisterSingleHotkey(handle, HOTKEY_CLEAR_ACTIONS_ID, _viewModel.ClearActionsHotkey);
        }

        private void RegisterSingleHotkey(IntPtr handle, int id, string hotkeyString)
        {
            if (string.IsNullOrWhiteSpace(hotkeyString)) return;

            uint modifiers = 0;
            string upper = hotkeyString.ToUpper();

            if (upper.Contains("CTRL")) modifiers |= MOD_CONTROL;
            if (upper.Contains("ALT")) modifiers |= MOD_ALT;
            if (upper.Contains("SHIFT")) modifiers |= MOD_SHIFT;

            ushort vk = InputSimulatorService.GetVirtualKeyCode(hotkeyString);
            if (vk > 0)
            {
                RegisterHotKey(handle, id, modifiers, vk);
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (id == HOTKEY_START_STOP_ID)
                {
                    _viewModel.ToggleAutomation();
                    handled = true;
                }
                else if (id == HOTKEY_RECORD_ID)
                {
                    _viewModel.ToggleRecording();
                    handled = true;
                }
                else if (id == HOTKEY_PAUSE_RESUME_ID)
                {
                    _viewModel.TogglePause();
                    handled = true;
                }
                else if (id == HOTKEY_CLEAR_ACTIONS_ID)
                {
                    _viewModel.ClearActions();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        private void HotkeyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox tb)
            {
                tb.Text = "Press key...";
            }
        }

        private void HotkeyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox tb)
            {
                UpdateTextBoxFromViewModel(tb);
            }
        }

        private void UpdateTextBoxFromViewModel(System.Windows.Controls.TextBox tb)
        {
            if (tb == null) return;
            string targetProp = tb.Tag?.ToString();
            if (targetProp == "RecordHotkey")
                tb.Text = _viewModel.RecordHotkey;
            else if (targetProp == "PauseResumeHotkey")
                tb.Text = _viewModel.PauseResumeHotkey;
            else if (targetProp == "ClearActionsHotkey")
                tb.Text = _viewModel.ClearActionsHotkey;
            else
                tb.Text = _viewModel.StartStopHotkey;
        }

        private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            
            Key key = (e.Key == Key.System) ? e.SystemKey : e.Key;
            
            // Ignore standalone modifier keys
            if (key == Key.LeftCtrl || key == Key.RightCtrl ||
                key == Key.LeftAlt || key == Key.RightAlt ||
                key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            var modifiers = Keyboard.Modifiers;
            string hotkeyString = "";
            
            // Add modifiers
            if (modifiers.HasFlag(ModifierKeys.Control))
                hotkeyString += "CTRL+";
            if (modifiers.HasFlag(ModifierKeys.Alt))
                hotkeyString += "ALT+";
            if (modifiers.HasFlag(ModifierKeys.Shift))
                hotkeyString += "SHIFT+";
            
            // Add key
            string keyString = key.ToString();
            
            // Handle special keys
            switch (key)
            {
                case Key.F1:
                case Key.F2:
                case Key.F3:
                case Key.F4:
                case Key.F5:
                case Key.F6:
                case Key.F7:
                case Key.F8:
                case Key.F9:
                case Key.F10:
                case Key.F11:
                case Key.F12:
                    hotkeyString += keyString;
                    break;
                case Key.Space:
                    hotkeyString += "SPACE";
                    break;
                case Key.Enter:
                    hotkeyString += "ENTER";
                    break;
                case Key.Escape:
                    hotkeyString += "ESCAPE";
                    break;
                case Key.Tab:
                    hotkeyString += "TAB";
                    break;
                case Key.Delete:
                    hotkeyString += "DELETE";
                    break;
                case Key.Insert:
                    hotkeyString += "INSERT";
                    break;
                case Key.Home:
                    hotkeyString += "HOME";
                    break;
                case Key.End:
                    hotkeyString += "END";
                    break;
                case Key.PageUp:
                    hotkeyString += "PAGEUP";
                    break;
                case Key.PageDown:
                    hotkeyString += "PAGEDOWN";
                    break;
                case Key.Left:
                    hotkeyString += "LEFT";
                    break;
                case Key.Up:
                    hotkeyString += "UP";
                    break;
                case Key.Right:
                    hotkeyString += "RIGHT";
                    break;
                case Key.Down:
                    hotkeyString += "DOWN";
                    break;
                case Key.NumPad0: hotkeyString += "NUMPAD0"; break;
                case Key.NumPad1: hotkeyString += "NUMPAD1"; break;
                case Key.NumPad2: hotkeyString += "NUMPAD2"; break;
                case Key.NumPad3: hotkeyString += "NUMPAD3"; break;
                case Key.NumPad4: hotkeyString += "NUMPAD4"; break;
                case Key.NumPad5: hotkeyString += "NUMPAD5"; break;
                case Key.NumPad6: hotkeyString += "NUMPAD6"; break;
                case Key.NumPad7: hotkeyString += "NUMPAD7"; break;
                case Key.NumPad8: hotkeyString += "NUMPAD8"; break;
                case Key.NumPad9: hotkeyString += "NUMPAD9"; break;
                case Key.Multiply: hotkeyString += "*"; break;
                case Key.Add: hotkeyString += "+"; break;
                case Key.Subtract: hotkeyString += "-"; break;
                case Key.Decimal: hotkeyString += "."; break;
                case Key.Divide: hotkeyString += "/"; break;
                default:
                    if (key >= Key.A && key <= Key.Z)
                    {
                        hotkeyString += keyString;
                    }
                    else if (key >= Key.D0 && key <= Key.D9)
                    {
                        hotkeyString += keyString.Substring(1);
                    }
                    else
                    {
                        hotkeyString += keyString;
                    }
                    break;
            }
            
            var textBox = sender as System.Windows.Controls.TextBox;
            string targetProp = textBox?.Tag?.ToString();

            if (targetProp == "RecordHotkey")
                _viewModel.RecordHotkey = hotkeyString;
            else if (targetProp == "PauseResumeHotkey")
                _viewModel.PauseResumeHotkey = hotkeyString;
            else if (targetProp == "ClearActionsHotkey")
                _viewModel.ClearActionsHotkey = hotkeyString;
            else
                _viewModel.StartStopHotkey = hotkeyString;

            Keyboard.ClearFocus();
        }
    }
}
