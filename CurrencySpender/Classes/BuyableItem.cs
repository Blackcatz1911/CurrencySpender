using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Client.Game;
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
    private String? whereToBuy;

    public required ItemType Type { get; init; }

    public uint ItemId { get; init; }
    public uint C_ID { get; init; }
    public uint Price { get; set; }

    [JsonIgnore] public string Name => Service.DataManager.GetExcelSheet<Item>()!.GetRow(ItemId).Name.ExtractText() ?? "Unable to read name";
    [JsonIgnore] public uint CurrentPrice { get; set; }
    [JsonIgnore] public uint AmountCanBuy { get; set; }
    [JsonIgnore] public uint Profit { get; set; }
    [JsonIgnore] public uint HasSoldWeek { get; set; }
    public uint Loc { get; set; }
}
