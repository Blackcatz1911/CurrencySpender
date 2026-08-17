using CurrencySpender.Windows.Config;

namespace CurrencySpender.Windows;

internal class ConfigWindow : Window
{
    public ConfigWindow() : base($"{P.Name} Settings {P.Version}###{P.Name}{P.Version}ConfigWindow")
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
        WindowName = $"{P.Name} Settings {P.Version}###{P.Name}{P.Version}ConfigWindow";
    }

    public override void Draw()
    {
        if (!C.ThirdParty)
        {
            if (Lifestream.Enabled || Vnavmesh.Enabled) C.ThirdParty = true;
        }
        ImGuiEx.EzTabBar("ConfigTabs", [
            ("General", GeneralTab.Draw, null, true),
            ("Currencies", CurrenciesTab.Draw, null, true),
            ("Societies", SocietiesConfigTab.Draw, null, true),
            ("Items", ItemsTab.Draw, null, true),
            ("Display", DisplayTab.Draw, null, true),
            (C.ThirdParty?"ThirdParty":null, ThirdPartyTab.Draw, null, true),
            (C.Debug?"Debug":null, DebugTab.Draw, null, true),
            ("Changelog", ChangelogTab.Draw, null, true),
            ("About", AboutTab.Draw, null, true),
         ]);
    }
}
