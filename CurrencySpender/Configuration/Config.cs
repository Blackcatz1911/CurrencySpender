using CurrencySpender.Classes;
using ECommons.Configuration;

namespace CurrencySpender.Configuration;

[Serializable]
public class Config: IEzConfig
{
    public string Version { get; set; } = "0.0.0.0";

    public List<TrackedCurrency> Currencies = [];
    public int Seperator = 0;
    //public Dictionary<uint, uint> FateRanks = [];
    //public List<BuyableItem> Items = [];

    public bool ShowVentures = true;

    public bool Debug = false;
}
