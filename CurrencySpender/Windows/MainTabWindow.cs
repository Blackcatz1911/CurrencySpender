using CurrencySpender.Classes;
using Dalamud.Interface;

namespace CurrencySpender.Windows;

internal class MainTabWindow : Window
{
    public MainTabWindow() : base($"")
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
        WindowName = $"{P.Name} {P.Version}###MainTabWindow";
    }

    public override void Draw()
    {
        ImGuiEx.EzTabBar("tabbar", [
            ("Currencies", MainTab.Draw, null, true),
            ("Instructions", InstructionsTab.Draw, null, true),
         ]);

    }
}
