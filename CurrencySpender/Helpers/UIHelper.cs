using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;

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
    public static void RightAlignWithIcon(string text, nint icon, bool formatString = false)
    {
        if (formatString)
            text = StringHelper.FormatString(text);

        // Get current column width
        float columnWidth = ImGui.GetColumnWidth();

        // Calculate the total width of text + icon + padding
        Vector2 iconSize = new Vector2(20, 20);
        float textWidth = ImGui.CalcTextSize(text).X;
        float totalWidth = textWidth + iconSize.X + 5; // 4px padding between text and icon

        // Calculate starting position for alignment
        float posX = ImGui.GetCursorPosX() + columnWidth - totalWidth - ImGui.GetScrollX();

        // Ensure position is within bounds
        if (posX > ImGui.GetCursorPosX())
            ImGui.SetCursorPosX(posX);

        // Render text
        ImGui.Text(text);

        // Render icon next to the text
        ImGui.SameLine();
        ImGui.Image(icon, iconSize);
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
