using CurrencySpender.Classes;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;

namespace CurrencySpender.Windows;
internal class SpendingWindow : Window
{
    public uint CurrencyId;
    public String CurrencyName;
    public List<BuyableItem> collectableItems;
    public SpendingWindow() : base("SpendingWindow")
    {
        this.SizeConstraints = new()
        {
            MinimumSize = new(600, 200),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };
        if (C.debug)
        {
            TitleBarButtons.Add(new()
            {
                Click = (m) =>
                { if (m == ImGuiMouseButton.Left) {
                        P.TaskManager.Enqueue(() => WebHelper.CheckPrices(true));
                        P.TaskManager.Enqueue(() => WebHelper.CheckPrices(true));
                    }
                },
                Icon = FontAwesomeIcon.Sync,
                IconOffset = new(2, 2),
                ShowTooltip = () => ImGui.SetTooltip("Force refresh Universalis"),
            });
        }
    }
    public unsafe override void Draw()
    {
        //WindowName = "SpendingGuide: " + this.CurrencyName;
        ImGui.Text($"'" + CurrencyName + "'... What to do with that:");
        if(C.debug) ImGui.Text($"DEBUG: CurrencyId: {CurrencyId}");
        ImGui.Separator();
        try
        {
            if (collectableItems.Count > 0)
            {
                ImGui.Text($"Collectables not yet registered:");
                if (ImGui.BeginTable("##collectables", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit))
                {
                    ImGui.TableSetupColumn("ID");
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("Price");
                    ImGui.TableSetupColumn("");
                    ImGui.TableHeadersRow();
                    foreach (BuyableItem item in collectableItems)
                    {
                        ImGui.TableNextColumn();
                        ImGui.Text(item.ItemId.ToString());
                        ImGui.TableNextColumn();
                        //Service.Log.Verbose("Starting ImGui item.Name rendering...");
                        ImGui.Text(item.Name);
                        ImGui.TableNextColumn();
                        //Service.Log.Verbose("Starting ImGui item.Price rendering...");
                        UiHelper.Rightalign(item.Price.ToString(), true);
                        ImGui.TableNextColumn();
                        //Service.Log.Verbose("Starting ImGui Flag Marker rendering...");
                        if (ImGui.Button("Flag Marker##collectable" + item.ItemId))
                        {
                            Location loc = Location.retrieve(item.Loc);
                            Service.GameGui.OpenMapWithMapLink(new MapLinkPayload(loc.TerritoryId, loc.MapId, loc.Postion.Item1, loc.Postion.Item2));
                        }
                        //Service.Log.Verbose("Ending ImGui Flag Marker rendering...");
                    }
                    //}
                    //Service.Log.Verbose("Starting ImGui EndTable rendering...");
                    ImGui.EndTable();
                }
                ImGui.Separator();
            }

            ImGui.Text($"Sellable items on the marketboard:");

            if (ImGui.BeginTable("##markettable", 7, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Sortable))
            {
                // Set up columns
                ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("Sales", ImGuiTableColumnFlags.None);
                ImGui.TableSetupColumn("Price", ImGuiTableColumnFlags.None);
                ImGui.TableSetupColumn("Qty", ImGuiTableColumnFlags.None);
                ImGui.TableSetupColumn("Sells for", ImGuiTableColumnFlags.None);
                ImGui.TableSetupColumn("Total", ImGuiTableColumnFlags.None);
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.NoSort);
                ImGui.TableHeadersRow();

                // Get sorting specs
                ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();
                List<BuyableItem> filteredItems = C.Items
                    .Where(item => item.C_ID == CurrencyId && item.Type == ItemType.Sellable)
                    .ToList();

                if (sortSpecs.NativePtr != null && sortSpecs.SpecsCount > 0 && filteredItems.Count > 0)
                {
                    // Retrieve sorting specification
                    ImGuiTableColumnSortSpecsPtr spec = sortSpecs.Specs;
                    int columnIndex = spec.ColumnIndex;
                    bool ascending = spec.SortDirection == ImGuiSortDirection.Ascending;

                    // Sort based on the column index
                    switch (columnIndex)
                    {
                        case 0: // Name
                            filteredItems = ascending
                                ? filteredItems.OrderBy(item => item.Name).ToList()
                                : filteredItems.OrderByDescending(item => item.Name).ToList();
                            break;
                        
                        case 1: // Sales
                            filteredItems = ascending
                                ? filteredItems.OrderBy(item => item.HasSoldWeek).ToList()
                                : filteredItems.OrderByDescending(item => item.HasSoldWeek).ToList();
                            break;

                        case 2: // Price
                            filteredItems = ascending
                                ? filteredItems.OrderBy(item => item.Price).ToList()
                                : filteredItems.OrderByDescending(item => item.Price).ToList();
                            break;

                        case 3: // Qty
                            filteredItems = ascending
                                ? filteredItems.OrderBy(item => item.AmountCanBuy).ToList()
                                : filteredItems.OrderByDescending(item => item.AmountCanBuy).ToList();
                            break;

                        case 4: // Sells for
                            filteredItems = ascending
                                ? filteredItems.OrderBy(item => item.CurrentPrice).ToList()
                                : filteredItems.OrderByDescending(item => item.CurrentPrice).ToList();
                            break;

                        case 5: // Total
                            filteredItems = ascending
                                ? filteredItems.OrderBy(item => item.Profit).ToList()
                                : filteredItems.OrderByDescending(item => item.Profit).ToList();
                            break;

                        default:
                            break; // Do nothing for unhandled columns
                    }
                }

                // Render the table rows
                foreach (BuyableItem item in filteredItems)
                {
                    ImGui.TableNextColumn();
                    if (ImGui.Selectable(item.Name)) // Make the name clickable
                    {
                        ImGui.SetClipboardText(item.Name); // Copy the name to the clipboard
                        Notify.Success("Name copied to clipboard");
                        Service.Log.Verbose($"Copied '{item.Name}' to clipboard.");
                    }
                    if (ImGui.IsItemHovered() && C.debug)
                    {
                        // Display a tooltip or additional info
                        ImGui.BeginTooltip();
                        ImGui.Text($"Additional Info:\nID: {item.ItemId}\nName: {item.Name}");
                        ImGui.EndTooltip();
                    }
                    ImGui.TableNextColumn();
                    UiHelper.Rightalign(item.HasSoldWeek.ToString(), true);
                    ImGui.TableNextColumn();
                    UiHelper.Rightalign(item.Price.ToString(), true);
                    ImGui.TableNextColumn();
                    UiHelper.Rightalign(item.AmountCanBuy.ToString(), true);
                    ImGui.TableNextColumn();
                    UiHelper.Rightalign(item.CurrentPrice == 0 ? "-" : item.CurrentPrice.ToString(), true);
                    ImGui.TableNextColumn();
                    UiHelper.Rightalign(item.Profit == 0 ? "-" : item.Profit.ToString(), true);
                    ImGui.TableNextColumn();
                    if (ImGui.Button($"Flag Marker##sellable{item.ItemId}"))
                    {
                        Location loc = Location.retrieve(item.Loc);
                        if (!Service.GameGui.OpenMapWithMapLink(new MapLinkPayload(loc.TerritoryId, loc.MapId, loc.Postion.Item1, loc.Postion.Item2)))
                        {
                            Service.Log.Verbose("Failed to open map.");
                        }
                        else
                        {
                            Service.Log.Verbose("Map opened successfully.");
                        }
                    }
                }

                ImGui.EndTable();
            }
        }
        catch (Exception e)
        {
            Service.Log.Error(e, "ImGuiTable");
        }
    }
}
