using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulbulatorLocalization.FileManagers
{
    public class TxtLocalizationFileManager : AbstractLocalizationFileManager
    {
        public TxtLocalizationFileManager() : base(".txt") { }

        public override string? GetLocalizedString(string filePath, string key)
        {
            string? value = null;
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');

                if (parts.Length == 2)
                {
                    string currentKey = parts[0].Trim();
                    string currentValue = parts[1].Trim();

                    if (currentKey.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        value = currentValue;
                        break;
                    }
                }
            }
            return value;
        }

        public override void AddKeyValuePair(string filePath, string key, string value)
        {
            string[]? lines = File.ReadAllLines(filePath);

            try
            {
                if (lines != null)
                {

                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(':');
                        if (parts.Length > 0 && parts[0].Trim() == key)
                        {
                            throw new ArgumentException("This key is already used in the localization file");
                        }
                    }
                }
                string keyValueString = key + ": " + value;
                File.AppendAllText(filePath, keyValueString + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to add a key-value pair to the localization file: {ex.Message}");
            }
        }

        public override void RemoveKeyValuePair(string filePath, string key)
        {
            string[]? strings = GetFileLines(filePath);
            List<string> lines;

            if (strings != null)
            {
                lines = strings.ToList();

                try
                {
                    if (lines != null)
                    {
                        bool keyExist = false;
                        for (int i = 0; i < lines.Count; i++)
                        {
                            string[] parts = lines[i].Split(':');
                            if (parts[0] == key)
                            {
                                lines.RemoveAt(i);
                                keyExist = true;
                            }
                        }
                        if (!keyExist)
                        {
                            throw new KeyNotFoundException("The key does not exist in the localization file");
                        }
                        TextWriter tw = new StreamWriter(filePath);
                        foreach (string line in lines)
                            tw.WriteLine(line);
                        tw.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while trying to delete a key-value pair to the localization file: {ex.Message}");
                }
            }
        }

        private string[]? GetFileLines(string filePath)
        {
            string[]? lines = null;
            try
            {
                lines = File.ReadAllLines(filePath);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"The localization file not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while removing the localization file: {ex.Message}");
            }
            return lines;
        }
    }
}
