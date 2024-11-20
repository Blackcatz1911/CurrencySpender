using CurrencySpender.Classes;
using ECommons.DalamudServices;

namespace CurrencySpender.Helpers
{
    internal class DataHelper
    {
        public static void GenerateCurrencyList() {
            if(C.Currencies.Count == 0 || C.Version != Svc.PluginInterface.Manifest.AssemblyVersion.ToString() || C.debug)
            {
                List<TrackedCurrency> currencies = [
                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 20, Threshold = 75000, MaxCount = 90000, Enabled = true, }, // StormSeal
                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 21, Threshold = 75000, MaxCount = 90000, Enabled = true, }, // SerpentSeal
                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 22, Threshold = 75000, MaxCount = 90000, Enabled = true, }, // FlameSeal

                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 25, Threshold = 18000, MaxCount = 20000, Enabled = true, }, // WolfMarks
                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 36656, Threshold = 18000, MaxCount = 20000, Enabled = false, }, // TrophyCrystals

                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 27, Threshold = 3500, Enabled = false, }, // AlliedSeals
                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 10307, Threshold = 3500, Enabled = false, }, // CenturioSeals
                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 26533, Threshold = 3500, Enabled = false, }, // SackOfNuts

                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 26807, Threshold = 800, Enabled = false, }, // BicolorGemstones

                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 28, Threshold = 1400, MaxCount = 2000, Enabled = true, }, // Poetics
                    new TrackedCurrency { Type = CurrencyType.NonLimitedTomestone, Threshold = 1400, MaxCount = 2000, Enabled = true, }, // NonLimitedTomestone
                    new TrackedCurrency { Type = CurrencyType.LimitedTomestone, Threshold = 1400, MaxCount = 2000, Enabled = false, }, // LimitedTomestone

                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 28063, Threshold = 7500, MaxCount = 10000, Enabled = false, }, // Skybuilders scripts
                ];
                C.Currencies = currencies;
            }
        }

        public static void GenerateItemList()
        {
            if (C.Items.Count == 0 || C.Version != Svc.PluginInterface.Manifest.AssemblyVersion.ToString() || C.debug)
            {
                List<BuyableItem> list = new List<BuyableItem>();
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
                    list.Add(new BuyableItem { Type = ItemType.Sellable, ItemId = item.Item1, C_ID = 28, Price = item.Item2, Loc = item.Item3 });
                }

                List<(uint, uint, uint)> poetics_sellable = [
                    //(14266, 375, 4), (15814, 375, 4), (17645, 375, 4), (17646, 375, 4), (17647, 375, 4), (17648, 375, 4), //Hismena
                    //(21279, 1600, 5), (22475, 1600, 5), (22477, 1600, 5), //Enna
                ];
                foreach (var item in poetics_sellable)
                {
                    list.Add(new BuyableItem { Type = ItemType.Sellable, ItemId = item.Item1, C_ID = 28, Price = item.Item2, Loc = item.Item3 });
                }

                List<(uint, uint, Boolean)> marks_collectable = [
                    (23367, 15000, false), (25002, 15000, false), (23370, 18000, false), (24234, 18000, false), (15442, 15000, false),
                    (23034, 15000, false), (24636, 15000, false), (23986, 20000, false), (39367, 1000, false),
                    (21277, 10000, true), (22473, 10000, true), (28890, 10000, true), (14273, 5000, true), (14274, 10000, true)
                ];
                foreach (var item in marks_collectable)
                {
                    list.Add(new BuyableItem { Type = ItemType.Collectable, ItemId = item.Item1, C_ID = 25, Price = item.Item2, Loc = 6 });
                    if(item.Item3)
                        list.Add(new BuyableItem { Type = ItemType.Sellable, ItemId = item.Item1, C_ID = 25, Price = item.Item2, Loc = 6 });
                }

                List<(uint, uint)> marks_sellable = [
                    (5594, 100), (5595, 100), (5596, 100), (5597, 100), (5598, 100), (10386, 100), (17837, 100), (33916, 100), // Dark Matter
                    (6954, 20), (6955, 20),
                    //Materia
                ];
                foreach (var item in marks_sellable)
                {
                    list.Add(new BuyableItem { Type = ItemType.Sellable, ItemId = item.Item1, C_ID = 25, Price = item.Item2, Loc = 6 });
                }

                List<(uint, uint)> nonlimited_sellable = [
                    (44143, 20), (44144, 20), (44145, 20), (44141, 20), (44142, 20), (44146, 20),

                ];
                foreach (var item in nonlimited_sellable)
                {
                    list.Add(new BuyableItem { Type = ItemType.Sellable, ItemId = item.Item1, C_ID = 46, Price = item.Item2, Loc = 7 });
                }

                C.Items = list;
            }
        }
    }
}
