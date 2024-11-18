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

namespace CurrencySpender;

public class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;

    private const string CommandName = "/currency";

    public Config config { get; init; }

    public String homeWorld;

    public readonly WindowSystem WindowSystem = new("CurrencySpender");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private SpendingWindow SpendingWindow { get; init; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        config = PluginInterface.GetPluginConfig() as Config ?? new Config();
        ECommonsMain.Init(pluginInterface, this);

        if (config is { Currencies.Count: 0 } or { Currencies: null } or { Version: not 7 })
        {
            //Service.Log.Verbose("Generating Initial Currency List.");

            config.Currencies = DataHelper.GenerateCurrencyList();
            config.Save();
        }
        if (config is { Items.Count: 0 } or { Items: null } or { Version: not 7 })
        {
            //Service.Log.Verbose("Generating Initial Currency List.");

            config.Items = DataHelper.GenerateItemList();
            config.Save();
        }

        // you might normally want to embed resources and load them from the manifest stream
        //var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);
        SpendingWindow = new SpendingWindow(this);
        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(SpendingWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        //var text = Dalamud.Game.ClientState.Objects.Enums.ObjectKind.

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        Service.Log.Verbose("Item Unlocked - Should be True"+ItemHelper.CheckUnlockStatus(15814).ToString()); //unlocked
        Service.Log.Verbose("Item Unlocked - Should be False" + ItemHelper.CheckUnlockStatus(38457).ToString());
        //ItemHelper.CheckUnlockStatus(38457); //not unlocked
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleSpendingUI(uint CurrencyId, String name, List<BuyableItem> cItems)
    {
        SpendingWindow.collectableItems = cItems;
        SpendingWindow.CurrencyId = CurrencyId;
        SpendingWindow.CurrencyName = name;
        SpendingWindow.IsOpen = true;
    }
    public void ToggleMainUI() => MainWindow.Toggle();

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
