using LookingGlass.LookingGlassLanguage;
using RoR2;

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
            //RegisterBuff(en, JunkContent.Buffs.IgnoreFallDamage, "", $"");
            #region Base Game
            ////Elites
            RegisterBuff(en, RoR2Content.Buffs.AffixRed, "Blazing", $"Leave a fire trail that hurts enemies, and apply a 50% total damage {damageString}burn{styleString} on hit.");
            RegisterBuff(en, RoR2Content.Buffs.AffixBlue, "Overloading", $"Attacks leave overloading orbs that explode for 50% TOTAL damage in a 3m radius after a short delay. 50% of {healingString}health{styleString} is replaced by {utilityString}shield{styleString}.");
            RegisterBuff(en, RoR2Content.Buffs.AffixWhite, "Glacial", $"Leave an ice explosion on death, and apply an 80% {utilityString}slow{styleString} on hit.");
            RegisterBuff(en, RoR2Content.Buffs.AffixPoison, "Malachite", $"Shoot occassional urchins and apply {healthString}disable healing for 8s{styleString} on hit.");
            RegisterBuff(en, RoR2Content.Buffs.AffixHaunted, "Celestine", $"{utilityString}Cloak{styleString} nearby allies, and apply an 80% {utilityString}slow{styleString} on hit.");
            RegisterBuff(en, RoR2Content.Buffs.AffixLunar, "Perfected", $"{utilityString}Cripple{styleString} on hit. Occasionally fire 4 bomb projectiles at enemies. Gain 30% increased {utilityString}movement speed{styleString}, and gain 25% {healthString}max health{styleString}. All health will be replaced with {utilityString}shields{styleString}.");

            RegisterBuff(en, RoR2Content.Buffs.HealingDisabled, $"Malachite Poisoning", $"Disables all healing, including health regeneration. ");
            RegisterBuff(en, RoR2Content.Buffs.AffixHauntedRecipient, "Celestine Cloak", $"You are {utilityString}cloaked{styleString} due to a Celestine Ally.");


            //Survivor
            RegisterBuff(en, RoR2Content.Buffs.BanditSkull, "Bandit Skull", $"Desperado gains {damageString}10% damage{styleString} per stack.");

            RegisterBuff(en, RoR2Content.Buffs.ArmorBoost, "Strong Armor Boost", $"Gain {utilityString}+200 armor.{styleString} ");
            RegisterBuff(en, RoR2Content.Buffs.SmallArmorBoost, "Small Armor Boost", $"Gain {utilityString}+100 armor{styleString}. ");

            RegisterBuff(en, RoR2Content.Buffs.CrocoRegen, "Regenerative", $"Gain {healingString}health regeneration{styleString} equal to 5% of your {healingString}maximum health{styleString}. ");

            RegisterBuff(en, RoR2Content.Buffs.Weak, "Weak", $"Reduces {utilityString}armor{styleString} by 30%, reduces {utilityString}movement speed and damage{styleString} by 40%. ");
            RegisterBuff(en, RoR2Content.Buffs.Entangle, "Entangle", $"Disables {utilityString}movement.{styleString}");
            RegisterBuff(en, RoR2Content.Buffs.Fruiting, "Fruiting", $"Spawn 2-8 {healingString}healing{styleString} fruits on death. ");
            RegisterBuff(en, RoR2Content.Buffs.MercExpose, "Expose", $"Will take {damageString}350% damage{styleString} from next attack.");

            ////Items
            //White
            RegisterBuff(en, RoR2Content.Buffs.MedkitHeal, "Medkit Heal", $"Gain {healingString}health{styleString} after a 2-second delay. ");
            RegisterBuff(en, RoR2Content.Buffs.Warbanner, "Warbanner", $"Gain {utilityString}+30% attack{styleString} and {utilityString}movement speed{styleString}. ");

            //Green
            RegisterBuff(en, RoR2Content.Buffs.ElementalRingsReady, "Elemental Bands Ready", $"Elemental bands can be activated.");
            RegisterBuff(en, RoR2Content.Buffs.ElementalRingsCooldown, "Elemental Bands Cooldown", $"Elemental Bands are on cooldown");
            RegisterBuff(en, RoR2Content.Buffs.WhipBoost, "Whip Boost", $"Gain {utilityString}+30% movement speed{styleString} for each Red Whip. ");
            RegisterBuff(en, RoR2Content.Buffs.AttackSpeedOnCrit, "Attack Speed On Crit", $"Gain {utilityString}+12% attack speed{styleString} per stack.");
            RegisterBuff(en, RoR2Content.Buffs.Cloak, "Cloak", $"{utilityString}Disappear{styleString}. Enemies cannot target invisible allies.");
            RegisterBuff(en, RoR2Content.Buffs.CloakSpeed, "Cloak Speed", $"Gain {utilityString}+40% movement speed{styleString} while invisible. ");
            RegisterBuff(en, RoR2Content.Buffs.Energized, "Energized", $"Gain {utilityString}+70% attack speed{styleString}.");
            RegisterBuff(en, RoR2Content.Buffs.DeathMark, "Death Mark", $"Increases {damageString}damage taken{styleString} from {healthString}all sources{styleString} by 50% for 7 (+7 per stack) seconds. ");

            //Red
            RegisterBuff(en, RoR2Content.Buffs.NoCooldowns, "No Cooldowns", $"Ability cooldowns reduced to 0.5s. ");
            RegisterBuff(en, RoR2Content.Buffs.LaserTurbineKillCharge, "Resonance Disc Kill Charge", $"Gain a stack per kill that lasts for 7 seconds. At 4 stacks, the Resonance Disc fires, resetting all stacks. ");
            RegisterBuff(en, RoR2Content.Buffs.TeslaField, "Tesla Field", $"Shock nearby enemies for {damageString}200% damage{styleString} every 0.5s.");
            RegisterBuff(en, RoR2Content.Buffs.Pulverized, "Pulverized", $"Reduces {utilityString}armor{styleString} by 60.");
            RegisterBuff(en, RoR2Content.Buffs.PulverizeBuildup, "Pulverize Buildup", $"Upon getting 5 stacks, reset all stacks and apply the Pulverized debuff. ");

            //Lunar
            RegisterBuff(en, RoR2Content.Buffs.PowerBuff, "Mercurial Rachis", $"Gain {damageString}+50% damage{styleString}. ");
            RegisterBuff(en, RoR2Content.Buffs.LunarSecondaryRoot, "Lunar Root", $"Disables {utilityString}movement{styleString}. ");
            RegisterBuff(en, RoR2Content.Buffs.LunarDetonationCharge, "Ruin", $"Consumes Ruin stacks to deal 300% {damageString}damage{styleString} plus 120% {damageString}damage{styleString} per Ruin stack ");

            ////Equipment
            RegisterBuff(en, RoR2Content.Buffs.ElephantArmorBoost, "Elephant Armor Boost", $"Gain {shrineString}+500 armor{styleString}, negating most attacks. ");
            RegisterBuff(en, RoR2Content.Buffs.FullCrit, "Full Crit", $"Gain {damageString}100% critical strike{styleString} chance. ");
            RegisterBuff(en, RoR2Content.Buffs.LifeSteal, "Life Steal", $"{healingString}Heal{styleString} for 20% of {damageString}damage{styleString} dealt.");
            RegisterBuff(en, RoR2Content.Buffs.TeamWarCry, "Team War Cry", $"Gain {utilityString}+50% movement speed{styleString} and {utilityString}+100% attack speed{styleString}. ");
            RegisterBuff(en, RoR2Content.Buffs.WarCryBuff, "War Cry", $"Gain {utilityString}+50% movement speed{styleString} and {utilityString}+100% attack speed{styleString}. ");

            RegisterBuff(en, RoR2Content.Buffs.TonicBuff, "Spinel Tonic", $"Gain the following boosts:\n{healingString}+150% max health{styleString}\n{healingString}+400% health regeneration{styleString}\n{utilityString}+170% attack speed{styleString}\n{utilityString}+130% movement speed{styleString}\n{utilityString}+20 armor{styleString}\n{damageString}+200% base damage{styleString}");

            //General
            RegisterBuff(en, RoR2Content.Buffs.Immune, "Immune", $"Become immune to all attacks.");
            RegisterBuff(en, RoR2Content.Buffs.HiddenInvincibility, "Hidden Invincibility", $"Become immune to all attacks.");

            RegisterBuff(en, RoR2Content.Buffs.PermanentCurse, "Permanent Curse", $"Maximum health reduced");

            //DoTs
            RegisterBuff(en, RoR2Content.Buffs.Bleeding, "Bleed", $"Deals {damageString}240% damage{styleString} over time. Refreshes duration when applied.");
            RegisterBuff(en, RoR2Content.Buffs.SuperBleed, "Hemorrhage", $"{damageString}2000% base damage{styleString} over 15s. Refreshes duration when applied.");
            RegisterBuff(en, RoR2Content.Buffs.Poisoned, "Poisoned", $"Deals 1% of the victim's {damageString}maximum health{styleString} per second");
            RegisterBuff(en, RoR2Content.Buffs.Blight, "Blight", $"Deals {damageString}20% base damage stat{styleString} per tick");
            RegisterBuff(en, RoR2Content.Buffs.OnFire, "Ignited", $"Applies a percent of {damageString}damage{styleString} over time, and {healthString}disables health regeneration.{styleString} ");
            RegisterBuff(en, DLC1Content.Buffs.StrongerBurn, "Stronger Burn", $"A burn with at least 4x effectiveness of regular Ignite. Health regeneration is disabled. ");



            RegisterBuff(en, JunkContent.Buffs.Slow30, "30% Slow", $"Reduces {utilityString}movement speed{styleString} by 30% ");
            RegisterBuff(en, RoR2Content.Buffs.Slow50, "50% Slow", $"Reduces {utilityString}movement speed{styleString} by 50% ");
            RegisterBuff(en, RoR2Content.Buffs.Slow60, "60% Slow", $"Reduces {utilityString}movement speed{styleString} by 60% ");
            RegisterBuff(en, RoR2Content.Buffs.Slow80, "80% Slow", $"Reduces {utilityString}movement speed{styleString} by 80% ");
            RegisterBuff(en, RoR2Content.Buffs.BeetleJuice, "Beetle Juice", $"Reduce {damageString}movement speed{styleString}, {healingString}character damage{styleString}, and {damageString}attack speed{styleString} by 5%. ");

            RegisterBuff(en, RoR2Content.Buffs.Cripple, "Cripple", $"Reduces {utilityString}armor{styleString} by 20. Reduces {utilityString}movement speed{styleString} by 50%. ");

            RegisterBuff(en, RoR2Content.Buffs.Nullified, "Nullified ", $"Disables {utilityString}movement{styleString}.");
            RegisterBuff(en, RoR2Content.Buffs.NullifyStack, "Nullify Buildup", $"Upon getting 3 stacks, reset all stacks and apply the Nullified debuff. ");
            RegisterBuff(en, RoR2Content.Buffs.Overheat, "Overheat", $"Increases the duration of burn damage from Grandparent's sun attack. ");


            RegisterBuff(en, RoR2Content.Buffs.ClayGoo, "Tar", $"Reduces {utilityString}movement speed{styleString} by 50%. ");
            RegisterBuff(en, RoR2Content.Buffs.VoidFogMild, "Void Fog", $"Deals a small amount of {damageString}damage{styleString} multiple times per second, {healthString}increasing with each tick{styleString}. ");
            RegisterBuff(en, RoR2Content.Buffs.VoidFogStrong, "Void Fog", $"Deals a medium amount of {damageString}damage{styleString} multiple times per second, {healthString}increasing with each tick{styleString}.");

            #endregion

            #region DLC1
            ////Survivor
            RegisterBuff(en, DLC1Content.Buffs.VoidSurvivorCorruptMode, "Void Corruption", $"+100 Armor and your skills are {voidString}Corrupted.{styleString}");

            ////Items
            RegisterBuff(en, DLC1Content.Buffs.OutOfCombatArmorBuff, "Opal Armor", $"Gain {shrineString}+100 armor{styleString}. Removed on hit.");

            RegisterBuff(en, DLC1Content.Buffs.KillMoveSpeed, "Harpoon Kill", $"Gain {utilityString}+25% movement speed{styleString}.");
            RegisterBuff(en, DLC1Content.Buffs.PrimarySkillShurikenBuff, "Shuriken", $"Your next primary attack launches a high damage Shuriken.");

            RegisterBuff(en, DLC1Content.Buffs.PermanentDebuff, "Scorpion Armor Reduction", $"Reduces {utilityString}armor{styleString} by 2 per stack for the remainder of the stage, or until killed. ");
            RegisterBuff(en, DLC1Content.Buffs.ImmuneToDebuffReady, "Debuff Immunity", $"Will {utilityString}prevent{styleString} the next debuff and add 10% Barrier.");
            RegisterBuff(en, DLC1Content.Buffs.ImmuneToDebuffCooldown, "Debuff Immunity Cooldown", $"Ben's Raincoat is on cooldown");

            RegisterBuff(en, DLC1Content.Buffs.Fracture, "Collapse", $"3 seconds after the first stack is applied, deals 400% {damageString}damage{styleString} per stack and removes all stacks. ");
            RegisterBuff(en, DLC1Content.Buffs.MushroomVoidActive, "Weeping Fungus Regeneration", $"{healingString}Heal{styleString} while sprinting.");
            RegisterBuff(en, DLC1Content.Buffs.BearVoidReady, "Safer Spaces Ready", $"Negates the next source of damage, then goes on cooldown.");
            RegisterBuff(en, DLC1Content.Buffs.BearVoidCooldown, "Safer Spaces Cooldown", $"Damage negation is on cooldown.");

            RegisterBuff(en, DLC1Content.Buffs.ElementalRingVoidReady, "Singularity Band Ready", $"Singularity Band can be activated.");
            RegisterBuff(en, DLC1Content.Buffs.ElementalRingVoidCooldown, "Singularity Band Cooldown", $"Singularity Band is on cooldown");


            ////Elite
            RegisterBuff(en, DLC1Content.Buffs.EliteEarth, "Mending", $"{healingString}Heal{styleString} nearby non-mending allies. Produce a {healingString}healing core{styleString} on death, which detonates and heals all nearby allies. ");
            RegisterBuff(en, DLC1Content.Buffs.EliteVoid, "Voidtouched", $"{damageString}Collapse{styleString} on hit and {utilityString}block one hit{styleString} every 15 seconds.");

            //Other
            RegisterBuff(en, DLC1Content.Buffs.VoidRaidCrabWardWipeFog, "Voidling Fog", $"Deals a small amount of {damageString}damage{styleString} multiple times per second, {healthString}increasing with each tick{styleString}. Until you return to Voidling. ");

            #endregion


            //TODO : STYLE
            #region DLC2
            //Survivors
            RegisterBuff(en, DLC2Content.Buffs.ChakraBuff, "Tranquility", $"Improves Seekers skills in various ways."); //Mentioning 7 different buffs sounds like, uhh no?
            RegisterBuff(en, DLC2Content.Buffs.RevitalizeBuff, "Saving Grace", $"Increase your stats by 7%. Seeker can revive you using Meditate.");
            RegisterBuff(en, DLC2Content.Buffs.SeekerRevivedOnce, "Consumed Grace", $"You were revived by Seeker and cannot be revived by her again until the next stage.");

            RegisterBuff(en, DLC2Content.Buffs.Boosted, "Yes, CHEF!", $"CHEFs next attack will be upgraded.");
            RegisterBuff(en, DLC2Content.Buffs.Oiled, "Oily", $"You take +300% Burn damage");
            RegisterBuff(en, DLC2Content.Buffs.Frost, "Frost", $"Slows you by stacking amounts. Freeze upon getting 6 stacks or if Oiled."); //0.11, 0.25, 0.42 slows like uhh

            string cookingDesc = $"Enemies with 2 Cooking buffs drop Food. Food heals for 2hp + 4% max hp for each unique cooking buff";
            RegisterBuff(en, DLC2Content.Buffs.CookingChopped, "Cooking", cookingDesc);
            RegisterBuff(en, DLC2Content.Buffs.CookingRoasted, "Cooking", cookingDesc);
            RegisterBuff(en, DLC2Content.Buffs.CookingChilled, "Cooking", cookingDesc);
            RegisterBuff(en, DLC2Content.Buffs.CookingRolled, "Cooking", cookingDesc);
            RegisterBuff(en, DLC2Content.Buffs.CookingOiled, "Cooking", cookingDesc);

            RegisterBuff(en, DLC2Content.Buffs.EnergizedCore, "Energized Core", $"Lunar Tampering stat changes will be three times as effective.");
            RegisterBuff(en, DLC2Content.Buffs.lunarruin, "Lunar Ruin", $"Increase damage taken by 10% per buff. Decrease healing by 20%");
            //
            // Other
            RegisterBuff(en, DLC2Content.Buffs.DisableAllSkills, "Skills Disabled", $"Your skills and equipment are disabled.");
            RegisterBuff(en, DLC2Content.Buffs.ExtraLifeBuff, "Extra Life", $"Revive if you die this stage.");
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
            RegisterBuff(en, DLC2Content.Buffs.TeleportOnLowHealthActive, "Dimensional Aura", $"Unstable Transmitter has activated a dimensional Aura. Enemies that touch it and die will increase duration by 1s.");

            RegisterBuff(en, DLC2Content.Buffs.BoostAllStatsBuff, "Growth Nectar", $"Increase all stats by 4%");
            RegisterBuff(en, DLC2Content.Buffs.ExtraBossMissile, "War Bonds Missile", $"Stored Missiles dealing 2.5% max Hp damage, for the next boss or Scavenger encounter");

            RegisterBuff(en, DLC2Content.Buffs.FreeUnlocks, "Free Purchase", $"Your next purchase is free");
            //
            ////Elites
            RegisterBuff(en, DLC2Content.Buffs.EliteAurelionite, "Gilded", $"Fire out a Gilded Spike every 10-15s. Doing damage has a chance to produce extra Gold.");
            RegisterBuff(en, DLC2Content.Buffs.EliteBead, "Twisted", $"Give nearby allies 300 armor. Unleash a Twisted Spike after nearby allies are hurt 10 times. This ability has a 10s cooldown.");
            RegisterBuff(en, DLC2Content.Buffs.BeadArmor, "Twisted Armor", $"Gain 300 armor");

            #endregion
            #region DLC3
            RegisterBuff(en, DLC3Content.Buffs.StimShot, "Stim-Shot", $"Increase <style=cIsDamage>attack speed</style> and <style=cIsDamage>movement speed</style> by <style=cIsDamage>25%</style>."); //Not usually on player
            RegisterBuff(en, DLC3Content.Buffs.NanoBug, "Nano Bugged", $"Nano Bugged"); //Not usually on player
            RegisterBuff(en, DLC3Content.Buffs.EliteCollective, "Collective", $"Gain a 30m dome that protects you from outside attacks. All allies within gain 25% cooldown reduction.");
            RegisterBuff(en, DLC3Content.Buffs.CollectiveShareBuff, "Part Collective", $"Gain 25% cooldown reduction.");
            RegisterBuff(en, DLC3Content.Buffs.InventoryDisable, "Items Disabled", $"All your items are temporarily disabled.");
            RegisterBuff(en, DLC3Content.Buffs.Accelerant, "Greased", $"Being ignited or hit by fire-like attacks causes you to explode.");
            RegisterBuff(en, DLC3Content.Buffs.PowerCubeBuff, "Prison Matrix", $"Increase base armor by 50%");
            RegisterBuff(en, DLC3Content.Buffs.PowerPyramidBuff, "Sentry Key", $"Increase movement speed by 15%");
            RegisterBuff(en, DLC3Content.Buffs.TrashToTreasureWhite, "Common Trash to Treasure", $"Increase movement speed by 6%");
            RegisterBuff(en, DLC3Content.Buffs.TrashToTreasureGreen, "Uncommon Trash to Treasure", $"Increase base health regen speed by 3 hp/s.");
            RegisterBuff(en, DLC3Content.Buffs.TrashToTreasureRed, "Legendary Trash to Treasure", $"Increase attack speed by 30%");
            RegisterBuff(en, DLC3Content.Buffs.TrashToTreasureYellow, "Boss Trash to Treasure", $"Reduce skill cooldowns by 15%");
            RegisterBuff(en, DLC3Content.Buffs.JumpDamageStrikeCharge, "Faraday Charge", $"Increase movement speed by 1.6% and jump height by 2%");
            RegisterBuff(en, DLC3Content.Buffs.SpeedOnPickup, "Collectors Compulsion", $"Increases all stats by 3%");
            RegisterBuff(en, DLC3Content.Buffs.SharedSuffering, "Networked Suffering", $"Your suffering is networked."); //Not usually on player
            RegisterBuff(en, DLC3Content.Buffs.SharedSufferingStock, "Network Suffering Capacity", $"Your network of suffering is at max capacity");
            RegisterBuff(en, DLC3Content.Buffs.SharedSufferingStockEmpty, "Network Suffering Full", $"Your network of suffering is at max capacity");
            RegisterBuff(en, DLC3Content.Buffs.ShockDamageEnergized, "Conductor Energized", $"Increase attack and movement speed by 30%. All Electrical attacks deal crtical damage.");
            RegisterBuff(en, DLC3Content.Buffs.Brittle, "Cooled", $"Instantly remove all overheat and burn effects");
            RegisterBuff(en, DLC3Content.Buffs.UltimateMealBoost, "Ultimate Meal", $"+2 Luck per Ultimate Meal");
            RegisterBuff(en, DLC3Content.Buffs.CritChanceAndDamage, "Hiker's Boots", $"+1% Crit chance and damage");
            RegisterBuff(en, DLC3Content.Buffs.Parrying, "Parrying", $"Blocks incoming damage once and turns it into a blessing.");
            RegisterBuff(en, DLC3Content.Buffs.SureProc, "Deus Ex Machina Blessing", $"Your next attack triggers all chance based on-hit and on-kill effects.");
            RegisterBuff(en, DLC3Content.Buffs.Slow10Stacking, "10% Slow", $"Decrease movement speed by 10%");
            RegisterBuff(en, DLC3Content.Buffs.HealNovaRegen, "Emergency Nova Regeneration", $"Gain {healingString}health regeneration{styleString} equal to 2.5% of your {healingString}maximum health{styleString}. ");
            RegisterBuff(en, DLC3Content.Buffs.TransferDebuffOnHit, "Netronium Weight", $"Netronium Weight is decreasing all your stats.");
            RegisterBuff(en, DLC3Content.Buffs.Jailed, "Jailed", $"Immobilized."); //Not usually on player

            #endregion
            #region DLC4
            RegisterBuff(en, DLC4Content.Buffs.BogSlow, "Bog Slow", $"Reduces movement speed by -50% while standing on this surface.");
            RegisterBuff(en, DLC4Content.Buffs.EliteDeath, "Stygian", $"Allies killed in a 60m radius revive as invulnerable ghosts with +45 movement speed and 70% skill cooldown reduction. Ghosts last 15 seconds. Limit of 3 ghosts.");

            ////Items
            //Boss
            RegisterBuff(en, DLC4Content.Buffs.WispOnKillCounter, "Whispering Fragment Counter", $"When this reaches 5, gain a orbiting spirit from Whispering Fragment.");

            //Hallowed
            RegisterBuff(en, DLC4Content.Buffs.LowHealthOrbSpawnerReady, "Dream Crucible Ready", $"Dream Crucible can prevent an item from going on cooldown.");
            RegisterBuff(en, DLC4Content.Buffs.LowHealthOrbSpawnerCooldown, "Dream Crucible Cooldown", $"Dream Crucible is recharging.");
            RegisterBuff(en, DLC4Content.Buffs.RechargeableExtraJumpCounter, "Earthbound Vessel Jump", $"An extra jump. Increases armor by 5 passively until landing after using it.");
            RegisterBuff(en, DLC4Content.Buffs.MalignantAttraction, "Soul-Branded Buildup", $"Upon getting 10 stacks, become Soul-Branded.");
            RegisterBuff(en, DLC4Content.Buffs.MalignantAttractionActive, "Soul-Branded", $"Members of the same team can damage you and will target you.");
            RegisterBuff(en, DLC4Content.Buffs.PheromoneExhaustion, "Infernal Mark Cooldown", "");
            RegisterBuff(en, DLC4Content.Buffs.MidasTouchCooldown, "Totem of Averice Cooldown", "Totem of Averice is on cooldown.");
            RegisterBuff(en, DLC4Content.Buffs.MidasTouchReady, "Totem of Averice Ready", "Totem of Averice can be used.");
            RegisterBuff(en, DLC4Content.Buffs.MidasTouchFrozen, "Midas", "Immobilized.");
            RegisterBuff(en, DLC4Content.Buffs.SprintDamageCooldown, "Ritual Helmet Cooldown", "Ritual Helmet needs to be recharged by sprinting.");
            RegisterBuff(en, DLC4Content.Buffs.SprintDamageReady, "Ritual Helmet Ready", "Ritual Helmet can be used.");
            RegisterBuff(en, DLC4Content.Buffs.SprintDamageSpeed, "Ritual Helmet Speed", "Increases movement speed by +5%.");
            RegisterBuff(en, DLC4Content.Buffs.VengefulRage, "Vengeful Spirit", "Increases attack speed by +40% and reduces skill cooldowns by 20%.");
            //Hex
            RegisterBuff(en, DLC4Content.Buffs.OutOfTPRadius, "Hex of Patience", $"Take damage. Damage amount increases after every 10 seconds.");
            RegisterBuff(en, DLC4Content.Buffs.WeakAssHamstringJumpReduction, "Hex of Leaping", $"Reduced jump height by 0.5%");
            //Pages
            RegisterBuff(en, DLC4Content.Buffs.Clarity, "Clarity", $"Reduces skill and equipment cooldowns by 75%");
            RegisterBuff(en, DLC4Content.Buffs.DisableDefenses, "Sacrifice", $"Take damage");
            ////Survivors
            RegisterBuff(en, DLC4Content.Buffs.CrowdControlBuff, "Rally Point", $"Gain barrier equal to 10% of your max health on kill.");
            RegisterBuff(en, DLC4Content.Buffs.TearGasBerserk, "Taunted", $"Reduces armor by 10. AI-controlled characters are made to move more aggressively."); //Ripcord
            RegisterBuff(en, DLC4Content.Buffs.AcoStealBackHealthBuff, "N'kuhana's Grace", $"Increases attack speed by 1%");
            RegisterBuff(en, DLC4Content.Buffs.AcoAttackSpeed, "N'kuhana's Favor", $"Increases attack speed by 1%");
            RegisterBuff(en, DLC4Content.Buffs.Haunted, "Haunted", $"Take 50% damage per second and release a seeking wight on death.");
            //
            RegisterBuff(en, DLC4Content.Buffs.LynxPoison, "Lynx Poison", $"Take 45% damage per second.");
            #endregion

        }

        public static void RegisterBuff(Language language, BuffDef buff, string name, string description) //coulda used this earlier, but w/e
        {
            LookingGlassLanguageAPI.SetupToken(language, $"NAME_{buff.name}", name);
            LookingGlassLanguageAPI.SetupToken(language, $"DESCRIPTION_{buff.name}", description);
        }
    }
}
