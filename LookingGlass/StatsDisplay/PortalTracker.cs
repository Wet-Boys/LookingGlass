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
    
        public PortalTracker()
        {
            Setup();
        }
        public void Setup()
        {
            //Green Portal -> Also when Goldshores?
            //Green Portal -> Also when Bazaar Mini Geodes?
            //Gold, Void Portal -> Also when Lunar Seer??


            var targetMethod = typeof(Stage).GetMethod(nameof(Stage.PreStartClient), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(PortalTracker).GetMethod(nameof(StageStart), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var overrideHook2 = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(PortalSpawner).GetMethod(nameof(PortalSpawner.OnWillSpawnUpdated), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(PortalTracker).GetMethod(nameof(TrackPortalSpawner), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook2 = new Hook(targetMethod, destMethod, this);
        }

        public void SetupRiskOfOptions()
        {
        }

        void TrackPortalSpawner(Action<PortalSpawner, bool> orig, PortalSpawner self, bool newValue)
        {
            //Track DLC Teleporter Portals
            //Track Halcyon Shrines
            Debug.Log(self.previewChildName);
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
        }
 
    }
}
