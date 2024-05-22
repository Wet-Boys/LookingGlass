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
        private static Hook overrideHook2;

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
            if (self.buffDef)
            {
                TooltipProvider toolTip = self.GetComponent<TooltipProvider>();
                if (!toolTip)
                {
                    if (!self.GetComponentInParent<Canvas>().gameObject.GetComponent<GraphicRaycaster>())
                    {
                        self.GetComponentInParent<Canvas>().gameObject.AddComponent<GraphicRaycaster>();
                    }
                    TooltipContent content = new TooltipContent();
                    content.titleToken = Language.GetString($"LG_TOKEN_NAME_{self.buffDef.buffIndex}");
                    content.titleColor = Color.gray;
                    content.bodyToken = Language.GetString($"LG_TOKEN_DESCRIPTION_{self.buffDef.buffIndex}");
                    content.bodyColor = Color.blue;
                    content.disableTitleRichText = false;
                    content.disableBodyRichText = false;
                    toolTip = self.gameObject.AddComponent<TooltipProvider>();
                    toolTip.SetContent(content);
                }
                if (toolTip)
                {
                    toolTip.titleToken = Language.GetString($"LG_TOKEN_NAME_{self.buffDef.buffIndex}");
                    toolTip.bodyToken = Language.GetString($"LG_TOKEN_DESCRIPTION_{self.buffDef.buffIndex}");
                }
            }
        }
    }
}
