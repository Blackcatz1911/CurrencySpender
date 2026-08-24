using System.Drawing;
using CurrencySpender.Classes;
using CurrencySpender.Data;
using CurrencySpender.Tasks;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;

namespace CurrencySpender.Windows;
internal class SpendingWindow : Window
{
    private static readonly List<uint> GcSealIds = [20, 21, 22];

    public static TrackedCurrency? Currency;
    internal static List<ShopItem>? CollectableItems;
    internal static List<ShopItem>? Ventures;
    internal static List<ShopItem>? SellableItems;
    internal static List<ShopItem>? ItemsOfInterest;
    
    private static ILookup<uint, ShopItem> ItemsByCurrency = Enumerable.Empty<ShopItem>().ToLookup(i => i.Currency);

    private bool lastDebug;
    private bool lastGlued;

    public SpendingWindow() : base($"{P.Name} {P.Version}###{P.Name}SpendingWindow")
    {
        this.SizeConstraints = new()
        {
            MinimumSize = new(600, 200),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };
        UpdateTitleBarButtons();

        P.ws.AddWindow(this);
    }

    private void UpdateTitleBarButtons()
    {
        TitleBarButtons.Clear();
        if (C.Debug)
        {
            TitleBarButtons.Add(new()
            {
                Click = (m) =>
                {
                    if (m == ImGuiMouseButton.Left && Currency != null)
                    {
                        P.TaskManager.Enqueue(() => WebHelper.CheckAll(Currency.ItemId, true));
                    }
                },
                Icon = FontAwesomeIcon.Sync,
                IconOffset = new(2, 2),
                ShowTooltip = () => ImGui.SetTooltip("Force refresh Universalis"),
            });
        }

        TitleBarButtons.Add(new()
        {
            Click = (m) =>
            {
                if (m == ImGuiMouseButton.Left)
                {
                    C.GlueToMainWindow = !C.GlueToMainWindow;
                    if (!C.GlueToMainWindow)
                    {
                        Position = null;
                        Size = null;
                        PositionCondition = ImGuiCond.None;
                        SizeCondition = ImGuiCond.None;
                    }
                }
            },
            Icon = FontAwesomeIcon.Clone,
            IconColor = C.GlueToMainWindow ? EColor.YellowBright : null,
            IconOffset = new(2, 2),
            ShowTooltip = () => ImGui.SetTooltip(C.GlueToMainWindow ? "Unglue from the main window" : "Glue to the main window"),
        });
    }

    public override bool DrawConditions()
    {
        return UiHelper.DrawConditions();
    }

    public override void PreDraw()
    {
        WindowName = $"{P.Name} {P.Version}###{P.Name}SpendingWindow";
        if (lastDebug != C.Debug || lastGlued != C.GlueToMainWindow)
        {
            lastDebug = C.Debug;
            lastGlued = C.GlueToMainWindow;
            UpdateTitleBarButtons();
        }
    }

    public override void Draw()
    {
        if(Currency == null) return;
        if (C.GlueToMainWindow)
        {
            var mainPos = MainTabWindow.LastPos;
            var mainSize = MainTabWindow.LastSize;
            if (mainPos != Vector2.Zero && mainSize != Vector2.Zero)
            {
                float x = C.GlueSide == GlueSide.Left
                    ? mainPos.X - ImGui.GetWindowSize().X - 5
                    : mainPos.X + mainSize.X + 5;
                Position = new Vector2(x, mainPos.Y);
                PositionCondition = ImGuiCond.Always;
                Size = new Vector2(ImGui.GetWindowSize().X, mainSize.Y);
                SizeCondition = ImGuiCond.Always;
            }
        }
        else
        {
            Position = null;
            Size = null;
            PositionCondition = ImGuiCond.None;
            SizeCondition = ImGuiCond.None;
        }

        ImGui.Image(Currency.Icon.Handle, new Vector2(21, 21));
        ImGui.SameLine();
        UiHelper.LeftAlign($"{Currency.Name}: {Currency.CurrentCount}");
        UiHelper.LeftAlign($"Status: {MovementTask.Status}");
        if (MovementTask.Status != "Idle")
        {
            ImGui.SameLine();
            if (ImGui.Button("Cancel"))
            {
                P.TaskManager.Abort();
                MovementTask.Cancel();
            }
        }
        if (C.Debug)
        {
            UiHelper.LeftAlign($"DEBUG: CurrencyId: {Currency.ItemId}");
            UiHelper.LeftAlign($"DEBUG: CollectableItems: {CollectableItems?.Count} | SellableItems: {SellableItems?.Count} | " +
                $"ItemsOfInterest: {ItemsOfInterest?.Count}");
            UiHelper.LeftAlign($"DEBUG: Storm: {PlayerHelper.GCRanks[1]} Serpent: {PlayerHelper.GCRanks[2]} Flame: {PlayerHelper.GCRanks[3]}");
            UiHelper.LeftAlign($"DEBUG: GlueToMainWindow: {C.GlueToMainWindow}");
        }
        if (GcSealIds.Contains(Currency.ItemId)) {
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
        if(CollectableItems?.Count == 0 && SellableItems?.Count == 0 && ItemsOfInterest?.Count == 0 && Ventures?.Count == 0)
            UiHelper.WarningText("Nothing can be displayed. Please check your settings. Especially the minimum sales.");

        if (!ItemHelper.AchievementsLoaded() && HasAchievementPrereqs() && PlayerHelper.IsAchievementWindowUnlocked())
        {
            UiHelper.WarningText("Achievement data not loaded yet. Some items may be hidden incorrectly.");
            ImGui.SameLine();
            if (ImGui.Button("Open Achievement Window"))
            {
                PlayerHelper.OpenAchievementWindow();
            }
        }

        DrawItemsOfInterest();
        DrawCollectables();
        DrawVentures();
        DrawSellables();
    }

    // ------------------------------------------------------------------
    // Shared building blocks
    // ------------------------------------------------------------------

    private static void BeginItemTable(string id)
    {
        ImGui.BeginTable(id, 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Sortable);
        ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("Price", ImGuiTableColumnFlags.WidthFixed, 100);
        ImGui.TableSetupColumn("Zone");
        ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.NoSort);
        ImGui.TableHeadersRow();
    }

