using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulbulatorLocalization.FileManagers
{
    public class JsonLocalizationFileManager : AbstractLocalizationFileManager
    {
        public JsonLocalizationFileManager() : base(".json") { }

        public override string? GetLocalizedString(string filePath, string key)
        {
            string? value = null;
            try
            {
                if (File.Exists(filePath))
                {
                    JObject? jObject = GetJObject(filePath);
                    if (jObject != null)
                    {
                        value = jObject[key]?.ToString() ?? null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to get a localized string: {ex.Message}");
            }
            return value;
        }

        public override void AddKeyValuePair(string filePath, string key, string value)
        {
            try
            {
                JObject? jObject = GetJObject(filePath);
                if (jObject != null)
                {
                    jObject.Add(key, value);
                    File.WriteAllText(filePath, jObject.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to add a key-value pair to the localization file: {ex.Message}");
            }
        }

        public override void RemoveKeyValuePair(string filePath, string key)
        {
            try
            {
                JObject? jObject = GetJObject(filePath);
                if (jObject != null)
                {
                    if (!jObject.ContainsKey(key))
                    {
                        throw new KeyNotFoundException("The key does not exist in the localization file");
                    }

                    jObject.Remove(key);
                    File.WriteAllText(filePath, jObject.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to delete a key-value pair to the localization file: {ex.Message}");
            }
        }

        private JObject? GetJObject(string filePath)
        {
            JObject? jObject = null;
            try
            {
                jObject = JObject.Parse(File.ReadAllText(filePath));
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Localization file not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to get a localized string: {ex.Message}");
            }
            return jObject;
        }
    }
}
