namespace CurrencySpender.Windows;

internal class ConfigTab
{
    internal static void Draw()
    {
        ImGui.TextWrapped("Settings");
        ImGui.Separator();
        ImGui.TextWrapped("Dont turn it on, unless you know what you are doing...");
        ImGui.Checkbox($"Debug Mode", ref C.debug);
    }
}
