using CurrencySpender.Classes;

namespace CurrencySpender.Data;

internal static class Generator
{
    // private static string ShopsPath => Path.Combine(PluginInterface.GetPluginConfigDirectory(), "shops.json");
    // private static string ItemsPath => Path.Combine(PluginInterface.GetPluginConfigDirectory(), "items.json");
    
    public static List<Shop> shops = new ();
    public static List<ShopItem> items = new ();
    public static void init()
    {
        // LoadData();
        // if((shops.Count == 0 && items.Count == 0) || VersionHelper.IsNewVersion() || VersionHelper.IsNewGameVersion() || C.Debug)
        // {
            PluginLog.Information("New init because:");
            if((shops.Count == 0 && items.Count == 0)) PluginLog.Information("shops.Count == 0 && items.Count == 0");
            if(VersionHelper.IsNewVersion()) PluginLog.Information("VersionHelper.IsNewVersion()");
            if(VersionHelper.IsNewGameVersion()) PluginLog.Information("VersionHelper.IsNewGameVersion()");
            P.TaskManager.Enqueue(() => ShopGen.init());
            P.TaskManager.Enqueue(() => ItemGen.init());
            // P.TaskManager.Enqueue(() => SaveData());
        // }
    }
    // private static void LoadData()
    // {
    //     PluginLog.Information("Loading data");
    //     if (File.Exists(ShopsPath))
    //         shops = JsonSerializer.Deserialize<List<Shop>>(File.ReadAllText(ShopsPath)) ?? new();
    //     if (File.Exists(ItemsPath))
    //         items = JsonSerializer.Deserialize<List<ShopItem>>(File.ReadAllText(ItemsPath)) ?? new();
    // }

    // public static void SaveData()
    // {
    //     File.WriteAllText(ShopsPath, JsonSerializer.Serialize(shops, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true }));
    //     File.WriteAllText(ItemsPath, JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true }));
    // }
}
