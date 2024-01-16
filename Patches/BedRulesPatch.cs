using HarmonyLib;
using System;
using UnityEngine;

namespace SleepUtilities.Patches
{
    [HarmonyPatch(typeof(Game))]
    public static class GamePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Game.UpdateSleeping))]
        public static bool UpdateSleeping(ref Game __instance)
        {
            // Only patch game code if we are worrying about sleep time
            if (!BedRulesConfigs.modEnabled.Value || !BedRulesConfigs.sleepAnyTime.Value)
            {
                return true;
            }

            if (!ZNet.instance.IsServer())
            {
                return false;
            }
            if (__instance.m_sleeping)
            {
                if (!EnvMan.instance.IsTimeSkipping())
                {
                    __instance.m_sleeping = false;
                    ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "SleepStop", Array.Empty<object>());
                    return false;
                }
            }
            else if (!EnvMan.instance.IsTimeSkipping())
            {
                if (!__instance.EverybodyIsTryingToSleep())
                {
                    return false;
                }
                EnvMan.instance.SkipToMorning();
                __instance.m_sleeping = true;
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "SleepStart", Array.Empty<object>());
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(Bed))]
    public static class BedPatch
    {
        [HarmonyPatch(nameof(Bed.GetHoverText))]
        private static void Postfix(Bed __instance, ref string __result, ZNetView ___m_nview)
        {
            if (BedRulesConfigs.modEnabled.Value && BedRulesConfigs.sleepWithoutSpawnpoint.Value && BedRulesConfigs.sleepinclaimedbeds.Value)
            {
                //Display a message to sleep with Left Shift and E - For anything that is not owned by player
                if (__instance.IsMine())
                {
                    //if it's mine but not set to spawn, I still might want to sleep here.. Ie temporary bed
                    if (!__instance.IsCurrent())
                    {
                        __result += Localization.instance.Localize("\n[" + "LShift" + "+<color=yellow><b>$KEY_Use</b></color>] $piece_bed_sleep");
              
                    }
                    else
                    {
                        __result += Localization.instance.Localize("\n"+ "<color=yellow><b>Spawn Point Bed</b></color>");
                    }
                    return;
                }

                __result += Localization.instance.Localize("\n[" + "LShift" + "+<color=yellow><b>$KEY_Use</b></color>] $piece_bed_sleep");
                return;
            }
            //check to  see if the bed has an owner. If it does then go away
            else if (BedRulesConfigs.modEnabled.Value && BedRulesConfigs.sleepWithoutSpawnpoint.Value && !BedRulesConfigs.sleepinclaimedbeds.Value)
            {
                bool ownerName = (___m_nview.GetZDO().GetLong("owner", 0L) != 0L) || Traverse.Create(__instance).Method("IsCurrent", new object[0]).GetValue<bool>();
                if (!ownerName)
                {
                    __result += Localization.instance.Localize("\n[" + "LShift" + "+<color=yellow><b>$KEY_Use</b></color>] $piece_bed_sleep");
                    return;
                }

                if (__instance.IsMine())
                {
                    //if it's mine but not set to spawn, I still might want to sleep here.. Ie temporary bed
                    if (!__instance.IsCurrent())
                    {
                        __result += Localization.instance.Localize("\n[" + "LShift" + "+<color=yellow><b>$KEY_Use</b></color>] $piece_bed_sleep");
              
                    }
                    else
                    {
                        __result += Localization.instance.Localize("\n"+ "<color=yellow><b>Spawn Point Bed</b></color>");
                     
                    }
                    return;
               
                }
                
            }
        }

       

        [HarmonyPrefix]
        [HarmonyPriority(Priority.High)]
        [HarmonyPatch(nameof(Bed.Interact))]
        public static bool Interact(Bed __instance, ref bool __result, ref Humanoid human, ref bool repeat, ZNetView ___m_nview)

        {
            if (BedRulesConfigs.modEnabled.Value && BedRulesConfigs.sleepWithoutSpawnpoint.Value)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        //bool flag = Traverse.Create(__instance).Method("IsCurrent", new object[0]).GetValue<bool>() || repeat || ___m_nview.GetZDO().GetLong("owner", 0L) != 0L;
                        //if (!flag)
                        //{
                        Player humanPlayer = human as Player;
                        if (!EnvMan.instance.IsAfternoon() && !EnvMan.instance.IsNight() && !BedRulesConfigs.sleepAnyTime.Value)
                        {
                            human.Message(MessageHud.MessageType.Center, "$msg_cantsleep", 0, null);
                            return false;
                        }
                        if (!__instance.CheckEnemies(humanPlayer) && !BedRulesConfigs.ignoreEnemies.Value)
                        {
                            return false;
                        }
                        if (!__instance.CheckExposure(humanPlayer) && !BedRulesConfigs.ignoreExposure.Value)
                        {
                            return false;
                        }
                        if (!__instance.CheckFire(humanPlayer) && !BedRulesConfigs.ignoreFire.Value)
                        {
                            return false;
                        }
                        if (!__instance.CheckWet(humanPlayer) && !BedRulesConfigs.ignoreWet.Value)
                        {
                            return false;
                        }
                        human.AttachStart(__instance.m_spawnPoint, __instance.gameObject, true, true, false, "attach_bed", new Vector3(0f, 0.5f, 0f));
                        return false;
                        //}
                    }
                }
            }

            long playerID = Game.instance.GetPlayerProfile().GetPlayerID();
            bool owner = __instance.GetOwner() != 0L;
            Player thePlayer = human as Player;
            string ownerName = __instance.GetOwnerName();

            if (ownerName == "")
            {
                __instance.SetOwner(playerID, Game.instance.GetPlayerProfile().GetName());
            }
            else if (__instance.IsMine())
            {
                ZLog.Log("Is mine");
                if (__instance.IsCurrent())
                {
                    if (!EnvMan.instance.IsAfternoon() && !EnvMan.instance.IsNight() && !BedRulesConfigs.sleepAnyTime.Value)
                    {
                        human.Message(MessageHud.MessageType.Center, "$msg_cantsleep", 0, null);
                        return false;
                    }
                    if (!__instance.CheckEnemies(thePlayer) && !BedRulesConfigs.ignoreEnemies.Value)
                    {
                        return false;
                    }
                    if (!__instance.CheckExposure(thePlayer) && !BedRulesConfigs.ignoreExposure.Value)
                    {
                        return false;
                    }
                    if (!__instance.CheckFire(thePlayer) && !BedRulesConfigs.ignoreFire.Value)
                    {
                        return false;
                    }
                    if (!__instance.CheckWet(thePlayer) && !BedRulesConfigs.ignoreWet.Value)
                    {
                        return false;
                    }
                    human.AttachStart(__instance.m_spawnPoint, __instance.gameObject, true, true, false, "attach_bed", new Vector3(0f, 0.5f, 0f));

                    return false;

                    //human.AttachStart(m_spawnPoint, base.gameObject, hideWeapons: true, isBed: true, onShip: false, "attach_bed", new Vector3(0f, 0.5f, 0f));
                }

                //is the owener but no spawn point is set.  Set the spawn point.

                Game.instance.GetPlayerProfile().SetCustomSpawnPoint(__instance.GetSpawnPoint());
                human.Message(MessageHud.MessageType.Center, "$msg_spawnpointset");
            }

            return false;
        }
    }

}