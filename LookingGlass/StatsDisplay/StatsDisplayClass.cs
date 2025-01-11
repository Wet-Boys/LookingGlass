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

namespace LookingGlass.StatsDisplay
{
    internal class StatsDisplayClass : BaseThing
    {
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
        const string syntaxList = " \n luck \n baseDamage \n crit \n attackSpeed \n armor \n armorDamageReduction \n regen \n speed \n availableJumps \n maxJumps \n killCount \n mountainShrines \n experience \n level \n maxHealth \n maxBarrier \n barrierDecayRate \n maxShield \n acceleration \n jumpPower \n maxJumpHeight \n damage \n critMultiplier \n bleedChance \n visionDistance \n critHeal \n cursePenalty \n hasOneShotProtection \n isGlass \n canPerformBackstab \n canReceiveBackstab \n healthPercentage \n goldPortal \n msPortal \n shopPortal \n dps \n currentCombatDamage \n remainingComboDuration \n maxCombo \n maxComboThisRun \n currentCombatKills \n maxKillCombo \n maxKillComboThisRun \n critWithLuck \n bleedChanceWithLuck \n velocity \n teddyBearBlockChance \n saferSpacesCD \n instaKillChance \n voidPortal \n difficultyCoefficient \n stage";
        public void Setup()
        {
            statsDisplay = BasePlugin.instance.Config.Bind<bool>("Stats Display", "StatsDisplay", true, "Enables Stats Display");
            statsDisplay.SettingChanged += Display_SettingChanged;
            statsDisplayString = BasePlugin.instance.Config.Bind<string>("Stats Display", "Stats Display String",
                "<size=120%>Stats</size>\n" +
                "Luck: [luck]\n" +
                "Damage: [damage]\n" +
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
                , $"String for the stats display. You can customize this with Unity Rich Text if you want, see \n https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/RichText.html for more info. \nAvailable syntax for the [] stuff is:{syntaxList}");
            statsDisplaySize = BasePlugin.instance.Config.Bind<float>("Stats Display", "StatsDisplay font size", -1, "General font size of the stats display menu. If set to -1, will copy the font size from the objective panel");
            statsDisplayUpdateInterval = BasePlugin.instance.Config.Bind<float>("Stats Display", "StatsDisplay update interval", 0.1f, "The interval at which stats display updates, in seconds. Lower values will increase responsiveness, but may potentially affect performance for large texts");
            statsDisplayUpdateInterval.SettingChanged += Display_SettingChanged;
            builtInColors = BasePlugin.instance.Config.Bind<bool>("Stats Display", "Use default colors", true, "Uses the default styling for stats display syntax items.");
            builtInColors.SettingChanged += BuiltInColors_SettingChanged;
            statsDisplayOverrideHeight = BasePlugin.instance.Config.Bind<bool>("Stats Display", "Override Stats Display Height", false, "Sets a user-specified height for Stats Display (may be necessary if you get particularly creative with formatting)");
            statsDisplayOverrideHeightValue = BasePlugin.instance.Config.Bind<int>("Stats Display", "Stats Display Height Value", 7, "Height, in lines of full-size text, for the Stats Display panel");
            floatPrecision = BasePlugin.instance.Config.Bind<int>("Stats Display", "StatsDisplay Float Precision", 2, "How many decimal points will be used in floating point values");
            floatPrecision.SettingChanged += BuiltInColors_SettingChanged;
            useSecondaryStatsDisplay = BasePlugin.instance.Config.Bind<bool>("Stats Display", "Use Secondary StatsDisplay", false, "Will enable the use of the secondary stats display string. This will overwrite the stats display string whenever the scoreboard is held open.");
            secondaryStatsDisplayString = BasePlugin.instance.Config.Bind<string>("Stats Display", "Secondary Stats Display String",
                "<size=120%>Stats</size>\n" +
                "Luck: [luck]\n" +
                "Damage: [damage]\n" +
                "Crit Chance: [critWithLuck]\n" +
                "Bleed Chance: [bleedChanceWithLuck]\n" +
                "Attack Speed: [attackSpeed]\n" +
                "Armor: [armor] | [armorDamageReduction]\n" +
                "Regen: [regen]\n" +
                "Speed: [speed]\n" +
                "Jumps: [availableJumps]/[maxJumps]\n" +
                "Kills: [killCount]\n" +
                "Mountain Shrines: [mountainShrines]\n" +
                "Max Combo: [maxCombo]\n" +
                "<size=120%>Portals:</size> \n" +
                "<size=50%>Gold:[goldPortal] Shop:[shopPortal] Celestial:[msPortal] Void:[voidPortal]</size>"
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

        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(statsDisplay, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new StringInputFieldOption(statsDisplayString, new InputFieldConfig() { restartRequired = false, lineType = TMP_InputField.LineType.MultiLineNewline, submitOn = InputFieldConfig.SubmitEnum.OnExit, richText = false }));
            ModSettingsManager.AddOption(new SliderOption(statsDisplaySize, new SliderConfig() { restartRequired = false, min = -1, max = 100 }));
            ModSettingsManager.AddOption(new CheckBoxOption(builtInColors, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new SliderOption(statsDisplayUpdateInterval, new SliderConfig() { restartRequired = false, min = 0.01f, max = 1f, formatString = "{0:F2}s" }));
            ModSettingsManager.AddOption(new CheckBoxOption(statsDisplayOverrideHeight, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new IntSliderOption(statsDisplayOverrideHeightValue, new IntSliderConfig() { restartRequired = false, min = 0, max = 100 }));
            ModSettingsManager.AddOption(new IntSliderOption(floatPrecision, new IntSliderConfig() { restartRequired = false, min = 0, max = 5 }));

            ModSettingsManager.AddOption(new CheckBoxOption(useSecondaryStatsDisplay, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new StringInputFieldOption(secondaryStatsDisplayString, new InputFieldConfig() { restartRequired = false, lineType = TMP_InputField.LineType.MultiLineNewline, submitOn = InputFieldConfig.SubmitEnum.OnExit, richText = false }));

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
                    if (statsDisplayAttached.Value)
                    {
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
