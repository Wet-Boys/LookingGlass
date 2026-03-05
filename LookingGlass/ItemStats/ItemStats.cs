using BepInEx.Configuration;
using LookingGlass.Base;
using LookingGlass.StatsDisplay;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.Skills;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LookingGlass.ItemStatsNameSpace
{
    internal class ItemStats : BaseThing
    {
        public static ConfigEntry<bool> fullDescInHud;
        public static ConfigEntry<bool> itemStatsCalculations;
        public static ConfigEntry<bool> fullDescOnPickup;
        public static ConfigEntry<bool> itemStatsOnPing;
        public static ConfigEntry<bool> droneStatsOnPing;
        public static ConfigEntry<bool> StatsOnPingByOtherPlayer;
        public static ConfigEntry<bool> itemStatsShowHidden;
        public static ConfigEntry<float> itemStatsFontSize;
        public static ConfigEntry<bool> capChancePercentage;
        public static ConfigEntry<bool> abilityProcCoefficients;
        public static ConfigEntry<bool> cfgShowItemProcsOnSkillIcons;

        /*private static Hook overrideHook;
        private static Hook overrideHook2;
        private static Hook overrideHook3;
        private static Hook overrideHook4;*/
        public ItemStats()
        {
            Setup();
        }
        public void Setup()
        {
            InitHooks();
            ItemCatalog.availability.CallWhenAvailable(ItemDefinitions.RegisterAll);
            fullDescInHud = BasePlugin.instance.Config.Bind<bool>("Misc", "Full Description in Hud", true, "Shows full item descriptions on mouseover");
            itemStatsCalculations = BasePlugin.instance.Config.Bind<bool>("Misc", "Item Stats Calculations", true, "Gives calculations for vanilla items and modded items which have added specific support. (Sadly, items are not designed in a way to allow this to be automatic)");

            //Not a big fan, sometimes too much text to read at once.
            //Some items have pickup as flavor text and just check with tab if you need the full v
            fullDescOnPickup = BasePlugin.instance.Config.Bind<bool>("Misc", "Full Descriptions On Pickup", false, "Shows full item/equipment/drone descriptions on pickup or purchase");
            itemStatsOnPing = BasePlugin.instance.Config.Bind<bool>("Misc", "Item Stats On Ping", true, "Shows item descriptions when you ping an item in the world, shop or pinter");
            droneStatsOnPing = BasePlugin.instance.Config.Bind<bool>("Misc", "Drone Info On Ping", true, "Shows drone descriptions when you ping a drone in the world or shop");
            StatsOnPingByOtherPlayer = BasePlugin.instance.Config.Bind<bool>("Misc", "Stats On Ping By Other Player", false, "Shows item and drone descriptions when another player pings an item/drone in the world");
            itemStatsShowHidden = BasePlugin.instance.Config.Bind<bool>("Misc", "Stats on pinging hidden items", false, "Shows item descriptions for hidden items in Multishops.");
            //Why is there just a random cheating config in this mod lol.

            itemStatsFontSize = BasePlugin.instance.Config.Bind<float>("Misc", "Item Stats Font Size", 100f, "Changes the font size of item stats");
            capChancePercentage = BasePlugin.instance.Config.Bind<bool>("Misc", "Cap Chance Percentage", true, "Caps displayed chances at 100%. May interact weirdly with luck if turned off");
            abilityProcCoefficients = BasePlugin.instance.Config.Bind<bool>("Misc", "Ability Proc Coefficients", true, "Shows ability cooldowns.\nShow ability proc coefficients on supported survivors");
            cfgShowItemProcsOnSkillIcons = BasePlugin.instance.Config.Bind<bool>("Misc", "Ability Item Procs", true, "Shows item proc chances multiplied by the proc coefficient of the ability, in the abilities info box.");
            SetupRiskOfOptions();
        }
        public void SetupRiskOfOptions()
        {
            //Config that people are likelier to turn off should be higher up in Risk Menu
            ModSettingsManager.AddOption(new CheckBoxOption(fullDescInHud, new CheckBoxConfig() { category = "Item Info", restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(fullDescOnPickup, new CheckBoxConfig() { category = "Item Info", restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(itemStatsOnPing, new CheckBoxConfig() { category = "Item Info", name = "Item Info on ping", restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(droneStatsOnPing, new CheckBoxConfig() { category = "Item Info", name = "Drone Info on ping", restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(StatsOnPingByOtherPlayer, new CheckBoxConfig() { category = "Item Info", restartRequired = false }));

            ModSettingsManager.AddOption(new CheckBoxOption(itemStatsCalculations, new CheckBoxConfig() { category = "Item Info", restartRequired = false, checkIfDisabled = ItemStatsDisabled }));
            ModSettingsManager.AddOption(new SliderOption(itemStatsFontSize, new SliderConfig() { category = "Item Info", restartRequired = false, min = 1, max = 300 }));
            ModSettingsManager.AddOption(new CheckBoxOption(capChancePercentage, new CheckBoxConfig() { category = "Item Info", restartRequired = false }));

            ModSettingsManager.AddOption(new CheckBoxOption(abilityProcCoefficients, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(cfgShowItemProcsOnSkillIcons, new CheckBoxConfig() { restartRequired = false, checkIfDisabled = AbilityProcsEnabled }));

            ModSettingsManager.AddOption(new CheckBoxOption(itemStatsShowHidden, new CheckBoxConfig() { category = "Item Info", restartRequired = false }));

        }
        private static bool ItemStatsDisabled()
        {
            return !fullDescInHud.Value;
        }
        private static bool AbilityProcsEnabled()
        {
            return !abilityProcCoefficients.Value;
        }
        void InitHooks()
        {
            //Full Desc on pickup Config
            new Hook(typeof(GenericNotification).GetMethod(nameof(GenericNotification.SetItem), BindingFlags.Public | BindingFlags.Instance),
                    typeof(ItemStats).GetMethod(nameof(Item_PickupText), BindingFlags.NonPublic | BindingFlags.Instance),
                    this);

            new Hook(typeof(GenericNotification).GetMethod(nameof(GenericNotification.SetEquipment), BindingFlags.Public | BindingFlags.Instance),
                     typeof(ItemStats).GetMethod(nameof(Equipment_PickupText), BindingFlags.NonPublic | BindingFlags.Instance),
                     this);

            new Hook(typeof(GenericNotification).GetMethod(nameof(GenericNotification.SetDrone), BindingFlags.Public | BindingFlags.Instance),
                                typeof(ItemStats).GetMethod(nameof(Drone_PickupText), BindingFlags.NonPublic | BindingFlags.Instance),
                                this);


            //Actual things
            var targetMethod = typeof(ItemIcon).GetMethod(nameof(ItemIcon.SetItemIndex), new[] { typeof(ItemIndex), typeof(int), typeof(float) });
            var destMethod = typeof(ItemStats).GetMethod(nameof(ItemIcon_FullDescriptionAndStats), BindingFlags.NonPublic | BindingFlags.Instance);
            new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(PingerController).GetMethod(nameof(PingerController.SetCurrentPing), BindingFlags.NonPublic | BindingFlags.Instance);
            destMethod = typeof(ItemStats).GetMethod(nameof(ItemPinged), BindingFlags.NonPublic | BindingFlags.Instance);
            new Hook(targetMethod, destMethod, this);






            new ILHook(
                 typeof(LoadoutPanelController.Row).GetMethod(nameof(LoadoutPanelController.Row.FromSkillSlot), BindingFlags.Public | BindingFlags.Static),
                 AddProcCDToLoadoutPanel);

            new Hook(typeof(HUD).GetMethod(nameof(HUD.ActivateScoreboard), BindingFlags.Public | BindingFlags.Instance),
                  UpdateAllHUDIconsWhenScoreboard);

            new Hook(typeof(ScoreboardStrip).GetMethod(nameof(ScoreboardStrip.SetMaster), BindingFlags.Public | BindingFlags.Instance),
                 UpdateScoreboardEquipIcon);

            new Hook(typeof(GameEndReportPanelController).GetMethod(nameof(GameEndReportPanelController.SetPlayerInfo), BindingFlags.NonPublic | BindingFlags.Instance),
                 ItemsOnDeathScreen);
        }

        void ItemsOnDeathScreen(Action<GameEndReportPanelController, RunReport.PlayerInfo, int> orig, GameEndReportPanelController self, RunReport.PlayerInfo playerInfo, int playerIndex)
        {
            orig(self, playerInfo, playerIndex);
            if (fullDescInHud.Value)
            {
                for (int i = 0; i < self.itemInventoryDisplay.itemIcons.Count; i++)
                {
                    SetItemDescription(self.itemInventoryDisplay.itemIcons[i]);
                }
            }
        }



        //You can't select the icons in any other way can you?
        //And because you override, we really *do Not* need to update it every single frame
        //We can just update it when the player would see it
        void UpdateAllHUDIconsWhenScoreboard(Action<HUD> orig, HUD self)
        {
            orig(self);
            //Debug.Log("SCOREBOARD");

            if (fullDescInHud.Value)
            {
                for (int i = 0; i < self.itemInventoryDisplay.itemIcons.Count; i++)
                {
                    SetItemDescription(self.itemInventoryDisplay.itemIcons[i]);
                }
                for (int i = 0; i < self.equipmentIcons.Length; i++)
                {
                    EquipmentIcon_AddCDProcInfo(self.equipmentIcons[i]);
                }
            }
            if (abilityProcCoefficients.Value)
            {
                for (int i = 0; i < self.skillIcons.Length; i++)
                {
                    SkillIcon_SkillInfoCDProc(self.skillIcons[i]);
                }
            }
        }

        void UpdateScoreboardEquipIcon(Action<ScoreboardStrip, CharacterMaster> orig, ScoreboardStrip self, CharacterMaster master)
        {
            orig(self, master);
            if (fullDescInHud.Value)
            {
                for (int i = 0; i < self.itemInventoryDisplay.itemIcons.Count; i++)
                {
                    SetItemDescription(self.itemInventoryDisplay.itemIcons[i]);
                }
                EquipmentIcon_AddCDProcInfo(self.equipmentIcon);
            }
        }

        public void SkillIcon_SkillInfoCDProc(SkillIcon self)
        {
            GenericSkill targetSkill = self.targetSkill;
            if (targetSkill == null)
                return;
            if (targetSkill.skillDescriptionToken == null)
                return;
            if (self.tooltipProvider == null)
                return;
            StringBuilder desc = new StringBuilder(Language.GetString(targetSkill.skillDescriptionToken));

            if (abilityProcCoefficients.Value)
            {
                //Why was there a "In Proc Dict" check for this?
                //Maybe could do if cooldown == 0 then dont show but it's fine
                var cooldown = CalculateSkillCooldown(targetSkill);
                desc.Append("\n\nSkill Cooldown: <style=\"cIsUtility\">" + cooldown.ToString("0.00") + "s</style>");

                if (cooldown != targetSkill.skillDef.baseRechargeInterval)
                {
                    //If final recharge differs from base, show base spereately?
                    String cooldownReductionFormatted = ((1 - (self.targetSkill.finalRechargeInterval / self.targetSkill.skillDef.baseRechargeInterval)) * 100).ToString(StatsDisplayDefinitions.floatPrecision);
                    //String itemBasedCDRFormatted = ((1 - self.targetSkill.cooldownScale) * 100).ToString(StatsDisplayDefinitions.floatPrecision);

                    desc.Append(" <style=\"cStack\">(Base: " + self.targetSkill.skillDef.baseRechargeInterval + ")</style>");
                    desc.Append($"\nCooldown Reduction: <style=\"cIsUtility>{cooldownReductionFormatted}%</style>");
                }

                bool blacklistedSkill = false;
                if (ProcCoefficientData.hasProcCoefficient(targetSkill.skillNameToken))
                {
                    blacklistedSkill = ProcCoefficientData.GetProcCoefficient(targetSkill.skillNameToken) == -1;
                    //This way we could blacklist procs on most movement skills.
                    //So it doesn't say like "10% ATG" on Mando Slide
                    if (!blacklistedSkill)
                    {
                        desc.Append("\nProc Coefficient: <style=cIsDamage>" + ProcCoefficientData.GetProcCoefficient(targetSkill.skillNameToken).ToString("0.0##") + "</color>");
                    }
                    //If -1, show nothing
                    //If 0, show that it has 0 Proc Coeff for clarity
                    //But don't show like "0 % chance to trigger all the items"
                    blacklistedSkill = blacklistedSkill || ProcCoefficientData.GetProcCoefficient(targetSkill.skillNameToken) == 0;
                }
                if (ProcCoefficientData.hasExtra(self.targetSkill.skillNameToken))
                {
                    //Extra info like Corrupted/Boosted proc and Ticks
                    desc.Append(ProcCoefficientData.GetExtraInfo(self.targetSkill.skillNameToken));
                }
                if (cfgShowItemProcsOnSkillIcons.Value)
                {
                    if (!blacklistedSkill)
                    {
                        CharacterBody body = targetSkill.characterBody;

                        int itemCount = 0;
                        ItemStatsDef itemStats;

                        //Dont check AllItemDefs and if inDict
                        //Just use the Dict, so it's also nicely sorted by tier.
                        foreach (var keypairValue in ItemDefinitions.allItemDefinitions)
                        {
                            itemCount = body.inventory.GetItemCountEffective((ItemIndex)keypairValue.Key);
                            if (itemCount > 0)
                            {
                                itemStats = keypairValue.Value;
                                if (itemStats.hasChance) //hasProc
                                {
                                    bool healing = itemStats.chanceScaling == ItemStatsDef.ChanceScaling.Health;
                                    desc.Append("\n  ").Append(Language.GetString(ItemCatalog.GetItemDef((ItemIndex)keypairValue.Key).nameToken));
                                    desc.Append(healing ? ": <style=cIsHealing>" : ": <style=cIsDamage>");

                                    if (healing)
                                    {
                                        //IDK BetterUI showed this and like behemoth ig?
                                        desc.Append((itemStats.calculateValuesNew(body.master.luck, itemCount, ProcCoefficientData.GetProcCoefficient(self.targetSkill.skillNameToken))[0]).ToString("0.#")).Append(" HP</style>");
                                    }
                                    else
                                    {
                                        desc.Append((itemStats.calculateValuesNew(body.master.luck, itemCount, ProcCoefficientData.GetProcCoefficient(self.targetSkill.skillNameToken))[0] * 100).ToString("0.#")).Append("%</style>");
                                    }


                                    //Math Ceil leads to 201 lost seers, 26 runic
                                    //No math leads to 9 on tri tip
                                    //What is this bro
                                    //Thank you UnityEngine.Mathf for actually working
                                    if (itemStats.chanceScaling == ItemStatsDef.ChanceScaling.Linear)
                                    {
                                        desc.Append(" <style=cStack>(");
                                        desc.Append(Mathf.CeilToInt(1 / itemStats.calculateValuesNew(0f, 1, ProcCoefficientData.GetProcCoefficient(self.targetSkill.skillNameToken))[0]));
                                        desc.Append(" to cap)</style>");
                                    }
                                    else if (itemStats.chanceScaling == ItemStatsDef.ChanceScaling.RunicLens)
                                    {
                                        //Most ideally would calculated the chance with the min damage of the skill and add it to the chance or whatever
                                        //But that's not really possible
                                        desc.Append(" <style=cStack>(");
                                        desc.Append(Mathf.CeilToInt(0.75f / itemStats.calculateValuesNew(0f, 1, ProcCoefficientData.GetProcCoefficient(self.targetSkill.skillNameToken))[0]));
                                        desc.Append(" to cap)</style>");
                                    }

                                }
                            }

                        }
                    }
                }
            }
            self.tooltipProvider.overrideBodyText = desc.ToString();
        }

        public void AddProcCDToLoadoutPanel(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld("RoR2.Skills.SkillDef", "skillDescriptionToken")))
            {
                //I cant figuer out how to not use remove here so whatever
                c.Remove();

                //c.Emit(OpCodes.Ldloc, 7);
                c.Emit(OpCodes.Ldloc, 2);
                c.EmitDelegate<Func<SkillDef, string, string>>((skill, loadoutCat) =>
                {
                    //If in, passive slot,uhh dont
                    //if (!skill.skillNameToken.Contains("PASSIVE"))
                    if (!(loadoutCat == "LOADOUT_SKILL_MISC" || loadoutCat == "LOADOUT_DRONE"))
                    {
                        if (abilityProcCoefficients.Value)
                        {
                            StringBuilder newDesc = new StringBuilder(Language.GetString(skill.skillDescriptionToken));
                            newDesc.Append("\n\nSkill Cooldown: <style=\"cIsUtility\">" + skill.baseRechargeInterval.ToString("0.00") + "s</style>");
                            if (ProcCoefficientData.hasProcCoefficient(skill.skillNameToken))
                            {
                                float proc = ProcCoefficientData.GetProcCoefficient(skill.skillNameToken);
                                if (proc != -1)
                                {
                                    newDesc.Append("\nProc Coefficient: <style=cIsDamage>" + proc.ToString("0.0##") + "</color>");
                                }
                            }
                            if (ProcCoefficientData.hasExtra(skill.skillNameToken))
                            {
                                //Extra info like Corrupted/Boosted proc and Ticks
                                newDesc.Append(ProcCoefficientData.GetExtraInfo(skill.skillNameToken));
                            }

                            return newDesc.ToString();
                        }
                    }
                    return skill.skillDescriptionToken;
                });
            }
            else
            {
                Debug.LogError("IL FAILED : AddProcCDToLoadoutPanel");
            }
        }


        void ItemIcon_FullDescriptionAndStats(Action<ItemIcon, ItemIndex, int, float> orig, ItemIcon self, ItemIndex newItemIndex, int newItemCount, float newDurationPercent)
        {
            orig(self, newItemIndex, newItemCount, newDurationPercent);
            if (StatsDisplayClass.scoreBoardOpen && fullDescInHud.Value)
            {
                SetItemDescription(self);
            }
        }

        internal void EquipmentIcon_AddCDProcInfo(EquipmentIcon self)
        {
            // Multiplayer compatibility

            if (self.targetInventory && self.tooltipProvider && self.currentDisplayData.equipmentDef)
            {
                if (self.currentDisplayData.equipmentDisabled)
                {
                    self.tooltipProvider.overrideBodyText = string.Empty;
                    return;
                }
                //Why did it do Master -> Body -> Inventory just do -> Inventory
                //CharacterMaster master = self.targetInventory.GetComponentInParent<CharacterMaster>();

                //Show if perma up time?
                //Show if perma up time WarHorn?
                //Show up time as %?

                float cooldownScale = self.targetInventory.CalculateEquipmentCooldownScale();
                StringBuilder desc = new StringBuilder(Language.GetString(self.currentDisplayData.equipmentDef.descriptionToken));

                String currentCooldownFormatted = (self.currentDisplayData.equipmentDef.cooldown * cooldownScale).ToString(StatsDisplayDefinitions.floatPrecision);
                desc.Append($"\n\nCooldown: <style=\"cIsUtility>{currentCooldownFormatted}s</style>");
                if (cooldownScale != 1)
                {
                    String cooldownReductionFormatted = ((1 - cooldownScale) * 100).ToString(StatsDisplayDefinitions.floatPrecision);
                    desc.Append(
                    $" <style=\"cStack\">(Base: " + self.currentDisplayData.equipmentDef.cooldown + ")</style>" +
                    $"\nCooldown Reduction: <style=\"cIsUtility>{cooldownReductionFormatted}%</style>"
                    );
                }
                desc.Append(GetEquipmentExtras(self.targetInventory.GetComponent<CharacterMaster>(), self.currentDisplayData.equipmentDef.equipmentIndex));

                self.tooltipProvider.overrideBodyText = desc.ToString();
            }

        }

        float CalculateSkillCooldown(GenericSkill self)
        {
            if (self.skillDef.baseRechargeInterval < 0.5f)
                return self.skillDef.baseRechargeInterval;

            //Post-SotS AttackSpeedCDR skills bypass the 0.5f cooldown cap
            //By modifying the .baseRechargeInterval
            if (self.baseRechargeInterval < 0.5f)
                return self.baseRechargeInterval;

            float calculated_skill_cooldown = self.baseRechargeInterval * self.cooldownScale - self.flatCooldownReduction;

            if (calculated_skill_cooldown < 0.5f)
                calculated_skill_cooldown = 0.5f;

            return calculated_skill_cooldown;
        }

        internal static void SetItemDescription(ItemIcon self)
        {

            var itemDef = ItemCatalog.GetItemDef(self.itemIndex);
            /* if (itemDef.nameToken == "ITEM_MYSTICSITEMS_MANUSCRIPT_NAME")
                 return;*/ //I dont think this is needed anymore...
            if (self.tooltipProvider != null && itemDef != null)
            {
                var strip = self.GetComponentInParent<ScoreboardStrip>();
                if (strip)
                {
                    self.tooltipProvider.overrideBodyText = GetItemDescription(itemDef, self.itemCount, strip.master, false);
                }
                else
                {
                    self.tooltipProvider.overrideBodyText = GetItemDescription(itemDef, self.itemCount, LocalUserManager.GetFirstLocalUser().cachedMaster, false);
                }
            }
        }
        public static string GetItemDescription(
            ItemDef itemDef, int newItemCount, CharacterMaster master, bool withOneMore, bool forceNew = false)
        {
            if (Language.IsTokenInvalid(itemDef.descriptionToken))
            {
                return Language.GetString(itemDef.pickupToken);
            }
            var itemDescription = $"<size={itemStatsFontSize.Value}%>{Language.GetString(itemDef.descriptionToken)}\n";
            try
            {
                if (itemStatsCalculations.Value && ItemDefinitions.allItemDefinitions.ContainsKey((int)itemDef.itemIndex))
                {
                    ItemStatsDef statsDef = ItemDefinitions.allItemDefinitions[(int)itemDef.itemIndex];


                    if (withOneMore && statsDef.descriptions.Count != 0)
                    {
                        if (newItemCount == 0 || forceNew)
                        {
                            itemDescription += $"\nWith this item, you will have:";
                        }
                        else
                        {
                            itemDescription += $"\nWith another stack, you will have:";
                        }
                        newItemCount++;
                    }

                    if (!statsDef.isScrap || master.inventory.GetItemCountEffective(DLC3Content.Items.StatsFromScrap) > 0)
                    {
                        List<float> values;
                        if (statsDef.calculateValuesFlat != null)
                        {
                            values = statsDef.calculateValuesFlat(newItemCount);
                        }
                        else
                        {
                            if (statsDef.calculateValuesNew != null)
                            {
                                values = statsDef.calculateValuesNew(master ? master.luck : 0, newItemCount, 1f);
                            }
                            else if (statsDef.calculateValues != null)
                            {
                                values = statsDef.calculateValues(master, newItemCount);
                            }
                            else
                            {
                                values = statsDef.calculateValuesBody(master ? master.GetBody() : LocalUserManager.GetFirstLocalUser().cachedBody, newItemCount);
                            }
                        }

                        if (values != null)
                        {
                            GetItemStatsFormatted(ref statsDef, ref values, ref itemDescription, true);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            itemDescription += "</size>";
            return itemDescription;
        }


        public static string GetStyled(ItemStatsDef.ValueType color)
        {
            switch (color)
            {
                case ItemStatsDef.ValueType.Healing:
                case ItemStatsDef.ValueType.Armor:
                    return "<style=cIsHealing";
                case ItemStatsDef.ValueType.Damage:
                    return "<style=cIsDamage";
                case ItemStatsDef.ValueType.Utility:
                    return "<style=cIsUtility";
                case ItemStatsDef.ValueType.Health:
                    return "<style=cIsHealth";
                case ItemStatsDef.ValueType.Void:
                    return "<style=cIsVoid";
                case ItemStatsDef.ValueType.Gold:
                case ItemStatsDef.ValueType.HumanObjective:
                    return "<style=cHumanObjective";
                case ItemStatsDef.ValueType.LunarObjective:
                    return "<style=cLunarObjective";
                case ItemStatsDef.ValueType.Stack:
                    return "<style=cStack";
                case ItemStatsDef.ValueType.WorldEvent:
                    return "<style=cWorldEvent";
                case ItemStatsDef.ValueType.Artifact:
                    return "<style=cArtifact";
                case ItemStatsDef.ValueType.UserSetting:
                    return "<style=cUserSetting";
                case ItemStatsDef.ValueType.Death:
                    return "<style=cDeath";
                case ItemStatsDef.ValueType.Sub:
                    return "<style=cSub";
                case ItemStatsDef.ValueType.Mono:
                    return "<style=cMono";
                case ItemStatsDef.ValueType.Shrine:
                    return "<style=cShrine";
                case ItemStatsDef.ValueType.Event:
                    return "<style=cEvent";
            }
            return "</style>";
        }

        //What does ref do pls help
        public static void GetItemStatsFormatted(ref ItemStatsDef statsDef, ref List<float> values, ref string input, bool white)
        {
            for (int i = 0; i < statsDef.descriptions.Count; i++)
            {
                if (white)
                {
                    input += $"\n<color=\"white\">{statsDef.descriptions[i]}</color>";
                }
                else
                {
                    input += $"\n{statsDef.descriptions[i]}";
                }
                switch (statsDef.valueTypes[i])
                {
                    case ItemStatsDef.ValueType.Healing:
                    case ItemStatsDef.ValueType.Armor:
                        input += "<style=\"cIsHealing";
                        break;
                    case ItemStatsDef.ValueType.Damage:
                        input += "<style=\"cIsDamage";
                        break;
                    case ItemStatsDef.ValueType.Utility:
                        input += "<style=\"cIsUtility";
                        break;
                    case ItemStatsDef.ValueType.Health:
                        input += "<style=\"cIsHealth";
                        break;
                    case ItemStatsDef.ValueType.Void:
                        input += "<style=\"cIsVoid";
                        break;
                    case ItemStatsDef.ValueType.Gold:
                    case ItemStatsDef.ValueType.HumanObjective:
                        input += "<style=\"cHumanObjective";
                        break;
                    case ItemStatsDef.ValueType.LunarObjective:
                        input += "<style=\"cLunarObjective";
                        break;
                    case ItemStatsDef.ValueType.Stack:
                        input += "<style=\"cStack";
                        break;
                    case ItemStatsDef.ValueType.WorldEvent:
                        input += "<style=\"cWorldEvent";
                        break;
                    case ItemStatsDef.ValueType.Artifact:
                        input += "<style=\"cArtifact";
                        break;
                    case ItemStatsDef.ValueType.UserSetting:
                        input += "<style=\"cUserSetting";
                        break;
                    case ItemStatsDef.ValueType.Death:
                        input += "<style=\"cDeath";
                        break;
                    case ItemStatsDef.ValueType.Sub:
                        input += "<style=\"cSub";
                        break;
                    case ItemStatsDef.ValueType.Mono:
                        input += "<style=\"cMono";
                        break;
                    case ItemStatsDef.ValueType.Shrine:
                        input += "<style=\"cShrine";
                        break;
                    case ItemStatsDef.ValueType.Event:
                        input += "<style=\"cEvent";
                        break;
                        //case ItemStatsDef.ValueType.Other:
                        //    itemDescription += "<color=\"white";
                        //    break;
                }
                //Probably aughta use float precisions config ig
                switch (statsDef.measurementUnits[i])
                {
                    case ItemStatsDef.MeasurementUnits.Meters:
                        input += $"\">{values[i]:0.##}m</style>";
                        break;
                    case ItemStatsDef.MeasurementUnits.Percentage:
                        input += $"\">{values[i] * 100:0.##}%</style>";
                        break;
                    case ItemStatsDef.MeasurementUnits.FlatHealth:
                        input += $"\">{values[i]:0.##} HP</style>";
                        break;
                    case ItemStatsDef.MeasurementUnits.PercentHealth:
                        input += $"\">{values[i] * 100:0.##}% HP</style>";
                        break;
                    case ItemStatsDef.MeasurementUnits.FlatHealing:
                        input += $"\">{values[i]:0.##} HP/s</style>";
                        break;
                    case ItemStatsDef.MeasurementUnits.PercentHealing:
                        input += $"\">{values[i] * 100:0.##}% HP/s</style>";
                        break;
                    case ItemStatsDef.MeasurementUnits.Number:
                        input += $"\">{values[i]:0.##}</style>";
                        break;
                    case ItemStatsDef.MeasurementUnits.ProcCoeff:
                        input += $"\">{values[i]:0.0##}</style>";
                        break;
                    case ItemStatsDef.MeasurementUnits.Money:
                        input += $"\">{values[i]:0.#}$</style>";
                        break;
                    case ItemStatsDef.MeasurementUnits.Seconds:
                        input += $"\">{values[i]:0.##} seconds</style>";
                        break;
                    default:
                        break;
                }
            }
        }

        public static string GetEquipmentExtras(CharacterMaster master, EquipmentIndex equipmentIndex)
        {
            //Put the formatting values in it's own method, hopefully it doesn't create issues.
            string equipmentExtraInfo = string.Empty;
            try
            {
                if (itemStatsCalculations.Value && ItemDefinitions.allEquipmentDefinitions.ContainsKey((int)equipmentIndex))
                {
                    ItemStatsDef statsDef = ItemDefinitions.allEquipmentDefinitions[(int)equipmentIndex];
                    List<float> values;

                    float luck = 0f;
                    if (master != null)
                    {
                        luck = master.luck;
                    }
                    if (statsDef.calculateValuesFlat == null)
                    {
                        values = statsDef.calculateValuesNew(luck, 1, 1f);
                    }
                    else
                    {
                        values = statsDef.calculateValuesFlat(0);
                    }
                    if (values is not null)
                    {
                        GetItemStatsFormatted(ref statsDef, ref values, ref equipmentExtraInfo, false);
                    }
                }
            }
            catch (Exception)
            {
            }
            return equipmentExtraInfo;
        }




        void Item_PickupText(Action<GenericNotification, ItemDef> orig, GenericNotification self, ItemDef itemDef)
        {
            orig(self, itemDef);
            if (fullDescOnPickup.Value)
                if (Language.IsTokenInvalid(itemDef.descriptionToken))
                {
                    self.descriptionText.token = itemDef.pickupToken;
                }
                else
                {
                    self.descriptionText.token = itemDef.descriptionToken;
                }
        }
        void Equipment_PickupText(Action<GenericNotification, EquipmentDef> orig, GenericNotification self, EquipmentDef equipmentDef)
        {
            orig(self, equipmentDef);
            if (fullDescOnPickup.Value)
            {
                if (Language.IsTokenInvalid(equipmentDef.descriptionToken))
                {
                    self.descriptionText.token = equipmentDef.pickupToken;
                }
                else
                {
                    self.descriptionText.token = equipmentDef.descriptionToken;
                }
            }
        }
        void Drone_PickupText(Action<GenericNotification, DroneDef> orig, GenericNotification self, DroneDef droneDef)
        {
            orig(self, droneDef);
            if (fullDescOnPickup.Value)
            {
                if (Language.IsTokenInvalid(droneDef.skillDescriptionToken))
                {
                    self.descriptionText.token = droneDef.pickupToken;
                }
                else
                {
                    self.descriptionText.token = droneDef.skillDescriptionToken;
                }
            }
        }



        //Heavily uses https://github.com/Moffein/ItemStats/blob/master/ItemStats/ItemStatsPlugin.cs
        void ItemPinged(Action<PingerController, PingerController.PingInfo> orig, PingerController self, PingerController.PingInfo newPingInfo)
        {
            orig(self, newPingInfo);
            if (!newPingInfo.targetGameObject)
                return;
            if (!itemStatsOnPing.Value && !droneStatsOnPing.Value)
                return;
            CharacterMaster characterMaster = LocalUserManager.GetFirstLocalUser().cachedMaster;
            if (!characterMaster || !StatsOnPingByOtherPlayer.Value && self.gameObject.GetComponent<CharacterMaster>() != characterMaster)
            {
                return;
            }


            //Item
            //Drone
            //Shop (Includes Printers)
            //DroneShop
            //TempShop


            PickupIndex pickupIndex = PickupIndex.none;
            int droneTier = 0;
            bool isTemp = false;
            if (newPingInfo.targetGameObject.TryGetComponent<GenericPickupController>(out var Item))
            {
                if (!itemStatsOnPing.Value)
                {
                    return;
                }
                pickupIndex = Item._pickupState.pickupIndex;
                isTemp = Item._pickupState.isTempItem;
                droneTier = Item._pickupState.upgradeValue;
            }
            else if (newPingInfo.targetGameObject.TryGetComponent<DroneAvailability>(out var Drone))
            {
                if (!droneStatsOnPing.Value)
                {
                    return;
                }
                pickupIndex = PickupCatalog.FindPickupIndex(Drone.droneDef.droneIndex);
                droneTier = newPingInfo.targetGameObject.GetComponent<SummonMasterBehavior>().droneUpgradeCount;
            }
            else if (newPingInfo.targetGameObject.TryGetComponent<ShopTerminalBehavior>(out var Shop))
            {
                if (!itemStatsOnPing.Value)
                {
                    return;
                }
                //Check for pickupDisplay because Cleansing Pools are not set to Hidden for some reason
                //But they show up as ? because they dont have a display
                if (itemStatsShowHidden.Value || (!Shop.hidden && Shop.pickupDisplay))
                {
                    pickupIndex = Shop.pickup.pickupIndex;
                }
            }
            else if (newPingInfo.targetGameObject.TryGetComponent<DroneVendorTerminalBehavior>(out var DroneShop))
            {
                if (!droneStatsOnPing.Value)
                {
                    return;
                }
                if (!DroneShop.hidden)
                {
                    pickupIndex = DroneShop.CurrentPickupIndex;
                }
            }
            else if (newPingInfo.targetGameObject.TryGetComponent<PickupDistributorBehavior>(out var TempShop))
            {
                if (!itemStatsOnPing.Value)
                {
                    return;
                }
                if (!TempShop.hidden)
                {
                    pickupIndex = TempShop.pickup.pickupIndex;
                    isTemp = TempShop.tempPickups;
                }
            }
            if (pickupIndex != PickupIndex.none)
            {
                PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
                //Filter out LunarCoins and other weird pickups without notifications
                if (pickupDef.itemIndex != ItemIndex.None || pickupDef.equipmentIndex != EquipmentIndex.None || pickupDef.droneIndex != DroneIndex.None)
                {
                    CharacterMasterNotificationQueue.PushPickupNotification(characterMaster, pickupIndex, isTemp, droneTier);
                    PutLastNotificationFirst(characterMaster);
                }
            }
        }

        internal void PutLastNotificationFirst(CharacterMaster characterMaster, float durationOverride = 5f)
        {


            //If duration needs to be modified, do here
            CharacterMasterNotificationQueue notificationQueueForMaster = CharacterMasterNotificationQueue.GetNotificationQueueForMaster(characterMaster);
            var newNotification = notificationQueueForMaster.notifications.Last();
            newNotification.duration = durationOverride;
            if (notificationQueueForMaster.notifications.Count > 1)
            {
                notificationQueueForMaster.notifications[0].duration = .01f;
                if (notificationQueueForMaster.notifications.Count > 2)
                {
                    notificationQueueForMaster.notifications.Remove(newNotification);
                    notificationQueueForMaster.notifications.Insert(1, newNotification);
                }
            }
        }
    }
}
