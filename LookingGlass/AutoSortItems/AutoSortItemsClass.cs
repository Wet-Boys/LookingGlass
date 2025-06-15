using BepInEx.Configuration;
using LookingGlass.Base;
using MonoMod.RuntimeDetour;
using RiskOfOptions.Components.Options;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoR2.UI;
using static Rewired.InputMapper;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using LookingGlass.ResizeCommandWindow;
using System.Collections;
using UnityEngine.EventSystems;

namespace LookingGlass.AutoSortItems
{
    internal class AutoSortItemsClass : BaseThing
    {
        internal enum ScrapSortMode
        {
            Start,
            End,
            Mixed
        }

        internal enum TierSortMode
        {
            Off,
            Tier,
            TierIgnoringAcquiredOrder
        }

        internal enum SortMode
        {
            Off,
            StackSize,
            StackSizeDecending,
            Alphabetical,
            AlphabeticalDecending,
        }

        public static ConfigEntry<ScrapSortMode> ScrapSorting;
        public static ConfigEntry<TierSortMode> SortByTier;
        public static ConfigEntry<string> TierOrder;
        public static ConfigEntry<bool> CombineVoidTiers;

        public static ConfigEntry<SortMode> InventorySorting;
        public static ConfigEntry<bool> SortInventoryTier;

        public static ConfigEntry<SortMode> CommandSorting;

        public static ConfigEntry<SortMode> ScrapperSorting;
        public static ConfigEntry<bool> SortScrapperTier;


        public static AutoSortItemsClass instance;
        RoR2.UI.ItemInventoryDisplay display;
        List<List<ItemIndex>> itemTierLists = new List<List<ItemIndex>>();
        List<ItemIndex> scrapList = new List<ItemIndex>();
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
            ScrapSorting = BasePlugin.instance.Config.Bind("Auto Sort Items", "Scrap Sorting", ScrapSortMode.Start, "Where scrap should be sorted");
            SortByTier = BasePlugin.instance.Config.Bind("Auto Sort Items", "Tier Sort", TierSortMode.Tier, "How Tiers should be sorted");
            TierOrder = BasePlugin.instance.Config.Bind<string>("Auto Sort Items", "Tier Order", "Lunar VoidBoss Boss VoidTier3 Tier3 VoidTier2 Tier2 VoidTier1 Tier1 NoTier", "How the tiers should be ordered");
            CombineVoidTiers = BasePlugin.instance.Config.Bind<bool>("Auto Sort Items", "Combine Normal And Void Tiers", false, "Considers void tiers to be the same as their normal counterparts");

            InventorySorting = BasePlugin.instance.Config.Bind("Auto Sort Items", "Inventory Sorting", SortMode.StackSizeDecending, "How should the inventory be sorted");
            SortInventoryTier = BasePlugin.instance.Config.Bind("Auto Sort Items", "Inventory Sort Tiers", true, "Should the inventory be sorted by tiers");

            CommandSorting = BasePlugin.instance.Config.Bind("Auto Sort Items", "Command Sorting", SortMode.StackSizeDecending, "How should the command menu be sorted");

            ScrapperSorting = BasePlugin.instance.Config.Bind("Auto Sort Items", "Scrapper Sorting", SortMode.StackSizeDecending, "How should the scrapper menu be sorted");
            SortScrapperTier = BasePlugin.instance.Config.Bind<bool>("Auto Sort Items", "Sort Scrapper Tier", true, "Sorts Scrapper by tier");


            ScrapSorting.SettingChanged += SettingsChanged;
            SortByTier.SettingChanged += SettingsChanged;
            TierOrder.SettingChanged += SettingsChanged;
            CombineVoidTiers.SettingChanged += SettingsChanged;
            InventorySorting.SettingChanged += SettingsChanged;
            SortInventoryTier.SettingChanged += SettingsChanged;
            CommandSorting.SettingChanged += SettingsChanged;
            ScrapperSorting.SettingChanged += SettingsChanged;
            SortScrapperTier.SettingChanged += SettingsChanged;

