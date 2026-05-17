using BepInEx.Configuration;
using LookingGlass.Base;
using LookingGlass.LookingGlassLanguage;
using MonoMod.RuntimeDetour;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using System;
using System.Reflection;

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
            disablePickupNotifications = BasePlugin.instance.Config.Bind<bool>("Misc", "Disable Pickup Notifications", false, LookingGlassLanguageAPI.ConfigDescription("Disable Pickup Notifications", "Disable item pickup notifications"));
            InitHooks();
            SetupRiskOfOptions();
        }
        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(disablePickupNotifications, new CheckBoxConfig() { category = LookingGlassLanguageAPI.ConfigCategory("Misc"), name = LookingGlassLanguageAPI.ConfigName(disablePickupNotifications.Definition.Key), restartRequired = false }));
        }
        void InitHooks()
        {
            var targetMethod = typeof(CharacterMasterNotificationQueue).GetMethod(nameof(CharacterMasterNotificationQueue.PushPickupNotification), new[] { typeof(CharacterMaster), typeof(PickupIndex), typeof(bool), typeof(int) });
            //var targetMethod = typeof(CharacterMasterNotificationQueue).GetMethod(nameof(CharacterMasterNotificationQueue.PushPickupNotification), BindingFlags.Public | BindingFlags.Static);
            var destMethod = typeof(HidePickupNotifications).GetMethod(nameof(PushPickupNotification), BindingFlags.NonPublic | BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
        }
        void PushPickupNotification(Action<CharacterMaster, PickupIndex, bool, int> orig, CharacterMaster characterMaster, PickupIndex pickupIndex, bool isTemporary, int upgradeCount)
        {
            if (disablePickupNotifications.Value)
            {
                return;
            }
            orig(characterMaster, pickupIndex, isTemporary, upgradeCount);
        }
    }
}
