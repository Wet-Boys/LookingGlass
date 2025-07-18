using LookingGlass.LookingGlassLanguage;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using System.Xml.Linq;
using UnityEngine;

namespace LookingGlass.BuffDescriptions
{
    internal static class BuffDefinitions
    {
        internal static void SetupEnglishDefs()
        {
            string utilityString = "<style=\"cIsUtility>";
            string damageString = "<style=\"cIsDamage>";
            string healingString = "<style=\"cIsHealing>";
            string healthString = "<style=\"cIsHealth>";
            string voidString = "<style=\"cIsVoid>";
            string shrineString = "<style=\"cShrine>";
            string styleString = "</style>";

            //Stop using "health" for "healing" related things

            //foreach (var item in Language.languagesByName.Keys)
            //{
            //    Log.Debug($"Language:   {item}");
            //}
            Language en = Language.languagesByName["en"];
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.AffixRed.name}", $"Blazing");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.AffixRed.name}", $"Leave a fire trail that hurts enemies, and apply a 50% total damage {damageString}burn{styleString} on hit.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.AffixHaunted.name}", $"Celestine");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.AffixHaunted.name}", $"{utilityString}Cloak{styleString} nearby allies, and apply an 80% {utilityString}slow{styleString} on hit. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.AffixWhite.name}", $"Glacial");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.AffixWhite.name}", $"Leave an ice explosion on death, and apply an 80% {utilityString}slow{styleString} on hit. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.AffixPoison.name}", $"Malachite");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.AffixPoison.name}", $"Shoot occassional urchins and apply {healthString}healing disabled{styleString} on hit. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.EliteEarth.name}", $"Mending");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.EliteEarth.name}", $"{healingString}Heal{styleString} nearby non-mending allies. Produce a {healingString}healing core{styleString} on death, which detonates and heals all nearby allies. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.AffixBlue.name}", $"Overloading");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.AffixBlue.name}", $"Attacks explode after a delay. 50% of {healingString}health{styleString} is replaced by {utilityString}shield{styleString}.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.AffixLunar.name}", $"Perfected");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.AffixLunar.name}", $"{utilityString}Cripple{styleString} on hit. Occasionally fire 5 bomb projectiles at enemies. Gain 35% increased {utilityString}movement speed{styleString}, and gain 25% {healthString}max health{styleString}. All health will be replaced with {utilityString}shields{styleString}.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.EliteVoid.name}", $"Voidtouched");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.EliteVoid.name}", $"{damageString}Collapse{styleString} on hit and {utilityString}block one hit{styleString} every 15 seconds. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.ArmorBoost.name}", $"Armor Boost");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.ArmorBoost.name}", $"Gain {utilityString}+200 armor.{styleString} ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.AttackSpeedOnCrit.name}", $"Attack Speed On Crit");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.AttackSpeedOnCrit.name}", $"Gain {utilityString}+12% attack speed{styleString} per stack.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.BanditSkull.name}", $"Bandit Skull");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.BanditSkull.name}", $"Desperado gains {damageString}10% damage{styleString} per stack.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.AffixHauntedRecipient.name}", $"Celestine Cloak");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.AffixHauntedRecipient.name}", $"{utilityString}Disappear{styleString}. Enemies cannot target invisible allies.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Cloak.name}", $"Cloak");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Cloak.name}", $"{utilityString}Disappear{styleString}. Enemies cannot target invisible allies.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.CloakSpeed.name}", $"Cloak Speed");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.CloakSpeed.name}", $"Gain {utilityString}+40% movement speed{styleString} while invisible. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.ImmuneToDebuffReady.name}", $"Debuff Immunity");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.ImmuneToDebuffReady.name}", $"Will {utilityString}prevent{styleString} the next debuff.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.ElementalRingsReady.name}", $"Elemental Rings Ready");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.ElementalRingsReady.name}", $"Elemental bands can be activated.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.ElephantArmorBoost.name}", $"Elephant Armor Boost");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.ElephantArmorBoost.name}", $"Gain {shrineString}+500 armor{styleString}, negating most attacks. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Energized.name}", $"Energized");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Energized.name}", $"Gain {utilityString}+70% attack speed{styleString}.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.FullCrit.name}", $"Full Crit");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.FullCrit.name}", $"Gain {damageString}100% critical strike{styleString} chance. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.HiddenInvincibility.name}", $"Hidden Invincibility");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.HiddenInvincibility.name}", $"Become immune to all attacks.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Immune.name}", $"Immune");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Immune.name}", $"Become immune to all attacks.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.KillMoveSpeed.name}", $"Kill Move Speed");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.KillMoveSpeed.name}", $"Gain {utilityString}+25% movement speed{styleString}. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.LifeSteal.name}", $"Life Steal");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.LifeSteal.name}", $"{healingString}Heal{styleString} for 20% of {damageString}damage{styleString} dealt.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.MedkitHeal.name}", $"Medkit Heal");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.MedkitHeal.name}", $"Gain {healingString}health{styleString} after a 2-second delay. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.NoCooldowns.name}", $"No Cooldowns");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.NoCooldowns.name}", $"Ability cooldowns reduced to 0.5s. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.OutOfCombatArmorBuff.name}", $"Opal Armor");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.OutOfCombatArmorBuff.name}", $"Gain {shrineString}+100 armor{styleString}. Removed on hit.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.PowerBuff.name}", $"Power Buff");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.PowerBuff.name}", $"Gain {damageString}+50% damage{styleString}. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.CrocoRegen.name}", $"Regenerative");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.CrocoRegen.name}", $"Gain {healingString}health regeneration{styleString} equal to 5% of your {healingString}maximum health{styleString}. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.LaserTurbineKillCharge.name}", $"Resonance Disc Kill Charge");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.LaserTurbineKillCharge.name}", $"Gain a stack per kill that lasts for 7 seconds. At 4 stacks, the Resonance Disc fires, resetting all stacks. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.BearVoidReady.name}", $"Safer Spaces Ready");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.BearVoidReady.name}", $"Negates the next source of damage, then goes on cooldown. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.PrimarySkillShurikenBuff.name}", $"Shuriken");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.PrimarySkillShurikenBuff.name}", $"Consumes a charge to launch a shuriken upon primary skill activation. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.ElementalRingVoidReady.name}", $"Singularity Band Ready");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.ElementalRingVoidReady.name}", $"Singularity Band can be activated.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.SmallArmorBoost.name}", $"Small Armor Boost");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.SmallArmorBoost.name}", $"Gain {utilityString}+100 armor{styleString}. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.TeamWarCry.name}", $"Team War Cry");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.TeamWarCry.name}", $"Gain {utilityString}+50% movement speed{styleString} and {utilityString}+100% attack speed{styleString}. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.TeslaField.name}", $"Tesla Field");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.TeslaField.name}", $"Shock nearby enemies for {damageString}200% damage{styleString} every 0.5s.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.TonicBuff.name}", $"Tonic Buff");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.TonicBuff.name}", $"Gain the following boosts:\n{healingString}+150% max health{styleString}\n{healingString}+400% health regeneration{styleString}\n{utilityString}+170% attack speed{styleString}\n{utilityString}+130% movement speed{styleString}\n{utilityString}+20 armor{styleString}\n{damageString}+200% base damage{styleString}");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.VoidSurvivorCorruptMode.name}", $"Void Survivor Corruption ");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.VoidSurvivorCorruptMode.name}", $"Corrupted, gain different skills.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.WarCryBuff.name}", $"War Cry Buff");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.WarCryBuff.name}", $"Gain {utilityString}+50% movement speed{styleString} and {utilityString}+100% attack speed{styleString}. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Warbanner.name}", $"Warbanner");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Warbanner.name}", $"Gain {utilityString}+30% attack{styleString} and {utilityString}movement speed{styleString}. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.MushroomVoidActive.name}", $"Weeping Fungus Regeneration");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.MushroomVoidActive.name}", $"{healingString}Heal{styleString} while sprinting. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.WhipBoost.name}", $"Whip Boost");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.WhipBoost.name}", $"Gain {utilityString}+30% movement speed{styleString} per Red Whip. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.ImmuneToDebuffCooldown.name}", $"Debuff Immunity Cooldown");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.ImmuneToDebuffCooldown.name}", $"Debuff Immunity is on cooldown");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.ElementalRingsCooldown.name}", $"Elemental Rings Cooldown");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.ElementalRingsCooldown.name}", $"Elemental Bands are on cooldown");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.BearVoidCooldown.name}", $"Safer Spaces Cooldown");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.BearVoidCooldown.name}", $"Damage negation is on cooldown.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.ElementalRingVoidCooldown.name}", $"Singularity Band Cooldown");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.ElementalRingVoidCooldown.name}", $"Singularity Band is on cooldown");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Slow50.name}", $"50% Slow");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Slow50.name}", $"Reduces {utilityString}movement speed{styleString} by 50% ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Slow60.name}", $"60% Slow");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Slow60.name}", $"Reduces {utilityString}movement speed{styleString} by 60% ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Slow80.name}", $"80% Slow");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Slow80.name}", $"Reduces {utilityString}movement speed{styleString} by 80% ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.BeetleJuice.name}", $"Beetle Juice");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.BeetleJuice.name}", $"Reduce {damageString}movement speed{styleString}, {healingString}character damage{styleString}, and {damageString}attack speed{styleString} by 5%. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Bleeding.name}", $"Bleed");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Bleeding.name}", $"Deals {damageString}240% damage{styleString} over time");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Blight.name}", $"Blight");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Blight.name}", $"Deals {damageString}20% base damage stat{styleString} per tick");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.OnFire.name}", $"Burn");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.OnFire.name}", $"Applies a percent of {damageString}damage{styleString} over time, and {healthString}disables health regeneration.{styleString} ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.Fracture.name}", $"Collapse");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.Fracture.name}", $"3 seconds after the first stack is applied, deals 400% {damageString}damage{styleString} per stack and removes all stacks. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Cripple.name}", $"Cripple");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Cripple.name}", $"Reduces {utilityString}armor{styleString} by 20. Reduces {utilityString}movement speed{styleString} by 50%. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.DeathMark.name}", $"Death Mark");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.DeathMark.name}", $"Increases {damageString}damage taken{styleString} from {healthString}all sources{styleString} by 50% for 7 (+7 per stack) seconds. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Entangle.name}", $"Entangle");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Entangle.name}", $"Reduce {utilityString}movement speed{styleString} to 0. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.MercExpose.name}", $"Expose");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.MercExpose.name}", $"Will take {damageString}350% damage{styleString} from next attack.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Fruiting.name}", $"Fruiting");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Fruiting.name}", $"Spawn 2-8 {healingString}healing{styleString} fruits on death. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.HealingDisabled.name}", $"Healing Disabled");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.HealingDisabled.name}", $"Disables all healing, including base health regeneration and item regeneration. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.SuperBleed.name}", $"Hemorrhage");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.SuperBleed.name}", $"{damageString}2000% base damage{styleString} over 15s. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.LunarSecondaryRoot.name}", $"Lunar Root");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.LunarSecondaryRoot.name}", $"Reduce {utilityString}movement speed{styleString} to 0 for 3 seconds per stack. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Nullified.name}", $"Nullified ");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Nullified.name}", $"Reduce {utilityString}movement speed{styleString} to 0.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.NullifyStack.name}", $"Nullify Stack");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.NullifyStack.name}", $"Upon getting 3 stacks, reset all stacks and apply the Nullified debuff. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Overheat.name}", $"Overheat");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Overheat.name}", $"Increases the duration of burn damage from Grandparent's sun attack. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.PermanentDebuff.name}", $"Permanent Armor Reduction");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.PermanentDebuff.name}", $"Reduces {utilityString}armor{styleString} by 2 per stack for the remainder of the stage, or until killed. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.PermanentCurse.name}", $"Permanent Curse");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.PermanentCurse.name}", $"Maximum health reduced");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Poisoned.name}", $"Poisoned");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Poisoned.name}", $"Deals 1% of the victim's {damageString}maximum health{styleString} per second");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.PulverizeBuildup.name}", $"Pulverize Buildup");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.PulverizeBuildup.name}", $"Upon getting 5 stacks, reset all stacks and apply the Pulverized debuff. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Pulverized.name}", $"Pulverized");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Pulverized.name}", $"Reduces {utilityString}armor{styleString} by 60.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.LunarDetonationCharge.name}", $"Ruin");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.LunarDetonationCharge.name}", $"Consumes Ruin stacks to deal 300% {damageString}damage{styleString} plus 120% {damageString}damage{styleString} per Ruin stack ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{DLC1Content.Buffs.StrongerBurn.name}", $"Stronger Burn");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{DLC1Content.Buffs.StrongerBurn.name}", $"Stronger variant of the Ignite effect. Increases {damageString}damage{styleString} taken from burning. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.ClayGoo.name}", $"Tar");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.ClayGoo.name}", $"Reduces {utilityString}movement speed{styleString} by 50%. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.VoidFogMild.name}", $"Void Fog");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.VoidFogMild.name}", $"Deals a small amount of {damageString}damage{styleString} multiple times per second, {healthString}increasing with each tick{styleString}. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.VoidFogStrong.name}", $"Void Fog");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.VoidFogStrong.name}", $"Deals a medium amount of {damageString}damage{styleString} multiple times per second, {healthString}increasing with each tick{styleString}.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Weak.name}", $"Weak");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Weak.name}", $"Reduces {utilityString}armor{styleString} by 30%, reduces {utilityString}movement speed and damage{styleString} by 40%. ");

            //TODO : STYLE
            RegisterBuff(en, DLC1Content.Buffs.VoidRaidCrabWardWipeFog, "Voidling Fog", $"Deals a small amount of {damageString}damage{styleString} multiple times per second, {healthString}increasing with each tick{styleString}. Until you return to Voidling. ");

            #region DLC2
            //Survivors
            RegisterBuff(en, DLC2Content.Buffs.ChakraBuff, "Tranquility", $"Improves Seekers skills in various ways.");
            RegisterBuff(en, DLC2Content.Buffs.RevitalizeBuff, "Saving Grace", $"Increase your stats by 7%. You will be revived by Seeker upon death.");
            RegisterBuff(en, DLC2Content.Buffs.SeekerRevivedOnce, "Grace Consumed", $"You were revived by Seeker and cannot be revived by her again until the next stage.");

            RegisterBuff(en, DLC2Content.Buffs.Boosted, "Yes, CHEF!", $"CHEFs next attack will be upgraded.");
            //The cooking buffs really dont matter

            RegisterBuff(en, DLC2Content.Buffs.EnergizedCore, "Meridians Will", "$Lunar Spike stat changes will be twice as effective.");
            RegisterBuff(en, DLC2Content.Buffs.lunarruin, "Lunar Ruin", "$Increase damage taken by 10% per buff. Decrease healing by 20%");
            //
            // Other
            RegisterBuff(en, DLC2Content.Buffs.DisableAllSkills, "All Skills Disabled", $"Your skills and equipment are disabled.");
            RegisterBuff(en, DLC2Content.Buffs.ExtraLifeBuff, "Extra Life", $"Revive once per stage");
            RegisterBuff(en, DLC2Content.Buffs.SoulCost, "Soul Cost", $"Reduce health by 10%");
            //    
            ////Items
            RegisterBuff(en, DLC2Content.Buffs.AttackSpeedPerNearbyAllyOrEnemyBuff, "Bolstering Lantern", $"Gain a attack speed boost because a ally is nearby");
            RegisterBuff(en, DLC2Content.Buffs.DelayedDamageBuff, "Warped Echo", $"Warped Echo is ready to split damage.");
            RegisterBuff(en, DLC2Content.Buffs.DelayedDamageDebuff, "Damage Echo", $"Instances of Echoed split damage.");
            RegisterBuff(en, DLC2Content.Buffs.ElusiveAntlersBuff, "Elusive Antler", $"Temporarily increase speed by 12% for 12s. Refreshes stack duration when reapplied");
            RegisterBuff(en, DLC2Content.Buffs.IncreaseDamageBuff, "Chronic Expansion", $"Increase your damage.");

            RegisterBuff(en, DLC2Content.Buffs.KnockBackAvailable, "Breaching Fin Ready", $"Breaching Fin is Ready");
            RegisterBuff(en, DLC2Content.Buffs.KnockBackUnavailable, "Breaching Fin Cooldown", $"Breaching Fin is on cooldown for 15s");
            RegisterBuff(en, DLC2Content.Buffs.ExtraStatsOnLevelUpBuff, "Prayer Beads XP", $"Stored Experience that will grant the stats of 0.2 (+0.05 per Prayer Beads) levels upon removal of a Prayer Bead");
            RegisterBuff(en, DLC2Content.Buffs.IncreasePrimaryDamageBuff, "Luminous Charge", $"Stored Lightning for your next primary attack. Can only be used at 3 stacks or higher.");
            RegisterBuff(en, DLC2Content.Buffs.TeleportOnLowHealthActive, "Unstable", $"Your Unstable Transmitter has activated a dimensional Aura. Enemies that touch it and die will increase duration by 1s.");

            RegisterBuff(en, DLC2Content.Buffs.BoostAllStatsBuff, "Growth Nectar", $"Increase all stats by 4%");
            RegisterBuff(en, DLC2Content.Buffs.ExtraBossMissile, "War Missile", $"Stored Missiles dealing 2.5% max Hp damage, for the next boss or Scavenger encounter");

            RegisterBuff(en, DLC2Content.Buffs.FreeUnlocks, "Free Purchase", $"Your next purchase is free");
            //
            ////Equipment
            RegisterBuff(en, DLC2Content.Buffs.EliteAurelionite, "Gilded", $"Fire out a Gilded Spike every 10-15s. Doing damage has a chance to produce extra Gold.");
            RegisterBuff(en, DLC2Content.Buffs.EliteBead, "Twisted", $"Give nearby allies 300 Armor. Unleash a Twisted Spike after nearby allies are hurt 10 times. This ability has a 10s cooldown.");

            RegisterBuff(en, DLC2Content.Buffs.BeadArmor, "Twisted Armor", $"Gain 300 Armor");

            #endregion
            #region DLC3

            #endregion
        }
        public static void RegisterBuff(Language language, BuffDef buff, string name, string description) //coulda used this earlier, but w/e
        {
            LookingGlassLanguageAPI.SetupToken(language, $"NAME_{buff.name}", name);
            LookingGlassLanguageAPI.SetupToken(language, $"DESCRIPTION_{buff.name}", description);
        }
    }
}
