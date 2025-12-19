using BepInEx.Configuration;
using LookingGlass.Base;
using LookingGlass.ItemStatsNameSpace;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.Projectile;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection;
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
            var targetMethod = typeof(EquipmentIcon).GetMethod(nameof(EquipmentIcon.SetDisplayData), BindingFlags.NonPublic | BindingFlags.Instance);
            var destMethod = typeof(CooldownFixer).GetMethod(nameof(SetDisplayData), BindingFlags.NonPublic | BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);

            new Hook(typeof(SkillIcon).GetMethod(nameof(SkillIcon.Update), BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(CooldownFixer).GetMethod(nameof(Update), BindingFlags.NonPublic | BindingFlags.Instance), 
                this);
            /*new Hook(
                typeof(EquipmentIcon.DisplayData).GetProperty(nameof(EquipmentIcon.DisplayData.showCooldown), BindingFlags.Public | BindingFlags.Instance).GetGetMethod(), 
                typeof(CooldownFixer).GetMethod(nameof(showCooldownOverride), BindingFlags.Public | BindingFlags.Instance),
                this);*/

            permanentEquipCooldownText = BasePlugin.instance.Config.Bind<bool>("Misc", "Permanent Cooldown Indicator For Equip", true, "Makes the cooldown indicator for the equip slot permanent and not just when you have 0 stock.");
            permanentSkillCooldownText = BasePlugin.instance.Config.Bind<bool>("Misc", "Permanent Cooldown Indicator For Skills", true, "Makes the cooldown indicator for skills permanent and not just when you have 0 stock.");
        
            
        }

       /* public delegate bool orig_get_showCooldown(global::RoR2.UI.EquipmentIcon.DisplayData self);
       
        public bool showCooldownOverride(orig_get_showCooldown orig, global::RoR2.UI.EquipmentIcon.DisplayData self)
        {
            if (permanentEquipCooldownText.Value && self.hasEquipment && self.stock < self.maxStock)
            {
                return true;
            }
            return !self.isReady && self.hasEquipment;
        }*/

        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(permanentEquipCooldownText, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(permanentSkillCooldownText, new CheckBoxConfig() { restartRequired = false }));
        }
        void SetDisplayData(Action<EquipmentIcon, EquipmentIcon.DisplayData> orig, EquipmentIcon self, EquipmentIcon.DisplayData newDisplayData)
        {
            orig(self, newDisplayData);
            //This runs every frame but there's no better alternative
            if (ItemStats.fullDescInHud.Value)
            {
                BasePlugin.instance.itemStats.EquipText(self);
            }
            if (self.hasEquipment && self.cooldownText && !newDisplayData.equipmentDisabled && permanentEquipCooldownText.Value)
            {
                self.cooldownText.gameObject.SetActive(newDisplayData.stock < newDisplayData.maxStock);
            }
          
        }
        void Update(Action<SkillIcon> orig, SkillIcon self)
        {
            orig(self);


            //If you had a Stock increase, but lost it
            //It kinda deletes this so idk ig it'd be good
            //Ie have 2 out of 1 stock, doesn't show that you have more

            if (permanentSkillCooldownText.Value &&
                self.targetSkill &&
                self.targetSkill.stock > 0 &&
                self.targetSkill.finalRechargeInterval != 0 &&
                self.targetSkill.stock < self.targetSkill.maxStock &&
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
