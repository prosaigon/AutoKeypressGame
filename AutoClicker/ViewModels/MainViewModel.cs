using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private readonly ProfileService _profileService;
        private IniFileService _iniFile;
        
        private ObservableCollection<ActionItem> _actions;
        private ActionItem _selectedAction;
        private ObservableCollection<ProfileModel> _profiles;
        private ProfileModel _selectedProfile;
        private string _newProfileName;

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
        private bool _isRecording;
        private string _recordHotkey;
        private string _pauseResumeHotkey;
        private string _clearActionsHotkey;
        private bool _isPaused;

        public MainViewModel()
        {
            _profileService = new ProfileService();
            _profiles = new ObservableCollection<ProfileModel>();
            
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
            _recordHotkey = "F7";
            _pauseResumeHotkey = "F8";
            _clearActionsHotkey = "F9";
            _playSound = false;
            _isCustomLoopVisible = false;
            _isRecording = false;
            _isPaused = false;

            RefreshProfiles();

            AddKeyboardActionCommand = new RelayCommand(AddKeyboardAction);
            AddClickActionCommand = new RelayCommand(AddClickAction);
            AddWaitForPixelActionCommand = new RelayCommand(AddWaitForPixelAction);
            AddPixelWatchActionCommand = new RelayCommand(AddPixelWatchAction);
            PickScreenColorCommand = new RelayCommand(PickScreenColor);
            DeleteActionCommand = new RelayCommand<ActionItem>(DeleteAction);
            GetCursorPositionCommand = new RelayCommand(GetCursorPosition);

            MoveUpCommand = new RelayCommand(MoveActionUp, () => SelectedAction != null && Actions.IndexOf(SelectedAction) > 0);
            MoveDownCommand = new RelayCommand(MoveActionDown, () => SelectedAction != null && Actions.IndexOf(SelectedAction) < Actions.Count - 1);
            DuplicateActionCommand = new RelayCommand(DuplicateAction, () => SelectedAction != null);

            StartCommand = new RelayCommand(StartAutomation);
            StopCommand = new RelayCommand(StopAutomation);
            PauseCommand = new RelayCommand(TogglePause);
            ClearActionsCommand = new RelayCommand(ClearActions);
            RecordCommand = new RelayCommand(ToggleRecording);

            SaveConfigCommand = new RelayCommand(SaveConfig);
            LoadConfigCommand = new RelayCommand(LoadConfig);
            CreateProfileCommand = new RelayCommand(CreateProfile);
            DeleteProfileCommand = new RelayCommand(DeleteProfile);

            ResetStartStopHotkeyCommand = new RelayCommand(() => StartStopHotkey = "F6");
            ResetRecordHotkeyCommand = new RelayCommand(() => RecordHotkey = "F7");
            ResetPauseResumeHotkeyCommand = new RelayCommand(() => PauseResumeHotkey = "F8");
            ResetClearActionsHotkeyCommand = new RelayCommand(() => ClearActionsHotkey = "F9");
            ResetAllHotkeysCommand = new RelayCommand(ResetAllHotkeys);

            InputRecorderService.ActionRecorded += OnActionRecorded;
        }

        public void ResetAllHotkeys()
        {
            StartStopHotkey = "F6";
            RecordHotkey = "F7";
            PauseResumeHotkey = "F8";
            ClearActionsHotkey = "F9";
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

        public bool IsRunning
        {
            get => _isRunning;
            private set { _isRunning = value; OnPropertyChanged(); }
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

        public bool IsRecording
        {
            get => _isRecording;
            set
            {
                _isRecording = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RecordButtonText));
            }
        }

        public string RecordHotkey
        {
            get => _recordHotkey;
            set { _recordHotkey = value; OnPropertyChanged(); }
        }

        public string PauseResumeHotkey
        {
            get => _pauseResumeHotkey;
            set { _pauseResumeHotkey = value; OnPropertyChanged(); }
        }

        public string ClearActionsHotkey
        {
            get => _clearActionsHotkey;
            set { _clearActionsHotkey = value; OnPropertyChanged(); }
        }

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                _isPaused = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PauseButtonText));
            }
        }

        public string RecordButtonText => IsRecording ? "⏹️ Stop Rec" : "🔴 Record";
        public string PauseButtonText => IsPaused ? "▶️ Resume" : "⏸️ Pause";

        public ObservableCollection<ProfileModel> Profiles
        {
            get => _profiles;
            set { _profiles = value; OnPropertyChanged(); }
        }

        public ProfileModel SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (_selectedProfile != value && value != null)
                {
                    _selectedProfile = value;
                    _profileService.ActiveProfileName = value.Name;
                    OnPropertyChanged();
                    LoadConfig();
                }
            }
        }

        public string NewProfileName
        {
            get => _newProfileName;
            set { _newProfileName = value; OnPropertyChanged(); }
        }

        public string AppVersion
        {
            get
            {
                var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return ver != null ? $"v{ver.Major}.{ver.Minor}.{ver.Build}" : "v2.0.0";
            }
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
        public ICommand AddWaitForPixelActionCommand { get; }
        public ICommand AddPixelWatchActionCommand { get; }
        public ICommand PickScreenColorCommand { get; }
        public ICommand DeleteActionCommand { get; }
        public ICommand GetCursorPositionCommand { get; }

        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand DuplicateActionCommand { get; }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ClearActionsCommand { get; }
        public ICommand RecordCommand { get; }
        public ICommand SaveConfigCommand { get; }
        public ICommand LoadConfigCommand { get; }
        public ICommand CreateProfileCommand { get; }
        public ICommand DeleteProfileCommand { get; }

        public ICommand ResetStartStopHotkeyCommand { get; }
        public ICommand ResetRecordHotkeyCommand { get; }
        public ICommand ResetPauseResumeHotkeyCommand { get; }
        public ICommand ResetClearActionsHotkeyCommand { get; }
        public ICommand ResetAllHotkeysCommand { get; }

        #endregion

        #region Profile Management

        public void RefreshProfiles()
        {
            var list = _profileService.ListProfiles();
            Profiles.Clear();
            foreach (var p in list)
            {
                Profiles.Add(p);
            }

            var current = Profiles.FirstOrDefault(p => string.Equals(p.Name, _profileService.ActiveProfileName, StringComparison.OrdinalIgnoreCase));
            _selectedProfile = current ?? Profiles.FirstOrDefault();
            OnPropertyChanged(nameof(SelectedProfile));

            LoadConfig();
        }

        public void CreateProfile()
        {
            if (string.IsNullOrWhiteSpace(NewProfileName))
            {
                MessageBox.Show("Please enter a profile name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = NewProfileName.Trim();
            if (_profileService.CreateProfile(name))
            {
                _profileService.ActiveProfileName = name;
                NewProfileName = string.Empty;
                RefreshProfiles();
                SaveConfig();
            }
            else
            {
                MessageBox.Show("A profile with this name already exists or name is invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteProfile()
        {
            if (SelectedProfile == null || string.Equals(SelectedProfile.Name, ProfileService.DefaultProfileName, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Cannot delete the Default profile.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nameToDelete = SelectedProfile.Name;
            var result = MessageBox.Show($"Are you sure you want to delete profile '{nameToDelete}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _profileService.DeleteProfile(nameToDelete);
                RefreshProfiles();
            }
        }

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
            SelectedAction = action;
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
            SelectedAction = action;
        }

        private void AddWaitForPixelAction()
        {
            InputSimulatorService.GetCursorPosition(out int x, out int y);
            var color = ScreenCaptureService.GetPixelColor(x, y);
            string hexColor = ScreenCaptureService.ColorToHex(color);

            var action = new ActionItem
            {
                ActionType = ActionType.WaitForPixelColor,
                X = x,
                Y = y,
                TargetColor = hexColor,
                ColorTolerance = 10,
                TimeoutMs = 5000,
                Delay = _defaultDelay
            };
            Actions.Add(action);
            SelectedAction = action;
            StatusText = $"Added Pixel Wait at ({x}, {y}) for {hexColor}";
        }

        private void AddPixelWatchAction()
        {
            InputSimulatorService.GetCursorPosition(out int x, out int y);
            var color = ScreenCaptureService.GetPixelColor(x, y);
            string hexColor = ScreenCaptureService.ColorToHex(color);

            var action = new ActionItem
            {
                ActionType = ActionType.ConditionalPixelColor,
                X = x,
                Y = y,
                TargetColor = hexColor,
                ColorTolerance = 10,
                KeyOrButton = "SPACE",
                Delay = _defaultDelay
            };
            Actions.Add(action);
            SelectedAction = action;
            StatusText = $"Added Vision Watch at ({x}, {y}) for {hexColor} -> Press SPACE";
        }

        private void PickScreenColor()
        {
            InputSimulatorService.GetCursorPosition(out int x, out int y);
            var color = ScreenCaptureService.GetPixelColor(x, y);
            string hex = ScreenCaptureService.ColorToHex(color);
            StatusText = $"Color at ({x}, {y}): {hex} (RGB: {color.R}, {color.G}, {color.B})";

            if (SelectedAction != null && (SelectedAction.ActionType == ActionType.WaitForPixelColor ||
                                          SelectedAction.ActionType == ActionType.WaitForPixelChange ||
                                          SelectedAction.ActionType == ActionType.ConditionalPixelColor))
            {
                SelectedAction.X = x;
                SelectedAction.Y = y;
                SelectedAction.TargetColor = hex;
            }
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
            var color = ScreenCaptureService.GetPixelColor(x, y);
            string hex = ScreenCaptureService.ColorToHex(color);
            MessageBox.Show($"Current cursor position: ({x}, {y})\nColor: {hex} (R:{color.R} G:{color.G} B:{color.B})", "Cursor Position & Color", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void MoveActionUp()
        {
            if (SelectedAction == null) return;
            int index = Actions.IndexOf(SelectedAction);
            if (index > 0)
            {
                var item = SelectedAction;
                Actions.Move(index, index - 1);
                SelectedAction = item;
            }
        }

        public void MoveActionDown()
        {
            if (SelectedAction == null) return;
            int index = Actions.IndexOf(SelectedAction);
            if (index >= 0 && index < Actions.Count - 1)
            {
                var item = SelectedAction;
                Actions.Move(index, index + 1);
                SelectedAction = item;
            }
        }

        public void DuplicateAction()
        {
            if (SelectedAction == null) return;
            int index = Actions.IndexOf(SelectedAction);
            var clone = new ActionItem
            {
                ActionType = SelectedAction.ActionType,
                KeyOrButton = SelectedAction.KeyOrButton,
                MouseButton = SelectedAction.MouseButton,
                X = SelectedAction.X,
                Y = SelectedAction.Y,
                TargetColor = SelectedAction.TargetColor,
                ColorTolerance = SelectedAction.ColorTolerance,
                TimeoutMs = SelectedAction.TimeoutMs,
                Delay = SelectedAction.Delay
            };
            Actions.Insert(index + 1, clone);
            SelectedAction = clone;
        }

        private async void StartAutomation()
        {
            if (Actions.Count == 0)
            {
                MessageBox.Show("Please add at least one action!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsRunning = true;
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
                IsRunning = false;
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

                    while (IsPaused && !cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(100, cancellationToken);
                    }

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
                    switch (action.ActionType)
                    {
                        case ActionType.Keyboard:
                            InputSimulatorService.PressKey(action.KeyOrButton);
                            StatusText = $"Pressed key: {action.KeyOrButton}";
                            break;

                        case ActionType.MouseClick:
                            InputSimulatorService.ClickMouse(action.X, action.Y, action.MouseButton);
                            StatusText = $"Clicked at ({action.X}, {action.Y})";
                            break;

                        case ActionType.WaitForPixelColor:
                            StatusText = $"Waiting for pixel ({action.X}, {action.Y}) == {action.TargetColor}...";
                            await WaitForPixelColorConditionAsync(action, cancellationToken);
                            break;

                        case ActionType.WaitForPixelChange:
                            StatusText = $"Waiting for pixel change at ({action.X}, {action.Y})...";
                            await WaitForPixelChangeConditionAsync(action, cancellationToken);
                            break;

                        case ActionType.ConditionalPixelColor:
                            var currentColor = ScreenCaptureService.GetPixelColor(action.X, action.Y);
                            var target = ScreenCaptureService.HexToColor(action.TargetColor);
                            if (ScreenCaptureService.ColorsMatch(currentColor, target, action.ColorTolerance))
                            {
                                InputSimulatorService.PressKey(action.KeyOrButton);
                                StatusText = $"Vision Match: Pressed {action.KeyOrButton}";
                            }
                            else
                            {
                                StatusText = $"Vision: No match at ({action.X}, {action.Y})";
                            }
                            break;
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

        private async Task WaitForPixelColorConditionAsync(ActionItem action, CancellationToken cancellationToken)
        {
            var targetColor = ScreenCaptureService.HexToColor(action.TargetColor);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            while (!cancellationToken.IsCancellationRequested)
            {
                var currentColor = ScreenCaptureService.GetPixelColor(action.X, action.Y);
                if (ScreenCaptureService.ColorsMatch(currentColor, targetColor, action.ColorTolerance))
                {
                    return;
                }

                if (action.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds >= action.TimeoutMs)
                {
                    StatusText = $"Pixel wait timed out ({action.TimeoutMs}ms)";
                    return;
                }

                await Task.Delay(50, cancellationToken);
            }
        }

        private async Task WaitForPixelChangeConditionAsync(ActionItem action, CancellationToken cancellationToken)
        {
            var initialColor = ScreenCaptureService.HexToColor(action.TargetColor);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            while (!cancellationToken.IsCancellationRequested)
            {
                var currentColor = ScreenCaptureService.GetPixelColor(action.X, action.Y);
                if (!ScreenCaptureService.ColorsMatch(currentColor, initialColor, action.ColorTolerance))
                {
                    return;
                }

                if (action.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds >= action.TimeoutMs)
                {
                    StatusText = $"Pixel change wait timed out ({action.TimeoutMs}ms)";
                    return;
                }

                await Task.Delay(50, cancellationToken);
            }
        }

        public void ToggleAutomation()
        {
            if (Application.Current != null && Application.Current.Dispatcher != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (CanStart)
                        StartAutomation();
                    else if (CanStop)
                        StopAutomation();
                });
            }
            else
            {
                if (CanStart)
                    StartAutomation();
                else if (CanStop)
                    StopAutomation();
            }
        }

        public void TogglePause()
        {
            if (!IsRunning) return;

            IsPaused = !IsPaused;
            StatusText = IsPaused ? "Paused..." : "Running...";
        }

        public void ClearActions()
        {
            if (IsRunning || IsRecording) return;

            Actions.Clear();
            StatusText = "Actions cleared.";
        }

        public void ToggleRecording()
        {
            if (IsRecording)
            {
                InputRecorderService.StopRecording();
                IsRecording = false;
                CanStart = true;
                CanStop = false;
                StatusText = $"Recording stopped. {Actions.Count} actions captured.";
            }
            else
            {
                if (IsRunning)
                    StopAutomation();

                Actions.Clear();
                InputRecorderService.StartRecording();
                IsRecording = true;
                CanStart = false;
                CanStop = false;
                StatusText = "Recording... Press F7 or Record button to Stop";
            }
        }

        private void OnActionRecorded(ActionItem action)
        {
            if (action == null) return;

            // Filter out the recording toggle hotkey (F7)
            if (action.ActionType == ActionType.Keyboard && string.Equals(action.KeyOrButton, RecordHotkey, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (Application.Current != null && Application.Current.Dispatcher != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Actions.Add(action);
                    StatusText = $"Recorded: {action.DisplayValue}";
                });
            }
            else
            {
                Actions.Add(action);
                StatusText = $"Recorded: {action.DisplayValue}";
            }
        }

        #endregion

        #region Config Methods

        public void SaveConfig()
        {
            try
            {
                string configPath = _profileService.GetConfigPath();
                _iniFile = new IniFileService(configPath);

                // Clear existing action data
                _iniFile.DeleteSection("Actions");
                
                // Save general settings
                _iniFile.Write("Settings", "LoopMode", SelectedLoopMode);
                _iniFile.Write("Settings", "LoopIterations", LoopIterations);
                _iniFile.Write("Settings", "DefaultDelay", DefaultDelay);
                _iniFile.Write("Settings", "StartStopHotkey", StartStopHotkey);
                _iniFile.Write("Settings", "RecordHotkey", RecordHotkey);
                _iniFile.Write("Settings", "PauseResumeHotkey", PauseResumeHotkey);
                _iniFile.Write("Settings", "ClearActionsHotkey", ClearActionsHotkey);
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
                    _iniFile.Write("Actions", $"{prefix}_TargetColor", action.TargetColor ?? "#FFFFFF");
                    _iniFile.Write("Actions", $"{prefix}_ColorTolerance", action.ColorTolerance);
                    _iniFile.Write("Actions", $"{prefix}_TimeoutMs", action.TimeoutMs);
                }

                _iniFile.Save();
                StatusText = $"Profile '{_profileService.ActiveProfileName}' saved.";
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
                string configPath = _profileService.GetConfigPath();
                
                // Fallback / migrate from root config.ini if default profile config doesn't exist
                if (!File.Exists(configPath) && string.Equals(_profileService.ActiveProfileName, ProfileService.DefaultProfileName, StringComparison.OrdinalIgnoreCase))
                {
                    string rootLegacyConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
                    if (File.Exists(rootLegacyConfig))
                    {
                        File.Copy(rootLegacyConfig, configPath, true);
                    }
                }

                if (!File.Exists(configPath))
                    return;

                _iniFile = new IniFileService(configPath);
                _iniFile.Load();

                // Load general settings
                SelectedLoopMode = _iniFile.Read("Settings", "LoopMode", "No Loop");
                LoopIterations = _iniFile.ReadInt("Settings", "LoopIterations", 1);
                DefaultDelay = _iniFile.ReadInt("Settings", "DefaultDelay", 100);
                StartStopHotkey = _iniFile.Read("Settings", "StartStopHotkey", _iniFile.Read("Settings", "Hotkey", "F6"));
                RecordHotkey = _iniFile.Read("Settings", "RecordHotkey", "F7");
                PauseResumeHotkey = _iniFile.Read("Settings", "PauseResumeHotkey", "F8");
                ClearActionsHotkey = _iniFile.Read("Settings", "ClearActionsHotkey", "F9");
                PlaySound = _iniFile.ReadBool("Settings", "PlaySound", false);

                // Load actions
                Actions.Clear();
                int actionCount = _iniFile.ReadInt("Actions", "Count", 0);
                
                for (int i = 0; i < actionCount; i++)
                {
                    string prefix = $"Action{i}";
                    
                    string actionTypeStr = _iniFile.Read("Actions", $"{prefix}_Type", "Keyboard");
                    if (!Enum.TryParse<ActionType>(actionTypeStr, true, out var actionType))
                    {
                        actionType = ActionType.Keyboard;
                    }

                    string mouseBtnStr = _iniFile.Read("Actions", $"{prefix}_MouseButton", "Left");
                    if (!Enum.TryParse<MouseButtonType>(mouseBtnStr, true, out var mouseButton))
                    {
                        mouseButton = MouseButtonType.Left;
                    }

                    var action = new ActionItem
                    {
                        ActionType = actionType,
                        KeyOrButton = _iniFile.Read("Actions", $"{prefix}_KeyOrButton", "A"),
                        MouseButton = mouseButton,
                        X = _iniFile.ReadInt("Actions", $"{prefix}_X", 0),
                        Y = _iniFile.ReadInt("Actions", $"{prefix}_Y", 0),
                        Delay = _iniFile.ReadInt("Actions", $"{prefix}_Delay", 100),
                        TargetColor = _iniFile.Read("Actions", $"{prefix}_TargetColor", "#FFFFFF"),
                        ColorTolerance = _iniFile.ReadInt("Actions", $"{prefix}_ColorTolerance", 10),
                        TimeoutMs = _iniFile.ReadInt("Actions", $"{prefix}_TimeoutMs", 5000)
                    };
                    
                    Actions.Add(action);
                }

                StatusText = $"Profile '{_profileService.ActiveProfileName}' loaded.";
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
