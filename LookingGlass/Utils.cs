using LookingGlass.ItemStatsNameSpace;
using RoR2;
using System;
using System.Collections.Generic;
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
            if (ItemStats.capChancePercentage.Value)
            {
                baseChance = Mathf.Min(baseChance, 1);
            }
            float chanceFloored = Mathf.Floor(baseChance);
            float chanceMod = baseChance % 1f;
            int luck = Mathf.CeilToInt(luckIn);
            if (luck > 0)
                return chanceFloored + (1f - Mathf.Pow(1f - chanceMod, luck + 1));
            if (luck < 0)
                return chanceFloored + Mathf.Pow(chanceMod, Mathf.Abs(luck) + 1);

            return baseChance;
        }

        public static float GetLuckFromCachedUserBody(CharacterBody cachedUserBody)
        {
            // Check for modded luck values
            if (cachedUserBody && cachedUserBody.master) return cachedUserBody.master.luck;
            return 0;
 
        }

        public static float GetExponentialRechargeTime(float baseCooldown, float extraPercent, int count)
        {
            return baseCooldown * Mathf.Pow(1 - extraPercent, count - 1);
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

        public static string DictToString<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            string s = "{\n";
            foreach (KeyValuePair<TKey, TValue> kv in dict)
            {
                s += $"[{kv.Key}] = {kv.Value}\n";
            }
            s += "}";
            return s;
        }
    }
}
