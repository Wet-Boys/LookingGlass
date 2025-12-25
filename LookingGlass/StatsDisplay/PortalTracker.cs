using BepInEx.Configuration;
using LookingGlass.Base;
using LookingGlass.BuffDescriptions;
using MonoMod.RuntimeDetour;
using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Reflection;
 
namespace LookingGlass
{
    internal class PortalTracker : BaseThing
    {
        public bool shopPortal
        {
            get
            {
                return TeleporterInteraction.instance && TeleporterInteraction.instance.shouldAttemptToSpawnShopPortal;
            }
        }
        public bool _goldPortal = false;
        public bool goldPortal
        {
            get
            {
                return _goldPortal || TeleporterInteraction.instance && TeleporterInteraction.instance.shouldAttemptToSpawnGoldshoresPortal;
            }
        }
        public bool msPortal
        {
            get
            {
                return TeleporterInteraction.instance && TeleporterInteraction.instance.shouldAttemptToSpawnMSPortal;
            }
        }
        public bool _greenPortal = false;
        public bool _voidPortal = false;
        public bool _acPortal = false;
        public bool _artifactPortal = false;
        public bool _accessNode = false;

        public PortalTracker()
        {
            Setup();
        }
        public void Setup()
        {
            //Green Portal -> Also when Goldshores?
            //Green Portal -> Also when Bazaar Mini Geodes?
            //Gold, Void Portal -> Also when Lunar Seer??


            var targetMethod = typeof(Stage).GetMethod(nameof(Stage.PreStartClient), BindingFlags.Public | BindingFlags.Instance);
            var destMethod = typeof(PortalTracker).GetMethod(nameof(StageStart), BindingFlags.NonPublic | BindingFlags.Instance);
            var overrideHook2 = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(PortalSpawner).GetMethod(nameof(PortalSpawner.OnWillSpawnUpdated), BindingFlags.NonPublic | BindingFlags.Instance);
            destMethod = typeof(PortalTracker).GetMethod(nameof(TrackPortalSpawner), BindingFlags.NonPublic | BindingFlags.Instance);
            overrideHook2 = new Hook(targetMethod, destMethod, this);
 
           new Hook(
               typeof(PortalDialerController.PortalDialerPerformActionState).GetMethod(nameof(PortalDialerController.PortalDialerPerformActionState.OnEnter), BindingFlags.Public | BindingFlags.Instance),
               typeof(PortalTracker).GetMethod(nameof(ArtifactPortal), BindingFlags.Public | BindingFlags.Instance)
               ,this);

            new Hook(
             typeof(EntityStates.Missions.AccessCodes.Node.NodesOnAndReady).GetMethod(nameof(EntityStates.Missions.AccessCodes.Node.NodesOnAndReady.OnEnter), BindingFlags.Public | BindingFlags.Instance),
             typeof(PortalTracker).GetMethod(nameof(AccessNode), BindingFlags.Public | BindingFlags.Instance)
             ,this);
        }

        public void ArtifactPortal(Action<PortalDialerController.PortalDialerPerformActionState> orig, PortalDialerController.PortalDialerPerformActionState self)
        {
            orig(self);
            _artifactPortal = true;
        }
        public void AccessNode(Action<EntityStates.Missions.AccessCodes.Node.NodesOnAndReady> orig, EntityStates.Missions.AccessCodes.Node.NodesOnAndReady self)
        {
            orig(self);
            _accessNode = true;
        }
        public void SetupRiskOfOptions()
        {

        }

        public string ReturnAllAvailablePortals()
        {
            if (!TeleporterInteraction.instance)
            {
                return "<style=cStack>None</style>";
            }
            else
            {
                string ActivePortals = string.Empty;
                int portals = 0;
                if (shopPortal)
                {
                    portals++;
                    ActivePortals += "<style=cLunarObjective>Bazaar </style>";
                }
                if (goldPortal)
                {
                    portals++;
                    ActivePortals += "<style=cIsDamage>Gold </style>";
                }
                if (_accessNode)
                {
                    portals++;
                    ActivePortals += "<style=cIsHealth>Node </style>";
                }
                if (_acPortal)
                {
                    portals++;
                    ActivePortals += "<style=cIsUtility>Encrypted </style>";
                }
                if (_greenPortal)
                {
                    portals++;
                    ActivePortals += "<style=cIsHealing>Storm </style>";
                }
                if (_voidPortal)
                {
                    portals++;
                    ActivePortals += "<style=cIsVoid>Void </style>";
                }
                if (msPortal)
                {
                    portals++;
                    ActivePortals += "<style=cIsUtility>Celestial </style>";
                }
                if (_artifactPortal)
                {
                    portals++;
                    ActivePortals += "<style=cIsDeath>Artifact </style>";
                }
                if (portals == 0)
                {
                    return "<style=cStack>None</style>";
                }
                return $"<size={(int)(7f/(7f+portals)*100f)}%>{ActivePortals}</size>";
            }
        }

        void TrackPortalSpawner(Action<PortalSpawner, bool> orig, PortalSpawner self, bool newValue)
        {
            //Track DLC Teleporter Portals
            //Track Halcyon Shrines
            //Debug.Log(self.previewChildName);
            if (newValue)
            {
                if (self.previewChildName == "VoidPortalIndicator")
                {
                    _voidPortal = true;
                }
                else if (self.previewChildName == "StormPortalIndicator")
                {
                    _greenPortal = true;
                }
                else if (self.previewChildName == "AccessCodesSymbol")
                {
                    _acPortal = true;
                }
                else if (self.previewChildName == "CEPortalIndicator")
                {
                    _acPortal = true;
                }
                else if (self.previewChildName == "GoldshoresPortalIndicator")
                {
                    _goldPortal = true;
                }
            }
            orig(self, newValue);
        }

        void StageStart(Action<Stage> orig, Stage self)
        {
            orig(self);
            _goldPortal = false;
            _greenPortal = false;
            _voidPortal = false;
            _acPortal = false;
            _artifactPortal = false;
            _accessNode = false;
        }

    }
}
