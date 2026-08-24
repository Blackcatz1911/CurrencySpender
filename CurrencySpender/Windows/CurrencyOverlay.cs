using FFXIVClientStructs.FFXIV.Component.GUI;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface;

namespace CurrencySpender.Windows;

internal unsafe class CurrencyOverlay : Window
{
    private float height;
    private bool mainIsOpen = false;

    public CurrencyOverlay() : base("Currency overlay", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoSavedSettings, true)
    {
        P.ws.AddWindow(this);
        IsOpen = true;
        
        
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "Currency", OnCurrencyOpen);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "Currency", OnCurrencyClose);
    }
    private void OnCurrencyOpen(AddonEvent type, AddonArgs args)
    {
        if (C.OpenAutomatically && !mainIsOpen)
        {
            P.mainTabWindow.IsOpen = true;
            mainIsOpen = true;
            PluginLog.Debug("Auto-opened main window on Currency addon open.");
        }
    }

    private void OnCurrencyClose(AddonEvent type, AddonArgs args)
    {
        mainIsOpen = false;
        PluginLog.Debug("Currency addon closed — reset auto-open flag.");
    }

    public override bool DrawConditions()
    {
        return GenericHelpers.TryGetAddonByName<AtkUnitBase>("Currency", out var addon) &&
               GenericHelpers.IsAddonReady(addon) && C.ShowButton;
    }

    public override void PreDraw()
    {
        //ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
    }

    public override void Draw()
    {
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("Currency", out var addon) && GenericHelpers.IsAddonReady(addon))
        {
            if(addon->X != 0 || addon->Y != 0)
            {
                Position = new(addon->X, addon->Y - height);
            }
            if(ImGuiEx.IconButtonWithText(FontAwesomeIcon.MoneyBills, "Open Currency Spender")) P.mainTabWindow.IsOpen = true;
            if(C.OpenAutomatically && !mainIsOpen) {
                P.mainTabWindow.IsOpen = true;
                mainIsOpen = true;
            }
        }
        else { mainIsOpen = false; }
        height = ImGui.GetWindowSize().Y;
    }

    public override void OnClose()
    {
        mainIsOpen = false;
        base.OnClose();
    }
}
