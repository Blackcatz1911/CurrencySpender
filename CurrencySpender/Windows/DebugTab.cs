using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace CurrencySpender.Windows;

internal class DebugTab
{
    internal unsafe static void Draw()
    {
        var agent = AgentMap.Instance();
        ImGui.Text("CurrentMapId: " + agent->CurrentMapId.ToString());
        ImGui.Text("CurrentTerritoryId: " + agent->CurrentTerritoryId.ToString());
        ImGui.Text("GCRankings:");
        foreach(var rank in PlayerHelper.GCRanks)
        {
            ImGui.Text($"{rank.Key} - {rank.Value}");
        }
        ImGui.Text("Fate Rank:");
        foreach (var rank in PlayerHelper.SharedFateRanks)
        {
            ImGui.Text($"{rank.Key} - {rank.Value}");
        }
        if (ImGuiEx.Button("Open Debug Window"))
        {
            P.debugTabWindow.IsOpen = true;
        }
    }
}
