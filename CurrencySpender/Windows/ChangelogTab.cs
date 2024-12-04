using Dalamud.Interface.GameFonts;
using Dalamud.Interface;
using System.IO;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Utility.Raii;
using CurrencySpender.Helpers;
using ImGuiScene;

namespace CurrencySpender.Windows;

internal class ChangelogTab
{
    internal static bool rendered = false;
    internal static void Draw()
    {
        if (P != null && P.changelogPath != null && File.Exists(P.changelogPath))
        {
            using (StreamReader reader = new StreamReader(P.changelogPath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("## "))
                    {
                        FontHelper.LargeText(line.Replace("#", "").Trim());
                    }
                    else if (line.Contains("# "))
                    {
                        FontHelper.LargerText(line.Replace("#", "").Trim());
                    }
                    else
                    {
                        ImGui.TextWrapped(line.Replace("#", "").Trim());
                    }
                }
            }
        }
    }
}
