using System.IO;

namespace CurrencySpender.Windows.Config;

internal class ChangelogTab
{
    internal static void Draw()
    {
        if (P.changelogPath != null && File.Exists(P.changelogPath))
        {
            using (StreamReader reader = new StreamReader(P.changelogPath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("### "))
                    {
                        FontHelper.LargeText(line.Replace("#", "").Trim());
                    }
                    else if (line.Contains("## "))
                    {
                        FontHelper.LargerText(line.Replace("#", "").Trim());
                    }
                    else if (line.Contains("# "))
                    {
                        FontHelper.LargestText(line.Replace("#", "").Trim());
                    }
                    else
                    {
                        ImGui.TextWrapped(line.Trim());
                    }
                }
            }
        }
    }
}
