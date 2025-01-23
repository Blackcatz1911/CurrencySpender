namespace CurrencySpender.Windows;

internal class InstructionsTab
{
    internal static void Draw()
    {
        ImGuiEx.TextWrapped("1. Select the desired currency and click the button to open the spending window.");
        ImGuiEx.TextWrapped("2. Collectables, ventures, and sellable items will be displayed.");
        ImGuiEx.TextWrapped("3. Ventures and collectables can be toggled in the settings.");
        ImGuiEx.TextWrapped("4. Button 'Flag' will open your map with a map marker to the vendor. " +
            $"Button 'TP' will teleport you to the nearest aetheryte and also places a flag marker on your map");
        ImGui.Separator();
        ImGuiEx.TextWrapped("If your current Shared FATE rank cannot be retrieved, a warning will appear in the main tab. " +
            $"Simply click the button to open the shared FATE rank screen to allow the plugin to gather the necessary information.");

    }
}
