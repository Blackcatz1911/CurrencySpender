using CurrencySpender.Classes;
using Dalamud.Interface;

namespace CurrencySpender.Windows;
internal class SpendingWindow : Window
{
    public TrackedCurrency? Currency;
    public List<ShopItem>? CollectableItems;
    public List<ShopItem>? Ventures;
    public bool VentureBuyable;
    public SpendingWindow() : base("SpendingWindow")
    {
        this.SizeConstraints = new()
        {
            MinimumSize = new(600, 200),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };
        if (C.Debug)
        {
            TitleBarButtons.Add(new()
            {
                Click = (m) =>
                { if (m == ImGuiMouseButton.Left && Currency != null) {
                        P.TaskManager.Enqueue(() => WebHelper.CheckAll(Currency.ItemId, true));
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
        if(Currency == null || CollectableItems == null) return;
        //WindowName = "SpendingGuide: " + this.CurrencyName;
        ImGui.Image(Currency.Icon.ImGuiHandle, new Vector2(21, 21));
        ImGui.SameLine();
        ImGui.Text($"{Currency.Name}? And {Currency.CurrentCount} of them? What to do with that:");
        if (C.Debug)
        {
            ImGui.Text($"DEBUG: CurrencyId: {Currency.ItemId}");
        }
        List<uint> ids = [20, 21, 22];
        if (ids.Contains(Currency.ItemId)) {
            if (PlayerHelper.GCRanks[Currency.ItemId - 19] < 10)
            {
                UiHelper.WarningText("Some items cannot be purchased yet due to GC rankings... So they will not be displayed here.");
            }
        }
        if (Currency.ItemId == 26807)
        {
            UiHelper.WarningText("Some items cannot be purchased yet due to GC rankings... So they will not be displayed here.");
        }

        ImGui.Separator();
        try
        {
            if (CollectableItems.Count > 0)
            {
                ImGui.Text($"Collectables not yet registered:");
                if (ImGui.BeginTable("##collectables", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit))
                {
                    //ImGui.TableSetupColumn("ID");
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("Price");
                    ImGui.TableSetupColumn("Zone");
                    ImGui.TableSetupColumn("Actions");
                    ImGui.TableHeadersRow();
                    foreach (var item in CollectableItems)
                    {
                        //ImGui.TableNextColumn();
                        //ImGui.Text(item.Id.ToString());
                        ImGui.TableNextColumn();
                        //PluginLog.Verbose("Starting ImGui item.Name rendering...");
                        ImGui.Text(item.Name);
                        if (ImGui.IsItemHovered() && C.Debug)
                        {
                            // Display a tooltip or additional info
                            ImGui.BeginTooltip();
                            ImGui.Text($"ID: {item.Id}\nCat: {item.Category}\nShopId: {item.Shop.ShopId}\nNPCName: {item.Shop.NpcName}\nNPCID: {item.Shop.NpcId}");
                            ImGui.EndTooltip();
                        }
                        ImGui.TableNextColumn();
                        //PluginLog.Verbose("Starting ImGui item.Price rendering...");
                        UiHelper.Rightalign(item.Price.ToString(), true);
                        //ImGui.Text(item.Price.ToString());
                        ImGui.TableNextColumn();
                        ImGui.Text(item.Shop.Location!=null?item.Shop.Location.Zone:"");
                        ImGui.TableNextColumn();
                        //PluginLog.Verbose("Starting ImGui Flag rendering...");
                        if (item.Shop.Location != null && item.Shop.Location != Location.locations[0])
                        {
                            if (ImGui.Button("Flag##collectable" + item.Id+"-"+item.ShopId))
                            {
                                Service.GameGui.OpenMapWithMapLink(item.Shop.Location.GetMapMarker());
                            }
                            ImGui.SameLine();
                            if (ImGui.Button("TP##collectable" + item.Id + "-" + item.ShopId))
                            {
                                item.Shop.Location.Teleport();
                                Service.GameGui.OpenMapWithMapLink(item.Shop.Location.GetMapMarker());
                            }
                        }
                        //PluginLog.Verbose("Ending ImGui Flag rendering...");
                    }
                    //}
                    //PluginLog.Verbose("Starting ImGui EndTable rendering...");
                    ImGui.EndTable();
                }
                ImGui.Separator();
            }

            if (VentureBuyable && C.ShowVentures && Ventures != null)
            {
                ImGui.Text($"Can buy Ventures:");
                if (ImGui.BeginTable("##collectables", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit))
                {
                    //ImGui.TableSetupColumn("ID");
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("Price");
                    ImGui.TableSetupColumn("Zone");
                    ImGui.TableSetupColumn("Actions");
                    ImGui.TableHeadersRow();
                    foreach (var item in Ventures)
                    {
                        //ImGui.TableNextColumn();
                        //ImGui.Text(item.Id.ToString());
                        ImGui.TableNextColumn();
                        //PluginLog.Verbose("Starting ImGui item.Name rendering...");
                        ImGui.Text(item.Name);
                        if (ImGui.IsItemHovered() && C.Debug)
                        {
                            // Display a tooltip or additional info
                            ImGui.BeginTooltip();
                            ImGui.Text($"ID: {item.Id}\nCat: {item.Category}\nShopId: {item.Shop.ShopId}\nNPCName: {item.Shop.NpcName}\nNPCID: {item.Shop.NpcId}");
                            ImGui.EndTooltip();
                        }
                        ImGui.TableNextColumn();
                        //PluginLog.Verbose("Starting ImGui item.Price rendering...");
                        UiHelper.Rightalign(item.Price.ToString(), true);
                        //ImGui.Text(item.Price.ToString());
                        ImGui.TableNextColumn();
                        ImGui.Text(item.Shop.Location != null ? item.Shop.Location.Zone : "");
                        ImGui.TableNextColumn();
                        //PluginLog.Verbose("Starting ImGui Flag rendering...");
                        if (item.Shop.Location != null && item.Shop.Location != Location.locations[0])
                        {
                            if (ImGui.Button("Flag##collectable" + item.Id + "-" + item.ShopId))
                            {
                                Service.GameGui.OpenMapWithMapLink(item.Shop.Location.GetMapMarker());
                            }
                            ImGui.SameLine();
                            if (ImGui.Button("TP##collectable" + item.Id + "-" + item.ShopId))
                            {
                                item.Shop.Location.Teleport();
                                Service.GameGui.OpenMapWithMapLink(item.Shop.Location.GetMapMarker());
                            }
                        }
                        //PluginLog.Verbose("Ending ImGui Flag rendering...");
                    }
                    //}
                    //PluginLog.Verbose("Starting ImGui EndTable rendering...");
                    ImGui.EndTable();
                }
                ImGui.Separator();
            }

            ImGui.Text($"Sellable items on the marketboard:");

            if (ImGui.BeginTable("##markettable", 8, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Sortable))
            {
                // Set up columns
                ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("Sales", ImGuiTableColumnFlags.None);
                ImGui.TableSetupColumn("Price", ImGuiTableColumnFlags.None);
                ImGui.TableSetupColumn("Qty", ImGuiTableColumnFlags.None);
                ImGui.TableSetupColumn("Sells for", ImGuiTableColumnFlags.None);
                ImGui.TableSetupColumn("Total", ImGuiTableColumnFlags.None);
                ImGui.TableSetupColumn("Zone", ImGuiTableColumnFlags.NoSort);
                ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.NoSort);
                ImGui.TableHeadersRow();

                // Get sorting specs
                ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();
                List<ShopItem> filteredItems = ShopHelper.GetItems(Currency);

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
                foreach (ShopItem item in filteredItems)
                {
                    item.Profit = item.CurrentPrice * item.AmountCanBuy;
                    ImGui.TableNextColumn();
                    if (ImGui.Selectable(item.Name)) // Make the name clickable
                    {
                        ImGui.SetClipboardText(item.Name); // Copy the name to the clipboard
                        Notify.Success("Name copied to clipboard");
                        PluginLog.Verbose($"Copied '{item.Name}' to clipboard.");
                    }
                    if (ImGui.IsItemHovered() && C.Debug)
                    {
                        // Display a tooltip or additional info
                        ImGui.BeginTooltip();
                        ImGui.Text($"ID: {item.Id}\nName: {item.Name}\nCat: {item.Category}\nNPC:{item.Shop.NpcName}\nShop:{item.Shop.ShopId}\nNpcName: {item.Shop.NpcName}\nNpcId: {item.Shop.NpcId}");
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
                    ImGui.Text(item.Shop.Location.Zone);
                    ImGui.TableNextColumn();
                    if (item.Shop.Location != null && item.Shop.Location != Location.locations[0])
                    {
                        if (ImGui.Button("Flag##collectable" + item.Id + "-" + item.ShopId))
                        {
                            Service.GameGui.OpenMapWithMapLink(item.Shop.Location.GetMapMarker());
                        }
                        ImGui.SameLine();
                        if (ImGui.Button("TP##collectable" + item.Id + "-" + item.ShopId))
                        {
                            item.Shop.Location.Teleport();
                            Service.GameGui.OpenMapWithMapLink(item.Shop.Location.GetMapMarker());
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
