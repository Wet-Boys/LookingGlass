using System;
using System.Collections.Generic;
using System.Text;

namespace LookingGlass.ItemStatsNameSpace
{
    internal class ProcCoefficientData
    {
        //Shouldn't this be public so people can add to it??
        public static readonly Dictionary<string, float> skills = new Dictionary<string, float>();
        public static readonly Dictionary<string, string> skillsAdditional = new Dictionary<string, string>();

        public static float GetProcCoefficient(string name)
        {
            return skills.TryGetValue(name, out var value) ? value : 1f;
        }
        public static string GetExtraInfo(string name)
        {
            return skillsAdditional.TryGetValue(name, out var value) ? value : "";
        }
        public static bool hasProcCoefficient(string name)
        {
            return skills.ContainsKey(name);
        }
        public static bool hasExtra(string name)
        {
            return skillsAdditional.ContainsKey(name);
        }
        static ProcCoefficientData()
        {
            //Use -1 on movement & non damaging skills
            //This prevens proc data from being shown

            //Would be nice to have an alt dict with string
            //so Boosted, Corrupted, or other additional effects could be mentioned
            //But would prevent getting the float easily so dunno
            //Maybe just skills<float> (main) + skillsAditional<string> which goes uncalculated but still visible?
            //Very extra extra ig
            //Could also ig mention like Flamethrower/ArrowRain/RexAltM2 total ticks?
            //That could be neat

            // Acrid
            skills.Add("CROCO_PRIMARY_NAME", 1f);
            skills.Add("CROCO_SECONDARY_NAME", 1f);
            skills.Add("CROCO_SECONDARY_ALT_NAME", 1f);
            skills.Add("CROCO_UTILITY_NAME", 1f); // Leap: 1.0 Pool: 0.1
            skillsAdditional.Add("CROCO_UTILITY_NAME", " + <style=cIsDamage>0.1</style> Pool</style>"); //No \n
            skills.Add("CROCO_UTILITY_ALT1_NAME", 1f);
            skills.Add("CROCO_SPECIAL_NAME", 1f);

            // Artificer
            skills.Add("MAGE_PRIMARY_FIRE_NAME", 1f);
            skills.Add("MAGE_PRIMARY_LIGHTNING_NAME", 1f);
            skills.Add("MAGE_SECONDARY_ICE_NAME", 1f);
            skills.Add("MAGE_SECONDARY_LIGHTNING_NAME", 1f);
            skillsAdditional.Add("MAGE_SECONDARY_LIGHTNING_NAME", " + <style=cIsDamage>0.3</style> Tendrils</style>"); //No \n
            skills.Add("MAGE_UTILITY_ICE_NAME", 1f);
            skills.Add("MAGE_SPECIAL_FIRE_NAME", 1f);
            skillsAdditional.Add("MAGE_SPECIAL_FIRE_NAME", "\nTicks: <style=cIsDamage>22 * AttackSpeed</style>");
            skills.Add("MAGE_SPECIAL_LIGHTNING_NAME", 1f);

            // Bandit
            skills.Add("BANDIT2_PRIMARY_NAME", 0.5f);
            skills.Add("BANDIT2_SECONDARY_NAME", 1f);
            skills.Add("BANDIT2_PRIMARY_ALT_NAME", 1f);
            skills.Add("BANDIT2_SECONDARY_ALT_NAME", 1f);
            skills.Add("BANDIT2_UTILITY_NAME", 1f);
            skills.Add("BANDIT2_SPECIAL_NAME", 1f);
            skills.Add("BANDIT2_SPECIAL_ALT_NAME", 1f);

            // Capitan
            skills.Add("CAPTAIN_PRIMARY_NAME", 0.75f);
            skills.Add("CAPTAIN_SECONDARY_NAME", 1f);
            skills.Add("CAPTAIN_UTILITY_NAME", 1f);
            skills.Add("CAPTAIN_UTILITY_ALT1_NAME", 1f);
            skills.Add("CAPTAIN_SPECIAL_NAME", 0f);
            skills.Add("CAPTAIN_SUPPLY_HEAL_NAME", 0f);
            skills.Add("CAPTAIN_SUPPLY_HACKING_NAME", 0f);
            skills.Add("CAPTAIN_SUPPLY_EQUIPMENT_RESTOCK_NAME", 0f);
            skills.Add("CAPTAIN_SUPPLY_SHOCKING_NAME", 0f);
            skills.Add("CAPTAIN_SKILL_USED_UP_NAME", 0f);
            skills.Add("CAPTAIN_SKILL_DISCONNECT_NAME", -1f);

            // Commando
            skills.Add("COMMANDO_PRIMARY_NAME", 1f);
            skills.Add("COMMANDO_SECONDARY_NAME", 1f);
            skills.Add("COMMANDO_SECONDARY_ALT1_NAME", 0.5f);
            skills.Add("COMMANDO_UTILITY_NAME", -1f);
            skills.Add("COMMANDO_UTILITY_ALT_NAME", -1f);
            skills.Add("COMMANDO_SPECIAL_NAME", 1f);
            skills.Add("COMMANDO_SPECIAL_ALT1_NAME", 1f);

            // Engineer
            skills.Add("ENGI_PRIMARY_NAME", 1f);
            skills.Add("ENGI_SECONDARY_NAME", 1f);
            skills.Add("ENGI_SPIDERMINE_NAME", 1f);
            skills.Add("ENGI_UTILITY_NAME", -1f);
            skills.Add("ENGI_SKILL_HARPOON_NAME", 1f);
            skills.Add("ENGI_SPECIAL_NAME", 1f);
            skills.Add("ENGI_SPECIAL_ALT1_NAME", 0.6f);

            // Huntress
            skills.Add("HUNTRESS_PRIMARY_NAME", 1f);
            skills.Add("HUNTRESS_PRIMARY_ALT_NAME", 0.7f);
            skills.Add("HUNTRESS_SECONDARY_NAME", 0.8f);
            skills.Add("HUNTRESS_UTILITY_NAME", -1f);
            skills.Add("HUNTRESS_UTILITY_ALT1_NAME", -1f);
            skills.Add("HUNTRESS_SPECIAL_NAME", 0.2f);
            skillsAdditional.Add("HUNTRESS_SPECIAL_NAME", "\nTicks: <style=cIsDamage>19</style>");
            skills.Add("HUNTRESS_SPECIAL_ALT1_NAME", 1f);

            // Loader <3
            skills.Add("LOADER_PRIMARY_NAME", 1f);
            skills.Add("LOADER_SECONDARY_NAME", -1f);
            skills.Add("LOADER_YANKHOOK_NAME", 1f);
            skills.Add("LOADER_UTILITY_NAME", 1f);
            skills.Add("LOADER_UTILITY_ALT1_NAME", 1f);
            skills.Add("LOADER_SPECIAL_NAME", 0.5f);
            skills.Add("LOADER_SPECIAL_ALT_NAME", 1f);

            // Mercenary
            skills.Add("MERC_PRIMARY_NAME", 1f);
            skills.Add("MERC_SECONDARY_NAME", 1f);
            skills.Add("MERC_SECONDARY_ALT1_NAME", 1f);
            skills.Add("MERC_UTILITY_NAME", 1f);
            skills.Add("MERC_UTILITY_ALT1_NAME", 1f);
            skills.Add("MERC_SPECIAL_NAME", 1f);
            skills.Add("MERC_SPECIAL_ALT1_NAME", 1f);

            // MUL-T
            skills.Add("TOOLBOT_PRIMARY_NAME", 0.6f);
            skillsAdditional.Add("TOOLBOT_PRIMARY_NAME", "\nNails/s: <style=cIsDamage>12 * AttackSpeed</style>");
            skills.Add("TOOLBOT_PRIMARY_ALT1_NAME", 1f);
            skills.Add("TOOLBOT_PRIMARY_ALT2_NAME", 1f);
            skillsAdditional.Add("TOOLBOT_PRIMARY_ALT2_NAME", "\nBlast Radius: <style=cIsDamage>7m</style>");
            skills.Add("TOOLBOT_PRIMARY_ALT3_NAME", 1f);
            skills.Add("TOOLBOT_SECONDARY_NAME", 1f);
            skills.Add("TOOLBOT_UTILITY_NAME", 1f);
            skills.Add("TOOLBOT_SPECIAL_NAME", -1f);
            skills.Add("TOOLBOT_SPECIAL_ALT_NAME", -1f);
            skills.Add("TOOLBOT_SPECIAL_ALT_QUIT_NAME", -1f);

            // REX
            skills.Add("TREEBOT_PRIMARY_NAME", 0.5f);
            skills.Add("TREEBOT_SECONDARY_NAME", 1f);
            skills.Add("TREEBOT_SECONDARY_ALT1_NAME", 0.5f);
            skillsAdditional.Add("TREEBOT_SECONDARY_ALT1_NAME", "\nTicks: <style=cIsDamage>19</style>");
            skills.Add("TREEBOT_UTILITY_NAME", 0f);
            skills.Add("TREEBOT_UTILITY_ALT1_NAME", 0.5f);
            skills.Add("TREEBOT_SPECIAL_NAME", 1f);
            skills.Add("TREEBOT_SPECIAL_ALT1_NAME", 1f);

            // Heretic
            skills.Add("SKILL_LUNAR_PRIMARY_REPLACEMENT_NAME", 1f); //0.1 initial hit
            skillsAdditional.Add("SKILL_LUNAR_PRIMARY_REPLACEMENT_NAME", " + <style=cIsDamage>0.1</style> <style=cSub>Stick</style>");
            skills.Add("SKILL_LUNAR_SECONDARY_REPLACEMENT_NAME", 0.2f); //1 on explosion, 0.2 on rapid hits.
            skillsAdditional.Add("SKILL_LUNAR_SECONDARY_REPLACEMENT_NAME", " + <style=cIsDamage>1</style> <style=cSub>Explosion</style>");
            skills.Add("SKILL_LUNAR_UTILITY_REPLACEMENT_NAME", -1f);
            skills.Add("SKILL_LUNAR_SPECIAL_REPLACEMENT_NAME", 1f);
            skills.Add("HERETIC_DEFAULT_SKILL_NAME", -1f);

            #region DLC1
            //DLC1
            // Railgunner
            skills.Add("RAILGUNNER_PRIMARY_NAME", 1f);
            skills.Add("RAILGUNNER_SECONDARY_NAME", 1f);
            skills.Add("RAILGUNNER_SECONDARY_ALT_NAME", 1f);
            skills.Add("RAILGUNNER_UTILITY_NAME", 0f);
            skills.Add("RAILGUNNER_UTILITY_ALT_NAME", -1f);
            skills.Add("RAILGUNNER_SPECIAL_NAME", 3f);
            skills.Add("RAILGUNNER_SPECIAL_ALT_NAME", 1.5f);
            //The M1 replacements
            skills.Add("RAILGUNNER_SNIPE_HEAVY_NAME", 1f);
            skills.Add("RAILGUNNER_SNIPE_LIGHT_NAME", 1f);
            skills.Add("RAILGUNNER_SNIPE_SUPER_NAME", 3f);
            skills.Add("RAILGUNNER_SNIPE_CRYO_NAME", 1.5f);


            // Void Fiend
            // TODO differentiate between corrupted and normal
            // Corrupt skills do not have different name tokens
            skills.Add("VOIDSURVIVOR_PRIMARY_NAME", 1f);
            skillsAdditional.Add("VOIDSURVIVOR_PRIMARY_NAME", "\nCorrupted Proc: <style=cIsVoid>0.625</style>");
            skills.Add("VOIDSURVIVOR_SECONDARY_NAME", 1f);
            skillsAdditional.Add("VOIDSURVIVOR_SECONDARY_NAME", "\nCorrupted Proc: <style=cIsVoid>1.0</style>");
            skills.Add("VOIDSURVIVOR_UTILITY_NAME", -1f);
            skills.Add("VOIDSURVIVOR_SPECIAL_NAME", -1f);
            skills.Add("CORRUPTED_VOIDSURVIVOR_PRIMARY_NAME", 0.625f);
            skills.Add("CORRUPTED_VOIDSURVIVOR_SECONDARY_NAME", 1f);
            #endregion
            #region DLC2
            // Seeker
            skills.Add("SEEKER_PRIMARY_NAME", 1f);
            skills.Add("SEEKER_SECONDARY_NAME", 1f);
            skills.Add("SEEKER_SECONDARY_ALT1_NAME", 1f);
            skills.Add("SEEKER_UTILITY_NAME", 1f);
            skills.Add("SEEKER_UTILITY_ALT1_NAME", 1f);
            skills.Add("SEEKER_SPECIAL_NAME", 1f);
            skills.Add("SEEKER_SPECIAL_ALT1_NAME", 1f);

            // False son
            skills.Add("FALSESON_PRIMARY_NAME", 1f);
            skillsAdditional.Add("FALSESON_PRIMARY_NAME", "\nMid-Air Charged Proc: <style=cHumanObjective>1.5</style>"); 
            skills.Add("FALSESON_SECONDARY_NAME", 1f);
            skills.Add("FALSESON_SECONDARY_ALT1_NAME", 1f);
            skills.Add("FALSESON_UTILITY_NAME", 1f);
            skills.Add("FALSESON_UTILITY_ALT1_NAME", 0f);
            skills.Add("FALSESON_SPECIAL_NAME", 0.45f);
            skillsAdditional.Add("FALSESON_SPECIAL_NAME", "\nTicks: <style=cIsDamage>32</style>, Scales with <style=cIsHealing>Growth</style>"); //??
            skills.Add("FALSESON_SPECIAL_ALT1_NAME", 1f);

            // Chef
            //Boosted skills do not have unique name tokens (Do have unique desc tho)
            skills.Add("CHEF_PRIMARY_NAME", 1f);        //Cleaver Held / Boosted 1.5
            skills.Add("CHEF_SECONDARY_NAME", 1f);      //Boosted 1
            skills.Add("CHEF_SECONDARY_ALT_NAME", 1f);  //Boosted 1
            skills.Add("CHEF_UTILITY_NAME", 1f);        //Boosted 1
            skills.Add("CHEF_UTILITY_ALT_NAME", 1f);    //Boosted 1
            skills.Add("CHEF_SPECIAL_NAME", 1f);        //Oil Puddle 0.1
            skills.Add("CHEF_SPECIAL_ALT1_NAME", 1f);

            skillsAdditional.Add("CHEF_PRIMARY_NAME", "\nBoosted / Held Proc: <style=cHumanObjective>2.25</style>");
            skillsAdditional.Add("CHEF_SECONDARY_NAME", "\nBoosted Proc: <style=cHumanObjective>1.0</style>");
            skillsAdditional.Add("CHEF_SECONDARY_ALT_NAME", "\nBoosted Proc: <style=cHumanObjective>1.0</style>");
            skillsAdditional.Add("CHEF_UTILITY_NAME", "\nBoosted Proc: <style=cHumanObjective>1.0</style>");
            skillsAdditional.Add("CHEF_UTILITY_ALT_NAME", "\nBoosted Proc: <style=cHumanObjective>1.0</style>");
            skillsAdditional.Add("CHEF_SPECIAL_NAME", " + <style=cIsDamage>0.1</style> <style=cSub>Oil</style>");

            #endregion

            #region DLC3

            #endregion
        }
    }


}
