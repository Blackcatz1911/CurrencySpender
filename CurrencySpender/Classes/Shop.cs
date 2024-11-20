using System.Reflection.Emit;
using System.Text.Json.Serialization;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace CurrencySpender.Classes;

public enum ShopType
{
    FccShop,
    GCShop,
    GilShop,
    SpecialShop
}

public unsafe class Shop
{
    public required ShopType Type { get; set; }

    private static readonly Dictionary<ShopType, Func<IExcelSheet>> ShopSheetResolvers = new()
    {
        { ShopType.FccShop, () => Service.DataManager.GetExcelSheet<FccShop>() },
        { ShopType.GCShop, () => Service.DataManager.GetExcelSheet<GCShop>() },
        { ShopType.GilShop, () => Service.DataManager.GetExcelSheet<GilShop>() },
        { ShopType.SpecialShop, () => Service.DataManager.GetExcelSheet<SpecialShop>() }
    };

        // Dynamically resolve and store the sheet
    public IExcelSheet Sheet { get; private set; }

    public void SetSheet(ShopType type)
    {
        if (ShopSheetResolvers.TryGetValue(type, out var resolver))
        {
            Sheet = resolver.Invoke();
        }
        else
        {
            throw new InvalidOperationException("Invalid shop type");
        }
    }
    public uint ShopId { get; set; }
    [JsonIgnore]
    public string ShopName
    {
        get
        {
            switch (Type)
            {
                case ShopType.FccShop:
                    return Service.DataManager.GetExcelSheet<FccShop>()!.GetRow(ShopId).Name.ExtractText() ?? "Unable to read name";

                case ShopType.GCShop:
                    return "GCShop";

                case ShopType.GilShop:
                    return Service.DataManager.GetExcelSheet<GilShop>()!.GetRow(ShopId).Name.ExtractText() ?? "Unable to read name";

                case ShopType.SpecialShop:
                    return Service.DataManager.GetExcelSheet<SpecialShop>()!.GetRow(ShopId).Name.ExtractText() ?? "Unable to read name";

                default:
                    return "Unable to read name";
            }
        }
    }
    public uint NpcId { get; set; }
    [JsonIgnore] public string NpcName => Service.DataManager.GetExcelSheet<ENpcResident>()!.GetRow(NpcId).Singular.ExtractText() ?? "Unable to read name";
}

