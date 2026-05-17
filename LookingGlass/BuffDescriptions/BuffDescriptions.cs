using BepInEx.Configuration;
using LookingGlass.Base;
using LookingGlass.LookingGlassLanguage;
using MonoMod.RuntimeDetour;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.UI;
using System;
using System.Reflection;
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
            buffDescriptions = BasePlugin.instance.Config.Bind<bool>("Buff Info", "Buff Descriptions", true, LookingGlassLanguageAPI.ConfigDescription("Buff Descriptions", "Gives descriptions to buffs (All vanilla by default, modded buffs need to be setup)"));
            buffDescriptionsFontSize = BasePlugin.instance.Config.Bind<float>("Buff Info", "Buff Font Size", 100f, LookingGlassLanguageAPI.ConfigDescription("Buff Font Size", "Changes the font size of buff descriptions"));

            var targetMethod = typeof(BuffIcon).GetMethod(nameof(BuffIcon.UpdateIcon), BindingFlags.Public | BindingFlags.Instance);
            var destMethod = typeof(BuffDescriptionsClass).GetMethod(nameof(BuffIconUpdateIcon), BindingFlags.NonPublic | BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);

            //This runs too early to use RoR2Content.Buffs, they're not populated yet
            /*targetMethod = typeof(Language).GetMethod(nameof(Language.LoadStrings), BindingFlags.Public | BindingFlags.Instance);
            destMethod = typeof(BuffDescriptionsClass).GetMethod(nameof(LoadLanguages), BindingFlags.NonPublic | BindingFlags.Instance);
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
            string buffInfoCategory = LookingGlassLanguageAPI.ConfigCategory("Buff Info");
            ModSettingsManager.AddOption(new CheckBoxOption(buffDescriptions, new CheckBoxConfig() { category = buffInfoCategory, name = LookingGlassLanguageAPI.ConfigName(buffDescriptions.Definition.Key), restartRequired = false }));
            ModSettingsManager.AddOption(new SliderOption(buffDescriptionsFontSize, new SliderConfig() { category = buffInfoCategory, name = LookingGlassLanguageAPI.ConfigName(buffDescriptionsFontSize.Definition.Key), restartRequired = false, min = 1, max = 300 }));
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
                    if (LookingGlassLanguageAPI.HasToken($"BUFF_NAME_{self.buffDef.name}") || LookingGlassLanguageAPI.HasToken($"NAME_{self.buffDef.name}"))
                    {
                        string name = LookingGlassLanguageAPI.GetString($"BUFF_NAME_{self.buffDef.name}", LookingGlassLanguageAPI.GetString($"NAME_{self.buffDef.name}", self.buffDef.name));
                        string desc = LookingGlassLanguageAPI.GetString($"BUFF_DESCRIPTION_{self.buffDef.name}", LookingGlassLanguageAPI.GetString($"DESCRIPTION_{self.buffDef.name}", string.Empty));
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
