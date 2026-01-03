using BepInEx.Configuration;
using LookingGlass.Base;
using LookingGlass.BuffDescriptions;
using MonoMod.RuntimeDetour;
using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace LookingGlass
{
    public static class LGColorDictionary
    {   
        //Some sort of easier customizability for people who wish to change color values, or want very specific color changes
        //Without needing to remove the whole like automatic coloring
        //Tho would definitely be kind of annoying and rare for people to do I imagine.
        public enum ColorSource
        {
            Health,
            Shield,
            Barrier,
            Armor,

            Damage,
            Crit,
            AttackSpeed,
            Bleed,

            MovementSpeed,
            Jumps,
            Cooldown,

            AllStats,
            Gold,
            Downside,
            Void,
            WorldEvent,
          
        }
        public static string GetColor(ColorSource colorSource)
        {
            switch (colorSource)
            {
                case ColorSource.Health:
                    return "00FF00";

            }
            return "FF00FF";
        }
 
    }
}
