using BepInEx.Configuration;
using LookingGlass.Base;
using MonoMod.RuntimeDetour;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace LookingGlass.HidePickupNotifs
{
    internal class HidePickupNotifications : BaseThing
    {
        public static ConfigEntry<bool> disablePickupNotifications;
        private static Hook overrideHook;

        public HidePickupNotifications()
        {
            Setup();
        }
        public void Setup()
        {
            disablePickupNotifications = BasePlugin.instance.Config.Bind<bool>("Settings", "Disable Pickup Notifications", false, "Disable item pickup notifications");
            InitHooks();
        }
        void InitHooks()
        {
            var targetMethod = typeof(RoR2.CharacterMasterNotificationQueue).GetMethod(nameof(RoR2.CharacterMasterNotificationQueue.PushPickupNotification), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var destMethod = typeof(HidePickupNotifications).GetMethod(nameof(PushPickupNotification), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
        }
        void PushPickupNotification(Action<CharacterMaster, PickupIndex> orig, CharacterMaster characterMaster, PickupIndex pickupIndex)
        {
            if (disablePickupNotifications.Value)
            {
                return;
            }
            orig(characterMaster, pickupIndex);
        }
    }
}
