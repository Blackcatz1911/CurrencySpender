using CurrencySpender.Classes;

namespace CurrencySpender.Data;

internal static class Generator
{
    public static List<Shop> shops = new ();
    public static List<ShopItem> items = new ();
    public static bool ShopsFinished = false;
    public static bool ItemsFinished = false;
    public static bool AllFinished = false;
        
    public static void init()
    {
        PluginLog.Information("New init because:");
        if((shops.Count == 0 && items.Count == 0)) PluginLog.Information("shops.Count == 0 && items.Count == 0");
        if(VersionHelper.IsNewVersion()) PluginLog.Information("VersionHelper.IsNewVersion()");
        if(VersionHelper.IsNewGameVersion()) PluginLog.Information("VersionHelper.IsNewGameVersion()");
        P.TaskManager.Enqueue(() => ShopGen.init());
        P.TaskManager.Enqueue(() => ItemGen.init());
    }
}
