using RoR2;
using System.Collections.Generic;

namespace LookingGlass.CommandItemCount
{
    internal enum CorruptionType
    {
        None,
        Corrupted, // item is corrupted by something else
        Void // this item is corrupting other items
    }

    internal class ItemCorruptionInfo
    {
        public CorruptionType Type;
        public List<ItemIndex> Items;
        public int ItemCount;
    }
}
