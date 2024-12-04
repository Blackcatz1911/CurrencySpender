using CurrencySpender.Data;
using Dalamud.Utility;
using ECommons.Logging;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.Sheets;

namespace CurrencySpender.Helpers
{
    internal unsafe class PlayerHelper
    {
        public static uint GCRankMaelstrom = 0;
        public static uint GCRankTwinAdders = 0;
        public static uint GCRankImmortalFlames = 0;
        public static Dictionary<uint, uint> GCRanks = new Dictionary<uint, uint>
        {
            { 1, GCRankMaelstrom },
            { 2, GCRankTwinAdders },
            { 3, GCRankImmortalFlames }
        };
        public static Dictionary<uint, uint> SharedFateRanks = new Dictionary<uint, uint>();

        public static void init()
        {
            PluginLog.Verbose("PlayerHelper init");
            P.TaskManager.Enqueue(() => populateGCRank());
            P.TaskManager.Enqueue(() => populateFateRanks());
            P.TaskManager.Enqueue(() => check());
        }
        public static bool populateGCRank()
        {
            if (PlayerState.Instance == null || PlayerState.Instance() == null)
            {
                PluginLog.Verbose("populateGCRank not created");
                return false;
            }
            if (PlayerState.Instance() != null)
            {
                GCRankMaelstrom = PlayerState.Instance()->GCRankMaelstrom;
                GCRankTwinAdders = PlayerState.Instance()->GCRankTwinAdders;
                GCRankImmortalFlames = PlayerState.Instance()->GCRankImmortalFlames;
                GCRanks = new Dictionary<uint, uint>
                {
                    { 1, GCRankMaelstrom },
                    { 2, GCRankTwinAdders },
                    { 3, GCRankImmortalFlames }
                };
                PluginLog.Verbose("populateGCRank created");
                return true;
            }
            PluginLog.Verbose("populateGCRank not created2");
            //EzThrottler.Throttle("AutoRetainerGenericThrottle", 200, true);
            return false;
        }
        public static bool populateFateRanks()
        {
            if (AgentFateProgress.Instance == null || AgentFateProgress.Instance() == null) return false;
            var agentFateProgress = AgentFateProgress.Instance();

            // Check if the instance is valid
            if (agentFateProgress == null)
            {
                PluginLog.Error("AgentFateProgress instance is null!");
                return false;
            }

            if (SharedFateRanks.Count == 0)
            {
                for (int tabIndex = 0; tabIndex < 3; tabIndex++) // FixedSizeArray3
                {
                    var tab = agentFateProgress->Tabs[tabIndex];

                    // Loop through each FateProgressZone in the tab
                    for (int zoneIndex = 0; zoneIndex < 6; zoneIndex++) // FixedSizeArray6
                    {
                        var zone = tab.Zones[zoneIndex];
                        if(zone.TerritoryTypeId != 0)
                            SharedFateRanks.Add(zone.TerritoryTypeId, zone.CurrentRank);
                        // Access the fields of the FateProgressZone
                        //PluginLog.Information($"Zone Name: {zone.ZoneName}");
                        //PluginLog.Information($"TerritoryTypeId: {zone.TerritoryTypeId}");
                        //PluginLog.Information($"Current Rank: {zone.CurrentRank}/{zone.MaxRank}");
                        //PluginLog.Information($"Fate Progress: {zone.FateProgress}/{zone.NeededFates}");
                    }
                }
                if (SharedFateRanks.Count == 0) P.Problem = true;
                else P.Problem = false;
                return true;
            }
            return false;
        }
        public static void check()
        {
            if (SharedFateRanks.Count != 0)
            {
                P.TaskManager.Enqueue(() => Generator.init());
            }
        }
        public static void openSharedFate()
        {
            if (UIModule.Instance()->IsMainCommandUnlocked(84))
            {
                UIModule.Instance()->ExecuteMainCommand(84);
                P.TaskManager.Enqueue(() => init());
                //P.TaskManager.InsertDelay(500);
                //P.TaskManager.Enqueue(() => UIModule.Instance()->ExecuteMainCommand(84));
            }
        }
    }
}
