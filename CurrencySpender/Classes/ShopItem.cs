using System.Text.Json.Serialization;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.Exd;
using Lumina.Excel.Sheets;

namespace CurrencySpender.Classes;

[Flags]
public enum ItemType
{
    None = 0,
    Tradeable = 1,       // 2^0
    Sellable = 2,        // 2^1
    Collectable = 4,     // 2^2
    Venture = 8,         // 2^3
    Currency = 16,       // 2^4
}
public enum CollectableType
{
    None,
    Mount,
    Minion,
    Scroll,
    Emote,
    Hairstyle,
    Barding,
    RidingMap,
    Facewear,
    FramersKit,
    TTCard,
    Mahjong,
    Container,
    MasterRecipes,
    FashionAccessory
}
public unsafe class ShopItem
{
    public ItemType Type { get; set; }
    public CollectableType CollectableType { get; set; }
    public uint Id { get; set; }
    [JsonIgnore] public string Name => Service.DataManager.GetExcelSheet<Item>()!.GetRow(Id).Name.ExtractText() ?? "Unable to read name";
    public uint Category { get; set; }
    public uint Price { get; set; }
    public Boolean Gamba = false;
    public uint Currency { get; set; }
    public uint ShopId { get; set; }
    public required Shop Shop { get; set; }

    public bool Disabled = false;
    public uint? RequiredRank;
    public uint? RequiredReputation;
    public bool PreReq = false;

    public List<int>? ContainerUnlocks { get; set; }

    public override string ToString()
    {
        var curName = Service.DataManager.GetExcelSheet<Item>().GetRow(Currency).Name.ExtractText();
        return $"Id: {Id}, Name: {Name}, Category: {Category}, Type: ({FormatFlags(Type)}), Price: {Price}, Currency: {curName}, ShopId: {ShopId}, Gamba: {Gamba}, Disabled: {Disabled}";
    }

    private string FormatFlags(ItemType type)
    {
        var flags = Enum.GetValues(typeof(ItemType))
            .Cast<ItemType>()
            .Where(flag => type.HasFlag(flag) && flag != ItemType.None);

        return string.Join(", ", flags);
    }
    public uint CurrentPrice { get; set; }
    public uint LastChecked { get; set; }
    public uint AmountCanBuy => (uint)Math.Floor((double)P.Currencies.First(cur => cur.ItemId == Currency).CurrentCount / Price);
    public uint Profit { get; set; }
    public float GilPerCur { get; set; }
    public uint HasSoldWeek { get; set; }
}

