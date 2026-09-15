using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoClicker.Models
{
    public enum ActionType
    {
        Keyboard,
        MouseClick,
        WaitForPixelColor,
        WaitForPixelChange,
        ConditionalPixelColor
    }

    public enum MouseButtonType
    {
        Left,
        Right,
        Middle
    }

    public class ActionItem : INotifyPropertyChanged
    {
        private string _keyOrButton;
        private int _delay;
        private int _x;
        private int _y;
        private ActionType _actionType;
        private MouseButtonType _mouseButton;
        private string _targetColor = "#FFFFFF";
        private int _colorTolerance = 10;
        private int _timeoutMs = 5000;

        public ActionType ActionType
        {
            get => _actionType;
            set
            {
                _actionType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayValue));
            }
        }

        public string KeyOrButton
        {
            get => _keyOrButton;
            set
            {
                _keyOrButton = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayValue));
            }
        }

        public int Delay
        {
            get => _delay;
            set
            {
                _delay = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayDelay));
            }
        }

        public int X
        {
            get => _x;
            set
            {
                _x = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayValue));
            }
        }

        public int Y
        {
            get => _y;
            set
            {
                _y = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayValue));
            }
        }

        public MouseButtonType MouseButton
        {
            get => _mouseButton;
            set
            {
                _mouseButton = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayValue));
            }
        }

        public string TargetColor
        {
            get => _targetColor;
            set
            {
                _targetColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayValue));
            }
        }

        public int ColorTolerance
        {
            get => _colorTolerance;
            set
            {
                _colorTolerance = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayValue));
            }
        }

        public int TimeoutMs
        {
            get => _timeoutMs;
            set
            {
                _timeoutMs = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayValue));
            }
        }

        public string DisplayValue
        {
            get
            {
                switch (ActionType)
                {
                    case ActionType.Keyboard:
                        return $"Key: {KeyOrButton}";
                    case ActionType.MouseClick:
                        return $"Click {MouseButton} at ({X}, {Y})";
                    case ActionType.WaitForPixelColor:
                        return $"Wait Pixel ({X}, {Y}) == {TargetColor} (Tol: {ColorTolerance})";
                    case ActionType.WaitForPixelChange:
                        return $"Wait Pixel ({X}, {Y}) changes from {TargetColor}";
                    case ActionType.ConditionalPixelColor:
                        return $"If Pixel ({X}, {Y}) == {TargetColor} -> Press {KeyOrButton}";
                    default:
                        return $"Action ({ActionType})";
                }
            }
        }

        public string DisplayDelay
        {
            get => $"Delay: {Delay}ms";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
