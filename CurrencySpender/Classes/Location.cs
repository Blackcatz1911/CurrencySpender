using Lumina.Excel.Sheets;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using CurrencySpender.Managers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System.Reflection;

namespace CurrencySpender.Classes
{
    public class Location
    {
        public uint MapId { get; set; }
        public uint TerritoryId { get; set; }
        private uint? aetheryteTerritoryId;
        public uint? AetheryteId;
        public uint AetheryteTerritoryId
        {
            get => aetheryteTerritoryId ?? TerritoryId; // Default to TerritoryId if not explicitly set
            set => aetheryteTerritoryId = value;        // Allow manual assignment
        }
        public (float, float) Postion { get; set; }

        public uint NpcId { get; set; }

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

        public static Location GetLocation(uint NpcId)
        {
            return locations.FirstOr0(loc => loc.NpcId == NpcId);
        }
        public MapLinkPayload GetMapMarker()
        {
            if ((Zone == "Unknown" || Zone == "") && C.Debug) PluginLog.Error("Unknown location");
            return new MapLinkPayload(TerritoryId, MapId, Postion.Item1, Postion.Item2);
        }

        public void Teleport()
        {
            TeleportInfo info;
            bool found = false;
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
                PluginLog.Verbose($"info.AetheryteId: {info.AetheryteId} info.SubIndex: {info.SubIndex}");
                TeleportManager.Teleport(info);
            }
            else
            {
                PluginLog.Verbose($"TP not found");
            }
        }

