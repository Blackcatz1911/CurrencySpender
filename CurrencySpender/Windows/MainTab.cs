using CurrencySpender.Classes;
using CurrencySpender.Data;
using Dalamud.Interface.Utility.Raii;
using System.Data;

namespace CurrencySpender.Windows;

internal class MainTab
{
    public static bool colored = false;
    internal static bool NotUpdated = true;
    internal static Dictionary<uint, int> MissingCollectables = new Dictionary<uint, int>();
    public List<TrackedCurrency> sortedCurrencies = P.Currencies;
    internal static unsafe void Draw()
    {
        update();
        if(Service.ClientState.LocalPlayer == null)
        {
            UiHelper.WarningText("Please login before using this Plugin!");
            return;
        }
        if (P.Problem)
        {
            UiHelper.WarningText("The current shared FATE ranks could not be fetched. Please click the button below:");
            if (ImGui.Button("Open shared FATE window"))
            {
                //PlayerHelper.reset();
                PlayerHelper.openSharedFate();
            }
            ImGui.Separator();
        }
        //var font = UiBuilder.;
        //font.Scale = 1.1f; 
        //ImGuiEx.Text(ImGuiColors.DalamudGrey, font, "Test");
        //ImGui.PopFont();
        //foreach (TrackedCurrency currency in P.Currencies)
        //{
        //    if (currency.Enabled && !currency.Child && C.SelectedCurrencies.Contains(currency.ItemId) &&
        //        (!C.HideEmptyCurrencies || currency.CurrentCount > 0))
        //    {
        //        if (currency.ItemId == 26807 && P.Problem) continue;
        //        ImGui.Image(currency.Icon.ImGuiHandle, new Vector2(21, 21));
        //        ImGui.SameLine();
        //        var text = $"{StringHelper.FormatString(currency.CurrentCount.ToString())}/{StringHelper.FormatString(currency.MaxCount.ToString())} ~{currency.Percentage}% full";
        //        if (currency.Percentage > 70) ImGuiEx.Text(EColor.RedBright, text);
        //        else if(currency.Percentage > 50) ImGuiEx.Text(EColor.YellowBright, text);
        //        else ImGuiEx.Text(text);
        //        if (C.ShowMissingCollectables && MissingCollectables.TryGetValue(currency.ItemId, out int value) && value > 0)
        //        {
        //            ImGuiEx.Text($"Collectables missing: {value}");
        //        }
        //        if (ImGuiEx.IconButtonWithText(Dalamud.Interface.FontAwesomeIcon.MagnifyingGlassChart, " "+currency.Name)) {
        //            P.ToggleSpendingUI(currency);
        //        }
        //        ImGui.Separator();
        //    }
        //    else
        //    {
        //        //if(C.Debug) ImGui.Text($"DEBUG: {currency.Name}");
        //    }
        //}
        if (ImGui.BeginTable("##currencies", C.ShowMissingCollectables?4:3, ImGuiTableFlags.Borders | ImGuiTableFlags.Sortable))
        {
            //ImGui.TableSetupColumn("ID");
            ImGui.TableSetupColumn("Cur.", ImGuiTableColumnFlags.WidthFixed, 38);
            if (ImGui.IsItemHovered())
            {
                // Display a tooltip or additional info
                ImGui.BeginTooltip();
                UiHelper.LeftAlign($"Currency");
                ImGui.EndTooltip();
            }
            ImGui.TableSetupColumn("Amount");
            if(C.ShowMissingCollectables) ImGui.TableSetupColumn("MC", ImGuiTableColumnFlags.WidthFixed, 30);
            if (ImGui.IsItemHovered())
            {
                // Display a tooltip or additional info
                ImGui.BeginTooltip();
                UiHelper.LeftAlign($"Missing Collectables");
                ImGui.EndTooltip();
            }
            ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.NoSort);
            ImGui.TableHeadersRow();

            var sortedCurrencies = P.Currencies;
            ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();
            //List<ShopItem> SellableItems = ShopHelper.GetSellableItems(Currency);

            if (!sortSpecs.IsNull && sortSpecs.SpecsCount > 0)
            {
                // Retrieve sorting specification
                ImGuiTableColumnSortSpecsPtr spec = sortSpecs.Specs;
                int columnIndex = spec.ColumnIndex;
                bool ascending = spec.SortDirection == ImGuiSortDirection.Ascending;

                // Sort based on the column index
                switch (columnIndex)
                {
                    case 0:
                        sortedCurrencies = ascending
                            ? P.Currencies.ToList()
                            : P.Currencies.ToList().AsEnumerable().Reverse().ToList();
                        break;

                    case 1:
                        sortedCurrencies = ascending
                            ? sortedCurrencies.OrderBy(c => c.Percentage).ToList()
                            : sortedCurrencies.OrderByDescending(c => c.Percentage).ToList();
                        break;

                    case 2: // Sort by MissingCollectables value
                        sortedCurrencies = ascending
                            ? sortedCurrencies.OrderBy(c =>
                                MissingCollectables.TryGetValue(c.ItemId, out int value) ? value : int.MaxValue).ToList()
                            : sortedCurrencies.OrderByDescending(c =>
                                MissingCollectables.TryGetValue(c.ItemId, out int value) ? value : int.MinValue).ToList();
                        break;
                    default:
                        sortedCurrencies = P.Currencies; // Use original list order
                        break; // Do nothing for unhandled columns
                }
            }
            foreach (TrackedCurrency currency in sortedCurrencies)
            {
                if (!currency.Child && C.SelectedCurrencies.Contains(currency.ItemId) &&
                    (!C.HideEmptyCurrencies || currency.CurrentCount > 0))
                {
                    if (currency.ItemId == 26807 && P.Problem) continue;
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Image(currency.Icon.Handle, new Vector2(36, 36));
                    if (ImGui.IsItemHovered())
                    {
                        // Display a tooltip or additional info
                        ImGui.BeginTooltip();
                        UiHelper.LeftAlign($"{currency.Name}");
                        ImGui.EndTooltip();
                    }
                    ImGui.TableNextColumn();
                    var text = $"{StringHelper.FormatString(currency.CurrentCount.ToString())}/{StringHelper.FormatString(currency.MaxCount.ToString())}\n~{currency.Percentage}% full";
                    if (currency.Percentage > 70) ImGuiEx.Text(EColor.RedBright, text);
                    else if (currency.Percentage > 50) ImGuiEx.Text(EColor.YellowBright, text);
                    else ImGuiEx.Text(text);
                    if (C.ShowMissingCollectables)
                    {
                        ImGui.TableNextColumn();
                        if (MissingCollectables.TryGetValue(currency.ItemId, out int value))
                        {
                            if(value > 0) ImGuiEx.Text($"{value}");
                            else ImGuiEx.Text("-");
                        }
                    }
                    ImGui.TableNextColumn();
                    if (ImGuiEx.Button($"Spend it!##{currency.ItemId}"))
                    {
                        P.ToggleSpendingUI(currency);
                    }
                }
            }
            //}
            //PluginLog.Verbose("Starting ImGui EndTable rendering...");
            ImGui.EndTable();
        }
    }
    public static void update(bool Force = false)
    {
        if (NotUpdated || Force)
        {
            MissingCollectables.Clear();
            foreach (TrackedCurrency currency in P.Currencies)
            {
                if (currency.Enabled && !currency.Child)
                {
                    var items_max = Generator.items
                        .Where(item => (item.Currency == currency.ItemId || (currency.Children != null && currency.Children.Contains(item.Currency))) && item.Type.HasFlag(ItemType.Collectable) && !item.Disabled && C.SelectedCollectableTypes.Contains((CollectableType)item.CollectableType))
                        .GroupBy(item => item.Id) // Group by unique item.ItemId
                        .Select(group => group.First()) // Take the first item from each group
                        .ToList().Count();
                    var items_unlocked = Generator.items
                        .Where(item => (item.Currency == currency.ItemId || (currency.Children != null && currency.Children.Contains(item.Currency))) && item.Type.HasFlag(ItemType.Collectable) && !item.Disabled && C.SelectedCollectableTypes.Contains((CollectableType)item.CollectableType) && ItemHelper.IsUnlocked(item.Id))
                        .GroupBy(item => item.Id) // Group by unique item.ItemId
                        .Select(group => group.First()) // Take the first item from each group
                        .ToList().Count();
                    MissingCollectables.Add(currency.ItemId, (items_max - items_unlocked));
                }
            }
            NotUpdated = false;
        }
    }
}
