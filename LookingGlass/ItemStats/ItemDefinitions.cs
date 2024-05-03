using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static RoR2.HealthComponent;

namespace LookingGlass.ItemStatsNameSpace
{
    public class ItemDefinitions
    {
        public static Dictionary<int, ItemStatsDef> allItemDefinitions = new Dictionary<int, ItemStatsDef>();
        public static void RegisterItemStatsDef(ItemStatsDef itemStatsDef, ItemIndex itemIndex)
        {
            allItemDefinitions.Add((int)itemIndex, itemStatsDef);
        }
        internal static void RegisterAll()
        {
            //foreach (var item in ItemCatalog.allItems)
            //{
            //    Log.Debug($"==========  {item}  {ItemCatalog.GetItemDef(item).nameToken}  {ItemCatalog.GetItemDef(item).name}");
            //}
            //Tougher Times
            ItemStatsDef itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Block Chance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([(0.15f * stackCount) / ((0.15f * stackCount) + 1)]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("Bear"), itemStat);

            //Armor-Piercing Rounds
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Damage Increase to Bosses: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.2f * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("BossDamageBonus"), itemStat);

            //Backup Magazine
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Extra Secondary Charges: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("SecondarySkillMagazine"), itemStat);

            //Bison Steak
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Health Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Health);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 25]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("FlatHealth"), itemStat);

            //Bundle of Fireworks
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Fireworks Launched: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([4 + (stackCount * 4)]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("Firework"), itemStat);

            //Bustling Fungus
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("hp/s: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Radius: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Meters);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.0225f + (.0225f * stackCount), 1.5f + (1.5f * stackCount)]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("Mushroom"), itemStat);

            //Cautious Slug
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("hp/s: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 3]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("HealWhileSafe"), itemStat);

            //Crowbar
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Damage Increase to Healthy Enemies: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * .75f]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("Crowbar"), itemStat);

            //Delicate Watch
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Damage Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.2f * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("FragileDamageBonus"), itemStat);

            //Energy Drink
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Sprint Bonus: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.25f * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("SprintBonus"), itemStat);

            //Focus Crystal
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Nearby Damage Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.2f * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("NearbyDamageBonus"), itemStat);

            //Gasoline
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Total Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Burn Duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.descriptions.Add("Radius: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Meters);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([2.25f + (.75f * stackCount), 1.5f + (1.5f * stackCount), 8 + (4 * stackCount)]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("IgniteOnKill"), itemStat);

            //Lens-Maker's Glasses
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Critical Chance Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.1f * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("CritGlasses"), itemStat);

            //Medkit
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Heal Amount: <style=\"cIsHealing\">20hp</style> + ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.05f * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("Medkit"), itemStat);

            //Mocha
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Attack Speed Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Movement Speed Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.075f * stackCount, .07f * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("AttackSpeedAndMoveSpeed"), itemStat);

            //Monster Tooth
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Heal Amount: <style=\"cIsHealing\">8hp</style> + ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.02f * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("Tooth"), itemStat);

            //Oddly-shaped Opal
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Armor Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([100 * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("OutOfCombatArmor"), itemStat);

            //Paul's Goat Hoof
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Movement Speed Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.14f * stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.Hoof.itemIndex, itemStat);

            //Personal Shield Generator
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Bonus Shield: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.08f * stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.PersonalShield.itemIndex, itemStat);

            //Power Elixir
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Available Charges: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("HealingPotion"), itemStat);

            //Repulsion Armor Plate
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Damage Reduction: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([5 * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("ArmorPlate"), itemStat);

            //Roll of Pennies
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Gold Gained: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([3 * stackCount * RoR2.Run.instance.difficultyCoefficient]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("GoldOnHurt"), itemStat);


            //Rusted Key
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Available Charges: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("TreasureCache"), itemStat);


            //Soldier's Syringe
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Attack Speed Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.15f * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("Syringe"), itemStat);


            //Sticky Bomb
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Sticky Bomb Chance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.05f * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("StickyBomb"), itemStat);


            //Stun Grenade
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Stun Chance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([(0.05f * stackCount) / ((0.05f * stackCount) + 1)]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("StunChanceOnHit"), itemStat);


            //Topaz Brooch
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Barrier Gain on Kill: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([15 * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("BarrierOnKill"), itemStat);


            //Tri-Tip Dagger
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Bleed Chance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.1f * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("BleedOnHit"), itemStat);


            //Warbanner
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Radius: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Meters);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([8 + (8 * stackCount)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.WardOnLevel.itemIndex, itemStat);



            //Green


            //AtG Missile Mk. 1
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([3 * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("Missile"), itemStat);


            //Bandolier
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Drop Chance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.08f + (.1f * stackCount)]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("Bandolier"), itemStat);


            //Berzerker's Pauldron
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Buff Duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([2 + (stackCount * 4)]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("WarCryOnMultiKill"), itemStat);


            //Chronobauble
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Debuff Duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([2 * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("SlowOnHit"), itemStat);


            //Death Mark
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Debuff Duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([7 * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("DeathMark"), itemStat);


            //Fuel Cell
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Extra Charges: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.descriptions.Add("Equipment Cooldown Reduction: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount, 1 - Mathf.Pow(1 - .15f, stackCount)]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("EquipmentMagazine"), itemStat);


            //Ghor's Tome
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Drop Chance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.04f * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("BonusGoldPackOnKill"), itemStat);


            //Harvester's Scythe
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Heal Amount: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Health);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([4 + (4 * stackCount)]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("HealOnCrit"), itemStat);


            //Hopoo Feather
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Bonus Jumps: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("Feather"), itemStat);


            //Hunter's Harpoon
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Buff Duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.5f + (.5f * stackCount)]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("MoveSpeedOnKill"), itemStat);


            //Ignition Tank
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Burn Damage Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([3 * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("StrengthenBurn"), itemStat);


            //Infusion
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Max Health Per Kill: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Health);
            itemStat.descriptions.Add("Max Health Gained: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Health);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount, 100 * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("Infusion"), itemStat);


            //Kjaro's Band
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Total Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([3 * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("FireRing"), itemStat);


            //Leeching Seed
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Heal Per Hit: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Health);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("Seed"), itemStat);


            //Lepton Daisy
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Total Novas: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Health);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("TPHealingNova"), itemStat);


            //Old Guillotine
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("InstaKill Threshhold: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([(0.13f * stackCount) / ((0.13f * stackCount) + 1)]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("ExecuteLowHealthElite"), itemStat);


            //Old War Stealthkit
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Recharge duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([30 * Mathf.Pow(0.5f, stackCount - 1)]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("Phasing"), itemStat);


            //Predatory Instincts
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Max Attack Speed Bonus: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.12f + (.24f * stackCount)]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("AttackSpeedOnCrit"), itemStat);


            //Regenerating Scrap
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Available Charges: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("RegeneratingScrap"), itemStat);


            //Razorwire
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Max Targets: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Meters);
            itemStat.descriptions.Add("Radius: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Meters);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([3 + (2 * stackCount), 15 + (10 * stackCount)]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("Thorns"), itemStat);


            //Red Whip
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Out of Combat Speed Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.3f * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("SprintOutOfCombat"), itemStat);


            //Rose Buckler
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Sprint Armor Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([30 * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("SprintArmor"), itemStat);


            //Runald's Band
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Slow Duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.descriptions.Add("Total Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([3 * stackCount, 2.5f * stackCount]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("IceRing"), itemStat);


            //Shipping Request Form
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Common Chance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Uncommon Chance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Legendary Chance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                float common = .79f;
                float uncommon = .2f * stackCount;
                float rare = .01f * Mathf.Pow(stackCount, 2);
                float sum = common + uncommon + rare;
                return new List<float>([common / sum, uncommon / sum, rare / sum]);
            };
            allItemDefinitions.Add((int)ItemCatalog.FindItemIndex("FreeChest"), itemStat);


            //Shuriken
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Base Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Total Shurikens: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([3 + (1 * stackCount), 2 + stackCount]);
            };
            allItemDefinitions.Add((int)RoR2.DLC1Content.Items.PrimarySkillShuriken.itemIndex, itemStat);


            //Squid Polyp
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Turret Attack Speed: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.Squid.itemIndex, itemStat);


            //Ukulele
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Max Targets: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.descriptions.Add("Chain Distance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Meters);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([1 + (2 * stackCount), 18 + (2 * stackCount)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.ChainLightning.itemIndex, itemStat);


            //War Horn
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Buff Durations: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([4 + (4 * stackCount)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.EnergizedOnEquipmentUse.itemIndex, itemStat);


            //Wax Quail
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Jump Boost: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Meters);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([10 * stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.JumpBoost.itemIndex, itemStat);


            //Will-o'-the-wisp
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Radius: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Meters);
            itemStat.descriptions.Add("Base Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([9.6f + (2.4f * stackCount), .7f + (2.8f * stackCount)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.ExplodeOnDeath.itemIndex, itemStat);




            //RED

            //57 Leaf Clover
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Rerolls: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.Clover.itemIndex, itemStat);

            //Aegis HOLY SHIT XENOBLADE???
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Barrier Conversion Rate: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.5f * stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.BarrierOnOverHeal.itemIndex, itemStat);

            //Alien Head
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Cooldown Reduction: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([1 - Mathf.Pow(0.75f, stackCount)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.AlienHead.itemIndex, itemStat);

            //Ben's Raincoat
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Debuff Prevention Stacks: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.ImmuneToDebuff.itemIndex, itemStat);

            //Bottled Chaos
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Random Effects: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.RandomEquipmentTrigger.itemIndex, itemStat);

            //Brainstalks
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Frenzy Duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 4]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.KillEliteFrenzy.itemIndex, itemStat);

            //Brilliant Behemoth
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Explosion Radius: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Meters);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([1.5f + (2.5f * stackCount)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.Behemoth.itemIndex, itemStat);

            //Ceremonial Dagger
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Dagger Base Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([1.5f * stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.Dagger.itemIndex, itemStat);

            //Defensive Microbots
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Blocked Projectiles: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.CaptainDefenseMatrix.itemIndex, itemStat);

            //Dio's Best Friend
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Revives: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.ExtraLife.itemIndex, itemStat);

            //Frost Relic
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Max Radius: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Meters);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([6 + (12 * stackCount)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.Icicle.itemIndex, itemStat);

            //H3AD-5T v2
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Recharge Time: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([10f * Mathf.Pow(0.5f, stackCount - 1)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.FallBoots.itemIndex, itemStat);

            //Happiest Mask
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Ghost Duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 30]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.GhostOnKill.itemIndex, itemStat);

            //Hardlight Afterburner
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Extra Utility Charges: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 2]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.UtilitySkillMagazine.itemIndex, itemStat);

            //Interstellar Desk Plant
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Radius: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Meters);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([5 + (5 * stackCount)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.Plant.itemIndex, itemStat);

            //Laser Scope
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Critical Strike Bonus Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.CritDamage.itemIndex, itemStat);

            //N'kuhana's Opinion
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Healing Stored: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.NovaOnHeal.itemIndex, itemStat);

            //Pocket I.C.B.M.
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Missile Damage Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([-.5f + (.5f * stackCount)]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.MoreMissile.itemIndex, itemStat);

            //Rejuvenation Rack
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Bonus Healing: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.IncreaseHealing.itemIndex, itemStat);

            //Resonance Disc
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Piercing Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Explosion Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 3, stackCount * 10]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.LaserTurbine.itemIndex, itemStat);

            //Sentient Meat Hook
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Hook Chance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Hook Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([(0.2f * stackCount) / ((0.2f * stackCount) + 1), stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.BounceNearby.itemIndex, itemStat);

            //Shattering Justice
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Debuff Duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 8]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.ArmorReductionOnHit.itemIndex, itemStat);

            //Soulbound Catalyst
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Reduction Amount: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([2 + (2 * stackCount)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.Talisman.itemIndex, itemStat);

            //Spare Drone Parts
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Extra Drone Attack Speed: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.5f * stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.DroneWeapons.itemIndex, itemStat);

            //Symbiotic Scorpion
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Armor Reduction Per Hit: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 2]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.PermanentDebuffOnHit.itemIndex, itemStat);

            //Unstable Tesla Coil
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Max Enemies Hit: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([1 + (2 * stackCount)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.ShockNearby.itemIndex, itemStat);

            //Wake of Vultures
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Buff Duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([3 + (stackCount * 5)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.HeadHunter.itemIndex, itemStat);


            //Boss

            //Charged Perforator
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 5]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.LightningStrikeOnHit.itemIndex, itemStat);

            //Defense Nucleus
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Max Constructs: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 4]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.MinorConstructOnKill.itemIndex, itemStat);

            //Empathy Cores
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Core Base Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.RoboBallBuddy.itemIndex, itemStat);

            //Genesis Loop
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Recharge Time: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([30 / (1 + stackCount)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.NovaOnLowHealth.itemIndex, itemStat);

            //Halcyon Seed
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Aurelionite Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Aurelionite Health: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.5f + (.5f * stackCount), stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.TitanGoldDuringTP.itemIndex, itemStat);

            //Irradiant Pearl
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Stat Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.1f * stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.ShinyPearl.itemIndex, itemStat);

            //Little Disciple
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Wisp Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 3]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.SprintWisp.itemIndex, itemStat);

            //Mired Urn
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Tethered Entities: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.SiphonOnLowHealth.itemIndex, itemStat);

            //Molten Perforator
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Magma Ball Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 3]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.FireballsOnHit.itemIndex, itemStat);

            //Pearl
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Health Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.1f * stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.Pearl.itemIndex, itemStat);

            //Planula
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Amount Healed: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Health);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 15]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.ParentEgg.itemIndex, itemStat);

            //Queen's Gland
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Beetle Guard Count: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.BeetleGland.itemIndex, itemStat);

            //Shatterspleen
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Explosion Base Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Explosion Health% Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([4 * stackCount, .15f * stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.BleedOnHitAndExplode.itemIndex, itemStat);

            //Titanic Knurl
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Max Health Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Health);
            itemStat.descriptions.Add("hp/s Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 40, stackCount * 1.6f]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.Knurl.itemIndex, itemStat);


            //why are you blue

            //Beads of Fealty
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Nothings Done: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.LunarTrinket.itemIndex, itemStat);

            //Brittle Crown
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Gold Gained: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.descriptions.Add("Gold Lost: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Health);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 2 * RoR2.Run.instance.difficultyCoefficient, stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.GoldOnHit.itemIndex, itemStat);

            //Corpsebloom
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Heal Bonus: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Max Healing Per Second: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount, .1f * (1/stackCount)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.RepeatHeal.itemIndex, itemStat);

            //Defiant Gouge
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Enemy Strength: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.MonstersOnShrineUse.itemIndex, itemStat);

            //Egocentrism
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Orb Cooldown: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.descriptions.Add("Max Orbs: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([3 * (1 / stackCount), 2 + stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.LunarSun.itemIndex, itemStat);

            //Essence of Heresy
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Stack Duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.descriptions.Add("Recharge Time: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 10, stackCount * 8]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.LunarSpecialReplacement.itemIndex, itemStat);

            //Eulogy Zero
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Lunar Item Chance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.05f * stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.RandomlyLunar.itemIndex, itemStat);

            //Focused Convergence
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Charge Time: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.descriptions.Add("Zone Size: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Health);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([90f / (1f + 0.3f * Mathf.Min(stackCount, 3f)), 1 / (2 * Mathf.Min(stackCount, 3f))]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.FocusConvergence.itemIndex, itemStat);

            //Gesture of the Drowned
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Equipment Cooldown: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([1f - (0.5f * Mathf.Pow(0.85f, stackCount - 1))]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.AutoCastEquipment.itemIndex, itemStat);

            //Hooks of Heresy
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Root Duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.descriptions.Add("Recharge Time: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 3, stackCount * 5]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.LunarSecondaryReplacement.itemIndex, itemStat);

            //Light Flux Pauldron
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Skill Cooldown: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Attack Speed Reduction: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([Mathf.Pow(.5f, stackCount), 1 / (stackCount + 1)]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.HalfAttackSpeedHalfCooldowns.itemIndex, itemStat);

            //Mercurial Rachis
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Radius: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Meters);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([16f * Mathf.Pow(1.5f, stackCount - 1)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.RandomDamageZone.itemIndex, itemStat);

            //Purity
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Skill Cooldown Reduction: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.descriptions.Add("Negative Luck: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Health);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount + 1, stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.LunarBadLuck.itemIndex, itemStat);

            //Shaped Glass
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Total Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Total Health: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([Mathf.Pow(2, stackCount), Mathf.Pow(.5f, stackCount)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.LunarDagger.itemIndex, itemStat);

            //Stone Flux Pauldron
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Health Increase: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Movement Decrease: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount, 1 / (1 + stackCount)]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.HalfSpeedDoubleHealth.itemIndex, itemStat);

            //Strides of Heresy
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Heal Amount: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.182f * stackCount, stackCount * 3]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.LunarUtilityReplacement.itemIndex, itemStat);

            //Transcendence
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Bonus Shields: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.25f + (.25f * stackCount)]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.ShieldOnly.itemIndex, itemStat);

            //Visions of Heresy
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Charges: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.descriptions.Add("Recharge Time: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([12 * stackCount, 2 * stackCount]);
            };
            allItemDefinitions.Add((int)RoR2Content.Items.LunarPrimaryReplacement.itemIndex, itemStat);

            //Benthic Bloom
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Items Upgraded Per Stage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 3]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.CloverVoid.itemIndex, itemStat);

            //Encrusted Key
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Charges: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.TreasureCacheVoid.itemIndex, itemStat);

            //Lost Seer's Lenses
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Instakill Chance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.005f * stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.CritGlassesVoid.itemIndex, itemStat);

            //Lysate Cell
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Extra Special Skill Stacks: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.EquipmentMagazineVoid.itemIndex, itemStat);

            //Needletick
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Collapse Chance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.1f * stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.BleedOnHitVoid.itemIndex, itemStat);

            //Newly Hatched Zoea
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Max Allies: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.descriptions.Add("Recharge Time: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount, 60f / stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.VoidMegaCrabItem.itemIndex, itemStat);

            //Plasma Shrimp
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Total Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.4f * stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.MissileVoid.itemIndex, itemStat);

            //Pluripotent Larva
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Extra Lives: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.ExtraLifeVoid.itemIndex, itemStat);

            //Polylute
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Total Hits: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount * 3]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.ChainLightningVoid.itemIndex, itemStat);

            //Safer Spaces
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Cooldown: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([15 * Mathf.Pow(.9f, stackCount)]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.BearVoid.itemIndex, itemStat);

            //Singularity Band
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Total Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.ElementalRingVoid.itemIndex, itemStat);

            //Tentabauble
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Root Chance: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.descriptions.Add("Duration: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([1 - 1/(1 + .05f * stackCount), stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.SlowOnHitVoid.itemIndex, itemStat);

            //Voidsent Flame
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Radius: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Meters);
            itemStat.descriptions.Add("Base Damage: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([9.6f + (2.4f * stackCount), 1.04f + (1.56f * stackCount)]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.ExplodeOnDeathVoid.itemIndex, itemStat);

            //Weeping Fungus
            itemStat = new ItemStatsDef();
            itemStat.descriptions.Add("Sprint Healing: ");
            itemStat.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            itemStat.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            itemStat.calculateValues = stackCount =>
            {
                return new List<float>([.02f * stackCount]);
            };
            allItemDefinitions.Add((int)DLC1Content.Items.MushroomVoid.itemIndex, itemStat);

        }
    }
}
