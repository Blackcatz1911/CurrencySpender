using System.Text.Json.Serialization;
using Lumina.Excel.Sheets;

namespace CurrencySpender.Classes;

public enum ItemType
{
    Sellable,
    Collectable,
    Venture
}

public unsafe class BuyableItem
{
    public required ItemType Type { get; init; }

    public uint ItemId { get; init; }
    public uint C_ID { get; init; }
    public uint Price { get; set; }

    [JsonIgnore] public string Name => Service.DataManager.GetExcelSheet<Item>()!.GetRow(ItemId).Name.ExtractText() ?? "Unable to read name";
    [JsonIgnore] public uint CurrentPrice { get; set; }
    [JsonIgnore] public uint LastChecked { get; set; }
    [JsonIgnore] public uint AmountCanBuy { get; set; }
    [JsonIgnore] public uint Profit { get; set; }
    [JsonIgnore] public uint HasSoldWeek { get; set; }
    public uint Loc { get; set; }
}
