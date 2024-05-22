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
        private static Hook overrideHook;
        private static Hook overrideHook2;
        public int damageDealtSincePeriod = 0;
        public ulong currentCombatDamage = 0;
        public static ConfigEntry<ulong> maxComboConfigEntry;
        public ulong maxCombo = 0;
        public const float DPS_MAX_TIME = 5;
        public float timer = DPS_MAX_TIME;
        public DPSMeter()
        {
            Setup();
        }
        public void Setup()
        {
            maxComboConfigEntry = BasePlugin.instance.Config.Bind<ulong>("Stats", "Max Combo", 0, "What are you gonna do, cheat the number?");
            maxCombo = maxComboConfigEntry.Value;
            var targetMethod = typeof(GlobalEventManager).GetMethod(nameof(GlobalEventManager.ClientDamageNotified), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var destMethod = typeof(DPSMeter).GetMethod(nameof(TrackDamage), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
            targetMethod = typeof(Run).GetMethod(nameof(Run.OnEnable), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(DPSMeter).GetMethod(nameof(RunOnEnable), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook2 = new Hook(targetMethod, destMethod, this);
        }

        public void SetupRiskOfOptions()
        {
        }
        void RunOnEnable(Action<Run> orig, Run self)
        {
            orig(self);
            damageDealtSincePeriod = 0;
            BuffDefinitions.SetupVanillaDefs();
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
                    damageDealtSincePeriod += (int)thing;
                    currentCombatDamage += thing;
                    if (maxCombo < currentCombatDamage)
                    {
                        maxCombo = currentCombatDamage;
                    }
                    timer = DPS_MAX_TIME;
                    Run.instance.StartCoroutine(RemoveFromDamageDealtAfterSeconds(DPS_MAX_TIME, (int)thing));
                }
            }
            catch (Exception)
            {
            }
        }
        IEnumerator RemoveFromDamageDealtAfterSeconds(float time, int damage)
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
                }
            }
        }
    }
}
