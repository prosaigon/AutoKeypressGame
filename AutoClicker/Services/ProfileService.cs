using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoClicker.Models;

namespace AutoClicker.Services
{
    public class ProfileService
    {
        public const string DefaultProfileName = "Default";
        private readonly string _profilesDirectory;
        private string _activeProfileName;

        public string ProfilesDirectory => _profilesDirectory;

        public string ActiveProfileName
        {
            get => _activeProfileName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Profile name cannot be empty.", nameof(value));
                _activeProfileName = value.Trim();
                EnsureProfileExists(_activeProfileName);
            }
        }

        public ProfileService(string baseDirectory = null)
        {
            string baseDir = baseDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
            _profilesDirectory = Path.Combine(baseDir, "profiles");

            if (!Directory.Exists(_profilesDirectory))
            {
                Directory.CreateDirectory(_profilesDirectory);
            }

            _activeProfileName = DefaultProfileName;
            EnsureProfileExists(DefaultProfileName);
        }

        public void EnsureProfileExists(string profileName)
        {
            if (string.IsNullOrWhiteSpace(profileName)) return;
            string profileDir = Path.Combine(_profilesDirectory, profileName);
            if (!Directory.Exists(profileDir))
            {
                Directory.CreateDirectory(profileDir);
            }
        }

        public List<ProfileModel> ListProfiles()
        {
            if (!Directory.Exists(_profilesDirectory))
                return new List<ProfileModel>();

            var dirs = Directory.GetDirectories(_profilesDirectory);
            var list = new List<ProfileModel>();

            foreach (var dir in dirs)
            {
                var dirInfo = new DirectoryInfo(dir);
                list.Add(new ProfileModel
                {
                    Name = dirInfo.Name,
                    DirectoryPath = dir,
                    LastModified = dirInfo.LastWriteTime
                });
            }

            if (!list.Any(p => string.Equals(p.Name, DefaultProfileName, StringComparison.OrdinalIgnoreCase)))
            {
                EnsureProfileExists(DefaultProfileName);
                list.Insert(0, new ProfileModel
                {
                    Name = DefaultProfileName,
                    DirectoryPath = Path.Combine(_profilesDirectory, DefaultProfileName),
                    LastModified = DateTime.Now
                });
            }

            return list.OrderBy(p => p.Name != DefaultProfileName).ThenBy(p => p.Name).ToList();
        }

        public bool CreateProfile(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            string safeName = string.Join("_", name.Split(Path.GetInvalidFileNameChars())).Trim();
            if (string.IsNullOrEmpty(safeName))
                return false;

            string targetDir = Path.Combine(_profilesDirectory, safeName);
            if (Directory.Exists(targetDir))
                return false;

            Directory.CreateDirectory(targetDir);
            return true;
        }

        public bool DeleteProfile(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || string.Equals(name, DefaultProfileName, StringComparison.OrdinalIgnoreCase))
                return false;

            string targetDir = Path.Combine(_profilesDirectory, name);
            if (!Directory.Exists(targetDir))
                return false;

            Directory.Delete(targetDir, true);

            if (string.Equals(_activeProfileName, name, StringComparison.OrdinalIgnoreCase))
            {
                _activeProfileName = DefaultProfileName;
            }

            return true;
        }

        public bool RenameProfile(string oldName, string newName)
        {
            if (string.IsNullOrWhiteSpace(oldName) || string.IsNullOrWhiteSpace(newName))
                return false;

            if (string.Equals(oldName, DefaultProfileName, StringComparison.OrdinalIgnoreCase))
                return false;

            string oldDir = Path.Combine(_profilesDirectory, oldName);
            string newDir = Path.Combine(_profilesDirectory, newName);

            if (!Directory.Exists(oldDir) || Directory.Exists(newDir))
                return false;

            Directory.Move(oldDir, newDir);

            if (string.Equals(_activeProfileName, oldName, StringComparison.OrdinalIgnoreCase))
            {
                _activeProfileName = newName;
            }

            return true;
        }

        public string GetConfigPath(string profileName = null)
        {
            string targetProfile = string.IsNullOrWhiteSpace(profileName) ? _activeProfileName : profileName;
            EnsureProfileExists(targetProfile);
            return Path.Combine(_profilesDirectory, targetProfile, "config.ini");
        }
    }
}
