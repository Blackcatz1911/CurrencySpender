using CurrencySpender.Classes;
using CurrencySpender.Data;

namespace CurrencySpender.Helpers
{
    internal static class ShopHelper
    {
        public static List<ShopItem> GetSellableItems(TrackedCurrency Currency)
        {
            List<ShopItem> SellableItems = new List<ShopItem>();
            if (Currency.ItemId == 26807)
            {
                //var shops = Generator.shops
                //    .Where(shop => shop.RequiredLevel > 0)
                //    .ToList();
                //foreach (var shop in shops)
                //{
                //    PluginLog.Verbose(shop.ToString());
                //}
                SellableItems = Generator.items
                    .Where(item => (item.Currency == Currency.ItemId) && item.Type.HasFlag(ItemType.Tradeable)
                    && !item.Disabled && !item.Shop.Disabled)
                    .ToList();
                //PluginLog.Verbose($"{SellableItems.Count}");
            }
            else
            {
                SellableItems = Generator.items
                    .Where(item => (item.Currency == Currency.ItemId) && item.Type.HasFlag(ItemType.Tradeable) && !item.Disabled && !item.Shop.Disabled)
                    .ToList();
                //PluginLog.Verbose($"{SellableItems.Count}");
            }
            return SellableItems;
        }
        public static List<ShopItem> GetCollectableItems(TrackedCurrency Currency)
        {
            List<ShopItem> Items = Generator.items
                .Where(item => (item.Currency == Currency.ItemId || (Currency.Children != null && Currency.Children.Contains(item.Currency))) && item.Type.HasFlag(ItemType.Collectable) && !item.Disabled && !ItemHelper.CheckUnlockStatus(item.Id))
                .ToList();
            return Items;
        }
        public static List<ShopItem> GetVentures(TrackedCurrency Currency)
        {
            return Generator.items
                .Where(item => item.Currency == Currency.ItemId && item.Type.HasFlag(ItemType.Venture))
                .ToList();
        }
        public static List<ShopItem> GetItemsOfInterest(TrackedCurrency Currency)
        {
            return Generator.items
                .Where(item => item.Currency == Currency.ItemId && C.ItemsOfInterest.Contains(item.Id) && !item.Shop.Disabled)
                .ToList();
        }
    }
}
