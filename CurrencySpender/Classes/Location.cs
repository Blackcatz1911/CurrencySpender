using Lumina.Excel.Sheets;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using CurrencySpender.Managers;
using ECommons.Automation.NeoTaskManager;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System.Reflection;
using CurrencySpender.Tasks;
using Map = Lumina.Excel.Sheets.Map;

namespace CurrencySpender.Classes
{
    public class Location
    {
        public uint MapId { get; init; }
        public uint TerritoryId { get; init; }
        private readonly uint? aetheryteTerritoryId;
        public uint? AetheryteId;
        public bool NeedsPresence;
        public uint? BackupNpc;
        public string? Lsc;
        public uint AetheryteTerritoryId
        {
            get => aetheryteTerritoryId ?? TerritoryId; // Default to TerritoryId if not explicitly set
            init => aetheryteTerritoryId = value;        // Allow manual assignment
        }
        public record Pos(float X, float Y);
        public Pos Position { get; init; } = new(0, 0);

        public Vector3 Target { get; init; } = new (0, 0, 0);

        public uint NpcId { get; init; }

        public string Zone {
            get
            {
                var data = Service.DataManager.GetExcelSheet<TerritoryType>()!.GetRowOrDefault(TerritoryId);
                if (data != null)
                {
                    return data.Value.PlaceName.ValueNullable?.Name.ToString() ?? "Unknown";
                }
                else return "Unknown";
            }
        }

        public static Location? GetLocation(uint npcId)
        {
            return Locations.FirstOrDefault(loc => loc.NpcId == npcId);
        }
        public MapLinkPayload? GetMapMarker()
        {
            if (Position is { X: 0, Y: 0 })
            {
                PluginLog.Error($"Location for NPC {NpcId} has null Position!");
                return null;
            }
            if (Zone is "Unknown" or "")
            {
                PluginLog.Error("Unknown location");
            }
    
            PluginLog.Debug($"Creating map marker: X={Position.X}, Y={Position.Y}");
            return new MapLinkPayload(TerritoryId, MapId, Position.X, Position.Y);
        }

        public Vector3 GetWorldPosition()
        {
            if (Position is { X: 0, Y: 0 }) return Vector3.Zero;
            var map = Service.DataManager.GetExcelSheet<Map>().GetRowOrDefault(MapId);
            if (map == null || map.Value.SizeFactor == 0)
            {
                PluginLog.Error($"Location for NPC {NpcId} has invalid Map row {MapId}!");
                return Vector3.Zero;
            }
            float scale = map.Value.SizeFactor / 100f;
            float worldX = (Position.X - 1f) * (2048f / 41f) - 1024f / scale - map.Value.OffsetX;
            float worldZ = (Position.Y - 1f) * (2048f / 41f) - 1024f / scale - map.Value.OffsetY;
            return new (worldX, 0, worldZ);
        }

        public void Teleport()
        {
            if (Lifestream.Enabled && C.UseLifestream && !Lifestream.IsBusy() && Lsc != null)
            {
                PluginLog.Debug($"Teleport via Lifestream to '{Lsc}' (NPC {NpcId})");
                P.TaskManager.Enqueue(() => Lifestream.ExecuteCommand(Lsc));
                P.TaskManager.Enqueue(MovementTask.WaitForZone(TerritoryId, MapId, "Lifestream", waitWhile: Lifestream.IsBusy),
                    new TaskManagerConfiguration { TimeLimitMS = 120_000 });
            }
            else
            {
                TeleportInfo info;
                Boolean found = false;
                if (AetheryteId != null)
                {
                    found = AetheryteManager.TryFindAetheryteById(AetheryteId, out info);
                }
                else
                {
                    found = AetheryteManager.TryFindAetheryteByTerritory(AetheryteTerritoryId, out info);
                }

                if (found)
                {
                    PluginLog.Debug($"Aetheryte found: id={info.AetheryteId} subIndex={info.SubIndex} (NPC {NpcId})");
                    TeleportManager.Teleport(info);
                    P.TaskManager.Enqueue(MovementTask.WaitForZone(TerritoryId, MapId, "Teleport"),
                        new TaskManagerConfiguration { TimeLimitMS = 120_000 });
                }
                else
                {
                    PluginLog.Debug($"Aetheryte NOT found for NPC {NpcId} (AetheryteId={AetheryteId}, territory={AetheryteTerritoryId})");
                }
            }
            MoveTo();
        }

