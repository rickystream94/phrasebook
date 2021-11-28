using Newtonsoft.Json;
using System;
using System.IO;

namespace Phrasebook.Common.Languages
{
    public static class LanguageManager
    {
        private static readonly SupportedLanguage[] supportedLanguages = GetSupportedLanguagesFromJsonFile();

        public static SupportedLanguage[] SupportedLanguages => supportedLanguages;

        private static SupportedLanguage[] GetSupportedLanguagesFromJsonFile()
        {
            string supportedLanguagesJson = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.Constants.SupportedLanguagesFilePath));
            return JsonConvert.DeserializeObject<SupportedLanguage[]>(supportedLanguagesJson);
        }
    }
}
