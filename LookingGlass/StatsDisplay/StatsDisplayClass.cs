using BepInEx.Configuration;
using LookingGlass.Base;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using System.Text.RegularExpressions;
using RoR2.UI;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using TMPro;

namespace LookingGlass.StatsDisplay
{
    internal class StatsDisplayClass : BaseThing
    {
        public static ConfigEntry<bool> statsDisplay;
        public static ConfigEntry<string> statsDisplayString;
        public static ConfigEntry<float> statsDisplaySize;
        public static ConfigEntry<float> statsDisplayUpdateInterval;
        public static ConfigEntry<bool> builtInColors;
        public static Dictionary<string, Func<CharacterBody, string>> statDictionary = new Dictionary<string, Func<CharacterBody, string>>();
        internal static CharacterBody cachedUserBody = null;
        Transform statTracker = null;
        TextMeshProUGUI textComponent;
        LayoutElement layoutElement;
        public StatsDisplayClass()
        {
            Setup();
            SetupRiskOfOptions();
        }

        public void Setup()
        {
            statsDisplay = BasePlugin.instance.Config.Bind<bool>("Stats Display", "StatsDisplay", true, "Enables Stats Display");
            statsDisplayString = BasePlugin.instance.Config.Bind<string>("Stats Display", "Stats Display String",
                "Stats\n" +
                "Luck: [luck]\n" +
                "Damage: [baseDamage]\n" +
                "Crit Chance: [critWithLuck]\n" +
                "Attack Speed: [attackSpeed]\n" +
                "Armor: [armor] | [armorDamageReduction]\n" +
                "Regen: [regen]\n" +
                "Speed: [speed]\n" +
                "Jumps: [availableJumps]/[maxJumps]\n" +
                "Kills: [killCount]\n" +
                "Mountain Shrines: [mountainShrines]\n" +
                "DPS: [dps]\n" +
                "Combo: [currentCombatDamage]\n" +
                "Combo Timer: [remainingComboDuration]\n" +
                "Max Combo: [maxCombo]"
                , "String for the stats display. You can customize this with Unity Rich Text if you want, see \n https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/RichText.html for more info. \nAvailable syntax for the [] stuff is: \nluck \n baseDamage \n crit \n attackSpeed \n armor \n armorDamageReduction \n regen \n speed \n availableJumps \n maxJumps \n killCount \n mountainShrines \n experience \n level \n maxHealth \n maxBarrier \n barrierDecayRate \n maxShield \n acceleration \n jumpPower \n maxJumpHeight \n damage \n critMultiplier \n bleedChance \n visionDistance \n critHeal \n cursePenalty \n hasOneShotProtection \n isGlass \n canPerformBackstab \n canReceiveBackstab \n healthPercentage \n goldPortal \n msPortal \n shopPortal \n dps \n currentCombatDamage \n remainingComboDuration \n maxCombo \n critWithLuck \n bleedChanceWithLuck \n velocity \n teddyBearBlockChance \n saferSpacesCD \n instaKillChance");
            statsDisplaySize = BasePlugin.instance.Config.Bind<float>("Stats Display", "StatsDisplay font size", -1, "General font size of the stats display menu. If set to -1, will copy the font size from the objective panel");
            statsDisplayUpdateInterval = BasePlugin.instance.Config.Bind<float>("Stats Display", "StatsDisplay update interval", 0.33f, "The interval at which stats display updates, in seconds. Lower values will increase responsiveness, but may potentially affect performance");
            builtInColors = BasePlugin.instance.Config.Bind<bool>("Stats Display", "Use default colors", true, "Uses the default styling for stats display syntax items.");
            builtInColors.SettingChanged += BuiltInColors_SettingChanged;
            StatsDisplayDefinitions.SetupDefs();
        }

