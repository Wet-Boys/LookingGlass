using BepInEx.Configuration;
using FOSSUI.Base;
using MonoMod.RuntimeDetour;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FOSSUI.AutoSortItems
{
    internal class AutoSortItemsClass : BaseThing
    {

        public static ConfigEntry<bool> SeperateScrap;
        public static ConfigEntry<bool> SortByTier;
        public static ConfigEntry<bool> DescendingTier;
        public static ConfigEntry<bool> SortByStackSize;
        public static ConfigEntry<bool> DescendingStackSize;


        public static AutoSortItemsClass instance;
        RoR2.UI.ItemInventoryDisplay display;
        List<List<ItemIndex>> itemTierLists = new List<List<ItemIndex>>();
        List<ItemIndex> scrapList = new List<ItemIndex>();
        List<ItemIndex> noTierList = new List<ItemIndex>();
        Dictionary<ItemTier, int> tierMatcher = new Dictionary<ItemTier, int>();
        private static Hook overrideHook;
        bool initialized = false;

        public AutoSortItemsClass()
        {
            Setup();
        }
        public void Setup()
        {

            instance = this;
            SeperateScrap = BasePlugin.instance.Config.Bind<bool>("Settings", "Seperate Scrap", true, "Sort's by Scrap");
            SortByTier = BasePlugin.instance.Config.Bind<bool>("Settings", "Tier Sort", true, "Sort's by Tier");
            DescendingTier = BasePlugin.instance.Config.Bind<bool>("Settings", "Descending Tier Sort", true, "Sort's by Tier Descending");
            SortByStackSize = BasePlugin.instance.Config.Bind<bool>("Settings", "Stack Size Sort", true, "Sort's by Stack Size");
            DescendingStackSize = BasePlugin.instance.Config.Bind<bool>("Settings", "Descending Stack Size Sort", true, "Sort's by Stack Size Descending");

            //if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            //{
            //    ROO.rooooooo();
            //}
            SeperateScrap.SettingChanged += SettingsChanged;
            SortByTier.SettingChanged += SettingsChanged;
            DescendingTier.SettingChanged += SettingsChanged;
            SortByStackSize.SettingChanged += SettingsChanged;
            DescendingStackSize.SettingChanged += SettingsChanged;

            InitHooks();
        }
        void InitHooks()
        {
            var targetMethod = typeof(RoR2.UI.ItemInventoryDisplay).GetMethod(nameof(RoR2.UI.ItemInventoryDisplay.UpdateDisplay), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(AutoSortItemsClass).GetMethod(nameof(UpdateDisplayOverride), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
        }
        private void UpdateDisplayOverride(Action<RoR2.UI.ItemInventoryDisplay> orig, RoR2.UI.ItemInventoryDisplay self)
        {
            display = self;
            var temp = self.itemOrder;
            try
            {
                if (!initialized)
                {
                    initialized = true;
                    foreach (var tierList in RoR2.ContentManagement.ContentManager.itemTierDefs)
                    {
                        if (tierList.tier.ToString() == "NoTier")
                        {
                            noTierNum = itemTierLists.Count;
                        }
                        tierMatcher.Add(tierList.tier, itemTierLists.Count);
                        itemTierLists.Add(new List<ItemIndex>());
                    }
                }
                self.itemOrder = SortItems(self.itemOrder, self.itemOrderCount, self);
            }
            catch (Exception e)
            {
                Log.Debug($"Had issue when sorting items: {e}");
            }
            orig(self);
            self.itemOrder = temp;
        }

        int noTierNum;

        private void SettingsChanged(object sender, EventArgs e)
        {
            try
            {
                if (display)
                {
                    display.UpdateDisplay();
                }
            }
            catch (Exception)
            {
            }
        }

        ItemIndex[] SortItems(ItemIndex[] items, int count, RoR2.UI.ItemInventoryDisplay display)
        {
            foreach (var tierList in itemTierLists)
            {
                tierList.Clear();
            }
            scrapList.Clear();
            noTierList.Clear();
            ItemIndex[] newArray = new ItemIndex[count];
            for (int i = 0; i < count; i++)
            {
                if (SeperateScrap.Value && (ItemCatalog.GetItemDef(items[i]).ContainsTag(ItemTag.Scrap) || ItemCatalog.GetItemDef(items[i]).ContainsTag(ItemTag.PriorityScrap)))
                {
                    scrapList.Add(items[i]);
                }
                else if (SortByTier.Value)
                {
                    if (ItemCatalog.GetItemDef(items[i]).tier == ItemTier.NoTier)
                    {
                        noTierList.Add(items[i]);
                    }
                    else
                    {
                        itemTierLists[tierMatcher[ItemCatalog.GetItemDef(items[i]).tier]].Add(items[i]);
                    }
                }
                else
                {
                    newArray[i] = items[i];
                }
            }
            items = newArray;

            if (SortByTier.Value)
            {
                for (int i = 0; i < itemTierLists.Count; i++)
                {
                    itemTierLists[i] = new List<ItemIndex>(itemTierLists[i].OrderBy((item) => (
                    (int)item)
                    + ((DescendingStackSize.Value ? -1 : 1) * (SortByStackSize.Value ? 1 : 0) * display.itemStacks[(int)item] * 20000)).ToArray());
                }
                int num = 0;
                if (SeperateScrap.Value)
                {
                    for (int i = 0; i < scrapList.Count; i++)
                    {
                        items[num] = scrapList[i];
                        num++;
                    }
                }
                if (DescendingTier.Value)
                {
                    for (int i = itemTierLists.Count - 1; i > -1; i--)
                    {
                        for (int x = 0; x < itemTierLists[i].Count; x++)
                        {
                            items[num] = itemTierLists[i][x];
                            num++;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < itemTierLists.Count; i++)
                    {
                        for (int x = 0; x < itemTierLists[i].Count; x++)
                        {
                            items[num] = itemTierLists[i][x];
                            num++;
                        }
                    }
                }
                for (int i = 0; i < noTierList.Count; i++)
                {
                    items[num] = noTierList[i];
                    num++;
                }
            }
            else
            {
                items = items.OrderBy((item) =>
                +((int)item)
                + ((DescendingStackSize.Value ? -1 : 1) * (SortByStackSize.Value ? 1 : 0) * display.itemStacks[(int)item] * 20000)
                ).Distinct().ToArray();
                if (SeperateScrap.Value)
                {
                    int num = 0;
                    for (int i = count - scrapList.Count; i < count; i++)
                    {
                        items[i] = scrapList[num++];
                    }
                }
            }
            return items;
        }
    }
}
