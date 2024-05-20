using LookingGlass.DPSMeterStuff;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LookingGlass.StatsDisplay
{
    internal class StatsDisplayDefinitions
    {
        internal static void SetupDefs()
        {
            string utilityString = StatsDisplayClass.builtInColors.Value ? "<style=\"cIsUtility>" : "";
            string damageString = StatsDisplayClass.builtInColors.Value ? "<style=\"cIsDamage>" : "";
            string healingString = StatsDisplayClass.builtInColors.Value ? "<style=\"cIsHealing>" : "";
            string healthString = StatsDisplayClass.builtInColors.Value ? "<style=\"cIsHealth>" : "";
            string styleString = StatsDisplayClass.builtInColors.Value ? "</style>" : "";
            StatsDisplayClass.statDictionary.Clear();
            StatsDisplayClass.statDictionary.Add("luck", cachedUserBody => { return $"{utilityString}{(cachedUserBody.inventory.GetItemCount(RoR2Content.Items.Clover) - cachedUserBody.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck))}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("baseDamage", cachedUserBody => { return $"{damageString}{(cachedUserBody.damage)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("crit", cachedUserBody => { return $"{damageString}{(cachedUserBody.crit)}%{styleString}"; });
            StatsDisplayClass.statDictionary.Add("attackSpeed", cachedUserBody => { return $"{damageString}{(cachedUserBody.attackSpeed)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("armor", cachedUserBody => { return $"{healingString}{(cachedUserBody.armor)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("armorDamageReduction", cachedUserBody => { return $"{healingString}{(100 - (100 * (100 / (100 + cachedUserBody.armor)))):0.###}%{styleString}"; });
            StatsDisplayClass.statDictionary.Add("regen", cachedUserBody => { return $"{healingString}{(cachedUserBody.regen)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("speed", cachedUserBody => { return $"{utilityString}{(cachedUserBody.moveSpeed),6:N2}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("availableJumps", cachedUserBody => { return $"{utilityString}{(cachedUserBody.maxJumpCount - cachedUserBody.characterMotor.jumpCount)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("maxJumps", cachedUserBody => { return $"{utilityString}{(cachedUserBody.maxJumpCount)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("killCount", cachedUserBody => { return $"{healthString}{(cachedUserBody.killCountServer)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("mountainShrines", cachedUserBody => { return $"{utilityString}{((TeleporterInteraction.instance is not null ? TeleporterInteraction.instance.shrineBonusStacks : "N/A"))}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("experience", cachedUserBody => { return $"{utilityString}{(cachedUserBody.experience)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("level", cachedUserBody => { return $"{utilityString}{(cachedUserBody.level)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("maxHealth", cachedUserBody => { return $"{healthString}{(cachedUserBody.maxHealth)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("maxBarrier", cachedUserBody => { return $"{utilityString}{(cachedUserBody.maxBarrier)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("barrierDecayRate", cachedUserBody => { return $"{utilityString}{(cachedUserBody.barrierDecayRate)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("maxShield", cachedUserBody => { return $"{utilityString}{(cachedUserBody.maxShield)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("acceleration", cachedUserBody => { return $"{utilityString}{(cachedUserBody.acceleration)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("jumpPower", cachedUserBody => { return $"{utilityString}{(cachedUserBody.jumpPower)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("maxJumpHeight", cachedUserBody => { return $"{utilityString}{(cachedUserBody.maxJumpHeight)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("damage", cachedUserBody => { return $"{damageString}{(cachedUserBody.damage)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("critMultiplier", cachedUserBody => { return $"{damageString}{(cachedUserBody.critMultiplier)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("bleedChance", cachedUserBody => { return $"{damageString}{(cachedUserBody.bleedChance)}%{styleString}"; });
            StatsDisplayClass.statDictionary.Add("visionDistance", cachedUserBody => { return $"{utilityString}{(cachedUserBody.visionDistance)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("critHeal", cachedUserBody => { return $"{healingString}{(cachedUserBody.critHeal)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("cursePenalty", cachedUserBody => { return $"{utilityString}{(cachedUserBody.cursePenalty)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("hasOneShotProtection", cachedUserBody => { return $"{utilityString}{(cachedUserBody.hasOneShotProtection)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("isGlass", cachedUserBody => { return $"{utilityString}{(cachedUserBody.isGlass)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("canPerformBackstab", cachedUserBody => { return $"{damageString}{(cachedUserBody.canPerformBackstab)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("canReceiveBackstab", cachedUserBody => { return $"{damageString}{(cachedUserBody.canReceiveBackstab)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("healthPercentage", cachedUserBody => { return $"{healthString}{(cachedUserBody.healthComponent.combinedHealthFraction * 100f)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("goldPortal", cachedUserBody => { return $"{utilityString}{(TeleporterInteraction.instance is not null ? TeleporterInteraction.instance.shouldAttemptToSpawnGoldshoresPortal.ToString() : "N/A")}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("msPortal", cachedUserBody => { return $"{utilityString}{(TeleporterInteraction.instance is not null ? TeleporterInteraction.instance.shouldAttemptToSpawnMSPortal.ToString() : "N/A")}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("shopPortal", cachedUserBody => { return $"{utilityString}{(TeleporterInteraction.instance is not null ? TeleporterInteraction.instance.shouldAttemptToSpawnShopPortal.ToString() : "N/A")}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("dps", cachedUserBody => { return $"{damageString}{BasePlugin.instance.dpsMeter.damageDealtSincePeriod / DPSMeter.DPS_MAX_TIME}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("currentCombatDamage", cachedUserBody => { return $"{damageString}{BasePlugin.instance.dpsMeter.currentCombatDamage}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("remainingComboDuration", cachedUserBody => { return $"{utilityString}{(int)BasePlugin.instance.dpsMeter.timer + 1}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("maxCombo", cachedUserBody => { return $"{damageString}{BasePlugin.instance.dpsMeter.maxCombo}{styleString}"; });

            StatsDisplayClass.statDictionary.Add("critWithLuck", cachedUserBody =>
            {
                int luck = cachedUserBody.inventory.GetItemCount(RoR2Content.Items.Clover) - cachedUserBody.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck);
                return $"{damageString}{CalculateChance(cachedUserBody.crit, luck):0.###}%{styleString}";
            });
            StatsDisplayClass.statDictionary.Add("bleedChanceWithLuck", cachedUserBody =>
            {
                int luck = cachedUserBody.inventory.GetItemCount(RoR2Content.Items.Clover) - cachedUserBody.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck);
                return $"{damageString}{CalculateChance(cachedUserBody.bleedChance, luck):0.###}%{styleString}";
            });

            StatsDisplayClass.statDictionary.Add("velocity", cachedUserBody =>
            {
                Rigidbody r = cachedUserBody.GetComponent<Rigidbody>();
                if (r)
                {
                    return $"{utilityString}{r.velocity.magnitude,6:N2}{styleString}";
                }
                return $"{utilityString}N/A{styleString}";
            });
            StatsDisplayClass.statDictionary.Add("teddyBearBlockChance", cachedUserBody => {
                int stackCount = cachedUserBody.inventory.GetItemCount(RoR2Content.Items.Bear);
                return $"{utilityString}{((0.15f * stackCount) / ((0.15f * stackCount) + 1)) * 100:0.###}%{styleString}";
            });
            StatsDisplayClass.statDictionary.Add("saferSpacesCD", cachedUserBody => {
                int stackCount = cachedUserBody.inventory.GetItemCount(DLC1Content.Items.BearVoid);
                if (stackCount == 0)
                {
                    return $"{utilityString}N/A{styleString}";
                }
                return $"{utilityString}{15 * Mathf.Pow(.9f, stackCount):0.###}{styleString}";
            });
            StatsDisplayClass.statDictionary.Add("instaKillChance", cachedUserBody => {
                int luck = cachedUserBody.inventory.GetItemCount(RoR2Content.Items.Clover) - cachedUserBody.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck);
                int stackCount = cachedUserBody.inventory.GetItemCount(DLC1Content.Items.CritGlassesVoid);
                return $"{damageString}{(CalculateChance(.5f * stackCount, luck)):0.###}%{styleString}";
            });
        }
        internal static float CalculateChance(float baseChance, int luck) //baseChance should be between 0 and 1
        {
            float num;
            if (baseChance >= 100)
            {
                return 100;
            }
            else if (luck > 0)
            {
                num = (1 - Mathf.Pow(1 - (baseChance * .01f), luck + 1)) * 100;
            }
            else if (luck < 0)
            {
                num = Mathf.Pow((baseChance * .01f), Mathf.Abs(luck) + 1) * 100;
            }
            else
            {
                num = baseChance;
            }
            return num;
        }
    }
}
