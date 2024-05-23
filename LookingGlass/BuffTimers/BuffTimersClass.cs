using BepInEx.Configuration;
using LookingGlass.Base;
using LookingGlass.StatsDisplay;
using MonoMod.RuntimeDetour;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using static RoR2.CharacterBody;

namespace LookingGlass.BuffTimers
{
    public class BuffTimersClass : BaseThing
    {
        private static Hook overrideHook;
        public static ConfigEntry<bool> buffTimers;
        public static ConfigEntry<float> buffTimersFontSize;

        public BuffTimersClass()
        {
            Setup();
            SetupRiskOfOptions();
        }
        public void Setup()
        {
            var targetMethod = typeof(BuffDisplay).GetMethod(nameof(BuffDisplay.UpdateLayout), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(BuffTimersClass).GetMethod(nameof(UpdateLayout), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
            buffTimers = BasePlugin.instance.Config.Bind<bool>("Misc", "Buff Timers", true, "Enables buff timers. These are not networked in the base game, please install NetworkedTimedBuffs if you want clients to see them aswell.");
            buffTimersFontSize = BasePlugin.instance.Config.Bind<float>("Misc", "Buff Timers Font Size", 80f, "Changes the font size of buff timers");
        }

        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(buffTimers, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new SliderOption(buffTimersFontSize, new SliderConfig() { restartRequired = false, min = 1, max = 300 }));

        }
        void UpdateLayout(Action<BuffDisplay> orig, BuffDisplay self)
        {
            orig(self);
            if (self.source && StatsDisplayClass.cachedUserBody && buffTimers.Value)
            {
                foreach (var buffIcon in self.buffIcons)
                {
                    foreach (var timedBuff in StatsDisplayClass.cachedUserBody.timedBuffs)
                    {
                        if (timedBuff.buffIndex == buffIcon.buffDef.buffIndex)
                        {
                            TextMeshProUGUI item = buffIcon.transform.GetComponentInChildren<TextMeshProUGUI>();
                            item.enabled = true;
                            item.text = $"<size={buffTimersFontSize.Value}%>{(timedBuff.timer):0.0}</size>\n";
                            if (buffIcon.buffCount > 1)
                            {
                                item.text += $"x{buffIcon.buffCount}";
                            }
                            else
                            {
                                item.text += " ";
                            }
                        }
                    }
                }
            }
        }
    }
}
