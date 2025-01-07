namespace CurrencySpender.Windows;

internal class InstructionsTab
{
    internal static void Draw()
    {
        ImGuiEx.TextWrapped($"1. Select the desired currency and click the button to open the spending window.");
        ImGuiEx.TextWrapped($"2. Collectables, ventures, and sellable items will be displayed.");
        ImGuiEx.TextWrapped($"3. Ventures can be toggled in the settings.");
        ImGui.Separator();
        ImGuiEx.TextWrapped($"If your current Shared FATE rank cannot be retrieved, a warning will appear in the main tab.");

    }
}
