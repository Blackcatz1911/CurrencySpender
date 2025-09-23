using CurrencySpender.Classes;
using CurrencySpender.Data;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;

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
        P.ws.AddWindow(this);
    }
    public unsafe override void Draw()
    {
        if(Currency == null) return;
        //WindowName = "SpendingGuide: " + this.CurrencyName;
        ImGui.Image(Currency.Icon.Handle, new Vector2(21, 21));
        ImGui.SameLine();
        UiHelper.LeftAlign($"{Currency.Name}: {Currency.CurrentCount}");
        if (C.Debug)
        {
            UiHelper.LeftAlign($"DEBUG: CurrencyId: {Currency.ItemId}");
            UiHelper.LeftAlign($"DEBUG: CollectableItems: {CollectableItems?.Count} | SellableItems: {SellableItems?.Count} | " +
                $"ItemsOfInterest: {ItemsOfInterest?.Count}");
            UiHelper.LeftAlign($"DEBUG: Storm: {PlayerHelper.GCRanks[1]} Serpent: {PlayerHelper.GCRanks[2]} Flame: {PlayerHelper.GCRanks[3]}");
        }
        List<uint> ids = [20, 21, 22];
        if (ids.Contains(Currency.ItemId)) {
            if(!PlayerHelper.GCRanksCreated) PlayerHelper.init();
            if (PlayerHelper.GCRanks[Currency.ItemId - 19] < 10)
            {
                UiHelper.WarningText("Some items cannot be purchased yet due to GC rankings... So they will not be displayed here.");
            }
        }
        if (Currency.ItemId == 26807 && !PlayerHelper.SharedFateRanksMax)
        {
            UiHelper.WarningText("Some items cannot be purchased yet due to shared FATE rankings... So they will not be displayed here.");
        }

        try
        {
            if (C.ShowItemsOfInterest && ItemsOfInterest != null && ItemsOfInterest.Count > 0)
            {
                ImGui.Separator();
                UiHelper.LeftAlign($"Can buy items of interest:");
                if (ImGui.BeginTable("##itemsofinterest", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Sortable))
                {
                    //ImGui.TableSetupColumn("ID");
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("Price", ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn("Zone");
                    ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.NoSort);
                    ImGui.TableHeadersRow();

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
                            case 0: // Name
                                ItemsOfInterest = ascending
                                    ? ItemsOfInterest.OrderBy(item => item.Name).ToList()
                                    : ItemsOfInterest.OrderByDescending(item => item.Name).ToList();
                                break;

                            case 1: // Price
                                ItemsOfInterest = ascending
                                    ? ItemsOfInterest.OrderBy(item => item.Price).ToList()
                                    : ItemsOfInterest.OrderByDescending(item => item.Price).ToList();
                                break;

                            case 2: // Zone
                                ItemsOfInterest = ascending
                                    ? ItemsOfInterest.OrderBy(item => item.Shop.Location.Zone).ToList()
                                    : ItemsOfInterest.OrderByDescending(item => item.Shop.Location.Zone).ToList();
                                break;
                            default:
                                break; // Do nothing for unhandled columns
                        }
                    }

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
                        using (var context = ImRaii.ContextPopupItem($"context##{item.Id}-{item.ShopId}-{item.Shop.NpcId}"))
                        {
                            if (context)
                            {
                                if (ImGui.Selectable("Copy item name"))
                                {
                                    ImGui.SetClipboardText(item.Name);
                                    UiHelper.Notification("Copied item name to clipboard");
                                }
                                if (ImGui.Selectable("Create item link"))
                                {
                                    UiHelper.LinkItem(item.Id);
                                    UiHelper.Notification("Item link created");
                                }
                            }
                        }
                        if (item.Currency != Currency.ItemId)
                        {
                            var child_cur = P.Currencies.Where(cur => cur.ItemId == item.Currency).First();
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
                        UiHelper.RightAlignWithIcon(item.Price.ToString(), Currency.Icon.Handle, true);
                        //UiHelper.LeftAlign(item.Price.ToString());
                        ImGui.TableNextColumn();
                        UiHelper.LeftAlign(item.Shop.Location != null ? item.Shop.Location.Zone : "");
                        ImGui.TableNextColumn();
                        //PluginLog.Verbose("Starting ImGui Flag rendering...");
                        UiHelper.BuildMapButtons(item);
                        //PluginLog.Verbose("Ending ImGui Flag rendering...");
                    }
                    //}
                    //PluginLog.Verbose("Starting ImGui EndTable rendering...");
                    ImGui.EndTable();
                }
            }

            if (C.ShowCollectables && CollectableItems != null && CollectableItems.Count > 0)
            {
                ImGui.Separator();
                UiHelper.LeftAlign("Selected collectables not yet registered:");
                if (ImGui.BeginTable("##collectables", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Sortable))
                {
                    //ImGui.TableSetupColumn("ID");
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("Price", ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn("Zone");
                    ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.NoSort);
                    ImGui.TableHeadersRow();

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
                            case 0: // Name
                                CollectableItems = ascending
                                    ? CollectableItems.OrderBy(item => item.Name).ToList()
                                    : CollectableItems.OrderByDescending(item => item.Name).ToList();
                                break;

                            case 1: // Price
                                CollectableItems = ascending
                                    ? CollectableItems.OrderBy(item => item.Price).ToList()
                                    : CollectableItems.OrderByDescending(item => item.Price).ToList();
                                break;

                            case 2: // Zone
                                CollectableItems = ascending
                                    ? CollectableItems.OrderBy(item => item.Shop.Location.Zone).ToList()
                                    : CollectableItems.OrderByDescending(item => item.Shop.Location.Zone).ToList();
                                break;
                            default:
                                break; // Do nothing for unhandled columns
                        }
                    }

                    foreach (var item in CollectableItems)
                    {
                        ImGui.TableNextRow();
                        //ImGui.TableNextColumn();
                        //UiHelper.LeftAlign(item.Id.ToString());
                        //ImGui.TableNextColumn();
                        ImGui.TableSetColumnIndex(0);
                        //PluginLog.Verbose("Starting ImGui item.Name rendering...");
                        
                        if (ItemHelper.ContainerUnlocked.ContainsKey(item.Id))
                        {
                            if (ItemHelper.ContainerUnlocked.TryGetValue(item.Id, out (uint, uint) tuple))
                            {
                                UiHelper.LeftAlign(item.Name+" (" +tuple.Item1 + "/"+ tuple.Item2+")");
                            }
                        } else
                        {
                            UiHelper.LeftAlign(item.Name);
                        }
                        using (var context = ImRaii.ContextPopupItem($"context##{item.Id}-{item.ShopId}-{item.Shop.NpcId}"))
                        {
                            if (context)
                            {
                                if (ImGui.Selectable("Copy item name"))
                                {
                                    ImGui.SetClipboardText(item.Name);
                                    UiHelper.Notification("Copied item name to clipboard");
                                }
                                if (ImGui.Selectable("Create item link"))
                                {
                                    UiHelper.LinkItem(item.Id);
                                    UiHelper.Notification("Item link created");
                                }
                            }
                        }
                        if (ImGui.IsItemHovered() && C.Debug)
                        {
                            // Display a tooltip or additional info
                            ImGui.BeginTooltip();
                            UiHelper.LeftAlign($"ID: {item.Id}\nCollectableType: {item.CollectableType}\nIsUnlocked: {ItemHelper.IsUnlocked(item.Id)}\nCat: {item.Category}\nShopId: {item.Shop.ShopId}\nNPCName: {item.Shop.NpcName}\nNPCID: {item.Shop.NpcId}\nShopType: {item.Shop.Type}");
                            ImGui.EndTooltip();
                        }
                        if (item.Currency != Currency.ItemId)
                        {
                            var child_cur = P.Currencies.Where(cur => cur.ItemId == item.Currency).First();
                            var items = Generator.items.Where(cur => cur.Id == item.Currency).ToList();
                            foreach (var item_ in items)
                            {
                                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3);
                                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 20);
                                UiHelper.LeftAlign(child_cur.Name);
                            }
                            //if (ImGui.IsItemHovered() && C.Debug)
                            //{
                            //    // Display a tooltip or additional info
                            //    ImGui.BeginTooltip();
                            //    if(child_item != null)
                            //    UiHelper.LeftAlign($"ID: {child_item.Id}\nCat: {child_item.Category}\nShopId: {child_item.Shop.ShopId}\nNPCName: {child_item.Shop.NpcName}\nNPCID: {child_item.Shop.NpcId}\nShopType: {child_item.Shop.Type}");
                            //    ImGui.EndTooltip();
                            //}
                        }
                        ImGui.TableSetColumnIndex(1);
                        //UiHelper.LeftAlign(item.Price.ToString());
                        //UiHelper.Rightalign(item.Price.ToString(), false);
                        if(item.Currency == Currency.ItemId)
                            UiHelper.RightAlignWithIcon(item.Price.ToString(), Currency.Icon.Handle, true);
                        if (item.Currency != Currency.ItemId)
                        {
                            var child_cur = P.Currencies.Where(cur => cur.ItemId == item.Currency).First();
                            UiHelper.RightAlignWithIcon(item.Price.ToString(), child_cur.Icon.Handle, true);
                            var items = Generator.items.Where(cur => cur.Id == item.Currency).ToList();
                            foreach (var item_ in items)
                            {
                                UiHelper.RightAlignWithIcon((item.Price * item_.Price).ToString(), Currency.Icon.Handle, true);
                            }
                            //UiHelper.LeftAlign("test2");
                            //UiHelper.Rightalign(item.Price.ToString(), Currency.Icon.ImGuiHandle, true);
                            //UiHelper.LeftAlign(item.Price.ToString());
                            //    //UiHelper.Rightalign(item.Price.ToString(), true);
                            //    ImGui.SameLine();
                            //    var child_cur = P.Currencies.Where(cur => cur.ItemId == item.Currency).First();
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
                            var items = Generator.items.Where(cur => cur.Id == item.Currency).ToList();
                            foreach(var item_ in items)
                            {
                                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3);
                                UiHelper.LeftAlign(item_.Shop.Location != null ? item_.Shop.Location.Zone : "Unknown");
                            }
                            //UiHelper.LeftAlign("test2");
                            //UiHelper.Rightalign(item.Price.ToString(), Currency.Icon.ImGuiHandle, true);
                            //UiHelper.LeftAlign(item.Price.ToString());
                            //    //UiHelper.Rightalign(item.Price.ToString(), true);
                            //    ImGui.SameLine();
                            //    var child_cur = P.Currencies.Where(cur => cur.ItemId == item.Currency).First();
                            //    ImGui.Image(child_cur.Icon.ImGuiHandle, new Vector2(20, 20));
                            //    UiHelper.LeftAlign((item.Price * child_cur.Price).ToString());
                            //    ImGui.SameLine();
                            //    ImGui.Image(Currency.Icon.ImGuiHandle, new Vector2(20, 20));
                        }
                        ImGui.TableSetColumnIndex(3);
                        //PluginLog.Verbose("Starting ImGui Flag rendering...");
                        UiHelper.BuildMapButtons(item);
                        if (item.Currency != Currency.ItemId)
                        {
                            var items = Generator.items.Where(cur => cur.Id == item.Currency).ToList();
                            foreach (var item_ in items)
                            {
                                UiHelper.BuildMapButtons(item_);
                            }
                        }
                        //PluginLog.Verbose("Ending ImGui Flag rendering...");
                    }
                    //}
                    //PluginLog.Verbose("Starting ImGui EndTable rendering...");
                    ImGui.EndTable();
                }
            }

            if (C.ShowVentures && Ventures != null && Ventures.Count > 0)
            {
                ImGui.Separator();
                UiHelper.LeftAlign($"Can buy ventures:");
                if (ImGui.BeginTable("##ventures", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Sortable))
                {
                    //ImGui.TableSetupColumn("ID");
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("Price", ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn("Zone");
                    ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.NoSort);
                    ImGui.TableHeadersRow();

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
                            case 0: // Name
                                Ventures = ascending
                                    ? Ventures.OrderBy(item => item.Name).ToList()
                                    : Ventures.OrderByDescending(item => item.Name).ToList();
                                break;

                            case 1: // Price
                                Ventures = ascending
                                    ? Ventures.OrderBy(item => item.Price).ToList()
                                    : Ventures.OrderByDescending(item => item.Price).ToList();
                                break;

                            case 2: // Zone
                                Ventures = ascending
                                    ? Ventures.OrderBy(item => item.Shop.Location.Zone).ToList()
                                    : Ventures.OrderByDescending(item => item.Shop.Location.Zone).ToList();
                                break;
                            default:
                                break; // Do nothing for unhandled columns
                        }
                    }

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
                        using (var context = ImRaii.ContextPopupItem($"context##{item.Id}-{item.ShopId}-{item.Shop.NpcId}"))
                        {
                            if (context)
                            {
                                if (ImGui.Selectable("Copy item name"))
                                {
                                    ImGui.SetClipboardText(item.Name);
                                    UiHelper.Notification("Copied item name to clipboard");
                                }
                                if (ImGui.Selectable("Create item link"))
                                {
                                    UiHelper.LinkItem(item.Id);
                                    UiHelper.Notification("Item link created");
                                }
                            }
                        }
                        if (item.Currency != Currency.ItemId)
                        {
                            var child_cur = P.Currencies.Where(cur => cur.ItemId == item.Currency).First();
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
                        UiHelper.RightAlignWithIcon(item.Price.ToString(), Currency.Icon.Handle, true);
                        //UiHelper.LeftAlign(item.Price.ToString());
                        ImGui.TableNextColumn();
                        UiHelper.LeftAlign(item.Shop.Location != null ? item.Shop.Location.Zone : "");
                        ImGui.TableNextColumn();
                        UiHelper.BuildMapButtons(item);
                        //PluginLog.Verbose("Ending ImGui Flag rendering...");
                    }
                    //}
                    //PluginLog.Verbose("Starting ImGui EndTable rendering...");
                    ImGui.EndTable();
                }
            }

            if (C.ShowSellables && SellableItems != null && SellableItems.Count > 0)
            {
                ImGui.Separator();
                UiHelper.LeftAlign($"Items eligible for sale on the marketboard:");
                if (ImGui.BeginTable("##markettable", 7, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Sortable))
                {
                    // Set up columns
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("Sales");
                    ImGui.TableSetupColumn("Price", ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn("Qty");
                    ImGui.TableSetupColumn("Sells for");
                    ImGui.TableSetupColumn("Total");
                    //ImGui.TableSetupColumn("Zone", ImGuiTableColumnFlags.NoSort);
                    ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.NoSort);
                    ImGui.TableHeadersRow();

                    // Get sorting specs
                    ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();
                    //List<ShopItem> SellableItems = ShopHelper.GetSellableItems(Currency);

                    if (!sortSpecs.IsNull && sortSpecs.SpecsCount > 0 && SellableItems.Count > 0)
                    {
                        // Retrieve sorting specification
                        ImGuiTableColumnSortSpecsPtr spec = sortSpecs.Specs;
                        int columnIndex = spec.ColumnIndex;
                        bool ascending = spec.SortDirection == ImGuiSortDirection.Ascending;
                        
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
                        UiHelper.LeftAlign(item.Name);
                        if (ImGui.IsItemHovered() && C.Debug)
                        {
                            // Display a tooltip or additional info
                            ImGui.BeginTooltip();
                            UiHelper.LeftAlign($"ID: {item.Id}\nName: {item.Name}\nCat: {item.Category}\nNPC:{item.Shop.NpcName}\nShop:{item.Shop.ShopId}\nNpcName: {item.Shop.NpcName}\nNpcId: {item.Shop.NpcId}");
                            ImGui.EndTooltip();
                        }
                        using (var context = ImRaii.ContextPopupItem($"context##{item.Id}-{item.ShopId}-{item.Shop.NpcId}"))
                        {
                            if (context)
                            {
                                if (ImGui.Selectable("Copy item name"))
                                {
                                    ImGui.SetClipboardText(item.Name);
                                    UiHelper.Notification("Copied item name to clipboard");
                                }
                                if (ImGui.Selectable("Create item link"))
                                {
                                    UiHelper.LinkItem(item.Id);
                                    UiHelper.Notification("Item link created");
                                }
                            }
                        }
                        ImGui.TableNextColumn();
                        UiHelper.RightAlign(item.HasSoldWeek.ToString(), true);
                        ImGui.TableNextColumn();
                        if (item.Currency == Currency.ItemId)
                        {
                            UiHelper.RightAlignWithIcon(item.Price.ToString(), Currency.Icon.Handle, true);
                        }
                        else
                        {
                            var child_cur = P.Currencies.Where(cur => cur.ItemId == item.Currency).First();
                            UiHelper.RightAlignWithIcon(item.Price.ToString(), child_cur.Icon.Handle, true);
                            UiHelper.RightAlignWithIcon((item.Price * child_cur.Price).ToString(), Currency.Icon.Handle, true);
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
                        UiHelper.BuildMapButtons(item);
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
    public void UpdateData()
    {
        if(Currency == null) { return; }
        CollectableItems = ShopHelper.GetCollectableItems(Currency);
        Ventures = ShopHelper.GetVentures(Currency);
        SellableItems = ShopHelper.GetSellableItems(Currency);
        ItemsOfInterest = ShopHelper.GetItemsOfInterest(Currency);
    }
}