        private void BuiltInColors_SettingChanged(object sender, EventArgs e)
        {
            StatsDisplayDefinitions.SetupDefs();
        }

        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(statsDisplay, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new StringInputFieldOption(statsDisplayString, new InputFieldConfig() { restartRequired = false, lineType = TMP_InputField.LineType.MultiLineNewline, submitOn = InputFieldConfig.SubmitEnum.All }));
            ModSettingsManager.AddOption(new SliderOption(statsDisplaySize, new SliderConfig() { restartRequired = false, min = -1, max = 100, formatString = "{0:F0}" }));
            ModSettingsManager.AddOption(new CheckBoxOption(builtInColors, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new SliderOption(statsDisplayUpdateInterval, new SliderConfig() { restartRequired = false, min = 0.05f, max = 1f, formatString = "{0:F2}s" }));


        }
        bool isRiskUI = false;
        float originalFontSize = -1;
        VerticalLayoutGroup layoutGroup;
        public void CalculateStuff()
        {
            if (!statsDisplay.Value)
                return;
            if (!cachedUserBody)
            {
                try
                {
                    cachedUserBody = LocalUserManager.GetFirstLocalUser().cachedBody;
                }
                catch (Exception)
                {
                }
            }
            if (cachedUserBody)
            {
                string stats = statsDisplayString.Value;
                foreach (var item in statDictionary.Keys)
                {
                    stats = Regex.Replace(stats, $@"(?<!\\)\[{item}\]", statDictionary[item](cachedUserBody));
                }
                if (!statTracker)
                {
                    foreach (var item in RoR2.Run.instance.uiInstance.GetComponentsInChildren<VerticalLayoutGroup>())
                    {
                        if (item.gameObject.name == "RightInfoBar")
                        {
                            GameObject g = GameObject.Instantiate(item.transform.Find("ObjectivePanel").gameObject);
                            g.transform.parent = item.transform.Find("ObjectivePanel").parent.transform;
                            g.name = "PlayerStats";

                            if (g.transform.Find("StripContainer"))
                                GameObject.Destroy(g.transform.Find("StripContainer").gameObject);
                            if (g.GetComponent<HudObjectiveTargetSetter>())
                                UnityEngine.Object.Destroy(g.GetComponent<HudObjectiveTargetSetter>());
                            if (g.GetComponent<ObjectivePanelController>())
                                UnityEngine.Object.Destroy(g.GetComponent<ObjectivePanelController>());

                            RectTransform r = g.GetComponent<RectTransform>();
                            r.localPosition = Vector3.zero;
                            r.localEulerAngles = Vector3.zero;
                            r.localScale = Vector3.one;
                            layoutGroup = g.GetComponent<VerticalLayoutGroup>();
                            statTracker = g.transform;
                            layoutGroup.enabled = false;
                            layoutGroup.enabled = true;
                            textComponent = g.GetComponentInChildren<TextMeshProUGUI>();
                            textComponent.alignment = TMPro.TextAlignmentOptions.TopLeft;
                            textComponent.color = Color.white;
                            layoutElement = g.GetComponentInChildren<LayoutElement>();

                            if (g.transform.Find("Seperator"))
                            {
                                isRiskUI = true;
                                VerticalLayoutGroup v = g.transform.Find("Seperator").GetComponent<VerticalLayoutGroup>();
                                v.padding.top = 0;
                                v.childAlignment = TextAnchor.UpperLeft;
                            }
                            break;
                        }
                    }
                }
                if (textComponent && layoutElement)
                {
                    textComponent.text = stats;
                    int num = stats.Split('\n').Length;
                    if (originalFontSize == -1)
                    {
                        originalFontSize = textComponent.fontSize;
                    }
                    textComponent.fontSize = statsDisplaySize.Value == -1 ? originalFontSize : statsDisplaySize.Value;
                    layoutElement.preferredHeight = textComponent.fontSize * (num + 1);
                    if (isRiskUI && layoutGroup)
                    {
                        layoutGroup.padding.bottom = (int)((num / 16f) * 50);
                    }
                }
                //Log.Debug(stats);
            }
        }
    }
}
