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

        int GetItemsOfTierFast(Inventory inv, ItemTier tier)
        {
            int num = 0;
            foreach (ItemIndex item in inv.itemAcquisitionOrder)
            {
                if (ItemCatalog.GetItemDef(item).tier == tier)
                {
                    num += inv.GetItemCountEffective(item);
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

 
            int whiteCount = GetItemsOfTierFast(self.inventory, ItemTier.Tier1);
            int greenCount = GetItemsOfTierFast(self.inventory, ItemTier.Tier2);
            int redCount = GetItemsOfTierFast(self.inventory, ItemTier.Tier3);
            int lunarCount = GetItemsOfTierFast(self.inventory, ItemTier.Lunar);
            int bossCount = GetItemsOfTierFast(self.inventory, ItemTier.Boss);
            int voidCount = GetItemsOfTierFast(self.inventory, ItemTier.VoidTier1)+ GetItemsOfTierFast(self.inventory, ItemTier.VoidTier2)+ GetItemsOfTierFast(self.inventory, ItemTier.VoidTier3)+ GetItemsOfTierFast(self.inventory, ItemTier.VoidBoss);
            int foodTierCount = GetItemsOfTierFast(self.inventory, ItemTier.FoodTier);
            int totalItemCount = whiteCount + greenCount + redCount + lunarCount + bossCount + voidCount + foodTierCount;


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
            sb.Append($"<color=#FFFFFF>{whiteCount}</color> ");
            sb.Append($"<color=#77FF17>{greenCount}</color> ");
            sb.Append($"<color=#E7543A>{redCount}</color> ");

            if (bossCount > 0 )
            {
                sb.Append($"<color=#FFEB04>{bossCount}</color> ");
            }
            if (lunarCount > 0)
            {
                sb.Append($"<color=#307FFF>{lunarCount}</color> ");
            }
            if (voidCount > 0)
            {
                sb.Append($"<color=#ED7FCD>{voidCount}</color> ");
            }
            if (foodTierCount > 0)
            {
                sb.Append($"<color=#FF8000>{foodTierCount}</color> ");
            }    
            sb.Append($"</size><size=75%><color=#fff>{totalItemCount}</color>");
            //Total item should be a little bigger?
            itemCountText.text = sb.ToString();
            
        }
    }
}
