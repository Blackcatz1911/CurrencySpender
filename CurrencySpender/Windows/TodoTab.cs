namespace CurrencySpender.Windows;

internal class TodoTab
{
    internal static void Draw()
    {
        ImGui.TextWrapped("Whats is planned for later updates:");
        List<String> list = ["Support more currencies",
            "Teleportation button", "Checking if you can get certain items"];
        foreach (var item in list)
        {
            ImGui.TextWrapped("- " + item);
        }
    }
}
