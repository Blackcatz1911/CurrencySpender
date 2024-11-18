using ImGuiNET;
using System;
using System.Collections.Generic;

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
}
