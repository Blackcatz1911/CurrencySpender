using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;

namespace CurrencySpender.Helpers;

internal static unsafe class UiHelper
{
    public static void RightAlign(string str, bool formatString = false)
    {
        if (formatString) str = StringHelper.FormatString(str);
        float rowHeight = ImGui.GetFrameHeight(); // Height of the row
        float textHeight = ImGui.GetTextLineHeight(); // Height of the text
        float iconSizef = 20; // Assuming your icon is 20x20
        float maxHeight = Math.Max(textHeight, iconSizef); // Get the largest height (icon or text)
        float offset = (rowHeight - maxHeight) / 1.0f; // Calculate offset

        if (offset > 0)
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + offset); // Adjust vertical position
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

        float rowHeight = ImGui.GetFrameHeight(); // Height of the row
        float textHeight = ImGui.GetTextLineHeight(); // Height of the text
        float iconSizef = 20; // Assuming your icon is 20x20
        float maxHeight = Math.Max(textHeight, iconSizef); // Get the largest height (icon or text)
        float offset = (rowHeight - maxHeight) / 1.0f; // Calculate offset

        if(offset > 0)
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + offset); // Adjust vertical position

        // Render text
        ImGui.Text(text);

        // Render icon next to the text
        ImGui.SameLine();
        ImGui.Image(icon, iconSize);
    }
    public static void LeftAlign(string str, bool formatString = false)
    {
        if (formatString) str = StringHelper.FormatString(str);
        float rowHeight = ImGui.GetFrameHeight(); // Height of the row
        float textHeight = ImGui.GetTextLineHeight(); // Height of the text
        float iconSizef = 20; // Assuming your icon is 20x20
        float maxHeight = Math.Max(textHeight, iconSizef); // Get the largest height (icon or text)
        float offset = (rowHeight - maxHeight) / 1.0f; // Calculate offset

        if (offset > 0)
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + offset); // Adjust vertical position
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
