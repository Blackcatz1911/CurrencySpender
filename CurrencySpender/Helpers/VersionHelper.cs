using System.Text;
using CurrencySpender.Classes;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace CurrencySpender.Helpers
{
    internal unsafe class VersionHelper
    {
        public static string? CheckVersion()
        {
            var oldVersion = NormalizeVersion(C.Version);
            var newVersion = GetVersion();

            if (CompareVersions(oldVersion, "1.1.0") < 0)
            {
                PluginLog.Information("Version below 1.1.0 found");
                foreach (CollectableType type in Enum.GetValues(typeof(CollectableType)))
                {
                    C.SelectedCollectableTypes.Add(type);
                }
            }
            MigrateCurrencies(oldVersion);
            MigrateCollectableTypes(oldVersion);

            C.Version = NormalizeVersion(newVersion);

            if (oldVersion == "0.0.0" || CompareVersions(newVersion, oldVersion) <= 0)
            {
                return null;
            }
            return oldVersion;
        }

        private static void MigrateCurrencies(string oldVersion)
        {
            foreach (var currency in P.Currencies.Where(c => !c.Child && CompareVersions(c.AddedInVersion, oldVersion) > 0))
            {
                C.SelectedCurrencies.Add(currency.ItemId);
            }
        }

        private static readonly Dictionary<string, CollectableType[]> CollectableTypeMigrations = new()
        {
            ["1.1.2"] = [CollectableType.Container, CollectableType.Mahjong],
            ["1.2.6"] = [CollectableType.FashionAccessory],
        };

        private static void MigrateCollectableTypes(string oldVersion)
        {
            foreach (var (version, types) in CollectableTypeMigrations)
            {
                if (CompareVersions(version, oldVersion) <= 0) continue;
                foreach (var type in types)
                {
                    C.SelectedCollectableTypes.Add(type);
                }
            }
        }

        public static bool IsNewVersion()
        {
            return CompareVersions(GetVersion(), C.Version) > 0;
        }

        public static string GetVersion()
        {
            string? version = P?.GetType()?.Assembly?.GetName()?.Version?.ToString();
            if (version == null) return "";
            return ToSemVer(version);
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

        public static string NormalizeVersion(string? version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                return "0.0.0";
            }
            var parts = version.Split('.');
            return $"{IntPart(0)}.{IntPart(1)}.{IntPart(2)}";

            int IntPart(int index) => index < parts.Length && int.TryParse(parts[index], out var value) ? value : 0;
        }

        public static int CompareVersions(string? a, string? b)
        {
            return Version.Parse(NormalizeVersion(a)).CompareTo(Version.Parse(NormalizeVersion(b)));
        }

        public static string GameVersion()
        {
            var gameVersionSpan = Framework.Instance()->GameVersion;
            
            var nullIndex = gameVersionSpan.IndexOf((byte)0);
            if (nullIndex != -1)
            {
                gameVersionSpan = gameVersionSpan[..nullIndex];
            }

            return Encoding.UTF8.GetString(gameVersionSpan);
        }
        public static bool IsNewGameVersion()
        {
            PluginLog.Information($"GameVersion: {GameVersion()}");
            if (C.GameVersion != GameVersion() || C.GameVersion == "")
            {
                C.GameVersion = GameVersion();
                return true;
            }
            return false;
        }
    }
}
