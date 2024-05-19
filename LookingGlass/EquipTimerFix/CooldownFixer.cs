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
using UnityEngine;

namespace LookingGlass.EquipTimerFix
{
    public class CooldownFixer : BaseThing
    {
        public CooldownFixer()
        {
            Setup();
            SetupRiskOfOptions();
        }
        private static Hook overrideHook;
        private static Hook overrideHook2;
        public static ConfigEntry<bool> permanentEquipCooldownText;
        public static ConfigEntry<bool> permanentSkillCooldownText;
        public void Setup()
        {
            var targetMethod = typeof(EquipmentIcon).GetMethod(nameof(EquipmentIcon.SetDisplayData), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(CooldownFixer).GetMethod(nameof(SetDisplayData), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
            targetMethod = typeof(SkillIcon).GetMethod(nameof(SkillIcon.Update), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(CooldownFixer).GetMethod(nameof(Update), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook2 = new Hook(targetMethod, destMethod, this);
            permanentEquipCooldownText = BasePlugin.instance.Config.Bind<bool>("Misc", "Permanent Cooldown Indicator For Equip", true, "Makes the cooldown indicator for the equip slot permanent and not just when you have 0 stock.");
            permanentSkillCooldownText = BasePlugin.instance.Config.Bind<bool>("Misc", "Permanent Cooldown Indicator For Skills", true, "Makes the cooldown indicator for skills permanent and not just when you have 0 stock.");
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
        void Update(Action<SkillIcon> orig, SkillIcon self)
        {
            orig(self);
            if (permanentSkillCooldownText.Value && 
                self.targetSkill && 
                self.targetSkill.stock > 0 && 
                self.targetSkill.stock != self.targetSkill.maxStock && 
                self.cooldownText)
            {
                SkillIcon.sharedStringBuilder.Clear();
                SkillIcon.sharedStringBuilder.AppendInt(Mathf.CeilToInt(self.targetSkill.cooldownRemaining), 1U, uint.MaxValue);
                self.cooldownText.SetText(SkillIcon.sharedStringBuilder);
                self.cooldownText.gameObject.SetActive(true);
            }
        }
    }
}
