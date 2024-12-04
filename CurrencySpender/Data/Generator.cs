using CurrencySpender.Classes;

namespace CurrencySpender.Data;

internal static class Generator
{
    public static List<Shop> shops = new List<Shop>();
    public static List<ShopItem> items = new List<ShopItem>();
    public static void init()
    {
        P.TaskManager.Enqueue(() => ShopGen.init());
        P.TaskManager.Enqueue(() => ItemGen.init());
    }
}