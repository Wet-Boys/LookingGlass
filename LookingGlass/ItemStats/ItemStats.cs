using BepInEx.Configuration;
using LookingGlass.Base;
using MonoMod.RuntimeDetour;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using static RoR2.Chat;
using System.Linq;

namespace LookingGlass.ItemStatsNameSpace
{
    internal class ItemStats : BaseThing
    {
        public static ConfigEntry<bool> itemStats;
        public static ConfigEntry<bool> itemStatsOnPing;

        private static Hook overrideHook;
        private static Hook overrideHook2;
        private static Hook overrideHook3;
        public ItemStats()
        {
            Setup();
        }
        public void Setup()
        {
            InitHooks();
            ItemDefinitions.RegisterAll();
            itemStats = BasePlugin.instance.Config.Bind<bool>("Misc", "Item Stats", true, "Shows item descriptions plus calculations");
            itemStatsOnPing = BasePlugin.instance.Config.Bind<bool>("Misc", "Item Stats On Ping", true, "Shows item descriptions when you ping an item in the world");
            SetupRiskOfOptions();
        }
        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(itemStats, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(itemStatsOnPing, new CheckBoxConfig() { restartRequired = false }));
        }
        void InitHooks()
        {
            var targetMethod = typeof(GenericNotification).GetMethod(nameof(GenericNotification.SetItem), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(ItemStats).GetMethod(nameof(PickupText), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(RoR2.UI.ItemIcon).GetMethod(nameof(RoR2.UI.ItemIcon.SetItemIndex), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(ItemStats).GetMethod(nameof(ItemIndexText), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook2 = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(PingerController).GetMethod(nameof(PingerController.SetCurrentPing), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(ItemStats).GetMethod(nameof(ItemPinged), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook3 = new Hook(targetMethod, destMethod, this);
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
                SetDescription(self, newItemIndex, newItemCount);
            }
        }
        internal static void SetDescription(ItemIcon self, ItemIndex newItemIndex, int newItemCount)
        {
            var itemDef = ItemCatalog.GetItemDef(newItemIndex);
            if (self.tooltipProvider != null && itemDef != null)
            {

                self.tooltipProvider.overrideBodyText = GetDescription(itemDef, newItemIndex, newItemCount, false);
            }
        }
        public static string GetDescription(ItemDef itemDef, ItemIndex newItemIndex, int newItemCount, bool withOneMore)
        {
            var itemDescription = $"{Language.GetString(itemDef.descriptionToken)}\n";

            if (ItemDefinitions.allItemDefinitions.ContainsKey((int)newItemIndex))
            {
                ItemStatsDef itemStats = ItemDefinitions.allItemDefinitions[(int)newItemIndex];
                if (withOneMore && itemStats.descriptions.Count != 0)
                {
                    itemDescription += $"\nWith one more stack than you have:";
                    newItemCount++;
                }
                List<float> values = itemStats.calculateValues(newItemCount);
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
            return itemDescription;
        }


        //Heavily uses https://github.com/Moffein/ItemStats/blob/master/ItemStats/ItemStatsPlugin.cs
        void ItemPinged(Action<PingerController, PingerController.PingInfo> orig, PingerController self, PingerController.PingInfo newPingInfo)
        {
            orig(self, newPingInfo);
            if (!itemStatsOnPing.Value || !(self.hasAuthority && newPingInfo.targetGameObject))
                return;

            PickupDef pickupDef = null;
            GenericPickupController genericPickupController = newPingInfo.targetGameObject.GetComponent<GenericPickupController>();
            if (genericPickupController)
            {
                pickupDef = PickupCatalog.GetPickupDef(genericPickupController.pickupIndex);
            }
            else
            {
                ShopTerminalBehavior shopTerminalBehavior = newPingInfo.targetGameObject.GetComponent<ShopTerminalBehavior>();
                if (shopTerminalBehavior && !shopTerminalBehavior.pickupIndexIsHidden && !shopTerminalBehavior.Networkhidden && shopTerminalBehavior.pickupDisplay)
                {
                    pickupDef = PickupCatalog.GetPickupDef(shopTerminalBehavior.pickupIndex);
                }
            }
            if (pickupDef != null)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
                CharacterMaster characterMaster = self.gameObject.GetComponent<CharacterMaster>();
                if (itemDef)
                {
                    if (characterMaster)
                    {
                        PushItemNotificationDuration(characterMaster, itemDef.itemIndex);
                    }
                }
                else
                {
                    EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(pickupDef.equipmentIndex);
                    if (equipmentDef)
                    {
                        if (characterMaster)
                        {
                            PushEquipmentNotificationDuration(characterMaster, equipmentDef.equipmentIndex);
                        }
                    }
                }
            }
        }
        internal void PushItemNotificationDuration(CharacterMaster characterMaster, ItemIndex itemIndex)
        {
            if (!characterMaster.hasAuthority)
            {
                Log.Error("Can't PushItemNotification for " + Util.GetBestMasterName(characterMaster) + " because they aren't local.");
                return;
            }
            CharacterMasterNotificationQueue notificationQueueForMaster = CharacterMasterNotificationQueue.GetNotificationQueueForMaster(characterMaster);
            if (notificationQueueForMaster && itemIndex != ItemIndex.None)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                if (itemDef == null || itemDef.hidden)
                {
                    return;
                }
                notificationQueueForMaster.PushNotification(new CharacterMasterNotificationQueue.NotificationInfo(ItemCatalog.GetItemDef(itemIndex), null), 6f);
                PutLastNotificationFirst(notificationQueueForMaster);
            }
        }

        internal void PushEquipmentNotificationDuration(CharacterMaster characterMaster, EquipmentIndex equipmentIndex)
        {
            if (!characterMaster.hasAuthority)
            {
                Log.Error("Can't PushEquipmentNotification for " + Util.GetBestMasterName(characterMaster) + " because they aren't local.");
                return;
            }
            CharacterMasterNotificationQueue notificationQueueForMaster = CharacterMasterNotificationQueue.GetNotificationQueueForMaster(characterMaster);
            if (notificationQueueForMaster && equipmentIndex != EquipmentIndex.None)
            {
                notificationQueueForMaster.PushNotification(new CharacterMasterNotificationQueue.NotificationInfo(EquipmentCatalog.GetEquipmentDef(equipmentIndex), null), 6f);
                PutLastNotificationFirst(notificationQueueForMaster);
            }
        }

        internal void PutLastNotificationFirst(CharacterMasterNotificationQueue notificationQueueForMaster)
        {
            if (notificationQueueForMaster.notifications.Count > 1)
            {
                notificationQueueForMaster.notifications[0].duration = .01f;
                if (notificationQueueForMaster.notifications.Count > 2)
                {
                    var newNotification = notificationQueueForMaster.notifications.Last();
                    notificationQueueForMaster.notifications.Remove(newNotification);
                    notificationQueueForMaster.notifications.Insert(1, newNotification);
                }
            }
        }
    }
}
