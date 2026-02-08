using static LookingGlass.AutoSortItems.AutoSortItemsClass;
using ItemQualities;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using HG;

namespace LookingGlass.AutoSortItems
{
    internal static class ItemQualitiesInterop
    {
        /// <summary>
        /// Effectively part of <see cref="AutoSortItemsClass.SortItems"/>, but split off because
        /// references to soft dependencies need to be in their own methods.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static void HandleQualityItems(ItemIndex itemIndex, ItemInventoryDisplay display,
            bool sortByStackSize, bool sortByAcquired, Dictionary<ItemIndex, int> acquiredOrder, 
            ref int stackSizeKey, ref int itemIndexKey, ref int qualityKey)
        {
            ItemQualityGroupIndex groupIndex = QualityCatalog.FindItemQualityGroupIndex(itemIndex);
            if (groupIndex == ItemQualityGroupIndex.Invalid)
            {
                return;
            }

            ItemQualityGroup group = QualityCatalog.GetItemQualityGroup(groupIndex);

            QualityTier qualityTier = QualityCatalog.GetQualityTier(itemIndex);
            if (SortQualityItems.Value == QualitySortType.Separated && QualityItemsOrder.Value == QualitySortOrder.Off)
            {
                // weird case, just group together all quality items to give somewhat sensible behavior
                qualityKey = (qualityTier > QualityTier.None) ? 1 : -1;
            }
            else if (QualityItemsOrder.Value != QualitySortOrder.Off)
            {
                qualityKey = (int)qualityTier;
            }

            if (SortQualityItems.Value == QualitySortType.Grouped)
            {
                // you might look at these and think "surely there is a better way to do this"
                // as far as i can tell, there is not
                if (sortByStackSize)
                {
                    stackSizeKey = ArrayUtils.GetSafe(display.itemStacks, (int)group.BaseItemIndex)
                        + ArrayUtils.GetSafe(display.itemStacks, (int)group.UncommonItemIndex)
                        + ArrayUtils.GetSafe(display.itemStacks, (int)group.RareItemIndex)
                        + ArrayUtils.GetSafe(display.itemStacks, (int)group.EpicItemIndex)
                        + ArrayUtils.GetSafe(display.itemStacks, (int)group.LegendaryItemIndex);
                }

                if (sortByAcquired)
                {
                    // grabbing all of them and taking the min is necessary to preserve acquisition order
                    // (we can't just assume you got the base version first, for instance)
                    int[] itemOrders =
                    [
                        acquiredOrder.GetValueOrDefault(group.BaseItemIndex, int.MaxValue),
                        acquiredOrder.GetValueOrDefault(group.UncommonItemIndex, int.MaxValue),
                        acquiredOrder.GetValueOrDefault(group.RareItemIndex, int.MaxValue),
                        acquiredOrder.GetValueOrDefault(group.EpicItemIndex, int.MaxValue),
                        acquiredOrder.GetValueOrDefault(group.LegendaryItemIndex, int.MaxValue),
                    ];
                    itemIndexKey = itemOrders.Min();
                }
                else
                {
                    itemIndexKey = (int)group.BaseItemIndex;
                }
            }
            // nothing else needs to be done for the Separated case, that's handled by the main method
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static bool IsConsumedRegenScrap(ItemIndex itemIndex)
        {
            ItemQualityGroupIndex groupIndex = QualityCatalog.FindItemQualityGroupIndex(itemIndex);
            if (groupIndex == ItemQualityGroupIndex.Invalid)
            {
                return false;
            }

            ItemQualityGroup group = QualityCatalog.GetItemQualityGroup(groupIndex);
            return group.BaseItemIndex == DLC1Content.Items.RegeneratingScrapConsumed.itemIndex;
        }
    }
}
