using CurrencySpender.Windows.Config;

namespace CurrencySpender.Windows;

internal class ConfigWindow : Window
{
    public ConfigWindow() : base($"ConfigTabWindow")
    {
        this.SizeConstraints = new()
        {
            MinimumSize = new(350, 100),
            MaximumSize = new(9999, 9999)
        };
        P.ws.AddWindow(this);
    }
    
    public override bool DrawConditions()
    {
        return UiHelper.DrawConditions();
    }

    public override void PreDraw()
    {
        WindowName = $"{P.Name} Settings {P.Version}###ConfigWindow";
    }

    public override void Draw()
    {
        ImGuiEx.EzTabBar("tabbar", [
            ("General", GeneralTab.Draw, null, true),
            ("Currencies", CurrenciesTab.Draw, null, true),
            ("Items", ItemsTab.Draw, null, true),
            ("Display", DisplayTab.Draw, null, true),
            ("Changelog", ChangelogTab.Draw, null, true),
            ("About", AboutTab.Draw, null, true),
            (C.Debug?"Debug":null, DebugTab.Draw, null, true),
         ]);
    }
}
