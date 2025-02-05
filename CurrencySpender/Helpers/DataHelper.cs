using CurrencySpender.Classes;

namespace CurrencySpender.Helpers
{
    internal class DataHelper
    {
        public static void GenerateCurrencyList() {
            C.Currencies.Clear();
            List<TrackedCurrency> currencies = [
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 20, Threshold = 75000, MaxCount = 90000, }, // StormSeal
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 21, Threshold = 75000, MaxCount = 90000, }, // SerpentSeal
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 22, Threshold = 75000, MaxCount = 90000, }, // FlameSeal

                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 29, Threshold = 9999999, MaxCount = 9999999, }, // MGP

                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 28, Threshold = 1400, MaxCount = 2000, }, // Poetics
                new TrackedCurrency { Type = CurrencyType.NonLimitedTomestone, Threshold = 1400, MaxCount = 2000, }, // NonLimitedTomestone
                new TrackedCurrency { Type = CurrencyType.LimitedTomestone, Threshold = 1400, MaxCount = 2000, Enabled = false, }, // LimitedTomestone

                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 25, Threshold = 18000, MaxCount = 20000, }, // WolfMarks
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 36656, Threshold = 18000, MaxCount = 20000, }, // TrophyCrystals

                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 27, Threshold = 3500, MaxCount = 4000, }, // AlliedSeals
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 10307, Threshold = 3500, MaxCount = 4000, Children=[13625, 20308, 21103] }, // CenturioSeals
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 13625, Price = 500, Child=true}, // Centurio Clan Mark
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 20308, Price = 500, Child=true}, // Veteran's Clan Mark
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 21103, Price = 500, Child=true}, // Mythic Clan Mark

                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 26533, Threshold = 3500, MaxCount = 4000, }, // SackOfNuts

                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 26807, Threshold = 800, MaxCount = 1500, Children=[43961, 35833] }, // BicolorGemstones
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 43961, Price = 100, Child=true }, // Turali Gemstone Voucher
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 35833, Price = 100, Child=true }, // Gemstone Voucher

                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 33913, Threshold = 2500, MaxCount = 4000, Children=[12839] }, // Purple Crafters' Scrip
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 12839, Price = 25, Child=true },
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 41784, Threshold = 2500, MaxCount = 4000, }, // Orange Crafters' Scrip
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 33914, Threshold = 2500, MaxCount = 4000, }, // Purple Gatherers' Scrip
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 41785, Threshold = 2500, MaxCount = 4000, Children=[41807] }, // Orange Gatherers' Scrip
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 41807, Price = 1000, Child=true }, // Gemstone Voucher
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 28063, Threshold = 7500, MaxCount = 10000 }, // Skybuilders scripts

                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 37549, Threshold = 9999999, MaxCount = 9999999 }, // Seafarer's Cowrie
                new TrackedCurrency { Type = CurrencyType.Item, ItemId = 37550, Threshold = 9999999, MaxCount = 9999999 }, // Islander's Cowrie

            ];
            //foreach(var cur in currencies)
            //{
            //    PluginLog.Information($"Currency: {cur.Name}");
            //}
            C.Currencies = currencies;
        }
    }
}
