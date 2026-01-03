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
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LookingGlass.ItemCounters
{
    internal class ItemCounter : BaseThing //https://github.com/MCMrARM/ror2-mods pretty good reference material for this
    {
        public static ConfigEntry<bool> cfg_TieredItemCounters;
        //public static ConfigEntry<float> cfgitemCountersSize;
        public static ConfigEntry<bool> cfg_TieredTempCounters;
        public static ConfigEntry<bool> cfg_TotalTempCounter;
        public static ConfigEntry<bool> cfg_EffectiveCount;
 
        public ItemCounter()
        {
            Setup();
            SetupRiskOfOptions();
        }
        public void Setup()
        {
            cfg_EffectiveCount = BasePlugin.instance.Config.Bind<bool>("Item Counters", "Include Temp Items In Item Counter Totals", false, "The main item counters will count both permanent and temporary items.");
            cfg_TotalTempCounter = BasePlugin.instance.Config.Bind<bool>("Item Counters", "Temp Item Total Counter", true, "Counts your temp item total separately");
            cfg_TieredItemCounters = BasePlugin.instance.Config.Bind<bool>("Item Counters", "Tiered Item Counters", true, "Adds tiered item counters to the scoreboard next to the total item counter.");
            cfg_TieredTempCounters = BasePlugin.instance.Config.Bind<bool>("Item Counters", "Temp Item Counters By Rarity", false, "Counts your temp items in the scoreboard separately by rarity");

            cfg_EffectiveCount.SettingChanged += Cfg_EffectiveCount_SettingChanged;
            cfg_TotalTempCounter.SettingChanged += Cfg_EffectiveCount_SettingChanged;
            cfg_TieredItemCounters.SettingChanged += Cfg_EffectiveCount_SettingChanged;
            cfg_TieredTempCounters.SettingChanged += Cfg_EffectiveCount_SettingChanged;

            var targetMethod = typeof(ScoreboardStrip).GetMethod(nameof(ScoreboardStrip.UpdateItemCountText), BindingFlags.NonPublic | BindingFlags.Instance);
            new Hook(targetMethod, UpdateItemCountText);
            targetMethod = typeof(ScoreboardStrip).GetMethod(nameof(ScoreboardStrip.SetMaster), BindingFlags.Public | BindingFlags.Instance);
            new Hook(targetMethod, SetMaster);

            targetMethod = typeof(ItemInventoryDisplay).GetMethod(nameof(ItemInventoryDisplay.UpdateDisplay), BindingFlags.Public | BindingFlags.Instance);
            new Hook(targetMethod, SetNeedUpdate);

            /*targetMethod = typeof(ScoreboardController).GetMethod(nameof(ScoreboardController.OnEnable), BindingFlags.NonPublic | BindingFlags.Instance);
            new Hook(targetMethod, ScoreboardController_onScoreboardOpen);*/

        }

        private void Cfg_EffectiveCount_SettingChanged(object sender, EventArgs e)
        {
            needToUpdate = 0.1f;
        }

        private void ScoreboardController_onScoreboardOpen(Action<ScoreboardController> orig, ScoreboardController self)
        {
            orig(self);
        }

        void SetNeedUpdate(Action<ItemInventoryDisplay> orig, ItemInventoryDisplay self)
        {
            orig(self);
            needToUpdate = 0.1f;
        }


        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(cfg_EffectiveCount, new CheckBoxConfig() { name = "Count Perm & Temp together for counters", restartRequired = false }));

            ModSettingsManager.AddOption(new CheckBoxOption(cfg_TotalTempCounter, new CheckBoxConfig() {name ="Total Temp Items Counter", restartRequired = false }));
          
            ModSettingsManager.AddOption(new CheckBoxOption(cfg_TieredItemCounters, new CheckBoxConfig() { restartRequired = false}));
           
            ModSettingsManager.AddOption(new CheckBoxOption(cfg_TieredTempCounters, new CheckBoxConfig() { name = "Tiered Temp Counters", restartRequired = false, checkIfDisabled = cfgEffective }));
         
        }
        public bool cfgEffective()
        {
            return  !cfg_TieredItemCounters.Value || cfg_EffectiveCount.Value;
        }

        void SetMaster(Action<ScoreboardStrip, CharacterMaster> orig, ScoreboardStrip self, CharacterMaster newMaster)
        {
            orig(self, newMaster);
            /*LayoutElement layout = self.itemCountText != null ? self.itemCountText.GetComponent<LayoutElement>() : self.moneyText.GetComponent<LayoutElement>();
            if (layout)
            {
                //layout.preferredWidth = 300;
            }*/
            if (self.itemCountText != null)
            {
                self.itemCountText.m_maxFontSize = 30;
                self.itemCountText.enableAutoSizing = true;
                needToUpdate = 0.1f;
            }
           /* if (self.itemCountText && self.itemCountText.transform.childCount == 0)
            {
                GameObject counters2 = GameObject.Instantiate(self.itemCountText.gameObject, self.itemCountText.transform);
                self.itemCountText.transform.localPosition = new Vector3(-10f, -5f, 0);
                counters2.transform.localPosition = new Vector3(0f, -17f, 0);
            }*/
        }
        public static int[] itemCountsPerm = Array.Empty<int>();
        public static int[] itemCountsTemp = Array.Empty<int>();
        public float needToUpdate = 1f; 
 
        public static int NewGetTotalItemStacks(ItemCollection inv, int[] array)
        {
            int num = 0;
            ReadOnlySpan<int> itemCounts = inv.inner.GetValuesSpan();
            ReadOnlySpan<SparseIndex> itemIndices = inv.inner.GetNonDefaultIndicesSpan();
            for (int i = 0; i < itemIndices.Length; i++)
            {
                SparseIndex index = itemIndices[i];
                num += itemCounts[(int)index];
                array[(int)ItemCatalog.GetItemDef((ItemIndex)index).tier] += itemCounts[(int)index];
            }
            return num;
        }


        //Temp vanilla : <color=#8bc7d5> //It's a bit hard to see especially with such small numbers.
        //Temp 125% sat: <color=#6FD8F1>
        //Temp 150% Sat: <color=#53E9FF>

        public void UpdateItemCountText(Action<ScoreboardStrip> orig, ScoreboardStrip self)
        {
            //Why not just skip orig(self) here tbh
            if (!self.inventory || (!cfg_TieredItemCounters.Value && !cfg_TotalTempCounter.Value))
            {
                orig(self); 
                return;
            }
            if (needToUpdate < 0) 
            {
                return;
            }
            needToUpdate -= Time.fixedDeltaTime;
            //Debug.Log("UpdateItemCountText");
            

            itemCountsPerm = new int[ItemTierCatalog.itemTierDefs.Length+2];
            itemCountsTemp = new int[ItemTierCatalog.itemTierDefs.Length+2];

            int totalItems = NewGetTotalItemStacks(self.inventory.permanentItemStacks, itemCountsPerm);
            int tempItems = NewGetTotalItemStacks(self.inventory.tempItemsStorage.tempItemStacks, cfg_EffectiveCount.Value ? itemCountsPerm : itemCountsTemp);

            totalItems -= itemCountsPerm[5]; //Remove Untiered items;
            tempItems -= itemCountsTemp[5]; //Easier than needing to filter them out probably

            if (cfg_EffectiveCount.Value)
            {
                totalItems += tempItems;
            }

            int voidCount = itemCountsPerm[6] + itemCountsPerm[7] + itemCountsPerm[8] + itemCountsPerm[9];
            int voidCountT = itemCountsTemp[6] + itemCountsTemp[7] + itemCountsTemp[8] + itemCountsTemp[9];
       
            StringBuilder sb = new StringBuilder();
            if (cfg_TieredItemCounters.Value)
            {
                sb.Append($"<size=60%>");
                sb.Append($"<color=#FFFFFF>{itemCountsPerm[0]}</color>");
                if (cfg_TieredTempCounters.Value && itemCountsTemp[0] > 0)
                    sb.Append($"</size><size=40%><color=#6FD8F1>[{itemCountsTemp[0]}]</color></size><size=60%> ");
 
                sb.Append($" <color=#77FF17>{itemCountsPerm[1]}</color>");
                if (cfg_TieredTempCounters.Value && itemCountsTemp[1] > 0)
                    sb.Append($"</size><size=40%><color=#6FD8F1>[{itemCountsTemp[1]}]</color></size><size=60%> ");
     

                sb.Append($" <color=#E7543A>{itemCountsPerm[2]}</color>");
                if (cfg_TieredTempCounters.Value && itemCountsTemp[2] > 0)
                    sb.Append($"</size><size=40%><color=#6FD8F1>[{itemCountsTemp[2]}]</color></size><size=60%>");
 

                if (itemCountsPerm[4] > 0 || itemCountsTemp[4] > 0)
                {
                    sb.Append($" <color=#FFEB04>{itemCountsPerm[4]}</color>");
                    if (cfg_TieredTempCounters.Value && itemCountsTemp[4] > 0)
                        sb.Append($"</size><size=40%><color=#6FD8F1>[{itemCountsTemp[4]}]</color></size><size=60%>");
     
                }
                if (itemCountsPerm[3] > 0 || itemCountsTemp[3] > 0)
                {
                    sb.Append($" <color=#307FFF>{itemCountsPerm[3]}</color>");
                    if (cfg_TieredTempCounters.Value && itemCountsTemp[3] > 0)
                        sb.Append($"</size><size=40%><color=#6FD8F1>[{itemCountsTemp[3]}]</color></size><size=60%>");
 
                }
                if (voidCount > 0 || voidCountT > 0)
                {
                    sb.Append($" <color=#ED7FCD>{voidCount}</color>");
                    if (cfg_TieredTempCounters.Value && voidCountT > 0)
                        sb.Append($"</size><size=40%><color=#6FD8F1>[{voidCountT}]</color></size><size=60%> ");
  
                }
                if (itemCountsPerm[10] > 0 || itemCountsTemp[10] > 0)
                {
                    sb.Append($" <color=#FF8000>{itemCountsPerm[10]}</color>");
                    if (cfg_TieredTempCounters.Value && itemCountsTemp[10] > 0)
                        sb.Append($"</size><size=40%><color=#6FD8F1>[{itemCountsTemp[10]}]</color></size><size=60%> ");
 
                }
                sb.Append($" </size><size=75%><color=#fff>{totalItems}</color>");
                if ((cfg_TotalTempCounter.Value) && tempItems > 0)
                {
                    sb.Append($"<size=50%><color=#6FD8F1>[{tempItems}]</color></size><size=75%>");
                }
            }
            else if (cfg_TotalTempCounter.Value)
            {
                sb.Append($" </size><size=100%><color=#fff>{totalItems}</color>");
                if (tempItems > 0)
                {
                    sb.Append($"<size=70%><color=#6FD8F1>[{tempItems}]</color></size><size=75%>");
                }
            }
                
            //Game does null check itemCountText, neither do we have to th  
            self.itemCountText.text = $"{sb.ToString()}";
 
        }
 
        /*
        void OLDUpdateItemCountText(Action<ScoreboardStrip> orig, ScoreboardStrip self)
        {
            //Why not just skip orig(self) here tbh
            if (!self.inventory || !cfg_TieredItemCounters.Value)
            {
                orig(self);
                return;
            }
            if (!needToUpdate)
            {
                return;
            }
            itemCountsPerm = new int[ItemTierCatalog.itemTierDefs.Length);
            itemCountsTemp = new int[ItemTierCatalog.itemTierDefs.Length];

            bool tempItemCheck = cfg_MergeCounters.Value;

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

            //itemCountText.fontSizeMax = 28; //Kinda breaks if above, but keeps it in line
            //Tho clips a bit
            //Doubt other huds would change anything about the spacing but might be worth checking

            StringBuilder sb = new StringBuilder();

            //Temp vanilla : <color=#8bc7d5> //It's a bit hard to see especially with such small numbers.
            //Temp 125% sat: <color=#6FD8F1>
            //Temp 150% Sat: <color=#53E9FF>

            sb.Append($"<size=60%>");
            sb.Append($"<color=#FFFFFF>{whiteCount}</color>");
            if (cfg_TieredTempCounters.Value && tempWhiteCount > 0)
                sb.Append($"</size><size=40%><color=#6FD8F1>({tempWhiteCount}]</color></size><size=60%> ");
            else sb.Append(" ");

            sb.Append($"<color=#77FF17>{greenCount}</color>");
            if (cfg_TieredTempCounters.Value && tempGreenCount > 0)
                sb.Append($"</size><size=40%><color=#6FD8F1>({tempGreenCount}]</color></size><size=60%> ");
            else sb.Append(" ");

            sb.Append($"<color=#E7543A>{redCount}</color>");
            if (cfg_TieredTempCounters.Value && tempRedCount > 0)
                sb.Append($"</size><size=40%><color=#6FD8F1>({tempRedCount}]</color></size><size=60%> ");
            else sb.Append(" ");

            if (bossCount > 0 || tempBossCount > 0)
            {
                sb.Append($"<color=#FFEB04>{bossCount}</color>");
                if (cfg_TieredTempCounters.Value && tempBossCount > 0)
                    sb.Append($"</size><size=40%><color=#6FD8F1>({tempBossCount}]</color></size><size=60%> ");
                else sb.Append(" ");
            }
            if (lunarCount > 0 || tempLunarCount > 0)
            {
                sb.Append($"<color=#307FFF>{lunarCount}</color>");
                if (cfg_TieredTempCounters.Value && tempLunarCount > 0)
                    sb.Append($"</size><size=40%><color=#6FD8F1>({tempLunarCount}]</color></size><size=60%> ");
                else sb.Append(" ");
            }
            if (voidCount > 0 || tempVoidCount > 0)
            {
                sb.Append($"<color=#ED7FCD>{voidCount}</color>");
                if (cfg_TieredTempCounters.Value && tempVoidCount > 0)
                    sb.Append($"</size><size=40%><color=#6FD8F1>({tempVoidCount}]</color></size><size=60%> ");
                else sb.Append(" ");
            }
            if (foodTierCount > 0 || tempFoodTierCount > 0)
            {
                sb.Append($"<color=#FF8000>{foodTierCount}</color>");
                if (cfg_TieredTempCounters.Value && tempFoodTierCount > 0)
                    sb.Append($"</size><size=40%><color=#6FD8F1>({tempFoodTierCount}]</color></size><size=60%> ");
                else sb.Append(" ");
            }
            sb.Append($"</size><size=75%><color=#fff>{totalItemCount}</color>");
            if ((cfg_TieredTempCounters.Value || cfg_TotalTempCounter.Value) && tempTotalItemCount > 0)
                sb.Append($"<size=50%><color=#6FD8F1>({tempTotalItemCount}]</color></size><size=75%>");


            //Game does null check itemCountText, neither do we have to th  
            self.itemCountText.text = $"{sb.ToString()}";

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

        */
    }
}
