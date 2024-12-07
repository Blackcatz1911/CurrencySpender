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
using InteropGenerator.Runtime;

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

    internal string? changelogPath;
    public string Version;
    public bool Problem = false;
    internal TaskManager TaskManager;
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
            ws.AddWindow(spendingWindow);
            mainTabWindow = new();
            configTabWindow = new();
            PluginInterface.UiBuilder.Draw += ws.Draw;
            PluginInterface.UiBuilder.OpenConfigUi += delegate { configTabWindow.IsOpen = true; };
            PluginInterface.UiBuilder.OpenMainUi += delegate { mainTabWindow.IsOpen = true; };
            TaskManager = new() { };
            DataHelper.GenerateCurrencyList();
            ItemHelper.initHairStyles();
        });
        //PlayerHelper.init();
        //Generator.init();
        FontHelper.SetupFonts();
        Version = StringHelper.ToSemVer(P.GetType().Assembly.GetName().Version.ToString());
        Service.ClientState.Login += OnLogin;
#if HAS_LOCAL_CS
        FFXIVClientStructs.Interop.Generated.Addresses.Register();
        //Addresses.Register();
        Resolver.GetInstance.Setup(
            Service.sigScanner.SearchBase,
            Service.DataManager.GameData.Repositories["ffxiv"].Version,
            new FileInfo(Path.Join(pluginInterface.ConfigDirectory.FullName, "SigCache.json")));
        Resolver.GetInstance.Resolve();
#endif
        if (Service.ClientState.IsLoggedIn)
        {
            PluginLog.Verbose("logged in");
            PlayerHelper.init();
        }
        else
        {
            PluginLog.Verbose("not logged in");
        }
        //var fateProgressSheet = Service.DataManager.GetExcelSheet<FateProgressUI>();
        //foreach (var row in fateProgressSheet)
        //{
        //    var zoneId = row.RowId;       // Zone ID
        //    var fateRank = row.ReqFatesToRank4;      // Current Shared FATE rank
        //    //var maxRank = row.;    // Maximum possible rank
        //    PluginLog.Verbose($"Zone {zoneId}: Rank {fateRank}");
        //}
        //PluginLog.Verbose(PlayerHelper.GCRankMaelstrom.ToString());
        //ECommons.ImGuiMethods.ImGuiEx.Text();
        // you might normally want to embed resources and load them from the manifest stream
        //var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");


        //ConfigWindow = new Windows.ConfigWindow(this);
        //MainWindow = new MainWindow(this);
        //SpendingWindow = new SpendingWindow(this);
        //WindowSystem.AddWindow(ConfigWindow);
        ////WindowSystem.AddWindow(MainWindow);
        //ws.AddWindow(SpendingWindow);

        //PluginInterface.UiBuilder.Draw += DrawUI;

        //var text = Dalamud.Game.ClientState.Objects.Enums.ObjectKind.

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        //PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin

        //PluginLog.Verbose("Item Unlocked - Should be True"+ItemHelper.CheckUnlockStatus(15814).ToString()); //unlocked
        //PluginLog.Verbose("Item Unlocked - Should be False" + ItemHelper.CheckUnlockStatus(38457).ToString());
        //ItemHelper.CheckUnlockStatus(38457); //not unlocked
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
        P.TaskManager.Enqueue(() => WebHelper.CheckAll(Currency.ItemId));
        spendingWindow.GetData(Currency);
        spendingWindow.IsOpen = true;
    }

    private void OnLogin()
    {
        PlayerHelper.init();
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        if (!Service.ClientState.IsLoggedIn) return;

        //System.OverlayController.Update();
    }

    private void OnZoneChange(ushort e)
    {
        //if (System.Config is { ChatWarning: false }) return;

        //foreach (var currency in System.Config.Currencies.Where(currency => currency is { HasWarning: true, ChatWarning: true, Enabled: true }))
        //{
        //    Service.ChatGui.Print($"{currency.Name} is {(currency.Invert ? "below" : "above")} threshold.", "CurrencyAlert", 43);
        //}
    }
}
