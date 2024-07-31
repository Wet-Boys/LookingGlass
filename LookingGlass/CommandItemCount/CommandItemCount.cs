using LookingGlass.Base;
using MonoMod.RuntimeDetour;
using RoR2.UI;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using UnityEngine;
using BepInEx.Configuration;
using LookingGlass.ItemStatsNameSpace;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;
using RoR2.Items;
using System.Reflection;
using System.Linq;

namespace LookingGlass.CommandItemCount
{
    internal class CommandItemCountClass : BaseThing
    {
        private static Hook overrideHook;
        private static Hook contagiousItemsHook;
        public static ConfigEntry<bool> commandItemCount;
        public static ConfigEntry<bool> hideCountIfZero;
        public static ConfigEntry<bool> commandToolTips;
        public static ConfigEntry<bool> showCorruptedItems;

        // needs to be a list because some void items corrupt multiple items
        private Dictionary<ItemIndex, List<ItemIndex>> transformedToOriginal;

        public CommandItemCountClass()
        {
            Setup();
        }
        public void Setup()
        {
            var targetMethod = typeof(RoR2.UI.PickupPickerPanel).GetMethod(nameof(RoR2.UI.PickupPickerPanel.SetPickupOptions), BindingFlags.Public | BindingFlags.Instance);
            var destMethod = typeof(CommandItemCountClass).GetMethod(nameof(PickupPickerPanel), BindingFlags.NonPublic | BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
            var targetMethod2 = typeof(ContagiousItemManager).GetMethod(nameof(ContagiousItemManager.InitTransformationTable), BindingFlags.NonPublic | BindingFlags.Static);
            var destMethod2 = typeof(CommandItemCountClass).GetMethod(nameof(InitializeTransformedToOriginal), BindingFlags.NonPublic | BindingFlags.Instance);
            contagiousItemsHook = new Hook(targetMethod2, destMethod2, this);
            commandItemCount = BasePlugin.instance.Config.Bind<bool>("Command Settings", "Command Item Count", true, "Shows how many items you have in the command menu");
            hideCountIfZero = BasePlugin.instance.Config.Bind<bool>("Command Settings", "Hide Count If Zero", false, "Hides the item count if you have none of an item");
            commandToolTips = BasePlugin.instance.Config.Bind<bool>("Command Settings", "Command Tooltips", true, "Shows tooltips in the command menu");
            showCorruptedItems = BasePlugin.instance.Config.Bind<bool>("Command Settings", "Show Corrupted Items", true, "Shows when items have been corrupted");
            SetupRiskOfOptions();
        }
        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(commandItemCount, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(hideCountIfZero, new CheckBoxConfig() { restartRequired = false, checkIfDisabled = CheckHideCountIfZero }));
            ModSettingsManager.AddOption(new CheckBoxOption(commandToolTips, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(showCorruptedItems, new CheckBoxConfig() { restartRequired = false, checkIfDisabled = CheckShowCorruptedItems }));
        }
        private static bool CheckHideCountIfZero()
        {
            return !commandItemCount.Value;
        }
        private static bool CheckShowCorruptedItems()
        {
            return !(commandItemCount.Value || commandToolTips.Value);
        }

        private void InitializeTransformedToOriginal(Action orig)
        {
            orig();
            transformedToOriginal = new();
            foreach(var info in ContagiousItemManager.transformationInfos)
            {
                if (!transformedToOriginal.TryGetValue(info.transformedItem, out List<ItemIndex> originalItemList))
                {
                    transformedToOriginal[info.transformedItem] = originalItemList = new();
                }
                originalItemList.Add(info.originalItem);
            }
        }

        //Largely copied from https://github.com/Vl4dimyr/CommandItemCount/blob/master/CommandItemCountPlugin.cs#L191
        void PickupPickerPanel(Action<PickupPickerPanel, PickupPickerController.Option[]> orig, PickupPickerPanel self, PickupPickerController.Option[] options)
        {
            orig(self, options);
            if (options.Length < 1)
            {
                return;
            }
            string parentName = self.gameObject.name;
            bool withOneMore = parentName.StartsWith("OptionPickerPanel") || parentName.StartsWith("CommandPickerPanel");
            ReadOnlyCollection<MPButton> elements = self.buttonAllocator.elements;
            Inventory inventory = LocalUserManager.GetFirstLocalUser().cachedMasterController.master.inventory;
            for (int i = 0; i < options.Length; i++)
            {
                ItemIndex itemIndex = PickupCatalog.GetPickupDef(options[i].pickupIndex).itemIndex;
                int count = inventory.GetItemCount(itemIndex);

                // check for corrupted items
                ItemCorruptionInfo corruption = null;
                if (showCorruptedItems.Value && count == 0)
                {
                    ItemIndex corruptedIndex = ContagiousItemManager.GetTransformedItemIndex(itemIndex);
                    if (corruptedIndex != ItemIndex.None && inventory.GetItemCount(corruptedIndex) > 0)
                    {
                        corruption = new()
                        {
                            Type = CorruptionType.Corrupted,
                            Items = [corruptedIndex],
                            ItemCount = inventory.GetItemCount(corruptedIndex)
                        };
                    }
                    else if (transformedToOriginal.TryGetValue(itemIndex, out var origList))
                    {
                        int origCount = 0;
                        foreach (ItemIndex origIndex in origList)
                        {
                            origCount += inventory.GetItemCount(origIndex);
                        }
                        if (origCount > 0)
                        {
                            corruption = new()
                            {
                                Type = CorruptionType.Void,
                                Items = origList,
                                ItemCount = origCount
                            };
                        }
                    }
                }
                corruption ??= new()
                {
                    Type = CorruptionType.None
                };

                if (commandItemCount.Value)
                    CreateNumber(elements[i].transform, count, corruption);
                if (commandToolTips.Value)
                    CreateToolTip(elements[i].transform, PickupCatalog.GetPickupDef(options[i].pickupIndex), count, withOneMore, corruption);
            }
        }
        void CreateNumber(Transform parent, int count, ItemCorruptionInfo corruption)
        {
            GameObject textContainer = new GameObject();
            textContainer.transform.parent = parent;

            textContainer.AddComponent<CanvasRenderer>();

            RectTransform rectTransform = textContainer.AddComponent<RectTransform>();
            HGTextMeshProUGUI hgtextMeshProUGUI = textContainer.AddComponent<HGTextMeshProUGUI>();

            hgtextMeshProUGUI.text = $"x{count}";
            if (count == 0)
            {
                hgtextMeshProUGUI.text = (hideCountIfZero.Value && corruption.Type == CorruptionType.None) ? "" : $"<color=#808080>{hgtextMeshProUGUI.text}</color>";
            }
            hgtextMeshProUGUI.fontSize = 18f;
            hgtextMeshProUGUI.color = Color.white;
            hgtextMeshProUGUI.alignment = TMPro.TextAlignmentOptions.TopRight;
            hgtextMeshProUGUI.enableWordWrapping = false;
            if (corruption.Type == CorruptionType.Corrupted)
            {
                hgtextMeshProUGUI.text += $"\n<color=#eda3d7>x{corruption.ItemCount}</color>";
            }
            else if (corruption.Type == CorruptionType.Void)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(corruption.Items[0]);
                ItemTierDef itemTierDef = ItemTierCatalog.GetItemTierDef(itemDef.tier);
                string color = ColorCatalog.GetColorHexString(itemTierDef.colorIndex);
                hgtextMeshProUGUI.text += $"\n<color=#{color}>x{corruption.ItemCount}</color>";
            }

            rectTransform.localPosition = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = new Vector2(-5f, -1.5f);
        }
        void CreateToolTip(Transform parent, PickupDef pickupDefinition, int count, bool withOneMore, ItemCorruptionInfo corruption)
        {
            ItemDef itemDefinition = ItemCatalog.GetItemDef(pickupDefinition.itemIndex);
            EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(pickupDefinition.equipmentIndex);
            bool isItem = itemDefinition != null;

            TooltipContent content = new TooltipContent();

            content.titleColor = pickupDefinition.darkColor;
            content.titleToken = isItem ? itemDefinition.nameToken : equipmentDef.nameToken;
            content.bodyToken = isItem ? itemDefinition.descriptionToken : equipmentDef.descriptionToken;

            if (isItem && ItemStats.itemStats.Value)
            {
                string stats;
                if (corruption.Type == CorruptionType.Corrupted)
                {
                    // intentional extra </style> tag because some items have broken descriptions *glares at titanic knurl*
                    stats = $"<size=85%><color=#808080>{Language.GetString(itemDefinition.descriptionToken)}</color></style></size>";
                    ItemDef corruptedItemDefinition = ItemCatalog.GetItemDef(corruption.Items[0]);
                    stats += $"\n\nHas been corrupted by: <style=cIsVoid>{Language.GetString(corruptedItemDefinition.nameToken)}</style>\n\n";
                    stats += ItemStats.GetDescription(corruptedItemDefinition, corruptedItemDefinition.itemIndex, corruption.ItemCount, null, withOneMore);
                }
                else if (corruption.Type == CorruptionType.Void)
                {
                    stats = ItemStats.GetDescription(itemDefinition, itemDefinition.itemIndex, corruption.ItemCount, null, withOneMore);
                }
                else
                {
                    stats = ItemStats.GetDescription(itemDefinition, itemDefinition.itemIndex, count, null, withOneMore);
                }

                if (stats != null)
                {
                    content.overrideBodyText = stats;
                }
            }
            else if (isItem && corruption.Type == CorruptionType.Corrupted)
            {
                string stats = $"<size=85%><color=#808080>{Language.GetString(itemDefinition.descriptionToken)}</color></style></size>";
                ItemDef corruptedItemDefinition = ItemCatalog.GetItemDef(corruption.Items[0]);
                stats += $"\n\nHas been corrupted by: <style=cIsVoid>{Language.GetString(corruptedItemDefinition.nameToken)}</style>\n\n";
                stats += Language.GetString(corruptedItemDefinition.descriptionToken);
                content.overrideBodyText = stats;
            }

            TooltipProvider tooltipProvider = parent.gameObject.AddComponent<TooltipProvider>();

            tooltipProvider.SetContent(content);
        }
    }
}
