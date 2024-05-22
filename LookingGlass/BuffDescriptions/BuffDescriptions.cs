using LookingGlass.Base;
using MonoMod.RuntimeDetour;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LookingGlass.BuffDescriptions
{
    public class BuffDescriptionsClass : BaseThing
    {
        private static Hook overrideHook;

        public BuffDescriptionsClass()
        {
            Setup();
            SetupRiskOfOptions();
        }
        public void Setup()
        {
            var targetMethod = typeof(BuffIcon).GetMethod(nameof(BuffIcon.UpdateIcon), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(BuffDescriptionsClass).GetMethod(nameof(BuffIconUpdateIcon), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
        }

        public void SetupRiskOfOptions()
        {

        }
        void BuffIconUpdateIcon(Action<BuffIcon> orig, BuffIcon self)
        {
            orig(self);
            if (!self.GetComponent<TooltipProvider>() && self.buffDef)
            {
                if (!self.GetComponentInParent<Canvas>().gameObject.GetComponent<GraphicRaycaster>())
                {
                    self.GetComponentInParent<Canvas>().gameObject.AddComponent<GraphicRaycaster>();
                }
                TooltipContent content = new TooltipContent();
                content.titleToken = self.buffDef.name;
                content.overrideTitleText = self.buffDef.name;
                content.titleColor = Color.gray;
                content.bodyToken = self.buffDef.name;
                content.overrideBodyText = self.buffDef.name;
                content.bodyColor = Color.blue;
                content.disableTitleRichText = false;
                content.disableBodyRichText = false;
                TooltipProvider toolTip = self.gameObject.AddComponent<TooltipProvider>();
                toolTip.SetContent(content);
            }
        }
    }
}
