using Lumina.Excel.Sheets;
using System.Reflection;

namespace CurrencySpender.Classes;

public enum ShopType
{
    FccShop,
    GCShop,
    GilShop,
    SpecialShop,
    FateShop,
    EmptyShop
}

public unsafe class Shop
{
    public required ShopType Type { get; set; }
    public uint ShopId { get; set; }
    public uint NpcId { get; set; }
    public string NpcName
    {
        get
        {
            string name = "";
            try
            {
                name = Service.DataManager.GetExcelSheet<ENpcResident>().GetRow(NpcId).Singular.ExtractText();
            }
            catch
            {
                name = "Unknown";
            }
            return name;
        }
    }// => Service.DataManager.GetExcelSheet<ENpcResident>().GetRow(NpcId).Singular.ExtractText() ?? "Unknown";
    public required Location Location { get; init; }

    public uint Currency;
    public uint? GC;
    public uint? RequiredLevel;
    public uint? CurrentLevel;
    public bool Disabled = false;
    public List<ShopItem> Items = new List<ShopItem>();
    public int ItemCount => Items.Count;
    public override string ToString()
    {
        var properties = GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Select(prop => $"{prop.Name}={prop.GetValue(this)}");

        var fields = GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(field => !field.Name.StartsWith("<")) // Exclude backing fields
            .Select(field => $"{field.Name}={field.GetValue(this)}");

        return $"{GetType().Name}: {string.Join(", ", properties.Concat(fields))}";
    }
}

