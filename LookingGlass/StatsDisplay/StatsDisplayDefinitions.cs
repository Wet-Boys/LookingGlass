using LookingGlass.DPSMeterStuff;
using RoR2;
using RoR2.Networking;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace LookingGlass.StatsDisplay
{
    internal class StatsDisplayDefinitions
    {
        internal static string floatPrecision;

        // these delegates are called on a seperate thread, so using most of unity api is illegal here
        internal static void SetupDefs()
        {
            string utilityString = StatsDisplayClass.builtInColors.Value ? "<style=\"cIsUtility>" : "";
            string damageString = StatsDisplayClass.builtInColors.Value ? "<style=\"cIsDamage>" : "";
            string healingString = StatsDisplayClass.builtInColors.Value ? "<style=\"cIsHealing>" : "";
            string healthString = StatsDisplayClass.builtInColors.Value ? "<style=\"cIsHealth>" : "";
            string voidString = StatsDisplayClass.builtInColors.Value ? "<style=\"cIsVoid>" : "";
            string styleString = StatsDisplayClass.builtInColors.Value ? "</style>" : "";
            //NumberFormatInfo floatPrecision = new NumberFormatInfo();
            //floatPrecision.NumberDecimalDigits = StatsDisplayClass.floatPrecision.Value;
            floatPrecision = "0." + new string('#', StatsDisplayClass.floatPrecision.Value);
            StatsDisplayClass.statDictionary.Clear();

            #region Damage Related
            StatsDisplayClass.statDictionary.Add("lvl1_damage", cachedUserBody => { return $"{damageString}{(cachedUserBody.baseDamage).ToString(floatPrecision)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("baseDamage", cachedUserBody => { return $"{damageString}{(cachedUserBody.damage).ToString(floatPrecision)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("damage", cachedUserBody => { return $"{damageString}{(cachedUserBody.damage).ToString(floatPrecision)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("attackSpeed", cachedUserBody => { return $"{damageString}{(cachedUserBody.attackSpeed).ToString(floatPrecision)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("attackSpeedPercent", cachedUserBody => { return $"{damageString}{((cachedUserBody.attackSpeed / cachedUserBody.baseAttackSpeed) * 100).ToString(floatPrecision)}%{styleString}"; });

            StatsDisplayClass.statDictionary.Add("crit", cachedUserBody => { return $"{damageString}{(cachedUserBody.crit).ToString(floatPrecision)}%{styleString}"; });
            StatsDisplayClass.statDictionary.Add("critWithLuck", cachedUserBody =>
            {
                float critWithLuck = Utils.CalculateChanceWithLuck(cachedUserBody.crit / 100f, Utils.GetLuckFromCachedUserBody(cachedUserBody)) * 100f;
                return $"{damageString}{critWithLuck.ToString(floatPrecision)}%{styleString}";
            });
            StatsDisplayClass.statDictionary.Add("critMultiplier", cachedUserBody => { return $"{damageString}×{(cachedUserBody.critMultiplier).ToString(floatPrecision)}{styleString}"; });

            StatsDisplayClass.statDictionary.Add("bleedChance", cachedUserBody => { return $"{damageString}{(cachedUserBody.bleedChance).ToString(floatPrecision)}%{styleString}"; });
            StatsDisplayClass.statDictionary.Add("bleedChanceWithLuck", cachedUserBody =>
            {
                float bleedChanceWithLuck = Utils.CalculateChanceWithLuck(cachedUserBody.bleedChance / 100f, Utils.GetLuckFromCachedUserBody(cachedUserBody)) * 100f;
                return $"{damageString}{bleedChanceWithLuck.ToString(floatPrecision)}%{styleString}";
            });

            StatsDisplayClass.statDictionary.Add("bleedChanceWithSpleen", cachedUserBody =>
            {
                float bleedChanceWithLuck = (Utils.CalculateChanceWithLuck(cachedUserBody.crit / 100f, Utils.GetLuckFromCachedUserBody(cachedUserBody)) * Utils.CalculateChanceWithLuck(cachedUserBody.bleedChance / 100f, Utils.GetLuckFromCachedUserBody(cachedUserBody))) * 100f;
                return $"{damageString}{bleedChanceWithLuck.ToString(floatPrecision)}%{styleString}";
            });

            #endregion
            #region Health Related

            StatsDisplayClass.statDictionary.Add("armor", cachedUserBody => { return $"{healingString}{(cachedUserBody.armor).ToString(floatPrecision)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("armorDamageReduction", cachedUserBody => { return $"{healingString}{(100 - (100 * (100 / (100 + cachedUserBody.armor)))).ToString(floatPrecision)}%{styleString}"; });
            StatsDisplayClass.statDictionary.Add("lvl1_regen", cachedUserBody => { return $"{healingString}{(cachedUserBody.baseRegen).ToString(floatPrecision)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("regen", cachedUserBody => { return $"{healingString}{(cachedUserBody.regen).ToString(floatPrecision)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("regenHp", cachedUserBody => { return $"{healingString}{(cachedUserBody.regen).ToString(floatPrecision)} hp/s{styleString}"; });

            StatsDisplayClass.statDictionary.Add("lvl1_maxHealth", cachedUserBody => { return $"{healingString}{(cachedUserBody.baseMaxHealth)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("maxHealth", cachedUserBody => { return $"{healingString}{(cachedUserBody.maxHealth)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("maxShield", cachedUserBody => { return $"{healingString}{(cachedUserBody.maxShield)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("maxBarrier", cachedUserBody => { return $"{healingString}{(cachedUserBody.maxBarrier)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("barrierDecayRate", cachedUserBody => { return $"{healingString}{(cachedUserBody.barrierDecayRate).ToString(floatPrecision)}{styleString}"; });

            //Current Health / ArmorReduc
            StatsDisplayClass.statDictionary.Add("effectiveHealth", cachedUserBody => {
                if (!cachedUserBody.healthComponent)
                {
                    return $"{healingString}N/A{styleString}";
                }
                return $"{healingString}{((cachedUserBody.healthComponent.combinedHealth) / (100f / (100f + cachedUserBody.armor))).ToString(floatPrecision)}{styleString}";
            });
            StatsDisplayClass.statDictionary.Add("effectiveMaxHealth", cachedUserBody => {
                if (!cachedUserBody.healthComponent)
                {
                    return $"{healingString}N/A{styleString}";
                }
                return $"{healingString}{((cachedUserBody.healthComponent.fullCombinedHealth) / (100f / (100f + cachedUserBody.armor))).ToString(floatPrecision)}{styleString}";
            });

            StatsDisplayClass.statDictionary.Add("healthPercentage", cachedUserBody => {
                if (!cachedUserBody.healthComponent)
                {
                    return $"{healingString}N/A{styleString}";
                }
                return $"{healingString}{(cachedUserBody.healthComponent.combinedHealthFraction * 100f).ToString(floatPrecision)}{styleString}";
            });

            StatsDisplayClass.statDictionary.Add("critHeal", cachedUserBody => { return $"{healingString}{(cachedUserBody.critHeal).ToString(floatPrecision)}{styleString}"; });
            //Unused stat so it might be confusing to include ^ 

            StatsDisplayClass.statDictionary.Add("hasOneShotProtection", cachedUserBody => {
                if (!cachedUserBody.healthComponent)
                {
                    return $"{healingString}N/A{styleString}";
                }
                //Does not account for Barrier being able to put you back into OSP range.$
                //But that is very minimal to be frank
                return $"{healingString}{(cachedUserBody.oneShotProtectionFraction * cachedUserBody.healthComponent.fullCombinedHealth - cachedUserBody.healthComponent.missingCombinedHealth) > 0}{styleString}";
            });

            //Curse Penalty is a technical stat so not really usefull v
            StatsDisplayClass.statDictionary.Add("cursePenalty", cachedUserBody => { return $"{utilityString}{(cachedUserBody.cursePenalty).ToString(floatPrecision)}{styleString}"; });
            //Curse HP Reduction as %
            StatsDisplayClass.statDictionary.Add("curseHealthReduction", cachedUserBody =>
            {
                return $"{healthString}{((1f - 1f / cachedUserBody.cursePenalty) * 100f).ToString(floatPrecision)}%{styleString}";
            });
            #endregion

            #region Movement Related

            StatsDisplayClass.statDictionary.Add("speed", cachedUserBody => { return $"{utilityString}{(cachedUserBody.moveSpeed).ToString(floatPrecision)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("speedPercent", cachedUserBody => { return $"{utilityString}{((cachedUserBody.moveSpeed / cachedUserBody.baseMoveSpeed) * 100).ToString(floatPrecision)}%{styleString}"; });

            StatsDisplayClass.statDictionary.Add("acceleration", cachedUserBody => { return $"{utilityString}{(cachedUserBody.acceleration).ToString(floatPrecision)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("velocity", cachedUserBody =>
            {
                string velocity =
                    cachedUserBody.characterMotor ? cachedUserBody.characterMotor.velocity.magnitude.ToString(floatPrecision) :
                    // rigidbody.velocity is illegal in unity debug build
                    cachedUserBody.rigidbody ? cachedUserBody.rigidbody.velocity.magnitude.ToString(floatPrecision) :
                    "N/A";
                return $"{utilityString}{velocity}{styleString}";
            });

            StatsDisplayClass.statDictionary.Add("jumpPower", cachedUserBody => { return $"{utilityString}{(cachedUserBody.jumpPower).ToString(floatPrecision)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("maxJumpHeight", cachedUserBody => { return $"{utilityString}{(cachedUserBody.maxJumpHeight).ToString(floatPrecision)}{styleString}"; });

            StatsDisplayClass.statDictionary.Add("maxJumps", cachedUserBody => { return $"{utilityString}{(cachedUserBody.maxJumpCount)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("availableJumps", cachedUserBody => {
                if (!cachedUserBody.characterMotor)
                {
                    return $"{utilityString}N/A{styleString}";
                }
                return $"{utilityString}{(cachedUserBody.maxJumpCount - cachedUserBody.characterMotor.jumpCount)}{styleString}";
            });

            #endregion

            #region Utility Related
            StatsDisplayClass.statDictionary.Add("luck", cachedUserBody => {
                return $"{utilityString}{Utils.GetLuckFromCachedUserBody(cachedUserBody).ToString(floatPrecision)}{styleString}";
            });

            StatsDisplayClass.statDictionary.Add("teddyBearBlockChance", cachedUserBody => {
                int stackCount = cachedUserBody.inventory.GetItemCount(RoR2Content.Items.Bear);
                return $"{utilityString}{(((0.15f * stackCount) / ((0.15f * stackCount) + 1)) * 100f).ToString(floatPrecision)}%{styleString}";
            });
            StatsDisplayClass.statDictionary.Add("saferSpacesCD", cachedUserBody => {
                int stackCount = cachedUserBody.inventory.GetItemCount(DLC1Content.Items.BearVoid);
                if (stackCount == 0)
                {
                    return $"{utilityString}N/A{styleString}";
                }
                return $"{utilityString}{(15 * Mathf.Pow(.9f, stackCount)).ToString(floatPrecision)}s{styleString}";
            });

            //
            StatsDisplayClass.statDictionary.Add("instaKillChance", cachedUserBody => {
                int stackCount = cachedUserBody.inventory.GetItemCount(DLC1Content.Items.CritGlassesVoid);
                float instakillChance = Utils.CalculateChanceWithLuck(.005f * stackCount, Utils.GetLuckFromCachedUserBody(cachedUserBody)) * 100f;
                return $"{damageString}{instakillChance.ToString(floatPrecision)}%{styleString}";
            });
            #endregion




            #region Portal / Teleporter Stuff
            StatsDisplayClass.statDictionary.Add("mountainShrines", cachedUserBody => { return $"{utilityString}{((TeleporterInteraction.instance is not null ? TeleporterInteraction.instance.shrineBonusStacks : "N/A"))}{styleString}"; });

            StatsDisplayClass.statDictionary.Add("shopPortal", cachedUserBody => { return $"{utilityString}{(TeleporterInteraction.instance ? BasePlugin.instance.portalTracking.shopPortal.ToString() : "N/A")}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("goldPortal", cachedUserBody => { return $"{damageString}{(TeleporterInteraction.instance ? BasePlugin.instance.portalTracking.goldPortal.ToString() : "N/A")}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("msPortal", cachedUserBody => { return $"{utilityString}{(TeleporterInteraction.instance ? BasePlugin.instance.portalTracking.msPortal.ToString() : "N/A")}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("voidPortal", cachedUserBody => { return $"{voidString}{(TeleporterInteraction.instance ? BasePlugin.instance.portalTracking._voidPortal.ToString() : "N/A")}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("greenPortal", cachedUserBody => { return $"{healingString}{(TeleporterInteraction.instance ? BasePlugin.instance.portalTracking._greenPortal.ToString() : "N/A")}{styleString}"; });

            StatsDisplayClass.statDictionary.Add("portals", cachedUserBody => BasePlugin.instance.portalTracking.ReturnAllAvailablePortals());
 
            #endregion

            #region DPS, Combo, Kills

            //SERVER ONLY//StatsDisplayClass.statDictionary.Add("killCountOld", cachedUserBody => { return $"{healthString}{(cachedUserBody.killCountServer)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("killCount", cachedUserBody => { return $"{healthString}{(BasePlugin.instance.dpsMeter.killsThisStage)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("killCountRun", cachedUserBody => { return $"{healthString}{(BasePlugin.instance.dpsMeter.killsThisRun)}{styleString}"; });


            StatsDisplayClass.statDictionary.Add("dps", cachedUserBody => { return $"{damageString}{(BasePlugin.instance.dpsMeter.damageDealtSincePeriod / DPSMeter.DPS_MAX_TIME).ToString("0.#")}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("percentDps", cachedUserBody
             => $"{damageString}{(Mathf.RoundToInt(BasePlugin.instance.dpsMeter.damageDealtSincePeriod * 100f / cachedUserBody.damage / DPSMeter.DPS_MAX_TIME)).ToString()}%{styleString}");
           
            //RoundToInt because (int) often drops a % and Ceil sometimes Adds one.
            //It's really just floating point errors

            StatsDisplayClass.statDictionary.Add("combo", cachedUserBody => { return $"{damageString}{(int)BasePlugin.instance.dpsMeter.currentCombatDamage}{styleString}"; });
            //Needs to be kept for older version support v
            StatsDisplayClass.statDictionary.Add("currentCombatDamage", cachedUserBody => { return $"{damageString}{(int)BasePlugin.instance.dpsMeter.currentCombatDamage}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("maxCombo", cachedUserBody => { return $"{damageString}{(int)BasePlugin.instance.dpsMeter.maxCombo}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("maxComboThisRun", cachedUserBody => { return $"{damageString}{(int)BasePlugin.instance.dpsMeter.maxRunCombo}{styleString}"; });

            StatsDisplayClass.statDictionary.Add("killCombo", cachedUserBody => { return $"{damageString}{BasePlugin.instance.dpsMeter.currentComboKills}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("currentCombatKills", cachedUserBody => { return $"{damageString}{BasePlugin.instance.dpsMeter.currentComboKills}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("maxKillCombo", cachedUserBody => { return $"{damageString}{BasePlugin.instance.dpsMeter.maxKillCombo}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("maxKillComboThisRun", cachedUserBody => { return $"{damageString}{BasePlugin.instance.dpsMeter.maxRunKillCombo}{styleString}"; });
            
            StatsDisplayClass.statDictionary.Add("remainingComboDuration", cachedUserBody => { return $"{utilityString}{(int)BasePlugin.instance.dpsMeter.timer + 1}{styleString}"; });
            #endregion
 
            StatsDisplayClass.statDictionary.Add("experience", cachedUserBody => { return $"{utilityString}{(cachedUserBody.experience).ToString(floatPrecision)}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("level", cachedUserBody => { return $"{utilityString}{(cachedUserBody.level)}{styleString}"; });
  
            StatsDisplayClass.statDictionary.Add("difficultyCoefficient", cachedUserBody => { return $"{utilityString}{(Run.instance ? Run.instance.difficultyCoefficient.ToString(floatPrecision) : "N/A")}{styleString}"; });
            StatsDisplayClass.statDictionary.Add("stage", cachedUserBody => { return $"{utilityString}{Language.GetString(Stage.instance ? Stage.instance.sceneDef.nameToken : "N/A")}{styleString}"; });
            
            //Idk if this needs like saftey checks
            StatsDisplayClass.statDictionary.Add("ping", cachedUserBody => { return $"{utilityString}{RttManager.GetConnectionRTTInMilliseconds(NetworkManagerSystem.singleton.client.connection)}{styleString}"; });


            //Are these really needed, they dont do anything v 
            //StatsDisplayClass.statDictionary.Add("isGlass", cachedUserBody => { return $"{utilityString}{(cachedUserBody.isGlass)}{styleString}"; });
            //StatsDisplayClass.statDictionary.Add("canPerformBackstab", cachedUserBody => { return $"{damageString}{(cachedUserBody.canPerformBackstab)}{styleString}"; });
            //StatsDisplayClass.statDictionary.Add("canReceiveBackstab", cachedUserBody => { return $"{damageString}{(cachedUserBody.canReceiveBackstab)}{styleString}"; });
            //StatsDisplayClass.statDictionary.Add("visionDistance", cachedUserBody => { return $"{utilityString}{(cachedUserBody.visionDistance).ToString(floatPrecision)}{styleString}"; });

        }
    }
}
