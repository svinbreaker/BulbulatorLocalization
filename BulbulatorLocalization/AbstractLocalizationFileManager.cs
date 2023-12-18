using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulbulatorLocalization
{
    public abstract class AbstractLocalizationFileManager
    {
        public string Extension { get; }

        public AbstractLocalizationFileManager(string extension)
        {
            Extension = extension;
        }

        public abstract string? GetLocalizedString(string filePath, string key);

        public abstract void AddKeyValuePair(string filePath, string key, string value);

        public abstract void RemoveKeyValuePair(string filePath, string key);
    }
}
