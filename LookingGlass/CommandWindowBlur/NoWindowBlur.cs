using BepInEx.Configuration;
using LookingGlass.Base;
using LeTai.Asset.TranslucentImage;
using MonoMod.RuntimeDetour;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;
using static Rewired.InputMapper;
using System.Collections.ObjectModel;

namespace LookingGlass.CommandWindowBlur
{
    internal class NoWindowBlur : BaseThing
    {
        public static ConfigEntry<bool> disable;
        private static Hook overrideHook;
        public NoWindowBlur()
        {
            Setup();
        }
        public void Setup()
        {
            disable = BasePlugin.instance.Config.Bind<bool>("Command Settings", "Disable Command Window Blur", true, "Disable the background blur effect from the command window");
            InitHooks();
            SetupRiskOfOptions();
        }
        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(disable, new CheckBoxConfig() { restartRequired = false }));
        }
        void InitHooks()
        {
            var targetMethod = typeof(RoR2.PickupPickerController).GetMethod(nameof(RoR2.PickupPickerController.OnDisplayBegin), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(NoWindowBlur).GetMethod(nameof(OnDisplayBegin), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
        }
        void OnDisplayBegin(Action<RoR2.PickupPickerController, NetworkUIPromptController, LocalUser, CameraRigController> orig, RoR2.PickupPickerController self, NetworkUIPromptController networkUIPromptController, LocalUser localUser, CameraRigController cameraRigController)
        {

            BasePlugin.instance.commandItemCountClass.isFromOnDisplayBegin = true; // tell the scrapper sorting that it was called from OnDisplayBegin

            orig(self, networkUIPromptController, localUser, cameraRigController);

            TranslucentImage t = self.panelInstance.gameObject.GetComponentInChildren<TranslucentImage>();
            if (t is not null)
            {
                t.enabled = !disable.Value;
            }
            BasePlugin.instance.buttonsToCloseMenu.OnDisplayBeginStuff(self);
            BasePlugin.instance.resizeCommandWindowClass.ResizeWindow(self);
        }
    }
}
