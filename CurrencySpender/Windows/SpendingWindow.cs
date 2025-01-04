using CurrencySpender.Classes;
using CurrencySpender.Data;
using Dalamud.Interface;

namespace CurrencySpender.Windows;
internal class SpendingWindow : Window
{
    public static TrackedCurrency? Currency;
    internal static List<ShopItem>? CollectableItems;
    internal static List<ShopItem>? Ventures;
    internal static List<ShopItem>? SellableItems;
    internal static List<ShopItem>? ItemsOfInterest;
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
        if(Currency == null) return;
        //WindowName = "SpendingGuide: " + this.CurrencyName;
        ImGui.Image(Currency.Icon.ImGuiHandle, new Vector2(21, 21));
        ImGui.SameLine();
        UiHelper.LeftAlign($"{Currency.Name}? And {Currency.CurrentCount} of them? What to do with that:");
        if (C.Debug)
        {
            UiHelper.LeftAlign($"DEBUG: CurrencyId: {Currency.ItemId}");
            UiHelper.LeftAlign($"DEBUG: CollectableItems: {CollectableItems.Count}");
            UiHelper.LeftAlign($"DEBUG: SellableItems: {SellableItems.Count}");
            UiHelper.LeftAlign($"DEBUG: ItemsOfInterest: {ItemsOfInterest.Count}");
        }
        List<uint> ids = [20, 21, 22];
        if (ids.Contains(Currency.ItemId)) {
            if (PlayerHelper.GCRanks[Currency.ItemId - 19] < 10)
            {
                if (C.Debug) UiHelper.LeftAlign($"DEBUG: GCRank: {PlayerHelper.GCRanks[Currency.ItemId - 19]}/10");
                UiHelper.WarningText("Some items cannot be purchased yet due to GC rankings... So they will not be displayed here.");
            }
        }
        if (Currency.ItemId == 26807 && !PlayerHelper.SharedFateRanksMax)
        {
            UiHelper.WarningText("Some items cannot be purchased yet due to shared FATE rankings... So they will not be displayed here.");
        }

