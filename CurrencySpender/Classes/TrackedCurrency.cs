using Newtonsoft.Json;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Textures;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;

namespace CurrencySpender.Classes;

public enum CurrencyType
{
    Item,
    HighQualityItem,
    Collectable,
    NonLimitedTomestone,
    LimitedTomestone,
}

public unsafe class TrackedCurrency
{
    private uint? iconId;
    private uint? itemId;
    private string? label;

    public required CurrencyType Type { get; init; }

    public uint ItemId
    {
        get => GetItemId();
        init => itemId = IsSpecialCurrency() ? GetItemId() : value;
    }

    [JsonIgnore]
    public IDalamudTextureWrap Icon => Service.TextureProvider.GetFromGameIcon(new GameIconLookup
    {
        HiRes = true, ItemHq = Type is CurrencyType.HighQualityItem, IconId = IconId,
    }).GetWrapOrEmpty();

    [JsonIgnore]
    public uint IconId
    {
        get => Service.DataManager.GetExcelSheet<Item>().GetRow(ItemId).Icon;
        set => iconId = value;
    }

    public uint? Threshold;

    public uint? Parent;

    public bool Enabled = true;

    public bool ChatWarning;

    public bool ShowInOverlay;

    public bool ShowItemName = true;

    public bool Invert;

    [JsonIgnore] public string Name => label ??= Service.DataManager.GetExcelSheet<Item>()!.GetRow(ItemId).Name.ExtractText() ?? "Unable to read name";

    [JsonIgnore] public bool CanRemove => Type is not (CurrencyType.LimitedTomestone or CurrencyType.NonLimitedTomestone);

    [JsonIgnore] public int CurrentCount => InventoryManager.Instance()->GetInventoryItemCount(ItemId, Type is CurrencyType.HighQualityItem, false, false);

    public int MaxCount;

    [JsonIgnore] public float Percentage => CurrentCount * 100 / MaxCount;

    [JsonIgnore] public bool HasWarning => Invert ? CurrentCount < Threshold : CurrentCount > Threshold;

    private uint GetItemId()
    {
        // Force regenerate itemId for special currencies
        if (IsSpecialCurrency() && itemId is 0 or null)
        {
            itemId = Type switch
            {
                CurrencyType.NonLimitedTomestone => Service.DataManager.GetExcelSheet<TomestonesItem>()!.First(item => item.Tomestones.RowId is 2).Item.RowId,
                CurrencyType.LimitedTomestone => Service.DataManager.GetExcelSheet<TomestonesItem>()!.First(item => item.Tomestones.RowId is 3).Item.RowId,
                _ => throw new Exception($"ItemId not initialized for type: {Type}"),
            };
        }

        return itemId ?? 0;
    }

    private bool IsSpecialCurrency() => Type switch
    {
        CurrencyType.NonLimitedTomestone => true,
        CurrencyType.LimitedTomestone => true,
        _ => false,
    };
}