        public void MoveTo()
        {
            if (Target == Vector3.Zero)
            {
                PluginLog.Debug($"MoveTo skipped: no Target set for NPC {NpcId}");
                return;
            }
            if (!Vnavmesh.Enabled || !C.UseVnavmesh)
            {
                PluginLog.Debug($"MoveTo skipped: vnavmesh not enabled (NPC {NpcId})");
                return;
            }
            PluginLog.Debug($"Enqueue MoveTo to {Target} for NPC {NpcId}");
            P.TaskManager.Enqueue(MovementTask.MoveTo(Target, TerritoryId, MapId), "MoveTo",
                new TaskManagerConfiguration { TimeLimitMS = 120_000 });
        }

        public static readonly List<Location> Locations = [
            new() { MapId = 011, TerritoryId = 0128, Position = new Pos(13.1f, 12.7f), NpcId = 1002387, AetheryteTerritoryId = 129, Lsc="gc Mael" },        // Maelstrom
            new() { MapId = 002, TerritoryId = 0132, Position = new Pos(9.8f, 11.0f), NpcId = 1002390, Lsc="gc Twin" },                                   // TwinAdders
            new() { MapId = 013, TerritoryId = 0130, Position = new Pos(8.3f, 9.0f), NpcId =  1002393, Lsc="gc Flame" },                                  // Immortal Flames

            new() { MapId = 012, TerritoryId = 0129, Position = new Pos(06.0f, 11.9f), NpcId = 1003633,
                Lsc="Hawkers' Alley", Target = new (-260,16,40) }, // Scrip Exchange Limsa
            new() { MapId = 014, TerritoryId = 0131, Position = new Pos(14.2f, 10.8f), NpcId = 1001617,
                AetheryteTerritoryId = 130, Lsc="Sapphire Avenue Exchange", Target = new (149,4,-19) }, // Scrip Exchange Uldah
            new() { MapId = 003, TerritoryId = 0133, Position = new Pos(14.1f, 09.1f), NpcId = 1003077,
                AetheryteTerritoryId = 132,  Lsc="Leatherworkers' Guild & Shaded Bower", Target = new (144,14,-105) }, // Scrip Exchange Gridania
            new() { MapId = 856, TerritoryId = 1186, Position = new Pos(09.1f, 13.2f), NpcId = 1003633,
                Lsc="Nexus Arcade", Target = new (-161,1,-44) }, // Scrip Exchange Solution Nine
            new() { MapId = 497, TerritoryId = 0819, Position = new Pos(10.4f, 07.8f), NpcId = 1045069,
                Lsc="The Crystalline Mean", Target = new (-39,20,-170) }, // Scrip Exchange Quinnana

            new() { MapId = 196, TerritoryId = 0144, Position = new Pos(5.1f,6.6f), NpcId =  1011039,
                Target = new (-46,2,29) }, // Gold Saucer Attendant
            new() { MapId = 196, TerritoryId = 0144, Position = new Pos(5.4f,6.5f), NpcId =  1011610,
                Target = new (-34,0,23) }, // Modern Aesthetics Saleswoman
            new() { MapId = 196, TerritoryId = 0144, Position = new Pos(5.0f,6.4f), NpcId =  1016294,
                Target = new (-55,2,15) }, // Triple Triad Trader
            new() { MapId = 196, TerritoryId = 0144, Position = new Pos(7.1f,7.8f), NpcId =  1044839,
                Lsc="Wonder Square East", Target = new (51,21,84) }, // Dibourdier

            new() { MapId = 197, TerritoryId = 0388, Position = new Pos(7.7f,6.9f), NpcId =  1011595,
                Lsc = "Minion Square", Target = new (80,1,43) }, // Minion Trader

            new() { MapId = 257, TerritoryId = 0478, Position = new Pos(5.7f, 5.2f), NpcId = 1012228,
                Target = new (-15,211,-40) }, // Hismena
            new() { MapId = 366, TerritoryId = 0635, Position = new Pos(13.9f, 11.6f), NpcId = 1019450,
                Target = new (132,1,18) }, // Enna
            new() { MapId = 051, TerritoryId = 0250, Position = new Pos(4.5f, 6.0f), NpcId = 1005244,
                Target = new (-1,2,-6) }, // Mark Quartermaster
            new() { MapId = 856, TerritoryId = 1186, Position = new Pos(8.6f, 13.5f), NpcId = 1049079,
                Lsc="Nexus Arcade", Target = new (-186,1,-28)  }, // Zircon
            new() { MapId = 694, TerritoryId = 0963, Position = new Pos(10.8f, 10.4f), NpcId = 1037301,
                Target = new (-18,1,-43) }, // Cihanti
            new() { MapId = 025, TerritoryId = 0156, Position = new Pos(22.7f, 6.6f), NpcId = 1008119,
                Target = new (63,32,-727) }, // Auriana
            new() { MapId = 014, TerritoryId = 0131, Position = new Pos(12.5f,13.0f), NpcId = 1032254,
                AetheryteTerritoryId = 130, Lsc="Weavers' Guild", Target = new (65,14,90) }, // Maudlin Latool Ja
            new() { MapId = 051, TerritoryId = 0250, Position = new Pos(4.4f,6.1f), NpcId = 1038441,
                Target = new (-1,2,0) }, // Crystal Quartermaster
            new() { MapId = 051, TerritoryId = 0250, Position = new Pos(5f,5.3f), NpcId = 1018655,
                Target = new(23, 4.5f, -40)}, // Disreputable Priest
            new() { MapId = 555, TerritoryId = 0820, Position = new Pos(10.2f,11.8f), NpcId = 1027564,
                Target = new (-47,85,31) },
            
            new() { MapId = 011, TerritoryId = 0128, Position = new Pos(13.2f,12.5f), NpcId = 1009552,
                AetheryteTerritoryId = 129, Lsc="gc Mael" },
            new() { MapId = 002, TerritoryId = 0132, Position = new Pos(9.7f,11.2f), NpcId = 1009152,
                Lsc="gc Twin" },
            new() { MapId = 013, TerritoryId = 0130, Position = new Pos(8.1f,9.3f), NpcId = 1001379,
                Lsc="gc Flame" },
            
            new() { MapId = 497, TerritoryId = 0819, Position = new Pos(9.4f,9.5f), NpcId = 1027988,
                Lsc="Temenos Rookery", Target = new(-91, 0, -84)  }, // Xylle
            new() { MapId = 554, TerritoryId = 0820, Position = new Pos(11.0f,10.8f), NpcId = 1029975,
                Lsc="The Mainstay", Target = new(-9, 36, -19) }, // Ilfroy
            new() { MapId = 693, TerritoryId = 0962, Position = new Pos(11.8f,13.2f), NpcId = 1037059,
                Lsc="Scholar's Harbor", Target = new(33, -15, 102) }, // J'lakshai
            new() { MapId = 694, TerritoryId = 0963, Position = new Pos(10.5f,7.4f), NpcId = 1037312,
                Lsc="Mehryde's Meyhane", Target = new(-35, 1.5f, -190) }, // Wilmetta
            new() { MapId = 855, TerritoryId = 1185, Position = new Pos(13.9f, 13.5f), NpcId = 1048387,
                Lsc="Bayside Bevy Marketplace", Target = new(26.1f, -14.0f, 127.8f) }, // Ryobool Ja
            new() { MapId = 370, TerritoryId = 0628, Position = new Pos(10.3f,10.2f), NpcId = 1019007,
                Lsc="Shiokaze Hostelry", Target = new(-43, -2, -51),  }, // Estrild
            new() { MapId = 370, TerritoryId = 0628, Position = new Pos(10.4f,10.2f), NpcId = 1019008, 
                Lsc="Shiokaze Hostelry", Target = new(-39, -2, -51) }, //Satsuya
            new() { MapId = 366, TerritoryId = 0635, Position = new Pos(13.0f,11.7f), NpcId = 1019454,
                Target = new(94, 0, 28) }, // Leuekin
            new() { MapId = 366, TerritoryId = 0635, Position = new Pos(13.8f,11.8f), NpcId = 1019451,
                Target = new(129, 0, 28) }, //Eschina
            new() { MapId = 366, TerritoryId = 0635, Position = new Pos(13.0f, 11.7f), NpcId = 1019455,
                Target = new(94, 0, 26) }, //Billebaut
            new() { MapId = 218, TerritoryId = 0418, Position = new Pos(13.1f,11.9f), NpcId = 1012225,
                Lsc="The Forgotten Knight", Target = new(93, 15, 30) }, // Ardolain
            new() { MapId = 257, TerritoryId = 0478, Position = new Pos(5.9f,5.2f), NpcId = 1015578,
                Target = new(-11, 211, -43) }, //Bertana
            new() { MapId = 025, TerritoryId = 0156, Position = new Pos(22.1f,4.9f), NpcId = 103691,
                Target = new(35, 29, -826) }, // Edelina
            new() { MapId = 574, TerritoryId = 0886, Position = new Pos(12.0f,14.0f), NpcId = 1031680,
                AetheryteId = 70, NeedsPresence = true, BackupNpc = 1031682 }, // Enie
            new() { MapId = 856, TerritoryId = 1186, Position = new Pos(9.1f, 13.2f), NpcId = 1049086,
                Target = new(-161.1f, 0.9f, -46.4f) }, //Splendors Vendor
            
            new() { MapId = 257, TerritoryId = 0478, Position = new Pos(5.7f, 6.1f), NpcId = 1033921,
                Target = new (-19,211,2) }, // Faux Commander
            
            new() { MapId = 003, TerritoryId = 133, Position = new Pos(10.6f, 6.3f), NpcId = 1008145,
                Target = new(-30.8f, 10.1f, -246.9f) }, // Jonathas
            
            new() { MapId = 218, TerritoryId = 0418, Position = new Pos(14.2f, 12.5f), NpcId = 1031682,
                Target = new(151.5f, -20.0f, 64.0f) }, // Thomelin (Gatekeep)
            
            // --- Start Bicolor Gemstones ---
            new() { MapId = 491, TerritoryId = 813, Position = new Pos(35.5f,20.6f), NpcId = 1027385,
                Target = new(701.9f, 21.8f, -42.8f) }, // Siulmet
            new() { MapId = 492, TerritoryId = 814, Position = new Pos(11.8f,08.9f), NpcId = 1027497,
                AetheryteId = 139, Target = new(-481.7f, 417.2f, -626.3f)}, // Zumutt
            new() { MapId = 493, TerritoryId = 815, Position = new Pos(10.6f,17.1f), NpcId = 1027892,
                AetheryteId = 141, Target = new(-539, 46, -217) }, // Halden
            new() { MapId = 494, TerritoryId = 816, Position = new Pos(16.2f,30.6f), NpcId = 1027665, 
                Target = new(-256.5f, 40.3f, 465.3f) }, // Sul Lad
            new() { MapId = 495, TerritoryId = 817, Position = new Pos(27.9f,18.2f), NpcId = 1027709,
                AetheryteId = 143, Target = new(323.2f, 33.8f, -164.8f) }, // Nacille
            new() { MapId = 496, TerritoryId = 818, Position = new Pos(33.2f,18.0f), NpcId = 1027766,
                Target = new(585.7f, 349.0f, -176.4f) }, // Goushs Ooan
            new() { MapId = 497, TerritoryId = 819, Position = new Pos(11.1f,13.6f), NpcId = 1027998,
                Lsc="Musica Universalis Markets", Target = new(-7.6f, -7.7f, 119.3f) }, // Gramsol
            new() { MapId = 555, TerritoryId = 820, Position = new Pos(10.5f,12.2f), NpcId = 1027538,
                Target = new(-33.1f, 84.2f, 46.8f) }, // Pedronille

            new() { MapId = 695, TerritoryId = 956, Position = new Pos(29.9f,12.9f), NpcId = 1037484, 
                Target = new(424.7f, 166.3f, -425.4f) }, // Faezbroes
            new() { MapId = 696, TerritoryId = 957, Position = new Pos(25.8f,34.6f), NpcId = 1037635,
                Target = new(217.8f, 4.8f, 657.0f) }, // Mahveydah
            new() { MapId = 697, TerritoryId = 958, Position = new Pos(12.9f,30.0f), NpcId = 1037724,
                Target = new(-425.1f, 22.2f, 430.0f) }, // Zawawa
            new() { MapId = 698, TerritoryId = 959, Position = new Pos(21.8f,12.2f), NpcId = 1037793,
                AetheryteId = 175, Target = new(20.8f, -132.9f, -461.0f) }, // Tradingway
            new() { MapId = 699, TerritoryId = 960, Position = new Pos(30.8f,28.0f), NpcId = 1038004,
                Target = new(471.2f, 437.0f, 330.1f) }, // N-1499
            new() { MapId = 700, TerritoryId = 961, Position = new Pos(24.4f,23.4f), NpcId = 1037909,
                Target = new(147.7f, 10.4f, 98.9f) }, // Aisara
            new() { MapId = 693, TerritoryId = 962, Position = new Pos(12.7f,10.4f), NpcId = 1037055, 
                Target = new(76.0f, 5.1f, -37.2f) }, // Gadfrid
            new() { MapId = 694, TerritoryId = 963, Position = new Pos(11.1f,10.2f), NpcId = 1037304, 
                Target = new(-6.8f, 0.9f, -51.1f) }, // Sajareen

            new() { MapId = 857, TerritoryId = 1187, Position = new Pos(27.5f,11.7f), NpcId = 1048628,
                Target = new(300.3f, -172.3f, -484.3f) }, // Tepli
            new() { MapId = 858, TerritoryId = 1188, Position = new Pos(17.4f,11.0f), NpcId = 1048778,
                Target = new(-199.4f, 6.3f, -519.8f) }, // Kunuhali
            new() { MapId = 859, TerritoryId = 1189, Position = new Pos(13.8f,12.7f), NpcId = 1048933,
                Target = new(-382.8f, 23.5f, -435.6f) }, // Rral Wuruq
            new() { MapId = 860, TerritoryId = 1190, Position = new Pos(28.6f,30.8f), NpcId = 1049283,
                Target = new(359.9f, -1.6f, 468.8f) }, // Mitepe
            new() { MapId = 861, TerritoryId = 1191, Position = new Pos(16.3f,09.6f), NpcId = 1049438,
                AetheryteId = 211, Target = new(-256.9f, 30.0f, -592.0f) }, // Toashana
            new() { MapId = 862, TerritoryId = 1192, Position = new Pos(22.0f,37.5f), NpcId = 1049528,
                Target = new(30.4f, 53.2f, 806.0f) }, // Clerk PX-0029
            new() { MapId = 855, TerritoryId = 1185, Position = new Pos(12.8f,13.0f), NpcId = 1048383,
                Lsc="Bayside Bevy Marketplace", Target = new(-25.8f, -10.0f, 103.5f) }, // Kajeel Ja
            new() { MapId = 856, TerritoryId = 1186, Position = new Pos(08.4f,14.0f), NpcId = 1049082,
                Lsc="Nexus Arcade", Target = new(-199.1f, 0.9f, -7.1f) }, // Beryl
            // --- End Bicolor Gemstones ---

            new() { MapId = 016, TerritoryId = 0135, Position = new Pos(24.9f, 34.8f), NpcId = 1043621,
                Target = new(173.3f, 14.1f, 668.3f) }, // Baldin
            new() { MapId = 793, TerritoryId = 1055, Position = new Pos(12.6f, 28.3f), NpcId = 1043463,
                AetheryteId = 10, NeedsPresence = true, BackupNpc = 1043621 }, // Horrendous Hoarder
            new() { MapId = 793, TerritoryId = 1055, Position = new Pos(12.8f, 26.9f), NpcId = 1043465,
                AetheryteId = 10, NeedsPresence = true, BackupNpc = 1043621 }, // Produce Producer
            
            new() { MapId = 0698, TerritoryId = 0959, Position = new Pos(21.9f,13.2f), NpcId = 1052581,
                Target = new(27, -137, -412)}, // Drivingway
            
            new() { MapId = 1031, TerritoryId = 1237, Position = new Pos(21.8f, 21.8f), NpcId = 1052608,
                AetheryteId = 175, NeedsPresence = true, BackupNpc = 1052581 }, // Mesouaidonque (Sinus Ardorum)
            new() { MapId = 1068, TerritoryId = 1291, Position = new Pos(28.6f, 13.5f), NpcId = 1052640,
                AetheryteId = 175, NeedsPresence = true, BackupNpc = 1052581 }, // Mesouaidonque (Phaenna)
            new() { MapId = 1160, TerritoryId = 1310, Position = new Pos(17.4f, 24.5f), NpcId = 1052650,
                AetheryteId = 175, NeedsPresence = true, BackupNpc = 1052581 }, // Mesouaidonque (Oizys)
            new() { MapId = 1260, TerritoryId = 1319, Position = new Pos(27.8f, 29.0f), NpcId = 1056824,
                AetheryteId = 175, NeedsPresence = true, BackupNpc = 1052581 }, // Mesouaidonque (Auxesia)
            
            new() { MapId = 1031, TerritoryId = 1237, Position = new Pos(21.8f, 21.1f), NpcId = 1052612,
                AetheryteId = 175, NeedsPresence = true, BackupNpc = 1052581 }, // Orbitingway (Sinus Ardorum)
            new() { MapId = 1068, TerritoryId = 1291, Position = new Pos(28.6f, 12.7f), NpcId = 1052642,
                AetheryteId = 175, NeedsPresence = true, BackupNpc = 1052581 }, // Orbitingway (Phaenna)
            new() { MapId = 1160, TerritoryId = 1310, Position = new Pos(18.2f, 24.5f), NpcId = 1052652,
                AetheryteId = 175, NeedsPresence = true, BackupNpc = 1052581 }, // Orbitingway (Oizys)
            new() { MapId = 1260, TerritoryId = 1319, Position = new Pos(27.3f, 28.4f), NpcId = 1056826,
                AetheryteId = 175, NeedsPresence = true, BackupNpc = 1052581 }, // Orbitingway (Auxesia)
            
            // Societies
            new() { MapId = 023, TerritoryId = 0146, Position = new Pos(23.3f, 14.1f), NpcId = 1005554, AetheryteId = 019, Target = new (92,15,-363)}, // Amalj'aa Vendor
            new() { MapId = 005, TerritoryId = 0152, Position = new Pos(22.3f, 26.3f), NpcId = 1005569, AetheryteId = 004, Target = new (44,6,250) }, // Sylphic Vendor
            
            new() { MapId = 030, TerritoryId = 0180, Position = new Pos(21.6f, 17.8f), NpcId = 1008909, AetheryteId = 016, Target = new (12,16,-184) }, // Kobold Vendor
            new() { MapId = 018, TerritoryId = 0138, Position = new Pos(16.9f, 22.4f), NpcId = 1008907, AetheryteId = 014, Target = new (-228,-40,47) }, // Sahagin Vendor
            new() { MapId = 007, TerritoryId = 0154, Position = new Pos(24.9f, 22.7f), NpcId = 1009205, AetheryteId = 007, Target = new (170,-3,64)}, // Ixali Vendor
            
            new() { MapId = 215, TerritoryId = 0401, Position = new Pos(07.0f, 14.2f), NpcId = 1016093, AetheryteId = 073, Target = new (-779,-133,-412) }, // Luna Vanu
            new() { MapId = 212, TerritoryId = 0398, Position = new Pos(23.6f, 19.1f), NpcId = 1016804, AetheryteId = 076, Target = new (57,-48,-170) }, // Vath Stickpeddler
            new() { MapId = 214, TerritoryId = 0400, Position = new Pos(15.9f, 28.5f), NpcId = 1017172, AetheryteId = 079, Target = new (-330,60,300) }, // Mogmul Mogbelly
            
            new() { MapId = 371, TerritoryId = 0613, Position = new Pos(29.3f, 16.8f), NpcId = 1024219, AetheryteId = 105, Target = new (390,-119,-233) }, // Shikathe
            new() { MapId = 367, TerritoryId = 0612, Position = new Pos(20.9f, 26.2f), NpcId = 1024774, AetheryteId = 099, Target = new (-24,56,234) }, // Madhura
            new() { MapId = 372, TerritoryId = 0622, Position = new Pos(05.8f, 23.5f), NpcId = 1025604, AetheryteId = 128, Target = new (-780,128,102) }, // Gyosho
            
            new() { MapId = 494, TerritoryId = 0816, Position = new Pos(12.4f, 32.9f), NpcId = 1031810, AetheryteId = 144, Target = new (-450,71,571) }, // Jul Oul
            new() { MapId = 495, TerritoryId = 0817, Position = new Pos(37.3f, 17.1f), NpcId = 1032661, AetheryteId = 143, Target = new (792,-45,-214) }, // Yuqurl Manl
            new() { MapId = 491, TerritoryId = 0813, Position = new Pos(09.4f, 13.1f), NpcId = 1033714, AetheryteId = 136, Target = new (-600,66,-420) }, // Mizutt
            
            new() { MapId = 696, TerritoryId = 0957, Position = new Pos(20.4f, 28.4f), NpcId = 1042424, AetheryteId = 169, Target = new (-52,40,350) }, // Ghanta
            new() { MapId = 699, TerritoryId = 0960, Position = new Pos(27.7f, 24.7f), NpcId = 1043418, AetheryteId = 181, Target = new (312,482,161) }, // N-0598
            new() { MapId = 698, TerritoryId = 0959, Position = new Pos(17.4f, 15.8f), NpcId = 1044404, AetheryteId = 175, Target = new (-201,-49,-280) }, // Coiningway
            
            new() { MapId = 858, TerritoryId = 1188, Position = new Pos(37.1f, 16.2f), NpcId = 1051799, AetheryteId = 238, Target = new (784,13,-261) }, // Pavli
            new() { MapId = 859, TerritoryId = 1189, Position = new Pos(33.3f, 36.1f), NpcId = 1052563, AetheryteId = 206, Target = new (590,-142,731) }, // Veerul Ja
            new() { MapId = 857, TerritoryId = 1187, Position = new Pos(31.2f, 37.3f), NpcId = 1054637, AetheryteId = 201, Target = new (492,143,792) }, // Rarkorgor
            
        ];

        public override string ToString()
        {
            var properties = GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(prop => $"{prop.Name}={prop.GetValue(this)}");

            return $"{GetType().Name}: {string.Join(", ", properties)}";
        }
    }
}
