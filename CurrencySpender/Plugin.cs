using System;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using CurrencySpender.Windows;
using CurrencySpender.Classes;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using CurrencySpender.Configuration;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Addon.Lifecycle;
using CurrencySpender.Helpers;
using ECommons;
using ECommons.Configuration;
using ECommons.Schedulers;
using ECommons.SimpleGui;
using CurrencySpender.Windows;
using ECommons.DalamudServices;
using ECommons.Logging;
using ECommons.Automation.NeoTaskManager;

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

    public String homeWorld = "";

    public readonly WindowSystem WindowSystem = new("CurrencySpender");

    internal WindowSystem ws;
    internal MainTabWindow mainTabWindow;
    internal ConfigTabWindow configTabWindow;
    internal SpendingWindow spendingWindow;

    internal TaskManager TaskManager;
    //private SpendingWindow SpendingWindow { get; init; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        //config = PluginInterface.GetPluginConfig() as Config ?? new Config();
        ECommonsMain.Init(pluginInterface, this);
        P = this;
        new TickScheduler(delegate
        {
            EzConfig.Migrate<Config>();
            config = EzConfig.Init<Config>();
            ws = new();
            spendingWindow = new();
            ws.AddWindow(spendingWindow);
            mainTabWindow = new();
            configTabWindow = new();
            PluginInterface.UiBuilder.Draw += ws.Draw;
            PluginInterface.UiBuilder.OpenConfigUi += delegate { configTabWindow.IsOpen = true; };
            PluginInterface.UiBuilder.OpenMainUi += delegate { mainTabWindow.IsOpen = true; };
            TaskManager = new() { };
            DataHelper.GenerateCurrencyList();
            DataHelper.GenerateItemList();
            EzCmd.Add("/cur", CommandHandler, "Open plugin interface");
            TaskManager.Enqueue(() => WebHelper.CheckPrices());
        });

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

        Service.Log.Verbose("Item Unlocked - Should be True"+ItemHelper.CheckUnlockStatus(15814).ToString()); //unlocked
        Service.Log.Verbose("Item Unlocked - Should be False" + ItemHelper.CheckUnlockStatus(38457).ToString());
        //ItemHelper.CheckUnlockStatus(38457); //not unlocked
    }

    private void CommandHandler(string command, string arguments)
    {
        if (arguments.EqualsIgnoreCase("test"))
        {
            spendingWindow.IsOpen = true;
        }
        else
        {
            mainTabWindow.IsOpen = !mainTabWindow.IsOpen;
        }
    }

    public void Dispose()
    {
        PluginInterface.UiBuilder.Draw -= ws.Draw;
        ECommonsMain.Dispose();
    }

    private void OnCommand(string command, string args)
    {
        mainTabWindow.IsOpen = true;
    }


    //public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleSpendingUI(uint CurrencyId, String name, List<BuyableItem> cItems)
    {
        TaskManager.Enqueue(() => WebHelper.CheckPrices());
        spendingWindow.collectableItems = cItems;
        spendingWindow.CurrencyId = CurrencyId;
        spendingWindow.CurrencyName = name;
        spendingWindow.IsOpen = true;
        //SpendingWindow.AllowPinning = true;
    }
    //public void ToggleMainUI() => MainWindow.Toggle();

    private void OnLogin()
    {
        homeWorld = GetHomeWorld();
        //Service.Log.Verbose("homeworld: " + homeWorld);
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

    public String GetHomeWorld()
    {
        // Ensure the LocalPlayer is not null (logged-in state)
        var localPlayer = Service.ClientState.LocalPlayer;
        if (localPlayer != null)
        {
            var homeWorld = Service.DataManager.Excel.GetSheet<Lumina.Excel.Sheets.World>().GetRow(localPlayer.CurrentWorld.RowId).Name.ExtractText();
            if (homeWorld != null)
            {
                return homeWorld;
            }
            else
            {
                return "Unknown";
            }
        }
        else
        {
            return "Unknown";
        }
    }
}
