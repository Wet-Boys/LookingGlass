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

namespace LookingGlass.CommandItemCount
{
    internal class CommandItemCountClass : BaseThing
    {
        private static Hook overrideHook;
        public static ConfigEntry<bool> commandItemCount;
        public static ConfigEntry<bool> commandToolTips;
        public static ConfigEntry<float> commandItemCountFontSize;


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
            commandToolTips = BasePlugin.instance.Config.Bind<bool>("Command Settings", "Command Tooltips", true, "Shows tooltips in the command menu");
            commandItemCountFontSize = BasePlugin.instance.Config.Bind<float>("Command Settings", "Command Item Count Font Size", 100f, "Changes the font size of command item count");
            SetupRiskOfOptions();
        }
        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(commandItemCount, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(commandToolTips, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new SliderOption(commandItemCountFontSize, new SliderConfig() { restartRequired = false, min = 1, max = 300 }));

        }

        //Largely copied from https://github.com/Vl4dimyr/CommandItemCount/blob/master/CommandItemCountPlugin.cs#L191
        void PickupPickerPanel(Action<PickupPickerPanel, PickupPickerController.Option[]> orig, PickupPickerPanel self, PickupPickerController.Option[] options)
        {
            orig(self, options);
            if (options.Length < 1)
            {
                return;
            }
            ReadOnlyCollection<MPButton> elements = self.buttonAllocator.elements;
            Inventory inventory = LocalUserManager.GetFirstLocalUser().cachedMasterController.master.inventory;
            for (int i = 0; i < options.Length; i++)
            {
                ItemIndex itemIndex = PickupCatalog.GetPickupDef(options[i].pickupIndex).itemIndex;
                int count = inventory.GetItemCount(itemIndex);
                if (commandItemCount.Value)
                    CreateNumber(elements[i].transform, count);
                if (commandToolTips.Value)
                    CreateToolTip(elements[i].transform, PickupCatalog.GetPickupDef(options[i].pickupIndex), count);
            }
        }
        void CreateNumber(Transform parent, int count)
        {
            GameObject textContainer = new GameObject();
            textContainer.transform.parent = parent;

            textContainer.AddComponent<CanvasRenderer>();

            RectTransform rectTransform = textContainer.AddComponent<RectTransform>();
            HGTextMeshProUGUI hgtextMeshProUGUI = textContainer.AddComponent<HGTextMeshProUGUI>();

            hgtextMeshProUGUI.text = count != 0 ? $"<scale={commandItemCountFontSize.Value}%>x{count}</scale>" : "";
            hgtextMeshProUGUI.fontSize = 18f;
            hgtextMeshProUGUI.color = Color.white;
            hgtextMeshProUGUI.alignment = TMPro.TextAlignmentOptions.TopRight;
            hgtextMeshProUGUI.enableWordWrapping = false;

            rectTransform.localPosition = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = new Vector2(-5f, -1.5f);
        }
        void CreateToolTip(Transform parent, PickupDef pickupDefinition, int count)
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
                string stats = ItemStats.GetDescription(itemDefinition, itemDefinition.itemIndex, count, null, true);

                if (stats != null)
                {
                    content.overrideBodyText = stats;
                }
            }

            TooltipProvider tooltipProvider = parent.gameObject.AddComponent<TooltipProvider>();

            tooltipProvider.SetContent(content);
        }
    }
}
