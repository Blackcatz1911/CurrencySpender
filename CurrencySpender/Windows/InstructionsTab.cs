using Dalamud.Interface.Colors;
using Dalamud.Interface;
using ECommons.DalamudServices;
using System.Diagnostics;

namespace CurrencySpender.Windows;

internal class InstructionsTab
{
    internal static void Draw()
    {
        ImGuiEx.TextWrapped($"1. Select the wanted currency");
        ImGuiEx.TextWrapped($"2. Collectables, Ventures and sellable items will be displayed");
        ImGuiEx.TextWrapped($"3. Ventures can be toggled in the settings");
        ImGui.Separator();
        ImGuiEx.TextWrapped($"If there are any problems or something needed you will see another tab called 'Problems'");

    }
}
