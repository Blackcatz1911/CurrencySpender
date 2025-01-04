namespace CurrencySpender.Windows;

unsafe internal class ConfigTabWindow : Window
{
    public ConfigTabWindow() : base($"ConfigTabWindow")
    {
        this.SizeConstraints = new()
        {
            MinimumSize = new(350, 100),
            MaximumSize = new(9999, 9999)
        };
        P.ws.AddWindow(this);
    }

    public override void PreDraw()
    {
        WindowName = $"{P.Name} Settings {P.Version}###ConfigTabWindow";
    }

    public override void Draw()
    {
        ImGuiEx.EzTabBar("tabbar", [
            ("Settings", ConfigTab.Draw, null, true),
            ("Items", ItemsTab.Draw, null, true),
            ("Changelog", ChangelogTab.Draw, null, true),
            ("About", AboutTab.Draw, null, true),
            (C.Debug?"Debug":null, DebugTab.Draw, null, true),
         ]);
    }
}
