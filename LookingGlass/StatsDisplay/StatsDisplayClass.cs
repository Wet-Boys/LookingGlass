#if DEBUG
#define ENABLE_PROFILER // enables Profiler.BeginSample
#endif
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
using MonoMod.RuntimeDetour;
using System.Collections;
using Unity.Jobs;
using UnityEngine.AddressableAssets;
using UnityEngine.Profiling;
using RiskOfOptions.Components.Options;

namespace LookingGlass.StatsDisplay
{
    internal class StatsDisplayClass : BaseThing
    {
        public enum StatDisplayPreset
        {
            Set,            //
            LookingGlass,   //
            Simpler,        //No DPS/Combo 
            Extra,          //CritDamage,Luck,Curse%, Osp
            Minimal,        //
            Classic,            //BetterUI-like
            //AddLineHeight,
        }
        public static ConfigEntry<StatDisplayPreset> statStringPresets;
        public static ConfigEntry<bool> movePurchaseText; //-240x
        public static ConfigEntry<bool> checkIfOldDefaultSettings;

        public static ConfigEntry<bool> statsDisplay;
        public static ConfigEntry<bool> useSecondaryStatsDisplay;
        public static ConfigEntry<string> secondaryStatsDisplayString;
        public static ConfigEntry<string> statsDisplayString;
        public static ConfigEntry<float> statsDisplaySize;
        public static ConfigEntry<float> statsDisplayUpdateInterval;
        public static ConfigEntry<bool> builtInColors;
        public static ConfigEntry<bool> statsDisplayOverrideHeight;
        public static ConfigEntry<int> statsDisplayOverrideHeightValue;
        public static ConfigEntry<int> floatPrecision;
        public static ConfigEntry<bool> statsDisplayAttached;
        public static ConfigEntry<Vector2> detachedPosition;
        public static Dictionary<string, Func<CharacterBody, string>> statDictionary = new Dictionary<string, Func<CharacterBody, string>>();
        internal static CharacterBody cachedUserBody = null;
        //internal static CharacterBody cachedUserMaster = null;
        Transform statTracker = null;
        TextMeshProUGUI textComponent;
        GameObject textComponentGameObject;
        LayoutElement layoutElement;
        Image cachedImage;
        private static Hook overrideHook;
        private static Hook overrideHook2;
        static bool scoreBoardOpen;
        static readonly Regex statsRegex = new(@"(?<!\\)\[(\w+)\]", RegexOptions.Compiled);
        static JobHandle regexHandle;
        // using timer as updating canvas with large changed strings is not cheap
        float timer;

        public StatsDisplayClass()
        {
            Setup();
            SetupRiskOfOptions();
        }

        const string syntaxList =
            "\n\n damage "
            + "\n attackSpeed, attackSpeedPercent "
            + "\n crit, critWithLuck, critMultiplier"
            + "\n bleedChance, bleedChanceWithLuck"

            + "\n maxHealth, maxShield, maxBarrier "
            + "\n effectiveHealth, effectiveMaxHealth"
            + "\n barrierDecayRate" //Static in vanilla so eh?
            + "\n healthPercentage"
            + "\n regen, regenHp"
            + "\n armor, armorDamageReduction"
            + "\n curseHealthReduction "
            + "\n hasOSP "

            + "\n speed, speedPercent, velocity"
            + "\n acceleration "
            + "\n availableJumps, maxJumps"
            + "\n jumpPower, maxJumpHeight"
            + "\n level, experience "
            + "\n luck "

