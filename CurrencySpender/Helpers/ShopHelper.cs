using CurrencySpender.Classes;
using CurrencySpender.Data;

namespace CurrencySpender.Helpers
{
    internal static class ShopHelper
    {
        public static List<ShopItem> GetSellableItems(TrackedCurrency Currency)
        {
            List<ShopItem> items;
            if (Currency.ItemId == 26807)
            {
                items = Generator.items
                                 .Where(item => (item.Currency == Currency.ItemId) && item.Type.HasFlag(ItemType.Tradeable)
                                                && !item.Disabled && !item.Shop.Disabled && item.HasSoldWeek >= C.MinSales)
                                 .ToList();
                //PluginLog.Verbose($"{SellableItems.Count}");
            }
            else
            {
                items = Generator.items
                                 .Where(item => (item.Currency == Currency.ItemId) && item.Type.HasFlag(ItemType.Tradeable) && !item.Disabled && !item.Shop.Disabled &&
                                                item.HasSoldWeek >= C.MinSales)
                                 .ToList();
                //PluginLog.Verbose($"{SellableItems.Count}");
            }
            return items;
        }
        public static List<ShopItem> GetCollectableItems(TrackedCurrency currency)
        {
            List<ShopItem> items;
            bool showAll = false;
            if (!showAll)
            {
                items = Generator.items
                                 .Where(item => (item.Currency == currency.ItemId || (currency.Children != null && currency.Children.Contains(item.Currency))) && item.Type.HasFlag(ItemType.Collectable) && !item.Disabled &&
                                                C.SelectedCollectableTypes.Contains((CollectableType)item.CollectableType) && !ItemHelper.IsUnlocked(item.Id))
                                 .ToList();
            }
            else
            {
                items = Generator.items
                                 .Where(item => (item.Currency == currency.ItemId || (currency.Children != null && currency.Children.Contains(item.Currency))) && item.Type.HasFlag(ItemType.Collectable) && !item.Disabled &&
                                                C.SelectedCollectableTypes.Contains((CollectableType)item.CollectableType))
                                 .ToList();
            }
            return items;
        }
        public static List<ShopItem> GetVentures(TrackedCurrency currency)
        {
            return Generator.items
                .Where(item => item.Currency == currency.ItemId && item.Type.HasFlag(ItemType.Venture))
                .ToList();
        }
        public static List<ShopItem> GetItemsOfInterest(TrackedCurrency currency)
        {
            return Generator.items
                .Where(item => item.Currency == currency.ItemId && C.ItemsOfInterest.Contains(item.Id) && !item.Shop.Disabled)
                .ToList();
        }
    }
}
