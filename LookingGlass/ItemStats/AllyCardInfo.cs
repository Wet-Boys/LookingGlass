using BepInEx.Configuration;
using LookingGlass.Base;
using LookingGlass.BuffDescriptions;
using LookingGlass.ItemStatsNameSpace;
using MonoMod.RuntimeDetour;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace LookingGlass
{
    internal class AllyCardInfo : BaseThing
    {
        public static ConfigEntry<bool> allyInfoConfig;

        public AllyCardInfo()
        {
            Setup();
        }
        public void Setup()
        {
            allyInfoConfig = BasePlugin.instance.Config.Bind<bool>("Misc", "Ally Info", true, "Hovering over ally cards shows info if they are a Drone.");
            ModSettingsManager.AddOption(new CheckBoxOption(allyInfoConfig, new CheckBoxConfig() { restartRequired = false }));

            var targetMethod = typeof(AllyCardController).GetMethod(nameof(AllyCardController.UpdateInfo), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(AllyCardInfo).GetMethod(nameof(AddAllyTooltips), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(AllyCardController).GetMethod(nameof(AllyCardController.Awake), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(AllyCardInfo).GetMethod(nameof(MakeTargetable), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(AllyCardManager).GetMethod(nameof(AllyCardManager.OnEnable), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(AllyCardInfo).GetMethod(nameof(AddRaycaster), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            new Hook(targetMethod, destMethod, this);
        }

        public void AddRaycaster(Action<AllyCardManager> orig, AllyCardManager self)
        {
            orig(self); 
            if (!self.GetComponent<GraphicRaycaster>())
            {
                self.gameObject.AddComponent<GraphicRaycaster>();
            }
        }
        public void MakeTargetable(Action<AllyCardController> orig, AllyCardController self)
        {
            orig(self);
            // Is it more optimized to have more hooks (ie do this in awake)
            // Or less hooks but some redundency
            self.GetComponent<Image>().raycastTarget = true;

        }

        public void AddAllyTooltips(Action<AllyCardController> orig, AllyCardController self)
        {
            orig(self);
            if (!allyInfoConfig.Value)
            {
                return;
            }
            TooltipProvider tooltipProvider = self.GetComponent<TooltipProvider>();
            if (!tooltipProvider)
            {
                tooltipProvider = self.gameObject.AddComponent<TooltipProvider>();
            }
            if (self.sourceMaster && self.cachedSourceCharacterBody)
            {
 
                TooltipContent content = new TooltipContent();
                content.titleToken = self.cachedSourceCharacterBody.baseNameToken;
                content.titleColor = self.cachedSourceCharacterBody.bodyColor;

                if (content.titleColor == Color.clear)
                {
                    content.titleColor = new Color(0.4902f, 0.8784f, 0.2588f, 1f);
                }
                
                DroneIndex drone = DroneCatalog.GetDroneIndexFromBodyIndex(self.cachedSourceCharacterBody.bodyIndex);
                if (drone != DroneIndex.None)
                {
                    content.bodyToken = DroneCatalog.GetDroneDef(drone).skillDescriptionToken;

                    //Could add a system to show info for drone-tiers.
                    //Could show Operator command, tho his little bar already shows that.
                }
                /*SurvivorIndex survivor = SurvivorCatalog.GetSurvivorIndexFromBodyIndex(self.cachedSourceCharacterBody.bodyIndex);
                if (survivor != SurvivorIndex.None)
                {  
                }*/
                tooltipProvider.enabled = true;
                tooltipProvider.SetContent(content);
            }
            else
            {
                tooltipProvider.enabled = false;
            }
        }

        public void SetupRiskOfOptions()
        {
        }
 
    }
}
