using System;
using System.Collections.Generic;

namespace LookingGlass.ItemStatsNameSpace
{
    internal class ItemStatsDef
    {
        public enum ValueType
        {
            Healing,
            Damage,
            Utility,
            Health
        }
        public enum MeasurementUnits
        {
            Meters,
            Percentage,
            Health,
            Number,
            Seconds
        }
        public List<string> descriptions = new List<string>();
        public List<ValueType> valueTypes = new List<ValueType>();
        public List<MeasurementUnits> measurementUnits = new List<MeasurementUnits>();
        public Func<int, List<float>> CalculateValues;
    }
}
