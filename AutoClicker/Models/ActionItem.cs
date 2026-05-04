using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoClicker.Models
{
    public enum ActionType
    {
        Keyboard,
        MouseClick
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

        public string DisplayValue
        {
            get
            {
                if (ActionType == ActionType.Keyboard)
                {
                    return $"Key: {KeyOrButton}";
                }
                else
                {
                    return $"Click {MouseButton} at ({X}, {Y})";
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
