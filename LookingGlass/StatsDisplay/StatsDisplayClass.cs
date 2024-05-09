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
                , "String for the stats display");
            StatsDisplayDefinitions.SetupDefs();
        }

        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(statsDisplay, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new StringInputFieldOption(statsDisplayString, new InputFieldConfig() { restartRequired = false, lineType = TMP_InputField.LineType.MultiLineNewline, submitOn = InputFieldConfig.SubmitEnum.All }));

        }

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
                            VerticalLayoutGroup layoutGroup = g.GetComponent<VerticalLayoutGroup>();
                            statTracker = g.transform;
                            layoutGroup.enabled = false;
                            layoutGroup.enabled = true;
                            textComponent = g.GetComponentInChildren<TextMeshProUGUI>();
                            textComponent.alignment = TMPro.TextAlignmentOptions.TopLeft;
                            textComponent.color = Color.white;
                            layoutElement = g.GetComponentInChildren<LayoutElement>();

                            if (g.transform.Find("Seperator"))
                            {
                                g.transform.Find("Seperator").GetComponent<VerticalLayoutGroup>().padding.top = -100;
                            }
                            break;
                        }
                    }
                }
                if (textComponent && layoutElement)
                {
                    textComponent.text = stats;
                    layoutElement.preferredHeight = textComponent.fontSize * (stats.Split('\n').Length + 1);
                }
                //Log.Debug(stats);
            }
        }
    }
}
