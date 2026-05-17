using BepInEx;
using LookingGlass.AutoSortItems;
using LookingGlass.BuffDescriptions;
using LookingGlass.BuffTimers;
using LookingGlass.CommandItemCount;
using LookingGlass.CommandWindowBlur;
using LookingGlass.DPSMeterStuff;
using LookingGlass.EquipTimerFix;
using LookingGlass.EscapeToCloseMenu;
using LookingGlass.HiddenItems;
using LookingGlass.HidePickupNotifs;
using LookingGlass.ItemCounters;
using LookingGlass.ItemStatsNameSpace;
using LookingGlass.LookingGlassLanguage;
using LookingGlass.PickupNotifsDuration;
using LookingGlass.ResizeCommandWindow;
using LookingGlass.StatsDisplay;
using RiskOfOptions;
using RoR2;
using System.IO;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
[module: UnverifiableCode]

namespace LookingGlass
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.Gorakh.ItemQualities", BepInDependency.DependencyFlags.SoftDependency)]
    public class BasePlugin : BaseUnityPlugin
    {
        internal static BasePlugin instance;
        internal AllyCardInfo allyCardStuff;
        internal AutoSortItemsClass autoSortItems;
        internal NoWindowBlur noWindowBlur;
        internal ButtonsToCloseMenu buttonsToCloseMenu;
        internal HidePickupNotifications hidePickupNotifications;
        internal ItemStats itemStats;
        internal CommandItemCountClass commandItemCountClass;
        internal ModifyCommandWindow resizeCommandWindowClass;
        internal StatsDisplayClass statsDisplayClass;
        internal DPSMeter dpsMeter;
        internal PortalTracker portalTracking;
        internal ItemCounter itemCounter;
        internal BuffTimersClass buffTimers;
        internal CooldownFixer equipFixer;
        internal UnHiddenItems unHiddenItems;
        internal BuffDescriptionsClass buffDescriptions;
        internal PickupNotifDurationClass pickupNotifDurationClass;
        public static byte[] logo;
        public static Sprite logo2;

        internal bool ItemQualitiesLoaded = false;

        public void Awake()
        {
            Log.Init(Logger);
            instance = this;
            LookingGlassLanguageAPI.Init(Info.Location);
            try
            {
                string folderName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), "icons");
                int i = UnityEngine.Random.Range(0, Directory.GetFiles(folderName).Length);
                int i2 = 0;
                foreach (var file in Directory.GetFiles(folderName))
                {
                    if (i2 == i)
                    {
                        logo = File.ReadAllBytes(file);
                    }
                    i2++;
                }
                Texture2D t = LoadTexture(logo, 256, 256);
                logo2 = Sprite.Create(t, new Rect(0.00f, 0.00f, t.width, t.height), new Vector2(0, 0), 100, 1, SpriteMeshType.Tight, new Vector4(0, 0, 0, 0), true);
                ModSettingsManager.SetModIcon(logo2);
            }
            catch (System.Exception)
            {
            }

            ItemQualitiesLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Gorakh.ItemQualities");

            statsDisplayClass = new StatsDisplayClass(); //More important config to have in first slot?
            autoSortItems = new AutoSortItemsClass();

            itemStats = new ItemStats();

            noWindowBlur = new NoWindowBlur();
            buttonsToCloseMenu = new ButtonsToCloseMenu();

            commandItemCountClass = new CommandItemCountClass();
            resizeCommandWindowClass = new ModifyCommandWindow();
            itemCounter = new ItemCounter();
            buffTimers = new BuffTimersClass();

            portalTracking = new PortalTracker();
            equipFixer = new CooldownFixer();

            buffDescriptions = new BuffDescriptionsClass();
            allyCardStuff = new AllyCardInfo();
            hidePickupNotifications = new HidePickupNotifications();
            pickupNotifDurationClass = new PickupNotifDurationClass();
            unHiddenItems = new UnHiddenItems();
            dpsMeter = new DPSMeter();

            statsDisplayClass.CheckForOldDefaultSettingsThatNeedToBeUpdated();

            ModSettingsManager.SetModDescription(LookingGlassLanguageAPI.GetString("MOD_DESCRIPTION", "Stat info, item stacking info, skill and equipment cooldown info, and much more ui related things."));
        }

        private void FixedUpdate()
        {
            dpsMeter.FixedUpdate();

            // get current camera body target
            StatsDisplayClass.cachedUserBody = LocalUserManager.GetFirstLocalUser()?.cameraRigController ?
                LocalUserManager.GetFirstLocalUser().cameraRigController.targetBody :
                null;

            statsDisplayClass.FixedUpdate();
        }


        public static Texture2D LoadTexture(byte[] bytes, int width, int height)
        {
            Texture2D Tex2D = new Texture2D(width, height, TextureFormat.ARGB32, false, false);
            Tex2D.LoadImage(bytes);
            Tex2D.filterMode = FilterMode.Point;
            return Tex2D;
        }
    }
}
