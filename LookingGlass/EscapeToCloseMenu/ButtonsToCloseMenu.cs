using BepInEx.Configuration;
using LookingGlass.Base;
using LookingGlass.CommandWindowBlur;
using LeTai.Asset.TranslucentImage;
using MonoMod.RuntimeDetour;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;

namespace LookingGlass.EscapeToCloseMenu
{
    internal class ButtonsToCloseMenu : BaseThing
    {
        public static ConfigEntry<bool> turnOffCommandMenu;
        private static Hook overrideHook;
        internal static List<HGButton> buttonsToClickOnMove = new List<HGButton>();

        private static bool interactHoldBlocker = false;
        private static float holdBlockerStartTime = 0;
        private static PickupPickerController pickupPickerController;

        public ButtonsToCloseMenu()
        {
            Setup();
        }
        public void Setup()
        {
            turnOffCommandMenu = BasePlugin.instance.Config.Bind<bool>("Command Settings", "Input Disables Command Prompt", true, "Makes any keyboard/mouse input disable the command prompt. Doesn't work with controllers because you wouldn't be able to use the menu.");
            SetupRiskOfOptions();
        }
        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(turnOffCommandMenu, new CheckBoxConfig() { restartRequired = false }));
        }
        public void OnDisplayBeginStuff(PickupPickerController self)
        {

            pickupPickerController = self;

            if (turnOffCommandMenu.Value)
            {
                BasePlugin.instance.StartCoroutine(AddToArrayAfterFrame(self));
            }
        }
        IEnumerator AddToArrayAfterFrame(PickupPickerController self)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            foreach (var item in self.panelInstance.GetComponentsInChildren<HGButton>())
            {
                if (item.name.Equals("CancelButton"))
                {
                    buttonsToClickOnMove.Add(item);
                }
            }
        }

        public void Update()
        {

            if (buttonsToClickOnMove.Count != 0 && Input.anyKeyDown && !Input.GetMouseButtonDown(0))
            {
                CloseMenuAfterFrame();
            }

            if (interactHoldBlocker && (!LocalUserManager.GetFirstLocalUser().inputPlayer.GetButton(5) || Time.time >= holdBlockerStartTime + 0.5)) //if blocker is active and  player is not holding interact
            {

                interactHoldBlocker = false; //turn off blocker
                
                if (pickupPickerController != null) // the command menu destorys the pickupPickerController before thus runs, so must check if it is null
                {
                    pickupPickerController.enabled = true;
                    // toggle the pickupPickerController like this and not with PickupPickerController.available because this way isnt networked, and the networking is what was causing issues
                }
            }

        }

        public static void CloseMenuAfterFrame()
        {
            while (buttonsToClickOnMove.Count != 0)
            {
                if (buttonsToClickOnMove[0] is not null)
                {
                    buttonsToClickOnMove[0].InvokeClick();
                    interactHoldBlocker = true;

                    if (pickupPickerController != null) // the command menu destorys the pickupPickerController before thus runs, so must check if it is null
                    {
                        pickupPickerController.enabled = false;
                        // toggle the pickupPickerController like this and not with PickupPickerController.available because this way isnt networked, and the networking is what was causing issues
                    }
                    holdBlockerStartTime = Time.time;
                }
                buttonsToClickOnMove.RemoveAt(0);
            }
        }
    }
}
