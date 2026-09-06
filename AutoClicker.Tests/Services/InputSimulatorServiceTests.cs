using System;
using AutoClicker.Services;
using Xunit;

namespace AutoClicker.Tests.Services
{
    public class InputSimulatorServiceTests
    {
        [Theory]
        [InlineData("A", 0x41)]
        [InlineData("Z", 0x5A)]
        [InlineData("0", 0x30)]
        [InlineData("9", 0x39)]
        [InlineData("F1", 0x70)]
        [InlineData("F6", 0x75)]
        [InlineData("F12", 0x7B)]
        [InlineData("ENTER", 0x0D)]
        [InlineData("SPACE", 0x20)]
        [InlineData("TAB", 0x09)]
        [InlineData("ESCAPE", 0x1B)]
        [InlineData("NUMPAD0", 0x60)]
        [InlineData("NUMPAD1", 0x61)]
        [InlineData("NUMPAD5", 0x65)]
        [InlineData("NUMPAD9", 0x69)]
        [InlineData("NUMLOCK", 0x90)]
        public void GetVirtualKeyCode_StandardKeys_ReturnsCorrectVkCode(string keyName, ushort expectedVk)
        {
            ushort actualVk = InputSimulatorService.GetVirtualKeyCode(keyName);
            Assert.Equal(expectedVk, actualVk);
        }

        [Theory]
        [InlineData("CTRL+F6", 0x75)]
        [InlineData("ALT+A", 0x41)]
        [InlineData("SHIFT+CTRL+Z", 0x5A)]
        public void GetVirtualKeyCode_ComboKeys_ExtractsMainKeyVkCode(string comboName, ushort expectedVk)
        {
            ushort actualVk = InputSimulatorService.GetVirtualKeyCode(comboName);
            Assert.Equal(expectedVk, actualVk);
        }

        [Theory]
        [InlineData(",", 0xBC)]
        [InlineData(".", 0xBE)]
        [InlineData("-", 0xBD)]
        [InlineData("=", 0xBB)]
        [InlineData("/", 0xBF)]
        [InlineData(";", 0xBA)]
        public void GetVirtualKeyCode_SymbolKeys_ReturnsCorrectOemVkCode(string symbol, ushort expectedVk)
        {
            ushort actualVk = InputSimulatorService.GetVirtualKeyCode(symbol);
            Assert.Equal(expectedVk, actualVk);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetVirtualKeyCode_InvalidOrEmpty_DefaultsToVkA(string invalidKey)
        {
            ushort actualVk = InputSimulatorService.GetVirtualKeyCode(invalidKey);
            Assert.Equal(0x41, actualVk); // VirtualKeys.VK_A
        }
    }
}
