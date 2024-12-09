using CurrencySpender.Classes;

namespace CurrencySpender.Windows;

internal class MainTab
{
    public static bool colored = false;
    internal static void Draw()
    {
        if(Service.ClientState.LocalPlayer == null)
        {
            UiHelper.WarningText("Please login before using this Plugin!");
            return;
        }
        if (P.Problem)
        {
            UiHelper.WarningText("The current shared FATE ranks could not be fetched. Please click the button below:");
            if(ImGui.Button("Open shared FATE window"))
            {
                PlayerHelper.openSharedFate();
            }
            ImGui.Separator();
        }
        //var font = UiBuilder.;
        //font.Scale = 1.1f; 
        //ImGuiEx.Text(ImGuiColors.DalamudGrey, font, "Test");
        //ImGui.PopFont();
        foreach (TrackedCurrency currency in C.Currencies)
        {
            if (currency.Enabled && currency.CurrentCount > 0 && !currency.Child)
            {
                if (currency.ItemId == 26807 && P.Problem) continue;
                ImGui.Image(currency.Icon.ImGuiHandle, new Vector2(21, 21));
                ImGui.SameLine();
                var text = $"{currency.CurrentCount}/{currency.MaxCount} - {currency.Percentage}%";
                if (currency.Percentage > 70) ImGuiEx.Text(EColor.RedBright, text);
                else if(currency.Percentage > 50) ImGuiEx.Text(EColor.YellowBright, text);
                else ImGuiEx.Text(text);

                if (ImGuiEx.IconButtonWithText(Dalamud.Interface.FontAwesomeIcon.MagnifyingGlassChart, " "+currency.Name)) {
                    //List<uint> itemIds = new List<uint>();
                    //foreach (BuyableItem item in C.Items)
                    //{
                    //    if (item.C_ID == currency.ItemId) itemIds.Add(item.ItemId);
                    //}

                    //List<ShopItem> collectableItems = Generator.items
                    //    .Where(item => item.Currency == currency.ItemId && item.Type.HasFlag(ItemType.Collectable) && !ItemHelper.CheckUnlockStatus(item.Id))
                    //    .ToList();

                    //foreach (ShopItem item in SellableItems)
                    //{
                    //    item.Profit = item.CurrentPrice * item.AmountCanBuy;
                    //}
                    P.ToggleSpendingUI(currency);
                }
                ImGui.Separator();
            }
        }
    }
}
