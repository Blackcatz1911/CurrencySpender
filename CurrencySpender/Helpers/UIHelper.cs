using Dalamud.Interface;
using Dalamud.Interface.ManagedFontAtlas;

namespace CurrencySpender.Helpers;

internal static unsafe class UiHelper
{
    public static void Rightalign(String str, bool formatString)
    {
        if (formatString) str = StringHelper.FormatString(str);
        var posX = ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - ImGui.CalcTextSize(str).X
            - ImGui.GetScrollX();
        if (posX > ImGui.GetCursorPosX())
            ImGui.SetCursorPosX(posX);
        ImGui.Text(str);
    }
    public static void WarningText(string str)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGuiEx.Text(EColor.YellowBright, FontAwesomeIcon.ExclamationTriangle.ToIconString());
        ImGui.PopFont();
        ImGui.SameLine();
        ImGuiEx.TextWrapped(EColor.YellowBright, str);
    }

}
