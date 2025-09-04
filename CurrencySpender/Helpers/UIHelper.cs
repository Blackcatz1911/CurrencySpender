using CurrencySpender.Classes;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiNotification;
using Lumina.Excel.Sheets;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

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
    public static void RightAlignWithIcon(string text, ImTextureID icon, bool formatString = false)
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

    internal static void BuildMapButtons(ShopItem item)
    {
        if (item.Shop.Location != null && item.Shop.Location != Location.locations[0])
        {
            Location Baldin = Location.locations[0];
            if(item.Shop.Location.TerritoryId == 1055)
                Baldin = Location.locations.Where(loc => loc.NpcId == 1043621).First();
            if (ImGui.Button($"Flag##sellable-{item.Id}-{item.ShopId}-{item.Shop.NpcId}"))
            {
                if(AgentMap.Instance()->CurrentTerritoryId == 1055 && item.Shop.Location.TerritoryId == 1055)
                {
                    Service.GameGui.OpenMapWithMapLink(Baldin.GetMapMarker());
                }
                else
                {
                    Service.GameGui.OpenMapWithMapLink(item.Shop.Location.GetMapMarker());
                }
            }
            if (ImGui.IsItemHovered())
            {
                // Display a tooltip or additional info
                ImGui.BeginTooltip();
                if (AgentMap.Instance()->CurrentTerritoryId != 1055 && item.Shop.Location.TerritoryId == 1055)
                {
                    UiHelper.LeftAlign($"The flag will only show up, if you are on your island.");
                    UiHelper.LeftAlign($"{Baldin.Zone}");
                }
                else
                    UiHelper.LeftAlign($"{item.Shop.Location.Zone}");
                ImGui.EndTooltip();
            }
            ImGui.SameLine();
            if (ImGui.Button($"TP##sellable-{item.Id}-{item.ShopId}-{item.Shop.NpcId}"))
            {
                item.Shop.Location.Teleport();
                if (item.Shop.Location.TerritoryId == 1055)
                    Service.GameGui.OpenMapWithMapLink(Baldin.GetMapMarker());
                else
                    Service.GameGui.OpenMapWithMapLink(item.Shop.Location.GetMapMarker());
            }
            if (ImGui.IsItemHovered())
            {
                // Display a tooltip or additional info
                ImGui.BeginTooltip();
                if(item.Shop.Location.TerritoryId == 1055)
                    UiHelper.LeftAlign($"{Baldin.Zone}");
                else
                    UiHelper.LeftAlign($"{item.Shop.Location.Zone}");
                ImGui.EndTooltip();
            }
        } else if(C.Debug)
        {
            //DuoLog.Error("Missing location!");
        }
    }
    public static void Notification(string content, NotificationType type = NotificationType.Info, bool minimized = true)
    {
        Service.Notification.AddNotification(new Notification { Content = content, Type = type, Minimized = minimized });
    }
    public static void LinkItem(uint id)
    {
        Item item = Service.DataManager.GetExcelSheet<Item>().GetRow(id);
        var payloadList = new List<Payload> {
                new UIForegroundPayload((ushort) (0x223 + item.Rarity * 2)),
                new UIGlowPayload((ushort) (0x224 + item.Rarity * 2)),
                new ItemPayload(item.RowId, item.CanBeHq),
                new UIForegroundPayload(500),
                new UIGlowPayload(501),
                new TextPayload($"{(char) SeIconChar.LinkMarker}"),
                new UIForegroundPayload(0),
                new UIGlowPayload(0),
                new TextPayload(item.Name.ExtractText()),
                new RawPayload(new byte[] {0x02, 0x27, 0x07, 0xCF, 0x01, 0x01, 0x01, 0xFF, 0x01, 0x03}),
                new RawPayload(new byte[] {0x02, 0x13, 0x02, 0xEC, 0x03})
            };

        var payload = new SeString(payloadList);

        Service.Chat.Print(new XivChatEntry
        {
            Message = payload
        });
    }
}
