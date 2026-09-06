using System;
using System.IO;
using AutoClicker.Services;
using Xunit;

namespace AutoClicker.Tests.Services
{
    public class IniFileServiceTests : IDisposable
    {
        private readonly string _tempFilePath;

        public IniFileServiceTests()
        {
            _tempFilePath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid():N}.ini");
        }

        public void Dispose()
        {
            if (File.Exists(_tempFilePath))
            {
                try { File.Delete(_tempFilePath); } catch { }
            }
        }

        [Fact]
        public void WriteAndRead_StringValue_SavesAndRetrievesCorrectly()
        {
            var service = new IniFileService(_tempFilePath);
            service.Write("Settings", "Hotkey", "CTRL+F6");
            service.Save();

            var newService = new IniFileService(_tempFilePath);
            string actual = newService.Read("Settings", "Hotkey");

            Assert.Equal("CTRL+F6", actual);
        }

        [Fact]
        public void WriteAndRead_IntAndBoolValue_SavesAndRetrievesCorrectly()
        {
            var service = new IniFileService(_tempFilePath);
            service.Write("Settings", "Delay", 250);
            service.Write("Settings", "PlaySound", true);
            service.Save();

            var newService = new IniFileService(_tempFilePath);

            Assert.Equal(250, newService.ReadInt("Settings", "Delay"));
            Assert.True(newService.ReadBool("Settings", "PlaySound"));
        }

        [Fact]
        public void Read_NonExistentKey_ReturnsDefaultValue()
        {
            var service = new IniFileService(_tempFilePath);
            string actual = service.Read("Settings", "UnknownKey", "DefaultVal");

            Assert.Equal("DefaultVal", actual);
        }

        [Fact]
        public void DeleteSection_RemovesSection()
        {
            var service = new IniFileService(_tempFilePath);
            service.Write("Actions", "Action0_Type", "Keyboard");
            service.DeleteSection("Actions");

            Assert.False(service.ContainsSection("Actions"));
        }
    }
}
