using BepInEx.Configuration;
using HarmonyLib;
using LookingGlass.Base;
using LookingGlass.ItemStatsNameSpace;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using RoR2.UI;
using System;
using System.Collections.Generic;
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
            permanentEquipCooldownText = BasePlugin.instance.Config.Bind<bool>("Misc", "Permanent Cooldown Indicator For Equip", true, "Makes the cooldown indicator for the equip slot permanent and not just when you have 0 stock.");
            permanentSkillCooldownText = BasePlugin.instance.Config.Bind<bool>("Misc", "Permanent Cooldown Indicator For Skills", true, "Makes the cooldown indicator for skills permanent and not just when you have 0 stock.");
 
            new ILHook(
              typeof(SkillIcon).GetMethod(nameof(SkillIcon.Update), BindingFlags.NonPublic | BindingFlags.Instance),
              Show_Skill_CooldownOverride);

            new Hook(AccessTools.PropertyGetter(typeof(EquipmentIcon.DisplayData), nameof(EquipmentIcon.DisplayData.showCooldown)),
                   ShowEquipCooldownOverride);
        }

        public delegate bool orig_EquipCooldown(ref EquipmentIcon.DisplayData self);
        public bool ShowEquipCooldownOverride(orig_EquipCooldown orig, ref global::RoR2.UI.EquipmentIcon.DisplayData self)
        {
            if (permanentEquipCooldownText.Value)
            {
                return self.stock < self.maxStock && self.hasEquipment;
            }
            return orig(ref self);
        }

        public void Show_Skill_CooldownOverride(ILContext il)
        {
            ILCursor c = new(il);
            /*if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdloc(2),
                x => x.MatchLdcI4(0),
                x => x.MatchBgt(out _))
                &&
                c.TryGotoNext(MoveType.After,
                x => x.MatchLdloc(2)))
            {
                c.EmitDelegate<Func<int, int>>((skill) =>
                {
                    if (permanentSkillCooldownText.Value)
                    {
                        return 0;
                    }
                    return skill;
                });
            }
            else
            {
                Debug.LogError("IL FAILED : Show_Skill_CooldownOverride");
            }*/

            c.TryGotoNext(MoveType.After,
            x => x.MatchLdfld("RoR2.UI.SkillIcon", "cooldownText"));
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchLdloc(3)))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, SkillIcon, bool>>((skill, self) =>
                {
                    if (permanentSkillCooldownText.Value && self.targetSkill.finalRechargeInterval != 0)
                    {
                        return !(self.targetSkill.stock < self.targetSkill.maxStock);
                    }
                    return skill;
                });
            }
            else
            {
                Debug.LogError("IL FAILED : Show_Skill_CooldownOverride2");
            }
 
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchLdloc(4)))
            {
                c.EmitDelegate<Func<bool, bool>>((skill) =>
                {
                    if (permanentSkillCooldownText.Value)
                    {
                        return false;
                    }
                    return skill;
                });
            }
            else
            {
                Debug.LogError("IL FAILED : Show_Skill_CooldownOverride2");
            }
        }



        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(permanentEquipCooldownText, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(permanentSkillCooldownText, new CheckBoxConfig() { restartRequired = false }));
        }
       
 
        void Skill_Cooldown(Action<SkillIcon> orig, SkillIcon self)
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
