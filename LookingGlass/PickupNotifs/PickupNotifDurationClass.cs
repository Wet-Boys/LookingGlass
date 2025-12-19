using LookingGlass.Base;
using MonoMod.RuntimeDetour;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;
using LookingGlass.HidePickupNotifs;
using System.Reflection;

namespace LookingGlass.PickupNotifsDuration
{
    internal class PickupNotifDurationClass : BaseThing
    {
        private static Hook overrideHook;
        public static ConfigEntry<float> notificationDuration;

        public PickupNotifDurationClass()
        {
            Setup();
            SetupRiskOfOptions();
        }
        public void Setup()
        {
            var targetMethod = typeof(RoR2.CharacterMasterNotificationQueue).GetMethod(nameof(RoR2.CharacterMasterNotificationQueue.PushNotification), BindingFlags.NonPublic | BindingFlags.Instance);
            var destMethod = typeof(PickupNotifDurationClass).GetMethod(nameof(PushNotification), BindingFlags.NonPublic | BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
            notificationDuration = BasePlugin.instance.Config.Bind<float>("Misc", "Pickup Notifications Duration", 6, "Duration of pickup notifications. 6s is the vanilla duration");
        }

        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new SliderOption(notificationDuration, new SliderConfig() { restartRequired = false, min = 0.01f, max = 24f, formatString = "{0:F1}s", checkIfDisabled = NotificationsDisabled }));

        }
        private static bool NotificationsDisabled()
        {
            return HidePickupNotifications.disablePickupNotifications.Value;
        }
        void PushNotification(Action<CharacterMasterNotificationQueue, CharacterMasterNotificationQueue.NotificationInfo, float> orig, CharacterMasterNotificationQueue self, CharacterMasterNotificationQueue.NotificationInfo info, float duration)
        {
            duration = notificationDuration.Value;
            orig(self, info, duration);
        }
    }
}
