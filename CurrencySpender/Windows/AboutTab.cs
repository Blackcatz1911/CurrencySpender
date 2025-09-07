using Dalamud.Interface.Colors;
using Dalamud.Interface;
using ECommons.DalamudServices;

namespace CurrencySpender.Windows;

internal class AboutTab
{
    internal static void Draw()
    {
        ImGuiEx.LineCentered("About0", delegate
        {
            ImGuiEx.Text($"{Svc.PluginInterface.Manifest.Name} - {P.Version}");
        });
        ImGui.Separator();

        ImGuiEx.LineCentered("About1-1", delegate
        {
            ImGuiEx.TextWrapped("Developed with");
        });
        ImGuiEx.LineCentered("About1-2", delegate
        {
            ImGui.PushFont(UiBuilder.IconFont);
            ImGuiEx.TextWrapped(ImGuiColors.DalamudRed, FontAwesomeIcon.Heart.ToIconString());
            ImGui.PopFont();
        });
        ImGuiEx.LineCentered("About1-3", delegate
        {
            ImGuiEx.TextWrapped("by Blackcatz1911/catz/Ayaya");
        });
        ImGui.Separator();
        List<String> thanks = ["The Dalamud Team", "FFXIVClientStructs", "Yuki", "Limiana", "Taurenkey", "MidoriKami", "CriticalImpact", "Haselnussbomber"];
        ImGuiEx.LineCentered("About3", delegate
        {
            ImGui.TextWrapped("Special thanks to:");
        });
        foreach (var thank in thanks)
        {
            ImGuiEx.LineCentered("Thanks-"+thank, delegate
            {
                ImGui.TextWrapped("- " + thank + " -");
            });
        }
        ImGui.Separator();
        ImGui.TextWrapped("If you have suggestions or feature requests, feel free to open an issue on the repository.");
        ImGui.Separator();
        ImGuiEx.LineCentered("About4", delegate
        {
            if (ImGui.Button("GitHub repository"))
            {
                GenericHelpers.ShellStart("https://github.com/Blackcatz1911/CurrencySpender");
            }
            ImGui.SameLine();
            if (ImGui.Button("Ko-fi"))
            {
                GenericHelpers.ShellStart("https://ko-fi.com/Blackcatz1911");
            }
        });
    }
}
