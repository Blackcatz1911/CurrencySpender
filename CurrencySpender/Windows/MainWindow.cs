using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CurrencySpender.Classes;
using CurrencySpender.Configuration;
using CurrencySpender.Helpers;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkEventDispatcher;

namespace CurrencySpender.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin plugin;

    public String MapId = "";
    public String TerritoryId = "";
    public String MapMarker = "";

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin)
        : base("Currency Spender Main", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        //GoatImagePath = goatImagePath;
        this.plugin = plugin;
    }

    public void Dispose() { }

    public unsafe void MarkerInfo()
    {
        var agent = AgentMap.Instance();
        //Service.Log.Verbose("CurrentMapId: " + agent->CurrentMapId.ToString());
        //Service.Log.Verbose("CurrentTerritoryId: " + agent->CurrentTerritoryId.ToString());
        MapId = agent->CurrentMapId.ToString();
        TerritoryId = agent->CurrentTerritoryId.ToString();
        var marker = agent->FlagMapMarker;
        MapMarker = marker.MapMarker.X + "-" + marker.MapMarker.Y;
        //Service.Log.Verbose("MapMarker: " + marker.MapMarker.X + "-" + marker.MapMarker.Y);
    }

    public override void Draw()
    {
        if (ImGui.Button("Display Map Marker"))
        {
            if (Service.ClientState.LocalPlayer != null)
            {
                MarkerInfo();
            }
        }
        ImGui.Text("CurrentMapId: " + MapId);
        ImGui.Text("CurrentTerritoryId: " + TerritoryId);
        ImGui.Text("MapMarker: " + MapMarker);

        if (ImGui.Button("Show Settings"))
        {
            plugin.ToggleConfigUI();
        }

        ImGui.Spacing();

        foreach (TrackedCurrency currency in plugin.config.Currencies)
        {
            if (currency.Enabled && ImGui.Button(currency.Name+": "+currency.CurrentCount.ToString()))
            {
                List<uint> itemIds = new List<uint>();
                foreach (BuyableItem item in plugin.config.Items)
                {
                    if (item.C_ID == currency.ItemId) itemIds.Add(item.ItemId);
                }

                var result = Task.Run(async () => await WebHelper.CheckPrices(itemIds, plugin)).Result;

                List<BuyableItem> collectableItems = plugin.config.Items
                    .Where(item => item.C_ID == currency.ItemId && item.Type == ItemType.Collectable && !ItemHelper.CheckUnlockStatus(item.ItemId))
                    .ToList();

                List<BuyableItem> filteredItems = plugin.config.Items
                    .Where(item => item.C_ID == currency.ItemId && item.Type == ItemType.Sellable)
                    .ToList();

                foreach (BuyableItem item in filteredItems)
                {
                    double buyableAmountD = (currency.CurrentCount / item.Price);
                    item.AmountCanBuy = (uint)Math.Floor(buyableAmountD);
                    item.Profit = item.CurrentPrice * item.AmountCanBuy;
                }
                //Service.Log.Verbose("CheckPrices: " + result);
                try { plugin.ToggleSpendingUI(currency.ItemId, currency.Name, collectableItems); } catch (Exception ex) { Service.Log.Error(ex.ToString()); }
            }
        }

        //ImGui.Text("Have a goat:");
        ////var goatImage = Plugin.TextureProvider.GetFromFile(goatImagePath).GetWrapOrDefault();
        //if (goatImage != null)
        //{
        //    ImGuiHelpers.ScaledIndent(55f);
        //    ImGui.Image(goatImage.ImGuiHandle, new Vector2(goatImage.Width, goatImage.Height));
        //    ImGuiHelpers.ScaledIndent(-55f);
        //}
        //else
        //{
        //    ImGui.Text("Image not found.");
        //}
    }
}
