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

namespace LookingGlass.CommandItemCount
{
    internal class CommandItemCountClass : BaseThing
    {
        private static Hook overrideHook;
        public static ConfigEntry<bool> commandItemCount;
        public static ConfigEntry<bool> hideCountIfZero;
        public static ConfigEntry<bool> commandToolTips;
        public static ConfigEntry<bool> showCorruptedItems;

        public CommandItemCountClass()
        {
            Setup();
        }
        public void Setup()
        {
            var targetMethod = typeof(RoR2.UI.PickupPickerPanel).GetMethod(nameof(RoR2.UI.PickupPickerPanel.SetPickupOptions), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(CommandItemCountClass).GetMethod(nameof(PickupPickerPanel), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
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

                // check for corrupted versions
                bool corrupted = false;
                int corruptedCount = 0;
                if (showCorruptedItems.Value && count == 0)
                {
                    ItemIndex corruptedIndex = ContagiousItemManager.GetTransformedItemIndex(itemIndex);
                    if (corruptedIndex != ItemIndex.None && inventory.GetItemCount(corruptedIndex) > 0)
                    {
                        corrupted = true;
                        corruptedCount = inventory.GetItemCount(corruptedIndex);
                    }
                }

                if (commandItemCount.Value)
                    CreateNumber(elements[i].transform, count, corrupted, corruptedCount);
                if (commandToolTips.Value)
                    CreateToolTip(elements[i].transform, PickupCatalog.GetPickupDef(options[i].pickupIndex), count, withOneMore, corrupted, corruptedCount);
            }
        }
        void CreateNumber(Transform parent, int count, bool corrupted = false, int corruptedCount = 0)
        {
            GameObject textContainer = new GameObject();
            textContainer.transform.parent = parent;

            textContainer.AddComponent<CanvasRenderer>();

            RectTransform rectTransform = textContainer.AddComponent<RectTransform>();
            HGTextMeshProUGUI hgtextMeshProUGUI = textContainer.AddComponent<HGTextMeshProUGUI>();

            hgtextMeshProUGUI.text = $"x{count}";
            if (count == 0)
            {
                hgtextMeshProUGUI.text = (hideCountIfZero.Value && !corrupted) ? "" : $"<color=#808080>{hgtextMeshProUGUI.text}</color>";
            }
            hgtextMeshProUGUI.fontSize = 18f;
            hgtextMeshProUGUI.color = Color.white;
            hgtextMeshProUGUI.alignment = TMPro.TextAlignmentOptions.TopRight;
            hgtextMeshProUGUI.enableWordWrapping = false;
            if (corrupted)
            {
                hgtextMeshProUGUI.text += $"\n<color=#eda3d7>x{corruptedCount}</color>";
            }

            rectTransform.localPosition = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = new Vector2(-5f, -1.5f);
        }
        void CreateToolTip(Transform parent, PickupDef pickupDefinition, int count, bool withOneMore, bool corrupted = false, int corruptedCount = 0)
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
                if (corrupted)
                {
                    // intentional extra </style> tag because some items have broken descriptions *glares at titanic knurl*
                    stats = $"<size=85%><color=#808080>{Language.GetString(itemDefinition.descriptionToken)}</color></style></size>";
                    ItemDef corruptedItemDefinition = ItemCatalog.GetItemDef(ContagiousItemManager.GetTransformedItemIndex(pickupDefinition.itemIndex));
                    stats += $"\n\nHas been corrupted by: <style=cIsVoid>{Language.GetString(corruptedItemDefinition.nameToken)}</style>\n\n";
                    stats += ItemStats.GetDescription(corruptedItemDefinition, corruptedItemDefinition.itemIndex, corruptedCount, null, withOneMore);
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
            else if (isItem && corrupted)
            {
                string stats = $"<size=85%><color=#808080>{Language.GetString(itemDefinition.descriptionToken)}</color></style></size>";
                ItemDef corruptedItemDefinition = ItemCatalog.GetItemDef(ContagiousItemManager.GetTransformedItemIndex(pickupDefinition.itemIndex));
                stats += $"\n\nHas been corrupted by: <style=cIsVoid>{Language.GetString(corruptedItemDefinition.nameToken)}</style>\n\n";
                stats += Language.GetString(corruptedItemDefinition.descriptionToken);
                content.overrideBodyText = stats;
            }

            TooltipProvider tooltipProvider = parent.gameObject.AddComponent<TooltipProvider>();

            tooltipProvider.SetContent(content);
        }
    }
}
