using RoR2;
using System;
using System.Collections.Generic;

namespace LookingGlass.ItemStatsNameSpace
{
    public class ItemStatsDef
    {
        //Much thanks to https://github.com/ontrigger/ItemStatsMod for being a great guidline for setting item calculations up
        public enum ValueType
        {
            Healing,
            Damage,
            Utility,
            Health,
            Void,
            HumanObjective,
            LunarObjective,
            Stack,
            WorldEvent,
            Artifact,
            UserSetting,
            Death,
            Sub,
            Mono,
            Shrine,
            Event,
            Gold,
            Armor,
            None,
        }
        public enum MeasurementUnits
        {
            Meters,
            Percentage,
            FlatHealth,
            FlatHealing,
            Number,
            Money,
            Seconds,
            PercentHealth,
            PercentHealing,
            ProcCoeff, //0.0##
            PlainString
        }
        public enum ChanceScaling
        {
            Linear,
            Hyperbolic,
            DoesNotScale,
            RunicLens, //Linear, capped at 75%
            Health //BetterUI had LeechSeed/Scythe here so idk?
        }
        public List<string> descriptions = new List<string>();
        public List<ValueType> valueTypes = new List<ValueType>();
        public List<MeasurementUnits> measurementUnits = new List<MeasurementUnits>();
        public Func<CharacterMaster, int, List<float>> calculateValues = null;
        public Func<float, int, float, List<float>> calculateValuesNew = null;
        public bool hasChance = false;
        public ChanceScaling chanceScaling = ChanceScaling.Linear;
        //public ValueType chanceValueType = ValueType.Linear;

        public ItemStatsDef(List<string> descriptions, List<ValueType> valueTypes, List<MeasurementUnits> measurementUnits, Func<CharacterMaster, int, List<float>> calculateValues)
        {
            this.descriptions = descriptions;
            this.valueTypes = valueTypes;
            this.measurementUnits = measurementUnits;
            this.calculateValues = calculateValues;
        }
        public ItemStatsDef(List<string> descriptions, List<ValueType> valueTypes, List<MeasurementUnits> measurementUnits, Func<float, int, float, List<float>> calculateValues)
        {
            this.descriptions = descriptions;
            this.valueTypes = valueTypes;
            this.measurementUnits = measurementUnits;
            this.calculateValuesNew = calculateValues;
        }
        public ItemStatsDef() { }
    }
}
