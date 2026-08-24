using CurrencySpender.Classes;
using CurrencySpender.Data;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace CurrencySpender.Windows;

internal static class CurrencyListTab
{
    private const double RefreshIntervalSeconds = 10;

    internal class State
    {
        internal bool NotUpdated = true;
        internal Dictionary<uint, int> MissingCollectables = new();
        internal DateTime LastComputed = DateTime.MinValue;
    }

    internal static void Update(State state, bool society, bool force = false)
    {
        if (!Generator.AllFinished)
        {
            state.NotUpdated = true;
            return;
        }
        var stale = (DateTime.Now - state.LastComputed).TotalSeconds >= RefreshIntervalSeconds;
        if (!state.NotUpdated && !force && !stale) return;

        // One grouped pass over all items instead of one full scan per currency.
        var byCurrency = Generator.items
            .Where(item => item.Type.HasFlag(ItemType.Collectable)
                && !item.Disabled
                && C.SelectedCollectableTypes.Contains((CollectableType)item.CollectableType)
                && ShopHelper.IsAttainable(item))
            .ToLookup(item => item.Currency);

        state.MissingCollectables.Clear();
        foreach (var currency in P.Currencies.Where(c => c.Society == society))
        {
            if (!currency.Enabled || currency.Child) continue;

            var items = byCurrency[currency.ItemId];
            if (currency.Children != null)
                items = items.Concat(currency.Children.SelectMany(child => byCurrency[child]));
            var distinct = items.DistinctBy(item => item.Id).ToList();

            var itemsUnlocked = distinct.Count(item => ItemHelper.IsUnlocked(item.Id));
            state.MissingCollectables[currency.ItemId] = distinct.Count - itemsUnlocked;
        }
        state.NotUpdated = false;
        state.LastComputed = DateTime.Now;
    }

    internal static void Draw(State state, bool society, Action? drawPreamble = null)
    {
        Update(state, society);
        if (Service.ObjectTable.LocalPlayer == null)
        {
            UiHelper.WarningText("Please login before using this Plugin!");
            return;
        }
        drawPreamble?.Invoke();

        if (!ImGui.BeginTable("##currencies", C.ShowMissingCollectables ? 4 : 3, ImGuiTableFlags.Borders | ImGuiTableFlags.Sortable))
            return;

        ImGui.TableSetupColumn("Cur.", ImGuiTableColumnFlags.WidthFixed, 38);
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            UiHelper.LeftAlign("Currency");
            ImGui.EndTooltip();
        }
        ImGui.TableSetupColumn("Amount");
        if (C.ShowMissingCollectables)
        {
            ImGui.TableSetupColumn("MC", ImGuiTableColumnFlags.WidthFixed, 30);
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                UiHelper.LeftAlign("Missing Collectables");
                ImGui.EndTooltip();
            }
        }
        ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.NoSort);
        ImGui.TableHeadersRow();

        var currencies = P.Currencies.FindAll(cur => cur.Society == society);

        // Read sort specs every frame (ImGui persists them; SpecsDirty only fires on click)
        ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();
        if (!sortSpecs.IsNull && sortSpecs.SpecsCount > 0)
        {
            ImGuiTableColumnSortSpecsPtr spec = sortSpecs.Specs;
            int columnIndex = spec.ColumnIndex;
            bool ascending = spec.SortDirection == ImGuiSortDirection.Ascending;
            switch (columnIndex)
            {
                case 0:
                    currencies = ascending
                        ? currencies.ToList()
                        : currencies.AsEnumerable().Reverse().ToList();
                    break;
                case 1:
                    currencies = ascending
                        ? currencies.OrderBy(c => c.Percentage).ToList()
                        : currencies.OrderByDescending(c => c.Percentage).ToList();
                    break;
                case 2:
                    currencies = ascending
                        ? currencies.OrderBy(c => state.MissingCollectables.TryGetValue(c.ItemId, out int value) ? value : int.MaxValue).ToList()
                        : currencies.OrderByDescending(c => state.MissingCollectables.TryGetValue(c.ItemId, out int value) ? value : int.MinValue).ToList();
                    break;
                default:
                    break;
            }
        }

        foreach (var currency in currencies)
        {
            if (currency.Child || !C.SelectedCurrencies.Contains(currency.ItemId) ||
                (C.HideEmptyCurrencies && currency.CurrentCount <= 0))
                continue;
            if (currency.ItemId == 26807 && P.Problem) continue;
            if (currency.NeedsPresence && !IsPlayerPresent(currency)) continue;

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Image(currency.Icon.Handle, new Vector2(36, 36));
            if (ImGui.IsItemHovered())
            {
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
                if (state.MissingCollectables.TryGetValue(currency.ItemId, out var value))
                {
                    ImGuiEx.Text(value > 0 ? $"{value}" : "-");
                }
            }
            ImGui.TableNextColumn();
            if (ImGuiEx.Button($"Spend it!##{currency.ItemId}"))
            {
                P.ToggleSpendingUI(currency);
            }
        }
        ImGui.EndTable();
    }

    private static unsafe bool IsPlayerPresent(TrackedCurrency currency)
    {
        var currentTerritory = AgentMap.Instance()->CurrentTerritoryId;
        foreach (var shop in Generator.shops)
        {
            if (shop.Location == null) continue;
            if (shop.Items.Any(item => item.Currency == currency.ItemId) &&
                shop.Location.NeedsPresence &&
                shop.Location.TerritoryId == currentTerritory)
                return true;
        }
        return false;
    }
}