    private static List<ShopItem> SortByColumn(List<ShopItem> items, int column, bool ascending)
    {
        return (column, ascending) switch
        {
            (0, true) => items.OrderBy(item => item.Name).ToList(),
            (0, false) => items.OrderByDescending(item => item.Name).ToList(),
            (1, true) => items.OrderBy(item => item.Price).ToList(),
            (1, false) => items.OrderByDescending(item => item.Price).ToList(),
            (2, true) => items.OrderBy(item => item.Shop.Location?.Zone ?? "").ToList(),
            (2, false) => items.OrderByDescending(item => item.Shop.Location?.Zone ?? "").ToList(),
            _ => items,
        };
    }

    private static void ItemContextMenu(ShopItem item)
    {
        using var context = ImRaii.ContextPopupItem($"context##{item.Id}-{item.ShopId}-{item.Shop.NpcId}");
        if (!context) return;
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

    private static void DebugTooltip(string text)
    {
        if (!C.Debug || !ImGui.IsItemHovered()) return;
        ImGui.BeginTooltip();
        UiHelper.LeftAlign(text);
        ImGui.EndTooltip();
    }

    private static void DrawSimpleRow(ShopItem item, TrackedCurrency currency)
    {
        ImGui.TableNextColumn();
        UiHelper.LeftAlign(item.Name);
        DebugTooltip($"ID: {item.Id}\nCat: {item.Category}\nShopId: {item.Shop.ShopId}\nNPCName: {item.Shop.NpcName}\nNPCID: {item.Shop.NpcId}");
        ItemContextMenu(item);

        var childCur = P.GetCurrency(item.Currency);
        if (item.Currency != currency.ItemId && childCur != null)
        {
            UiHelper.LeftAlign(childCur.Name);
            DebugTooltip($"ID: {childCur.ItemId}");
        }

        ImGui.TableNextColumn();
        UiHelper.RightAlignWithIcon(item.Price.ToString(), currency.Icon.Handle, true);

        ImGui.TableNextColumn();
        UiHelper.LeftAlign(item.Shop.Location?.Zone ?? "");

        ImGui.TableNextColumn();
        UiHelper.BuildMapButtons(item);
    }

    private static void DrawFourColumnSection(string header, string tableId, List<ShopItem>? items)
    {
        if (items == null || items.Count == 0) return;
        ImGui.Separator();
        UiHelper.LeftAlign(header);
        if (!ImGui.BeginTable(tableId, 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Sortable))
            return;

        ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("Price", ImGuiTableColumnFlags.WidthFixed, 100);
        ImGui.TableSetupColumn("Zone");
        ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.NoSort);
        ImGui.TableHeadersRow();

        // Read sort specs every frame (ImGui persists them; SpecsDirty only fires on click)
        ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();
        if (!sortSpecs.IsNull && sortSpecs.SpecsCount > 0)
        {
            ImGuiTableColumnSortSpecsPtr spec = sortSpecs.Specs;
            items = SortByColumn(items, spec.ColumnIndex, spec.SortDirection == ImGuiSortDirection.Ascending);
        }

        var currency = Currency!;
        foreach (var item in items)
        {
            ImGui.TableNextRow();
            DrawSimpleRow(item, currency);
        }
        ImGui.EndTable();
    }

