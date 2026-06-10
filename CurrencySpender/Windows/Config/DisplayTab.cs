namespace CurrencySpender.Windows.Config;

public class DisplayTab
{
    internal static void Draw()
    {
        ImGui.TextWrapped("Will show a 'Open Currency Spender' button when the in-game currency window is opened.");
        ImGui.Checkbox("Show button", ref C.ShowButton);
        ImGui.Separator();
        ImGui.TextWrapped("Will open Currency Spender when the in-game currency window is opened.");
        ImGui.Checkbox("Open automatically", ref C.OpenAutomatically);
        ImGui.Separator();
        ImGui.TextWrapped("Will hide all windows when in a loading screen.");
        ImGui.Checkbox("Hide in loading screens", ref C.HideInLoadingScreens);
        ImGui.Separator();
        ImGui.TextWrapped("Will hide all windows when in a duty.");
        ImGui.Checkbox("Hide in duties", ref C.HideInDuties);
        ImGui.Separator();
        ImGui.TextWrapped("Will hide all windows when in combat.");
        ImGui.Checkbox("Hide in combat", ref C.HideInCombat);
        ImGui.Separator();
        ImGui.TextWrapped("Will hide all windows when in a cutscene.");
        ImGui.Checkbox("Hide in cutscenes", ref C.HideInCutscenes);
        ImGui.Separator();
        ImGui.TextWrapped("Will highlight the NPC when marking the flag or teleporting.");
        ImGui.Checkbox("Hightlight NPC", ref C.HighlightNpc);
        ImGui.Separator();
        ImGui.TextWrapped("Will highlight the menus where to find the item when marking the flag or teleporting.");
        ImGui.Checkbox("Hightlight Menus", ref C.HighlightMenu);
        ImGui.Separator();
    }
}
