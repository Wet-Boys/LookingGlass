using BepInEx.Configuration;
using LookingGlass.Base;
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
        }

        public void SetupRiskOfOptions()
        {
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
