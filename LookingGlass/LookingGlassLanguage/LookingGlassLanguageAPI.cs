using Newtonsoft.Json;
using RoR2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LookingGlass.LookingGlassLanguage
{
    public static class LookingGlassLanguageAPI //calling this an API is really stretching it...
    {
        public const string TokenPrefix = "LG_TOKEN_";
        const string LanguageFolder = "Languages";
        const string LanguageFile = "LookingGlass.json";
        static string languageRootPath;
        static readonly Dictionary<string, Dictionary<string, string>> cachedLanguageFiles = new Dictionary<string, Dictionary<string, string>>();
        static readonly Dictionary<string, string> languageAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["schinese"] = "zh-CN",
            ["simplifiedchinese"] = "zh-CN",
            ["zh"] = "zh-CN",
            ["zh-Hans"] = "zh-CN",
            ["zh_CN"] = "zh-CN",
        };
        static readonly Regex tagsRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
        static readonly Regex tokenCharsRegex = new Regex("[^A-Z0-9]+", RegexOptions.Compiled);

        public static void Init(string pluginLocation)
        {
            languageRootPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(pluginLocation), LanguageFolder);
            if (Directory.Exists(languageRootPath))
            {
                Language.collectLanguageRootFolders += AddLanguageRootFolder;
            }
        }

        static void AddLanguageRootFolder(List<string> folders)
        {
            if (!string.IsNullOrEmpty(languageRootPath) && Directory.Exists(languageRootPath) && !folders.Contains(languageRootPath))
            {
                folders.Add(languageRootPath);
            }
        }

        public static string Token(string token)
        {
            return token.StartsWith(TokenPrefix, StringComparison.Ordinal) ? token : TokenPrefix + token;
        }

        public static void SetupToken(Language language, string token, string value)
        {
            //Log.Debug($"{token}   {value}");
            language.stringsByToken[Token(token)] = value;
            if (token.StartsWith("NAME_", StringComparison.Ordinal) || token.StartsWith("DESCRIPTION_", StringComparison.Ordinal))
            {
                language.stringsByToken[Token("BUFF_" + token)] = value;
            }
        }

        public static bool HasToken(string token)
        {
            string fullToken = Token(token);
            return Language.currentLanguage?.stringsByToken.ContainsKey(fullToken) == true || TryGetFileString(fullToken, out _);
        }

        public static string GetString(string token, string fallback = "")
        {
            string fullToken = Token(token);
            if (Language.currentLanguage?.stringsByToken.TryGetValue(fullToken, out string currentValue) == true)
            {
                return currentValue;
            }

            if (TryGetFileString(fullToken, out string fileValue))
            {
                return fileValue;
            }

            string languageValue = Language.GetString(fullToken);
            if (!string.IsNullOrEmpty(languageValue) && languageValue != fullToken)
            {
                return languageValue;
            }

            return fallback;
        }

        public static string Format(string token, string fallback, params object[] args)
        {
            return string.Format(GetString(token, fallback), args);
        }

        public static string GetStringForLanguage(string languageName, string token, string fallback = "")
        {
            string fullToken = Token(token);
            return TryGetFileString(languageName, fullToken, out string fileValue) ? fileValue : fallback;
        }

        public static bool IsKnownLocalizedString(string token, string fallback, string value)
        {
            if (string.Equals(value, fallback, StringComparison.Ordinal))
            {
                return true;
            }

            string fullToken = Token(token);
            if (Language.currentLanguage?.stringsByToken.TryGetValue(fullToken, out string currentValue) == true &&
                string.Equals(value, currentValue, StringComparison.Ordinal))
            {
                return true;
            }

            if (string.IsNullOrEmpty(languageRootPath) || !Directory.Exists(languageRootPath))
            {
                return false;
            }

            foreach (string languageDirectory in Directory.GetDirectories(languageRootPath))
            {
                string languageName = System.IO.Path.GetFileName(languageDirectory);
                if (TryGetFileString(languageName, fullToken, out string fileValue) &&
                    string.Equals(value, fileValue, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetItemStatLabel(string defaultLabel)
        {
            return GetString(GetItemStatToken(defaultLabel), defaultLabel);
        }

        public static string GetItemStatToken(string defaultLabel)
        {
            string withoutTags = tagsRegex.Replace(defaultLabel, string.Empty).Trim().TrimEnd(':').Trim();
            string normalized = tokenCharsRegex.Replace(withoutTags.ToUpperInvariant(), "_").Trim('_');
            return "ITEMSTAT_" + normalized;
        }

        public static string ConfigName(string key)
        {
            return GetString("CONFIG_" + ToTokenSegment(key) + "_NAME", key);
        }

        public static string ConfigDescription(string key, string fallback)
        {
            return GetString("CONFIG_" + ToTokenSegment(key) + "_DESCRIPTION", fallback);
        }

        public static string ConfigCategory(string category)
        {
            return GetString("CONFIG_CATEGORY_" + ToTokenSegment(category), category);
        }

        public static string ToTokenSegment(string value)
        {
            string withoutTags = tagsRegex.Replace(value ?? string.Empty, string.Empty).Trim();
            return tokenCharsRegex.Replace(withoutTags.ToUpperInvariant(), "_").Trim('_');
        }

        static bool TryGetFileString(string fullToken, out string value)
        {
            value = null;
            string currentLanguageName = Language.currentLanguageName;
            if (!string.IsNullOrEmpty(currentLanguageName) && TryGetFileString(currentLanguageName, fullToken, out value))
            {
                return true;
            }

            return TryGetFileString("en", fullToken, out value);
        }

        static bool TryGetFileString(string languageName, string fullToken, out string value)
        {
            value = null;
            languageName = NormalizeLanguageName(languageName);
            if (string.IsNullOrEmpty(languageRootPath))
            {
                return false;
            }

            if (!cachedLanguageFiles.TryGetValue(languageName, out Dictionary<string, string> strings))
            {
                string path = System.IO.Path.Combine(languageRootPath, languageName, LanguageFile);
                if (!File.Exists(path))
                {
                    cachedLanguageFiles[languageName] = null;
                    return false;
                }

                try
                {
                    strings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path, Encoding.UTF8));
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    strings = null;
                }
                cachedLanguageFiles[languageName] = strings;
            }

            return strings?.TryGetValue(fullToken, out value) == true;
        }

        static string NormalizeLanguageName(string languageName)
        {
            if (string.IsNullOrEmpty(languageName))
            {
                return languageName;
            }

            return languageAliases.TryGetValue(languageName, out string alias) ? alias : languageName;
        }
    }
}
