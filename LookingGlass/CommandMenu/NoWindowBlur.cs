using BepInEx.Configuration;
using LeTai.Asset.TranslucentImage;
using LookingGlass.Base;
using LookingGlass.LookingGlassLanguage;
using MonoMod.RuntimeDetour;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using System;
using System.Reflection;

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
            //Good to have the option,
            //But makes the command menu look like a clipping transparent mess like it's there for a reason.
            //
            disable = BasePlugin.instance.Config.Bind<bool>("Command Settings", "Disable Command Window Blur", false, LookingGlassLanguageAPI.ConfigDescription("Disable Command Window Blur", "Disable the background blur effect from the command window"));
            InitHooks();
            SetupRiskOfOptions();
        }
        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(disable, new CheckBoxConfig() { category = LookingGlassLanguageAPI.ConfigCategory("Command Settings"), name = LookingGlassLanguageAPI.ConfigName(disable.Definition.Key), restartRequired = false }));
        }
        void InitHooks()
        {
            var targetMethod = typeof(RoR2.PickupPickerController).GetMethod(nameof(RoR2.PickupPickerController.OnDisplayBegin), BindingFlags.NonPublic | BindingFlags.Instance);
            var destMethod = typeof(NoWindowBlur).GetMethod(nameof(OnDisplayBegin), BindingFlags.NonPublic | BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);


            new Hook(
                typeof(DroneScrapperPickerController).GetMethod(nameof(DroneScrapperPickerController.OnDisplayBegin), BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(NoWindowBlur).GetMethod(nameof(DroneScrapper_OnDisplayBegin), BindingFlags.NonPublic | BindingFlags.Instance), this);
        }
        void OnDisplayBegin(Action<RoR2.PickupPickerController, NetworkUIPromptController, LocalUser, CameraRigController> orig, RoR2.PickupPickerController self, NetworkUIPromptController networkUIPromptController, LocalUser localUser, CameraRigController cameraRigController)
        {
            BasePlugin.instance.commandItemCountClass.OnDisplayBeginStuff(); // needs to before orig() inorder to set up option menu sorting correctly

            orig(self, networkUIPromptController, localUser, cameraRigController);

            TranslucentImage t = self.panelInstance.gameObject.GetComponentInChildren<TranslucentImage>();
            if (t is not null)
            {
                t.enabled = !disable.Value;
            }
            BasePlugin.instance.buttonsToCloseMenu.AddCloser(self.panelInstance, networkUIPromptController);

        }

        void DroneScrapper_OnDisplayBegin(Action<DroneScrapperPickerController, NetworkUIPromptController, LocalUser, CameraRigController> orig, DroneScrapperPickerController self, NetworkUIPromptController networkUIPromptController, LocalUser localUser, CameraRigController cameraRigController)
        {

            orig(self, networkUIPromptController, localUser, cameraRigController);

            TranslucentImage t = self.panelInstance.gameObject.GetComponentInChildren<TranslucentImage>();
            if (t is not null)
            {
                t.enabled = !disable.Value;
            }
            BasePlugin.instance.buttonsToCloseMenu.AddCloser(self.panelInstance, networkUIPromptController);
        }

    }
}
