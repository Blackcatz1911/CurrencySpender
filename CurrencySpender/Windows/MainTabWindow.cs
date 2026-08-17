using CurrencySpender.Classes;
using Dalamud.Interface;

namespace CurrencySpender.Windows;

internal class MainTabWindow : Window
{
    internal static Vector2 LastPos;
    internal static Vector2 LastSize;
    public MainTabWindow() : base($"{P.Name} {P.Version}###{P.Name}MainTabWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(250, 100),
            MaximumSize = new(9999, 9999)
        };
        P.ws.AddWindow(this);
        TitleBarButtons.Add(new()
        {
            Click = (m) => { if (m == ImGuiMouseButton.Left) P.ConfigWindow.IsOpen = true; },
            Icon = FontAwesomeIcon.Cog,
            IconOffset = new(2, 2),
            ShowTooltip = () => ImGui.SetTooltip("Open settings window"),
        });
    }
    
    public override bool DrawConditions()
    {
        return UiHelper.DrawConditions();
    }

    public override void PreDraw()
    {
        WindowName = $"{P.Name} {P.Version}###{P.Name}MainTabWindow";
    }

    public override void Draw()
    {
        LastPos = ImGui.GetWindowPos();
        LastSize = ImGui.GetWindowSize();

        var tabs = new List<(string, System.Action, Vector4?, bool)>
        {
            ("Currencies", MainTab.Draw, null, true),
        };

        if (C.ShowSocieties)
            tabs.Add(("Societies", SocietiesTab.Draw, null, true));
        tabs.Add(("Instructions", InstructionsTab.Draw, null, true));

        ImGuiEx.EzTabBar("MainTabs", tabs.ToArray());
    }
}
