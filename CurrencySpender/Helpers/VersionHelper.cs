using System.Text;
using CurrencySpender.Classes;
using CurrencySpender.Data;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace CurrencySpender.Helpers
{
    internal unsafe class VersionHelper
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
            //C.Version = "1.1.1";
            if (LowerVersionThan("1.1.0"))
            {
                PluginLog.Information("Version below 1.1.0 found");
                foreach (CollectableType type in Enum.GetValues(typeof(CollectableType)))
                {
                    C.SelectedCollectableTypes.Add(type);
                }
                foreach (var cur in P.Currencies.Where(cur => cur.Child == false).ToList())
                {
                    if (!C.SelectedCurrencies.Contains(cur.ItemId))
                        C.SelectedCurrencies.Add(cur.ItemId);
                }
            }
            if (LowerVersionThan("1.1.2"))
            {
                C.SelectedCollectableTypes.Add(CollectableType.Container);
                C.SelectedCollectableTypes.Add(CollectableType.Mahjong);
                C.SelectedCurrencies.Add(37549);
                C.SelectedCurrencies.Add(37550);
            }
            if (C.Version == "0.0.0") return;
            if (LowerVersionThan(GetVersion()))
            {
                P.configWizard.IsOpen = true;
            }
            P.configWizard.SetVersion(C.Version);
            C.Version = GetVersion();
        }
        public static void OpenConfigWizard()
        {
            P.configWizard.SetVersion(LastVersion());
            P.configWizard.IsOpen = true;
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
        public static string LastVersion()
        {
            // Split the versions into major, minor, and patch components
            var v1Parts = C.Version.Split('.');

            // Parse components as integers
            int major1 = int.Parse(v1Parts[0]);
            int minor1 = int.Parse(v1Parts[1]);
            int patch1 = int.Parse(v1Parts[2]);
            patch1--;
            if(patch1 < 0)
            {
                patch1 = 0;
                minor1--;
            }
            if (minor1 < 0)
            {
                minor1 = 0;
                major1--;
            }
            if (major1 < 0)
            {
                major1 = 0;
            }
            return major1 + "." + minor1 + "." + patch1;
        }
        public static bool LowerVersionThan(String version)
        {
            return LowerVersionThan(version, C.Version);
        }
        public static string GameVersion()
        {
            return Encoding.UTF8.GetString(Framework.Instance()->GameVersion);
        }
        public static void CheckGameVersion()
        {
            Generator.init();
            PluginLog.Information($"GameVersion: {GameVersion()}");
            if (C.GameVersion != GameVersion() || C.GameVersion == "" || C.Debug)
            {
                Generator.init();
                C.GameVersion = GameVersion();
            }
        }
    }
}
