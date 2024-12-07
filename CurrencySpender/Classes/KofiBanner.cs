using Dalamud.Interface.Utility;

namespace CurrencySpender.Classes;
public static class KofiBanner
{
    public static string Text = "♥ Ko-fi";
    public static string DonateLink => "https://ko-fi.com/catz1911";
    public static void DrawRaw()
    {
        DrawButton();
    }

    private static uint ColorNormal
    {
        get
        {
            var vector1 = ImGuiEx.Vector4FromRGB(0x232526);
            var vector2 = ImGuiEx.Vector4FromRGB(0x414345);

            var gen = GradientColor.Get(vector1, vector2).ToUint();
            return gen;
        }
    }

    private static uint ColorHovered => ColorNormal;

    private static uint ColorActive => ColorNormal;

    private static readonly uint ColorText = 0xFFFFFFFF;

    public static void DrawButton()
    {
        ImGui.PushStyleColor(ImGuiCol.Button, ImGuiEx.Vector4FromRGB(0x232526));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ColorHovered);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, ColorActive);
        ImGui.PushStyleColor(ImGuiCol.Text, ColorText);
        if (ImGui.Button(Text))
        {
            GenericHelpers.ShellStart(DonateLink);
        }
        Popup();
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
        ImGui.PopStyleColor(4);
    }

    public static void RightTransparentTab(string? text = null)
    {
        text ??= Text;
        var textWidth = ImGui.CalcTextSize(text).X;
        var spaceWidth = ImGui.CalcTextSize(" ").X;
        ImGui.BeginDisabled();
        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0f);
        if (ImGuiEx.BeginTabItem(" ".Repeat((int)MathF.Ceiling(textWidth / spaceWidth)), ImGuiTabItemFlags.Trailing))
        {
            ImGui.EndTabItem();
        }
        ImGui.PopStyleVar();
        ImGui.EndDisabled();
    }

    public static void DrawRight()
    {
        var cur = ImGui.GetCursorPos();
        ImGui.SetCursorPosX(cur.X + ImGui.GetContentRegionAvail().X - ImGuiHelpers.GetButtonSize(Text).X);
        DrawRaw();
        ImGui.SetCursorPos(cur);
    }

    private static string SmallPatreonButtonTooltip => $"""
				If you like {Service.PluginInterface.Manifest.Name},
				please consider supporting its developer via Ko-fi.
				""";

    private static void Popup()
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
            ImGuiEx.Text(SmallPatreonButtonTooltip);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
    }
}


