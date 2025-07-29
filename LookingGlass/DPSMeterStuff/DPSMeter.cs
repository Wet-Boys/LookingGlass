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

namespace LookingGlass.DPSMeterStuff
{
    internal class DPSMeter : BaseThing
    {
       
        public ulong killsThisStage = 0;
        public ulong killsThisRun = 0;



        private static Hook overrideHook;
        private static Hook overrideHook2;
        public float damageDealtSincePeriod = 0;
        public float currentCombatDamage = 0;
        public static ConfigEntry<float> maxComboConfigEntry;
        public float maxCombo = 0;
        public float maxRunCombo = 0;


        public ulong currentComboKills = 0;
        public static ConfigEntry<ulong> maxKillComboConfigEntry;
        public ulong maxKillCombo = 0;
        public ulong maxRunKillCombo = 0;
        public const float DPS_MAX_TIME = 5;
        public float timer = DPS_MAX_TIME;
        public DPSMeter()
        {
            Setup();
        }
        public void Setup()
        {
            maxComboConfigEntry = BasePlugin.instance.Config.Bind<float>("Stats", "Max Combo", 0, "What are you gonna do, cheat the number?");
            maxCombo = maxComboConfigEntry.Value;
            maxKillComboConfigEntry = BasePlugin.instance.Config.Bind<ulong>("Stats", "Max Kill Combo", 0, "What are you gonna do, cheat the number?");
            maxKillCombo = maxKillComboConfigEntry.Value;
            var targetMethod = typeof(GlobalEventManager).GetMethod(nameof(GlobalEventManager.ClientDamageNotified), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var destMethod = typeof(DPSMeter).GetMethod(nameof(TrackDamage), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
            targetMethod = typeof(Run).GetMethod(nameof(Run.OnEnable), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(DPSMeter).GetMethod(nameof(RunOnEnable), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook2 = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(Stage).GetMethod(nameof(Stage.PreStartClient), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(DPSMeter).GetMethod(nameof(ResetOnStage), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook2 = new Hook(targetMethod, destMethod, this);
        }

        public void SetupRiskOfOptions()
        {
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
        }
        void TrackDamage(Action<DamageDealtMessage> orig, DamageDealtMessage damageDealtMessage)
        {
            orig(damageDealtMessage);
            try
            {
                CharacterBody attacker = damageDealtMessage.attacker.GetComponent<CharacterBody>();
                if (attacker == LocalUserManager.GetFirstLocalUser().cachedBody || (attacker.master.minionOwnership && attacker.master.minionOwnership.ownerMaster && attacker.master.minionOwnership.ownerMaster.GetBody() == LocalUserManager.GetFirstLocalUser().cachedBody))
                {
                    ulong thing = (ulong)damageDealtMessage.damage;
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
                        if (victim && !victim.alive)
                        {
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
        IEnumerator RemoveFromDamageDealtAfterSeconds(float time, float damage)
        {
            yield return new WaitForSeconds(time);
            damageDealtSincePeriod -= damage;
            if (damageDealtSincePeriod < 0)
            {
                damageDealtSincePeriod = 0;
            }
        }
        internal void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timer = -1;
                    maxComboConfigEntry.Value = maxCombo;
                    currentCombatDamage = 0;

                    maxKillComboConfigEntry.Value = maxKillCombo;
                    currentComboKills = 0;
                }
            }
        }
    }
}
