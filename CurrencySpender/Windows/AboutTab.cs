using Dalamud.Interface.Colors;
using Dalamud.Interface;
using ECommons.DalamudServices;
using System.Diagnostics;

namespace CurrencySpender.Windows;

internal class AboutTab
{
    private static string GetImageURL()
    {
        return Svc.PluginInterface.Manifest.IconUrl ?? "";
    }

    internal static void Draw()
    {
        ImGuiEx.LineCentered("About0", delegate
        {
            ImGuiEx.Text($"{Svc.PluginInterface.Manifest.Name} - {Svc.PluginInterface.Manifest.AssemblyVersion}");
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
            ImGuiEx.TextWrapped("by Blackcatz1911");
        });

        ImGuiHelpers.ScaledDummy(10f);
        ImGuiEx.LineCentered("About2", delegate
        {
            if (ThreadLoadImageHandler.TryGetTextureWrap(GetImageURL(), out var texture))
            {
                ImGui.Image(texture.ImGuiHandle, new(200f, 200f));
            }
        });
        ImGuiHelpers.ScaledDummy(10f);
        List<String> thanks = ["The Dalamud Team", "Taurenkey", "Limiana", "MidoriKami"];
        ImGuiEx.LineCentered("About3", delegate
        {
            ImGui.TextWrapped("Special thanks to:");
        });
        foreach (var thank in thanks)
        {
            ImGuiEx.LineCentered("Thanks-"+thank, delegate
            {
                ImGui.TextWrapped("- "+ thank);
            });
        }
        ImGui.Separator();
        ImGui.TextWrapped("If you want something added, dont hesitate to make a ticket on the repo.");
        ImGui.Separator();
        ImGuiEx.LineCentered("About4", delegate
        {
            if (ImGui.Button("GitHub Repo"))
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = Svc.PluginInterface.Manifest.RepoUrl,
                    UseShellExecute = true
                });
            }
        });
    }
}
