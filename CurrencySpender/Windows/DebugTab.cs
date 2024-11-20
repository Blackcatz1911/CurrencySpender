using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace CurrencySpender.Windows;

internal class DebugTab
{
    internal unsafe static void Draw()
    {
        var agent = AgentMap.Instance();
        ImGui.Text("CurrentMapId: " + agent->CurrentMapId.ToString());
        ImGui.Text("CurrentTerritoryId: " + agent->CurrentTerritoryId.ToString());
        if (ImGuiEx.Button("Open Debug Window"))
        {
            P.debugTabWindow.IsOpen = true;
        }
    }
}
