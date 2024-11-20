namespace CurrencySpender.Windows;

internal class ChangelogTab
{
    internal static void Draw()
    {
        ImGui.TextWrapped("0.0.2.0");
        ImGui.TextWrapped("- Added a changelog");
        ImGui.TextWrapped("- Improved Universalis handling");
        ImGui.TextWrapped("- Barding, Emotes, Hairstyles and TT Cards are now correctly tracked");
        ImGui.TextWrapped("- Added weekly sales to the sellables table");
        ImGui.TextWrapped("- Added faded copies to the collectables, when crafted scroll is not yet unlocked");
        ImGui.Separator();
        ImGui.TextWrapped("0.0.1.0");
        ImGui.TextWrapped("- Initial Release");
    }
}