        ImGui.Separator();
        try
        {
            if (C.ShowItemsOfInterest && ItemsOfInterest != null && ItemsOfInterest.Count > 0)
            {
                UiHelper.LeftAlign($"Can buy items of interest:");
                if (ImGui.BeginTable("##itemsofinterest", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit))
                {
                    //ImGui.TableSetupColumn("ID");
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("Price", ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn("Zone");
                    ImGui.TableSetupColumn("Actions");
                    ImGui.TableHeadersRow();
                    foreach (var item in ItemsOfInterest)
                    {
                        //ImGui.TableNextColumn();
                        //UiHelper.LeftAlign(item.Id.ToString());
                        ImGui.TableNextColumn();
                        //PluginLog.Verbose("Starting ImGui item.Name rendering...");
                        UiHelper.LeftAlign(item.Name);
                        if (ImGui.IsItemHovered() && C.Debug)
                        {
                            // Display a tooltip or additional info
                            ImGui.BeginTooltip();
                            UiHelper.LeftAlign($"ID: {item.Id}\nCat: {item.Category}\nShopId: {item.Shop.ShopId}\nNPCName: {item.Shop.NpcName}\nNPCID: {item.Shop.NpcId}");
                            ImGui.EndTooltip();
                        }
                        if (item.Currency != Currency.ItemId)
                        {
                            var child_cur = C.Currencies.Where(cur => cur.ItemId == item.Currency).First();
                            UiHelper.LeftAlign(child_cur.Name);
                            if (ImGui.IsItemHovered() && C.Debug)
                            {
                                // Display a tooltip or additional info
                                ImGui.BeginTooltip();
                                UiHelper.LeftAlign($"ID: {child_cur.ItemId}");
                                ImGui.EndTooltip();
                            }
                        }
                        ImGui.TableNextColumn();
                        //PluginLog.Verbose("Starting ImGui item.Price rendering...");
                        UiHelper.RightAlignWithIcon(item.Price.ToString(), Currency.Icon.ImGuiHandle, true);
                        //UiHelper.LeftAlign(item.Price.ToString());
                        ImGui.TableNextColumn();
                        UiHelper.LeftAlign(item.Shop.Location != null ? item.Shop.Location.Zone : "");
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

            if (CollectableItems != null && CollectableItems.Count > 0)
            {
                UiHelper.LeftAlign($"Collectables not yet registered:");
                if (ImGui.BeginTable("##collectables", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit))
                {
                    //ImGui.TableSetupColumn("ID");
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("Price", ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn("Zone");
                    ImGui.TableSetupColumn("Actions");
                    ImGui.TableHeadersRow();
                    foreach (var item in CollectableItems)
                    {
                        ImGui.TableNextRow();
                        //ImGui.TableNextColumn();
                        //UiHelper.LeftAlign(item.Id.ToString());
                        //ImGui.TableNextColumn();
                        ImGui.TableSetColumnIndex(0);
                        //PluginLog.Verbose("Starting ImGui item.Name rendering...");
                        UiHelper.LeftAlign(item.Name);
                        if (ImGui.IsItemHovered() && C.Debug)
                        {
                            // Display a tooltip or additional info
                            ImGui.BeginTooltip();
                            UiHelper.LeftAlign($"ID: {item.Id}\nCat: {item.Category}\nShopId: {item.Shop.ShopId}\nNPCName: {item.Shop.NpcName}\nNPCID: {item.Shop.NpcId}");
                            ImGui.EndTooltip();
                        }
                        if (item.Currency != Currency.ItemId)
                        {
                            var child_cur = C.Currencies.Where(cur => cur.ItemId == item.Currency).First();
                            UiHelper.LeftAlign(child_cur.Name);
                            if (ImGui.IsItemHovered() && C.Debug)
                            {
                                // Display a tooltip or additional info
                                ImGui.BeginTooltip();
                                UiHelper.LeftAlign($"ID: {child_cur.ItemId}");
                                ImGui.EndTooltip();
                            }
                        }
                        ImGui.TableSetColumnIndex(1);
                        //UiHelper.LeftAlign(item.Price.ToString());
                        //UiHelper.Rightalign(item.Price.ToString(), false);
                        if(item.Currency == Currency.ItemId)
                            UiHelper.RightAlignWithIcon(item.Price.ToString(), Currency.Icon.ImGuiHandle, true);
                        if (item.Currency != Currency.ItemId)
                        {
                            var child_cur = C.Currencies.Where(cur => cur.ItemId == item.Currency).First();
                            UiHelper.RightAlignWithIcon(item.Price.ToString(), child_cur.Icon.ImGuiHandle, true);
                            UiHelper.RightAlignWithIcon((item.Price * child_cur.Price).ToString(), Currency.Icon.ImGuiHandle, true);
                            //UiHelper.LeftAlign("test2");
                            //UiHelper.Rightalign(item.Price.ToString(), Currency.Icon.ImGuiHandle, true);
                            //UiHelper.LeftAlign(item.Price.ToString());
                            //    //UiHelper.Rightalign(item.Price.ToString(), true);
                            //    ImGui.SameLine();
                            //    var child_cur = C.Currencies.Where(cur => cur.ItemId == item.Currency).First();
                            //    ImGui.Image(child_cur.Icon.ImGuiHandle, new Vector2(20, 20));
                            //    UiHelper.LeftAlign((item.Price * child_cur.Price).ToString());
                            //    ImGui.SameLine();
                            //    ImGui.Image(Currency.Icon.ImGuiHandle, new Vector2(20, 20));
                        }
                        //ImGui.TableNextColumn();
                        ImGui.TableSetColumnIndex(2);
                        UiHelper.LeftAlign(item.Shop.Location != null ? item.Shop.Location.Zone : "Unknown");
                        if (item.Currency != Currency.ItemId)
                        {
                            var item_ = Generator.items.Where(cur => cur.Id == item.Currency).First();
                            UiHelper.LeftAlign(item_.Shop.Location != null ? item_.Shop.Location.Zone : "Unknown");
                            //UiHelper.LeftAlign("test2");
                            //UiHelper.Rightalign(item.Price.ToString(), Currency.Icon.ImGuiHandle, true);
                            //UiHelper.LeftAlign(item.Price.ToString());
                            //    //UiHelper.Rightalign(item.Price.ToString(), true);
                            //    ImGui.SameLine();
                            //    var child_cur = C.Currencies.Where(cur => cur.ItemId == item.Currency).First();
                            //    ImGui.Image(child_cur.Icon.ImGuiHandle, new Vector2(20, 20));
                            //    UiHelper.LeftAlign((item.Price * child_cur.Price).ToString());
                            //    ImGui.SameLine();
                            //    ImGui.Image(Currency.Icon.ImGuiHandle, new Vector2(20, 20));
                        }
                        ImGui.TableSetColumnIndex(3);
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
                        if (item.Currency != Currency.ItemId)
                        {
                            var item_ = Generator.items.Where(cur => cur.Id == item.Currency).First();
                            if (ImGui.Button("Flag##collectable" + item_.Id + "-" + item_.ShopId))
                            {
                                Service.GameGui.OpenMapWithMapLink(item_.Shop.Location.GetMapMarker());
                            }
                            ImGui.SameLine();
                            if (ImGui.Button("TP##collectable" + item_.Id + "-" + item_.ShopId))
                            {
                                item_.Shop.Location.Teleport();
                                Service.GameGui.OpenMapWithMapLink(item_.Shop.Location.GetMapMarker());
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

            if (Ventures != null && Ventures.Count > 0 && C.ShowVentures)
            {
                UiHelper.LeftAlign($"Can buy ventures:");
                if (ImGui.BeginTable("##ventures", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit))
                {
                    //ImGui.TableSetupColumn("ID");
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("Price", ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn("Zone");
                    ImGui.TableSetupColumn("Actions");
                    ImGui.TableHeadersRow();
                    foreach (var item in Ventures)
                    {
                        //ImGui.TableNextColumn();
                        //UiHelper.LeftAlign(item.Id.ToString());
                        ImGui.TableNextColumn();
                        //PluginLog.Verbose("Starting ImGui item.Name rendering...");
                        UiHelper.LeftAlign(item.Name);
                        if (ImGui.IsItemHovered() && C.Debug)
                        {
                            // Display a tooltip or additional info
                            ImGui.BeginTooltip();
                            UiHelper.LeftAlign($"ID: {item.Id}\nCat: {item.Category}\nShopId: {item.Shop.ShopId}\nNPCName: {item.Shop.NpcName}\nNPCID: {item.Shop.NpcId}");
                            ImGui.EndTooltip();
                        }
                        if (item.Currency != Currency.ItemId)
                        {
                            var child_cur = C.Currencies.Where(cur => cur.ItemId == item.Currency).First();
                            UiHelper.LeftAlign(child_cur.Name);
                            if (ImGui.IsItemHovered() && C.Debug)
                            {
                                // Display a tooltip or additional info
                                ImGui.BeginTooltip();
                                UiHelper.LeftAlign($"ID: {child_cur.ItemId}");
                                ImGui.EndTooltip();
                            }
                        }
                        ImGui.TableNextColumn();
                        //PluginLog.Verbose("Starting ImGui item.Price rendering...");
                        UiHelper.RightAlignWithIcon(item.Price.ToString(), Currency.Icon.ImGuiHandle, true);
                        //UiHelper.LeftAlign(item.Price.ToString());
                        ImGui.TableNextColumn();
                        UiHelper.LeftAlign(item.Shop.Location != null ? item.Shop.Location.Zone : "");
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

            UiHelper.LeftAlign($"Items eligible for sale on the marketboard:");
            if (SellableItems != null)
            {
                if (ImGui.BeginTable("##markettable", 7, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Sortable))
                {
                    // Set up columns
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("Sales", ImGuiTableColumnFlags.None);
                    ImGui.TableSetupColumn("Price", ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn("Qty", ImGuiTableColumnFlags.None);
                    ImGui.TableSetupColumn("Sells for", ImGuiTableColumnFlags.None);
                    ImGui.TableSetupColumn("Total", ImGuiTableColumnFlags.None);
                    //ImGui.TableSetupColumn("Zone", ImGuiTableColumnFlags.NoSort);
                    ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.NoSort);
                    ImGui.TableHeadersRow();

                    // Get sorting specs
                    ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();
                    //List<ShopItem> SellableItems = ShopHelper.GetSellableItems(Currency);

                    if (sortSpecs.NativePtr != null && sortSpecs.SpecsCount > 0 && SellableItems.Count > 0)
                    {
                        // Retrieve sorting specification
                        ImGuiTableColumnSortSpecsPtr spec = sortSpecs.Specs;
                        int columnIndex = spec.ColumnIndex;
                        bool ascending = spec.SortDirection == ImGuiSortDirection.Ascending;

                        // Sort based on the column index
                        switch (columnIndex)
                        {
                            case 0: // Name
                                SellableItems = ascending
                                    ? SellableItems.OrderBy(item => item.Name).ToList()
                                    : SellableItems.OrderByDescending(item => item.Name).ToList();
                                break;

                            case 1: // Sales
                                SellableItems = ascending
                                    ? SellableItems.OrderBy(item => item.HasSoldWeek).ToList()
                                    : SellableItems.OrderByDescending(item => item.HasSoldWeek).ToList();
                                break;

                            case 2: // Price
                                SellableItems = ascending
                                    ? SellableItems.OrderBy(item => item.Price).ToList()
                                    : SellableItems.OrderByDescending(item => item.Price).ToList();
                                break;

                            case 3: // Qty
                                SellableItems = ascending
                                    ? SellableItems.OrderBy(item => item.AmountCanBuy).ToList()
                                    : SellableItems.OrderByDescending(item => item.AmountCanBuy).ToList();
                                break;

                            case 4: // Sells for
                                SellableItems = ascending
                                    ? SellableItems.OrderBy(item => item.CurrentPrice).ToList()
                                    : SellableItems.OrderByDescending(item => item.CurrentPrice).ToList();
                                break;

                            case 5: // Total
                                SellableItems = ascending
                                    ? SellableItems.OrderBy(item => item.Profit).ToList()
                                    : SellableItems.OrderByDescending(item => item.Profit).ToList();
                                break;

                            default:
                                break; // Do nothing for unhandled columns
                        }
                    }

                    // Render the table rows
                    foreach (ShopItem item in SellableItems)
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
                            UiHelper.LeftAlign($"ID: {item.Id}\nName: {item.Name}\nCat: {item.Category}\nNPC:{item.Shop.NpcName}\nShop:{item.Shop.ShopId}\nNpcName: {item.Shop.NpcName}\nNpcId: {item.Shop.NpcId}");
                            ImGui.EndTooltip();
                        }
                        ImGui.TableNextColumn();
                        UiHelper.RightAlign(item.HasSoldWeek.ToString(), true);
                        ImGui.TableNextColumn();
                        if (item.Currency == Currency.ItemId)
                        {
                            UiHelper.RightAlignWithIcon(item.Price.ToString(), Currency.Icon.ImGuiHandle, true);
                        }
                        else
                        {
                            var child_cur = C.Currencies.Where(cur => cur.ItemId == item.Currency).First();
                            UiHelper.RightAlignWithIcon(item.Price.ToString(), child_cur.Icon.ImGuiHandle, true);
                            UiHelper.RightAlignWithIcon((item.Price * child_cur.Price).ToString(), Currency.Icon.ImGuiHandle, true);
                        }
                        ImGui.TableNextColumn();
                        if (item.Currency == Currency.ItemId)
                        {
                            UiHelper.RightAlign(item.AmountCanBuy.ToString(), true);
                        }
                        else
                        {
                            UiHelper.RightAlign($"-\n{item.AmountCanBuy.ToString()}", true);
                        }
                        ImGui.TableNextColumn();
                        UiHelper.RightAlign(item.CurrentPrice == 0 ? "-" : item.CurrentPrice.ToString(), true);
                        ImGui.TableNextColumn();
                        if (item.Currency == Currency.ItemId)
                        {
                            UiHelper.RightAlign(item.Profit == 0 ? "-" : item.Profit.ToString(), true);
                        }
                        else
                        {
                            UiHelper.RightAlign("-\n-", true);
                        }
                        //ImGui.TableNextColumn();
                        //UiHelper.LeftAlign(item.Shop.Location.Zone);
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
        }
        catch (Exception e)
        {
            Service.Log.Error(e, "ImGuiTable");
        }
    }

    public void GetData(TrackedCurrency cur)
    {
        Currency = cur;
        CollectableItems = ShopHelper.GetCollectableItems(Currency);
        Ventures = ShopHelper.GetVentures(Currency);
        SellableItems = ShopHelper.GetSellableItems(Currency);
        ItemsOfInterest = ShopHelper.GetItemsOfInterest(Currency);
    }
}
