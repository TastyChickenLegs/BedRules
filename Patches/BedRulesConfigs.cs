using BedRules;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepUtilities.Patches
{

    internal class BedRulesConfigs
    {
        internal static ConfigEntry<bool> modEnabled;
        internal static ConfigEntry<bool> enableMultipleBedfellows;
        internal static ConfigEntry<bool> sleepAnyTime;
        internal static ConfigEntry<bool> ignoreExposure;
        internal static ConfigEntry<bool> ignoreEnemies;
        internal static ConfigEntry<bool> ignoreFire;
        internal static ConfigEntry<bool> ignoreWet;
        internal static ConfigEntry<bool> sleepWithoutSpawnpoint;
        internal static ConfigEntry<bool> sleepWithoutClaiming;
        internal static ConfigEntry<bool> sleepinclaimedbeds;


        internal static void Generate()
        {
            modEnabled = BedRulesPlugin.context.config("", "Enabled", true, "Enable this mod");
            //enableMultipleBedfellows = BedRulesPlugin.context.config("General", "multiplePlayers", true,
            //    new ConfigDescription("Allow multiple people to use this bed simultaneously.", null,
            //    new ConfigurationManagerAttributes { DispName = "Multiple Players in One Bed"}));
                sleepAnyTime = BedRulesPlugin.context.config("General", "Ignoretimerestrictions", true, 
                    new ConfigDescription("Sleep at any time of day, not just at night.",null,
                    new ConfigurationManagerAttributes { DispName = "Ignore Time Restrictions"}));
                ignoreExposure = BedRulesPlugin.context.config("General", "ignorExposurRestrictions", true,
                    new ConfigDescription("Ignore restrictions for walls and a roof.", null,
                    new ConfigurationManagerAttributes { DispName = "Ignore Exposure Restrictions"}));
                ignoreEnemies = BedRulesPlugin.context.config("General", "ignoreNearbyEnemies", true,
                    new ConfigDescription("Enemies no longer prevent you from sleeping.", null,
                    new ConfigurationManagerAttributes { DispName = "Ingore Nearby Enemies"}));
                ignoreFire = BedRulesPlugin.context.config("General", "noFireRequirements", true,
                    new ConfigDescription("Sleep without fire.", null,
                    new ConfigurationManagerAttributes { DispName = "No Fire Requirements"}));
            sleepWithoutSpawnpoint = BedRulesPlugin.context.config("General", "doNotSetSpawnPoint", true,
                new ConfigDescription("Sleeping in a bed will not automatically set a spawn point.", null,
                new ConfigurationManagerAttributes { DispName = "Sleep Without Setting Spawn Point." }));
            //sleepWithoutClaiming = BedRulesPlugin.context.config("General", "doNotClaimBed", true,
            //        new ConfigDescription("Sleep without claiming a bed first.", null,
            //        new ConfigurationManagerAttributes { DispName = "Do Not Automatically Claim Bed"}));
            ignoreWet = BedRulesPlugin.context.config("General", "noWetRequirements", true,
                    new ConfigDescription("Sleep while wet.", null,
                    new ConfigurationManagerAttributes { DispName = "Sleep While Wet" }));
           sleepinclaimedbeds = BedRulesPlugin.context.config("General", "sleepinclaimedbeds", true,
                    new ConfigDescription("Sleep In Claimed Beds.  Must have Sleep Without SpawnPoint set to TRUE", null,
                    new ConfigurationManagerAttributes { DispName = "Sleep In Claimed Beds." }));


        }
    }
}
   

