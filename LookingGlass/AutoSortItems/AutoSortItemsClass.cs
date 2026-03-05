using BepInEx.Configuration;
using LookingGlass.Base;
using MonoMod.RuntimeDetour;
using RiskOfOptions;
using RiskOfOptions.Components.Options;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LookingGlass.AutoSortItems
{
    internal class AutoSortItemsClass : BaseThing
    {
        internal enum ScrapSortMode
        {
            Off,
            Start,
            End,
        }

        internal enum TierSortMode
        {
            Off,
            Tier,
            TierIgnoringAcquiredOrder
        }
        internal enum StackSortType
        {
            Off,
            Largest_Smallest,
            Smallest_Largest,
        }

        public static ConfigEntry<ScrapSortMode> ScrapSorting;
        public static ConfigEntry<TierSortMode> cfgSortByTier;
        public static ConfigEntry<string> TierOrder;
        public static ConfigEntry<bool> CombineVoidTiers;
        public static ConfigEntry<StackSortType> cfgSortByStackSize;

        public static ConfigEntry<CommandSortType> SortCommand;
        public static ConfigEntry<ScrapperSortType> SortScrapper;


        public static ConfigEntry<bool> SortScrapperTier;

        public static ConfigEntry<bool> SortPotentials;
        public static ConfigEntry<bool> SortDeathScreen;
        public static ConfigEntry<bool> sortCraftableItems;


        public static AutoSortItemsClass instance;
        RoR2.UI.ItemInventoryDisplay display;
        List<List<ItemIndex>> itemTierLists = new List<List<ItemIndex>>();
        List<ItemIndex> scrapList = new List<ItemIndex>();
        Dictionary<ItemTier, int> tierMatcher = new Dictionary<ItemTier, int>();
        private static Hook overrideHook;
        bool initialized = false;

        public enum CommandSortType
        {
            Off,
            Largest_Smallest,
            Smallest_Largest,
            A_Z,
            Z_A,
        }
        public enum ScrapperSortType
        {
            Off,
            MatchHud,
            Largest_Smallest,
            Smallest_Largest,
            A_Z,
            Z_A,
        }

        public enum TierSortPreset
        {
            Set,
            LowTier__HighTier,
            HighTier__LowTier
        }

        public AutoSortItemsClass()
        {
            Setup();
        }
        public void Setup()
        {

            instance = this;
            ScrapSorting = BasePlugin.instance.Config.Bind<ScrapSortMode>("Auto Sort Items", "Scrap Sorting", ScrapSortMode.Start, "Where scrap should be sorted");
            cfgSortByTier = BasePlugin.instance.Config.Bind("Auto Sort Items", "Tier Sort", TierSortMode.Tier, "Sorts by Tier");
            TierOrder = BasePlugin.instance.Config.Bind<string>("Auto Sort Items", "Tier Order", "Lunar FoodTier VoidBoss Boss VoidTier3 Tier3 VoidTier2 Tier2 VoidTier1 Tier1 NoTier", "How the tiers should be ordered");
            CombineVoidTiers = BasePlugin.instance.Config.Bind("Auto Sort Items", "Combine Normal And Void Tiers", false, "Considers void tiers to be the same as their normal counterparts");

            cfgSortByStackSize = BasePlugin.instance.Config.Bind("Auto Sort Items", "Stack Size Sort", StackSortType.Largest_Smallest, "Sorts by Stack Size");

            SortCommand = BasePlugin.instance.Config.Bind("Auto Sort Items", "Command Sorting", CommandSortType.Off, "Sorts Command menus by stack count or alphabetically.\n\n");
            //Most people would be accustomed to the vanilla sort order, so shouldn't mess with that.
            //Additionally sorting Void Potentials is just kind of, who cares.


            SortScrapper = BasePlugin.instance.Config.Bind("Auto Sort Items", "Scrapper Sorting", ScrapperSortType.MatchHud, "Sorts Scrapper by stack count or alphetically.\n\nMatchHud will automatically use tier sorting and stacking sorting of the regular hud.");
            SortScrapperTier = BasePlugin.instance.Config.Bind("Auto Sort Items", "Sort Scrapper by Tier", true, "Sorts Scrapper by tier"); //While this should be on by default, because your hud is sorted by tier and you'd expect those two to correlate
                                                                                                                                            //The scrapper sorting should match, whatever is shown in the Hud, so they can see like ah yeah X item is between YZ and it's the same in the scrapper.

            ScrapSorting.SettingChanged += SettingsChanged;
            cfgSortByTier.SettingChanged += SettingsChanged;
            TierOrder.SettingChanged += SettingsChanged;
            CombineVoidTiers.SettingChanged += SettingsChanged;
            cfgSortByStackSize.SettingChanged += SettingsChanged;

            //
            SortPotentials = BasePlugin.instance.Config.Bind("Auto Sort Items", "Sort Potentials & Fragments", false, "Sorts Void Potentials & Aurelionite Fragments according to Scrapper rules.");
            SortDeathScreen = BasePlugin.instance.Config.Bind("Auto Sort Items", "Sort Death Screen Items", false, "Sort items on the game over screen & run reports.");
            sortCraftableItems = BasePlugin.instance.Config.Bind("Auto Sort Items", "Sort Crafting Menu", true, "Sort items in the Wandering Chef or any crafting station\n\nTier sorting if Hud is tier sorted.\n\nAll items that cannot be used in any crafting recipe sorted to the bottom");

            //
            InitHooks();
            SetupRiskOfOptions();
        }

        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new ChoiceOption(ScrapSorting, new ChoiceConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new ChoiceOption(cfgSortByTier, new ChoiceConfig() { restartRequired = false }));

            ModSettingsManager.AddOption(new ChoiceOption(cfgSortByStackSize, new ChoiceConfig() { restartRequired = false }));

            ModSettingsManager.AddOption(new ChoiceOption(SortCommand, new ChoiceConfig() { restartRequired = false, /*checkIfDisabled = CheckNotCommandSortAlphabetical*/ }));
            ModSettingsManager.AddOption(new ChoiceOption(SortScrapper, new ChoiceConfig() { restartRequired = false, /*checkIfDisabled = CheckNotScrapperSortTierAlphabetical*/ }));
            ModSettingsManager.AddOption(new CheckBoxOption(SortScrapperTier, new CheckBoxConfig() { restartRequired = false, checkIfDisabled = CheckNotScrapperSortTierAlphabetical }));


            ModSettingsManager.AddOption(new StringInputFieldOption(TierOrder, new InputFieldConfig() { restartRequired = false, checkIfDisabled = CheckTierSort, lineType = TMPro.TMP_InputField.LineType.MultiLineSubmit, submitOn = InputFieldConfig.SubmitEnum.OnExitOrSubmit }));
            ModSettingsManager.AddOption(new GenericButtonOption("Use Ascending Tiers Preset", "Auto Sort Items", "Sets the Tier Order option to use ascending tiers", "Set", SetAscendingTiers));
            ModSettingsManager.AddOption(new GenericButtonOption("Use Descending Tiers Preset", "Auto Sort Items", "Sets the Tier Order option to use descending tiers", "Set", SetDescendingTiers));
            ModSettingsManager.AddOption(new CheckBoxOption(CombineVoidTiers, new CheckBoxConfig() { restartRequired = false, checkIfDisabled = CheckTierSort }));


            ModSettingsManager.AddOption(new CheckBoxOption(SortPotentials, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(SortDeathScreen, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(sortCraftableItems, new CheckBoxConfig() { restartRequired = false }));

        }
        //I'm going cross-eyed looking at all these
        private static bool CheckTierSort()
        {
            return cfgSortByTier.Value == TierSortMode.Off;
        }

        private static bool CheckNotScrapperSortTierAlphabetical()
        {
            return SortScrapper.Value == ScrapperSortType.A_Z || SortScrapper.Value == ScrapperSortType.Z_A || SortScrapper.Value == ScrapperSortType.MatchHud;
        }

        private void SetDescendingTiers()
        {
            // manually update the tier order option in the menu (which also updates the setting)
            foreach (var controller in UnityEngine.Object.FindObjectsOfType<InputFieldController>())
            {
                if (controller.name.Contains("Tier Order"))
                {
                    controller.SubmitValue("Lunar FoodTier VoidBoss Boss VoidTier3 Tier3 VoidTier2 Tier2 VoidTier1 Tier1 NoTier");
                }
            }
        }
        private void SetAscendingTiers()
        {
            foreach (var controller in UnityEngine.Object.FindObjectsOfType<InputFieldController>())
            {
                if (controller.name.Contains("Tier Order"))
                {

                    controller.SubmitValue("Tier1 VoidTier1 Tier2 VoidTier2 Tier3 VoidTier3 Boss VoidBoss FoodTier Lunar NoTier");
                }
            }
        }

        internal (PickupPickerController.Option[], List<int>) SortPickupPicker(PickupPickerController.Option[] options, bool isCommand, CraftingController mealPrep)
        {
            //Mealprep can have Equipment in it, how do we deal with that
            List<int> mapping = new List<int>(options.Length);
            if (mealPrep == null)
            {
                List<ItemIndex> items = new List<ItemIndex>();
                List<ItemIndex> unSortedItems = new List<ItemIndex>(); //used to make mapping of what changed when sorting

                //bool meal2 = isMealprep && sortCraftableItems.Value;
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

                if (isCommand)
                {
                    if (SortCommand.Value == CommandSortType.Z_A)
                    {
                        items.Sort(delegate (ItemIndex index, ItemIndex index2)
                        {
                            return Language.GetString(ItemCatalog.GetItemDef(index2).nameToken).CompareTo(Language.GetString(ItemCatalog.GetItemDef(index).nameToken));
                        });
                    }
                    else if (SortCommand.Value == CommandSortType.A_Z)
                    {
                        items.Sort(delegate (ItemIndex index, ItemIndex index2)
                        {
                            return Language.GetString(ItemCatalog.GetItemDef(index).nameToken).CompareTo(Language.GetString(ItemCatalog.GetItemDef(index2).nameToken));
                        });
                    }
                    else
                    {
                        items = new List<ItemIndex>(SortItems(items.ToArray(), items.Count, display, false, false, SortCommand.Value >= CommandSortType.Largest_Smallest, SortCommand.Value == CommandSortType.Largest_Smallest));

                    }
                }
                else
                {
                    //Who in their right mind would sort a Scrapper alphebetically tbh
                    if (SortScrapper.Value == ScrapperSortType.Z_A)
                    {
                        items.Sort(delegate (ItemIndex index, ItemIndex index2)
                        {
                            return Language.GetString(ItemCatalog.GetItemDef(index2).nameToken).CompareTo(Language.GetString(ItemCatalog.GetItemDef(index).nameToken));
                        });
                    }
                    else if (SortScrapper.Value == ScrapperSortType.A_Z)
                    {
                        items.Sort(delegate (ItemIndex index, ItemIndex index2)
                        {
                            return Language.GetString(ItemCatalog.GetItemDef(index).nameToken).CompareTo(Language.GetString(ItemCatalog.GetItemDef(index2).nameToken));
                        });
                    }
                    else if (SortScrapper.Value == ScrapperSortType.MatchHud)
                    {
                        items = new List<ItemIndex>(SortItems(items.ToArray(), items.Count, display, false, cfgSortByTier.Value != TierSortMode.Off, cfgSortByStackSize.Value >= StackSortType.Largest_Smallest, cfgSortByStackSize.Value == StackSortType.Largest_Smallest));
                    }
                    else
                    {
                        items = new List<ItemIndex>(SortItems(items.ToArray(), items.Count, display, false, SortScrapperTier.Value, SortScrapper.Value >= ScrapperSortType.Largest_Smallest, SortScrapper.Value == ScrapperSortType.Largest_Smallest));
                    }

                }
                //items = new List<ItemIndex>(SortItems(items.ToArray(), items.Count, display, false, isCommand ? false : SortScrapperTier.Value, isCommand ? SortCommand.Value == MenuSortType.Largest_Smallest: SortScrapper.Value, isCommand ? SortCommandDescending.Value : SortScrapperDescending.Value));


                // make mapping of what was changed
                mapping = Enumerable.ToList(Enumerable.Select(items, (ItemIndex item) => unSortedItems.IndexOf(item)));

            }
            else if (mealPrep && sortCraftableItems.Value)
            {
                List<PickupIndex> unsorted = new List<PickupIndex>();
                for (int i = 0; i < options.Length; i++)
                {
                    unsorted.Add(options[i].pickupIndex);
                }
                var sorted = new List<PickupIndex>(SortPickups(unsorted.ToArray(), unsorted.Count, display, SortScrapperTier.Value, cfgSortByStackSize.Value >= StackSortType.Largest_Smallest, cfgSortByStackSize.Value == StackSortType.Largest_Smallest));


                List<PickupIndex> ingredients = new List<PickupIndex>();
                List<PickupIndex> NOTingredients = new List<PickupIndex>(); //used to make mapping of what changed when sorting
                HashSet<PickupIndex> hashSet = new HashSet<PickupIndex>();
                foreach (CraftableCatalog.RecipeEntry recipeEntry in CraftableCatalog.GetAllRecipes())
                {
                    List<PickupIndex> list = recipeEntry.GetAllPickups();
                    if (list != null)
                    {
                        foreach (PickupIndex item in list)
                        {
                            if (!hashSet.Contains(item))
                            {
                                hashSet.Add(item);
                            }
                        }
                    }
                }

                for (int j = 0; j < sorted.Count; j++)
                {
                    if (hashSet.Contains(sorted[j]))
                    {
                        ingredients.Add(sorted[j]);
                    }
                    else
                    {
                        NOTingredients.Add(sorted[j]);
                    }
                }
                ingredients.AddRange(NOTingredients);
                mapping = Enumerable.ToList(Enumerable.Select(ingredients, (PickupIndex item) => unsorted.IndexOf(item)));

            }
            else
            {
                return (options, [-1]); // -1 indicates no sorting
            }
            // apply mapping to options
            PickupPickerController.Option[] sortedOptions = Enumerable.ToArray(Enumerable.Select(mapping, (int index) => options[index]));
            //sortedOptions = sortedOptions.OrderBy(entry => !entry.available).ToArray();

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
            var targetMethod = typeof(RoR2.UI.ItemInventoryDisplay).GetMethod(nameof(RoR2.UI.ItemInventoryDisplay.UpdateDisplay), BindingFlags.Public | BindingFlags.Instance);
            var destMethod = typeof(AutoSortItemsClass).GetMethod(nameof(UpdateDisplayOverride), BindingFlags.NonPublic | BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);

        }

        private void UpdateDisplayOverride(Action<RoR2.UI.ItemInventoryDisplay> orig, RoR2.UI.ItemInventoryDisplay self)
        {
            //If inventory is null is a good check to see if it's a GameOver or RunReport Inventory.
            //Because there the items are added manually
            //However would also prevent sorting of enemy items. (Vields, Evo)
            //I am unaware of a better check atm
            // if (self != null && (self.inventory != null || SortDeathScreen.Value))
            if (self != null && (Run.instance && Run.instance.isRunning || SortDeathScreen.Value))
            {
                display = self;
                var temp = self.itemOrder;
                try
                {
                    if (!initialized)
                    {
                        initialized = true;
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
                        //Log.Debug($"tierMatcher: {Utils.DictToString(tierMatcher)}");
                    }
                    self.itemOrder = SortItems(self.itemOrder, self.itemOrderCount, self, ScrapSorting.Value != ScrapSortMode.Off, cfgSortByTier.Value != TierSortMode.Off, cfgSortByStackSize.Value >= StackSortType.Largest_Smallest, cfgSortByStackSize.Value == StackSortType.Largest_Smallest);
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
            initialized = false; // force re-initialization
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

        ItemIndex[] SortItems(ItemIndex[] items, int count, RoR2.UI.ItemInventoryDisplay display, bool seperateScrap, bool sortByTier, bool sortByStackSize, bool descendingStackSize) //This really should be refactored but it works so...
        {
            foreach (var tierList in itemTierLists)
            {
                tierList.Clear();
            }
            scrapList.Clear();
            ItemIndex[] newArray = new ItemIndex[count];
            List<ItemIndex> allItems = new List<ItemIndex>();
            for (int i = 0; i < count; i++)
            {
                if (seperateScrap && (ItemCatalog.GetItemDef(items[i]).ContainsTag(ItemTag.Scrap) || ItemCatalog.GetItemDef(items[i]).ContainsTag(ItemTag.PriorityScrap) || items[i] == DLC1Content.Items.RegeneratingScrapConsumed.itemIndex))
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
                bool sortByAcquired = cfgSortByTier.Value != TierSortMode.TierIgnoringAcquiredOrder;
                for (int i = 0; i < itemTierLists.Count; i++)
                {
                    itemTierLists[i] = new List<ItemIndex>(itemTierLists[i].OrderBy((itemIndex) =>
                        // if sort by acquired enabled, will ignore itemIndex
                        (sortByAcquired ? 0 : (int)itemIndex)
                        // if sort by stack size disabled, will ignore stacks
                        + (!sortByStackSize ? 0 : (descendingStackSize ? -1 : 1) * display.itemStacks[(int)itemIndex] * 20000)).ToArray());
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

        PickupIndex[] SortPickups(PickupIndex[] pickups, int count, ItemInventoryDisplay display, bool sortByTier, bool sortByStackSize, bool descendingStackSize) //This really should be refactored but it works so...
        {
            foreach (var tierList in itemTierLists)
            {
                tierList.Clear();
            }
            scrapList.Clear();
            List<PickupIndex> newArray = new List<PickupIndex>();
            List<PickupIndex> ITEMS = new List<PickupIndex>();
            List<PickupIndex> ITEMSSorted = new List<PickupIndex>();
            List<PickupIndex> equipment = new List<PickupIndex>();
            for (int i = 0; i < count; i++)
            {
                PickupDef def = pickups[i].pickupDef;
                if (def.itemIndex != ItemIndex.None)
                {
                    ItemDef item = ItemCatalog.GetItemDef(def.itemIndex);
                    if (sortByTier)
                    {
                        ItemTier tier = item.tier;
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
                        itemTierLists[tierMatcher[tier]].Add(def.itemIndex);
                    }
                    else
                    {
                        ITEMS.Add(def.pickupIndex);
                    }

                }
                else if (def.equipmentIndex != EquipmentIndex.None)
                {
                    equipment.Add(def.pickupIndex);
                }
            }

            if (sortByTier)
            {
                bool sortByAcquired = cfgSortByTier.Value != TierSortMode.TierIgnoringAcquiredOrder;
                for (int i = 0; i < itemTierLists.Count; i++)
                {
                    itemTierLists[i] = new List<ItemIndex>(itemTierLists[i].OrderBy((itemIndex) =>
                        // if sort by acquired enabled, will ignore itemIndex
                        (sortByAcquired ? 0 : (int)itemIndex)
                        // if sort by stack size disabled, will ignore stacks
                        + (!sortByStackSize ? 0 : (descendingStackSize ? -1 : 1) * display.itemStacks[(int)itemIndex] * 20000)).ToArray());
                }

                if (scrapList.Count >= 0)
                {
                    scrapList = scrapList.OrderBy(item => tierMatcher[ItemCatalog.GetItemDef(item).tier]).ToList();
                }
                for (int i = 0; i < itemTierLists.Count; i++)
                {
                    for (int x = 0; x < itemTierLists[i].Count; x++)
                    {
                        ITEMS.Add(PickupCatalog.FindPickupIndex(itemTierLists[i][x]));
                    }
                }

            }

            newArray.AddRange(ITEMS);
            newArray.AddRange(equipment);
            return newArray.ToArray();
        }

    }
}
