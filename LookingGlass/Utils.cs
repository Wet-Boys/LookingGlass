using RoR2;
using System;
using UnityEngine;

namespace LookingGlass
{
    class Utils
    {
        public static float CalculateChance(float baseChance, float luckIn)
        {
            int luck = Mathf.CeilToInt(luckIn);
            if (luck > 0)
                return 1f - Mathf.Pow(1f - baseChance, luck + 1);
            if (luck < 0)
                return Mathf.Pow(baseChance, Mathf.Abs(luck) + 1);

            return baseChance;
        }

        public static float GetLuckFromCachedUserBody(CharacterBody cachedUserBody)
        {
            // Check for modded luck values
            if (cachedUserBody && cachedUserBody.master) return cachedUserBody.master.luck;

            return cachedUserBody.inventory.GetItemCount(RoR2Content.Items.Clover) - cachedUserBody.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck);
        }
    }
}
