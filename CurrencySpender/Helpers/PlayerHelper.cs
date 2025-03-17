using CurrencySpender.Data;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Text;

namespace CurrencySpender.Helpers
{
    internal unsafe class PlayerHelper
    {
        public static uint GCRankMaelstrom = 99;
        public static uint GCRankTwinAdders = 99;
        public static uint GCRankImmortalFlames = 99;
        public static Dictionary<uint, uint> GCRanks = new Dictionary<uint, uint>
        {
            { 1, GCRankMaelstrom },
            { 2, GCRankTwinAdders },
            { 3, GCRankImmortalFlames }
        };
        public static bool GCRanksCreated = false;
        public static Dictionary<uint, uint> SharedFateRanks = new Dictionary<uint, uint>();
        public static bool SharedFateRanksCreated = false;
        public static bool SharedFateRanksMax = true;

        public static void init()
        {
            PluginLog.Debug("PlayerHelper init");
            if (PlayerState.Instance == null || PlayerState.Instance() == null)
            {
                PluginLog.Debug("PlayerHelper not logged in");
            }
            else
            {
                P.TaskManager.Enqueue(() => populateGCRank());
                P.TaskManager.Enqueue(() => populateFateRanks());
            }
        }
        public static bool reset()
        {
            GCRankMaelstrom = 99;
            GCRankTwinAdders = 99;
            GCRankImmortalFlames = 99;
            GCRanks = new Dictionary<uint, uint>
            {
                { 1, GCRankMaelstrom },
                { 2, GCRankTwinAdders },
                { 3, GCRankImmortalFlames }
            };
            GCRanksCreated = false;
            SharedFateRanks = new Dictionary<uint, uint>();
            SharedFateRanksCreated = false;
            SharedFateRanksMax = true;
            return true;
        }
        public static bool populateGCRank()
        {
            if (PlayerState.Instance == null || PlayerState.Instance() == null || Service.ClientState.LocalPlayer == null)
            {
                PluginLog.Debug("populateGCRank not created");
                return true;
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
                if (GCRankMaelstrom != 99 && GCRankTwinAdders != 99 && GCRankImmortalFlames != 99)
                {
                    PluginLog.Debug($"populateGCRank created: {GCRankMaelstrom}, {GCRankTwinAdders}, {GCRankImmortalFlames}");
                    GCRanksCreated = true;
                    P.TaskManager.Enqueue(() => ItemGen.GCShops());
                    return true;
                }
            }
            PluginLog.Verbose("populateGCRank not created2");
            //EzThrottler.Throttle("AutoRetainerGenericThrottle", 200, true);
            return true;
        }
        public static bool populateFateRanks()
        {
            if (AgentFateProgress.Instance == null || AgentFateProgress.Instance() == null)
            {
                PluginLog.Error("populateFateRanks: Instance is null");
                return false;
            }
            var agentFateProgress = AgentFateProgress.Instance();

            // Check if the instance is valid
            if (agentFateProgress == null)
            {
                PluginLog.Error("populateFateRanks: Instance is null");
                return false;
            }

            if (SharedFateRanks.Count == 0)
            {
                PluginLog.Debug("populateFateRanks: init");
                for (int tabIndex = 0; tabIndex < 3; tabIndex++) // FixedSizeArray3
                {
                    var tab = agentFateProgress->Tabs[tabIndex];

                    // Loop through each FateProgressZone in the tab
                    for (int zoneIndex = 0; zoneIndex < 6; zoneIndex++) // FixedSizeArray6
                    {
                        var zone = tab.Zones[zoneIndex];
                        if (zone.TerritoryTypeId != 0)
                        {
                            SharedFateRanks.Add(zone.TerritoryTypeId, zone.CurrentRank);
                            if (zone.TerritoryTypeId < 1187 && zone.CurrentRank < 3) SharedFateRanksMax = false;
                            else if (zone.TerritoryTypeId >= 1187 && zone.CurrentRank < 4) SharedFateRanksMax = false;
                        }
                        // Access the fields of the FateProgressZone
                        //PluginLog.Debug($"Zone Name: {zone.ZoneName}, TerritoryTypeId: {zone.TerritoryTypeId}, Current Rank: {zone.CurrentRank}, Fate Progress: {zone.FateProgress}/{zone.NeededFates}");
                    }
                }
                if (SharedFateRanks.Count == 0)
                {
                    PluginLog.Debug("populateFateRanks: SharedFateRanks.Count = 0");
                    P.Problem = true;
                }
                else
                {
                    PluginLog.Debug("populateFateRanks finished");
                    PluginLog.Debug($"SharedFateRanksMax: {SharedFateRanksMax}");
                    P.Problem = false;
                    SharedFateRanksCreated = true;
                    P.TaskManager.Enqueue(() => ItemGen.fateShops());
                }
                return true;
            }
            return true;
        }
        public static void openSharedFate()
        {
            if (UIModule.Instance()->IsMainCommandUnlocked(84))
            {
                UIModule.Instance()->ExecuteMainCommand(84);
                P.TaskManager.Enqueue(() => checkRefresh());
                P.TaskManager.Enqueue(() => populateFateRanks());
                P.TaskManager.Enqueue(() => ItemGen.fateShops());
            }
        }
        public static bool checkRefresh()
        {
            if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("FateProgress", out var addon) && GenericHelpers.IsAddonReady(addon))
            {
                if (((AtkValue*)(nint)(&addon->AtkValues[54]))->Bool == false) return true;
            }
            return false;
        }
    }
}
