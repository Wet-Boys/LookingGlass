using BepInEx.Configuration;
using LookingGlass.Base;
using MonoMod.RuntimeDetour;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace LookingGlass.ItemStatsNameSpace
{
    internal class ItemStats : BaseThing
    {
        public static ConfigEntry<bool> itemStats;

        private static Hook overrideHook;
        private static Hook overrideHook2;
        public ItemStats()
        {
            Setup();
        }
        public void Setup()
        {
            InitHooks();
            ItemDefinitions.RegisterAll();
            itemStats = BasePlugin.instance.Config.Bind<bool>("Settings", "Item Stats", true, "Shows item descriptions plus calculations");
        }
        void InitHooks()
        {
            var targetMethod = typeof(GenericNotification).GetMethod(nameof(GenericNotification.SetItem), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(ItemStats).GetMethod(nameof(PickupText), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(RoR2.UI.ItemIcon).GetMethod(nameof(RoR2.UI.ItemIcon.SetItemIndex), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(ItemStats).GetMethod(nameof(ItemIndexText), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook2 = new Hook(targetMethod, destMethod, this);
        }

        void PickupText(Action<GenericNotification, ItemDef> orig, GenericNotification self, ItemDef itemDef)
        {
            orig(self, itemDef);
            if (itemStats.Value)
                self.descriptionText.token = itemDef.descriptionToken;
        }
        void ItemIndexText(Action<ItemIcon, ItemIndex, int> orig, ItemIcon self, ItemIndex newItemIndex, int newItemCount)
        {
            orig(self, newItemIndex, newItemCount);
            if (itemStats.Value)
            {
                var itemDef = ItemCatalog.GetItemDef(newItemIndex);
                if (self.tooltipProvider != null && itemDef != null)
                {
                    var itemDescription = $"{Language.GetString(itemDef.descriptionToken)}\n";

                    if (ItemDefinitions.allItemDefinitions.ContainsKey((int)newItemIndex))
                    {
                        ItemStatsDef itemStats = ItemDefinitions.allItemDefinitions[(int)newItemIndex];
                        List<float> values = itemStats.CalculateValues(newItemCount);
                        for (int i = 0; i < itemStats.descriptions.Count; i++)
                        {
                            itemDescription += $"\n<color=\"white\">{itemStats.descriptions[i]}</color>";
                            switch (itemStats.valueTypes[i])
                            {
                                case ItemStatsDef.ValueType.Healing:
                                    itemDescription += "<style=\"cIsHealing";
                                    break;
                                case ItemStatsDef.ValueType.Damage:
                                    itemDescription += "<style=\"cIsDamage";
                                    break;
                                case ItemStatsDef.ValueType.Utility:
                                    itemDescription += "<style=\"cIsUtility";
                                    break;
                                case ItemStatsDef.ValueType.Health:
                                    itemDescription += "<style=\"cIsHealth";
                                    break;
                                    //case ItemStatsDef.ValueType.Other:
                                    //    itemDescription += "<color=\"white";
                                    //    break;
                            }
                            switch (itemStats.measurementUnits[i])
                            {
                                case ItemStatsDef.MeasurementUnits.Meters:
                                    itemDescription += $"\">{values[i]:0.###}m</style>";
                                    break;
                                case ItemStatsDef.MeasurementUnits.Percentage:
                                    itemDescription += $"\">{values[i] * 100:0.###}%</style>";
                                    break;
                                case ItemStatsDef.MeasurementUnits.Health:
                                    itemDescription += $"\">{values[i]:0.###}hp</style>";
                                    break;
                                case ItemStatsDef.MeasurementUnits.Number:
                                    itemDescription += $"\">{values[i]:0.###}</style>";
                                    break;
                                case ItemStatsDef.MeasurementUnits.Seconds:
                                    itemDescription += $"\">{values[i]:0.###}s</style>";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    Log.Debug(itemDescription);
                    self.tooltipProvider.overrideBodyText = itemDescription;
                }
            }
        }
    }
}
