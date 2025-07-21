using BepInEx.Configuration;
using LookingGlass.Base;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;
using System;
using System.Collections.Generic;
using System.Text;
using MonoMod.RuntimeDetour;
using RoR2.UI;
using RoR2;
using UnityEngine.UI;
using TMPro;

namespace LookingGlass.ItemCounters
{
    internal class ItemCounter : BaseThing //https://github.com/MCMrARM/ror2-mods pretty good reference material for this
    {
        public static ConfigEntry<bool> itemCounters;
        public static ConfigEntry<float> itemCountersSize;
        private static Hook overrideHook;
        private static Hook overrideHook2;

        public ItemCounter()
        {
            Setup();
            SetupRiskOfOptions();
        }
        public void Setup()
        {
            itemCounters = BasePlugin.instance.Config.Bind<bool>("Misc", "Item Counters", true, "Counts your items in the scoreboard");
            var targetMethod = typeof(ScoreboardStrip).GetMethod(nameof(ScoreboardStrip.UpdateMoneyText), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(ItemCounter).GetMethod(nameof(UpdateMoneyText), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
            targetMethod = typeof(ScoreboardStrip).GetMethod(nameof(ScoreboardStrip.SetMaster), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(ItemCounter).GetMethod(nameof(SetMaster), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook2 = new Hook(targetMethod, destMethod, this);
        }

        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(itemCounters, new CheckBoxConfig() { restartRequired = false }));
        }

        void SetMaster(Action<ScoreboardStrip, CharacterMaster> orig, ScoreboardStrip self, CharacterMaster newMaster)
        {
            orig(self, newMaster);
            LayoutElement layout = self.itemCountText != null ? self.itemCountText.GetComponent<LayoutElement>() : self.moneyText.GetComponent<LayoutElement>();
            if (layout)
            {
                layout.preferredWidth = 300;
            }
        }

        void UpdateMoneyText(Action<ScoreboardStrip> orig, ScoreboardStrip self)
        {
            orig(self);
            if (!self.master || !self.master.inventory || !itemCounters.Value)
                return;
            TextMeshProUGUI itemCountText = self.itemCountText != null ? self.itemCountText : self.moneyText;
            int whiteCount = self.master.inventory.GetTotalItemCountOfTier(ItemTier.Tier1);
            int greenCount = self.master.inventory.GetTotalItemCountOfTier(ItemTier.Tier2);
            int redCount = self.master.inventory.GetTotalItemCountOfTier(ItemTier.Tier3);
            int lunarCount = self.master.inventory.GetTotalItemCountOfTier(ItemTier.Lunar);
            int bossCount = self.master.inventory.GetTotalItemCountOfTier(ItemTier.Boss);
            int voidWhiteCount = self.master.inventory.GetTotalItemCountOfTier(ItemTier.VoidTier1);
            int voidGreenCount = self.master.inventory.GetTotalItemCountOfTier(ItemTier.VoidTier2);
            int voidRedCount = self.master.inventory.GetTotalItemCountOfTier(ItemTier.VoidTier3);
            int voidBossCount = self.master.inventory.GetTotalItemCountOfTier(ItemTier.VoidBoss);
            int totalItemCount = whiteCount + greenCount + redCount + lunarCount + bossCount + voidWhiteCount + voidGreenCount + voidRedCount + voidBossCount;

            //Replaced font size with % so it can still dynamically be reduced/fitted
            //itemCountText.enableAutoSizing = true;
            //itemCountText.fontSizeMax = 25; //Vanilla extends far for some reason, idk how you'd change bounds?
            StringBuilder sb = new StringBuilder();
            sb.Append($"<size=65%>");
            sb.Append($"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Tier1Item)}>{whiteCount}</color> ");
            sb.Append($"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Tier2Item)}>{greenCount}</color> ");
            sb.Append($"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Tier3Item)}>{redCount}</color> ");
            sb.Append($"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.BossItem)}>{bossCount}</color> ");
            sb.Append($"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.LunarItem)}>{lunarCount}</color> ");
            sb.Append($"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.VoidItem)}>{voidWhiteCount + voidGreenCount + voidRedCount + voidBossCount}</color> ");
            sb.Append($"</size><size=80%><color=#fff>[{totalItemCount}]</color>");
            //Total item should be a little bigger?
            itemCountText.text = sb.ToString();
            
        }
    }
}
