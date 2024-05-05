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

namespace LookingGlass.ItemCounters
{
    internal class ItemCounter : BaseThing
    {
        public static ConfigEntry<bool> itemCounters;
        private static Hook overrideHook;

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
        }

        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(itemCounters, new CheckBoxConfig() { restartRequired = false }));
        }

        void UpdateMoneyText(Action<ScoreboardStrip> orig, ScoreboardStrip self)
        {
            orig(self);
            if (!self.master || !self.master.inventory)
                return;
            LayoutElement layout = self.moneyText.GetComponent<LayoutElement>();
            if (layout)
            {
                layout.preferredWidth = 300;
            }
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

            StringBuilder sb = new StringBuilder();
            sb.Append($"<size={self.moneyText.fontSize * .75f}><color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Tier1Item)}>{whiteCount}</color> ");
            sb.Append($"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Tier2Item)}>{greenCount}</color> ");
            sb.Append($"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Tier3Item)}>{redCount}</color> ");
            sb.Append($"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.BossItem)}>{bossCount}</color> ");
            sb.Append($"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.LunarItem)}>{lunarCount}</color> ");
            sb.Append($"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.VoidItem)}>{voidWhiteCount + voidGreenCount + voidRedCount + voidBossCount}</color> ");
            sb.Append($"<color=#fff>[{totalItemCount}]</color></size>");
            sb.Append($"\n${self.master.money}");
            self.moneyText.text = sb.ToString();
        }
    }
}
