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

namespace LookingGlass.EquipTimerFix
{
    public class EquipFixer : BaseThing
    {
        public EquipFixer()
        {
            Setup();
            SetupRiskOfOptions();
        }
        private static Hook overrideHook;
        public static ConfigEntry<bool> permanentEquipCooldownText;
        public void Setup()
        {
            var targetMethod = typeof(EquipmentIcon).GetMethod(nameof(EquipmentIcon.SetDisplayData), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(EquipFixer).GetMethod(nameof(SetDisplayData), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
            permanentEquipCooldownText = BasePlugin.instance.Config.Bind<bool>("Misc", "Permanent Cooldown Indicator For Equip", true, "Makes the cooldown indicator for the equip slot permanent and not just when you have 0 stock.");
        }

        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(permanentEquipCooldownText, new CheckBoxConfig() { restartRequired = false }));
        }
        void SetDisplayData(Action<EquipmentIcon, EquipmentIcon.DisplayData> orig, EquipmentIcon self, EquipmentIcon.DisplayData newDisplayData)
        {
            orig(self, newDisplayData);
            if (permanentEquipCooldownText.Value && self.hasEquipment && newDisplayData.stock != newDisplayData.maxStock && self.cooldownText)
            {
                self.cooldownText.gameObject.SetActive(true);
            }
        }
    }
}
