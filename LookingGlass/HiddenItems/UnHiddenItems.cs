using BepInEx.Configuration;
using LookingGlass.Base;
using MonoMod.RuntimeDetour;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.ContentManagement;

namespace LookingGlass.HiddenItems
{
    internal class UnHiddenItems : BaseThing
    {
        public UnHiddenItems()
        {
            Setup();
            SetupRiskOfOptions();
        }
        private static Hook overrideHook;
        public static ConfigEntry<bool> noHiddenItems;
        public void Setup()
        {
            var targetMethod = typeof(ItemCatalog).GetMethod(nameof(ItemCatalog.Init), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var destMethod = typeof(UnHiddenItems).GetMethod(nameof(ItemCatalogInit), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
            noHiddenItems = BasePlugin.instance.Config.Bind<bool>("Misc", "Unhide Hidden Items", false, "Unhides normally hidden items such as the Drizzle/MonsoonHelpers");
        }

        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(noHiddenItems, new CheckBoxConfig() { restartRequired = true }));
        }
        void ItemCatalogInit(Action orig)
        {
            if (noHiddenItems.Value)
            {
                foreach (var item in ContentManager.itemDefs)
                {
                    item.hidden = false;
                }
            }
            orig();
        }
    }
}
