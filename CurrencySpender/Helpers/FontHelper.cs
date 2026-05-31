using Dalamud.Interface.ManagedFontAtlas;

namespace CurrencySpender.Helpers
{
    internal static class FontHelper
    {
        public static IFontHandle LargeFont { get; private set; }
        public static IFontHandle LargerFont { get; private set; }
        public static IFontHandle LargestFont { get; private set; }

        public static void SetupFonts()
        {
            LargeFont = PluginInterface.UiBuilder.FontAtlas.NewDelegateFontHandle(e =>
            {
                e.OnPreBuild(tk => tk.AddDalamudDefaultFont(24));
            });

            LargerFont = PluginInterface.UiBuilder.FontAtlas.NewDelegateFontHandle(e =>
            {
                e.OnPreBuild(tk => tk.AddDalamudDefaultFont(28));
            });
            LargestFont = PluginInterface.UiBuilder.FontAtlas.NewDelegateFontHandle(e =>
            {
                e.OnPreBuild(tk => tk.AddDalamudDefaultFont(32));
            });
        }
        
        public static void LargestText(string uidText)
        {
            LargestFont.Push();
            ImGui.TextWrapped(uidText);
            LargestFont.Pop();
        }
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
            LargestFont.Dispose();
        }
    }
}
