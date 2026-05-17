using BepInEx.Configuration;
using Newtonsoft.Json;
using RiskOfOptions;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
        static readonly BindingFlags riskOfOptionsFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

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

        public static void RefreshRiskOfOptionsTokens()
        {
            try
            {
                FieldInfo optionCollectionField = typeof(ModSettingsManager).GetField("OptionCollection", riskOfOptionsFlags);
                object optionCollections = optionCollectionField?.GetValue(null);
                MethodInfo addLanguageEntry = typeof(ModSettingsManager).Assembly
                    .GetType("RiskOfOptions.Lib.LanguageApi")
                    ?.GetMethod("Add", riskOfOptionsFlags);
                if (optionCollections is not IEnumerable enumerable || addLanguageEntry == null)
                {
                    return;
                }

                foreach (object optionCollection in enumerable)
                {
                    if (!string.Equals(GetStringProperty(optionCollection, "ModGuid"), global::LookingGlass.PluginInfo.PLUGIN_GUID, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    int categoryCount = GetIntProperty(optionCollection, "CategoryCount");
                    for (int i = 0; i < categoryCount; i++)
                    {
                        RefreshRiskOfOptionsCategory(GetIndexedValue(optionCollection, i), addLanguageEntry);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        static void RefreshRiskOfOptionsCategory(object category, MethodInfo addLanguageEntry)
        {
            if (category == null)
            {
                return;
            }

            string localizedCategory = null;
            int optionCount = GetIntProperty(category, "OptionCount");
            for (int i = 0; i < optionCount; i++)
            {
                object option = GetIndexedValue(category, i);
                ConfigEntryBase configEntry = GetConfigEntry(option);
                if (configEntry == null)
                {
                    continue;
                }

                localizedCategory ??= ConfigCategory(configEntry.Definition.Section);
                AddRiskOfOptionsLanguageEntry(addLanguageEntry, InvokeStringMethod(option, "GetNameToken"), ConfigName(configEntry.Definition.Key));
                AddRiskOfOptionsLanguageEntry(addLanguageEntry, InvokeStringMethod(option, "GetDescriptionToken"), ConfigDescription(configEntry.Definition.Key, configEntry.Description.Description));
            }

            localizedCategory ??= ConfigCategory(GetStringField(category, "name"));
            AddRiskOfOptionsLanguageEntry(addLanguageEntry, GetStringProperty(category, "NameToken"), localizedCategory);
        }

        static ConfigEntryBase GetConfigEntry(object option)
        {
            return option?.GetType().GetProperty("ConfigEntry", riskOfOptionsFlags)?.GetValue(option) as ConfigEntryBase;
        }

        static object GetIndexedValue(object target, int index)
        {
            return target?.GetType().GetProperty("Item", riskOfOptionsFlags, null, null, new[] { typeof(int) }, null)?.GetValue(target, new object[] { index });
        }

        static string GetStringProperty(object target, string name)
        {
            return target?.GetType().GetProperty(name, riskOfOptionsFlags)?.GetValue(target) as string;
        }

        static string GetStringField(object target, string name)
        {
            return target?.GetType().GetField(name, riskOfOptionsFlags)?.GetValue(target) as string;
        }

        static int GetIntProperty(object target, string name)
        {
            return target?.GetType().GetProperty(name, riskOfOptionsFlags)?.GetValue(target) as int? ?? 0;
        }

        static string InvokeStringMethod(object target, string name)
        {
            return target?.GetType().GetMethod(name, riskOfOptionsFlags)?.Invoke(target, Array.Empty<object>()) as string;
        }

        static void AddRiskOfOptionsLanguageEntry(MethodInfo addLanguageEntry, string token, string value)
        {
            if (!string.IsNullOrEmpty(token))
            {
                addLanguageEntry.Invoke(null, new object[] { token, value ?? string.Empty });
            }
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
