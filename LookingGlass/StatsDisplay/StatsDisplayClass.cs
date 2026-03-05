#if DEBUG
#define ENABLE_PROFILER // enables Profiler.BeginSample
#endif
using BepInEx.Configuration;
using LookingGlass.Base;
using MonoMod.RuntimeDetour;
using RiskOfOptions;
using RiskOfOptions.Components.Options;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace LookingGlass.StatsDisplay
{
    internal class StatsDisplayClass : BaseThing
    {
        public enum Colors
        {
            Logbook,
            CategoryChest,
            None,
        }

        public enum StatsDisplayEnum
        {
            Off,
            Same_On_Both,
            Different_On_Tab,
            Only_Show_On_Main,
            Only_Show_On_Tab,
            Show_Only_Stats_On_Tab,
        }

        public enum StatDisplayPreset
        {
            Set,            //
            LookingGlass,   //
            //Percentages,   //
            Simpler,        //No DPS/Combo 
            Extra,          //CritDamage,Luck,Curse%, Osp
            Extra_For_OnlyStats,          //CritDamage,Luck,Curse%, Osp
            Minimal,        //
            Old,            //BetterUI-like
            //AddLineHeight,
        }
        public static ConfigEntry<StatDisplayPreset> statStringPresets;
        public static ConfigEntry<bool> movePurchaseText; //-240x
        public static ConfigEntry<bool> checkIfOldDefaultSettings;

        public static ConfigEntry<StatsDisplayEnum> statsDisplay;

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
        public static bool scoreBoardOpen;
        public static bool deathScreen;
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
            + "\n hpAddPercent, healthAddPercent, shieldAddPercent"
            + "\n effectiveHealth, effectiveMaxHealth"
            + "\n shieldFraction, barrierDecayRate"
            + "\n healthPercentage"
            + "\n regen, regenHp, regenRaw"
            + "\n armor, armorDamageReduction"
            + "\n curseHealthReduction "
            + "\n hasOSP "
            + "\n damagePercent, damagePercentWithWatch"
            + "\n maxHpPercent, maxHealthStat, maxShieldStat"

            + "\n speed, speedPercent, speedPercentSprintAffected"
            + "\n velocity, acceleration "
            + "\n availableJumps, maxJumps"
            + "\n jumpPower, maxJumpHeight"
            + "\n level, experience "
            + "\n luck "

            + "\n mountainShrines "
            + "\n portals, shopPortal, goldPortal, msPortal, voidPortal, greenPortal"
            + "\n difficultyCoefficient, stage"
            + "\n ping"
            + "\n"
            + "\n killCount, killCountRun "
            + "\n dps, percentDps "
            + "\n combo, maxComboThisRun, maxCombo"
            + "\n killCombo, maxKillComboThisRun, maxKillCombo"
            + "\n remainingComboDuration"
            + "\n"
            + "\n teddyBearBlockChance, saferSpacesCD "
            + "\n instaKillChance "
            + "\n"
            + "\n lvl1_damage, lvl1_maxHealth";
        public void Setup()
        {
            //statsDisplay = BasePlugin.instance.Config.Bind<StatsDisplayEnum>("Stats Display", "StatsDisplay", StatsDisplayEnum.AltSecondary, "Enables Stats Display.\n\nSecondary: Will display different text while the Scoreboard is open\n\nOnlyTab: Will display text only while the scoreboard is open. ");

            //Could maybe combine into 1 config?
            statsDisplay = BasePlugin.instance.Config.Bind<StatsDisplayEnum>("Stats Display", "Stats Display", StatsDisplayEnum.Different_On_Tab, "Enables Stats Display in various cases.\n\n-Same stats regardless of scoreboard\n\n-Tab stats on scoreboard for 2 different stat displays\n\n-Only show when Scoreboard is open\n\n-Only show when scoreboard is open and hide everything else when it is open. (In case other mods decide to use that space, to avoid the clutter)");


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
            statsDisplaySize = BasePlugin.instance.Config.Bind<float>("Stats Display", "Stats Display font size", -1f, "General font size of the stats display menu.\n\nIf set to -1, it will be sized relative to the Objective Header and Objectives. 13.5 on the default hud.");

            statsDisplayUpdateInterval = BasePlugin.instance.Config.Bind<float>("Stats Display", "Stats Display update interval", 0.25f, "The interval at which stats display updates, in seconds. Lower values will increase responsiveness, but may potentially affect performance for large texts\n\nValues below 0.2 are not recommended for normal play for performance reasons.\n\n");
            statsDisplayUpdateInterval.SettingChanged += Display_SettingChanged;
            builtInColors = BasePlugin.instance.Config.Bind<bool>("Stats Display", "Use default colors", true, "Uses the default styling for stats display syntax items.");
            builtInColors.SettingChanged += BuiltInColors_SettingChanged;
            statsDisplayOverrideHeight = BasePlugin.instance.Config.Bind<bool>("Stats Display", "Override Stats Display Height", false, "Sets a user-specified height for Stats Display (may be necessary if you get particularly creative with formatting)");
            statsDisplayOverrideHeightValue = BasePlugin.instance.Config.Bind<int>("Stats Display", "Stats Display Height Value", 7, "Height, in lines of full-size text, for the Stats Display panel");
            floatPrecision = BasePlugin.instance.Config.Bind<int>("Stats Display", "StatsDisplay Float Precision", 2, "How many decimal points will be used in floating point values");
            floatPrecision.SettingChanged += BuiltInColors_SettingChanged;
            secondaryStatsDisplayString = BasePlugin.instance.Config.Bind<string>("Stats Display", "Secondary Stats Display String",
                "<margin-left=0.6em>"
                + "<size=115%>Stats</size>\n"
                + "Damage: [damage]\n"
                + "Attack Speed: [attackSpeed]\n"
                + "Crit Chance: [critWithLuck]\n"
                //+ "Crit Stats: [critWithLuck] | [critMultiplier]\n"
                + "Bleed Chance: [bleedChanceWithLuck]\n"
                + "Regen: [regen]\n"
                + "Armor: [armor] | [armorDamageReduction]\n"
                + "Speed: [speed]\n"
                + "Jumps: [availableJumps] / [maxJumps]\n"
                + "Total Kills: [killCountRun]\n" //Kills Primary -> Run Kills Secondary
                + "Max Combo: [maxComboThisRun]\n" //Combo Primary -> Run Combo Secondary
                + "Mountain Shrines: [mountainShrines]\n"
                + "Portals: [portals] \n"
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

            var targetMethod = typeof(ScoreboardController).GetMethod(nameof(ScoreboardController.OnEnable), BindingFlags.NonPublic | BindingFlags.Instance);
            new Hook(targetMethod, OnEnable);

            targetMethod = typeof(ScoreboardController).GetMethod(nameof(ScoreboardController.OnDisable), BindingFlags.NonPublic | BindingFlags.Instance);
            new Hook(targetMethod, OnDisable);


            statStringPresets = BasePlugin.instance.Config.Bind<StatDisplayPreset>("Stats Display", "Preset", StatDisplayPreset.Set, "Override current Stat Display settings with a premade preset.Further changes can made from there.\n\n" +
                "Extra: More stats on Tab\n\n" +
                "Simpler: Dont include DPS, Combo\n\n" +
                "Minimal: DPS + Jump, Few stats on Tab for mathing or remembering.\n\n");

            statStringPresets.SettingChanged += ApplyPresets;


            movePurchaseText = BasePlugin.instance.Config.Bind<bool>("Stats Display", "Move Purchase Text", true, "Move purchase text further to the left to avoid clipping with larger Stat Displays, Evolution Inventory, Objectives, etc..");
            movePurchaseText.SettingChanged += MovePurchase;




            checkIfOldDefaultSettings = BasePlugin.instance.Config.Bind<bool>("Stats", "Check For Old Default Settings v2", true, "Override the stat display with the updated one, if you were using the default one prior to updating.\nNot meant as a config just needs to be tracked.");
            //Run in case some guy doesnt use RiskOfOptions and to check if Old?
            if (!checkIfOldDefaultSettings.Value)
            {
                ApplyPresets(null, null);
            }



        }

        bool SecondaryDisabled()
        {
            return statsDisplay.Value == StatsDisplayEnum.Same_On_Both || statsDisplay.Value == StatsDisplayEnum.Only_Show_On_Main;
        }
        bool PrimaryDisabled()
        {
            return statsDisplay.Value >= StatsDisplayEnum.Only_Show_On_Tab;
        }
        public void SetupRiskOfOptions()
        {

            ModSettingsManager.AddOption(new ChoiceOption(statsDisplay, new ChoiceConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new ChoiceOption(statStringPresets, false));
            ModSettingsManager.AddOption(new StringInputFieldOption(statsDisplayString, new InputFieldConfig()
            {
                name = "Main Display String",
                restartRequired = false,
                lineType = TMP_InputField.LineType.MultiLineNewline,
                submitOn = InputFieldConfig.SubmitEnum.OnExitOrSubmit,
                richText = false,
                checkIfDisabled = PrimaryDisabled
            }));
            ModSettingsManager.AddOption(new StringInputFieldOption(secondaryStatsDisplayString, new InputFieldConfig()
            {
                name = "Tab Display String",
                restartRequired = false,
                lineType = TMP_InputField.LineType.MultiLineNewline,
                submitOn = InputFieldConfig.SubmitEnum.OnExitOrSubmit,
                richText = false,
                checkIfDisabled = SecondaryDisabled
            }));




            ModSettingsManager.AddOption(new SliderOption(statsDisplaySize, new SliderConfig() { restartRequired = false, min = -1, max = 24, FormatString = "{0:F1}" }));
            //ModSettingsManager.AddOption(new StepSliderOption(statsDisplaySize, new StepSliderConfig() { restartRequired = false, min = -1, max = 24, increment = 0.2f,FormatString = "{0:F1}" }));

            ModSettingsManager.AddOption(new CheckBoxOption(movePurchaseText, new CheckBoxConfig() { restartRequired = false }));

            ModSettingsManager.AddOption(new CheckBoxOption(builtInColors, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new SliderOption(statsDisplayUpdateInterval, new SliderConfig() { name = "Update interval", restartRequired = false, min = 0.01f, max = 1f, FormatString = "{0:F2}s" }));
            ModSettingsManager.AddOption(new CheckBoxOption(statsDisplayOverrideHeight, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new IntSliderOption(statsDisplayOverrideHeightValue, new IntSliderConfig() { restartRequired = false, min = 0, max = 100 }));
            ModSettingsManager.AddOption(new IntSliderOption(floatPrecision, new IntSliderConfig() { name = "Float precision", restartRequired = false, min = 0, max = 5 }));


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



        public void CheckForOldDefaultSettingsThatNeedToBeUpdated()
        {
            if (!checkIfOldDefaultSettings.Value)
            {
                return;
            }
            //Check once upon updating if they were using old default stat string.
            //So it keeps any custom ones, but people who are using default dont have to manually reset

            //Check BetterUI Like (with baseDamage)
            //Check BetterUI Like
            bool reset1 = false;
            bool reset2 = false;
            Debug.Log("Checking for old");

            #region BetterUI-like 
            string old1_1 =
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
                + "Combo: [currentCombatDamage]\n"
                + "Combo Timer: [remainingComboDuration]\n"
                + "Max Combo: [maxCombo]";
            string old1_2 =
                "<size=120%>Stats</size>\n"
                + "Luck: [luck]\n"
                + "Damage: [baseDamage]\n"
                + "Crit Chance: [critWithLuck]\n"
                + "Attack Speed: [attackSpeed]\n"
                + "Armor: [armor] | [armorDamageReduction]\n"
                + "Regen: [regen]\n"
                + "Speed: [speed]\n"
                + "Jumps: [availableJumps]/[maxJumps]\n"
                + "Kills: [killCount]\n"
                + "Mountain Shrines: [mountainShrines]\n"
                + "DPS: [dps]\n"
                + "Combo: [currentCombatDamage]\n"
                + "Combo Timer: [remainingComboDuration]\n"
                + "Max Combo: [maxCombo]";
            string old2_1 =
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
                + "Max Combo: [maxCombo]\n"
                + "<size=120%>Portals:</size> \n"
                + "<size=50%>Gold:[goldPortal] Shop:[shopPortal] Celestial:[msPortal] Void:[voidPortal]</size>";
            string old2_2 =
                "<size=120%>Stats</size>\n"
                + "Luck: [luck]\n"
                + "Damage: [baseDamage]\n"
                + "Crit Chance: [critWithLuck]\n"
                + "Bleed Chance: [bleedChanceWithLuck]\n"
                + "Attack Speed: [attackSpeed]\n"
                + "Armor: [armor] | [armorDamageReduction]\n"
                + "Regen: [regen]\n"
                + "Speed: [speed]\n"
                + "Jumps: [availableJumps]/[maxJumps]\n"
                + "Kills: [killCount]\n"
                + "Mountain Shrines: [mountainShrines]\n"
                + "Max Combo: [maxCombo]\n"
                + "<size=120%>Portals:</size> \n"
                + "<size=50%>Gold:[goldPortal] Shop:[shopPortal] Celestial:[msPortal] Void:[voidPortal]</size>";



            if (statsDisplayString.Value == old1_1 || statsDisplayString.Value == old1_2)
            {
                reset1 = true;
            }
            if (secondaryStatsDisplayString.Value == old2_1 || secondaryStatsDisplayString.Value == old2_2)
            {
                reset2 = true;
            }
            #endregion

            if (reset1)
            {
                Debug.Log("Old1 detected");
                statsDisplayString.Value = (string)statsDisplayString.DefaultValue;
            }
            if (reset2)
            {
                Debug.Log("Old2 detected");
                secondaryStatsDisplayString.Value = (string)secondaryStatsDisplayString.DefaultValue;
            }

            checkIfOldDefaultSettings.Value = false;

            //Other values that had their default updated

            //Would get set every time so bad idea
            //useSecondaryStatsDisplay.Value = true;
            //statsDisplayUpdateInterval.Value = 0.2f;
            //CommandWindowBlur.NoWindowBlur.disable.Value = false;
            //ItemStatsNameSpace.ItemStats.fullDescOnPickup.Value = false; 


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
                case StatDisplayPreset.LookingGlass:
                    new1 = (string)statsDisplayString.DefaultValue;
                    new2 = (string)secondaryStatsDisplayString.DefaultValue;
                    break;
                /*case StatDisplayPreset.LookingGlass:
                    new1 = (string)statsDisplayString.DefaultValue;
                    new2 = (string)secondaryStatsDisplayString.DefaultValue;

                    new1.Replace("damage", "damagePercent");
                    new1.Replace("attackSpeed", "attackSpeedPercent");
                    new1.Replace("speed", "speedPercent");
                    break;*/
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
                        + "Damage: [damage] | [damagePercentWithWatch]\n"
                        + "Attack Speed: [attackSpeedPercent]\n"
                        + "Crit Stats: [critWithLuck] | [critMultiplier]\n"
                        + "Bleed Chance: [bleedChanceWithLuck]\n"
                        + "Regen: [regenHp]\n"
                        + "Armor: [armor] | [armorDamageReduction]\n"
                        + "Max Hp: [maxHpPercent]\n"
                        + "Ehp: [effectiveHealth]\n"
                        + "Speed: [speed] | [speedPercent]\n"
                        + "Jumps: [availableJumps] / [maxJumps]\n"
                        + "Luck: [luck]\n"
                        //+ "Curse: [curseHealthReduction]\n"
                        + "Total Kills: [killCountRun]\n"
                        + "Ping: [ping]\n"
                        + "Portals: [portals] \n"
                        + "</margin>";
                    break;
                case StatDisplayPreset.Extra_For_OnlyStats:
                    //+Luck
                    //+Crit Damage Mult
                    //+Curse HP Reduction
                    //+Has OSP rn ig
                    //AS / MS % for idk nerds
                    //statsDisplay.Value = StatsDisplayEnum.Show_Only_Stats_On_Tab;
                    //
                    //

                    new1 = string.Empty;
                    new2 =
                        "<size=16px><line-height=7.5px>"
                        + "\n<align=center>Stats:</align>\r\n\r\n\r\n"
                        + "</line-height></size>"
                        + "<margin-left=0.6em><line-height=110%>"
                        + "Damage: [damage] | [damagePercentWithWatch]\n"
                        + "Attack Speed: [attackSpeedPercent]\n"
                        + "Crit Stats: [critWithLuck] | [critMultiplier]\n"
                        + "Bleed Chance: [bleedChanceWithLuck]\n"
                        + "Regen: [regenHp]\n"
                        + "Armor: [armor] | [armorDamageReduction]\n"
                        + "Max Hp: [maxHpPercent]\n"
                        + "Ehp: [effectiveHealth]\n"
                        + "Speed: [speed] | [speedPercent]\n"
                        + "Jumps: [availableJumps] / [maxJumps]\n"
                        + "Luck: [luck]\n"
                        //+ "Curse: [curseHealthReduction]\n"
                        + "Kills: [killCount]\n"
                        //+ "Total Kills: [killCountRun]\n"
                        //+ "Max Combo: [maxComboThisRun]\n"
                        //+ "Mountain Shrines: [mountainShrines]\n" //Stat lost most of it's relevance with icons stacking in vanilla
                        + "Ping: [ping]\n"
                        + "Portals: [portals] \n"
                        + "</line-height></margin>";

                    //"<size=16px><line-height=7.5px>\r\n<align=center>Stats:</align>\r\n\r\n</line-height></size><margin-left=0.6em>Damage: [damage]\r\nAttack Speed: [attackSpeedPercent]\r\nCrit Stats: [critWithLuck] | [critMultiplier]\r\nBleed Chance: [bleedChanceWithLuck]\r\nRegen: [regenHp]\r\nArmor: [armor] | [armorDamageReduction]\r\nEhp: [effectiveHealth]\r\nSpeed: [speedPercent]\r\nJumps: [availableJumps] / [maxJumps]\r\nLuck: [luck]\r\nCurse: [curseHealthReduction]\r\nTotal Kills: [killCountRun]\r\nMax Combo: [maxComboThisRun]\r\nMountain Shrines: [mountainShrines]\r\nPortals: [portals] \r\n</line-height></margin>"
                    //"<size=16px><line-height=7.5px>\r\n<align=center>Stats:</align>\r\n\r\n\r\n</line-height></size><margin-left=0.6em>Damage: [damage]\r\nAttack Speed: [attackSpeedPercent]\r\nCrit Stats: [critWithLuck] | [critMultiplier]\r\nBleed Chance: [bleedChanceWithLuck]\r\nRegen: [regenHp]\r\nArmor: [armor] | [armorDamageReduction]\r\nEhp: [effectiveHealth]\r\nSpeed: [speedPercent]\r\nJumps: [availableJumps] / [maxJumps]\r\nLuck: [luck]\r\nCurse: [curseHealthReduction]\r\nTotal Kills: [killCountRun]\r\nMax Combo: [maxComboThisRun]\r\nMountain Shrines: [mountainShrines]\r\nPortals: [portals] \r\n</line-height></margin>"
                    break;

                case StatDisplayPreset.Simpler:
                    //No Combo or DPS stuff
                    //
                    new1 =
                         "<line-height=110%>"
                         + "<align=center><size=16px>Stats:</align></size></line-height>" +
                         "<margin-left=0.6em>\n"
                         + "Damage: [damage]\n"
                         + "Attack Speed: [attackSpeed]\n"
                         + "Crit Chance: [critWithLuck]\n"
                         + "Regen: [regen]\n"
                         + "Armor: [armor] | [armorDamageReduction]\n"
                         + "Speed: [speed]\n"
                         + "Jumps: [availableJumps] / [maxJumps]\n"
                         + "Kills: [killCount]\n"
                         + "</margin>";
                    new2 =
                         "<line-height=110%>"
                         + "<align=center><size=16px>Stats:</align></size></line-height>\n"
                         + "<margin-left=0.6em>"
                         + "Damage: [damage]\n"
                         + "Attack Speed: [attackSpeed]\n"
                         + "Crit Chance: [critWithLuck]\n"
                         + "Bleed Chance: [bleedChanceWithLuck]\n"
                         + "Regen: [regen]\n"
                         + "Armor: [armor] | [armorDamageReduction]\n"
                         + "Speed: [speed]\n"
                         + "Jumps: [availableJumps] / [maxJumps]\n"
                         + "Kills: [killCount]\n"
                         + "Portals: [portals]\n"
                         + "</margin>";
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
                          + "Portal: [portals]"
                          + "</line-height></margin>";
                    break;

                case StatDisplayPreset.Old:
                    //Do not modify
                    //Ugly BetterUI version
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
                        + "Max Combo: [maxCombo]";
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
                          + "Max Combo: [maxCombo]\n"
                          + "<size=120%>Portals:</size> \n"
                          + "<size=50%>Gold:[goldPortal] Shop:[shopPortal] Celestial:[msPortal] Void:[voidPortal]</size>";
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
            GameObject option1 = GameObject.Find("SafeArea/SubPanelArea/SettingsSubPanel, (Mod Options)/Options Panel(Clone)/Scroll View/Viewport/VerticalLayout/Mod Option Input Field, Main Display String");
            GameObject option2 = GameObject.Find("SafeArea/SubPanelArea/SettingsSubPanel, (Mod Options)/Options Panel(Clone)/Scroll View/Viewport/VerticalLayout/Mod Option Input Field, Tab Display String");

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
            if (statsDisplay.Value == StatsDisplayEnum.Off)
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
                                    textComponent.fontSize = (textComponent.fontSize + 11) / 2f;
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
                                Run.instance.StartCoroutine(SetLastDelayed());
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

                    if (statTracker)
                    {
                        GameObject.Destroy(statTracker.GetComponentInChildren<LanguageTextMeshController>()); //Prevents "Objective:" from showing up briefly
                        if (statsDisplay.Value >= StatsDisplayEnum.Only_Show_On_Tab && !scoreBoardOpen)
                        {
                            statTracker.gameObject.SetActive(false);
                        }
                        else if (statsDisplay.Value == StatsDisplayEnum.Only_Show_On_Main && scoreBoardOpen)
                        {
                            statTracker.gameObject.SetActive(false);
                        }
                        else
                        {
                            statTracker.gameObject.SetActive(true);
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
            : textComponent.preferredHeight;
            layoutElement.preferredHeight = intendedHeight;
        }
        IEnumerator SetLastDelayed()//SetToASetSiblingIndex to be consistent with other mods
        {
            yield return new WaitForSeconds(1);
            statTracker.SetAsLastSibling();
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

        internal void FixedUpdate()
        {
            if (statsDisplay.Value != StatsDisplayEnum.Off && cachedUserBody)
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

            string statsText = statsDisplay.Value >= StatsDisplayEnum.Different_On_Tab && scoreBoardOpen ? secondaryStatsDisplayString.Value : statsDisplayString.Value;
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
            if (statsDisplay.Value != StatsDisplayEnum.Off && cachedUserBody)
            {
                regexHandle = new RegexJob().Schedule(regexHandle);
                timer = 0;
                FixedUpdate();
            }

            if (statTracker)
            {
                if (statsDisplay.Value == StatsDisplayEnum.Show_Only_Stats_On_Tab)
                {
                    //0 is Artifact Info and cannot be disabled // automatically turns on so dont mess with it.
                    for (int i = 1; i < statTracker.parent.childCount; i++)
                    {
                        statTracker.parent.GetChild(i).gameObject.SetActive(!scoreBoardOpen);
                    }
                }

                if (statsDisplay.Value >= StatsDisplayEnum.Only_Show_On_Tab && !scoreBoardOpen)
                {
                    statTracker.gameObject.SetActive(false);
                }
                else if (statsDisplay.Value == StatsDisplayEnum.Only_Show_On_Main && scoreBoardOpen)
                {
                    statTracker.gameObject.SetActive(false);
                }
                else
                {
                    statTracker.gameObject.SetActive(true);
                }
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
