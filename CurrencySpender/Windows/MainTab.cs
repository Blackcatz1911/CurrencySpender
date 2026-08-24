using CurrencySpender.Classes;

namespace CurrencySpender.Windows;

internal static class MainTab
{
    internal static readonly CurrencyListTab.State State = new();

    internal static void Draw()
    {
        CurrencyListTab.Draw(State, society: false, drawPreamble: DrawPreamble);
    }

    private static void DrawPreamble()
    {
        if (!P.Problem) return;
        UiHelper.WarningText("The current shared FATE ranks could not be fetched. Please click the button below:");
        if (ImGui.Button("Open shared FATE window"))
        {
            PlayerHelper.openSharedFate();
        }
        ImGui.Separator();
    }

    public static void Update(bool Force = false)
    {
        CurrencyListTab.Update(State, society: false, Force);
    }
}