    private void DrawItemsOfInterest()
    {
        if (!C.ShowItemsOfInterest) return;
        DrawFourColumnSection("Can buy items of interest:", "##itemsofinterest", ItemsOfInterest);
    }

    private void DrawVentures()
    {
        if (!C.ShowVentures) return;
        DrawFourColumnSection("Can buy ventures:", "##ventures", Ventures);
    }

    private void DrawCollectables()
    {
        var currency = Currency;
        var items = CollectableItems;
        if (!C.ShowCollectables || currency == null || items == null || items.Count == 0) return;

        ImGui.Separator();
        UiHelper.LeftAlign("Selected collectables not yet registered:");
        if (!ImGui.BeginTable("##collectables", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Sortable))
            return;

        ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("Price", ImGuiTableColumnFlags.WidthFixed, 100);
        ImGui.TableSetupColumn("Zone");
        ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.NoSort);
        ImGui.TableHeadersRow();

        // Read sort specs every frame (ImGui persists them; SpecsDirty only fires on click)
        ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();
        if (!sortSpecs.IsNull && sortSpecs.SpecsCount > 0)
        {
            ImGuiTableColumnSortSpecsPtr spec = sortSpecs.Specs;
            items = SortByColumn(items, spec.ColumnIndex, spec.SortDirection == ImGuiSortDirection.Ascending);
        }

        foreach (var item in items)
        {
            var childCur = P.GetCurrency(item.Currency) ?? currency;
            var childItems = item.Currency != currency.ItemId
                ? ItemsByCurrency[item.Currency].ToList()
                : null;

            ImGui.TableNextRow();

            // --- Name ---
            ImGui.TableSetColumnIndex(0);
            if (ItemHelper.ContainerUnlocked.TryGetValue(item.Id, out var tuple))
            {
                UiHelper.LeftAlign(item.Name + " (" + tuple.Item1 + "/" + tuple.Item2 + ")");
            }
            else
            {
                if (item.PreReq) UiHelper.PrereqIcon(item);
                UiHelper.LeftAlign(item.Name);
            }
            ItemContextMenu(item);
            DebugTooltip($"ID: {item.Id}\nCollectableType: {item.CollectableType}\nIsUnlocked: {ItemHelper.IsUnlocked(item.Id)}\nCat: {item.Category}\nShopId: {item.Shop.ShopId}\nNPCName: {item.Shop.NpcName}\nNPCID: {item.Shop.NpcId}\nShopType: {item.Shop.Type}\nShopName: {item.Shop.ShopName}");

            if (childItems != null)
                UiHelper.LeftAlign(childCur.Name);

            // --- Price ---
            ImGui.TableSetColumnIndex(1);
            if (childItems == null)
            {
                UiHelper.RightAlignWithIcon(item.Price.ToString(), currency.Icon.Handle, true, gamba: item.Gamba);
            }
            else
            {
                UiHelper.RightAlignWithIcon(item.Price.ToString(), childCur.Icon.Handle, true);
                foreach (var source in childItems)
                    UiHelper.RightAlignWithIcon((item.Price * source.Price).ToString(), currency.Icon.Handle, true);
            }

            // --- Zone ---
            ImGui.TableSetColumnIndex(2);
            UiHelper.LeftAlign(item.Shop.Location?.Zone ?? "Unknown");
            if (childItems != null)
            {
                foreach (var source in childItems)
                {
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3);
                    UiHelper.LeftAlign(source.Shop.Location?.Zone ?? "Unknown");
                }
            }

