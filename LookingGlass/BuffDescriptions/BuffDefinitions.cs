using LookingGlass.LookingGlassLanguage;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using System.Xml.Linq;

namespace LookingGlass.BuffDescriptions
{
    internal static class BuffDefinitions
    {
        private static bool SetupAlready = false;

        internal static void SetupVanillaDefs()
        {
            if (SetupAlready)
            {
                return;
            }
            SetupAlready = true;
            string utilityString = "<style=\"cIsUtility>";
            string damageString = "<style=\"cIsDamage>";
            string healingString = "<style=\"cIsHealing>";
            string healthString = "<style=\"cIsHealth>";
            string voidString = "<style=\"cIsVoid>";
            string styleString = "</style>";


            LookingGlassLanguageAPI.SetupToken(Language.english, $"NAME_{RoR2Content.Buffs.AffixRed.buffIndex}", $"Blazing");
            LookingGlassLanguageAPI.SetupToken(Language.english, $"DESCRIPTION_{RoR2Content.Buffs.AffixRed.buffIndex}", $"Leave a fire trail that hurts enemies, and apply a 50% total damage {healthString}burn{styleString} on hit.");
            LookingGlassLanguageAPI.SetupToken(Language.english, $"NAME_{RoR2Content.Buffs.AffixHaunted.buffIndex}", $"Celestine");
            LookingGlassLanguageAPI.SetupToken(Language.english, $"DESCRIPTION_{RoR2Content.Buffs.AffixHaunted.buffIndex}", $"{utilityString}Cloak{styleString} nearby allies, and apply an 80% {utilityString}slow{styleString} on hit. ");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"NAME_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"DESCRIPTION_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"NAME_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"DESCRIPTION_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"NAME_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"DESCRIPTION_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"NAME_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"DESCRIPTION_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"NAME_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"DESCRIPTION_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"NAME_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"DESCRIPTION_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"NAME_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"DESCRIPTION_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"NAME_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"DESCRIPTION_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"NAME_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"DESCRIPTION_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"NAME_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"DESCRIPTION_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"NAME_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"DESCRIPTION_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"NAME_{RoR2Content.Buffs..buffIndex}", $"");
            //LookingGlassLanguageAPI.SetupToken(Language.english, $"DESCRIPTION_{RoR2Content.Buffs..buffIndex}", $"");
        }
    }
}
