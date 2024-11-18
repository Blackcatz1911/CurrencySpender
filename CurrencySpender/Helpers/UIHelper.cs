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

    //public static void CreateTable(String name, ImGuiTableFlags flags, List<String> headers)
    //{
    //    //Service.Log.Verbose("Starting ImGui Table '" + name + "' rendering...");

    //    ImGui.BeginTable(name, list.Count, flags);
    //    foreach ( var item in list )
    //    {
    //        //Service.Log.Verbose($"Setting up column: {item}");
    //        ImGui.TableSetupColumn(item?.Replace("\0", "") ?? "Unnamed", ImGuiTableColumnFlags.None);
    //    }

    //    ImGui.TableHeadersRow();

    //    foreach (var item in list)
    //    {
    //        //Service.Log.Verbose("TableSetupColumn '" + item + "' rendering...");
    //        ImGui.TableNextColumn();
    //        ImGui.Text($"-");
    //    }

    //    ImGui.EndTable();
    //    //Service.Log.Verbose("Ending ImGui Table '"+name+"' rendering...");
    //}
}
