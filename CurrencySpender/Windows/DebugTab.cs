using System;
using CurrencySpender;
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
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkEventDispatcher;

namespace CurrencySpender.Windows;

internal class DebugTab
{
    internal unsafe static void Draw()
    {
        var agent = AgentMap.Instance();
        ImGui.Text("CurrentMapId: " + agent->CurrentMapId.ToString());
        ImGui.Text("CurrentTerritoryId: " + agent->CurrentTerritoryId.ToString());
    }
}
