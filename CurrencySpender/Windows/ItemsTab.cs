using CurrencySpender.Classes;
using CurrencySpender.Data;
using Lumina.Excel.Sheets;

namespace CurrencySpender.Windows;

internal class ItemsTab
{
    private static string SearchQuery = "";
    private static List<ShopItem> FilteredItems = new List<ShopItem>();
    internal static void Draw()
    {   
        ImGui.TextWrapped("Displays table if you can buy items of interest with current currency:");
        ImGui.Checkbox($"Show items of interest", ref C.ShowItemsOfInterest);
        ImGui.Separator();
        ImGui.TextWrapped("Items to add to the list:");
        if (ImGui.InputText("##Search", ref SearchQuery, 100))
        {
            // Filter the list based on the query
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredItems = Generator.items
                    .Where(item => item.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                    .GroupBy(item => item.Name) // Group by Name to remove duplicates
                    .Select(group => group.First()) // Take the first item from each group
                    .Where(item => !C.ItemsOfInterest.Contains(item.Id))
                    .ToList();
            }
            else
            {
                FilteredItems.Clear();
            }
        }
        if (!string.IsNullOrWhiteSpace(SearchQuery) && FilteredItems.Count > 0)
        {
            if (ImGui.BeginListBox("##FilteredDropdown", new Vector2(0, Math.Min(FilteredItems.Count * 20.0f, 100.0f))))
            {
                foreach (var item in FilteredItems)
                {
                    if (ImGui.Selectable(item.Name))
                    {
                        // Add the selected item to the list
                        if (!C.ItemsOfInterest.Contains(item.Id))
                        {
                            C.ItemsOfInterest.Add(item.Id);
                        }

                        // Clear search and close dropdown
                        SearchQuery = "";
                        FilteredItems.Clear();
                        break; // Exit loop after selection
                    }
                }
                ImGui.EndListBox();
            }
        }
        ImGui.Separator();
        ImGui.TextWrapped("Current items of interest:");
        uint? itemToRemove = null; // Store the ID of the item to remove

        if (ImGui.BeginTable("##itemsofinterest", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit))
        {
            ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("Actions");
            ImGui.TableHeadersRow();

            foreach (var item in C.ItemsOfInterest)
            {
                ImGui.TableNextColumn();

                var cur_item = Service.DataManager.GetExcelSheet<Item>().GetRow(item);
                UiHelper.LeftAlign(cur_item.Name.ToString());

                if (ImGui.IsItemHovered() && C.Debug)
                {
                    ImGui.BeginTooltip();
                    UiHelper.LeftAlign($"ID: {item}");
                    ImGui.EndTooltip();
                }

                ImGui.TableNextColumn();
                if (ImGui.Button($"Remove##{item}"))
                {
                    // Open confirmation popup and set the item awaiting confirmation
                    itemToRemove = item;
                }
            }

            ImGui.EndTable();
        }
        // Remove the item outside of the table rendering
        if (itemToRemove.HasValue)
        {
            SearchQuery = "";
            C.ItemsOfInterest.Remove(itemToRemove.Value);
            itemToRemove = null;
        }
    }
}
