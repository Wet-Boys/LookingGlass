using BepInEx;
using LookingGlass.AutoSortItems;
using LookingGlass.CommandWindowBlur;
using LookingGlass.EscapeToCloseMenu;
using LookingGlass.HidePickupNotifs;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LookingGlass
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class BasePlugin : BaseUnityPlugin
    {
        public const string PluginGUID = $"{PluginAuthor}.{PluginName}";
        public const string PluginAuthor = "Balls";
        public const string PluginName = "InYourMouth";
        public const string PluginVersion = "1.0.0";
        internal static BasePlugin instance;
        internal AutoSortItemsClass autoSortItems;
        internal NoWindowBlur noWindowBlur;
        internal ButtonsToCloseMenu buttonsToCloseMenu;
        internal HidePickupNotifications hidePickupNotifications;
        public void Awake()
        {
            Log.Init(Logger);
            instance = this;
            autoSortItems = new AutoSortItemsClass();
            noWindowBlur = new NoWindowBlur();
            buttonsToCloseMenu = new ButtonsToCloseMenu();
            hidePickupNotifications = new HidePickupNotifications();
        }

        private void Update()
        {
            if (ButtonsToCloseMenu.buttonsToClickOnMove.Count != 0 && Input.anyKeyDown && !Input.GetMouseButtonDown(0))
            {
                ButtonsToCloseMenu.CloseMenuAfterFrame();
            }
        }
    }
}
