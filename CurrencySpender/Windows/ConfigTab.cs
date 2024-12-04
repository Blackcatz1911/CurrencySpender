namespace CurrencySpender.Windows;

internal class ConfigTab
{
    internal static void Draw()
    {
        ImGui.TextWrapped("Settings");
        ImGui.Separator();
        ImGui.TextWrapped("Shows you if you can buy ventures with it.");
        ImGui.Checkbox($"Show Ventures", ref C.ShowVentures);
        ImGui.Separator();
        ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(1.0f, 0.25f, 0.25f, 1.0f)); // RGBA for red
        ImGui.TextWrapped("Dont turn it on, unless you know what you are doing...");
        ImGui.Checkbox($"Debug Mode", ref C.Debug);
        ImGui.PopStyleColor();
    }
}
