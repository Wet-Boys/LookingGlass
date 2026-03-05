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


            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.HealingDisabled.name}", $"Malachite Poisoning");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.HealingDisabled.name}", $"Disables all healing, including health regeneration. ");
            RegisterBuff(en, RoR2Content.Buffs.AffixHauntedRecipient, "Celestine Cloak", $"You are {utilityString}cloaked{styleString} due to a Celestine Ally.");


            //((Survivor
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.BanditSkull.name}", $"Bandit Skull");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.BanditSkull.name}", $"Desperado gains {damageString}10% damage{styleString} per stack.");

            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.ArmorBoost.name}", $"Strong Armor Boost");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.ArmorBoost.name}", $"Gain {utilityString}+200 armor.{styleString} ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.SmallArmorBoost.name}", $"Small Armor Boost");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.SmallArmorBoost.name}", $"Gain {utilityString}+100 armor{styleString}. ");

            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.CrocoRegen.name}", $"Regenerative");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.CrocoRegen.name}", $"Gain {healingString}health regeneration{styleString} equal to 5% of your {healingString}maximum health{styleString}. ");

            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Weak.name}", $"Weak");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Weak.name}", $"Reduces {utilityString}armor{styleString} by 30%, reduces {utilityString}movement speed and damage{styleString} by 40%. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Entangle.name}", $"Entangle");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Entangle.name}", $"Disables {utilityString}movement.{styleString}");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Fruiting.name}", $"Fruiting");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Fruiting.name}", $"Spawn 2-8 {healingString}healing{styleString} fruits on death. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.MercExpose.name}", $"Expose");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.MercExpose.name}", $"Will take {damageString}350% damage{styleString} from next attack.");

            ////Items

            //White
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.MedkitHeal.name}", $"Medkit Heal");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.MedkitHeal.name}", $"Gain {healingString}health{styleString} after a 2-second delay. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Warbanner.name}", $"Warbanner");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Warbanner.name}", $"Gain {utilityString}+30% attack{styleString} and {utilityString}movement speed{styleString}. ");

            //Green
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.ElementalRingsReady.name}", $"Elemental Bands Ready");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.ElementalRingsReady.name}", $"Elemental bands can be activated.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.ElementalRingsCooldown.name}", $"Elemental Bands Cooldown");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.ElementalRingsCooldown.name}", $"Elemental Bands are on cooldown");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.WhipBoost.name}", $"Whip Boost");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.WhipBoost.name}", $"Gain {utilityString}+30% movement speed{styleString} for each Red Whip. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.AttackSpeedOnCrit.name}", $"Attack Speed On Crit");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.AttackSpeedOnCrit.name}", $"Gain {utilityString}+12% attack speed{styleString} per stack.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Cloak.name}", $"Cloak");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Cloak.name}", $"{utilityString}Disappear{styleString}. Enemies cannot target invisible allies.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.CloakSpeed.name}", $"Cloak Speed");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.CloakSpeed.name}", $"Gain {utilityString}+40% movement speed{styleString} while invisible. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Energized.name}", $"Energized");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Energized.name}", $"Gain {utilityString}+70% attack speed{styleString}.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.DeathMark.name}", $"Death Mark");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.DeathMark.name}", $"Increases {damageString}damage taken{styleString} from {healthString}all sources{styleString} by 50% for 7 (+7 per stack) seconds. ");

            //Red
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.NoCooldowns.name}", $"No Cooldowns");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.NoCooldowns.name}", $"Ability cooldowns reduced to 0.5s. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.LaserTurbineKillCharge.name}", $"Resonance Disc Kill Charge");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.LaserTurbineKillCharge.name}", $"Gain a stack per kill that lasts for 7 seconds. At 4 stacks, the Resonance Disc fires, resetting all stacks. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.TeslaField.name}", $"Tesla Field");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.TeslaField.name}", $"Shock nearby enemies for {damageString}200% damage{styleString} every 0.5s.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Pulverized.name}", $"Pulverized");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Pulverized.name}", $"Reduces {utilityString}armor{styleString} by 60.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.PulverizeBuildup.name}", $"Pulverize Buildup");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.PulverizeBuildup.name}", $"Upon getting 5 stacks, reset all stacks and apply the Pulverized debuff. ");

            //Lunar
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.PowerBuff.name}", $"Power Buff"); //Rachis
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.PowerBuff.name}", $"Gain {damageString}+50% damage{styleString}. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.LunarSecondaryRoot.name}", $"Lunar Root");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.LunarSecondaryRoot.name}", $"Disables {utilityString}movement{styleString}. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.LunarDetonationCharge.name}", $"Ruin");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.LunarDetonationCharge.name}", $"Consumes Ruin stacks to deal 300% {damageString}damage{styleString} plus 120% {damageString}damage{styleString} per Ruin stack ");

            ////Equipment
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.ElephantArmorBoost.name}", $"Elephant Armor Boost");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.ElephantArmorBoost.name}", $"Gain {shrineString}+500 armor{styleString}, negating most attacks. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.FullCrit.name}", $"Full Crit");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.FullCrit.name}", $"Gain {damageString}100% critical strike{styleString} chance. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.LifeSteal.name}", $"Life Steal");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.LifeSteal.name}", $"{healingString}Heal{styleString} for 20% of {damageString}damage{styleString} dealt.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.TeamWarCry.name}", $"Team War Cry");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.TeamWarCry.name}", $"Gain {utilityString}+50% movement speed{styleString} and {utilityString}+100% attack speed{styleString}. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.WarCryBuff.name}", $"War Cry");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.WarCryBuff.name}", $"Gain {utilityString}+50% movement speed{styleString} and {utilityString}+100% attack speed{styleString}. ");

            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.TonicBuff.name}", $"Spinel Tonic");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.TonicBuff.name}", $"Gain the following boosts:\n{healingString}+150% max health{styleString}\n{healingString}+400% health regeneration{styleString}\n{utilityString}+170% attack speed{styleString}\n{utilityString}+130% movement speed{styleString}\n{utilityString}+20 armor{styleString}\n{damageString}+200% base damage{styleString}");

            //General
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Immune.name}", $"Immune");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Immune.name}", $"Become immune to all attacks.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.HiddenInvincibility.name}", $"Hidden Invincibility");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.HiddenInvincibility.name}", $"Become immune to all attacks.");

            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.PermanentCurse.name}", $"Permanent Curse");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.PermanentCurse.name}", $"Maximum health reduced");

            //DoTs
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Bleeding.name}", $"Bleed");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Bleeding.name}", $"Deals {damageString}240% damage{styleString} over time. Refreshes duration when applied.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.SuperBleed.name}", $"Hemorrhage");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.SuperBleed.name}", $"{damageString}2000% base damage{styleString} over 15s. Refreshes duration when applied.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Poisoned.name}", $"Poisoned");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Poisoned.name}", $"Deals 1% of the victim's {damageString}maximum health{styleString} per second");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Blight.name}", $"Blight");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Blight.name}", $"Deals {damageString}20% base damage stat{styleString} per tick");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.OnFire.name}", $"Ignited"); //Game keeps saying Ignite ig
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.OnFire.name}", $"Applies a percent of {damageString}damage{styleString} over time, and {healthString}disables health regeneration.{styleString} ");
            RegisterBuff(en, DLC1Content.Buffs.StrongerBurn, "Wild Burn", $"A deadly burn with at least 4x effectiveness of regular Ignite. Health regeneration is disabled. ");





            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Slow50.name}", $"50% Slow");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Slow50.name}", $"Reduces {utilityString}movement speed{styleString} by 50% ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Slow60.name}", $"60% Slow");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Slow60.name}", $"Reduces {utilityString}movement speed{styleString} by 60% ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Slow80.name}", $"80% Slow");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Slow80.name}", $"Reduces {utilityString}movement speed{styleString} by 80% ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.BeetleJuice.name}", $"Beetle Juice");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.BeetleJuice.name}", $"Reduce {damageString}movement speed{styleString}, {healingString}character damage{styleString}, and {damageString}attack speed{styleString} by 5%. ");

            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Cripple.name}", $"Cripple");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Cripple.name}", $"Reduces {utilityString}armor{styleString} by 20. Reduces {utilityString}movement speed{styleString} by 50%. ");

            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Nullified.name}", $"Nullified ");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Nullified.name}", $"Disables {utilityString}movement{styleString}.");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.NullifyStack.name}", $"Nullify Buildup");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.NullifyStack.name}", $"Upon getting 3 stacks, reset all stacks and apply the Nullified debuff. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.Overheat.name}", $"Overheat");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.Overheat.name}", $"Increases the duration of burn damage from Grandparent's sun attack. ");


            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.ClayGoo.name}", $"Tar");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.ClayGoo.name}", $"Reduces {utilityString}movement speed{styleString} by 50%. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.VoidFogMild.name}", $"Void Fog");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.VoidFogMild.name}", $"Deals a small amount of {damageString}damage{styleString} multiple times per second, {healthString}increasing with each tick{styleString}. ");
            LookingGlassLanguageAPI.SetupToken(en, $"NAME_{RoR2Content.Buffs.VoidFogStrong.name}", $"Void Fog");
            LookingGlassLanguageAPI.SetupToken(en, $"DESCRIPTION_{RoR2Content.Buffs.VoidFogStrong.name}", $"Deals a medium amount of {damageString}damage{styleString} multiple times per second, {healthString}increasing with each tick{styleString}.");

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
            RegisterBuff(en, DLC2Content.Buffs.TeleportOnLowHealthActive, "Unstable Aura", $"Unstable Transmitter has activated a dimensional Aura. Enemies that touch it and die will increase duration by 1s.");

            RegisterBuff(en, DLC2Content.Buffs.BoostAllStatsBuff, "Growth Nectar", $"Increase all stats by 4%");
            RegisterBuff(en, DLC2Content.Buffs.ExtraBossMissile, "War Missile", $"Stored Missiles dealing 2.5% max Hp damage, for the next boss or Scavenger encounter");

            RegisterBuff(en, DLC2Content.Buffs.FreeUnlocks, "Free Purchase", $"Your next purchase is free");
            //
            ////Elites
            RegisterBuff(en, DLC2Content.Buffs.EliteAurelionite, "Gilded", $"Fire out a Gilded Spike every 10-15s. Doing damage has a chance to produce extra Gold.");
            RegisterBuff(en, DLC2Content.Buffs.EliteBead, "Twisted", $"Give nearby allies 300 Armor. Unleash a Twisted Spike after nearby allies are hurt 10 times. This ability has a 10s cooldown.");
            RegisterBuff(en, DLC2Content.Buffs.BeadArmor, "Twisted Armor", $"Gain 300 Armor");

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
            RegisterBuff(en, DLC3Content.Buffs.SharedSuffering, "Networked Suffering", $""); //Not usually on player
            RegisterBuff(en, DLC3Content.Buffs.SharedSufferingStock, "Network Capacity", $"Your network of suffering is at max capacity");
            RegisterBuff(en, DLC3Content.Buffs.SharedSufferingStockEmpty, "Network Full", $"Your network of suffering is at max capacity");
            RegisterBuff(en, DLC3Content.Buffs.ShockDamageEnergized, "Conductor Energized", $"Increase attack and movement speed by 30%. All Electrical attacks deal crtical damage.");
            RegisterBuff(en, DLC3Content.Buffs.Brittle, "Cooled", $"Instantly remove all overheat and burn effects");
            RegisterBuff(en, DLC3Content.Buffs.UltimateMealBoost, "Ultimate Meal", $"+2 Luck per Ultimate Meal");
            RegisterBuff(en, DLC3Content.Buffs.CritChanceAndDamage, "Hiking", $"+1% Crit chance and damage");
            RegisterBuff(en, DLC3Content.Buffs.Parrying, "Parrying", $"Blocks incoming damage once and turns it into a blessing.");
            RegisterBuff(en, DLC3Content.Buffs.SureProc, "God's Blessing", $"Your next attack triggers all chance based on-hit and on-kill effects.");
            RegisterBuff(en, DLC3Content.Buffs.Slow10Stacking, "10% Slow", $"Decrease movement speed by 10%");
            RegisterBuff(en, DLC3Content.Buffs.HealNovaRegen, "Emergency Nova Regeneration", $"Gain {healingString}health regeneration{styleString} equal to 2.5% of your {healingString}maximum health{styleString}. ");
            RegisterBuff(en, DLC3Content.Buffs.TransferDebuffOnHit, "Weighted", $"Netronium Weight is decreasing all your stats.");
            RegisterBuff(en, DLC3Content.Buffs.Jailed, "Jailed", $"Jailed"); //Not usually on player

            #endregion
        }
        public static void RegisterBuff(Language language, BuffDef buff, string name, string description) //coulda used this earlier, but w/e
        {
            LookingGlassLanguageAPI.SetupToken(language, $"NAME_{buff.name}", name);
            LookingGlassLanguageAPI.SetupToken(language, $"DESCRIPTION_{buff.name}", description);
        }
    }
}
