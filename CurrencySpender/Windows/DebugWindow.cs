namespace CurrencySpender.Windows;

internal class DebugWindow : Window
{
    public DebugWindow() : base("DebugTabWindow")
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
        WindowName = $"{P.Name} Debug {P.Version}###DebugTabWindow";
    }

    public override void Draw()
    {
        ImGuiEx.EzTabBar("tabbar", [
            ("Main Debug Tab", DebugMainTab.Draw, null, true),
         ]);
    }
}
