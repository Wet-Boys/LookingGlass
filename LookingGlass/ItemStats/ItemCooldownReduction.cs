using LookingGlass.ItemStatsNameSpace;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace LookingGlass.ItemStatsNameSpace
{
    internal class ItemCooldownReduction
    {
        private static readonly Dictionary<int, int> SkillCooldownReduction = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> SkillReductionValueIndex = new Dictionary<int, int>();

        public static int GetItemTargetSkill(int itemIndex)
        {
            return SkillCooldownReduction.TryGetValue(itemIndex, out var value) ? value : 0;
        }
        public static int GetReductionValueIndex(int itemIndex)
        {
            return SkillReductionValueIndex.TryGetValue(itemIndex, out var value) ? value : 0;
        }

        public static bool hasSkillCooldown(int itemIndex)
        {
            return SkillCooldownReduction.ContainsKey(itemIndex);
        }

        static ItemCooldownReduction()
        {
            //No longer used
            //Cooldown should be determined with GenericSkill or EquipmentSlot

            SkillCooldownReduction.Add((int)RoR2Content.Items.AlienHead.itemIndex, (int)SkillSlot.None);
            SkillReductionValueIndex.Add((int)RoR2Content.Items.AlienHead.itemIndex, 0);

            SkillCooldownReduction.Add((int)RoR2Content.Items.UtilitySkillMagazine.itemIndex, (int)SkillSlot.Utility);
            SkillReductionValueIndex.Add((int)RoR2Content.Items.UtilitySkillMagazine.itemIndex, -1);

            SkillCooldownReduction.Add((int)DLC1Content.Items.HalfAttackSpeedHalfCooldowns.itemIndex, (int)SkillSlot.None);
            SkillReductionValueIndex.Add((int)DLC1Content.Items.HalfAttackSpeedHalfCooldowns.itemIndex, 0);

            SkillCooldownReduction.Add((int)RoR2Content.Items.LunarBadLuck.itemIndex, (int)SkillSlot.None);
            SkillReductionValueIndex.Add((int)RoR2Content.Items.LunarBadLuck.itemIndex, 0);

            SkillCooldownReduction.Add((int)DLC1Content.Items.EquipmentMagazineVoid.itemIndex, (int)SkillSlot.Special);
            SkillReductionValueIndex.Add((int)DLC1Content.Items.EquipmentMagazineVoid.itemIndex, -1);
        }
    }
}
