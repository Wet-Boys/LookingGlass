using BepInEx.Configuration;
using LookingGlass.Base;
using MonoMod.RuntimeDetour;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.ContentManagement;
using RoR2.UI;
using System;
using System.Reflection;
using UnityEngine;

namespace LookingGlass.HiddenItems
{
    internal class UnHiddenItems : BaseThing
    {
        public UnHiddenItems()
        {
            Setup();
            SetupRiskOfOptions();
        }
        private static Hook itemHook;
        private static Hook buffHook;
        public static ConfigEntry<bool> noHiddenItems;
        public static ConfigEntry<bool> noHiddenBuffs;
        public void Setup()
        {
            noHiddenItems = BasePlugin.instance.Config.Bind<bool>("Misc", "Unhide Internal Items", false, "Makes normally hidden items visible. (Drizzle/MonsoonHelper)");
            noHiddenBuffs = BasePlugin.instance.Config.Bind<bool>("Misc", "Unhide Internal Buffs", false, "Makes normally hidden buffs visible.");
            noHiddenItems.SettingChanged += SettingsChanged;
            noHiddenBuffs.SettingChanged += SettingsChanged;
        }

       
        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(noHiddenItems, new CheckBoxConfig() { restartRequired = false}));
            ModSettingsManager.AddOption(new CheckBoxOption(noHiddenBuffs, new CheckBoxConfig() { restartRequired = false}));
            ItemCatalog.availability.CallWhenAvailable(CallLate);   
        }
        private void CallLate()
        {
            //Doesnt work if called in Awake(), so ig call late.
            SettingsChanged(null, null);
        }

        private void SettingsChanged(object _, EventArgs __)
        {
            if (noHiddenItems.Value)
            {
                if (itemHook == null)
                {
                    var targetMethod = typeof(ItemInventoryDisplay).GetMethod(nameof(ItemInventoryDisplay.ItemIsVisible), BindingFlags.NonPublic | BindingFlags.Static);
                    itemHook = new Hook(targetMethod, ItemIsVisible);
                }
            }
            else
            {
                if (itemHook != null)
                {
                    itemHook.Undo();
                    itemHook = null;
                }
            }

            if (noHiddenBuffs.Value)
            {
                //Set buffSprite for all ones lacking one to the <!> one.
                foreach (BuffDef buffDef in BuffCatalog.buffDefs)
                {
                    if (buffDef.iconSprite == null)
                    {
                        buffDef.iconSprite = DLC2Content.Buffs.GeodeBuff.iconSprite;
                    }
                }
                if (buffHook == null)
                {
                    var targetMethod = typeof(BuffDisplay).GetMethod(nameof(BuffDisplay.AllocateIcons), BindingFlags.NonPublic | BindingFlags.Instance);
                    buffHook = new Hook(targetMethod, ShowAllBuffs);
                }
            }
            else
            {
                if (buffHook != null)
                {
                    buffHook.Undo();
                    buffHook = null;
                }
            }

        }


        void ShowAllBuffs(Action<BuffDisplay> orig, BuffDisplay self)
        {
            if (self.buffsReallocated)
            {
                return;
            }
            orig(self);
            if (self.source == null)
            {
                return;
            }
            self.PrepareToCullBuffDisplayData();
            int num = self.buffIconDisplayData.Count;
            int num2 = 0;
            int num3 = num;
            foreach (BuffDef buffDef in BuffCatalog.buffDefs)
            {
                BuffIndex buffIndex = buffDef.buffIndex;
                if (self.source.HasBuff(buffIndex))
                {
                    int buffCount = self.source.GetBuffCount(buffIndex);
                    while (num2 < num && self.buffIconDisplayData[num2].buffIndex < buffIndex)
                    {
                        num2++;
                    }
                    if (num2 < num)
                    {
                        BuffDisplay.BuffIconDisplayData buffIconDisplayData = self.buffIconDisplayData[num2];
                        if (buffIconDisplayData.buffIndex == buffIndex)
                        {
                            self.buffsReallocated |= (buffIconDisplayData.buffCount != buffCount);
                            buffIconDisplayData.removeIcon = false;
                            buffIconDisplayData.buffCount = buffCount;
                            num2++;
                        }
                        else
                        {
                            buffIconDisplayData = new BuffDisplay.BuffIconDisplayData(buffIndex);
                            buffIconDisplayData.buffCount = buffCount;
                            self.buffIconDisplayData.Insert(num2, buffIconDisplayData);
                            num2++;
                            num++;
                            self.buffsReallocated = true;
                        }
                    }
                    else
                    {
                        BuffDisplay.BuffIconDisplayData buffIconDisplayData = new BuffDisplay.BuffIconDisplayData(buffIndex);
                        buffIconDisplayData.buffCount = buffCount;
                        self.buffIconDisplayData.Add(buffIconDisplayData);
                        self.buffsReallocated = true;
                    }
                }
            }
            self.buffsReallocated |= (num3 != self.buffIconDisplayData.Count);
        }

        bool ItemIsVisible(Func<ItemIndex, bool> orig, ItemIndex item)
        {
            return true;
        }
    }
}
