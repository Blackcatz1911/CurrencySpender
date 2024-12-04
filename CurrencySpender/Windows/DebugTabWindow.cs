namespace CurrencySpender.Windows;

unsafe internal class DebugTabWindow : Window
{
    public DebugTabWindow() : base("DebugTabWindow")
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
        WindowName = $"{P.Name} Debug {P.Version}###DebugTabWindow";
    }

    public override void Draw()
    {
        ImGuiEx.EzTabBar("tabbar", [
            ("Main Debug Tab", DebugMainTab.Draw, null, true),
            //("Todo list", TodoTab.Draw, null, true),
            //("Changelog", ChangelogTab.Draw, null, true),
            //(C.Debug?"Debug":null, DebugTab.Draw, null, true),
         ]);
    }
}
