using CurrencySpender.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface.Windowing;
using System;
using System;
using System.Numerics;
using CurrencySpender.Classes;
using CurrencySpender.Configuration;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using System.Threading.Tasks;
using CurrencySpender.Helpers;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using CurrencySpender.Classes;
using CurrencySpender.Configuration;
using CurrencySpender.Helpers;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using System.Linq;
using Lumina.Excel.Sheets;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace CurrencySpender.Helpers
{
    internal class DataHelper
    {
        public static List<TrackedCurrency> GenerateCurrencyList() => [
            new TrackedCurrency { Type = CurrencyType.Item, ItemId = 20, Threshold = 75000, Enabled = true, }, // StormSeal
            new TrackedCurrency { Type = CurrencyType.Item, ItemId = 21, Threshold = 75000, Enabled = true, }, // SerpentSeal
            new TrackedCurrency { Type = CurrencyType.Item, ItemId = 22, Threshold = 75000, Enabled = true, }, // FlameSeal

            new TrackedCurrency { Type = CurrencyType.Item, ItemId = 25, Threshold = 18000, Enabled = false, }, // WolfMarks
            new TrackedCurrency { Type = CurrencyType.Item, ItemId = 36656, Threshold = 18000, Enabled = false, }, // TrophyCrystals

            new TrackedCurrency { Type = CurrencyType.Item, ItemId = 27, Threshold = 3500, Enabled = false, }, // AlliedSeals
            new TrackedCurrency { Type = CurrencyType.Item, ItemId = 10307, Threshold = 3500, Enabled = false, }, // CenturioSeals
            new TrackedCurrency { Type = CurrencyType.Item, ItemId = 26533, Threshold = 3500, Enabled = false, }, // SackOfNuts

            new TrackedCurrency { Type = CurrencyType.Item, ItemId = 26807, Threshold = 800, Enabled = false, }, // BicolorGemstones

            new TrackedCurrency { Type = CurrencyType.Item, ItemId = 28, Threshold = 1400, Enabled = true, }, // Poetics
            new TrackedCurrency { Type = CurrencyType.NonLimitedTomestone, Threshold = 1400, Enabled = false, }, // NonLimitedTomestone
            new TrackedCurrency { Type = CurrencyType.LimitedTomestone, Threshold = 1400, Enabled = false, }, // LimitedTomestone

            new TrackedCurrency { Type = CurrencyType.Item, ItemId = 28063, Threshold = 7500, Enabled = false, }, // Skybuilders scripts
        ];

        public static List<BuyableItem> GenerateItemList()
        {
            List<BuyableItem > list = new List<BuyableItem>();
            List<(uint, uint)> gc = [(4564, 120), (4566, 290), (4715, 500), (5119, 200), (5261, 200), (5274, 200), (5358, 200), (5501, 200),
                (5530, 200), (5532, 200), (5558, 200), (6141, 500), (6153, 20), (6154, 20), (6600, 14470), (7596, 1500), (7597, 1500),
                (7601, 1500), (7621, 200), (10386, 345), (12847, 200), (12849, 200), (12854, 800), (12858, 600), (13589, 200), (13591, 200),
                (13593, 200), (13595, 200), (15649, 7000), (15856, 200), (21800, 200), (33916, 600)];
            foreach (var item in gc)
            {
                list.Add(new BuyableItem { Type = ItemType.Sellable, ItemId = item.Item1, C_ID = 20, Price = item.Item2, Loc = 1 });
                list.Add(new BuyableItem { Type = ItemType.Sellable, ItemId = item.Item1, C_ID = 21, Price = item.Item2, Loc = 2 });
                list.Add(new BuyableItem { Type = ItemType.Sellable, ItemId = item.Item1, C_ID = 22, Price = item.Item2, Loc = 3 });
            }
            List<(uint, uint)> storm_registerable = [(6020, 4000), (6021, 6000), (6021, 8000), (6170, 20000), (38642, 40000), (38645, 40000)];
            foreach (var item in storm_registerable)
            {
                list.Add(new BuyableItem { Type = ItemType.Collectable, ItemId = item.Item1, C_ID = 20, Price = item.Item2, Loc = 1 });
            }
            List<(uint, uint)> serpant_registerable = [(6023, 4000), (6024, 6000), (6025, 8000), (6171, 20000), (38641, 40000), (38644, 40000)];
            foreach (var item in serpant_registerable)
            {
                list.Add(new BuyableItem { Type = ItemType.Collectable, ItemId = item.Item1, C_ID = 21, Price = item.Item2, Loc = 2 });
            }
            List<(uint, uint)> flame_registerable = [(6026, 4000), (6027, 6000), (6028, 8000), (6172, 20000), (38643, 40000), (38646, 40000)];
            foreach (var item in flame_registerable)
            {
                list.Add(new BuyableItem { Type = ItemType.Collectable, ItemId = item.Item1, C_ID = 22, Price = item.Item2, Loc = 3 });
            }

            List<(uint, uint, uint)> poetics_registerable = [
                (14266, 375, 4), (15814, 375, 4), (17645, 375, 4), (17646, 375, 4), (17647, 375, 4), (17648, 375, 4), //Hismena
                (21279, 1600, 5), (22475, 1600, 5), (22477, 1600, 5), //Enna
                ];
            foreach (var item in poetics_registerable)
            {
                list.Add(new BuyableItem { Type = ItemType.Collectable, ItemId = item.Item1, C_ID = 28, Price = item.Item2, Loc = item.Item3 });
            }

            List<(uint, uint, uint)> poetics_sellable = [
                (14266, 375, 4), (15814, 375, 4), (17645, 375, 4), (17646, 375, 4), (17647, 375, 4), (17648, 375, 4), //Hismena
                (21279, 1600, 5), (22475, 1600, 5), (22477, 1600, 5), //Enna
            ];
            foreach (var item in poetics_sellable)
            {
                list.Add(new BuyableItem { Type = ItemType.Sellable, ItemId = item.Item1, C_ID = 28, Price = item.Item2, Loc = item.Item3 });
            }

            return list;
        }
    }
}
