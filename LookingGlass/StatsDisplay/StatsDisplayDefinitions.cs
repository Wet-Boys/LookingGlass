using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace LookingGlass.StatsDisplay
{
    internal class StatsDisplayDefinitions
    {
        /*
                         StringBuilder sb = new StringBuilder();
                sb.Append($"Stats\n" +
                    $"Luck: {cachedUserBody.inventory.GetItemCount(RoR2Content.Items.Clover) - cachedUserBody.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck)}\n" +
                    $"Base Damage: {cachedUserBody.baseDamage}\n" +
                    $"Crit Chance: {cachedUserBody.crit}\n" +
                    $"Attack Speed: {cachedUserBody.attackSpeed}\n" +
                    $"Armor: {cachedUserBody.armor} | {100 / (100 + cachedUserBody.armor)}\n" +
                    $"Regen: {cachedUserBody.regen}\n" +
                    $"Speed: {cachedUserBody.moveSpeed}\n" +
                    $"Jumps: {cachedUserBody.maxJumpCount - cachedUserBody.characterMotor.jumpCount}/{cachedUserBody.maxJumpCount}\n" +
                    $"Kills: {cachedUserBody.killCountServer}\n" +
                    $"Mountain Shrines: {(TeleporterInteraction.instance is not null ? TeleporterInteraction.instance.shrineBonusStacks : "Not Applicable")}\n");
         */
        internal static void SetupDefs()
        {
            StatsDisplayClass.statDictionary.Add("luck", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.inventory.GetItemCount(RoR2Content.Items.Clover) - cachedUserBody.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck))}</style>"; });
            StatsDisplayClass.statDictionary.Add("baseDamage", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.baseDamage).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("crit", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.crit).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("attackSpeed", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.attackSpeed).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("armor", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.armor).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("armorDamageReduction", cachedUserBody => { return $"<style=\"cIsUtility>{(100 / (100 + cachedUserBody.armor)).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("regen", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.regen).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("speed", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.moveSpeed).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("availableJumps", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.maxJumpCount - cachedUserBody.characterMotor.jumpCount).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("maxJumps", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.maxJumpCount).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("killCount", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.killCountServer).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("mountainShrines", cachedUserBody => { return $"<style=\"cIsUtility>{((TeleporterInteraction.instance is not null ? TeleporterInteraction.instance.shrineBonusStacks.ToString() : "Not Applicable"))}</style>"; });
            StatsDisplayClass.statDictionary.Add("experience", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.experience).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("level", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.level).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("maxHealth", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.maxHealth).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("maxBarrier", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.maxBarrier).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("barrierDecayRate", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.barrierDecayRate).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("maxShield", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.maxShield).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("acceleration", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.acceleration).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("jumpPower", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.jumpPower).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("maxJumpHeight", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.maxJumpHeight).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("damage", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.damage).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("critMultiplier", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.critMultiplier).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("bleedChance", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.bleedChance).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("visionDistance", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.visionDistance).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("critHeal", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.critHeal).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("cursePenalty", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.cursePenalty).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("hasOneShotProtection", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.hasOneShotProtection).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("isGlass", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.isGlass).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("canPerformBackstab", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.canPerformBackstab).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("canReceiveBackstab", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.canReceiveBackstab).ToString()}</style>"; });
            StatsDisplayClass.statDictionary.Add("healthPercentage", cachedUserBody => { return $"<style=\"cIsUtility>{(cachedUserBody.healthComponent.combinedHealthFraction * 100f).ToString()}</style>"; });
            //StatsDisplayClass.statDictionary.Add("goldPortal", cachedUserBody => { return $"<style=\"cIsUtility>{((((TeleporterInteraction.instance is not null ? TeleporterInteraction.instance.shouldAttemptToSpawnGoldshoresPortal.ToString() : "Not Applicable")); });
            //StatsDisplayClass.statDictionary.Add("msPortal", cachedUserBody => { return $"<style=\"cIsUtility>{(((TeleporterInteraction.instance is not null ? TeleporterInteraction.instance.shouldAttemptToSpawnMSPortal.ToString() : "Not Applicable")); });
            //StatsDisplayClass.statDictionary.Add("shopPortal", cachedUserBody => { return $"<style=\"cIsUtility>{(((TeleporterInteraction.instance is not null ? TeleporterInteraction.instance.shouldAttemptToSpawnShopPortal.ToString() : "Not Applicable")); });

        }
    }
}