            // --- Actions ---
            ImGui.TableSetColumnIndex(3);
            UiHelper.BuildMapButtons(item);
            if (childItems != null)
            {
                foreach (var source in childItems)
                    UiHelper.BuildMapButtons(source);
            }
        }
        ImGui.EndTable();
    }

    private void DrawSellables()
    {
        var currency = Currency;
        var items = SellableItems;
        if (!C.ShowSellables) return;
        if (items == null || items.Count == 0)
        {
            UiHelper.WarningText("No sellable items found. Please also check your minimum sales setting.");
            return;
        }

        ImGui.Separator();
        UiHelper.LeftAlign("Items eligible for sale on the marketboard:");
        if (!ImGui.BeginTable("##markettable", 7, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Sortable))
            return;

        ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("Sales");
        ImGui.TableSetupColumn("Price", ImGuiTableColumnFlags.WidthFixed, 100);
        ImGui.TableSetupColumn("Qty");
        ImGui.TableSetupColumn("Sells for");
        ImGui.TableSetupColumn("Total");
        ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.NoSort);
        ImGui.TableHeadersRow();

        // Read sort specs every frame (ImGui persists them; SpecsDirty only fires on click)
        ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();
        if (!sortSpecs.IsNull && sortSpecs.SpecsCount > 0)
        {
            ImGuiTableColumnSortSpecsPtr spec = sortSpecs.Specs;
            int columnIndex = spec.ColumnIndex;
            bool ascending = spec.SortDirection == ImGuiSortDirection.Ascending;
            items = (columnIndex, ascending) switch
            {
                (0, true) => items.OrderBy(item => item.Name).ToList(),
                (0, false) => items.OrderByDescending(item => item.Name).ToList(),
                (1, true) => items.OrderBy(item => item.HasSoldWeek).ToList(),
                (1, false) => items.OrderByDescending(item => item.HasSoldWeek).ToList(),
                (2, true) => items.OrderBy(item => item.Price).ToList(),
                (2, false) => items.OrderByDescending(item => item.Price).ToList(),
                (3, true) => items.OrderBy(item => item.AmountCanBuy).ToList(),
                (3, false) => items.OrderByDescending(item => item.AmountCanBuy).ToList(),
                (4, true) => items.OrderBy(item => item.CurrentPrice).ToList(),
                (4, false) => items.OrderByDescending(item => item.CurrentPrice).ToList(),
                (5, true) => items.OrderBy(item => item.Profit).ToList(),
                (5, false) => items.OrderByDescending(item => item.Profit).ToList(),
                _ => items,
            };
        }

        foreach (var item in items)
        {
            item.Profit = item.CurrentPrice * item.AmountCanBuy;

            ImGui.TableNextColumn();
            UiHelper.LeftAlign(item.Name);
            DebugTooltip($"ID: {item.Id}\nName: {item.Name}\nCat: {item.Category}\nNPC:{item.Shop.NpcName}\nShop:{item.Shop.ShopId}\nShopType:{item.Shop.Type}\nNpcName: {item.Shop.NpcName}\nNpcId: {item.Shop.NpcId}");
            ItemContextMenu(item);

            ImGui.TableNextColumn();
            UiHelper.RightAlign(item.HasSoldWeek.ToString(), true);

            ImGui.TableNextColumn();
            var childCur = P.GetCurrency(item.Currency);
            if (item.Currency == currency.ItemId || childCur == null)
            {
                UiHelper.RightAlignWithIcon(item.Price.ToString(), currency.Icon.Handle, true);
            }
            else
            {
                UiHelper.RightAlignWithIcon(item.Price.ToString(), childCur.Icon.Handle, true);
                UiHelper.RightAlignWithIcon((item.Price * childCur.Price).ToString(), currency.Icon.Handle, true);
            }

            ImGui.TableNextColumn();
            if (item.Currency == currency.ItemId || childCur == null)
            {
                UiHelper.RightAlign(item.AmountCanBuy.ToString(), true);
            }
            else
            {
                UiHelper.RightAlign($"-\n{item.AmountCanBuy}", true);
            }

            ImGui.TableNextColumn();
            UiHelper.RightAlign(item.CurrentPrice == 0 ? "-" : item.CurrentPrice.ToString(), true);

            ImGui.TableNextColumn();
            if (item.Currency == currency.ItemId || childCur == null)
            {
                UiHelper.RightAlign(item.Profit == 0 ? "-" : item.Profit.ToString(), true);
            }
            else
            {
                UiHelper.RightAlign("-\n-", true);
            }

            ImGui.TableNextColumn();
            UiHelper.BuildMapButtons(item);
        }
        ImGui.EndTable();
    }

    public void GetData(TrackedCurrency cur)
    {
        Currency = cur;
        UpdateData();
    }

    public void UpdateData()
    {
        if(Currency == null) { return; }
        CollectableItems = ShopHelper.GetCollectableItems(Currency);
        Ventures = ShopHelper.GetVentures(Currency);
        SellableItems = ShopHelper.GetSellableItems(Currency);
        ItemsOfInterest = ShopHelper.GetItemsOfInterest(Currency);
        ItemsByCurrency = Generator.items.ToLookup(i => i.Currency);
    }

    private static bool HasAchievementPrereqs()
    {
        return (CollectableItems?.Any(i => i.AchievementId.HasValue) == true)
            || (ItemsOfInterest?.Any(i => i.AchievementId.HasValue) == true)
            || (Ventures?.Any(i => i.AchievementId.HasValue) == true)
            || (SellableItems?.Any(i => i.AchievementId.HasValue) == true);
    }
}
