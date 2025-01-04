using CurrencySpender.Classes;
using CurrencySpender.Data;
using Lumina.Excel.Sheets;

namespace CurrencySpender.Windows;

internal class ItemsTab
{
    private static string searchQuery = "";
    private static List<string> itemList = new List<string> { "Potion", "Hi-Potion", "Elixir", "Mega Elixir", "Phoenix Down" };
    private static List<ShopItem> filteredItems;
    private static List<uint> selectedItems = new List<uint>();
    private static int selectedIndex = -1;
    internal static void Draw()
    {   
        ImGui.TextWrapped("Displays table if you can buy items of interest with current currency:");
        ImGui.Checkbox($"Show items of interest", ref C.ShowItemsOfInterest);
        ImGui.Separator();
        ImGui.TextWrapped("Items to add to the list:");
        if (ImGui.InputText("##Search", ref searchQuery, 100))
        {
            // Filter the list based on the query
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                filteredItems = Generator.items
                    .Where(item => item.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                    .GroupBy(item => item.Name) // Group by Name to remove duplicates
                    .Select(group => group.First()) // Take the first item from each group
                    .Where(item => !C.ItemsOfInterest.Contains(item.Id))
                    .ToList();
            }
            else
            {
                filteredItems.Clear();
            }
        }
        if (!string.IsNullOrWhiteSpace(searchQuery) && filteredItems.Count > 0)
        {
            if (ImGui.BeginListBox("##FilteredDropdown", new Vector2(0, Math.Min(filteredItems.Count * 20.0f, 100.0f))))
            {
                foreach (var item in filteredItems)
                {
                    if (ImGui.Selectable(item.Name))
                    {
                        // Add the selected item to the list
                        if (!C.ItemsOfInterest.Contains(item.Id))
                        {
                            C.ItemsOfInterest.Add(item.Id);
                        }

                        // Clear search and close dropdown
                        searchQuery = "";
                        filteredItems.Clear();
                        break; // Exit loop after selection
                    }
                }
                ImGui.EndListBox();
            }
        }
        ImGui.Separator();
        ImGui.TextWrapped("Current items of interest:");
        uint? itemToRemove = null; // Store the ID of the item to remove
        uint? confirmRemoveItem = null; // Store the ID of the item awaiting confirmation

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
            searchQuery = "";
            C.ItemsOfInterest.Remove(itemToRemove.Value);
            itemToRemove = null;
        }
    }
}
