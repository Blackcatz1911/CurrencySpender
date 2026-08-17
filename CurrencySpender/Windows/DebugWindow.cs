namespace CurrencySpender.Windows;

internal class DebugWindow : Window
{
    public DebugWindow() : base($"{P.Name} {P.Version} - Debug###{P.Name}DebugWindow")
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
        WindowName = $"{P.Name} {P.Version} - Debug###{P.Name}DebugWindow";
    }

    public override void Draw()
    {
        ImGuiEx.EzTabBar("DebugTabs", [
            ("Main Debug Tab", DebugMainTab.Draw, null, true),
         ]);
    }
}
