using CurrencySpender.Classes;
using CurrencySpender.Windows;

namespace CurrencySpender.Helpers
{
    internal class VersionHelper
    {
        public static string GetVersion()
        {
            string? version = P?.GetType()?.Assembly?.GetName()?.Version?.ToString();
            if (version == null) return "";
            return ToSemVer(version);
        }
        public static void CheckVersion()
        {
            if (C.Version == "0.0.0.0") C.Version = "0.0.0";
            if (LowerVersionThan("1.1.0"))
            {
                PluginLog.Information("Version below 1.1.0 found");
                foreach (CollectableType type in Enum.GetValues(typeof(CollectableType)))
                {
                    C.SelectedCollectableTypes.Add(type);
                }
                foreach (var cur in C.Currencies.Where(cur => cur.Child == false && cur.Enabled).ToList())
                {
                    if (!C.SelectedCurrencies.Contains(cur.ItemId))
                        C.SelectedCurrencies.Add(cur.ItemId);
                }
                P.configWizard.IsOpen = true;
            }
            P.configWizard.SetVersion(C.Version);
            C.Version = GetVersion();
        }
        public static string ToSemVer(string version)
        {
            // Split the version into parts
            var parts = version.Split('.');

            // SemVer only uses the first three components (Major.Minor.Patch)
            if (parts.Length >= 3)
            {
                return $"{parts[0]}.{parts[1]}.{parts[2]}";
            }

            return "";
        }
        public static bool LowerVersionThan(String version, string version2)
        {
            // Split the versions into major, minor, and patch components
            var v1Parts = version.Split('.');
            var v2Parts = version2.Split('.');

            // Parse components as integers
            int major1 = int.Parse(v1Parts[0]);
            int minor1 = int.Parse(v1Parts[1]);
            int patch1 = int.Parse(v1Parts[2]);

            int major2 = int.Parse(v2Parts[0]);
            int minor2 = int.Parse(v2Parts[1]);
            int patch2 = int.Parse(v2Parts[2]);

            // Compare major versions
            if (major2 < major1) return true;
            if (major2 > major1) return false;

            // Compare minor versions
            if (minor2 < minor1) return true;
            if (minor2 > minor1) return false;

            // Compare patch versions
            return patch2 < patch1;
        }
        public static bool LowerVersionThan(String version)
        {
            return LowerVersionThan(version, C.Version);
        }

        internal static void DrawVersion110Step2()
        {
            ImGui.TextWrapped("Shows you if you can buy collectables with it.");
            ImGui.Checkbox("Show collectables", ref C.ShowCollectables);
            if (C.ShowCollectables)
            {
                ImGui.TextWrapped("You can have a little info in the main window when you are still missing collectables from that currency.");
                ImGui.Checkbox("Show missing collectables in the main window", ref C.ShowMissingCollectables);
                ImGui.TextWrapped("If you don't want to see specific item you can deselect them here and they won't show up.");
                ImGui.TextWrapped("Select which items you see as collectables:");
                foreach (CollectableType type in Enum.GetValues(typeof(CollectableType)))
                {
                    if (type == CollectableType.None) continue; // Skip 'None'
                    string label = CollectableTypeLabels.TryGetValue(type, out var displayName) ? displayName : type.ToString();
                    bool isSelected = C.SelectedCollectableTypes.Contains(type);
                    if (ImGui.Checkbox($"##{type}", ref isSelected))
                    {
                        if (isSelected)
                        {
                            C.SelectedCollectableTypes.Add(type);
                        }
                        else
                        {
                            C.SelectedCollectableTypes.Remove(type);
                        }
                        P.spendingWindow.UpdateData();
                        MainTab.update(true);
                    }
                    ImGui.SameLine();
                    ImGui.Text(label);
                }
            }
        }
    }
}