            + "\n mountainShrines "
            + "\n shopPortal, goldPortal, msPortal, voidPortal, greenPortal"
            + "\n killCount, killCountRun "
            + "\n dps, percentDps "
            + "\n combo, maxComboThisRun, maxCombo "
            + "\n killCombo, maxKillComboThisRun, maxKillCombo"
            + "\n remainingComboDuration"
            + "\n teddyBearBlockChance, saferSpacesCD "
            + "\n instaKillChance "
            + "\n difficultyCoefficient, stage";
        public void Setup()
        {
            statsDisplay = BasePlugin.instance.Config.Bind<bool>("Stats Display", "StatsDisplay", true, "Enables Stats Display");
            statsDisplay.SettingChanged += Display_SettingChanged;
            statsDisplayString = BasePlugin.instance.Config.Bind<string>("Stats Display", "Stats Display String",
                //Removing Combo timer just cuz
                //Removing Luck because not important enough to be primary
                //Mountain Shrine Secondary only
                //MaxCombo -> MaxCombo Per Run
                //MaxCombo -> Seconary Only
                "<margin-left=0.6em>"
                + "<size=115%>Stats</size>\n"
                + "Damage: [damage]\n"
                + "Attack Speed: [attackSpeed]\n"
                + "Crit Chance: [critWithLuck]\n"
                + "Regen: [regen]\n"
                + "Armor: [armor] | [armorDamageReduction]\n"
                + "Speed: [speed]\n"
                + "Jumps: [availableJumps] / [maxJumps]\n"
                + "Kills: [killCount]\n"
                + "DPS: [dps]\n"
                + "Combo: [combo]\n"
                + "</margin>"
                , $"String for the stats display. You can customize this with Unity Rich Text if you want, see \n https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/RichText.html for more info. \nAvailable syntax for the [] stuff is:{syntaxList}");
            statsDisplaySize = BasePlugin.instance.Config.Bind<float>("Stats Display", "Stats Display font size", -1, "General font size of the stats display menu. If set to -1, will copy the font size from the objective panel Header. Objective Header is font size 16, Objectives are font size 12 for reference.");

            statsDisplayUpdateInterval = BasePlugin.instance.Config.Bind<float>("Stats Display", "StatsDisplay update interval", 0.2f, "The interval at which stats display updates, in seconds. Lower values will increase responsiveness, but may potentially affect performance for large texts");
            statsDisplayUpdateInterval.SettingChanged += Display_SettingChanged;
            builtInColors = BasePlugin.instance.Config.Bind<bool>("Stats Display", "Use default colors", true, "Uses the default styling for stats display syntax items.");
            builtInColors.SettingChanged += BuiltInColors_SettingChanged;
            statsDisplayOverrideHeight = BasePlugin.instance.Config.Bind<bool>("Stats Display", "Override Stats Display Height", false, "Sets a user-specified height for Stats Display (may be necessary if you get particularly creative with formatting)");
            statsDisplayOverrideHeightValue = BasePlugin.instance.Config.Bind<int>("Stats Display", "Stats Display Height Value", 7, "Height, in lines of full-size text, for the Stats Display panel");
            floatPrecision = BasePlugin.instance.Config.Bind<int>("Stats Display", "StatsDisplay Float Precision", 2, "How many decimal points will be used in floating point values");
            floatPrecision.SettingChanged += BuiltInColors_SettingChanged;
            useSecondaryStatsDisplay = BasePlugin.instance.Config.Bind<bool>("Stats Display", "Use Secondary Stats Display", true, "Will enable the use of the secondary stats display string. This will overwrite the stats display string whenever the scoreboard is held open.");
            secondaryStatsDisplayString = BasePlugin.instance.Config.Bind<string>("Stats Display", "Secondary Stats Display String",
                "<margin-left=0.6em>"
                + "<size=115%>Stats</size>\n"
                + "Damage: [damage]\n"
                + "Attack Speed: [attackSpeed]\n" //AS first because Crit expands into Crit + Crit Damage + Bleed
                + "Crit Chance: [critWithLuck]\n"
                //+ "Crit Multiplier: [critMultiplier]\n" //DLC3 is adding a Crit Damage item so maybe
                + "Bleed Chance: [bleedChanceWithLuck]\n"
                + "Regen: [regen]\n"
                + "Armor: [armor] | [armorDamageReduction]\n"              
                + "Speed: [speed]\n"
                + "Jumps: [availableJumps] / [maxJumps]\n"
                //+ "Luck: [luck]\n" //If any mods/DLCs add Luck items maybe worth having on default secondary
                + "Total Kills: [killCountRun]\n" //Kills Primary -> Run Kills Secondary
                + "Max Combo: [maxComboThisRun]\n" //Combo Primary -> Run Combo Secondary
                + "Mountain Shrines: [mountainShrines]\n"
                + "<size=115%>Portals:</size> \n"
                + "<size=67%>Bazaar: [shopPortal] Storm: [greenPortal] Gold: [goldPortal]</size>" //Keeping Post-Loop portals with no influence out of it
                + "</margin>"
                , $"Secondary string for the stats display. You can customize this with Unity Rich Text if you want, see \n https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/RichText.html for more info. \nAvailable syntax for the [] stuff is: {syntaxList}");
            StatsDisplayDefinitions.SetupDefs();

            // position override
            Vector2 defaultPos = new Vector2(1810, 1015);
            statsDisplayAttached = BasePlugin.instance.Config.Bind("Stats Display", "Attach To Objective Panel", true,
                "If enabled, will be attached to below the objective panel, otherwise position can be configured");
            detachedPosition = BasePlugin.instance.Config.Bind("Stats Display", "Stats Display Position", defaultPos,
                $"Position of detached Stats Display.\n[Default: {defaultPos.x:f0}, {defaultPos.y:f0}]");

            statsDisplayAttached.SettingChanged += Display_SettingChanged;
            detachedPosition.SettingChanged += DetachedPosition_SettingChanged;

            var targetMethod = typeof(ScoreboardController).GetMethod(nameof(ScoreboardController.OnEnable), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(StatsDisplayClass).GetMethod(nameof(OnEnable), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
            targetMethod = typeof(ScoreboardController).GetMethod(nameof(ScoreboardController.OnDisable), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(StatsDisplayClass).GetMethod(nameof(OnDisable), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook2 = new Hook(targetMethod, destMethod, this);



            statStringPresets = BasePlugin.instance.Config.Bind<StatDisplayPreset>("Stats Display", "Stats Display Preset", (StatDisplayPreset)100, "Override current Stat Display settings with a premade preset,\nfurther changes can made from there.\n\n" +
                "Extra: Include Crit Damage, Luck, CurseFrac on Tab\n\n" +
                "Simpler: Dont include DPS, Combo\n\n" +
                "Minimal: DPS + Jump, Few stats on Tab for mathing or remembering.\n\n");

            statStringPresets.SettingChanged += ApplyPresets;


            movePurchaseText = BasePlugin.instance.Config.Bind<bool>("Stats Display", "Move Purchase Text", true, "Move purchase text further to the left to avoid clipping with larger Stat Displays or generally fuller RightSideInfos.");
            movePurchaseText.SettingChanged += MovePurchase;




            checkIfOldDefaultSettings = BasePlugin.instance.Config.Bind<bool>("Misc", "Check For Old Default Settings1", true, "Override the stat display with the updated one, if you were using the default one prior to updating.\nNot meant as a config just needs to be tracked.");
            //Run in case some guy doesnt use RiskOfOptions and to check if Old?
            ApplyPresets(null, null);
            if (checkIfOldDefaultSettings.Value)
            {
                CheckOldDefaultStatDisplayStrings();
            }
            
        }


        public void SetupRiskOfOptions()
        {

            ModSettingsManager.AddOption(new CheckBoxOption(statsDisplay, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new ChoiceOption(statStringPresets, false));
            ModSettingsManager.AddOption(new StringInputFieldOption(statsDisplayString, new InputFieldConfig() { restartRequired = false, lineType = TMP_InputField.LineType.MultiLineNewline, submitOn = InputFieldConfig.SubmitEnum.OnExitOrSubmit, richText = false }));
            ModSettingsManager.AddOption(new SliderOption(statsDisplaySize, new SliderConfig() { restartRequired = false, min = -1, max = 100 }));
            ModSettingsManager.AddOption(new CheckBoxOption(useSecondaryStatsDisplay, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new StringInputFieldOption(secondaryStatsDisplayString, new InputFieldConfig() { restartRequired = false, lineType = TMP_InputField.LineType.MultiLineNewline, submitOn = InputFieldConfig.SubmitEnum.OnExitOrSubmit, richText = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(movePurchaseText, new CheckBoxConfig() { restartRequired = false }));

            ModSettingsManager.AddOption(new CheckBoxOption(builtInColors, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new SliderOption(statsDisplayUpdateInterval, new SliderConfig() { restartRequired = false, min = 0.01f, max = 1f, formatString = "{0:F2}s" }));
            ModSettingsManager.AddOption(new CheckBoxOption(statsDisplayOverrideHeight, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new IntSliderOption(statsDisplayOverrideHeightValue, new IntSliderConfig() { restartRequired = false, min = 0, max = 100 }));
            ModSettingsManager.AddOption(new IntSliderOption(floatPrecision, new IntSliderConfig() { restartRequired = false, min = 0, max = 5 }));


            // position override
            ModSettingsManager.AddOption(new CheckBoxOption(statsDisplayAttached, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new GenericButtonOption(
                detachedPosition.Definition.Key,
                detachedPosition.Definition.Section,
                detachedPosition.Description.Description,
                "Open",
                () => CreatePositionWindow(detachedPosition)
            ));


        }



        void CheckOldDefaultStatDisplayStrings()
        {
            //Check once upon updating if they were using old default stat string.
            //So it keeps any custom ones, but people who are using default dont have to manually reset

            #region BetterUI-like 
            if ((string)statsDisplayString.Value == 
                        "<size=120%>Stats</size>\n"
                        + "Luck: [luck]\n"
                        + "Damage: [damage]\n"
                        + "Crit Chance: [critWithLuck]\n"
                        + "Attack Speed: [attackSpeed]\n"
                        + "Armor: [armor] | [armorDamageReduction]\n"
                        + "Regen: [regen]\n"
                        + "Speed: [speed]\n"
                        + "Jumps: [availableJumps]/[maxJumps]\n"
                        + "Kills: [killCount]\n"
                        + "Mountain Shrines: [mountainShrines]\n"
                        + "DPS: [dps]\n"
                        + "Combo: [combo]\n"
                        + "Combo Timer: [remainingComboDuration]\n"
                        + "Max Combo: [maxComboThisRun]"
                     )
            {
                Debug.Log("Old1 detected");
                statsDisplayString.Value = (string)statsDisplayString.DefaultValue;
            }

            if ((string)secondaryStatsDisplayString.Value == 
                   "<size=120%>Stats</size>\n"
                  + "Luck: [luck]\n"
                  + "Damage: [damage]\n"
                  + "Crit Chance: [critWithLuck]\n"
                  + "Bleed Chance: [bleedChanceWithLuck]\n"
                  + "Attack Speed: [attackSpeed]\n"
                  + "Armor: [armor] | [armorDamageReduction]\n"
                  + "Regen: [regen]\n"
                  + "Speed: [speed]\n"
                  + "Jumps: [availableJumps]/[maxJumps]\n"
                  + "Kills: [killCount]\n"
                  + "Mountain Shrines: [mountainShrines]\n"
                  + "Max Combo: [maxComboThisRun]\n"
                  + "<size=120%>Portals:</size> \n"
                  + "<size=50%>Gold:[goldPortal] Shop:[shopPortal] Celestial:[msPortal] Void:[voidPortal]</size>"
                  )
            {
                Debug.Log("Old2 detected");
                secondaryStatsDisplayString.Value = (string)secondaryStatsDisplayString.DefaultValue;
            }
            #endregion
            checkIfOldDefaultSettings.Value = false;
        }

        void ApplyPresets(object sender, EventArgs e)
        {
            if (statStringPresets.Value == StatDisplayPreset.Set)
            {
                return;
            }

            string new1 = statsDisplayString.Value;
            string new2 = secondaryStatsDisplayString.Value;
            switch (statStringPresets.Value)
            {
                case StatDisplayPreset.Classic:
                    //Do not modify
                    new1 =
                        "<size=120%>Stats</size>\n"
                        + "Luck: [luck]\n"
                        + "Damage: [damage]\n"
                        + "Crit Chance: [critWithLuck]\n"
                        + "Attack Speed: [attackSpeed]\n"
                        + "Armor: [armor] | [armorDamageReduction]\n"
                        + "Regen: [regen]\n"
                        + "Speed: [speed]\n"
                        + "Jumps: [availableJumps]/[maxJumps]\n"
                        + "Kills: [killCount]\n"
                        + "Mountain Shrines: [mountainShrines]\n"
                        + "DPS: [dps]\n"
                        + "Combo: [combo]\n"
                        + "Combo Timer: [remainingComboDuration]\n"
                        + "Max Combo: [maxComboThisRun]";
                    new2 =
                          "<size=120%>Stats</size>\n"
                          + "Luck: [luck]\n"
                          + "Damage: [damage]\n"
                          + "Crit Chance: [critWithLuck]\n"
                          + "Bleed Chance: [bleedChanceWithLuck]\n"
                          + "Attack Speed: [attackSpeed]\n"
                          + "Armor: [armor] | [armorDamageReduction]\n"
                          + "Regen: [regen]\n"
                          + "Speed: [speed]\n"
                          + "Jumps: [availableJumps]/[maxJumps]\n"
                          + "Kills: [killCount]\n"
                          + "Mountain Shrines: [mountainShrines]\n"
                          + "Max Combo: [maxComboThisRun]\n"
                          + "<size=120%>Portals:</size> \n"
                          + "<size=50%>Gold:[goldPortal] Shop:[shopPortal] Celestial:[msPortal] Void:[voidPortal]</size>";
                    break;
                case StatDisplayPreset.LookingGlass:
                    new1 = (string)statsDisplayString.DefaultValue;
                    new2 = (string)secondaryStatsDisplayString.DefaultValue;
                    break;
                case StatDisplayPreset.Extra:
                    //+Luck
                    //+Crit Damage Mult
                    //+Curse HP Reduction
                    //+Has OSP rn ig
                    //AS / MS % for idk nerds
                    new1 = (string)statsDisplayString.DefaultValue;
                    new2 =
                        "<margin-left=0.6em>"
                        + "<size=115%>Stats</size>\n"
                        + "Damage: [damage]\n"
                        + "Attack Speed: [attackSpeedPercent]\n"
                        + "Crit Stats: [critWithLuck] | [critMultiplier]\n"
                        //+ "Crit Chance: [critWithLuck]\n"
                        //+ "Crit Multiplier: [critMultiplier]\n"
                        + "Bleed Chance: [bleedChanceWithLuck]\n"
                        + "Regen: [regenHp]\n"
                        + "Armor: [armor] | [armorDamageReduction]\n"
                        + "Osp: [hasOneShotProtection]\n"
                        + "Speed: [speedPercent]\n"
                        + "Jumps: [availableJumps] / [maxJumps]\n"
                        + "Luck: [luck]\n"
                        + "Curse Penalty: [curseHealthReduction]\n"
                        + "Total Kills: [killCountRun]\n"
                        + "Max Combo: [maxComboThisRun]\n"                     
                        + "Mountain Shrines: [mountainShrines]\n"
                        + "<size=115%>Portals:</size> \n"
                        + "<size=67%>"
                        + "Bazaar: [shopPortal] Storm: [greenPortal] Gold: [goldPortal]\n"
                        + "Celestial: [msPortal] Void: [voidPortal]</size>"
                        + "</margin>";
                    break;
                case StatDisplayPreset.Simpler:
                    //No Combo or DPS stuff
                    //
                    new1 =
                         "<margin-left=0.6em><line-height=110%>"
                         + "<align=center><size=115%>Stats:</align></size>\n"
                         + "Damage: [damage]\n"
                         + "Attack Speed: [attackSpeed]\n"
                         + "Crit Chance: [critWithLuck]\n"
                         + "Regen: [regen]\n" 
                         + "Armor: [armor] | [armorDamageReduction]\n"
                         + "Speed: [speed]\n"
                         + "Jumps: [availableJumps] / [maxJumps]\n"
                         + "Kills: [killCount]\n"
                         + "</line-height></margin>";
                    new2 =
                         "<margin-left=0.6em><line-height=110%>"
                         + "<align=center><size=115%>Stats:</align></size>\n"
                         + "Damage: [damage]\n"
                         + "Attack Speed: [attackSpeed]\n"
                         + "Crit Chance: [critWithLuck]\n"
                         + "Bleed Chance: [bleedChanceWithLuck]\n"
                         + "Regen: [regen]\n" 
                         + "Armor: [armor] | [armorDamageReduction]\n"
                         + "Speed: [speed]\n"
                         + "Jumps: [availableJumps] / [maxJumps]\n"
                         + "Kills: [killCount]\n"
                         + "Mountain Shrines: [mountainShrines]\n"
                         + "Bazaar Portal: [shopPortal]\n"
                         + "</line-height></margin>";
                    break;
                case StatDisplayPreset.Minimal:
                    new1 =
                        "<margin-left=0.6em><line-height=110%>"
                        + "Jumps: [availableJumps] / [maxJumps]\n" 
                        + "DPS: [dps] | [percentDps]\n"
                        + "</line-height></margin>";
                    new2 =
                          "<margin-left=0.6em><line-height=110%>"
                          + "Crit Chance: [critWithLuck]\n"
                          + "Bleed Chance: [bleedChanceWithLuck]\n"
                          + "Mountain Shrines: [mountainShrines]\n"
                          + "Bazaar Portal: [shopPortal]"
                          + "</line-height></margin>";
                    break;
            
                //Preset to just add LineHeight?
                //Preset to just center header?
                //Preset for Margin?
            }

          
            statsDisplayString.Value = new1;
            secondaryStatsDisplayString.Value = new2;
            statStringPresets.Value = StatDisplayPreset.Set;
             

            //This is dumb as hell but somehow
            //If both the Preset & DisplayString are in the same category
            //It just doesnt work unless we input it directly
            //And as far as I can tell
            //There is no "SettingsPanel" instance
            //or "RiskOfOptions.ModOptions" instance
            //So fuck it just do this
            // GameObject option1 = GameObject.Find("/MainMenu/MENU: Settings/MainSettings/SettingsPanelTitle(Clone)/SafeArea/SubPanelArea/SettingsSubPanel, (Mod Options)/Options Panel(Clone)/Scroll View/Viewport/VerticalLayout/Mod Option Input Field, Stats Display String");
            GameObject option1 = GameObject.Find("SafeArea/SubPanelArea/SettingsSubPanel, (Mod Options)/Options Panel(Clone)/Scroll View/Viewport/VerticalLayout/Mod Option Input Field, Stats Display String");
            GameObject option2 = GameObject.Find("SafeArea/SubPanelArea/SettingsSubPanel, (Mod Options)/Options Panel(Clone)/Scroll View/Viewport/VerticalLayout/Mod Option Input Field, Secondary Stats Display String");

            if (!option1)
            {
                return;
            }
            InputFieldController inputField1 = option1.GetComponent<InputFieldController>();
            InputFieldController inputField2 = option2.GetComponent<InputFieldController>();

            inputField1.SubmitValue(new1);
            inputField2.SubmitValue(new2);

        }

        void MovePurchase(object sender, EventArgs e)
        {
           
            HUD gameHud = HUD.instancesList[0];
            if (!gameHud)
                return;
            //Move purchase text left so it doesnt clip with the stat display because thats ugly
            //X and Z never seem to be different from 0 but maybe check a custom hud?
            Transform ContextNotification = gameHud.transform.Find("MainContainer/MainUIArea/SpringCanvas/RightCluster/ContextNotification/");
            if (ContextNotification)
            {
                ContextNotification.localPosition = new Vector3(movePurchaseText.Value ? -240 : 0, ContextNotification.localPosition.y, 0);
            }
        }

        void OnEnable(Action<ScoreboardController> orig, ScoreboardController self)
        {
            scoreBoardOpen = true;
            ForceUpdate();
            orig(self);
        }
        void OnDisable(Action<ScoreboardController> orig, ScoreboardController self)
        {
            scoreBoardOpen = false;
            ForceUpdate();
            orig(self);
        }
        private void BuiltInColors_SettingChanged(object sender, EventArgs e)
        {
            StatsDisplayDefinitions.SetupDefs();
        }

        void Display_SettingChanged(object sender, EventArgs e)
        {
            // if active, recreate
            if (statTracker)
            {
                UnityEngine.Object.DestroyImmediate(statTracker.gameObject);
                ForceUpdate();
            }
        }

        void DetachedPosition_SettingChanged(object sender, EventArgs e)
        {
            // if active and not attached, move
            if (statTracker && !statsDisplayAttached.Value)
            {
                RectTransform parent = statTracker.parent as RectTransform;
                (statTracker as RectTransform).anchoredPosition3D = new Vector3(detachedPosition.Value.x, detachedPosition.Value.y, -parent.anchoredPosition3D.z);
            }
        }

  
        bool isRiskUI = false;
        float originalFontSize = -1;
        VerticalLayoutGroup layoutGroup;

        public void CalculateStuff(string statsText)
        {
            if (!statsDisplay.Value)
                return;
            if (cachedUserBody)
            {
                if (!statTracker)
                {
                    const string PanelName = "PlayerStats";
                    GameObject gameHud = Run.instance.uiInstances[0];
                    if (!gameHud)
                        return;
                    if (statsDisplayAttached.Value)
                    {
                        MovePurchase(null, null);
                        originalFontSize = -1; //Reset upon reset
                        foreach (var item in gameHud.GetComponentsInChildren<VerticalLayoutGroup>())
                        {

                            if (item.gameObject.name == "RightInfoBar")
                            {
                                Transform objectivePanel = item.transform.Find("ObjectivePanel");
                                GameObject labelObject = objectivePanel.Find("Label") ? objectivePanel.Find("Label").gameObject : null;
                                bool originalActiveState = false;
                                if (labelObject)
                                {
                                    originalActiveState = labelObject.activeSelf;
                                    labelObject.SetActive(true);
                                }
                                GameObject g = GameObject.Instantiate(objectivePanel.gameObject);
                                g.transform.parent = objectivePanel.parent.transform;
                                g.name = PanelName;
                                if (labelObject)
                                {
                                    labelObject.SetActive(originalActiveState);
                                }

                                if (g.transform.Find("StripContainer"))
                                    GameObject.Destroy(g.transform.Find("StripContainer").gameObject);
                                if (g.transform.Find("Minimap"))
                                    GameObject.DestroyImmediate(g.transform.Find("Minimap").gameObject);
                                if (g.GetComponent<HudObjectiveTargetSetter>())
                                    UnityEngine.Object.Destroy(g.GetComponent<HudObjectiveTargetSetter>());
                                if (g.GetComponent<ObjectivePanelController>())
                                    UnityEngine.Object.Destroy(g.GetComponent<ObjectivePanelController>());

                                RectTransform r = g.GetComponent<RectTransform>();
                                layoutGroup = g.GetComponent<VerticalLayoutGroup>();
                                textComponent = g.GetComponentInChildren<TextMeshProUGUI>();
                                layoutElement = g.GetComponentInChildren<LayoutElement>();

                                if (!r || !layoutGroup || !textComponent || !layoutElement)
                                {
                                    layoutGroup = null;
                                    textComponent = null;
                                    layoutElement = null;
                                    GameObject.DestroyImmediate(g);
                                    break;
                                }
                                r.localPosition = Vector3.zero;
                                r.localEulerAngles = Vector3.zero;
                                r.localScale = Vector3.one;
                                layoutGroup.enabled = false;
                                layoutGroup.enabled = true;
                                textComponent.alignment = TMPro.TextAlignmentOptions.TopLeft;
                                textComponent.color = Color.white;
                                textComponentGameObject = textComponent.gameObject;


                                //-1 is font size of 16, same as the Objective HEADER not the contents, the contents would be 12.
                                //With the extra 120% its 19.2
                                //This is the main reason why it takes up so much damn space.
                                //However 12 would be rather small, 14, the average between the two looks good tho.
                                //For other hud compat just div Header vs Objective size
                                Transform defaultStrip = objectivePanel.Find("StripContainer/ObjectiveStrip/Label");
                                if (defaultStrip)
                                {
                                    textComponent.fontSize = (labelObject.GetComponent<HGTextMeshProUGUI>().fontSize + defaultStrip.GetComponent<HGTextMeshProUGUI>().fontSize) / 2f;
                                }
                                else
                                {
                                    textComponent.fontSize = (textComponent.fontSize + 12) / 2f;
                                }
                                //Increased Padding to match Objectives spacing and general alignment
                                /*if (matchingLeftPadding.Value)
                                {
                                    layoutGroup.padding.left = 12;
                                }*/


                                if (g.transform.Find("Seperator"))
                                {
                                    isRiskUI = true;
                                    VerticalLayoutGroup v = g.transform.Find("Seperator").GetComponent<VerticalLayoutGroup>();
                                    v.padding.top = 0;
                                    v.childAlignment = TextAnchor.UpperLeft;
                                }
                                statTracker = g.transform;
                                break;
                            }
                        }
                    }
                    else // unattached panel
                    {
                        HUD hud = gameHud.GetComponentInParent<HUD>();
                        if (hud)
                        {
                            GameObject statsObjectContainer = new GameObject(PanelName);
                            RectTransform rectContainer = statsObjectContainer.AddComponent<RectTransform>();
                            RectTransform parent = hud.mainUIPanel.transform as RectTransform;
                            rectContainer.SetParent(parent, false);
                            rectContainer.anchoredPosition3D = new Vector3(detachedPosition.Value.x, detachedPosition.Value.y, -parent.anchoredPosition3D.z);
                            Vector2 anchor = -parent.anchorMin / (parent.anchorMax - parent.anchorMin);
                            rectContainer.anchorMin = anchor;
                            rectContainer.anchorMax = anchor;
                            rectContainer.sizeDelta = Vector2.zero;

                            // layouts not actually needed, but this mod checks for it so adding it
                            layoutGroup = statsObjectContainer.AddComponent<VerticalLayoutGroup>();
                            layoutElement = statsObjectContainer.AddComponent<LayoutElement>();

                            textComponent = statsObjectContainer.AddComponent<TextMeshProUGUI>();
                            textComponent.fontSize = 16;
                            textComponent.color = Color.white;
                            textComponent.enableWordWrapping = false;
                            textComponentGameObject = textComponent.gameObject;
                            statTracker = rectContainer;
                        }
                    }
                }

                if (textComponentGameObject)
                {
                    //Log.Debug($"Somebody disabled my object :(");
                    textComponentGameObject.SetActive(true);
                }
                if (textComponent && layoutElement)
                {
                    // canvas gets updated in postlateupdate if text is different
                    textComponent.text = statsText;

                    int nlines = statsDisplayOverrideHeight.Value
                        ? statsDisplayOverrideHeightValue.Value
                        : statsText.Split('\n').Length;

                    if (originalFontSize == -1)
                    {
                        originalFontSize = textComponent.fontSize;
                    }

                    textComponent.fontSize = statsDisplaySize.Value == -1 ? originalFontSize : statsDisplaySize.Value;
                    Run.instance.StartCoroutine(FixScaleAfterFrame(nlines));
                    if (statsDisplayAttached.Value)
                    {
                        if (!cachedImage)
                        {
                            cachedImage = layoutElement.transform.parent.GetComponent<Image>();
                        }
                        if (cachedImage)
                        {
                            cachedImage.enabled = nlines != 0;
                        }
                    }
                    if (isRiskUI && layoutGroup)
                    {
                        layoutGroup.padding.bottom = (int)((nlines / 16f) * 50);
                    }
                }
                else
                {
                    layoutGroup = null;
                    textComponent = null;
                    layoutElement = null;
                    if (statTracker)
                    {
                        Log.Debug($"Somehow statTracker [{statTracker.gameObject.name}] got set but other items didn't attempting to delete it and try again.");
                        GameObject.DestroyImmediate(statTracker.gameObject);
                    }
                }
                //Log.Debug(stats);
            }
        }
        IEnumerator FixScaleAfterFrame(int nlines)//needed to make the tab stuff work nicely
        {
            yield return new WaitForEndOfFrame();
            float intendedHeight = statsDisplayOverrideHeight.Value
    ? textComponent.fontSize * (nlines + 1)
    : textComponent.renderedHeight;
            layoutElement.preferredHeight = intendedHeight;
        }

        // basic version of vector2 sliders option window
        static void CreatePositionWindow(ConfigEntry<Vector2> configEntry)
        {
            // canvas
            GameObject canvasObj = new GameObject("Vector2 Popup");
            canvasObj.layer = LayerIndex.ui.intVal;
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            CanvasScaler canvasScalar = canvasObj.AddComponent<CanvasScaler>();
            canvasScalar.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScalar.referenceResolution = new Vector2(1920, 1080);
            canvasScalar.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasObj.AddComponent<CanvasRenderer>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvasObj.AddComponent<RiskOfOptions.Components.Options.RooEscapeRouter>().escapePressed.AddListener(Close);

            // window, a rect in center of screen
            GameObject windowObj = new GameObject("Window");
            RectTransform windowRect = windowObj.AddComponent<RectTransform>();
            windowRect.SetParent(canvasObj.transform, false);
            windowRect.anchorMin = new Vector2(0.5f, 0.5f);
            windowRect.anchorMax = new Vector2(0.5f, 0.5f);
            windowRect.pivot = new Vector2(0.5f, 0.5f);
            windowRect.sizeDelta = new Vector2(450, 160);
            windowObj.AddComponent<VerticalLayoutGroup>();

            // background
            GameObject background = new GameObject("Background");
            RectTransform backRect = background.AddComponent<RectTransform>();
            backRect.SetParent(windowRect.transform, false);
            backRect.anchorMin = new Vector2(-0.25f, -0.25f);
            backRect.anchorMax = new Vector2(1.25f, 1.25f);
            backRect.pivot = new Vector2(0.5f, 0.5f);
            background.AddComponent<Image>().color = new Color(0.125f, 0.25f, 0.4f, 1);
            background.AddComponent<LayoutElement>().ignoreLayout = true;

            // slider
            GameObject sliderPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/SettingsSliderControl.prefab").WaitForCompletion();
            CreateSlider("X", 0, 0, 1920);
            CreateSlider("Y", 1, 0, 1080);

            // close button
            GameObject closeButton = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/GenericMenuButton.prefab").WaitForCompletion(), windowRect);
            closeButton.GetComponent<LanguageTextMeshController>().token = "Close";
            closeButton.AddComponent<LayoutElement>().minHeight = 32;
            closeButton.GetComponentInChildren<HGButton>().onClick.AddListener(Close);

            void CreateSlider(string name, int index, float min, float max)
            {
                GameObject sliderObj = UnityEngine.Object.Instantiate(sliderPrefab, windowRect);

                // ror2 slider
                SettingsSlider origSlider = sliderObj.GetComponent<SettingsSlider>();
                origSlider.nameLabel.token = name;
                Slider slider = origSlider.slider;
                slider.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 40);

                // remove ror2 slider
                UnityEngine.Object.DestroyImmediate(origSlider);
#pragma warning disable CS0618 // Type or member is obsolete
                UnityEngine.Object.DestroyImmediate(sliderObj.GetComponent<SelectableDescriptionUpdater>());
#pragma warning restore CS0618 // Type or member is obsolete

                // roo slider
                SliderWithText rooSlider = sliderObj.AddComponent<SliderWithText>();
                rooSlider.Setup(slider, sliderObj.GetComponentInChildren<TMP_InputField>(), min, max);

                // on slider changed
                rooSlider.onValueChanged.AddListener(newValue =>
                {
                    Vector2 pos = configEntry.Value;
                    pos[index] = newValue;
                    configEntry.Value = pos;
                });

                // set initial value
                rooSlider.Value = configEntry.Value[index];
            }

            void Close() => UnityEngine.Object.Destroy(canvasObj);
        }

        internal void Update()
        {
            if (statsDisplay.Value && cachedUserBody)
            {
                // job is scheduled if timer <= 0
                if (timer <= 0)
                {
                    timer += statsDisplayUpdateInterval.Value;
                    Profiler.BeginSample("LookingGlass.StatsDisplay");

                    // complete regex job
                    regexHandle.Complete();
                    CalculateStuff(RegexJob.output);

                    Profiler.EndSample();
                }

                timer -= Time.deltaTime;
                // if interval has passed
                if (timer <= 0)
                {
                    // start new job that will be completed next tick
                    regexHandle = new RegexJob().Schedule();
                }
            }
        }

        static string GenerateStatsText()
        {
            Profiler.BeginSample("LookingGlass.StatsDisplay.Regex");

            string statsText = useSecondaryStatsDisplay.Value && scoreBoardOpen ? secondaryStatsDisplayString.Value : statsDisplayString.Value;
            statsText = statsRegex.Replace(statsText, MatchEvaluator);

            Profiler.EndSample();
            return statsText;
        }

        // replace matched [statname]
        static string MatchEvaluator(Match match)
        {
            Profiler.BeginSample("LookingGlass.StatsDisplay.Regex.Match");
            string replacement = statDictionary.TryGetValue(match.Groups[1].Value, out var replacer) ? replacer(cachedUserBody) : match.Value;
            Profiler.EndSample();
            return replacement;
        }

        // if setting changed, force update next tick
        void ForceUpdate()
        {
            if (statsDisplay.Value && cachedUserBody)
            {
                regexHandle = new RegexJob().Schedule(regexHandle);
                timer = 0;
            }
        }

        struct RegexJob : IJob
        {
            internal static string output = "";

            public void Execute()
            {
                try
                {
                    output = GenerateStatsText();
                }
                catch (Exception e) { Log.Error(e); }
            }
        }
    }
}
