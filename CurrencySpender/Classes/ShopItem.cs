using System.Text.Json.Serialization;
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
    Currency = 16,          // 2^4
}
public unsafe class ShopItem
{
    public ItemType Type { get; set; }
    public uint Id { get; set; }
    [JsonIgnore] public string Name => Service.DataManager.GetExcelSheet<Item>()!.GetRow(Id).Name.ExtractText() ?? "Unable to read name";
    public uint Category { get; set; }
    public uint Price { get; set; }
    public uint Currency { get; set; }
    public uint ShopId { get; set; }
    public required Shop Shop { get; set; }

    public bool Disabled = false;
    public uint? RequiredRank;

    public override string ToString()
    {
        var cur_name = Service.DataManager.GetExcelSheet<Item>().GetRow(Currency).Name.ExtractText();
        return $"Id: {Id}, Name: {Name}, Category: {Category}, Type: ({FormatFlags(Type)}), Price: {Price}, Currency: {cur_name}, ShopId: {ShopId}";
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
    public uint AmountCanBuy => (uint)Math.Floor((double)C.Currencies.First(cur => cur.ItemId == Currency).CurrentCount / Price);
    public uint Profit { get; set; }
    public uint HasSoldWeek { get; set; }
}

