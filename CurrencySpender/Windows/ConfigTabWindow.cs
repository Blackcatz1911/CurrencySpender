namespace CurrencySpender.Windows;

unsafe internal class ConfigTabWindow : Window
{
    public ConfigTabWindow() : base($"ConfigTabWindow")
    {
        this.SizeConstraints = new()
        {
            MinimumSize = new(250, 100),
            MaximumSize = new(9999, 9999)
        };
        P.ws.AddWindow(this);
    }

    public override void PreDraw()
    {
        WindowName = $"{P.Name} Settings {P.GetType().Assembly.GetName().Version}###ConfigTabWindow";
    }

    public override void Draw()
    {
        ImGuiEx.EzTabBar("tabbar", [
            ("Main Settings", ConfigTab.Draw, null, true),
            (C.debug?"Debug":null, DebugTab.Draw, null, true),
         ]);
    }
}
