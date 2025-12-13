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
        public static ConfigEntry<bool> tempItemCounters;
        public static ConfigEntry<bool> tempItemCountersTotalCounter;
        public static ConfigEntry<bool> tempItemCountersTotal;
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
            tempItemCounters = BasePlugin.instance.Config.Bind<bool>("Misc", "Temp Item Counters By Rarity", false, "Counts your temp items in the scoreboard separately by rarity");
            tempItemCountersTotalCounter = BasePlugin.instance.Config.Bind<bool>("Misc", "Temp Item Total Counter", true, "Counts your temp item total separately");
            tempItemCountersTotal = BasePlugin.instance.Config.Bind<bool>("Misc", "Include Temp Items In Item Counter Totals", false, "Include temp items in the item counter totals");
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
            ModSettingsManager.AddOption(new CheckBoxOption(tempItemCounters, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(tempItemCountersTotalCounter, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(tempItemCountersTotal, new CheckBoxConfig() { restartRequired = false }));
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

        int GetItemsOfTierFast(Inventory inv, ItemTier tier, bool includeTemp = true)
        {
            int num = 0;
            foreach (ItemIndex item in inv.itemAcquisitionOrder)
            {
                if (ItemCatalog.GetItemDef(item).tier == tier)
                {
                    if (includeTemp)
                        num += inv.GetItemCountEffective(item);
                    else
                        num += inv.GetItemCountPermanent(item);
                }
            }
            return num;
        }

        int GetTempItemsOfTierFast(Inventory inv, ItemTier tier)
        {
            int num = 0;
            foreach (ItemIndex item in inv.itemAcquisitionOrder)
            {
                if (ItemCatalog.GetItemDef(item).tier == tier)
                {
                    num += inv.GetItemCountTemp(item);
                }
            }
            return num;
        }

        void UpdateMoneyText(Action<ScoreboardStrip> orig, ScoreboardStrip self)
        {
            orig(self);
            if (!self.inventory || !itemCounters.Value)
                return;
            TextMeshProUGUI itemCountText = self.itemCountText != null ? self.itemCountText : self.moneyText;
            //int whiteCount = self.master.inventory.GetTotalItemCountOfTier(ItemTier.Tier1);
            //int greenCount = self.master.inventory.GetTotalItemCountOfTier(ItemTier.Tier2);
            //int redCount = self.master.inventory.GetTotalItemCountOfTier(ItemTier.Tier3);

            bool tempItemCheck = tempItemCountersTotal.Value;

            int whiteCount = GetItemsOfTierFast(self.inventory, ItemTier.Tier1, tempItemCheck);
            int greenCount = GetItemsOfTierFast(self.inventory, ItemTier.Tier2, tempItemCheck);
            int redCount = GetItemsOfTierFast(self.inventory, ItemTier.Tier3, tempItemCheck);
            int lunarCount = GetItemsOfTierFast(self.inventory, ItemTier.Lunar, tempItemCheck);
            int bossCount = GetItemsOfTierFast(self.inventory, ItemTier.Boss, tempItemCheck);
            int voidCount = GetItemsOfTierFast(self.inventory, ItemTier.VoidTier1, tempItemCheck) + GetItemsOfTierFast(self.inventory, ItemTier.VoidTier2, tempItemCheck) + GetItemsOfTierFast(self.inventory, ItemTier.VoidTier3, tempItemCheck) + GetItemsOfTierFast(self.inventory, ItemTier.VoidBoss, tempItemCheck);
            int foodTierCount = GetItemsOfTierFast(self.inventory, ItemTier.FoodTier, tempItemCheck);
            int totalItemCount = whiteCount + greenCount + redCount + lunarCount + bossCount + voidCount + foodTierCount;

            int tempWhiteCount = GetTempItemsOfTierFast(self.inventory, ItemTier.Tier1);
            int tempGreenCount = GetTempItemsOfTierFast(self.inventory, ItemTier.Tier2);
            int tempRedCount = GetTempItemsOfTierFast(self.inventory, ItemTier.Tier3);
            int tempLunarCount = GetTempItemsOfTierFast(self.inventory, ItemTier.Lunar);
            int tempBossCount = GetTempItemsOfTierFast(self.inventory, ItemTier.Boss);
            int tempVoidCount = GetTempItemsOfTierFast(self.inventory, ItemTier.VoidTier1) + GetTempItemsOfTierFast(self.inventory, ItemTier.VoidTier2) + GetTempItemsOfTierFast(self.inventory, ItemTier.VoidTier3) + GetTempItemsOfTierFast(self.inventory, ItemTier.VoidBoss);
            int tempFoodTierCount = GetTempItemsOfTierFast(self.inventory, ItemTier.FoodTier);
            int tempTotalItemCount = tempWhiteCount + tempGreenCount + tempRedCount + tempLunarCount + tempBossCount + tempVoidCount + tempFoodTierCount;

            //Made TotalItems larger
            //Removed the [] because it just kinda misaligned it
            //Made it resize so it doesn't get shrunk into ... with very large amounts of items.
            //But also keeps it larger at low item counts for readability.

            itemCountText.fontSizeMax = 28; //Kinda breaks if above, but keeps it in line
            //Tho clips a bit
            //Doubt other huds would change anything about the spacing but might be worth checking
            itemCountText.enableAutoSizing = true;
            StringBuilder sb = new StringBuilder();
            sb.Append($"<size=60%>");
            //sb.Append($"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Tier1Item)}>{whiteCount}</color> ");

            //Temp vanilla : <color=#8bc7d5>
            //Temp more sat: <color=#6ADCF6>

            sb.Append($"<color=#FFFFFF>{whiteCount}</color>");
            if (tempItemCounters.Value && tempWhiteCount > 0)
                sb.Append($"</size><size=40%><style=cIsTemporary>({tempWhiteCount})</color></size><size=60%> ");
            else sb.Append(" ");

                sb.Append($"<color=#77FF17>{greenCount}</color>");
            if (tempItemCounters.Value && tempGreenCount > 0)
                sb.Append($"</size><size=40%><style=cIsTemporary>({tempGreenCount})</color></size><size=60%> ");
            else sb.Append(" ");

            sb.Append($"<color=#E7543A>{redCount}</color>");
            if (tempItemCounters.Value && tempRedCount > 0)
                sb.Append($"</size><size=40%><style=cIsTemporary>({tempRedCount})</color></size><size=60%> ");
            else sb.Append(" ");

            if (bossCount > 0 || tempBossCount > 0)
            {
                sb.Append($"<color=#FFEB04>{bossCount}</color>");
                if (tempItemCounters.Value && tempBossCount > 0)
                    sb.Append($"</size><size=40%><style=cIsTemporary>({tempBossCount})</color></size><size=60%> ");
                else sb.Append(" ");
            }
            if (lunarCount > 0 || tempLunarCount > 0)
            {
                sb.Append($"<color=#307FFF>{lunarCount}</color>");
                if (tempItemCounters.Value && tempLunarCount > 0)
                    sb.Append($" </size><size=40%><style=cIsTemporary>({tempLunarCount})</color></size><size=60%> ");
                else sb.Append(" ");
            }
            if (voidCount > 0 || tempVoidCount > 0)
            {
                sb.Append($"<color=#ED7FCD>{voidCount}</color>");
                if (tempItemCounters.Value && tempVoidCount > 0)
                    sb.Append($"</size><size=40%><style=cIsTemporary>({tempVoidCount})</color></size><size=60%> ");
                else sb.Append(" ");
            }
            if (foodTierCount > 0 || tempFoodTierCount > 0)
            {
                sb.Append($"<color=#FF8000>{foodTierCount}</color>");
                if (tempItemCounters.Value && tempFoodTierCount > 0)
                    sb.Append($"</size><size=40%><style=cIsTemporary>({tempFoodTierCount})</color></size><size=60%> ");
                else sb.Append(" ");
            }    
            sb.Append($"</size><size=75%><color=#fff>{totalItemCount}</color>");
            if ((tempItemCounters.Value || tempItemCountersTotalCounter.Value) && tempTotalItemCount > 0)
                sb.Append($"<size=50%><style=cIsTemporary>({tempTotalItemCount})</color></size><size=75%>");

            //Total item should be a little bigger?
            itemCountText.text = $"{sb.ToString()}";
            
        }
    }
}
