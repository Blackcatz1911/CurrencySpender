using CurrencySpender.Classes;
using CurrencySpender.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencySpender.Helpers
{
    internal static class ShopHelper
    {
        public static List<ShopItem> GetItems(TrackedCurrency Currency)
        {
            List<ShopItem> filteredItems = new List<ShopItem>();
            if (Currency.ItemId == 26807)
            {
                //var shops = Generator.shops
                //    .Where(shop => shop.RequiredLevel > 0)
                //    .ToList();
                //foreach (var shop in shops)
                //{
                //    PluginLog.Verbose(shop.ToString());
                //}
                filteredItems = Generator.items
                    .Where(item => item.Currency == Currency.ItemId && item.Type.HasFlag(ItemType.Tradeable)
                    && !item.Disabled && !item.Shop.Disabled)
                    .ToList();
                //PluginLog.Verbose($"{filteredItems.Count}");
            }
            else
            {
                filteredItems = Generator.items
                    .Where(item => item.Currency == Currency.ItemId && item.Type.HasFlag(ItemType.Tradeable))
                    .ToList();
                //PluginLog.Verbose($"{filteredItems.Count}");
            }
            return filteredItems;
        }
    }
}
