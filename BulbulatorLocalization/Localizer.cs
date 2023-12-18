using BulbulatorLocalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace KeyValueLocalization
{
    public class Localizer
    {
        private List<AbstractLocalizationFileManager> _localizationFileManagers = new List<AbstractLocalizationFileManager>() { };
        private Dictionary<string, string> _languagesPaths = new Dictionary<string, string>();

        public Localizer() 
        {
            AddDefaultLocalizationFileManagers();
        }

        public Localizer(List<string> filePaths)
        {
            AddDefaultLocalizationFileManagers();
            foreach (string filePath in filePaths)
            {
                AddLocalizationFile(filePath);
            }
        }

        public string? GetLocalizedString(string languageCode, string key)
        {
            string filePath = _languagesPaths[languageCode];
            string extension = Path.GetExtension(filePath);

            AbstractLocalizationFileManager? localizationFileReader = GetLocalizationFileManagerByExtension(extension);
            if (localizationFileReader == null)
            {
                throw new NotSupportedException($"Unsupported file format: {extension}");
            }

            return localizationFileReader.GetLocalizedString(filePath, key);
        }

        public void CreateLocalizationFile(string filePath, string languageCode, string extension)
        {
            if (!ExtensionIsSupported(extension))
            {
                throw new NotSupportedException($"Unsupported file format: {extension}");
            }

            if (_languagesPaths.ContainsKey(languageCode))
            {
                throw new ArgumentException($"Language file already exist: {languageCode}{extension}");
            }

            try
            {
                FileInfo fi = new FileInfo($@"{filePath}\{languageCode}.{extension}");
                DirectoryInfo di = new DirectoryInfo(filePath);
                if (!di.Exists)
                {
                    di.Create();
                }

                if (!fi.Exists)
                {
                    fi.Create().Dispose();
                }

                AddLocalizationFile(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the localization file: {ex.Message}");
            }
        }

        public void DeleteLocalizationFile(string languageCode)
        {
            try
            {
                RemoveLocalizationFile(languageCode);
                File.Delete(_languagesPaths[languageCode]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting the localization file: {ex.Message}");
            }
        }

        public void AddLocalizationFile(string filePath) 
        {
            try
            {
                _languagesPaths.Add(Path.GetFileNameWithoutExtension(filePath), filePath);
            }
            catch (FileNotFoundException ex) 
            {
                Console.WriteLine($"The localization file not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting the localization file: {ex.Message}");
            }
        }

        public void AddKeyValuePair(string languageCode, string key, string value) 
        {
            string filePath = _languagesPaths[languageCode];
            string extension = Path.GetExtension(filePath);

            AbstractLocalizationFileManager? localizationFileReader = GetLocalizationFileManagerByExtension(extension);

            if (localizationFileReader == null)
            {
                throw new NotSupportedException($"Unsupported file format: {extension}");
            }

            localizationFileReader.AddKeyValuePair(filePath, key, value);
        }

        public void RemoveKeyValuePair(string languageCode, string key, string value)
        {
            string filePath = _languagesPaths[languageCode];
            string extension = Path.GetExtension(filePath);

            AbstractLocalizationFileManager? localizationFileReader = GetLocalizationFileManagerByExtension(extension);

            if (localizationFileReader == null)
            {
                throw new NotSupportedException($"Unsupported file format: {extension}");
            }

            localizationFileReader.AddKeyValuePair(filePath, key, value);
        }

        public void RemoveLocalizationFile(string languageCode) 
        {
            try
            {
                _languagesPaths.Remove(languageCode);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"The localization file not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while removing the localization file: {ex.Message}");
            }
        }

        private bool ExtensionIsSupported(string extension)
        {
            return (_localizationFileManagers.Find(reader => reader.Extension == extension) != null);
        }

        private AbstractLocalizationFileManager? GetLocalizationFileManagerByExtension(string extension) 
        {
            return _localizationFileManagers.FirstOrDefault(reader => reader.Extension == extension);
        }

        private void AddDefaultLocalizationFileManagers() 
        {
            IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(AbstractLocalizationFileManager).IsAssignableFrom(t) && !t.IsAbstract);
            foreach(var type in types) 
            {
                _localizationFileManagers.Add((AbstractLocalizationFileManager)Activator.CreateInstance(type));
            }
        }

        public void AddLocalizationFileManager(AbstractLocalizationFileManager localizationFileManager) 
        {
            try 
            {
                _localizationFileManagers.Add(localizationFileManager);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to add a localization file manager: {ex.Message}");
            }
        }

        public void RemoveLocalizationFileManager(AbstractLocalizationFileManager localizationFileManager)
        {
            try
            {
                _localizationFileManagers.Remove(localizationFileManager);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to add a localization file manager: {ex.Message}");
            }
        }

        public bool ContainsLanguage(string languageCode) 
        {
            return _languagesPaths.ContainsKey(languageCode);
        }

        public List<string> GetLanguages() 
        {
            return _languagesPaths.Keys.ToList();
        }
    }
}
