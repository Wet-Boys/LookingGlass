using RoR2;
using System;
using UnityEngine;

namespace LookingGlass
{
    class Utils
    {
        public static float CalculateChanceWithNullableMaster(float baseChance, CharacterMaster master)
        {
            if (master)
                return CalculateChanceWithLuck(baseChance, master.luck);
            
            return baseChance;
        }

        public static float CalculateChanceWithLuck(float baseChance, float luckIn)
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

        public static float GetExponentialRechargeTime(float baseCooldown, float basePercent, float extraPercent, int count)
        {
            return baseCooldown * basePercent * Mathf.Pow(1 - extraPercent, count - 1);
        }

        public static float GetExponentialStacking(float basePercent, float extraPercent, int count)
        {
            return 1 - (1 - basePercent) * Mathf.Pow(1 - extraPercent, count - 1);
        }

        public static float GetHyperbolicStacking(float basePercent, float extraPercent, int count)
        {
            return 1f - 1f / (1f + basePercent + extraPercent * (count - 1));
        }

        public static float GetBandolierStacking(int count)
        {
            return 1f - 1f / Mathf.Pow(1f + count, 0.33f);
        }
    }
}
