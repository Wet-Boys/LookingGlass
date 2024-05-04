using LookingGlass.DPSMeterStuff;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace LookingGlass.StatsDisplay
{
    internal class StatsDisplayDefinitions
    {
        internal static void SetupDefs()
        {
            StatsDisplayClass.statDictionary.Add("luck", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.inventory.GetItemCount(RoR2Content.Items.Clover) - cachedUserBody.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck))}</style>"; });
            StatsDisplayClass.statDictionary.Add("baseDamage", cachedUserBody => { return $"<style=\"cIsDamage>{(cachedUserBody.baseDamage)}</style>"; });
            StatsDisplayClass.statDictionary.Add("crit", cachedUserBody => { return $"<style=\"cIsDamage>{(cachedUserBody.crit)}</style>"; });
            StatsDisplayClass.statDictionary.Add("attackSpeed", cachedUserBody => { return $"<style=\"cIsDamage>{(cachedUserBody.attackSpeed)}</style>"; });
            StatsDisplayClass.statDictionary.Add("armor", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.armor)}</style>"; });
            StatsDisplayClass.statDictionary.Add("armorDamageReduction", cachedUserBody => { return $"<style=\"cIsUtility>{(100 - (100 * (100 / (100 + cachedUserBody.armor)))):0.###}%</style>"; });
            StatsDisplayClass.statDictionary.Add("regen", cachedUserBody => { return $"<style=\"cIsHealing>{(cachedUserBody.regen)}</style>"; });
            StatsDisplayClass.statDictionary.Add("speed", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.moveSpeed)}</style>"; });
            StatsDisplayClass.statDictionary.Add("availableJumps", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.maxJumpCount - cachedUserBody.characterMotor.jumpCount)}</style>"; });
            StatsDisplayClass.statDictionary.Add("maxJumps", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.maxJumpCount)}</style>"; });
            StatsDisplayClass.statDictionary.Add("killCount", cachedUserBody => { return $"<style=\"cIsHealth>{(cachedUserBody.killCountServer)}</style>"; });
            StatsDisplayClass.statDictionary.Add("mountainShrines", cachedUserBody => { return $"<style=\"cIsUtility>{((TeleporterInteraction.instance is not null ? TeleporterInteraction.instance.shrineBonusStacks : "Not Applicable"))}</style>"; });
            StatsDisplayClass.statDictionary.Add("experience", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.experience)}</style>"; });
            StatsDisplayClass.statDictionary.Add("level", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.level)}</style>"; });
            StatsDisplayClass.statDictionary.Add("maxHealth", cachedUserBody => { return $"<style=\"cIsHealth>{(cachedUserBody.maxHealth)}</style>"; });
            StatsDisplayClass.statDictionary.Add("maxBarrier", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.maxBarrier)}</style>"; });
            StatsDisplayClass.statDictionary.Add("barrierDecayRate", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.barrierDecayRate)}</style>"; });
            StatsDisplayClass.statDictionary.Add("maxShield", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.maxShield)}</style>"; });
            StatsDisplayClass.statDictionary.Add("acceleration", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.acceleration)}</style>"; });
            StatsDisplayClass.statDictionary.Add("jumpPower", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.jumpPower)}</style>"; });
            StatsDisplayClass.statDictionary.Add("maxJumpHeight", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.maxJumpHeight)}</style>"; });
            StatsDisplayClass.statDictionary.Add("damage", cachedUserBody => { return $"<style=\"cIsDamage>{(cachedUserBody.damage)}</style>"; });
            StatsDisplayClass.statDictionary.Add("critMultiplier", cachedUserBody => { return $"<style=\"cIsDamage>{(cachedUserBody.critMultiplier)}</style>"; });
            StatsDisplayClass.statDictionary.Add("bleedChance", cachedUserBody => { return $"<style=\"cIsDamage>{(cachedUserBody.bleedChance)}</style>"; });
            StatsDisplayClass.statDictionary.Add("visionDistance", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.visionDistance)}</style>"; });
            StatsDisplayClass.statDictionary.Add("critHeal", cachedUserBody => { return $"<style=\"cIsHealing>{(cachedUserBody.critHeal)}</style>"; });
            StatsDisplayClass.statDictionary.Add("cursePenalty", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.cursePenalty)}</style>"; });
            StatsDisplayClass.statDictionary.Add("hasOneShotProtection", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.hasOneShotProtection)}</style>"; });
            StatsDisplayClass.statDictionary.Add("isGlass", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.isGlass)}</style>"; });
            StatsDisplayClass.statDictionary.Add("canPerformBackstab", cachedUserBody => { return $"<style=\"cIsDamage>{(cachedUserBody.canPerformBackstab)}</style>"; });
            StatsDisplayClass.statDictionary.Add("canReceiveBackstab", cachedUserBody => { return $"<style=\"cIsDamage>{(cachedUserBody.canReceiveBackstab)}</style>"; });
            StatsDisplayClass.statDictionary.Add("healthPercentage", cachedUserBody => { return $"<style=\"cIsHealth>{(cachedUserBody.healthComponent.combinedHealthFraction * 100f)}</style>"; });
            StatsDisplayClass.statDictionary.Add("goldPortal", cachedUserBody => { return $"<style=\"cIsUtility>{(TeleporterInteraction.instance is not null ? TeleporterInteraction.instance.shouldAttemptToSpawnGoldshoresPortal.ToString() : "Not Applicable")}</style>"; });
            StatsDisplayClass.statDictionary.Add("msPortal", cachedUserBody => { return $"<style=\"cIsUtility>{(TeleporterInteraction.instance is not null ? TeleporterInteraction.instance.shouldAttemptToSpawnMSPortal.ToString() : "Not Applicable")}</style>"; });
            StatsDisplayClass.statDictionary.Add("shopPortal", cachedUserBody => { return $"<style=\"cIsUtility>{(TeleporterInteraction.instance is not null ? TeleporterInteraction.instance.shouldAttemptToSpawnShopPortal.ToString() : "Not Applicable")}</style>"; });
            StatsDisplayClass.statDictionary.Add("dps", cachedUserBody => {return $"<style=\"cIsDamage>{BasePlugin.instance.dpsMeter.damageDealtSincePeriod / DPSMeter.DPS_MAX_TIME}</style>"; });
            StatsDisplayClass.statDictionary.Add("currentCombatDamage", cachedUserBody => { return $"<style=\"cIsDamage>{BasePlugin.instance.dpsMeter.currentCombatDamage}</style>"; });
            StatsDisplayClass.statDictionary.Add("remainingComboDuration", cachedUserBody => { return $"<style=\"cIsUtility>{(int)BasePlugin.instance.dpsMeter.timer + 1}</style>"; });
            StatsDisplayClass.statDictionary.Add("maxCombo", cachedUserBody => { return $"<style=\"cIsDamage>{BasePlugin.instance.dpsMeter.maxCombo}</style>"; });

        }
    }
}