            InitHooks();
            SetupRiskOfOptions();
        }

        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new ChoiceOption(ScrapSorting, new ChoiceConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new ChoiceOption(SortByTier, new ChoiceConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new StringInputFieldOption(TierOrder, new InputFieldConfig() { restartRequired = false, checkIfDisabled = CheckTierSort, lineType = TMPro.TMP_InputField.LineType.MultiLineSubmit, submitOn = InputFieldConfig.SubmitEnum.OnExitOrSubmit }));
            ModSettingsManager.AddOption(new GenericButtonOption("Use Descending Tiers Preset", "Auto Sort Items", "Sets the Tier Order option to use descending tiers", "Set", SetDescendingTiers));
            ModSettingsManager.AddOption(new GenericButtonOption("Use Ascending Tiers Preset", "Auto Sort Items", "Sets the Tier Order option to use ascending tiers", "Set", SetAscendingTiers));
            ModSettingsManager.AddOption(new CheckBoxOption(CombineVoidTiers, new CheckBoxConfig() { restartRequired = false, checkIfDisabled = CheckTierSort }));

            ModSettingsManager.AddOption(new ChoiceOption(InventorySorting, new ChoiceConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(SortInventoryTier, new CheckBoxConfig() { restartRequired = false, checkIfDisabled = CheckSortInventoryTier }));

            ModSettingsManager.AddOption(new ChoiceOption(CommandSorting, new ChoiceConfig() { restartRequired = false }));

            ModSettingsManager.AddOption(new ChoiceOption(ScrapperSorting, new ChoiceConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(SortScrapperTier, new CheckBoxConfig() { restartRequired = false, checkIfDisabled = CheckSortScrapperTier }));
        }
        //I'm going cross-eyed looking at all these
        private static bool CheckTierSort()
        {
            return SortByTier.Value == TierSortMode.Off;
        }

        private static bool CheckSortInventoryTier()
        {
            return SortByTier.Value == TierSortMode.Off || InventorySorting.Value == SortMode.Off || InventorySorting.Value == SortMode.Alphabetical || InventorySorting.Value == SortMode.AlphabeticalDecending;
        }

        private static bool CheckSortScrapperTier()
        {
            return SortByTier.Value == TierSortMode.Off || ScrapperSorting.Value == SortMode.Off || ScrapperSorting.Value == SortMode.Alphabetical || ScrapperSorting.Value == SortMode.AlphabeticalDecending;
        }

        private void SetDescendingTiers()
        {
            // manually update the tier order option in the menu (which also updates the setting)
            foreach (var controller in UnityEngine.Object.FindObjectsOfType<InputFieldController>())
            {
                if (controller.name.Contains("Tier Order"))
                {
                    controller.SubmitValue("Lunar VoidBoss Boss VoidTier3 Tier3 VoidTier2 Tier2 VoidTier1 Tier1 NoTier");
                }
            }
        }
        private void SetAscendingTiers()
        {
            foreach (var controller in UnityEngine.Object.FindObjectsOfType<InputFieldController>())
            {
                if (controller.name.Contains("Tier Order"))
                {
                    controller.SubmitValue("Tier1 VoidTier1 Tier2 VoidTier2 Tier3 VoidTier3 Boss VoidBoss Lunar NoTier");
                }
            }
        }

        internal (PickupPickerController.Option[], List<int>) SortPickupPicker(PickupPickerController.Option[] options, string parentName)
        {
            SortMode mode;
            bool sortTiers;
            bool isCommand = parentName.StartsWith("CommandPickerPanel");
            bool isScrapper = parentName.StartsWith("ScrapperPickerPanel");

            if (isCommand) { mode = CommandSorting.Value; sortTiers = false; }
            else if (isScrapper) { mode = ScrapperSorting.Value; sortTiers = SortScrapperTier.Value; }
            else { return (options, [-1]); }

            if (mode == SortMode.Off) { return (options, [-1]); }


            List<ItemIndex> items = new List<ItemIndex>();
            List<ItemIndex> unSortedItems = new List<ItemIndex>(); //used to make mapping of what changed when sorting
            List<int> mapping = new List<int>(options.Length);

            for (int i = 0; i < options.Length; i++)
            {
                ItemIndex itemIndex = PickupCatalog.GetPickupDef(options[i].pickupIndex).itemIndex;
                if (itemIndex == ItemIndex.None)
                {
                    return (options, [-1]); // -1 indicates no sorting
                }
                items.Add(itemIndex);
                unSortedItems.Add(itemIndex);
            }

            //sort items
            items = new List<ItemIndex>(SortItems(items.ToArray(), items.Count, display, mode, sortTiers));


            // make mapping of what was changed
            mapping = Enumerable.ToList(Enumerable.Select(items, (ItemIndex item) => unSortedItems.IndexOf(item)));

            // apply mapping to options
            PickupPickerController.Option[] sortedOptions = Enumerable.ToArray(Enumerable.Select(mapping, (int index) => options[index]));

            return (sortedOptions, mapping);
        }
        IEnumerator ReOrganizeItems(PickupPickerController.Option[] options, PickupPickerPanel self)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            List<MPButton> buttons = new List<MPButton>();
            foreach (var item in self.GetComponentInChildren<GridLayoutGroup>().GetComponentsInChildren<MPButton>())
            {
                if (item.gameObject.activeSelf)
                {
                    buttons.Add(item);
                }
            }
            for (int j = 0; j < options.Length; j++)
            {
                MPButton mpbutton = buttons[j];
                //Log.Debug($"{Language.GetString(mpbutton.GetComponent<TooltipProvider>().titleToken)}");
                int num = j - j % self.maxColumnCount;
                int num2 = j % self.maxColumnCount;
                int num3 = num2 - self.maxColumnCount;
                int num4 = num2 - 1;
                int num5 = num2 + 1;
                int num6 = num2 + self.maxColumnCount;
                //Log.Debug($"num[{num}] num2[{num2}] num3[{num3}] num4[{num4}] num5[{num5}] num6[{num6}] ");
                Navigation navigation = mpbutton.navigation;
                navigation.mode = Navigation.Mode.Explicit;
                navigation.selectOnRight = null;
                navigation.selectOnLeft = null;
                navigation.selectOnUp = null;
                navigation.selectOnDown = null;
                if (num4 >= 0)
                {
                    MPButton mpbutton2 = buttons[num + num4];
                    //Log.Debug($"selectOnLeft :{Language.GetString(mpbutton2.GetComponent<TooltipProvider>().titleToken)}");
                    navigation.selectOnLeft = mpbutton2;
                }
                if (num5 < self.maxColumnCount && num + num5 < options.Length)
                {
                    MPButton mpbutton3 = buttons[num + num5];
                    //Log.Debug($"selectOnRight :{Language.GetString(mpbutton3.GetComponent<TooltipProvider>().titleToken)}");
                    navigation.selectOnRight = mpbutton3;
                }
                if (num + num3 >= 0)
                {
                    MPButton mpbutton4 = buttons[num + num3];
                    //Log.Debug($"selectOnUp :{Language.GetString(mpbutton4.GetComponent<TooltipProvider>().titleToken)}");
                    navigation.selectOnUp = mpbutton4;
                }
                if (num + num6 < options.Length)
                {
                    MPButton mpbutton5 = buttons[num + num6];
                    //Log.Debug($"selectOnDown :{Language.GetString(mpbutton5.GetComponent<TooltipProvider>().titleToken)}");
                    navigation.selectOnDown = mpbutton5;
                }
                mpbutton.navigation = navigation;
            }
            EventSystem.current.SetSelectedGameObject(buttons.First().gameObject);
        }
        void InitHooks()
        {
            var targetMethod = typeof(RoR2.UI.ItemInventoryDisplay).GetMethod(nameof(RoR2.UI.ItemInventoryDisplay.UpdateDisplay), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(AutoSortItemsClass).GetMethod(nameof(UpdateDisplayOverride), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
        }
        private void UpdateDisplayOverride(Action<RoR2.UI.ItemInventoryDisplay> orig, RoR2.UI.ItemInventoryDisplay self)
        {
            if (self != null)
            {
                display = self;
                var temp = self.itemOrder;

                if (tierMatcher.Count == 0) //initialize tierMatcher if not done already
                {
                    SetupTierMatcher();
                }

                try
                {
                    self.itemOrder = SortItems(self.itemOrder, self.itemOrderCount, self, InventorySorting.Value, SortInventoryTier.Value);
                }
                catch (Exception e)
                {
                    Log.Debug($"Had issue when sorting items: {e}");
                }
                orig(self);
                self.itemOrder = temp;
            }
            else
            {
                orig(self);
            }
        }

        private void SettingsChanged(object sender, EventArgs e)
        {
            // force re-initialization

            SetupTierMatcher();
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

        private void SetupTierMatcher()
        {
            tierMatcher.Clear();
            itemTierLists.Clear();
            foreach (string tierString in TierOrder.Value.Split(' '))
            {
                if (Enum.TryParse(tierString, out ItemTier tier) && !tierMatcher.ContainsKey(tier))
                {
                    tierMatcher.Add(tier, itemTierLists.Count);
                    itemTierLists.Add(new List<ItemIndex>());
                }
            }
            foreach (var tierDef in RoR2.ContentManagement.ContentManager.itemTierDefs)
            {
                if (!tierMatcher.ContainsKey(tierDef.tier)) // use default ordering for any not present in the setting
                {
                    tierMatcher.Add(tierDef.tier, itemTierLists.Count);
                    itemTierLists.Add(new List<ItemIndex>());
                }
            }
            // apparently this is just not in itemTierDefs? wack
            if (!tierMatcher.ContainsKey(ItemTier.NoTier))
            {
                tierMatcher.Add(ItemTier.NoTier, itemTierLists.Count);
                itemTierLists.Add(new List<ItemIndex>());
            }
        }

        ItemIndex[] SortItems(ItemIndex[] items, int count, RoR2.UI.ItemInventoryDisplay display, SortMode mode, bool sortByTier) //This really should be refactored but it works so...
        {

            if (mode == SortMode.Off) { return items; };

            if (mode == SortMode.Alphabetical || mode == SortMode.AlphabeticalDecending) { 

                //Log.Debug("Sorting Alphabetical");

                items = items[0..count]; //remove everything past length of items. IDK why its there to begin with 

                Array.Sort(items, (ItemIndex index, ItemIndex index2) =>
                {
                    return Language.GetString(ItemCatalog.GetItemDef(index).nameToken).CompareTo(Language.GetString(ItemCatalog.GetItemDef(index2).nameToken));
                });
                if (mode == SortMode.AlphabeticalDecending)
                {
                    items.Reverse();
                }

                return items;
            }


           // Log.Debug("Sorting Everything Else");

            sortByTier = sortByTier && (SortByTier.Value != TierSortMode.Off);

            bool seperateScrap = ScrapSorting.Value != ScrapSortMode.Mixed;
            bool sortByStackSize = (mode == SortMode.StackSize || mode == SortMode.StackSizeDecending);
            bool descendingStackSize = mode == SortMode.StackSizeDecending;


            foreach (var tierList in itemTierLists)
            {
                tierList.Clear();
            }
            scrapList.Clear();
            ItemIndex[] newArray = new ItemIndex[count];
            List<ItemIndex> allItems = new List<ItemIndex>();
            for (int i = 0; i < count; i++)
            {
                if (seperateScrap && (ItemCatalog.GetItemDef(items[i]).ContainsTag(ItemTag.Scrap) || ItemCatalog.GetItemDef(items[i]).ContainsTag(ItemTag.PriorityScrap) || ItemCatalog.GetItemDef(items[i]).nameToken == "ITEM_REGENERATINGSCRAPCONSUMED_NAME"))
                {
                    scrapList.Add(items[i]);
                }
                else if (sortByTier)
                {
                    ItemTier tier = ItemCatalog.GetItemDef(items[i]).tier;
                    if (CombineVoidTiers.Value)
                    {
                        // pretend the item is the regular version of the tier
                        tier = tier switch
                        {
                            ItemTier.VoidBoss => ItemTier.Boss,
                            ItemTier.VoidTier3 => ItemTier.Tier3,
                            ItemTier.VoidTier2 => ItemTier.Tier2,
                            ItemTier.VoidTier1 => ItemTier.Tier1,
                            _ => tier
                        };
                    }
                    itemTierLists[tierMatcher[tier]].Add(items[i]);
                }
                else
                {
                    allItems.Add(items[i]);
                    newArray[i] = items[i];
                }
            }
            items = newArray;

            if (sortByTier)
            {
                bool sortByAcquired = SortByTier.Value != TierSortMode.TierIgnoringAcquiredOrder;
                for (int i = 0; i < itemTierLists.Count; i++)
                {
                    itemTierLists[i] = new List<ItemIndex>(itemTierLists[i].OrderBy((itemIndex) =>
                        // if sort by acquired enabled, will ignore itemIndex
                        (sortByAcquired ? 0 : (int) itemIndex)
                        // if sort by stack size disabled, will ignore stacks
                        + (!sortByStackSize ? 0 : (descendingStackSize ? -1 : 1) * display.itemStacks[(int) itemIndex] * 20000)).ToArray());
                }

                if (scrapList.Count >= 0)
                {
                    scrapList = scrapList.OrderBy(item => tierMatcher[ItemCatalog.GetItemDef(item).tier]).ToList();
                }
                int num = 0;
                if (seperateScrap && ScrapSorting.Value == ScrapSortMode.Start)
                {
                    for (int i = 0; i < scrapList.Count; i++)
                    {
                        items[num] = scrapList[i];
                        num++;
                    }
                }
                for (int i = 0; i < itemTierLists.Count; i++)
                {
                    for (int x = 0; x < itemTierLists[i].Count; x++)
                    {
                        items[num] = itemTierLists[i][x];
                        num++;
                    }
                }
                if (seperateScrap && ScrapSorting.Value == ScrapSortMode.End)
                {
                    for (int i = 0; i < scrapList.Count; i++)
                    {
                        items[num] = scrapList[i];
                        num++;
                    }
                }
            }
            else
            {
                allItems = new List<ItemIndex>(allItems.ToArray().OrderBy((item) =>
                +((descendingStackSize ? -1 : 1) * (sortByStackSize ? 1 : 0) * display.itemStacks[(int)item] * 20000)).ToArray());
                foreach (var item in scrapList)
                {
                    if (ScrapSorting.Value == ScrapSortMode.Start)
                    {
                        allItems.Insert(0, item);
                    }
                    else if (ScrapSorting.Value == ScrapSortMode.End)
                    {
                        allItems.Add(item);
                    }
                }
                items = allItems.ToArray();
            }
            return items;
        }
    }
}
