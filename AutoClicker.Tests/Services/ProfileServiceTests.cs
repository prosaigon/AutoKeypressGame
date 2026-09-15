using System;
using System.IO;
using System.Linq;
using AutoClicker.Services;
using Xunit;

namespace AutoClicker.Tests.Services
{
    public class ProfileServiceTests : IDisposable
    {
        private readonly string _tempDirectory;
        private readonly ProfileService _service;

        public ProfileServiceTests()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), "AutoClicker_ProfileTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDirectory);
            _service = new ProfileService(_tempDirectory);
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_tempDirectory))
                {
                    Directory.Delete(_tempDirectory, true);
                }
            }
            catch { }
        }

        [Fact]
        public void Constructor_InitializesDefaultProfile()
        {
            var profiles = _service.ListProfiles();
            Assert.Contains(profiles, p => p.Name == ProfileService.DefaultProfileName);
            Assert.Equal(ProfileService.DefaultProfileName, _service.ActiveProfileName);
        }

        [Fact]
        public void CreateProfile_ValidName_CreatesSuccessfully()
        {
            bool created = _service.CreateProfile("GamingProfile");
            Assert.True(created);

            var profiles = _service.ListProfiles();
            Assert.Contains(profiles, p => p.Name == "GamingProfile");
        }

        [Fact]
        public void CreateProfile_DuplicateName_ReturnsFalse()
        {
            _service.CreateProfile("GamingProfile");
            bool duplicate = _service.CreateProfile("GamingProfile");
            Assert.False(duplicate);
        }

        [Fact]
        public void CreateProfile_EmptyOrNullName_ReturnsFalse()
        {
            Assert.False(_service.CreateProfile(""));
            Assert.False(_service.CreateProfile("   "));
            Assert.False(_service.CreateProfile(null));
        }

        [Fact]
        public void DeleteProfile_NonDefaultProfile_DeletesSuccessfully()
        {
            _service.CreateProfile("TempProfile");
            Assert.Contains(_service.ListProfiles(), p => p.Name == "TempProfile");

            bool deleted = _service.DeleteProfile("TempProfile");
            Assert.True(deleted);
            Assert.DoesNotContain(_service.ListProfiles(), p => p.Name == "TempProfile");
        }

        [Fact]
        public void DeleteProfile_DefaultProfile_ReturnsFalse()
        {
            bool deleted = _service.DeleteProfile(ProfileService.DefaultProfileName);
            Assert.False(deleted);
        }

        [Fact]
        public void RenameProfile_ValidProfile_RenamesSuccessfully()
        {
            _service.CreateProfile("OldName");
            bool renamed = _service.RenameProfile("OldName", "NewName");
            Assert.True(renamed);

            var profiles = _service.ListProfiles();
            Assert.DoesNotContain(profiles, p => p.Name == "OldName");
            Assert.Contains(profiles, p => p.Name == "NewName");
        }

        [Fact]
        public void GetConfigPath_ReturnsValidPath()
        {
            string path = _service.GetConfigPath("CustomProfile");
            Assert.EndsWith(Path.Combine("CustomProfile", "config.ini"), path);
            Assert.True(Directory.Exists(Path.GetDirectoryName(path)));
        }

        [Fact]
        public void ActiveProfileName_Switch_UpdatesActiveProfile()
        {
            _service.CreateProfile("Profile2");
            _service.ActiveProfileName = "Profile2";
            Assert.Equal("Profile2", _service.ActiveProfileName);
        }
    }
}
