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
namespace CurrencySpender;

public class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;

    private const string CommandName = "/currency";

    public config Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("CurrencySpender");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        Configuration = PluginInterface.GetPluginConfig() as config ?? new config();

        // you might normally want to embed resources and load them from the manifest stream
        //var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

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
    public void ToggleMainUI() => MainWindow.Toggle();

    private void OnFrameworkUpdate(IFramework framework)
    {
        if (!Service.ClientState.IsLoggedIn) return;

        System.OverlayController.Update();
    }

    private void OnZoneChange(ushort e)
    {
        if (System.Config is { ChatWarning: false }) return;

        foreach (var currency in System.Config.Currencies.Where(currency => currency is { HasWarning: true, ChatWarning: true, Enabled: true }))
        {
            Service.ChatGui.Print($"{currency.Name} is {(currency.Invert ? "below" : "above")} threshold.", "CurrencyAlert", 43);
        }
    }

    private static List<TrackedCurrency> GenerateInitialList() => [
        new TrackedCurrency { Type = CurrencyType.Item, ItemId = 20, Threshold = 75000, Enabled = true, }, // StormSeal
        new TrackedCurrency { Type = CurrencyType.Item, ItemId = 21, Threshold = 75000, Enabled = true, }, // SerpentSeal
        new TrackedCurrency { Type = CurrencyType.Item, ItemId = 22, Threshold = 75000, Enabled = true, }, // FlameSeal

        new TrackedCurrency { Type = CurrencyType.Item, ItemId = 25, Threshold = 18000, Enabled = true, }, // WolfMarks
        new TrackedCurrency { Type = CurrencyType.Item, ItemId = 36656, Threshold = 18000, Enabled = true, }, // TrophyCrystals

        new TrackedCurrency { Type = CurrencyType.Item, ItemId = 27, Threshold = 3500, Enabled = true, }, // AlliedSeals
        new TrackedCurrency { Type = CurrencyType.Item, ItemId = 10307, Threshold = 3500, Enabled = true, }, // CenturioSeals
        new TrackedCurrency { Type = CurrencyType.Item, ItemId = 26533, Threshold = 3500, Enabled = true, }, // SackOfNuts

        new TrackedCurrency { Type = CurrencyType.Item, ItemId = 26807, Threshold = 800, Enabled = true, }, // BicolorGemstones

        new TrackedCurrency { Type = CurrencyType.Item, ItemId = 28, Threshold = 1400, Enabled = true, }, // Poetics
        new TrackedCurrency { Type = CurrencyType.NonLimitedTomestone, Threshold = 1400, Enabled = true, }, // NonLimitedTomestone
        new TrackedCurrency { Type = CurrencyType.LimitedTomestone, Threshold = 1400, Enabled = true, }, // LimitedTomestone

        new TrackedCurrency { Type = CurrencyType.Item, ItemId = 28063, Threshold = 7500, Enabled = true, }, // Skybuilders scripts
    ];
}
