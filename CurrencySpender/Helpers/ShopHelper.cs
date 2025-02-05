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
            List<ShopItem> Items = new List<ShopItem>();
            bool showAll = false; // TODO
            if (!showAll)
            {
                Items = Generator.items
                .Where(item => (item.Currency == Currency.ItemId || (Currency.Children != null && Currency.Children.Contains(item.Currency))) && item.Type.HasFlag(ItemType.Collectable) && !item.Disabled &&
                C.SelectedCollectableTypes.Contains((CollectableType)item.CollectableType) && !ItemHelper.IsUnlocked(item.Id))
                .ToList();
            }
            else
            {
                Items = Generator.items
                .Where(item => (item.Currency == Currency.ItemId || (Currency.Children != null && Currency.Children.Contains(item.Currency))) && item.Type.HasFlag(ItemType.Collectable) && !item.Disabled &&
                C.SelectedCollectableTypes.Contains((CollectableType)item.CollectableType))
                .ToList();
            }
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
