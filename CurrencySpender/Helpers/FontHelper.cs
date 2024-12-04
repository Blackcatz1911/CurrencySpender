using Dalamud.Interface.ManagedFontAtlas;

namespace CurrencySpender.Helpers
{
    internal static class FontHelper
    {
        public static IFontHandle LargeFont { get; private set; }
        public static IFontHandle LargerFont { get; private set; }

        public static void SetupFonts()
        {
            LargeFont = PluginInterface.UiBuilder.FontAtlas.NewDelegateFontHandle(e =>
            {
                e.OnPreBuild(tk => tk.AddDalamudAssetFont(Dalamud.DalamudAsset.NotoSansJpMedium, new()
                {
                    SizePx = 26 // Small font size
                }));
            });

            LargerFont = PluginInterface.UiBuilder.FontAtlas.NewDelegateFontHandle(e =>
            {
                e.OnPreBuild(tk => tk.AddDalamudAssetFont(Dalamud.DalamudAsset.NotoSansJpMedium, new()
                {
                    SizePx = 32 // Large font size
                }));
            });
        }

        // Example usage during ImGui rendering
        public static void LargerText(string uidText)
        {
            LargerFont.Push();
            ImGui.TextWrapped(uidText);
            LargerFont.Pop();
        }
        public static void LargeText(string uidText)
        {
            LargeFont.Push();
            ImGui.TextWrapped(uidText);
            LargeFont.Pop();
        }
        public static void DisposeFonts()
        {
            LargeFont.Dispose();
            LargerFont.Dispose();
        }
    }
}
