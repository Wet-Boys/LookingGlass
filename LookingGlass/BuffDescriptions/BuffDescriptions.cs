using BepInEx.Configuration;
using LookingGlass.Base;
using MonoMod.RuntimeDetour;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LookingGlass.BuffDescriptions
{
    public class BuffDescriptionsClass : BaseThing
    {
        public static ConfigEntry<bool> buffDescriptions;
        public static ConfigEntry<float> buffDescriptionsFontSize;
        private static Hook overrideHook;
        private static Hook overrideHook2;

        public BuffDescriptionsClass()
        {
            Setup();
            SetupRiskOfOptions();
        }
        public void Setup()
        {
            var targetMethod = typeof(BuffIcon).GetMethod(nameof(BuffIcon.UpdateIcon), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(BuffDescriptionsClass).GetMethod(nameof(BuffIconUpdateIcon), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
            targetMethod = typeof(Language).GetMethod(nameof(Language.LoadStrings), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(BuffDescriptionsClass).GetMethod(nameof(LoadLanguages), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook2 = new Hook(targetMethod, destMethod, this);

            buffDescriptions = BasePlugin.instance.Config.Bind<bool>("Buff Descriptions", "Buff Descriptions", true, "Gives descriptions to buffs (All vanilla by default, modded buffs need to be setup)");
            buffDescriptionsFontSize = BasePlugin.instance.Config.Bind<float>("Buff Descriptions", "Buff Font Size", 100f, "Changes the font size of buff descriptions");

        }

        void LoadLanguages(Action<Language> orig, Language self)
        {
            try
            {
                if (!self.stringsLoaded)
                {
                    BuffDefinitions.SetupEnglishDefs();
                }
            }
            catch (Exception)
            {
            }
            orig(self);
        }
        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(buffDescriptions, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new SliderOption(buffDescriptionsFontSize, new SliderConfig() { restartRequired = false, min = 1, max = 300 }));
        }
        void BuffIconUpdateIcon(Action<BuffIcon> orig, BuffIcon self)
        {
            orig(self);
            if (self.buffDef && buffDescriptions.Value)
            {
                TooltipProvider toolTip = self.GetComponent<TooltipProvider>();
                if (!toolTip)
                {
                    if (self.GetComponentInParent<Canvas>() == null)
                    {
                        //Issue with disabled Huds tht can go on forever
                        return;
                    }
                    if (!self.GetComponentInParent<Canvas>().gameObject.GetComponent<GraphicRaycaster>())
                    {
                        self.GetComponentInParent<Canvas>().gameObject.AddComponent<GraphicRaycaster>();
                    }
                    TooltipContent content = new TooltipContent();
                    content.titleColor = Color.gray;
                    content.bodyColor = Color.blue;
                    content.disableTitleRichText = false;
                    content.disableBodyRichText = false;
                    toolTip = self.gameObject.AddComponent<TooltipProvider>();

                    if (Language.currentLanguage.stringsByToken.ContainsKey($"LG_TOKEN_NAME_{self.buffDef.name}"))
                    {
                        string name = Language.GetString($"LG_TOKEN_NAME_{self.buffDef.name}");
                        string desc = Language.GetString($"LG_TOKEN_DESCRIPTION_{self.buffDef.name}");
                        content.overrideTitleText = $"<size={buffDescriptionsFontSize.Value}%>{Language.GetString(name)}</size>";
                        content.overrideBodyText = $"<size={buffDescriptionsFontSize.Value}%>{Language.GetString(desc)}";
                    }
                    else
                    {
                        content.overrideTitleText = self.buffDef.name;
                    }

                    toolTip.SetContent(content);
                }
                if (toolTip)
                {
                    if (Language.currentLanguage.stringsByToken.ContainsKey($"LG_TOKEN_NAME_{self.buffDef.name}"))
                    {
                        string name = Language.GetString($"LG_TOKEN_NAME_{self.buffDef.name}");
                        string desc = Language.GetString($"LG_TOKEN_DESCRIPTION_{self.buffDef.name}");
                        toolTip.overrideTitleText = $"<size={buffDescriptionsFontSize.Value}%>{name}</size>";
                        toolTip.overrideBodyText = $"<size={buffDescriptionsFontSize.Value}%>{desc}</size>";
                    }
                    else
                    {
                        toolTip.overrideTitleText = self.buffDef.name;
                    }
                }
            }
        }
    }
}
