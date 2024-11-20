using CurrencySpender.Classes;

namespace CurrencySpender.Windows;

internal class MainTab
{
    public static Boolean colored = false;
    internal static void Draw()
    {
        if(Service.ClientState.LocalPlayer == null)
        {
            ImGui.TextWrapped("Please login before using this Plugin!");
            return;
        }
        ImGui.TextWrapped("Select the wanted currency below and get to know what to do with it!");
        ImGui.Separator();
        foreach (TrackedCurrency currency in C.Currencies)
        {
            if (currency.Enabled && currency.CurrentCount > 0)
            {
                if (ImGuiEx.Button(currency.Name + ": " + currency.CurrentCount.ToString())) {
                    List<uint> itemIds = new List<uint>();
                    foreach (BuyableItem item in C.Items)
                    {
                        if (item.C_ID == currency.ItemId) itemIds.Add(item.ItemId);
                    }

                    List<BuyableItem> collectableItems = C.Items
                        .Where(item => item.C_ID == currency.ItemId && item.Type == ItemType.Collectable && !ItemHelper.CheckUnlockStatus(item.ItemId))
                        .ToList();

                    List<BuyableItem> filteredItems = C.Items
                        .Where(item => item.C_ID == currency.ItemId && item.Type == ItemType.Sellable)
                        .ToList();

                    foreach (BuyableItem item in filteredItems)
                    {
                        double buyableAmountD = (currency.CurrentCount / item.Price);
                        item.AmountCanBuy = (uint)Math.Floor(buyableAmountD);
                        item.Profit = item.CurrentPrice * item.AmountCanBuy;
                    }
                    P.ToggleSpendingUI(currency.ItemId, currency.Name, collectableItems);
                }
            }
        }
    }
}
