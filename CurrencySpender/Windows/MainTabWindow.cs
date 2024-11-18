using Dalamud.Interface.Components;
using ECommons.Configuration;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface.Colors;
using Dalamud.Interface;

namespace CurrencySpender.Windows;

internal class MainTabWindow : Window
{
    public MainTabWindow() : base($"")
    {
        this.SizeConstraints = new()
        {
            MinimumSize = new(250, 100),
            MaximumSize = new(9999, 9999)
        };
        P.ws.AddWindow(this);
        TitleBarButtons.Add(new()
        {
            Click = (m) => { if (m == ImGuiMouseButton.Left) P.configTabWindow.IsOpen = true; },
            Icon = FontAwesomeIcon.Cog,
            IconOffset = new(2, 2),
            ShowTooltip = () => ImGui.SetTooltip("Open settings window"),
        });
    }

    public override void PreDraw()
    {
        WindowName = $"{P.Name} {P.GetType().Assembly.GetName().Version}###MainTabWindow";
    }

    public override void Draw()
    {
        ImGuiEx.EzTabBar("tabbar", [
            ("Main", MainTab.Draw, null, true),
            ("Todo list", TodoTab.Draw, null, true),
            ("About", AboutTab.Draw, null, true),
         ]);

    }
}
