using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AutoClicker.Models;
using AutoClicker.Services;

namespace AutoClicker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IniFileService _iniFile;
        private readonly string _configPath;
        
        private ObservableCollection<ActionItem> _actions;
        private ActionItem _selectedAction;
        private bool _isRunning;
        private CancellationTokenSource _cancellationTokenSource;
        private string _statusText;
        private string _progressText;
        private double _progressValue;
        private bool _canStart;
        private bool _canStop;
        private string _selectedLoopMode;
        private int _loopIterations;
        private int _defaultDelay;
        private string _startStopHotkey;
        private bool _playSound;
        private bool _isCustomLoopVisible;

        public MainViewModel()
        {
            _configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            _iniFile = new IniFileService(_configPath);
            
            _actions = new ObservableCollection<ActionItem>();
            _statusText = "Ready";
            _progressText = "0 / 0";
            _progressValue = 0;
            _canStart = true;
            _canStop = false;
            _selectedLoopMode = "No Loop";
            _loopIterations = 1;
            _defaultDelay = 100;
            _startStopHotkey = "F6";
            _playSound = false;
            _isCustomLoopVisible = false;

            LoadConfig();

            AddKeyboardActionCommand = new RelayCommand(AddKeyboardAction);
            AddClickActionCommand = new RelayCommand(AddClickAction);
            DeleteActionCommand = new RelayCommand<ActionItem>(DeleteAction);
            GetCursorPositionCommand = new RelayCommand(GetCursorPosition);
            StartCommand = new RelayCommand(StartAutomation);
            StopCommand = new RelayCommand(StopAutomation);
            SaveConfigCommand = new RelayCommand(SaveConfig);
            LoadConfigCommand = new RelayCommand(LoadConfig);

            // Start hotkey listener
            Task.Run(() => HotKeyListener());
        }

        #region Properties

        public ObservableCollection<ActionItem> Actions
        {
            get => _actions;
            set { _actions = value; OnPropertyChanged(); }
        }

        public ActionItem SelectedAction
        {
            get => _selectedAction;
            set { _selectedAction = value; OnPropertyChanged(); }
        }

        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        public string ProgressText
        {
            get => _progressText;
            set { _progressText = value; OnPropertyChanged(); }
        }

        public double ProgressValue
        {
            get => _progressValue;
            set { _progressValue = value; OnPropertyChanged(); }
        }

        public bool CanStart
        {
            get => _canStart;
            set { _canStart = value; OnPropertyChanged(); }
        }

        public bool CanStop
        {
            get => _canStop;
            set { _canStop = value; OnPropertyChanged(); }
        }

        public string[] LoopModes => new[] { "No Loop", "Loop Once", "Continuous", "Custom" };

        public string SelectedLoopMode
        {
            get => _selectedLoopMode;
            set 
            { 
                _selectedLoopMode = value; 
                OnPropertyChanged();
                IsCustomLoopVisible = (value == "Custom");
            }
        }

        public int LoopIterations
        {
            get => _loopIterations;
            set { _loopIterations = value; OnPropertyChanged(); }
        }

        public int DefaultDelay
        {
            get => _defaultDelay;
            set { _defaultDelay = value; OnPropertyChanged(); }
        }

        public string StartStopHotkey
        {
            get => _startStopHotkey;
            set { _startStopHotkey = value; OnPropertyChanged(); }
        }

        public bool PlaySound
        {
            get => _playSound;
            set { _playSound = value; OnPropertyChanged(); }
        }

        public bool IsCustomLoopVisible
        {
            get => _isCustomLoopVisible;
            set { _isCustomLoopVisible = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands

        public ICommand AddKeyboardActionCommand { get; }
        public ICommand AddClickActionCommand { get; }
        public ICommand DeleteActionCommand { get; }
        public ICommand GetCursorPositionCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand SaveConfigCommand { get; }
        public ICommand LoadConfigCommand { get; }

        #endregion

        #region Command Methods

        private void AddKeyboardAction()
        {
            var action = new ActionItem
            {
                ActionType = ActionType.Keyboard,
                KeyOrButton = "A",
                Delay = _defaultDelay
            };
            Actions.Add(action);
        }

        private void AddClickAction()
        {
            InputSimulatorService.GetCursorPosition(out int x, out int y);
            
            var action = new ActionItem
            {
                ActionType = ActionType.MouseClick,
                MouseButton = MouseButtonType.Left,
                X = x,
                Y = y,
                Delay = _defaultDelay
            };
            Actions.Add(action);
        }

        private void DeleteAction(ActionItem action)
        {
            if (action != null && Actions.Contains(action))
            {
                Actions.Remove(action);
            }
        }

        private void GetCursorPosition()
        {
            InputSimulatorService.GetCursorPosition(out int x, out int y);
            MessageBox.Show($"Current cursor position: ({x}, {y})", "Cursor Position", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void StartAutomation()
        {
            if (Actions.Count == 0)
            {
                MessageBox.Show("Please add at least one action!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _isRunning = true;
            CanStart = false;
            CanStop = true;
            StatusText = "Running...";

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await RunAutomation(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                StatusText = "Stopped";
            }
            finally
            {
                _isRunning = false;
                CanStart = true;
                CanStop = false;
                
                if (PlaySound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
        }

        private void StopAutomation()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                StatusText = "Stopping...";
            }
        }

        private async Task RunAutomation(CancellationToken cancellationToken)
        {
            int totalIterations = 1;
            
            switch (SelectedLoopMode)
            {
                case "No Loop":
                    totalIterations = 1;
                    break;
                case "Loop Once":
                    totalIterations = 2;
                    break;
                case "Continuous":
                    totalIterations = int.MaxValue;
                    break;
                case "Custom":
                    totalIterations = LoopIterations;
                    break;
            }

            int completedIterations = 0;

            while (completedIterations < totalIterations && !cancellationToken.IsCancellationRequested)
            {
                for (int i = 0; i < Actions.Count; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var action = Actions[i];
                    
                    // Update progress
                    int currentAction = i + 1;
                    int totalActions = Actions.Count;
                    int overallProgress = (completedIterations * totalActions + currentAction);
                    int maxProgress = totalIterations * totalActions;
                    
                    if (maxProgress > 0 && maxProgress != int.MaxValue)
                    {
                        ProgressValue = (overallProgress * 100.0) / maxProgress;
                        ProgressText = $"{overallProgress} / {maxProgress}";
                    }
                    else
                    {
                        ProgressText = $"Iteration: {completedIterations + 1}";
                    }

                    // Execute action
                    if (action.ActionType == ActionType.Keyboard)
                    {
                        InputSimulatorService.PressKey(action.KeyOrButton);
                        StatusText = $"Pressed key: {action.KeyOrButton}";
                    }
                    else if (action.ActionType == ActionType.MouseClick)
                    {
                        InputSimulatorService.ClickMouse(action.X, action.Y, action.MouseButton);
                        StatusText = $"Clicked at ({action.X}, {action.Y})";
                    }

                    // Wait for delay
                    await Task.Delay(action.Delay, cancellationToken);
                }

                completedIterations++;

                if (SelectedLoopMode != "Continuous" && completedIterations >= totalIterations)
                {
                    StatusText = "Completed!";
                    ProgressValue = 100;
                    break;
                }

                // Small delay between iterations
                if (!cancellationToken.IsCancellationRequested && SelectedLoopMode != "No Loop")
                {
                    await Task.Delay(100, cancellationToken);
                }
            }
        }

        private async void HotKeyListener()
        {
            while (true)
            {
                await Task.Delay(50);

                ushort hotkeyCode = InputSimulatorService.GetVirtualKeyCode(_startStopHotkey);
                
                if (InputSimulatorService.IsKeyDown(hotkeyCode))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (CanStart)
                            StartAutomation();
                        else if (CanStop)
                            StopAutomation();
                    });

                    // Wait for key release
                    while (InputSimulatorService.IsKeyDown(hotkeyCode))
                    {
                        await Task.Delay(50);
                    }
                }
            }
        }

        #endregion

        #region Config Methods

        public void SaveConfig()
        {
            try
            {
                // Clear existing action data
                _iniFile.DeleteSection("Actions");
                
                // Save general settings
                _iniFile.Write("Settings", "LoopMode", SelectedLoopMode);
                _iniFile.Write("Settings", "LoopIterations", LoopIterations);
                _iniFile.Write("Settings", "DefaultDelay", DefaultDelay);
                _iniFile.Write("Settings", "Hotkey", StartStopHotkey);
                _iniFile.Write("Settings", "PlaySound", PlaySound);

                // Save actions
                _iniFile.Write("Actions", "Count", Actions.Count);
                for (int i = 0; i < Actions.Count; i++)
                {
                    var action = Actions[i];
                    string prefix = $"Action{i}";
                    
                    _iniFile.Write("Actions", $"{prefix}_Type", action.ActionType.ToString());
                    _iniFile.Write("Actions", $"{prefix}_KeyOrButton", action.KeyOrButton ?? "");
                    _iniFile.Write("Actions", $"{prefix}_MouseButton", action.MouseButton.ToString());
                    _iniFile.Write("Actions", $"{prefix}_X", action.X);
                    _iniFile.Write("Actions", $"{prefix}_Y", action.Y);
                    _iniFile.Write("Actions", $"{prefix}_Delay", action.Delay);
                }

                _iniFile.Save();
                MessageBox.Show("Configuration saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving config: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadConfig()
        {
            try
            {
                if (!System.IO.File.Exists(_configPath))
                    return;

                _iniFile.Load();

                // Load general settings
                SelectedLoopMode = _iniFile.Read("Settings", "LoopMode", "No Loop");
                LoopIterations = _iniFile.ReadInt("Settings", "LoopIterations", 1);
                DefaultDelay = _iniFile.ReadInt("Settings", "DefaultDelay", 100);
                StartStopHotkey = _iniFile.Read("Settings", "Hotkey", "F6");
                PlaySound = _iniFile.ReadBool("Settings", "PlaySound", false);

                // Load actions
                Actions.Clear();
                int actionCount = _iniFile.ReadInt("Actions", "Count", 0);
                
                for (int i = 0; i < actionCount; i++)
                {
                    string prefix = $"Action{i}";
                    
                    var action = new ActionItem
                    {
                        ActionType = Enum.Parse<ActionType>(_iniFile.Read("Actions", $"{prefix}_Type", "Keyboard")),
                        KeyOrButton = _iniFile.Read("Actions", $"{prefix}_KeyOrButton", "A"),
                        MouseButton = Enum.Parse<MouseButtonType>(_iniFile.Read("Actions", $"{prefix}_MouseButton", "Left")),
                        X = _iniFile.ReadInt("Actions", $"{prefix}_X", 0),
                        Y = _iniFile.ReadInt("Actions", $"{prefix}_Y", 0),
                        Delay = _iniFile.ReadInt("Actions", $"{prefix}_Delay", 100)
                    };
                    
                    Actions.Add(action);
                }

                StatusText = "Configuration loaded";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading config: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;
        public void Execute(object parameter) => _execute((T)parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
