using CurrencySpender.Classes;

namespace CurrencySpender.Helpers
{
    internal class DataHelper
    {
        public static void GenerateCurrencyList() {
            if(C.Currencies.Count == 0 || C.Version != P.Version || C.Debug)
            {
                List<TrackedCurrency> currencies = [
                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 20, Threshold = 75000, MaxCount = 90000, Enabled = true, }, // StormSeal
                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 21, Threshold = 75000, MaxCount = 90000, Enabled = true, }, // SerpentSeal
                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 22, Threshold = 75000, MaxCount = 90000, Enabled = true, }, // FlameSeal

                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 25, Threshold = 18000, MaxCount = 20000, Enabled = true, }, // WolfMarks
                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 36656, Threshold = 18000, MaxCount = 20000, Enabled = true, }, // TrophyCrystals

                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 27, Threshold = 3500, MaxCount = 4000, Enabled = true, }, // AlliedSeals
                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 10307, Threshold = 3500, MaxCount = 4000, Enabled = true, Children=[13625] }, // CenturioSeals
                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 13625, Price = 100 , Enabled = true}, // Centurio Clan Mark

                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 26533, Threshold = 3500, MaxCount = 4000, Enabled = true, }, // SackOfNuts

                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 26807, Threshold = 800, MaxCount = 1500, Enabled = true, Children=[43961, 35833] }, // BicolorGemstones
                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 43961, Price = 100 , Enabled = true }, // Turali Gemstone Voucher
                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 35833, Price = 100 , Enabled = true }, // Gemstone Voucher

                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 28, Threshold = 1400, MaxCount = 2000, Enabled = true, }, // Poetics
                    new TrackedCurrency { Type = CurrencyType.NonLimitedTomestone, Threshold = 1400, MaxCount = 2000, Enabled = true, }, // NonLimitedTomestone
                    new TrackedCurrency { Type = CurrencyType.LimitedTomestone, Threshold = 1400, MaxCount = 2000, Enabled = false, }, // LimitedTomestone

                    new TrackedCurrency { Type = CurrencyType.Item, ItemId = 28063, Threshold = 7500, MaxCount = 10000, Enabled = false, }, // Skybuilders scripts
                ];
                C.Currencies = currencies;
            }
        }
    }
}
