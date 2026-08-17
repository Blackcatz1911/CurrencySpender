using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using CurrencySpender.Classes;

namespace CurrencySpender.Windows.Config;

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
            P.DebugWindow.IsOpen = true;
        }
        ImGui.Separator();
        var localPlayer = Service.ObjectTable.LocalPlayer;
        var target = localPlayer?.TargetObject;
        ImGui.Text("Current NpcId: " + (target != null ? target.DataId.ToString() : "no target"));
        if (ImGuiEx.Button("Copy NpcId"))
        {
            if (target != null)
            {
                ImGui.SetClipboardText(target.DataId.ToString());
                PluginLog.Information($"Copied NpcId {target.DataId}");
            }
            else
            {
                PluginLog.Information("No NPC targeted");
            }
        }
        ImGui.SameLine();
        if (ImGuiEx.Button("Copy Target"))
        {
            if (localPlayer != null)
            {
                var pos = localPlayer.Position;
                var targetString = $"Target = new({pos.X:F1}f, {pos.Y:F1}f, {pos.Z:F1}f)";
                ImGui.SetClipboardText(targetString);
                PluginLog.Information($"Copied: {targetString}");
            }
        }
        ImGui.SameLine();
        if (localPlayer != null)
        {
            var pos = localPlayer.Position;
            ImGui.Text($"({pos.X:F1}, {pos.Y:F1}, {pos.Z:F1})");
        }
        ImGui.Separator();
        uint[] alliedSealNpcs = [1002387, 1002390, 1002393, 1009552, 1009152, 1001379];
        var missingTargets = Location.Locations
            .Where(loc => loc.Target == Vector3.Zero && !alliedSealNpcs.Contains(loc.NpcId))
            .Where(loc => loc.BackupNpc == null)
            .OrderBy(loc => loc.MapId)
            .ThenBy(loc => loc.NpcId)
            .ToList();
        ImGui.Text($"Locations without a target: {missingTargets.Count}");
        if (missingTargets.Count > 0)
        {
            ImGui.BeginChild("MissingTargets", new Vector2(0, 300), true);
            foreach (var loc in missingTargets)
            {
                if (ImGuiEx.Button($"TP##{loc.NpcId}-{loc.TerritoryId}-{loc.MapId}"))
                {
                    if (agent->CurrentTerritoryId != loc.TerritoryId)
                        loc.Teleport();
                    else
                        loc.MoveTo();
                    var marker = loc.GetMapMarker();
                    if (marker != null) Service.GameGui.OpenMapWithMapLink(marker);
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    UiHelper.LeftAlign(loc.Zone);
                    ImGui.EndTooltip();
                }
                ImGui.SameLine();
                ImGui.Text($"{loc.NpcId} | {loc.Zone} | ({loc.Position.X:F1}, {loc.Position.Y:F1})");
            }
            ImGui.EndChild();
        }
    }
}
