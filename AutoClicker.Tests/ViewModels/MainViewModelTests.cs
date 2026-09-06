using System;
using System.IO;
using AutoClicker.Models;
using AutoClicker.Services;
using AutoClicker.ViewModels;
using Xunit;

namespace AutoClicker.Tests.ViewModels
{
    public class MainViewModelTests
    {
        [Fact]
        public void AddKeyboardAction_AddsItemToActionsList()
        {
            var vm = new MainViewModel();
            int countBefore = vm.Actions.Count;

            vm.AddKeyboardActionCommand.Execute(null);

            Assert.Equal(countBefore + 1, vm.Actions.Count);
            Assert.Equal(ActionType.Keyboard, vm.Actions[vm.Actions.Count - 1].ActionType);
        }

        [Fact]
        public void DeleteAction_RemovesSelectedItemFromList()
        {
            var vm = new MainViewModel();
            var item = new ActionItem { ActionType = ActionType.Keyboard, KeyOrButton = "X" };
            vm.Actions.Add(item);

            vm.DeleteActionCommand.Execute(item);

            Assert.DoesNotContain(item, vm.Actions);
        }

        [Fact]
        public void SelectedLoopMode_Custom_SetsIsCustomLoopVisibleToTrue()
        {
            var vm = new MainViewModel();
            vm.SelectedLoopMode = "Custom";

            Assert.True(vm.IsCustomLoopVisible);
        }

        [Fact]
        public void SelectedLoopMode_NoLoop_SetsIsCustomLoopVisibleToFalse()
        {
            var vm = new MainViewModel();
            vm.SelectedLoopMode = "No Loop";

            Assert.False(vm.IsCustomLoopVisible);
        }

        [Fact]
        public void ClearActions_ClearsAllActionItems()
        {
            var vm = new MainViewModel();
            vm.Actions.Add(new ActionItem { ActionType = ActionType.Keyboard, KeyOrButton = "A" });
            vm.Actions.Add(new ActionItem { ActionType = ActionType.Keyboard, KeyOrButton = "B" });

            vm.ClearActions();

            Assert.Empty(vm.Actions);
        }

        [Fact]
        public void SaveConfigAndLoadConfig_PersistsAllFourControlHotkeys()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            var ini = new IniFileService(configPath);

            ini.Write("Settings", "StartStopHotkey", "CTRL+F6");
            ini.Write("Settings", "RecordHotkey", "CTRL+F7");
            ini.Write("Settings", "PauseResumeHotkey", "CTRL+F8");
            ini.Write("Settings", "ClearActionsHotkey", "CTRL+F9");
            ini.Save();

            var vm = new MainViewModel();

            Assert.Equal("CTRL+F6", vm.StartStopHotkey);
            Assert.Equal("CTRL+F7", vm.RecordHotkey);
            Assert.Equal("CTRL+F8", vm.PauseResumeHotkey);
            Assert.Equal("CTRL+F9", vm.ClearActionsHotkey);
        }

        [Fact]
        public void PauseButtonText_ReflectsIsPausedState()
        {
            var vm = new MainViewModel();
            Assert.Equal("⏸️ Pause", vm.PauseButtonText);

            vm.IsPaused = true;
            Assert.Equal("▶️ Resume", vm.PauseButtonText);

            vm.IsPaused = false;
            Assert.Equal("⏸️ Pause", vm.PauseButtonText);
        }

        [Fact]
        public void ResetHotkeysCommands_ResetIndividualHotkeysToDefaults()
        {
            var vm = new MainViewModel();
            vm.StartStopHotkey = "CTRL+A";
            vm.RecordHotkey = "CTRL+B";
            vm.PauseResumeHotkey = "CTRL+C";
            vm.ClearActionsHotkey = "CTRL+D";

            vm.ResetStartStopHotkeyCommand.Execute(null);
            Assert.Equal("F6", vm.StartStopHotkey);

            vm.ResetRecordHotkeyCommand.Execute(null);
            Assert.Equal("F7", vm.RecordHotkey);

            vm.ResetPauseResumeHotkeyCommand.Execute(null);
            Assert.Equal("F8", vm.PauseResumeHotkey);

            vm.ResetClearActionsHotkeyCommand.Execute(null);
            Assert.Equal("F9", vm.ClearActionsHotkey);
        }

        [Fact]
        public void ResetAllHotkeysCommand_ResetsAllHotkeysToDefaults()
        {
            var vm = new MainViewModel();
            vm.StartStopHotkey = "ALT+1";
            vm.RecordHotkey = "ALT+2";
            vm.PauseResumeHotkey = "ALT+3";
            vm.ClearActionsHotkey = "ALT+4";

            vm.ResetAllHotkeysCommand.Execute(null);

            Assert.Equal("F6", vm.StartStopHotkey);
            Assert.Equal("F7", vm.RecordHotkey);
            Assert.Equal("F8", vm.PauseResumeHotkey);
            Assert.Equal("F9", vm.ClearActionsHotkey);
        }

        [Fact]
        public void AppVersion_ReturnsFormattedAssemblyVersion()
        {
            var vm = new MainViewModel();
            Assert.NotNull(vm.AppVersion);
            Assert.StartsWith("v", vm.AppVersion);
        }
    }
}