        public static List<Location> locations = [
            new Location {  },
            new Location { MapId = 011, TerritoryId = 0128, Postion = (13.1f, 12.7f), NpcId = 1002387, AetheryteTerritoryId = 129 },
            new Location { MapId = 002, TerritoryId = 0132, Postion = (9.8f, 11.0f), NpcId = 1002390 },
            new Location { MapId = 013, TerritoryId = 0130, Postion = (8.3f, 9.0f), NpcId =  1002393 },

            new Location { MapId = 196, TerritoryId = 0144, Postion = (5.1f,6.6f), NpcId =  1011039 }, // Gold Saucer Attendant
            new Location { MapId = 196, TerritoryId = 0144, Postion = (5.4f,6.5f), NpcId =  1011610 }, // Modern Aesthetics Saleswoman
            new Location { MapId = 196, TerritoryId = 0144, Postion = (5.0f,6.4f), NpcId =  1010478 }, // Triple Triad Trader

            new Location { MapId = 197, TerritoryId = 0388, Postion = (7.7f,6.9f), NpcId =  1011595 }, // Minion Trader

            new Location { MapId = 257, TerritoryId = 0478, Postion = (5.7f, 5.2f), NpcId = 1012228 },
            new Location { MapId = 366, TerritoryId = 0635, Postion = (13.9f, 11.6f), NpcId = 1019450 },
            new Location { MapId = 051, TerritoryId = 0250, Postion = (4.5f, 6.0f), NpcId = 1005244 },
            new Location { MapId = 856, TerritoryId = 1186, Postion = (8.6f, 13.5f), NpcId = 1049079 },
            new Location { MapId = 694, TerritoryId = 0963, Postion = (10.8f, 10.4f), NpcId = 1037301 },
            new Location { MapId = 025, TerritoryId = 0156, Postion = (22.7f, 6.6f), NpcId = 1008119 },
            new Location { MapId = 014, TerritoryId = 0131, Postion = (12.5f,13.0f), NpcId = 1032254, AetheryteTerritoryId = 130 },
            new Location { MapId = 051, TerritoryId = 0250, Postion = (4.4f,6.1f), NpcId = 1038441 },
            new Location { MapId = 014, TerritoryId = 0131, Postion = (5f,5.3f), NpcId = 1018655 },
            new Location { MapId = 555, TerritoryId = 0820, Postion = (10.2f,11.8f), NpcId = 1027564 },
            new Location { MapId = 011, TerritoryId = 0128, Postion = (13.2f,12.5f), NpcId = 1001379, AetheryteTerritoryId = 129 },
            new Location { MapId = 002, TerritoryId = 0132, Postion = (9.7f,11.2f), NpcId = 1009152 },
            new Location { MapId = 013, TerritoryId = 0130, Postion = (8.1f,9.3f), NpcId = 1009552 },
            new Location { MapId = 497, TerritoryId = 0819, Postion = (9.4f,9.5f), NpcId = 1027988 },
            new Location { MapId = 554, TerritoryId = 0820, Postion = (11.0f,10.8f), NpcId = 1029975 },
            new Location { MapId = 693, TerritoryId = 0962, Postion = (11.8f,13.2f), NpcId = 1037059 },
            new Location { MapId = 694, TerritoryId = 0963, Postion = (10.5f,7.4f), NpcId = 1037312 },
            new Location { MapId = 855, TerritoryId = 1185, Postion = (13.9f, 13.5f), NpcId = 1048387 },
            new Location { MapId = 370, TerritoryId = 0628, Postion = (10.3f,10.2f), NpcId = 1019007 },
            new Location { MapId = 370, TerritoryId = 0628, Postion = (10.4f,10.2f), NpcId = 1019008 },
            new Location { MapId = 366, TerritoryId = 0635, Postion = (13.0f,11.7f), NpcId = 1019454 },
            new Location { MapId = 366, TerritoryId = 0635, Postion = (13.8f,11.8f), NpcId = 1019451 },
            new Location { MapId = 366, TerritoryId = 0635, Postion = (13.0f, 11.7f), NpcId = 1019455 },
            new Location { MapId = 218, TerritoryId = 0418, Postion = (13.1f,11.9f), NpcId = 1012225 },
            new Location { MapId = 257, TerritoryId = 0478, Postion = (5.9f,5.2f), NpcId = 1015578 },
            new Location { MapId = 025, TerritoryId = 0156, Postion = (22.1f,4.9f), NpcId = 1036913 },
            new Location { MapId = 574, TerritoryId = 0886, Postion = (12.0f,14.0f), NpcId = 1031680, AetheryteId = 70 },
            new Location { MapId = 856, TerritoryId = 1186, Postion = (9.1f, 13.2f), NpcId = 1049086 },
            new Location { MapId = 856, TerritoryId = 1186, Postion = (9.1f, 13.2f), NpcId = 1049085 },
            
            //Bicolor Gemstones
            new Location { MapId = 491, TerritoryId = 813, Postion = (35.5f,20.6f), NpcId = 1027385 }, // Siulmet
            new Location { MapId = 492, TerritoryId = 814, Postion = (11.8f,8.9f), NpcId = 1027497 }, // Zumutt
            new Location { MapId = 493, TerritoryId = 815, Postion = (10.6f,17.1f), NpcId = 1027892, AetheryteId = 141 }, // Halden
            new Location { MapId = 494, TerritoryId = 816, Postion = (16.2f,30.6f), NpcId = 1027665 }, // Sul Lad
            new Location { MapId = 495, TerritoryId = 817, Postion = (27.9f,18.2f), NpcId = 1027709 }, // Nacille
            new Location { MapId = 496, TerritoryId = 818, Postion = (33.2f,18.0f), NpcId = 1027766 }, // Goushs Ooan
            new Location { MapId = 497, TerritoryId = 819, Postion = (11.1f,13.6f), NpcId = 1027998 }, // Gramsol
            new Location { MapId = 555, TerritoryId = 820, Postion = (10.5f,12.2f), NpcId = 1027538 }, // Pedronille

            new Location { MapId = 695, TerritoryId = 956, Postion = (29.9f,12.9f), NpcId = 1037484 }, // Faezbroes
            new Location { MapId = 696, TerritoryId = 957, Postion = (25.8f,34.6f), NpcId = 1037635 }, // Mahveydah
            new Location { MapId = 697, TerritoryId = 958, Postion = (12.9f,30.0f), NpcId = 1037724 }, // Zawawa
            new Location { MapId = 698, TerritoryId = 959, Postion = (21.8f,12.2f), NpcId = 1037793, AetheryteId = 175 }, // Tradingway
            new Location { MapId = 699, TerritoryId = 960, Postion = (30.8f,28.0f), NpcId = 1038004 }, // N-1499
            new Location { MapId = 700, TerritoryId = 961, Postion = (24.4f,23.4f), NpcId = 1037909 }, // Aisara
            new Location { MapId = 693, TerritoryId = 962, Postion = (12.7f,10.4f), NpcId = 1037055 }, // Gadfrid
            new Location { MapId = 694, TerritoryId = 963, Postion = (11.1f,10.2f), NpcId = 1037304 }, // Sajareen

            new Location { MapId = 857, TerritoryId = 1187, Postion = (27.5f,11.7f), NpcId = 1048628 }, // Tepli
            new Location { MapId = 858, TerritoryId = 1188, Postion = (17.4f,11.0f), NpcId = 1048778 }, // Kunuhali
            new Location { MapId = 859, TerritoryId = 1189, Postion = (13.8f,12.7f), NpcId = 1048933 }, // Rral Wuruq
            new Location { MapId = 860, TerritoryId = 1190, Postion = (28.6f,30.8f), NpcId = 1049283 }, // Mitepe
            new Location { MapId = 861, TerritoryId = 1191, Postion = (16.3f,09.6f), NpcId = 1049438 }, // Toashana
            new Location { MapId = 862, TerritoryId = 1192, Postion = (22.0f,37.5f), NpcId = 1049528 }, // Clerk PX-0029
            new Location { MapId = 855, TerritoryId = 1185, Postion = (12.8f,13.0f), NpcId = 1048383 }, // Kajeel Ja
            new Location { MapId = 856, TerritoryId = 1186, Postion = (08.4f,14.0f), NpcId = 1049082 }, // Beryl
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
