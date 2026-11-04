using BepInEx.Configuration;
using LookingGlass.Base;
using LookingGlass.StatsDisplay;
using MonoMod.RuntimeDetour;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2.UI;
using RoR2;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace LookingGlass.BuffTimers
{
    public class BuffTimersClass : BaseThing
    {
        private static Hook overrideHook;
        public static ConfigEntry<bool> buffTimers;
        public static ConfigEntry<bool> buffTimerDecimal;
        public static ConfigEntry<float> buffTimersFontSize;

        public BuffTimersClass()
        {
            Setup();
            SetupRiskOfOptions();
        }
        public void Setup()
        {
            var targetMethod = typeof(BuffDisplay).GetMethod(nameof(BuffDisplay.UpdateLayout), BindingFlags.Public | BindingFlags.Instance);
            var destMethod = typeof(BuffTimersClass).GetMethod(nameof(UpdateLayout), BindingFlags.NonPublic | BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
            buffTimers = BasePlugin.instance.Config.Bind<bool>("Buff Info", "Buff Timers", true, "Enables buff timers. These are not networked in the base game, please install NetworkedTimedBuffs if you want clients to see them aswell.");
            buffTimerDecimal = BasePlugin.instance.Config.Bind<bool>("Buff Info", "Buff Timers Decimal", true, "Display buff timers with 1 decimal. 1.4 instead of rounding up to 2.");
            buffTimersFontSize = BasePlugin.instance.Config.Bind<float>("Buff Info", "Buff Timers Font Size", 80f, "Changes the font size of buff timers");
        }

        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(buffTimers, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(buffTimerDecimal, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new SliderOption(buffTimersFontSize, new SliderConfig() { restartRequired = false, min = 1, max = 300 }));

        }
        void UpdateLayout(Action<BuffDisplay> orig, BuffDisplay self)
        {
            //We ideally would make it so the original code runs always and then just insert it there instead of re-doing it.

            orig(self);
            if (self.source && self.source == StatsDisplayClass.cachedUserBody && buffTimers.Value)
            {
                foreach (var buffIcon in self.buffIconDisplayData)
                {
                    foreach (var timedBuff in StatsDisplayClass.cachedUserBody.timedBuffs)
                    {
                        if (timedBuff.buffIndex == buffIcon.buffIndex)
                        {
                            BuffIcon.sharedStringBuilder.Clear();
                            if (buffTimerDecimal.Value)
                            {
                                BuffIcon.sharedStringBuilder.Append($"<size={buffTimersFontSize.Value}%>{(timedBuff.timer):0.0}</size>\n");
                            }
                            else
                            {
                                BuffIcon.sharedStringBuilder.Append($"<size={buffTimersFontSize.Value}%>{(Mathf.CeilToInt(timedBuff.timer))}</size>\n");
                            }

                            if (buffIcon.buffDef.canStack)
                            {
                                int value = buffIcon.buffCount + buffIcon.buffDef.displayOffset;
                                BuffDef.StackingDisplayMethod stackingDisplayMethod = buffIcon.buffDef.stackingDisplayMethod;
                                if (stackingDisplayMethod != BuffDef.StackingDisplayMethod.Default && stackingDisplayMethod == BuffDef.StackingDisplayMethod.Percentage)
                                {
                                    BuffIcon.sharedStringBuilder.AppendInt(value, 1U, uint.MaxValue);
                                    BuffIcon.sharedStringBuilder.Append("%");
                                }
                                else
                                {
                                    BuffIcon.sharedStringBuilder.Append("x");
                                    BuffIcon.sharedStringBuilder.AppendInt(value, 1U, uint.MaxValue);
                                }
                            }
                            else
                            {
                                BuffIcon.sharedStringBuilder.Append(" "); //Empty space so the timer stays ontop
                            }
                            buffIcon.buffIconComponent.stackCount.enabled = true;
                            buffIcon.buffIconComponent.stackCount.SetText(BuffIcon.sharedStringBuilder);
                        }
                    }
                }
            }
        }
    }
}
