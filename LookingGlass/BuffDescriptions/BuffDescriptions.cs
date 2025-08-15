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
            buffDescriptions = BasePlugin.instance.Config.Bind<bool>("Buff Info", "Buff Descriptions", true, "Gives descriptions to buffs (All vanilla by default, modded buffs need to be setup)");
            buffDescriptionsFontSize = BasePlugin.instance.Config.Bind<float>("Buff Info", "Buff Font Size", 100f, "Changes the font size of buff descriptions");

            var targetMethod = typeof(BuffIcon).GetMethod(nameof(BuffIcon.UpdateIcon), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(BuffDescriptionsClass).GetMethod(nameof(BuffIconUpdateIcon), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);

            //This runs too early to use RoR2Content.Buffs, they're not populated yet
            /*targetMethod = typeof(Language).GetMethod(nameof(Language.LoadStrings), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(BuffDescriptionsClass).GetMethod(nameof(LoadLanguages), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook2 = new Hook(targetMethod, destMethod, this);*/

            //BuffCatalog does not have availablity
            //ItemCatalog is one of the soonest that runs after it
            ItemCatalog.availability.CallWhenAvailable(BuffDefinitions.SetupEnglishDefs);
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
            //Checking self hopefully prevents log spam for mod packs that are, Dying for unrelated reasons
            //Less misreports
            if (self && self.buffDef && buffDescriptions.Value)
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
                    //Use colors if not white, else gray because can be hard to read on white.
                    content.bodyColor = Color.blue;
                    content.disableTitleRichText = false;
                    content.disableBodyRichText = false;
                    toolTip = self.gameObject.AddComponent<TooltipProvider>();

                    //tooltip gets set here, so desc gets set both here and right afterwards which isnt needed
                    toolTip.SetContent(content);
                }
                if (toolTip)
                {
                    //Always update color
                    toolTip.titleColor = self.buffDef.buffColor == Color.white ? Color.gray : self.buffDef.buffColor;
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
                        toolTip.overrideBodyText = string.Empty;
                    }
                }
            }
        }
    }
}
