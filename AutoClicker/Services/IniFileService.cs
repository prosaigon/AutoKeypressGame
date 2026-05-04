using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoClicker.Services
{
    public class IniFileService
    {
        private readonly string _filePath;
        private readonly Dictionary<string, Dictionary<string, string>> _data;

        public IniFileService(string filePath)
        {
            _filePath = filePath;
            _data = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            
            if (File.Exists(filePath))
            {
                Load();
            }
        }

        public void Load()
        {
            if (!File.Exists(_filePath))
                return;

            _data.Clear();
            string currentSection = "";

            foreach (string line in File.ReadLines(_filePath, Encoding.UTF8))
            {
                string trimmedLine = line.Trim();
                
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                    continue;

                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    if (!_data.ContainsKey(currentSection))
                        _data[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                else if (trimmedLine.Contains("="))
                {
                    int eqIndex = trimmedLine.IndexOf('=');
                    string key = trimmedLine.Substring(0, eqIndex).Trim();
                    string value = trimmedLine.Substring(eqIndex + 1).Trim();
                    
                    // Remove quotes if present
                    if (value.StartsWith("\"") && value.EndsWith("\""))
                        value = value.Substring(1, value.Length - 2);
                    
                    if (!string.IsNullOrEmpty(currentSection) && !_data[currentSection].ContainsKey(key))
                        _data[currentSection][key] = value;
                }
            }
        }

        public void Save()
        {
            var sb = new StringBuilder();
            sb.AppendLine("; Auto Clicker Configuration File");
            sb.AppendLine($"; Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            foreach (var section in _data)
            {
                sb.AppendLine($"[{section.Key}]");
                foreach (var kvp in section.Value)
                {
                    sb.AppendLine($"{kvp.Key}={kvp.Value}");
                }
                sb.AppendLine();
            }

            File.WriteAllText(_filePath, sb.ToString(), Encoding.UTF8);
        }

        public string Read(string section, string key, string defaultValue = "")
        {
            if (_data.ContainsKey(section) && _data[section].ContainsKey(key))
                return _data[section][key];
            return defaultValue;
        }

        public void Write(string section, string key, string value)
        {
            if (!_data.ContainsKey(section))
                _data[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
            _data[section][key] = value;
        }

        public int ReadInt(string section, string key, int defaultValue = 0)
        {
            string value = Read(section, key, defaultValue.ToString());
            if (int.TryParse(value, out int result))
                return result;
            return defaultValue;
        }

        public void Write(string section, string key, int value)
        {
            Write(section, key, value.ToString());
        }

        public bool ReadBool(string section, string key, bool defaultValue = false)
        {
            string value = Read(section, key, defaultValue.ToString());
            if (bool.TryParse(value, out bool result))
                return result;
            return defaultValue;
        }

        public void Write(string section, string key, bool value)
        {
            Write(section, key, value.ToString());
        }

        public List<string> GetKeys(string section)
        {
            if (_data.ContainsKey(section))
                return new List<string>(_data[section].Keys);
            return new List<string>();
        }

        public List<string> GetSections()
        {
            return new List<string>(_data.Keys);
        }

        public bool ContainsSection(string section)
        {
            return _data.ContainsKey(section);
        }

        public bool ContainsKey(string section, string key)
        {
            return _data.ContainsKey(section) && _data[section].ContainsKey(key);
        }

        public void DeleteKey(string section, string key)
        {
            if (_data.ContainsKey(section) && _data[section].ContainsKey(key))
                _data[section].Remove(key);
        }

        public void DeleteSection(string section)
        {
            if (_data.ContainsKey(section))
                _data.Remove(section);
        }
    }
}
