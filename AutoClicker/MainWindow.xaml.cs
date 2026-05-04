using System.Windows;
using System.Windows.Input;
using AutoClicker.ViewModels;

namespace AutoClicker
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            
            var key = e.Key;
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
                default:
                    // Handle regular keys
                    if (key >= Key.A && key <= Key.Z)
                    {
                        hotkeyString += keyString;
                    }
                    else if (key >= Key.D0 && key <= Key.D9)
                    {
                        hotkeyString += keyString.Substring(1); // Remove 'D' prefix
                    }
                    else
                    {
                        hotkeyString += keyString;
                    }
                    break;
            }
            
            _viewModel.StartStopHotkey = hotkeyString;
        }
    }
}
