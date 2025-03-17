using Dalamud.IoC;
using Dalamud.Plugin.Services;
using CurrencySpender.Windows;
using CurrencySpender.Classes;
using ECommons.Configuration;
using ECommons.Schedulers;
using ECommons.Automation.NeoTaskManager;
using CurrencySpender.Data;
using System.IO;
using Dalamud.Game.Command;

namespace CurrencySpender;

public sealed class Plugin : IDalamudPlugin
{
    public string Name => "Currency Spender";
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;

    internal static Plugin P;
    internal static Config C => P.config;

    public Config config;

    public readonly WindowSystem WindowSystem = new("CurrencySpender");

    internal WindowSystem ws;
    internal MainTabWindow mainTabWindow;
    internal ConfigTabWindow configTabWindow;
    internal SpendingWindow spendingWindow;
    internal DebugTabWindow debugTabWindow;
    internal ConfigWizardWindow configWizard;

    internal string? changelogPath;
    public string Version;
    public bool Problem = false;
    internal TaskManager TaskManager;
    public List<TrackedCurrency> Currencies;
    //private SpendingWindow SpendingWindow { get; init; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        //config = PluginInterface.GetPluginConfig() as Config ?? new Config();
        ECommonsMain.Init(pluginInterface, this, Module.DalamudReflector);
        P = this;
        changelogPath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "CHANGELOG.md");

        CommandManager.AddHandler("/cur", new CommandInfo(OnCommand)
        {
            HelpMessage = "Short command. Also pairable with all arguments. Arguments: config, c, settings, s"
        });

        CommandManager.AddHandler("/currencyspender", new CommandInfo(OnCommand)
        {
            HelpMessage = "Open plugin interface. Also pairable with all arguments. Arguments: config, c, settings, s"
        });

        _ = new TickScheduler(delegate
        {
            EzConfig.Migrate<Config>();
            config = EzConfig.Init<Config>();
            ws = new();
            spendingWindow = new();
            debugTabWindow = new();
            mainTabWindow = new();
            configTabWindow = new();
            configWizard = new();
            PluginInterface.UiBuilder.Draw += ws.Draw;
            PluginInterface.UiBuilder.OpenConfigUi += delegate { configTabWindow.IsOpen = true; };
            PluginInterface.UiBuilder.OpenMainUi += delegate { mainTabWindow.IsOpen = true; };
            TaskManager = new() { };
            Currencies = TrackedCurrency.GenerateCurrencyList();
            Generator.init();
            //VersionHelper.CheckGameVersion();
            PlayerHelper.init();
            VersionHelper.CheckVersion();
            //PluginLog.Debug($"unlocked: {ItemHelper.IsUnlocked(36636)}");
            //mainTabWindow.IsOpen = true;
        });
        
        //PlayerHelper.init();
        //Generator.init();
        FontHelper.SetupFonts();
        Version = VersionHelper.GetVersion();
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
        //PluginLog.Information($"Unlocked 38443: {ItemHelper.IsUnlocked(38443)}");
        //PluginLog.Information($"Unlocked 15613: {ItemHelper.IsUnlocked(15613)}");

#if HAS_LOCAL_CS
        FFXIVClientStructs.Interop.Generated.Addresses.Register();
        //Addresses.Register();
        Resolver.GetInstance.Setup(
            Service.sigScanner.SearchBase,
            Service.DataManager.GameData.Repositories["ffxiv"].Version,
            new FileInfo(Path.Join(pluginInterface.ConfigDirectory.FullName, "SigCache.json")));
        Resolver.GetInstance.Resolve();
#endif
    }

    public void Dispose()
    {
        PluginInterface.UiBuilder.Draw -= ws.Draw;
        ECommonsMain.Dispose();
        FontHelper.DisposeFonts();
    }

    private void OnCommand(string command, string args)
    {
        if (args.EqualsIgnoreCase("debug") || args.EqualsIgnoreCase("d"))
        {
            debugTabWindow.IsOpen = true;
        }
        else if (args.EqualsIgnoreCase("config") || args.EqualsIgnoreCase("c")
            || args.EqualsIgnoreCase("settings") || args.EqualsIgnoreCase("s"))
        {
            configTabWindow.IsOpen = true;
        }
        else
        {
            mainTabWindow.IsOpen = !mainTabWindow.IsOpen;
        }
    }
    

    public void ToggleSpendingUI(TrackedCurrency Currency)
    {
        if(C.ShowSellables) P.TaskManager.Enqueue(() => WebHelper.CheckAll(Currency.ItemId));
        spendingWindow.GetData(Currency);
        spendingWindow.IsOpen = true;
    }

    private void OnLogin()
    {
        PluginLog.Debug("OnLogin");
        P.TaskManager.Enqueue(() => PlayerHelper.reset());
        P.TaskManager.Enqueue(() => PlayerHelper.init());
    }

    private void OnLogout(int type, int code)
    {
        PluginLog.Debug("OnLogout");
        P.TaskManager.Enqueue(() => PlayerHelper.reset());
    }
    
    internal static readonly Dictionary<CollectableType, string> CollectableTypeLabels = new()
    {
        { CollectableType.Mount, "Mounts" },
        { CollectableType.Minion, "Minions" },
        { CollectableType.Scroll, "Orchestration Scrolls" },
        { CollectableType.Emote, "Emotes" },
        { CollectableType.Hairstyle, "Hairstyles" },
        { CollectableType.Barding, "Bardings" },
        { CollectableType.RidingMap, "Riding Maps" },
        { CollectableType.Facewear, "Facewear" },
        { CollectableType.FramersKit, "Framer's Kits" },
        { CollectableType.TTCard, "Triple Triad Cards" },
        { CollectableType.Mahjong, "Mahjong Voices" },
    };
}
