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

    public List<uint>? Children;
    public int? Price;
    public bool Child = false;

    public bool Enabled = true;

    public bool ChatWarning;

    public bool ShowInOverlay;

    public bool ShowItemName = true;

    public bool Society = false;

    public bool Invert;

    public bool NeedsPresence;

    public string? AddedInVersion;

    [JsonIgnore] public string Name => label ??= Service.DataManager.GetExcelSheet<Item>()!.GetRow(ItemId).Name.ExtractText() ?? "Unable to read name";

    [JsonIgnore] public bool CanRemove => Type is not (CurrencyType.LimitedTomestone or CurrencyType.NonLimitedTomestone);

    [JsonIgnore] public int CurrentCount => InventoryManager.Instance()->GetInventoryItemCount(ItemId, Type is CurrencyType.HighQualityItem, false, false);

    public int MaxCount = 0;

    // ReSharper disable once PossibleLossOfFraction
    [JsonIgnore] public float Percentage => MaxCount != 0?CurrentCount * 100 / MaxCount:0;

    [JsonIgnore] public bool HasWarning => Invert ? CurrentCount < Threshold : CurrentCount > Threshold;

    private uint GetItemId()
    {
        // Force regenerate itemId for special currencies
        if (IsSpecialCurrency() && itemId is 0 or null)
        {
            itemId = Type switch
            {
                CurrencyType.NonLimitedTomestone => Service.DataManager.GetExcelSheet<TomestonesItem>().First(item => item.Tomestones.RowId is 2).Item.RowId,
                CurrencyType.LimitedTomestone => Service.DataManager.GetExcelSheet<TomestonesItem>().First(item => item.Tomestones.RowId is 3).Item.RowId,
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

    public static List<TrackedCurrency> GenerateCurrencyList()
    {
        return new List<TrackedCurrency> {
            new() { Type = CurrencyType.Item, ItemId = 20, Threshold = 75000, MaxCount = 90000, AddedInVersion = "1.0.0" }, // StormSeal
            new() { Type = CurrencyType.Item, ItemId = 21, Threshold = 75000, MaxCount = 90000, AddedInVersion = "1.0.0" }, // SerpentSeal
            new() { Type = CurrencyType.Item, ItemId = 22, Threshold = 75000, MaxCount = 90000, AddedInVersion = "1.0.0" }, // FlameSeal

            new() { Type = CurrencyType.Item, ItemId = 29, Threshold = 9999999, MaxCount = 9999999, AddedInVersion = "1.0.0" }, // MGP

            new() { Type = CurrencyType.Item, ItemId = 28, Threshold = 1400, MaxCount = 2000, AddedInVersion = "1.0.0" },                // Poetics
            new() { Type = CurrencyType.NonLimitedTomestone, Threshold = 1400, MaxCount = 2000, AddedInVersion = "1.0.0" },              // NonLimitedTomestone
            new() { Type = CurrencyType.LimitedTomestone, Threshold = 1400, MaxCount = 2000, Enabled = false, AddedInVersion = "1.0.0" }, // LimitedTomestone

            new() { Type = CurrencyType.Item, ItemId = 25, Threshold = 18000, MaxCount = 20000, AddedInVersion = "1.0.0" },    // WolfMarks
            new() { Type = CurrencyType.Item, ItemId = 36656, Threshold = 18000, MaxCount = 20000, AddedInVersion = "1.0.0" }, // TrophyCrystals

            new() { Type = CurrencyType.Item, ItemId = 27, Threshold = 3500, MaxCount = 4000, AddedInVersion = "1.0.0" },                                   // AlliedSeals
            new() { Type = CurrencyType.Item, ItemId = 10307, Threshold = 3500, MaxCount = 4000, Children=[13625, 20308, 21103], AddedInVersion = "1.0.0" }, // CenturioSeals
            new() { Type = CurrencyType.Item, ItemId = 13625, Price = 500, Child=true, AddedInVersion = "1.0.0" },                                            // Centurio Clan Mark
            new() { Type = CurrencyType.Item, ItemId = 20308, Price = 500, Child=true, AddedInVersion = "1.0.0" },                                            // Veteran's Clan Mark
            new() { Type = CurrencyType.Item, ItemId = 21103, Price = 500, Child=true, AddedInVersion = "1.0.0" },                                            // Mythic Clan Mark

            new() { Type = CurrencyType.Item, ItemId = 26533, Threshold = 3500, MaxCount = 4000, AddedInVersion = "1.0.0" }, // SackOfNuts
            new() { Type = CurrencyType.Item, ItemId = 30341, Threshold = 9999, MaxCount = 1000, AddedInVersion = "1.3.0" },  // Faux Leaf
            new() { Type = CurrencyType.Item, ItemId = 21172, Threshold = 999, MaxCount = 999, AddedInVersion = "1.3.0" },  // Achievement Certificate

            new() { Type = CurrencyType.Item, ItemId = 26807, Threshold = 800, MaxCount = 1500, Children=[43961, 35833], AddedInVersion = "1.0.0" }, // BicolorGemstones
            new() { Type = CurrencyType.Item, ItemId = 43961, Price = 100, Child=true, AddedInVersion = "1.0.0" },                                   // Turali Gemstone Voucher
            new() { Type = CurrencyType.Item, ItemId = 35833, Price = 100, Child=true, AddedInVersion = "1.0.0" },                                   // Gemstone Voucher

            new() { Type = CurrencyType.Item, ItemId = 33913, Threshold = 2500, MaxCount = 4000, Children=[12839], AddedInVersion = "1.0.0" }, // Purple Crafters' Scrip
            new() { Type = CurrencyType.Item, ItemId = 12839, Price = 25, Child=true, AddedInVersion = "1.0.0" },
            new() { Type = CurrencyType.Item, ItemId = 41784, Threshold = 2500, MaxCount = 4000, AddedInVersion = "1.0.0" },                  // Orange Crafters' Scrip
            new() { Type = CurrencyType.Item, ItemId = 33914, Threshold = 2500, MaxCount = 4000, AddedInVersion = "1.0.0" },                  // Purple Gatherers' Scrip
            new() { Type = CurrencyType.Item, ItemId = 41785, Threshold = 2500, MaxCount = 4000, Children=[41807], AddedInVersion = "1.0.0" }, // Orange Gatherers' Scrip
            new() { Type = CurrencyType.Item, ItemId = 41807, Price = 1000, Child=true, AddedInVersion = "1.0.0" },                            // Gemstone Voucher
            new() { Type = CurrencyType.Item, ItemId = 28063, Threshold = 7500, MaxCount = 10000, AddedInVersion = "1.0.0" },                  // Skybuilders scripts

            new() { Type = CurrencyType.Item, ItemId = 37549, Threshold = 9999999, MaxCount = 9999999, AddedInVersion = "1.1.2", NeedsPresence = true }, // Seafarer's Cowrie
            new() { Type = CurrencyType.Item, ItemId = 37550, Threshold = 9999999, MaxCount = 9999999, AddedInVersion = "1.1.2", NeedsPresence = true }, // Islander's Cowrie

            new() { Type = CurrencyType.Item, ItemId = 45690, Threshold = 25000, MaxCount = 30000, AddedInVersion = "1.2.2", NeedsPresence = true }, // Cosmocredit
            new() { Type = CurrencyType.Item, ItemId = 45691, Threshold = 8000, MaxCount = 10000, AddedInVersion = "1.2.4", NeedsPresence = true },  // Lunar Credit
            new() { Type = CurrencyType.Item, ItemId = 48146, Threshold = 8000, MaxCount = 10000, AddedInVersion = "1.2.4", NeedsPresence = true },  // Phaenna Credit
            new() { Type = CurrencyType.Item, ItemId = 48147, Threshold = 8000, MaxCount = 10000, AddedInVersion = "1.2.7", NeedsPresence = true },  // Oizys Credit
            new() { Type = CurrencyType.Item, ItemId = 48148, Threshold = 8000, MaxCount = 10000, AddedInVersion = "1.2.7", NeedsPresence = true },  // Auxesia Credit
            
            // Societial currencies
            new() { Type = CurrencyType.Item, ItemId = 21076, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Steel Amalj'ok
            new() { Type = CurrencyType.Item, ItemId = 21075, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Sylphic Goldleaf
            new() { Type = CurrencyType.Item, ItemId = 21078, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Titan Cobaltpiece
            new() { Type = CurrencyType.Item, ItemId = 21077, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Rainbowtide Psashp
            new() { Type = CurrencyType.Item, ItemId = 21073, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Ixali Oaknot
            
            new() { Type = CurrencyType.Item, ItemId = 21074, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Vanu Whitebone
            new() { Type = CurrencyType.Item, ItemId = 21079, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Black Copper Gil
            new() { Type = CurrencyType.Item, ItemId = 21080, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Carved Kupo Nut
            
            new() { Type = CurrencyType.Item, ItemId = 21081, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Kojin Sango
            new() { Type = CurrencyType.Item, ItemId = 21935, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Ananta Dreamstaff
            new() { Type = CurrencyType.Item, ItemId = 22525, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Namazu Koban
            
            new() { Type = CurrencyType.Item, ItemId = 28186, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Fae Fancy
            new() { Type = CurrencyType.Item, ItemId = 28187, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Qitari Compliment
            new() { Type = CurrencyType.Item, ItemId = 28188, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Hammered Frogment
            
            new() { Type = CurrencyType.Item, ItemId = 36657, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Arkasodara Pana
            new() { Type = CurrencyType.Item, ItemId = 37854, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Omicron Omnitoken
            new() { Type = CurrencyType.Item, ItemId = 38952, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Loporrit Carat
            
            new() { Type = CurrencyType.Item, ItemId = 44472, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Pelu Pelplume
            new() { Type = CurrencyType.Item, ItemId = 48084, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Mamool Ja Nanook
            new() { Type = CurrencyType.Item, ItemId = 46178, Threshold = 900, MaxCount = 999, Society = true, AddedInVersion = "1.3.0" }, // Yok Huy Ward
        };
    }
}
