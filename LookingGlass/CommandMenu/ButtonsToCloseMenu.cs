using BepInEx.Configuration;
using HG;
using LookingGlass.Base;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.UI;
using UnityEngine;

namespace LookingGlass.EscapeToCloseMenu
{
    public class ClosePickupPicker : MonoBehaviour
    {
        public Rewired.Player player;
        public HGButton closeButton;
        public NetworkUIPromptController networkUIPromptController;
        public Interactor interactor;
        public void Start()
        {
            //ScrapperPickerPanel(Clone)/MainPanel/Juice/CancelButton
            //CommandPickerPanel(Clone)/MainPanel/Juice/CancelButton
            //DroneScrapperPickerPanel(Clone)/MainPanel/Juice/CancelButton
            //All got the same path

            var button = this.transform.Find("MainPanel/Juice/CancelButton");
            if (button)
            {
                closeButton = button.GetComponent<HGButton>();
            }
            if (networkUIPromptController.currentLocalParticipant != null)
            {
                player = networkUIPromptController.currentLocalParticipant.inputPlayer;
            }
            if (networkUIPromptController.currentParticipantMaster != null)
            {
                player = networkUIPromptController.currentLocalParticipant.inputPlayer;

            }

        }
        public void Update()
        {
            if (player.GetButton(5)) //Interact Key
            {
                return;
            }
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) //Mouse Keys
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Return)) //Requested
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.F2)) //Console opening keys
            {
                return;
            }
            if (Input.anyKeyDown)
            {
                CloseMenuAfterFrame();
            }
        }
        public void CloseMenuAfterFrame()
        {
            if (closeButton)
            {
                closeButton.InvokeClick();
            }
            if (networkUIPromptController.currentLocalParticipant != null)
            {
                if (networkUIPromptController.currentLocalParticipant.cachedBody)
                {
                    if (networkUIPromptController.currentLocalParticipant.cachedBody.TryGetComponent<InteractionDriver>(out var interactor))
                    {
                        interactor.currentInteractable = null;
                        interactor.interactableCheckCooldown = 0.1f;
                    }
                }
            }
        }


    }

    internal class ButtonsToCloseMenu : BaseThing
    {
        public static ConfigEntry<bool> turnOffCommandMenu;

        //internal static List<HGButton> buttonsToClickOnMove = new List<HGButton>();
        //internal static HGButton buttonToClickOnMove = null;
        //internal static bool check = false;

        private static bool interactHoldBlocker = false;
        private static float holdBlockerStartTime = 0;
        //private static PickupPickerController pickupPickerController;

        public ButtonsToCloseMenu()
        {
            Setup();
        }
        public void Setup()
        {
            turnOffCommandMenu = BasePlugin.instance.Config.Bind<bool>("Command Settings", "Input Disables Command Prompt", true, "Makes any keyboard input other than [Interact] close command/ scrapper / potential / etc. menus.\n\nDoesn't work with controllers because you wouldn't be able to use the menu.");
            SetupRiskOfOptions();
        }
        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(turnOffCommandMenu, new CheckBoxConfig() { name = "Input closes Command/Pickup menus", restartRequired = false }));
        }
        public void AddCloser(GameObject panelInstance, NetworkUIPromptController net)
        {
            if (turnOffCommandMenu.Value && panelInstance)
            {
                panelInstance.EnsureComponent<ClosePickupPicker>().networkUIPromptController = net;
            }
        }

        /* IEnumerator AddToArrayAfterFrame(PickupPickerController self)
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
         }*/

        //This thing, running literally every frame forever every forever
        //Nah dude dont do that cmon
        /*public void Update()
        {
          
            if (buttonsToClickOnMove.Count != 0 && Input.anyKeyDown && !Input.GetMouseButtonDown(0))
            {
                CloseMenuAfterFrame();
            }

            // Interact hold blocker to prevent rappidly opening/closing menu because that breaks the menu
            if (interactHoldBlocker && (!LocalUserManager.GetFirstLocalUser().inputPlayer.GetButton(5) || Time.time >= holdBlockerStartTime + 0.5))
            {//if blocker is active and  player is not holding interact or it has been more than 0.5 seconds
                interactHoldBlocker = false; //turn off blocker
                
                if (pickupPickerController != null) // the command menu destroys the pickupPickerController before thus runs, so must check if it is null
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

                    if (pickupPickerController != null) // the command menu destroys the pickupPickerController before thus runs, so must check if it is null
                    {
                        pickupPickerController.enabled = false;
                        // toggle the pickupPickerController like this and not with PickupPickerController.available because this way isnt networked, and the networking is what was causing issues
                    }
                    holdBlockerStartTime = Time.time;
                }
                buttonsToClickOnMove.RemoveAt(0);
            }
        }
        */


    }
}
