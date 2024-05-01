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

namespace LookingGlass.EscapeToCloseMenu
{
    internal class ButtonsToCloseMenu : BaseThing
    {
        public static ConfigEntry<bool> turnOffCommandMenu;
        private static Hook overrideHook;
        internal static List<HGButton> buttonsToClickOnMove = new List<HGButton>();
        public ButtonsToCloseMenu()
        {
            Setup();
        }
        public void Setup()
        {
            turnOffCommandMenu = BasePlugin.instance.Config.Bind<bool>("Settings", "Input Disables Command Prompt", true, "Makes any keyboard/mouse input disable the command prompt. Doesn't work with controllers because you wouldn't be able to use the menu.");
        }
        public void OnDisplayBeginStuff(PickupPickerController self)
        {
            if (turnOffCommandMenu.Value)
            {
                foreach (var item in self.panelInstance.GetComponentsInChildren<HGButton>())
                {
                    if (item.name.Equals("CancelButton"))
                    {
                        BasePlugin.instance.StartCoroutine(AddToArrayAfterFrame(item));
                    }
                }
            }
        }
        IEnumerator AddToArrayAfterFrame(HGButton button)
        {
            yield return new WaitForEndOfFrame();
            buttonsToClickOnMove.Add(button);
        }
        public static void CloseMenuAfterFrame()
        {
            while (buttonsToClickOnMove.Count != 0)
            {
                if (buttonsToClickOnMove[0] is not null)
                {
                    buttonsToClickOnMove[0].InvokeClick();
                }
                buttonsToClickOnMove.RemoveAt(0);
            }
        }
    }
}
