using BepInEx.Configuration;
using LookingGlass.Base;
using LookingGlass.BuffDescriptions;
using MonoMod.RuntimeDetour;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LookingGlass.DPSMeterStuff
{
    internal class DPSMeter : BaseThing
    {
       
        public ulong killsThisStage = 0;
        public ulong killsThisRun = 0;
 
 
        public float damageDealtSincePeriod = 0;
        public float currentCombatDamage = 0;
        public static ConfigEntry<float> maxComboConfigEntry;
        public float maxCombo = 0;
        public float maxRunCombo = 0;


        public ulong currentComboKills = 0;
        public static ConfigEntry<ulong> maxKillComboConfigEntry;
        public ulong maxKillCombo = 0;
        public ulong maxRunKillCombo = 0;
        public static float DPS_MAX_TIME = 5;
        public float timer = DPS_MAX_TIME;


        public static ConfigEntry<bool> disableDPSMeter;
        public static ConfigEntry<bool> dpsCountMinions;
        public static ConfigEntry<float> dpsDuration;


        public DPSMeter()
        {
            Setup();
        }
        public void Setup()
        {
            disableDPSMeter = BasePlugin.instance.Config.Bind<bool>("Misc", "Disable Damage tracking", false, "Disables DPS & Combo & Kill tracking.\n\nIncase you are not interested in these features & stats and are looking for optimization.");
            dpsDuration = BasePlugin.instance.Config.Bind<float>("Misc", "DPS Tracking Duration", 5f, "Duration during which Combos & DPS is added together");
            dpsDuration.SettingChanged += DpsDuration_SettingChanged;
            DPS_MAX_TIME = dpsDuration.Value;
            SetupRiskOfOptions();
            if (disableDPSMeter.Value)
            {
                return;
            }

            maxComboConfigEntry = BasePlugin.instance.Config.Bind<float>("Stats", "Max Combo", 0, "What are you gonna do, cheat the number?");
            maxCombo = maxComboConfigEntry.Value;
            maxKillComboConfigEntry = BasePlugin.instance.Config.Bind<ulong>("Stats", "Max Kill Combo", 0, "What are you gonna do, cheat the number?");
            maxKillCombo = maxKillComboConfigEntry.Value;

            
 
            new Hook(typeof(GlobalEventManager).GetMethod(nameof(GlobalEventManager.ClientDamageNotified), BindingFlags.Public | BindingFlags.Static),
                TrackDamage);

            new Hook(typeof(Run).GetMethod(nameof(Run.OnEnable), BindingFlags.NonPublic | BindingFlags.Instance), 
                RunOnEnable);

            new Hook(typeof(Stage).GetMethod(nameof(Stage.PreStartClient), BindingFlags.Public | BindingFlags.Instance), 
                ResetOnStage);
        }

        private void DpsDuration_SettingChanged(object sender, EventArgs e)
        {
            DPS_MAX_TIME = dpsDuration.Value;
        }

        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new SliderOption(dpsDuration, new SliderConfig() { restartRequired = false, min = 3, max = 10f, FormatString = "{0:0}s" }));
            ModSettingsManager.AddOption(new CheckBoxOption(disableDPSMeter, new CheckBoxConfig() { restartRequired = true }));
           
        }

        void ResetOnStage(Action<Stage> orig, Stage self)
        {
            orig(self);
            killsThisStage = 0;
        }

        void RunOnEnable(Action<Run> orig, Run self)
        {
            orig(self);
            damageDealtSincePeriod = 0;
            maxRunCombo = 0;
            killsThisRun = 0;
            maxRunKillCombo = 0;
        }
        public HealthComponent latestKill;
        void TrackDamage(Action<DamageDealtMessage> orig, DamageDealtMessage damageDealtMessage)
        {
            orig(damageDealtMessage);
            try
            {
                if (damageDealtMessage.attacker == LocalUserManager.GetFirstLocalUser().cachedBodyObject)
                //if (damageDealtMessage.attacker == LocalUserManager.GetFirstLocalUser().cachedBodyObject || damageDealtMessage.attacker.TryGetComponent<CharacterBody>(out var attacker) || attacker.master.minionOwnership && attacker.master.minionOwnership.ownerMaster && LocalUserManager.GetFirstLocalUser().cachedMasterObject))
                {
             
                    damageDealtSincePeriod += damageDealtMessage.damage;
                    currentCombatDamage += damageDealtMessage.damage;
                    if (maxCombo < currentCombatDamage)
                    {
                        maxCombo = currentCombatDamage;
                    }
                    if (maxRunCombo < currentCombatDamage)
                    {
                        maxRunCombo = currentCombatDamage;
                    }
                    timer = DPS_MAX_TIME;
                    if (damageDealtMessage.victim)
                    {
                        HealthComponent victim = damageDealtMessage.victim.GetComponent<HealthComponent>();

                        //Like .alive can be a bit unreliable and count kills twice but like uhh
                        //So is this weird other check
                        //if (victim && (victim.combinedHealth + victim.barrier) - damageDealtMessage.damage <= 0)
                        if (victim && !victim.alive && latestKill != victim)
                        {
                            latestKill = victim;
                            currentComboKills++;
                            killsThisRun++;
                            killsThisStage++;
                            if (maxKillCombo < currentComboKills)
                            {
                                maxKillCombo = currentComboKills;
                            }
                            if (maxRunKillCombo < currentComboKills)
                            {
                                maxRunKillCombo = currentComboKills;
                            }
                        }
                    }
                    Run.instance.StartCoroutine(RemoveFromDamageDealtAfterSeconds(DPS_MAX_TIME, damageDealtMessage.damage));
                }
            }
            catch (Exception)
            {
            }
        }
        //Who the fuck made ALL DAMAGE int
        //This seems like not the best thing to spam up to 10s of times per second tbh.
        IEnumerator RemoveFromDamageDealtAfterSeconds(float time, float damage)
        {
            yield return new WaitForSeconds(time);
            damageDealtSincePeriod -= damage;
            if (damageDealtSincePeriod < 0)
            {
                damageDealtSincePeriod = 0;
            }
        }
        internal void FixedUpdate()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timer = -1f;
                    maxComboConfigEntry.Value = maxCombo;
                    currentCombatDamage = 0;

                    maxKillComboConfigEntry.Value = maxKillCombo;
                    currentComboKills = 0;
                }
            }
        }
    }
}
